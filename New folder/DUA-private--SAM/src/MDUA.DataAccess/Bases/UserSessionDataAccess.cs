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
	public partial class UserSessionDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTUSERSESSION = "InsertUserSession";
		private const string UPDATEUSERSESSION = "UpdateUserSession";
		private const string DELETEUSERSESSION = "DeleteUserSession";
		private const string GETUSERSESSIONBYID = "GetUserSessionById";
		private const string GETALLUSERSESSION = "GetAllUserSession";
		private const string GETPAGEDUSERSESSION = "GetPagedUserSession";
		private const string GETUSERSESSIONBYUSERID = "GetUserSessionByUserId";
		private const string GETUSERSESSIONMAXIMUMID = "GetUserSessionMaximumId";
		private const string GETUSERSESSIONROWCOUNT = "GetUserSessionRowCount";	
		private const string GETUSERSESSIONBYQUERY = "GetUserSessionByQuery";
		#endregion
		
		#region Constructors
		public UserSessionDataAccess(IConfiguration configuration) : base(configuration) { }
		public UserSessionDataAccess(ClientContext context) : base(context) { }
		public UserSessionDataAccess(SqlTransaction transaction) : base(transaction) { }
		public UserSessionDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="userSessionObject"></param>
		private void AddCommonParams(SqlCommand cmd, UserSessionBase userSessionObject)
		{	
			AddParameter(cmd, pGuid(UserSessionBase.Property_SessionKey, userSessionObject.SessionKey));
			AddParameter(cmd, pInt32(UserSessionBase.Property_UserId, userSessionObject.UserId));
			AddParameter(cmd, pNVarChar(UserSessionBase.Property_IPAddress, 50, userSessionObject.IPAddress));
			AddParameter(cmd, pNVarChar(UserSessionBase.Property_DeviceInfo, 200, userSessionObject.DeviceInfo));
			AddParameter(cmd, pDateTime(UserSessionBase.Property_CreatedAt, userSessionObject.CreatedAt));
			AddParameter(cmd, pDateTime(UserSessionBase.Property_LastActiveAt, userSessionObject.LastActiveAt));
			AddParameter(cmd, pBool(UserSessionBase.Property_IsActive, userSessionObject.IsActive));
            AddParameter(cmd, pNVarChar(UserSessionBase.Property_LoginMethod, 50, userSessionObject.LoginMethod));
        }
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts UserSession
        /// </summary>
        /// <param name="userSessionObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(UserSessionBase userSessionObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTUSERSESSION);
	
				AddParameter(cmd, pInt32Out(UserSessionBase.Property_Id));
				AddCommonParams(cmd, userSessionObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					userSessionObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					userSessionObject.Id = (Int32)GetOutParameter(cmd, UserSessionBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(userSessionObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates UserSession
        /// </summary>
        /// <param name="userSessionObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(UserSessionBase userSessionObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEUSERSESSION);
				
				AddParameter(cmd, pInt32(UserSessionBase.Property_Id, userSessionObject.Id));
				AddCommonParams(cmd, userSessionObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					userSessionObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(userSessionObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes UserSession
        /// </summary>
        /// <param name="Id">Id of the UserSession object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEUSERSESSION);	
				
				AddParameter(cmd, pInt32(UserSessionBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(UserSession), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves UserSession object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the UserSession object to retrieve</param>
        /// <returns>UserSession object, null if not found</returns>
		public UserSession Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETUSERSESSIONBYID))
			{
				AddParameter( cmd, pInt32(UserSessionBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all UserSession objects 
        /// </summary>
        /// <returns>A list of UserSession objects</returns>
		public UserSessionList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLUSERSESSION))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all UserSession objects by UserId
        /// </summary>
        /// <returns>A list of UserSession objects</returns>
		public UserSessionList GetByUserId(Int32 _UserId)
		{
			using( SqlCommand cmd = GetSPCommand(GETUSERSESSIONBYUSERID))
			{
				
				AddParameter( cmd, pInt32(UserSessionBase.Property_UserId, _UserId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all UserSession objects by PageRequest
        /// </summary>
        /// <returns>A list of UserSession objects</returns>
		public UserSessionList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDUSERSESSION))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				UserSessionList _UserSessionList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _UserSessionList;
			}
		}
		
		/// <summary>
        /// Retrieves all UserSession objects by query String
        /// </summary>
        /// <returns>A list of UserSession objects</returns>
		public UserSessionList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETUSERSESSIONBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get UserSession Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of UserSession
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETUSERSESSIONMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get UserSession Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of UserSession
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _UserSessionRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETUSERSESSIONROWCOUNT))
			{
				SqlDataReader reader;
				_UserSessionRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _UserSessionRowCount;
		}

        #endregion

        #region Fill Methods
        /// <summary>
        /// Fills UserSession object
        /// </summary>
        /// <param name="userSessionObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
        protected void FillObject(UserSessionBase userSessionObject, SqlDataReader reader, int start)
        {

            userSessionObject.Id = reader.GetInt32(start + 0);
            userSessionObject.SessionKey = reader.GetGuid(start + 1);
            userSessionObject.UserId = reader.GetInt32(start + 2);
            if (!reader.IsDBNull(3)) userSessionObject.IPAddress = reader.GetString(start + 3);
            if (!reader.IsDBNull(4)) userSessionObject.DeviceInfo = reader.GetString(start + 4);
            if (!reader.IsDBNull(5)) userSessionObject.CreatedAt = reader.GetDateTime(start + 5);
            if (!reader.IsDBNull(6)) userSessionObject.LastActiveAt = reader.GetDateTime(start + 6);
            if (!reader.IsDBNull(7)) userSessionObject.IsActive = reader.GetBoolean(start + 7);
            if (!reader.IsDBNull(8)) userSessionObject.LoginMethod = reader.GetString(start + 8);

            FillBaseObject(userSessionObject, reader, (start + 9));


            userSessionObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
        }
        /// <summary>
        /// Fills UserSession object
        /// </summary>
        /// <param name="userSessionObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        protected void FillObject(UserSessionBase userSessionObject, SqlDataReader reader)
		{
			FillObject(userSessionObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves UserSession object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>UserSession object</returns>
		private UserSession GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					UserSession userSessionObject= new UserSession();
					FillObject(userSessionObject, reader);
					return userSessionObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of UserSession objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of UserSession objects</returns>
		private UserSessionList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//UserSession list
			UserSessionList list = new UserSessionList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					UserSession userSessionObject = new UserSession();
					FillObject(userSessionObject, reader);

					list.Add(userSessionObject);
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