using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Facade.Interface;
using System;
using System.Collections.Generic;

namespace MDUA.Facade
{
    public class ChatFacade : IChatFacade
    {
        private readonly IChatDataAccess _chatDataAccess;

        public ChatFacade(IChatDataAccess chatDataAccess)
        {
            _chatDataAccess = chatDataAccess;
        }

        #region Session Management

        public ChatSession InitGuestSession(Guid sessionGuid)
        {
            if (sessionGuid == Guid.Empty)
                throw new ArgumentException("Session GUID cannot be empty for guests.");

            var session = new ChatSession
            {
                SessionGuid = sessionGuid,
                UserLoginId = null,
                GuestName = "Guest-" + sessionGuid.ToString().Substring(0, 4), // Default name e.g. Guest-A1B2
                Status = "New",
                IsActive = true
            };

            return _chatDataAccess.CreateOrGetSession(session);
        }

        public ChatSession InitUserSession(int userId, string userName)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid User ID.");

            var session = new ChatSession
            {
                SessionGuid = Guid.NewGuid(), // Generate a new GUID for internal tracking
                UserLoginId = userId,
                GuestName = userName, // We store the actual name here for easier display
                Status = "New",
                IsActive = true
            };

            return _chatDataAccess.CreateOrGetSession(session);
        }

        public List<ChatSession> GetActiveSessionsForAdmin()
        {
            return _chatDataAccess.GetActiveSessions();
        }

        #endregion

        #region Message Handling

        public long SendMessage(ChatMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (string.IsNullOrWhiteSpace(message.MessageText))
                throw new ArgumentException("Message text cannot be empty.");

            // 1. Ensure Session ID is valid
            if (message.ChatSessionId <= 0)
            {
                // Fallback: If for some reason SessionId is missing but we have GUID/UserId, 
                // we could try to recover the session here. 
                // For now, we assume the UI/Hub passes a valid SessionId.
                throw new InvalidOperationException("Cannot send message without a valid Chat Session ID.");
            }

            // 2. Set Defaults
            if (message.SentAt == DateTime.MinValue)
                message.SentAt = DateTime.Now;

            // 3. Save via Data Access
            return _chatDataAccess.SaveMessage(message);
        }

        public List<ChatMessage> GetChatHistory(int sessionId)
        {
            return _chatDataAccess.GetMessagesBySessionId(sessionId);
        }

        public void MarkMessagesAsRead(int sessionId, bool readerIsAdmin)
        {
            // If the Reader is Admin, they are reading the User's messages (IsFromAdmin = false)
            // If the Reader is Customer, they are reading Admin's messages (IsFromAdmin = true)
            // The 'messagesFromAdmin' parameter in DA expects "Whose messages are we marking as read?"

            // So if ReaderIsAdmin = true, we mark messages where IsFromAdmin = false (TargetIsAdmin = false)
            bool targetIsAdminMessages = !readerIsAdmin;

            _chatDataAccess.MarkMessagesAsRead(sessionId, targetIsAdminMessages);
        }

        public ChatSession GetSessionByGuid(Guid sessionGuid)
        {
            return _chatDataAccess.GetSessionByGuid(sessionGuid);
        }
        #endregion

        public void Dispose()
        {
            _chatDataAccess?.Dispose();
        }
    }
}