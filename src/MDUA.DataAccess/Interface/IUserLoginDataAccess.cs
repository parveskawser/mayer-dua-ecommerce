using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess.Interface
{
    /// <summary>
    /// ICommonDataAccess Provides a generic contract for data access operations 
    /// (CRUD and utility methods) that can be implemented for any entity type.
    /// 
    /// This interface defines common patterns such as:
    /// - Creating new records (Insert)
    /// - Reading single or multiple records (Get, GetAll, GetByQuery, GetPaged)
    /// - Updating existing records (Update)
    /// - Deleting records (Delete)
    /// - Utility operations like retrieving maximum ID and row count
    /// 
    /// By using generics (<typeparamref name="T"/>, <typeparamref name="L"/>, <typeparamref name="B"/>),
    /// it ensures reusability across multiple entities while maintaining 
    /// type safety and consistency.
    /// </summary>
    /// <typeparam name="T">Represents the entity type for a single record.</typeparam>
    /// <typeparam name="L">Represents the collection type for multiple records (e.g., a list).</typeparam>
    /// <typeparam name="B">Represents the base type used for insert and update operations (e.g., DTO or base entity).</typeparam>


    public interface IUserLoginDataAccess : ICommonDataAccess<UserLogin, UserLoginList, UserLoginBase>
    {
        public UserLogin GetUserLogin(string email, string password);

        void EnableTwoFactor(int userId, string secret);

        void InvalidateAllSessionsByUser(int userId);

        string GetTwoFactorSecretByUserId(int userId);

        void DisableTwoFactor(int userId);
        void InvalidateAllSessions(int userId);
        UserLogin GetByUsername(string username);

        UserPasskeyResult GetPasskeyByCredentialId(byte[] credentialId);
        void UpdatePasskeyCounter(int id, uint counter);
        void AddUserPasskey(UserPasskey passkey);
        List<UserPasskey> GetPasskeysByUserId(int userId);
        UserLogin GetByEmail(string email);

    }
}
