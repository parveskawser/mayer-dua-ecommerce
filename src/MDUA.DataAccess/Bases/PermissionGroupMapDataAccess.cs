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
	public partial class PermissionGroupMapDataAccess : BaseDataAccess, IPermissionGroupMapDataAccess
	{
		#region Constants
		private const string INSERTPERMISSIONGROUPMAP = "InsertPermissionGroupMap";
		private const string UPDATEPERMISSIONGROUPMAP = "UpdatePermissionGroupMap";
		private const string DELETEPERMISSIONGROUPMAP = "DeletePermissionGroupMap";
		private const string GETPERMISSIONGROUPMAPBYID = "GetPermissionGroupMapById";
		private const string GETALLPERMISSIONGROUPMAP = "GetAllPermissionGroupMap";
		private const string GETPAGEDPERMISSIONGROUPMAP = "GetPagedPermissionGroupMap";
		private const string GETPERMISSIONGROUPMAPBYPERMISSIONID = "GetPermissionGroupMapByPermissionId";
		private const string GETPERMISSIONGROUPMAPBYPERMISSIONGROUPID = "GetPermissionGroupMapByPermissionGroupId";
		private const string GETPERMISSIONGROUPMAPMAXIMUMID = "GetPermissionGroupMapMaximumId";
		private const string GETPERMISSIONGROUPMAPROWCOUNT = "GetPermissionGroupMapRowCount";	
		private const string GETPERMISSIONGROUPMAPBYQUERY = "GetPermissionGroupMapByQuery";
		#endregion
		
		#region Constructors
		public PermissionGroupMapDataAccess(IConfiguration configuration) : base(configuration) { }
		public PermissionGroupMapDataAccess(ClientContext context) : base(context) { }
		public PermissionGroupMapDataAccess(SqlTransaction transaction) : base(transaction) { }
		public PermissionGroupMapDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="permissionGroupMapObject"></param>
		private void AddCommonParams(SqlCommand cmd, PermissionGroupMapBase permissionGroupMapObject)
		{	
			AddParameter(cmd, pInt32(PermissionGroupMapBase.Property_PermissionId, permissionGroupMapObject.PermissionId));
			AddParameter(cmd, pInt32(PermissionGroupMapBase.Property_PermissionGroupId, permissionGroupMapObject.PermissionGroupId));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts PermissionGroupMap
        /// </summary>
        /// <param name="permissionGroupMapObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(PermissionGroupMapBase permissionGroupMapObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTPERMISSIONGROUPMAP);
	
				AddParameter(cmd, pInt32Out(PermissionGroupMapBase.Property_Id));
				AddCommonParams(cmd, permissionGroupMapObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					permissionGroupMapObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					permissionGroupMapObject.Id = (Int32)GetOutParameter(cmd, PermissionGroupMapBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(permissionGroupMapObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates PermissionGroupMap
        /// </summary>
        /// <param name="permissionGroupMapObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(PermissionGroupMapBase permissionGroupMapObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEPERMISSIONGROUPMAP);
				
				AddParameter(cmd, pInt32(PermissionGroupMapBase.Property_Id, permissionGroupMapObject.Id));
				AddCommonParams(cmd, permissionGroupMapObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					permissionGroupMapObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(permissionGroupMapObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes PermissionGroupMap
        /// </summary>
        /// <param name="Id">Id of the PermissionGroupMap object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEPERMISSIONGROUPMAP);	
				
				AddParameter(cmd, pInt32(PermissionGroupMapBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(PermissionGroupMap), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves PermissionGroupMap object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the PermissionGroupMap object to retrieve</param>
        /// <returns>PermissionGroupMap object, null if not found</returns>
		public PermissionGroupMap Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETPERMISSIONGROUPMAPBYID))
			{
				AddParameter( cmd, pInt32(PermissionGroupMapBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all PermissionGroupMap objects 
        /// </summary>
        /// <returns>A list of PermissionGroupMap objects</returns>
		public PermissionGroupMapList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLPERMISSIONGROUPMAP))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all PermissionGroupMap objects by PermissionId
        /// </summary>
        /// <returns>A list of PermissionGroupMap objects</returns>
		public PermissionGroupMapList GetByPermissionId(Int32 _PermissionId)
		{
			using( SqlCommand cmd = GetSPCommand(GETPERMISSIONGROUPMAPBYPERMISSIONID))
			{
				
				AddParameter( cmd, pInt32(PermissionGroupMapBase.Property_PermissionId, _PermissionId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all PermissionGroupMap objects by PermissionGroupId
        /// </summary>
        /// <returns>A list of PermissionGroupMap objects</returns>
		public PermissionGroupMapList GetByPermissionGroupId(Int32 _PermissionGroupId)
		{
			using( SqlCommand cmd = GetSPCommand(GETPERMISSIONGROUPMAPBYPERMISSIONGROUPID))
			{
				
				AddParameter( cmd, pInt32(PermissionGroupMapBase.Property_PermissionGroupId, _PermissionGroupId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all PermissionGroupMap objects by PageRequest
        /// </summary>
        /// <returns>A list of PermissionGroupMap objects</returns>
		public PermissionGroupMapList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDPERMISSIONGROUPMAP))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				PermissionGroupMapList _PermissionGroupMapList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _PermissionGroupMapList;
			}
		}
		
		/// <summary>
        /// Retrieves all PermissionGroupMap objects by query String
        /// </summary>
        /// <returns>A list of PermissionGroupMap objects</returns>
		public PermissionGroupMapList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETPERMISSIONGROUPMAPBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get PermissionGroupMap Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of PermissionGroupMap
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPERMISSIONGROUPMAPMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get PermissionGroupMap Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of PermissionGroupMap
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _PermissionGroupMapRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPERMISSIONGROUPMAPROWCOUNT))
			{
				SqlDataReader reader;
				_PermissionGroupMapRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _PermissionGroupMapRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills PermissionGroupMap object
        /// </summary>
        /// <param name="permissionGroupMapObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(PermissionGroupMapBase permissionGroupMapObject, SqlDataReader reader, int start)
		{
			
				permissionGroupMapObject.Id = reader.GetInt32( start + 0 );			
				permissionGroupMapObject.PermissionId = reader.GetInt32( start + 1 );			
				permissionGroupMapObject.PermissionGroupId = reader.GetInt32( start + 2 );			
			FillBaseObject(permissionGroupMapObject, reader, (start + 3));

			
			permissionGroupMapObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills PermissionGroupMap object
        /// </summary>
        /// <param name="permissionGroupMapObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(PermissionGroupMapBase permissionGroupMapObject, SqlDataReader reader)
		{
			FillObject(permissionGroupMapObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves PermissionGroupMap object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>PermissionGroupMap object</returns>
		private PermissionGroupMap GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					PermissionGroupMap permissionGroupMapObject= new PermissionGroupMap();
					FillObject(permissionGroupMapObject, reader);
					return permissionGroupMapObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of PermissionGroupMap objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of PermissionGroupMap objects</returns>
		private PermissionGroupMapList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//PermissionGroupMap list
			PermissionGroupMapList list = new PermissionGroupMapList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					PermissionGroupMap permissionGroupMapObject = new PermissionGroupMap();
					FillObject(permissionGroupMapObject, reader);

					list.Add(permissionGroupMapObject);
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
