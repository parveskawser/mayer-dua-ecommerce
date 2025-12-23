using MDUA.Entities;
using MDUA.Entities.List;
using MDUA.Framework.DataAccess;
using System;
using System.Collections.Generic;

namespace MDUA.DataAccess.Interface
{
    public interface IChatDataAccess : IDisposable
    {

        /// Creates a new chat session or returns the existing one based on GUID or UserID.

        ChatSession CreateOrGetSession(ChatSession session);

        void UpdateSession(ChatSession session);

        /// Retrieves a specific session by its GUID (useful for guests).

        ChatSession GetSessionByGuid(Guid sessionGuid);

        /// Retrieves a session by User ID (useful for logged-in users).

        ChatSession GetSessionByUserId(int userId);


        /// Saves a new message to the database.

        long SaveMessage(ChatMessage message);


        /// Retrieves chat history for a specific session.

        List<ChatMessage> GetMessagesBySessionId(int sessionId);


        /// Gets all active/open sessions for Admins to view.

        List<ChatSession> GetActiveSessions();


        /// Marks all messages in a session as read for a specific recipient type (Admin or Customer).

        void MarkMessagesAsRead(int sessionId, bool messagesFromAdmin);
        ChatSession GetSessionById(int sessionId);
    }
}