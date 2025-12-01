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
	public partial class PermissionGroupDataAccess : BaseDataAccess, IPermissionGroupDataAccess
	{
		#region Constants
		private const string INSERTPERMISSIONGROUP = "InsertPermissionGroup";
		private const string UPDATEPERMISSIONGROUP = "UpdatePermissionGroup";
		private const string DELETEPERMISSIONGROUP = "DeletePermissionGroup";
		private const string GETPERMISSIONGROUPBYID = "GetPermissionGroupById";
		private const string GETALLPERMISSIONGROUP = "GetAllPermissionGroup";
		private const string GETPAGEDPERMISSIONGROUP = "GetPagedPermissionGroup";
		private const string GETPERMISSIONGROUPMAXIMUMID = "GetPermissionGroupMaximumId";
		private const string GETPERMISSIONGROUPROWCOUNT = "GetPermissionGroupRowCount";	
		private const string GETPERMISSIONGROUPBYQUERY = "GetPermissionGroupByQuery";
		#endregion
		
		#region Constructors
		public PermissionGroupDataAccess(IConfiguration configuration) : base(configuration) { }
		public PermissionGroupDataAccess(ClientContext context) : base(context) { }
		public PermissionGroupDataAccess(SqlTransaction transaction) : base(transaction) { }
		public PermissionGroupDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="permissionGroupObject"></param>
		private void AddCommonParams(SqlCommand cmd, PermissionGroupBase permissionGroupObject)
		{	
			AddParameter(cmd, pNVarChar(PermissionGroupBase.Property_Name, 100, permissionGroupObject.Name));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts PermissionGroup
        /// </summary>
        /// <param name="permissionGroupObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(PermissionGroupBase permissionGroupObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTPERMISSIONGROUP);
	
				AddParameter(cmd, pInt32Out(PermissionGroupBase.Property_Id));
				AddCommonParams(cmd, permissionGroupObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					permissionGroupObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					permissionGroupObject.Id = (Int32)GetOutParameter(cmd, PermissionGroupBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(permissionGroupObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates PermissionGroup
        /// </summary>
        /// <param name="permissionGroupObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(PermissionGroupBase permissionGroupObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEPERMISSIONGROUP);
				
				AddParameter(cmd, pInt32(PermissionGroupBase.Property_Id, permissionGroupObject.Id));
				AddCommonParams(cmd, permissionGroupObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					permissionGroupObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(permissionGroupObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes PermissionGroup
        /// </summary>
        /// <param name="Id">Id of the PermissionGroup object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEPERMISSIONGROUP);	
				
				AddParameter(cmd, pInt32(PermissionGroupBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(PermissionGroup), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves PermissionGroup object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the PermissionGroup object to retrieve</param>
        /// <returns>PermissionGroup object, null if not found</returns>
		public PermissionGroup Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETPERMISSIONGROUPBYID))
			{
				AddParameter( cmd, pInt32(PermissionGroupBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all PermissionGroup objects 
        /// </summary>
        /// <returns>A list of PermissionGroup objects</returns>
		public PermissionGroupList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLPERMISSIONGROUP))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all PermissionGroup objects by PageRequest
        /// </summary>
        /// <returns>A list of PermissionGroup objects</returns>
		public PermissionGroupList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDPERMISSIONGROUP))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				PermissionGroupList _PermissionGroupList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _PermissionGroupList;
			}
		}
		
		/// <summary>
        /// Retrieves all PermissionGroup objects by query String
        /// </summary>
        /// <returns>A list of PermissionGroup objects</returns>
		public PermissionGroupList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETPERMISSIONGROUPBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get PermissionGroup Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of PermissionGroup
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPERMISSIONGROUPMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get PermissionGroup Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of PermissionGroup
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _PermissionGroupRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPERMISSIONGROUPROWCOUNT))
			{
				SqlDataReader reader;
				_PermissionGroupRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _PermissionGroupRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills PermissionGroup object
        /// </summary>
        /// <param name="permissionGroupObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(PermissionGroupBase permissionGroupObject, SqlDataReader reader, int start)
		{
			
				permissionGroupObject.Id = reader.GetInt32( start + 0 );			
				permissionGroupObject.Name = reader.GetString( start + 1 );			
			FillBaseObject(permissionGroupObject, reader, (start + 2));

			
			permissionGroupObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills PermissionGroup object
        /// </summary>
        /// <param name="permissionGroupObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(PermissionGroupBase permissionGroupObject, SqlDataReader reader)
		{
			FillObject(permissionGroupObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves PermissionGroup object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>PermissionGroup object</returns>
		private PermissionGroup GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					PermissionGroup permissionGroupObject= new PermissionGroup();
					FillObject(permissionGroupObject, reader);
					return permissionGroupObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of PermissionGroup objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of PermissionGroup objects</returns>
		private PermissionGroupList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//PermissionGroup list
			PermissionGroupList list = new PermissionGroupList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					PermissionGroup permissionGroupObject = new PermissionGroup();
					FillObject(permissionGroupObject, reader);

					list.Add(permissionGroupObject);
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
