using MDUA.Entities;
using System;
using System.Collections.Generic;

namespace MDUA.Facade.Interface
{
    public interface IChatFacade : IDisposable
    {
        /// Initializes a session for a Guest (Non-logged in user).
        /// If a session exists for this GUID, it returns it. Otherwise, it creates a new one.
        ChatSession InitGuestSession(Guid sessionGuid);

        /// Initializes a session for a Registered User (Logged in).
        /// If an active session exists, it returns it. Otherwise, creates a new one.
        ChatSession InitUserSession(int userId, string userName);

        /// Saves a message to the database.
        long SendMessage(ChatMessage message);

        /// Retrieves the full conversation history for a specific session.
        List<ChatMessage> GetChatHistory(int sessionId);
        void UpdateSessionStatus(int sessionId, string newStatus);


        /// For Admins: Gets a list of all currently active/open chat sessions.

        List<ChatSession> GetActiveSessionsForAdmin();

        /// Marks messages as read.
        /// If readerIsAdmin = true, it marks user messages as read.
        /// If readerIsAdmin = false, it marks admin messages as read.
        void MarkMessagesAsRead(int sessionId, bool readerIsAdmin);
        ChatSession GetSessionByGuid(Guid sessionGuid);
        ChatSession GetSessionById(int sessionId); //-- Add This Line
    }
}