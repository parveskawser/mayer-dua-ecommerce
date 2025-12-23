using Microsoft.AspNetCore.SignalR;
using MDUA.Entities;
using MDUA.Facade.Interface;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MDUA.Web.UI.Hubs
{
    public class SupportHub : Hub
    {
        private readonly IChatFacade _chatFacade;

        public SupportHub(IChatFacade chatFacade)
        {
            _chatFacade = chatFacade;
        }

        // 1. GUEST SENDS MESSAGE
        public async Task SendMessageToAdmin(string user, string message, string sessionGuidString)
        {
            if (!Guid.TryParse(sessionGuidString, out Guid sessionGuid)) return;
            string groupName = sessionGuidString.ToLower();

            // Init session (also handles name update in facade if implemented)
            var session = _chatFacade.InitGuestSession(sessionGuid);

            var chatMsg = new ChatMessage
            {
                ChatSessionId = session.Id,
                SenderName = user,
                MessageText = message,
                IsFromAdmin = false,
                SentAt = DateTime.Now
            };
            _chatFacade.SendMessage(chatMsg);

            await Clients.Group("Admins").SendAsync("ReceiveMessage", user, message, groupName);
        }

        // 2. ADMIN JOINS A SESSION (Notification)
        public async Task AdminJoinSession(string adminName, string targetSessionGuidString)
        {
            if (string.IsNullOrEmpty(targetSessionGuidString)) return;
            string targetGroup = targetSessionGuidString.ToLower();

            // Notify Guest
            await Clients.Group(targetGroup).SendAsync("ReceiveSystemMessage", $"{adminName} has joined the chat.");
        }

        public async Task SendReplyToUser(string clientProvidedName, string message, string targetSessionGuidString)
        {
            // 1. Security Check
            var user = Context.GetHttpContext().User;
            if (!user.Identity.IsAuthenticated)
                throw new HubException("Unauthorized");

            if (!Guid.TryParse(targetSessionGuidString, out Guid sessionGuid)) return;
            string targetGroup = targetSessionGuidString.ToLower();

            // 2. Get Real Name from Server Context (Secure)
            string actualSenderName = user.Identity.Name ?? clientProvidedName ?? "Support";

            // Get User ID if available
            int? senderId = null;
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("sub");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int uid))
                senderId = uid;

            var session = _chatFacade.InitGuestSession(sessionGuid);

            var chatMsg = new ChatMessage
            {
                ChatSessionId = session.Id,
                SenderId = senderId,
                SenderName = actualSenderName, // Saves "Inaya" to DB
                MessageText = message,
                IsFromAdmin = true,
                SentAt = DateTime.Now
            };

            // 3. Save
            _chatFacade.SendMessage(chatMsg);

            // 4. Broadcast to Customer
            // IMPORTANT: Sending "ReceiveReply" allows customer JS to distinguish admin messages
            await Clients.Group(targetGroup).SendAsync("ReceiveReply", actualSenderName, message);
        }



        public override async Task OnConnectedAsync()
        {
            var user = Context.GetHttpContext().User;

            // 1. If user is Admin, join the "Admins" channel (to see ALL chats)
            if (user.Identity.IsAuthenticated && (user.HasClaim(c => c.Value == "Chat.View") || user.HasClaim(c => c.Value == "Order.View")))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            }

            // 2. ALWAYS join the specific Session Group if a sessionId is provided.
            // This fixes the issue: Even if you are Admin, you need to listen to your own frontend chat window.
            string sessionId = Context.GetHttpContext().Request.Query["sessionId"];
            if (!string.IsNullOrEmpty(sessionId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, sessionId.ToLower());
            }

            await base.OnConnectedAsync();
        }
    }
}