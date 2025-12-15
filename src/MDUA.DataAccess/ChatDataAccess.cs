using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Framework;
using MDUA.Framework.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using static MDUA.Entities.ChatSession;

namespace MDUA.DataAccess
{
    public partial class ChatDataAccess : BaseDataAccess, IChatDataAccess
    {
        public ChatDataAccess(IConfiguration configuration) : base(configuration)
        {
        }

        // -----------------------------------------------------------
        // 1. SESSION MANAGEMENT
        // -----------------------------------------------------------

        public ChatSession CreateOrGetSession(ChatSession session)
        {
            // First, try to find if the session already exists
            ChatSession existingSession = null;

            if (session.UserLoginId.HasValue && session.UserLoginId > 0)
                existingSession = GetSessionByUserId(session.UserLoginId.Value);
            else if (session.SessionGuid != Guid.Empty)
                existingSession = GetSessionByGuid(session.SessionGuid);

            if (existingSession != null)
                return existingSession;

            // If not exists, create new using the SP you provided
            using (SqlCommand cmd = GetSPCommand("InsertChatSession"))
            {
                // Output Parameter for ID
                SqlParameter pId = pInt32Out("Id");
                AddParameter(cmd, pId);

                // Input Parameters
                AddParameter(cmd, pGuid("SessionGuid", session.SessionGuid));
                AddParameter(cmd, pInt32("UserLoginId", session.UserLoginId));
                AddParameter(cmd, pNVarChar("GuestName", session.GuestName));
                AddParameter(cmd, pNVarChar("Status", string.IsNullOrEmpty(session.Status) ? "New" : session.Status));
                AddParameter(cmd, pDateTime("StartedAt", DateTime.Now));
                AddParameter(cmd, pDateTime("LastMessageAt", DateTime.Now));
                AddParameter(cmd, pBool("IsActive", true));

                // Execute
                ExecuteCommand(cmd);

                // Retrieve the new ID
                session.Id = (int)pId.Value;
            }

            return session;
        }

        public ChatSession GetSessionByGuid(Guid sessionGuid)
        {
            string query = "SELECT * FROM ChatSession WHERE SessionGuid = @SessionGuid";
            using (SqlCommand cmd = GetSQLCommand(query))
            {
                AddParameter(cmd, pGuid("SessionGuid", sessionGuid));
                return GetObject(cmd); // Assumes GetObject is a helper in BaseDataAccess or we implement filler manually
            }
        }

        public ChatSession GetSessionByUserId(int userId)
        {
            // Fetches the most recent open session for a user
            string query = "SELECT TOP 1 * FROM ChatSession WHERE UserLoginId = @UserId AND IsActive = 1 ORDER BY LastMessageAt DESC";
            using (SqlCommand cmd = GetSQLCommand(query))
            {
                AddParameter(cmd, pInt32("UserId", userId));
                return GetObject(cmd);
            }
        }

        // In MDUA.DataAccess/ChatDataAccess.cs

        public List<ChatSession> GetActiveSessions()
        {
            // Updated Query: Includes a subquery to count unread messages from the Guest
            string query = @"
        SELECT 
            s.*,
            (SELECT COUNT(*) FROM ChatMessage m 
             WHERE m.ChatSessionId = s.Id 
             AND m.IsRead = 0 
             AND m.IsFromAdmin = 0) AS UnreadCount
        FROM ChatSession s 
        WHERE s.IsActive = 1 
        AND s.Status != 'Closed' 
        ORDER BY 
            CASE WHEN (SELECT COUNT(*) FROM ChatMessage m WHERE m.ChatSessionId = s.Id AND m.IsRead = 0 AND m.IsFromAdmin = 0) > 0 THEN 0 ELSE 1 END, 
            s.LastMessageAt DESC";

            using (SqlCommand cmd = GetSQLCommand(query))
            {
                // We use a custom filler here because we added the extra 'UnreadCount' column
                SqlDataReader reader;
                SelectRecords(cmd, out reader);

                var list = new List<ChatSession>();
                using (reader)
                {
                    while (reader.Read())
                    {
                        var session = new ChatSession();
                        // Call your existing helper
                        FillChatSession(session, reader);

                        // Manually fill the new property
                        session.UnreadCount = reader.GetInt32(reader.GetOrdinal("UnreadCount"));

                        list.Add(session);
                    }
                }
                return list;
            }
        }

        // -----------------------------------------------------------
        // 2. MESSAGE MANAGEMENT
        // -----------------------------------------------------------

        public long SaveMessage(ChatMessage message)
        {
            // 👇 UPDATED QUERY: 
            // This now updates the 'GuestName' in the ChatSession table 
            // whenever a message is received from the user (IsFromAdmin = 0).
            string query = @"
                INSERT INTO ChatMessage 
                (ChatSessionId, SenderId, SenderName, MessageText, IsFromAdmin, IsRead, SentAt)
                VALUES 
                (@ChatSessionId, @SenderId, @SenderName, @MessageText, @IsFromAdmin, 0, GETDATE());
                
                -- Update the parent session's timestamp AND GuestName
                UPDATE ChatSession 
                SET 
                    LastMessageAt = GETDATE(), 
                    Status = 'Active',
                    GuestName = CASE 
                        -- Only update name if message is from User AND name is not empty
                        WHEN @IsFromAdmin = 0 AND @SenderName IS NOT NULL AND LEN(@SenderName) > 0 
                        THEN @SenderName 
                        ELSE GuestName 
                    END
                WHERE Id = @ChatSessionId;

                SELECT CAST(SCOPE_IDENTITY() as bigint);";

            using (SqlCommand cmd = GetSQLCommand(query))
            {
                AddParameter(cmd, pInt32("ChatSessionId", message.ChatSessionId));
                AddParameter(cmd, pInt32("SenderId", message.SenderId));
                AddParameter(cmd, pNVarChar("SenderName", message.SenderName));
                AddParameter(cmd, pNVarChar("MessageText", message.MessageText));
                AddParameter(cmd, pBool("IsFromAdmin", message.IsFromAdmin));

                return (long)SelectScaler(cmd);
            }
        }
        public List<ChatMessage> GetMessagesBySessionId(int sessionId)
        {
            string query = "SELECT * FROM ChatMessage WHERE ChatSessionId = @ChatSessionId ORDER BY SentAt ASC";
            using (SqlCommand cmd = GetSQLCommand(query))
            {
                AddParameter(cmd, pInt32("ChatSessionId", sessionId));

                // Manual List Filling to ensure independence if framework helpers aren't generic enough
                List<ChatMessage> list = new List<ChatMessage>();
                SqlDataReader reader;
                SelectRecords(cmd, out reader); // Uses your BaseDataAccess method

                using (reader)
                {
                    while (reader.Read())
                    {
                        var msg = new ChatMessage
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ChatSessionId = reader.GetInt32(reader.GetOrdinal("ChatSessionId")),
                            SenderName = reader["SenderName"].ToString(),
                            MessageText = reader["MessageText"].ToString(),
                            IsFromAdmin = reader.GetBoolean(reader.GetOrdinal("IsFromAdmin")),
                            IsRead = reader.GetBoolean(reader.GetOrdinal("IsRead")),
                            SentAt = reader.GetDateTime(reader.GetOrdinal("SentAt"))
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("SenderId")))
                            msg.SenderId = reader.GetInt32(reader.GetOrdinal("SenderId"));

                        list.Add(msg);
                    }
                }
                return list;
            }
        }

        public void MarkMessagesAsRead(int sessionId, bool messagesFromAdmin)
        {
            // If I am Admin, I want to mark User messages as read (IsFromAdmin = 0)
            // If I am User, I want to mark Admin messages as read (IsFromAdmin = 1)

            string query = @"
                UPDATE ChatMessage 
                SET IsRead = 1 
                WHERE ChatSessionId = @SessionId 
                AND IsFromAdmin = @TargetIsAdmin 
                AND IsRead = 0";

            using (SqlCommand cmd = GetSQLCommand(query))
            {
                AddParameter(cmd, pInt32("SessionId", sessionId));
                AddParameter(cmd, pBool("TargetIsAdmin", messagesFromAdmin));
                ExecuteCommand(cmd);
            }
        }

        // -----------------------------------------------------------
        // 3. HELPER METHODS (MAPPING)
        // -----------------------------------------------------------

        /// <summary>
        /// Helper to map a single ChatSession from a DataReader (Matches your pattern in New Text Document.txt)
        /// </summary>
        private ChatSession GetObject(SqlCommand cmd)
        {
            SqlDataReader reader;
            SelectRecord(cmd, out reader); // Assumes BaseDataAccess has SelectRecord (singular)

            using (reader)
            {
                if (reader.Read())
                {
                    var session = new ChatSession();
                    FillChatSession(session, reader);
                    return session;
                }
            }
            return null;
        }

        private List<ChatSession> GetList(SqlCommand cmd)
        {
            SqlDataReader reader;
            SelectRecords(cmd, out reader);

            var list = new List<ChatSession>();
            using (reader)
            {
                while (reader.Read())
                {
                    var session = new ChatSession();
                    FillChatSession(session, reader);
                    list.Add(session);
                }
            }
            return list;
        }

        private void FillChatSession(ChatSession session, SqlDataReader reader)
        {
            session.Id = reader.GetInt32(reader.GetOrdinal("Id"));
            session.SessionGuid = reader.GetGuid(reader.GetOrdinal("SessionGuid"));
            session.Status = reader["Status"].ToString();
            session.StartedAt = reader.GetDateTime(reader.GetOrdinal("StartedAt"));
            session.LastMessageAt = reader.GetDateTime(reader.GetOrdinal("LastMessageAt"));
            session.IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
            session.GuestName = reader["GuestName"] as string;

            if (!reader.IsDBNull(reader.GetOrdinal("UserLoginId")))
                session.UserLoginId = reader.GetInt32(reader.GetOrdinal("UserLoginId"));
        }
    }
}