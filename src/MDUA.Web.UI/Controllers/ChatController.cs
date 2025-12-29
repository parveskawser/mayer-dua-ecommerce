using MDUA.Entities;
using MDUA.Facade;
using MDUA.Facade.Interface;
using MDUA.Web.UI.Hubs; // Ensure this namespace matches your Hub location
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDUA.Web.UI.Controllers
{
    [Authorize]
    public class ChatController : BaseController
    {
        private readonly IChatFacade _chatFacade;
        private readonly IAiChatService _aiChatService;
        private readonly IHubContext<SupportHub> _hubContext;
        public readonly IProductFacade _productFacade; // Add this line
        public ChatController(
            IChatFacade chatFacade,
            IAiChatService aiChatService,
            IHubContext<SupportHub> hubContext,
            IProductFacade productFacade)
        {
            _chatFacade = chatFacade;
            _aiChatService = aiChatService;
            _hubContext = hubContext;
            _productFacade = productFacade;

        }

        // ====================================================================
        //  REAL-WORLD MESSAGE HANDLING (User -> DB -> SignalR -> AI -> DB -> SignalR)
        // ====================================================================



        // ====================================================================
        //  EXISTING GET METHODS (Cleaned & Optimized)
        // ====================================================================

        [HttpGet]
        [Route("chat/guest-history")]
        [AllowAnonymous]
        public IActionResult GetGuestHistory(string sessionGuid)
        {
            if (!Guid.TryParse(sessionGuid, out Guid guid)) return BadRequest();

            // 1. Find the session
            var session = _chatFacade.GetSessionByGuid(guid);

            // 2. Graceful fallback for new users
            if (session == null)
            {
                return Ok(new List<object>());
            }

            // 3. Get messages
            var history = _chatFacade.GetChatHistory(session.Id);
            return Json(history);
        }

        [HttpGet]
        [Route("chat/active-sessions")]
        public IActionResult GetActiveSessions()
        {
            if (!HasPermission("Chat.View")) return Unauthorized();

            var sessions = _chatFacade.GetActiveSessionsForAdmin();
            return Json(sessions);
        }
        // 🆕 HELPER: Save & Send Bot Message
        private async Task SendBotReply(ChatSession session, string messageText)
        {
            // 1. Create the message object
            var botMessage = new ChatMessage
            {
                ChatSessionId = session.Id,
                SenderName = "MDUA Assistant", // Or "System"
                SenderType = "Bot",
                IsFromAdmin = true,
                MessageText = messageText,
                SentAt = DateTime.UtcNow,
                IsRead = true
            };

            // 2. Save to Database
            _chatFacade.SendMessage(botMessage);

            // 3. Send to Client via SignalR
            await _hubContext.Clients
                .Group(session.SessionGuid.ToString().ToLower())
                .SendAsync("ReceiveReply",
                    botMessage.SenderName,
                    botMessage.MessageText);
        }
        [HttpPost]
        [Route("chat/send")]
        [AllowAnonymous]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.MessageText))
                return BadRequest("Invalid message.");

            try
            {
                // 1. Resolve Session
                ChatSession session = null;
                if (!string.IsNullOrEmpty(request.SessionGuid) && Guid.TryParse(request.SessionGuid, out Guid guid))
                    session = _chatFacade.InitGuestSession(guid);

                if (session == null) return BadRequest("Invalid session.");

                // 2. Save User Message
                var message = new ChatMessage
                {
                    ChatSessionId = session.Id,
                    SenderName = request.SenderName ?? "Guest",
                    MessageText = request.MessageText,
                    SentAt = DateTime.UtcNow,
                    SenderType = "Customer"
                };
                _chatFacade.SendMessage(message);

                // 3. Get AI Response
                var history = _chatFacade.GetChatHistory(session.Id)
                                         .OrderByDescending(m => m.SentAt)
                                         .Take(10)
                                         .OrderBy(m => m.SentAt)
                                         .Select(m => $"{m.SenderName}: {m.MessageText}")
                                         .ToList();

                // ✅ PASS THE CONTEXT PRODUCT ID HERE
                string aiResponse = await _aiChatService.GetResponseAsync(
                    request.MessageText,
                    history,
                    request.ContextProductId
                );

                // 4. Save and Send Bot Reply
                await SendBotReply(session, aiResponse);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
      
        // 🆕 HUMAN HANDOFF LOGIC
        private async Task TransferToHuman(ChatSession session, string reason)
        {
            // 1. Update session status
            session.Status = "Assigned"; // This stops the bot from replying
                                         // Note: You'll need to add an Update method to ChatFacade if not present
            _chatFacade.UpdateSessionStatus(session.Id, "Assigned");

            // 2. Notify admins with URGENT flag
            string clientGroup = session.SessionGuid.ToString().ToLower();
            await _hubContext.Clients.Group("Admins").SendAsync(
                "ReceiveUrgentHandoff",
                session.GuestName,
                reason,
                clientGroup);

            // 3. Send system message to customer
            await _hubContext.Clients.Group(clientGroup).SendAsync(
                "ReceiveSystemMessage",
                "🔔 A support agent will join shortly. Please wait...");
        }

        // 🔍 Check if user wants human
        private bool ContainsHandoffKeyword(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return false;

            message = message.ToLower();

            string[] keywords =
            {
        "human",
        "agent",
        "admin",
        "support",
        "representative",
        "someone there",
        "anyone there",
        "real person",
        "talk to someone",
        "connect me",
        "can a human",
        "handover",
        "hand over"
    };

            return keywords.Any(k => message.Contains(k));
        }

        // 🔍 Check if AI triggered handoff
        private bool ContainsHandoffTrigger(string aiResponse)
        {
            var triggers = new[] {
                "support team", "human agent", "connect you"
            };

            return triggers.Any(t => aiResponse.ToLower().Contains(t));
        }
        // 🆕 Request Model
        public class ChatMessageRequest
        {
            public int ChatSessionId { get; set; }
            public string SessionGuid { get; set; }
            public string SenderName { get; set; }
            public string MessageText { get; set; }
            public int? ContextProductId { get; set; }
        }

        [HttpGet]
        [Route("chat/history")]
        public IActionResult GetHistory(int sessionId)
        {
            if (!HasPermission("Chat.View")) return Unauthorized();

            var history = _chatFacade.GetChatHistory(sessionId);

            // Mark as read immediately when Admin loads history
            bool isAdmin = true;
            _chatFacade.MarkMessagesAsRead(sessionId, isAdmin);

            return Json(history);
        }
    }
}