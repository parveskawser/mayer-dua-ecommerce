using MDUA.Entities;
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

        public ChatController(
            IChatFacade chatFacade,
            IAiChatService aiChatService,
            IHubContext<SupportHub> hubContext)
        {
            _chatFacade = chatFacade;
            _aiChatService = aiChatService;
            _hubContext = hubContext;
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
                {
                    session = _chatFacade.InitGuestSession(guid);
                }
                else if (request.ChatSessionId > 0)
                {
                    session = _chatFacade.GetSessionById(request.ChatSessionId);
                }

                if (session == null)
                    return BadRequest("Invalid session.");

                // 2. Create User Message
                var message = new ChatMessage
                {
                    ChatSessionId = session.Id,
                    SenderName = request.SenderName ?? "Guest",
                    MessageText = request.MessageText,
                    IsFromAdmin = false,
                    IsRead = false,
                    SenderType = "Customer",
                    SentAt = DateTime.UtcNow
                };

                // 3. Save User Message
                _chatFacade.SendMessage(message);

                // 4. Broadcast to Admins
                await _hubContext.Clients.Group("Admins").SendAsync(
                    "ReceiveMessage",
                    message.SenderName,
                    message.MessageText,
                    session.SessionGuid.ToString().ToLower());

                // 5. AI BOT LOGIC
                if (session.Status == "New" || session.Status == "BotActive")
                {
                    if (ContainsHandoffKeyword(request.MessageText))
                    {
                        await TransferToHuman(session, "Customer requested human agent");

                        await SendBotReply(
                            session,
                            "👋 I've notified a support agent. Please hold on — a human will join shortly."
                        );

                        return Ok(new { success = true, handedOff = true });
                    }

                    var history = _chatFacade.GetChatHistory(session.Id)
                                             .OrderByDescending(m => m.SentAt)
                                             .Take(10)
                                             .OrderBy(m => m.SentAt)
                                             .Select(m => $"{m.SenderName}: {m.MessageText}")
                                             .ToList();

                    string aiResponse = await _aiChatService.GetResponseAsync(
                        request.MessageText,
                        history
                    );

                    if (ContainsHandoffTrigger(aiResponse))
                    {
                        await TransferToHuman(session, "AI unable to assist");
                    }

                    if (session.Status == "New")
                    {
                        session.Status = "BotActive";
                        _chatFacade.UpdateSessionStatus(session.Id, "BotActive");
                    }

                    // Use the helper method
                    await SendBotReply(session, aiResponse);



                }






                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
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