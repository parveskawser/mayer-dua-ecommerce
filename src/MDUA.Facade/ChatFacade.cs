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
                GuestName = "Guest-" + sessionGuid.ToString().Substring(0, 4), // Friendly Name
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
                SessionGuid = Guid.NewGuid(),
                UserLoginId = userId,
                GuestName = userName,
                Status = "New",
                IsActive = true
            };

            return _chatDataAccess.CreateOrGetSession(session);
        }

        public List<ChatSession> GetActiveSessionsForAdmin()
        {
            return _chatDataAccess.GetActiveSessions();
        }

        public ChatSession GetSessionByGuid(Guid sessionGuid)
        {
            return _chatDataAccess.GetSessionByGuid(sessionGuid);
        }
        public ChatSession GetSessionById(int sessionId)
        {
            return _chatDataAccess.GetSessionById(sessionId);
        }

        #endregion

        #region Message Handling

        public long SendMessage(ChatMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (string.IsNullOrWhiteSpace(message.MessageText))
                throw new ArgumentException("Message text cannot be empty.");

            // 1. Validation
            if (message.ChatSessionId <= 0)
            {
                throw new InvalidOperationException("Cannot send message without a valid Chat Session ID.");
            }

            // 2. Set Defaults if missing
            if (message.SentAt == DateTime.MinValue)
                message.SentAt = DateTime.UtcNow;

            // 3. Save via Data Access
            return _chatDataAccess.SaveMessage(message);
        }
        public void UpdateSessionStatus(int sessionId, string newStatus)
        {
            var session = _chatDataAccess.GetSessionById(sessionId);
            if (session != null)
            {
                session.Status = newStatus;
                session.LastMessageAt = DateTime.UtcNow;
                _chatDataAccess.UpdateSession(session);
            }
        }
        public List<ChatMessage> GetChatHistory(int sessionId)
        {
            return _chatDataAccess.GetMessagesBySessionId(sessionId);
        }

        public void MarkMessagesAsRead(int sessionId, bool readerIsAdmin)
        {
            // Logic: If Admin reads, we mark 'Customer' messages as read.
            // If Customer reads, we mark 'Admin/Bot' messages as read.

            // Param expected by DB: "Which messages should be marked read?"
            // If Reader=Admin, update messages where IsFromAdmin = false
            bool targetIsAdminMessages = !readerIsAdmin;

            _chatDataAccess.MarkMessagesAsRead(sessionId, targetIsAdminMessages);
        }

        #endregion

        public void Dispose()
        {
            _chatDataAccess?.Dispose();
        }
    }
}