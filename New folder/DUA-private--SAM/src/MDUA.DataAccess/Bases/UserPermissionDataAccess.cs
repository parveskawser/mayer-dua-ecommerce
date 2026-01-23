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
using MDUA.DataAccess.Interface;

namespace MDUA.DataAccess
{
	public partial class UserPermissionDataAccess : BaseDataAccess, IUserPermissionDataAccess
	{
		#region Constants
		private const string INSERTUSERPERMISSION = "InsertUserPermission";
		private const string UPDATEUSERPERMISSION = "UpdateUserPermission";
		private const string DELETEUSERPERMISSION = "DeleteUserPermission";
		private const string GETUSERPERMISSIONBYID = "GetUserPermissionById";
		private const string GETALLUSERPERMISSION = "GetAllUserPermission";
		private const string GETPAGEDUSERPERMISSION = "GetPagedUserPermission";
		private const string GETUSERPERMISSIONBYUSERID = "GetUserPermissionByUserId";
		private const string GETUSERPERMISSIONBYPERMISSIONID = "GetUserPermissionByPermissionId";
		private const string GETUSERPERMISSIONBYPERMISSIONGROUPID = "GetUserPermissionByPermissionGroupId";
		private const string GETUSERPERMISSIONMAXIMUMID = "GetUserPermissionMaximumId";
		private const string GETUSERPERMISSIONROWCOUNT = "GetUserPermissionRowCount";	
		private const string GETUSERPERMISSIONBYQUERY = "GetUserPermissionByQuery";
		#endregion
		
		#region Constructors
		public UserPermissionDataAccess(IConfiguration configuration) : base(configuration) { }
		public UserPermissionDataAccess(ClientContext context) : base(context) { }
		public UserPermissionDataAccess(SqlTransaction transaction) : base(transaction) { }
		public UserPermissionDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="userPermissionObject"></param>
		private void AddCommonParams(SqlCommand cmd, UserPermissionBase userPermissionObject)
		{	
			AddParameter(cmd, pInt32(UserPermissionBase.Property_UserId, userPermissionObject.UserId));
			AddParameter(cmd, pInt32(UserPermissionBase.Property_PermissionId, userPermissionObject.PermissionId));
			AddParameter(cmd, pInt32(UserPermissionBase.Property_PermissionGroupId, userPermissionObject.PermissionGroupId));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts UserPermission
        /// </summary>
        /// <param name="userPermissionObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(UserPermissionBase userPermissionObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTUSERPERMISSION);
	
				AddParameter(cmd, pInt32Out(UserPermissionBase.Property_Id));
				AddCommonParams(cmd, userPermissionObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					userPermissionObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					userPermissionObject.Id = (Int32)GetOutParameter(cmd, UserPermissionBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(userPermissionObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates UserPermission
        /// </summary>
        /// <param name="userPermissionObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(UserPermissionBase userPermissionObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEUSERPERMISSION);
				
				AddParameter(cmd, pInt32(UserPermissionBase.Property_Id, userPermissionObject.Id));
				AddCommonParams(cmd, userPermissionObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					userPermissionObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(userPermissionObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes UserPermission
        /// </summary>
        /// <param name="Id">Id of the UserPermission object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEUSERPERMISSION);	
				
				AddParameter(cmd, pInt32(UserPermissionBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(UserPermission), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves UserPermission object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the UserPermission object to retrieve</param>
        /// <returns>UserPermission object, null if not found</returns>
		public UserPermission Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETUSERPERMISSIONBYID))
			{
				AddParameter( cmd, pInt32(UserPermissionBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all UserPermission objects 
        /// </summary>
        /// <returns>A list of UserPermission objects</returns>
		public UserPermissionList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLUSERPERMISSION))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all UserPermission objects by UserId
        /// </summary>
        /// <returns>A list of UserPermission objects</returns>
		public UserPermissionList GetByUserId(Int32 _UserId)
		{
			using( SqlCommand cmd = GetSPCommand(GETUSERPERMISSIONBYUSERID))
			{
				
				AddParameter( cmd, pInt32(UserPermissionBase.Property_UserId, _UserId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all UserPermission objects by PermissionId
        /// </summary>
        /// <returns>A list of UserPermission objects</returns>
		public UserPermissionList GetByPermissionId(Nullable<Int32> _PermissionId)
		{
			using( SqlCommand cmd = GetSPCommand(GETUSERPERMISSIONBYPERMISSIONID))
			{
				
				AddParameter( cmd, pInt32(UserPermissionBase.Property_PermissionId, _PermissionId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all UserPermission objects by PermissionGroupId
        /// </summary>
        /// <returns>A list of UserPermission objects</returns>
		public UserPermissionList GetByPermissionGroupId(Nullable<Int32> _PermissionGroupId)
		{
			using( SqlCommand cmd = GetSPCommand(GETUSERPERMISSIONBYPERMISSIONGROUPID))
			{
				
				AddParameter( cmd, pInt32(UserPermissionBase.Property_PermissionGroupId, _PermissionGroupId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all UserPermission objects by PageRequest
        /// </summary>
        /// <returns>A list of UserPermission objects</returns>
		public UserPermissionList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDUSERPERMISSION))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				UserPermissionList _UserPermissionList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _UserPermissionList;
			}
		}
		
		/// <summary>
        /// Retrieves all UserPermission objects by query String
        /// </summary>
        /// <returns>A list of UserPermission objects</returns>
		public UserPermissionList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETUSERPERMISSIONBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get UserPermission Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of UserPermission
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETUSERPERMISSIONMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get UserPermission Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of UserPermission
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _UserPermissionRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETUSERPERMISSIONROWCOUNT))
			{
				SqlDataReader reader;
				_UserPermissionRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _UserPermissionRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills UserPermission object
        /// </summary>
        /// <param name="userPermissionObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(UserPermissionBase userPermissionObject, SqlDataReader reader, int start)
		{
			
				userPermissionObject.Id = reader.GetInt32( start + 0 );			
				userPermissionObject.UserId = reader.GetInt32( start + 1 );			
				if(!reader.IsDBNull(2)) userPermissionObject.PermissionId = reader.GetInt32( start + 2 );			
				if(!reader.IsDBNull(3)) userPermissionObject.PermissionGroupId = reader.GetInt32( start + 3 );			
			FillBaseObject(userPermissionObject, reader, (start + 4));

			
			userPermissionObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills UserPermission object
        /// </summary>
        /// <param name="userPermissionObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(UserPermissionBase userPermissionObject, SqlDataReader reader)
		{
			FillObject(userPermissionObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves UserPermission object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>UserPermission object</returns>
		private UserPermission GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					UserPermission userPermissionObject= new UserPermission();
					FillObject(userPermissionObject, reader);
					return userPermissionObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of UserPermission objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of UserPermission objects</returns>
		private UserPermissionList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//UserPermission list
			UserPermissionList list = new UserPermissionList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					UserPermission userPermissionObject = new UserPermission();
					FillObject(userPermissionObject, reader);

					list.Add(userPermissionObject);
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
