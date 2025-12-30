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
		private const string GETALLUSERPASSKEY = "GetAllUserPasskey";
		private const string GETPAGEDUSERPASSKEY = "GetPagedUserPasskey";
		private const string GETUSERPASSKEYBYUSERID = "GetUserPasskeyByUserId";
		private const string GETUSERPASSKEYMAXIMUMID = "GetUserPasskeyMaximumId";
		private const string GETUSERPASSKEYROWCOUNT = "GetUserPasskeyRowCount";	
		private const string GETUSERPASSKEYBYQUERY = "GetUserPasskeyByQuery";
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
			AddParameter(cmd, pVarBinary(UserPasskeyBase.Property_CredentialId, userPasskeyObject.CredentialId));
			AddParameter(cmd, pVarBinary(UserPasskeyBase.Property_PublicKey, userPasskeyObject.PublicKey));
			AddParameter(cmd, pInt32(UserPasskeyBase.Property_SignatureCounter, userPasskeyObject.SignatureCounter));
			AddParameter(cmd, pNVarChar(UserPasskeyBase.Property_CredType, 50, userPasskeyObject.CredType));
			AddParameter(cmd, pDateTime(UserPasskeyBase.Property_RegDate, userPasskeyObject.RegDate));
			AddParameter(cmd, pGuid(UserPasskeyBase.Property_AaGuid, userPasskeyObject.AaGuid));
			AddParameter(cmd, pNVarChar(UserPasskeyBase.Property_FriendlyName, 100, userPasskeyObject.FriendlyName));
			AddParameter(cmd, pNVarChar(UserPasskeyBase.Property_DeviceType, 100, userPasskeyObject.DeviceType));
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
			catch(SqlException x)
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
			catch(SqlException x)
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
			catch(SqlException x)
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
			using( SqlCommand cmd = GetSPCommand(GETUSERPASSKEYBYID))
			{
				AddParameter( cmd, pInt32(UserPasskeyBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all UserPasskey objects 
        /// </summary>
        /// <returns>A list of UserPasskey objects</returns>
		public UserPasskeyList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLUSERPASSKEY))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all UserPasskey objects by UserId
        /// </summary>
        /// <returns>A list of UserPasskey objects</returns>
		public UserPasskeyList GetByUserId(Int32 _UserId)
		{
			using( SqlCommand cmd = GetSPCommand(GETUSERPASSKEYBYUSERID))
			{
				
				AddParameter( cmd, pInt32(UserPasskeyBase.Property_UserId, _UserId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all UserPasskey objects by PageRequest
        /// </summary>
        /// <returns>A list of UserPasskey objects</returns>
		public UserPasskeyList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDUSERPASSKEY))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				UserPasskeyList _UserPasskeyList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _UserPasskeyList;
			}
		}
		
		/// <summary>
        /// Retrieves all UserPasskey objects by query String
        /// </summary>
        /// <returns>A list of UserPasskey objects</returns>
		public UserPasskeyList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETUSERPASSKEYBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get UserPasskey Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of UserPasskey
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETUSERPASSKEYMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get UserPasskey Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of UserPasskey
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _UserPasskeyRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETUSERPASSKEYROWCOUNT))
			{
				SqlDataReader reader;
				_UserPasskeyRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _UserPasskeyRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills UserPasskey object
        /// </summary>
        /// <param name="userPasskeyObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(UserPasskeyBase userPasskeyObject, SqlDataReader reader, int start)
		{
			
				userPasskeyObject.Id = reader.GetInt32( start + 0 );			
				userPasskeyObject.UserId = reader.GetInt32( start + 1 );			
				userPasskeyObject.CredentialId = (Byte[])reader.GetValue( start + 2);			
				userPasskeyObject.PublicKey = (Byte[])reader.GetValue( start + 3);			
				userPasskeyObject.SignatureCounter = reader.GetInt32( start + 4 );			
				if(!reader.IsDBNull(5)) userPasskeyObject.CredType = reader.GetString( start + 5 );			
				if(!reader.IsDBNull(6)) userPasskeyObject.RegDate = reader.GetDateTime( start + 6 );			
				if(!reader.IsDBNull(7)) userPasskeyObject.AaGuid = reader.GetGuid( start + 7 );			
				if(!reader.IsDBNull(8)) userPasskeyObject.FriendlyName = reader.GetString( start + 8 );			
				if(!reader.IsDBNull(9)) userPasskeyObject.DeviceType = reader.GetString( start + 9 );			
			FillBaseObject(userPasskeyObject, reader, (start + 10));

			
			userPasskeyObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills UserPasskey object
        /// </summary>
        /// <param name="userPasskeyObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(UserPasskeyBase userPasskeyObject, SqlDataReader reader)
		{
			FillObject(userPasskeyObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves UserPasskey object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>UserPasskey object</returns>
		private UserPasskey GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					UserPasskey userPasskeyObject= new UserPasskey();
					FillObject(userPasskeyObject, reader);
					return userPasskeyObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of UserPasskey objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of UserPasskey objects</returns>
		private UserPasskeyList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//UserPasskey list
			UserPasskeyList list = new UserPasskeyList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					UserPasskey userPasskeyObject = new UserPasskey();
					FillObject(userPasskeyObject, reader);

					list.Add(userPasskeyObject);
				}
				
				// Close the reader in order to receive output parameters
				// Output parameters are not available until reader is closed.
				reader.Close();
			}

			return list;
		}
		
		#endregion
	}	
}