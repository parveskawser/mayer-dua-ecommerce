using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using MDUA.Framework;
using MDUA.Framework.DataAccess;
using MDUA.Framework.Exceptions;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess
{
    public partial class UserPasskeyDataAccess : BaseDataAccess
    {
        #region Constants
        private const string INSERTUSERPASSKEY = "InsertUserPasskey";
        private const string UPDATEUSERPASSKEY = "UpdateUserPasskey";
        private const string DELETEUSERPASSKEY = "DeleteUserPasskey";
        private const string GETUSERPASSKEYBYID = "GetUserPasskeyById";
        private const string GETUSERPASSKEYBYCREDENTIALID = "GetUserPasskeyByCredentialId";
        private const string GETUSERPASSKEYBYUSERID = "GetUserPasskeyByUserId";
        #endregion

        #region Constructors
        public UserPasskeyDataAccess(IConfiguration configuration) : base(configuration) { }
        public UserPasskeyDataAccess(ClientContext context) : base(context) { }
        public UserPasskeyDataAccess(SqlTransaction transaction) : base(transaction) { }
        public UserPasskeyDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion

        #region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="userPasskeyObject"></param>
        private void AddCommonParams(SqlCommand cmd, UserPasskeyBase userPasskeyObject)
        {
            AddParameter(cmd, pInt32(UserPasskeyBase.Property_UserId, userPasskeyObject.UserId));
            // Assuming pVarBinary exists in BaseDataAccess, otherwise use cmd.Parameters.Add
            AddParameter(cmd, pVarBinary(UserPasskeyBase.Property_CredentialId, userPasskeyObject.CredentialId));
            AddParameter(cmd, pVarBinary(UserPasskeyBase.Property_PublicKey, userPasskeyObject.PublicKey));
            AddParameter(cmd, pInt32(UserPasskeyBase.Property_SignatureCounter, userPasskeyObject.SignatureCounter));
            AddParameter(cmd, pNVarChar(UserPasskeyBase.Property_CredType, 50, userPasskeyObject.CredType));
            AddParameter(cmd, pDateTime(UserPasskeyBase.Property_RegDate, userPasskeyObject.RegDate));
            AddParameter(cmd, pGuid(UserPasskeyBase.Property_AaGuid, userPasskeyObject.AaGuid));
            AddParameter(cmd, pNVarChar(UserPasskeyBase.Property_FriendlyName, 100, userPasskeyObject.FriendlyName));
            AddParameter(cmd, pNVarChar(UserPasskeyBase.Property_DeviceType, 100, userPasskeyObject.DeviceType));
        }

        // Helper if pVarBinary/pGuid is missing in your base class
        private SqlParameter pVarBinary(string name, byte[] val)
        {
            var p = new SqlParameter(name, SqlDbType.VarBinary);
            p.Value = (object)val ?? DBNull.Value;
            return p;
        }
        private SqlParameter pGuid(string name, Guid? val)
        {
            var p = new SqlParameter(name, SqlDbType.UniqueIdentifier);
            p.Value = (object)val ?? DBNull.Value;
            return p;
        }
        #endregion

        #region Insert Method
        /// <summary>
        /// Inserts UserPasskey
        /// </summary>
        /// <param name="userPasskeyObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
        public long Insert(UserPasskeyBase userPasskeyObject)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(INSERTUSERPASSKEY);

                AddParameter(cmd, pInt32Out(UserPasskeyBase.Property_Id));
                AddCommonParams(cmd, userPasskeyObject);

                long result = InsertRecord(cmd);
                if (result > 0)
                {
                    userPasskeyObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
                    userPasskeyObject.Id = (Int32)GetOutParameter(cmd, UserPasskeyBase.Property_Id);
                }
                return result;
            }
            catch (SqlException x)
            {
                throw new ObjectInsertException(userPasskeyObject, x);
            }
        }
        #endregion

        #region Update Method
        /// <summary>
        /// Updates UserPasskey
        /// </summary>
        /// <param name="userPasskeyObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
        public long Update(UserPasskeyBase userPasskeyObject)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(UPDATEUSERPASSKEY);

                AddParameter(cmd, pInt32(UserPasskeyBase.Property_Id, userPasskeyObject.Id));
                AddCommonParams(cmd, userPasskeyObject);

                long result = UpdateRecord(cmd);
                if (result > 0)
                    userPasskeyObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
                return result;
            }
            catch (SqlException x)
            {
                throw new ObjectUpdateException(userPasskeyObject, x);
            }
        }
        #endregion

        #region Delete Method
        /// <summary>
        /// Deletes UserPasskey
        /// </summary>
        /// <param name="Id">Id of the UserPasskey object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
        public long Delete(Int32 _Id)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(DELETEUSERPASSKEY);

                AddParameter(cmd, pInt32(UserPasskeyBase.Property_Id, _Id));

                return DeleteRecord(cmd);
            }
            catch (SqlException x)
            {
                throw new ObjectDeleteException(typeof(UserPasskey), _Id, x);
            }

        }
        #endregion

        #region Get By Id Method
        /// <summary>
        /// Retrieves UserPasskey object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the UserPasskey object to retrieve</param>
        /// <returns>UserPasskey object, null if not found</returns>
        public UserPasskey Get(Int32 _Id)
        {
            using (SqlCommand cmd = GetSPCommand(GETUSERPASSKEYBYID))
            {
                AddParameter(cmd, pInt32(UserPasskeyBase.Property_Id, _Id));

                return GetObject(cmd);
            }
        }
        #endregion

        #region Custom Get Methods
        /// <summary>
        /// Retrieves UserPasskey by CredentialId (Used for Login)
        /// </summary>
        public UserPasskey GetByCredentialId(byte[] credentialId)
        {
            using (SqlCommand cmd = GetSPCommand(GETUSERPASSKEYBYCREDENTIALID))
            {
                AddParameter(cmd, pVarBinary(UserPasskeyBase.Property_CredentialId, credentialId));
                return GetObject(cmd);
            }
        }

        /// <summary>
        /// Retrieves List by UserId
        /// </summary>
        public UserPasskeyList GetByUserId(Int32 userId)
        {
            using (SqlCommand cmd = GetSPCommand(GETUSERPASSKEYBYUSERID))
            {
                AddParameter(cmd, pInt32(UserPasskeyBase.Property_UserId, userId));
                // Assuming ALL_AVAILABLE_RECORDS is a constant in BaseDataAccess (-1 or similar)
                return GetList(cmd, -1);
            }
        }
        #endregion

        #region Fill Methods
        /// <summary>
        /// Fills UserPasskey object
        /// </summary>
        protected void FillObject(UserPasskeyBase userPasskeyObject, SqlDataReader reader, int start)
        {
            userPasskeyObject.Id = reader.GetInt32(start + 0);
            userPasskeyObject.UserId = reader.GetInt32(start + 1);
            // Handling byte[]
            if (!reader.IsDBNull(start + 2)) userPasskeyObject.CredentialId = (byte[])reader.GetValue(start + 2);
            if (!reader.IsDBNull(start + 3)) userPasskeyObject.PublicKey = (byte[])reader.GetValue(start + 3);

            userPasskeyObject.SignatureCounter = reader.GetInt32(start + 4);
            if (!reader.IsDBNull(start + 5)) userPasskeyObject.CredType = reader.GetString(start + 5);
            if (!reader.IsDBNull(start + 6)) userPasskeyObject.RegDate = reader.GetDateTime(start + 6);
            if (!reader.IsDBNull(start + 7)) userPasskeyObject.AaGuid = reader.GetGuid(start + 7);
            if(!reader.IsDBNull(start + 8)) userPasskeyObject.FriendlyName=reader.GetString(start + 8);
            if(!reader.IsDBNull(start + 9)) userPasskeyObject.DeviceType=reader.GetString(start + 9);

            FillBaseObject(userPasskeyObject, reader, (start + 10));

            userPasskeyObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
        }

        /// <summary>
        /// Fills UserPasskey object
        /// </summary>
        protected void FillObject(UserPasskeyBase userPasskeyObject, SqlDataReader reader)
        {
            FillObject(userPasskeyObject, reader, 0);
        }

        /// <summary>
        /// Retrieves UserPasskey object from SqlCommand
        /// </summary>
        private UserPasskey GetObject(SqlCommand cmd)
        {
            SqlDataReader reader;
            long rows = SelectRecords(cmd, out reader);

            using (reader)
            {
                if (reader.Read())
                {
                    UserPasskey obj = new UserPasskey();
                    FillObject(obj, reader);
                    return obj;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Retrieves list of UserPasskey objects from SqlCommand
        /// </summary>
        private UserPasskeyList GetList(SqlCommand cmd, long rows)
        {
            SqlDataReader reader;
            long result = SelectRecords(cmd, out reader);

            UserPasskeyList list = new UserPasskeyList();

            using (reader)
            {
                while (reader.Read() && (rows == -1 || rows-- != 0))
                {
                    UserPasskey obj = new UserPasskey();
                    FillObject(obj, reader);
                    list.Add(obj);
                }
                reader.Close();
            }

            return list;
        }
        #endregion
    }
}