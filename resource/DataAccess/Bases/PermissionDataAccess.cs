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
	public partial class PermissionDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTPERMISSION = "InsertPermission";
		private const string UPDATEPERMISSION = "UpdatePermission";
		private const string DELETEPERMISSION = "DeletePermission";
		private const string GETPERMISSIONBYID = "GetPermissionById";
		private const string GETALLPERMISSION = "GetAllPermission";
		private const string GETPAGEDPERMISSION = "GetPagedPermission";
		private const string GETPERMISSIONMAXIMUMID = "GetPermissionMaximumId";
		private const string GETPERMISSIONROWCOUNT = "GetPermissionRowCount";	
		private const string GETPERMISSIONBYQUERY = "GetPermissionByQuery";
		#endregion
		
		#region Constructors
		public PermissionDataAccess(IConfiguration configuration) : base(configuration) { }
		public PermissionDataAccess(ClientContext context) : base(context) { }
		public PermissionDataAccess(SqlTransaction transaction) : base(transaction) { }
		public PermissionDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="permissionObject"></param>
		private void AddCommonParams(SqlCommand cmd, PermissionBase permissionObject)
		{	
			AddParameter(cmd, pNVarChar(PermissionBase.Property_Name, 100, permissionObject.Name));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts Permission
        /// </summary>
        /// <param name="permissionObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(PermissionBase permissionObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTPERMISSION);
	
				AddParameter(cmd, pInt32Out(PermissionBase.Property_Id));
				AddCommonParams(cmd, permissionObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					permissionObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					permissionObject.Id = (Int32)GetOutParameter(cmd, PermissionBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(permissionObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates Permission
        /// </summary>
        /// <param name="permissionObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(PermissionBase permissionObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEPERMISSION);
				
				AddParameter(cmd, pInt32(PermissionBase.Property_Id, permissionObject.Id));
				AddCommonParams(cmd, permissionObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					permissionObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(permissionObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes Permission
        /// </summary>
        /// <param name="Id">Id of the Permission object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEPERMISSION);	
				
				AddParameter(cmd, pInt32(PermissionBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(Permission), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves Permission object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the Permission object to retrieve</param>
        /// <returns>Permission object, null if not found</returns>
		public Permission Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETPERMISSIONBYID))
			{
				AddParameter( cmd, pInt32(PermissionBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all Permission objects 
        /// </summary>
        /// <returns>A list of Permission objects</returns>
		public PermissionList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLPERMISSION))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all Permission objects by PageRequest
        /// </summary>
        /// <returns>A list of Permission objects</returns>
		public PermissionList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDPERMISSION))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				PermissionList _PermissionList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _PermissionList;
			}
		}
		
		/// <summary>
        /// Retrieves all Permission objects by query String
        /// </summary>
        /// <returns>A list of Permission objects</returns>
		public PermissionList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETPERMISSIONBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get Permission Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Permission
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPERMISSIONMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get Permission Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Permission
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _PermissionRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPERMISSIONROWCOUNT))
			{
				SqlDataReader reader;
				_PermissionRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _PermissionRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills Permission object
        /// </summary>
        /// <param name="permissionObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(PermissionBase permissionObject, SqlDataReader reader, int start)
		{
			
				permissionObject.Id = reader.GetInt32( start + 0 );			
				permissionObject.Name = reader.GetString( start + 1 );			
			FillBaseObject(permissionObject, reader, (start + 2));

			
			permissionObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills Permission object
        /// </summary>
        /// <param name="permissionObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(PermissionBase permissionObject, SqlDataReader reader)
		{
			FillObject(permissionObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves Permission object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>Permission object</returns>
		private Permission GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					Permission permissionObject= new Permission();
					FillObject(permissionObject, reader);
					return permissionObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of Permission objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of Permission objects</returns>
		private PermissionList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//Permission list
			PermissionList list = new PermissionList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					Permission permissionObject = new Permission();
					FillObject(permissionObject, reader);

					list.Add(permissionObject);
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