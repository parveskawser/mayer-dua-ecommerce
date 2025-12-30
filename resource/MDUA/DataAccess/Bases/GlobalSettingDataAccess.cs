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
	public partial class GlobalSettingDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTGLOBALSETTING = "InsertGlobalSetting";
		private const string UPDATEGLOBALSETTING = "UpdateGlobalSetting";
		private const string DELETEGLOBALSETTING = "DeleteGlobalSetting";
		private const string GETGLOBALSETTINGBYID = "GetGlobalSettingById";
		private const string GETALLGLOBALSETTING = "GetAllGlobalSetting";
		private const string GETPAGEDGLOBALSETTING = "GetPagedGlobalSetting";
		private const string GETGLOBALSETTINGMAXIMUMID = "GetGlobalSettingMaximumId";
		private const string GETGLOBALSETTINGROWCOUNT = "GetGlobalSettingRowCount";	
		private const string GETGLOBALSETTINGBYQUERY = "GetGlobalSettingByQuery";
		#endregion
		
		#region Constructors
		public GlobalSettingDataAccess(IConfiguration configuration) : base(configuration) { }
		public GlobalSettingDataAccess(ClientContext context) : base(context) { }
		public GlobalSettingDataAccess(SqlTransaction transaction) : base(transaction) { }
		public GlobalSettingDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="globalSettingObject"></param>
		private void AddCommonParams(SqlCommand cmd, GlobalSettingBase globalSettingObject)
		{	
			AddParameter(cmd, pInt32(GlobalSettingBase.Property_CompanyId, globalSettingObject.CompanyId));
			AddParameter(cmd, pNVarChar(GlobalSettingBase.Property_GKey, 50, globalSettingObject.GKey));
			AddParameter(cmd, pNVarChar(GlobalSettingBase.Property_GContent, globalSettingObject.GContent));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts GlobalSetting
        /// </summary>
        /// <param name="globalSettingObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(GlobalSettingBase globalSettingObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTGLOBALSETTING);
	
				AddParameter(cmd, pInt32(GlobalSettingBase.Property_Id, globalSettingObject.Id));
				AddCommonParams(cmd, globalSettingObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					globalSettingObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(globalSettingObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates GlobalSetting
        /// </summary>
        /// <param name="globalSettingObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(GlobalSettingBase globalSettingObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEGLOBALSETTING);
				
				AddParameter(cmd, pInt32(GlobalSettingBase.Property_Id, globalSettingObject.Id));
				AddCommonParams(cmd, globalSettingObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					globalSettingObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(globalSettingObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes GlobalSetting
        /// </summary>
        /// <param name="Id">Id of the GlobalSetting object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEGLOBALSETTING);	
				
				AddParameter(cmd, pInt32(GlobalSettingBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(GlobalSetting), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves GlobalSetting object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the GlobalSetting object to retrieve</param>
        /// <returns>GlobalSetting object, null if not found</returns>
		public GlobalSetting Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETGLOBALSETTINGBYID))
			{
				AddParameter( cmd, pInt32(GlobalSettingBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all GlobalSetting objects 
        /// </summary>
        /// <returns>A list of GlobalSetting objects</returns>
		public GlobalSettingList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLGLOBALSETTING))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all GlobalSetting objects by PageRequest
        /// </summary>
        /// <returns>A list of GlobalSetting objects</returns>
		public GlobalSettingList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDGLOBALSETTING))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				GlobalSettingList _GlobalSettingList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _GlobalSettingList;
			}
		}
		
		/// <summary>
        /// Retrieves all GlobalSetting objects by query String
        /// </summary>
        /// <returns>A list of GlobalSetting objects</returns>
		public GlobalSettingList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETGLOBALSETTINGBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get GlobalSetting Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of GlobalSetting
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETGLOBALSETTINGMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get GlobalSetting Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of GlobalSetting
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _GlobalSettingRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETGLOBALSETTINGROWCOUNT))
			{
				SqlDataReader reader;
				_GlobalSettingRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _GlobalSettingRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills GlobalSetting object
        /// </summary>
        /// <param name="globalSettingObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(GlobalSettingBase globalSettingObject, SqlDataReader reader, int start)
		{
			
				globalSettingObject.Id = reader.GetInt32( start + 0 );			
				globalSettingObject.CompanyId = reader.GetInt32( start + 1 );			
				globalSettingObject.GKey = reader.GetString( start + 2 );			
				globalSettingObject.GContent = reader.GetString( start + 3 );			
			FillBaseObject(globalSettingObject, reader, (start + 4));

			
			globalSettingObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills GlobalSetting object
        /// </summary>
        /// <param name="globalSettingObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(GlobalSettingBase globalSettingObject, SqlDataReader reader)
		{
			FillObject(globalSettingObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves GlobalSetting object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>GlobalSetting object</returns>
		private GlobalSetting GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					GlobalSetting globalSettingObject= new GlobalSetting();
					FillObject(globalSettingObject, reader);
					return globalSettingObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of GlobalSetting objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of GlobalSetting objects</returns>
		private GlobalSettingList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//GlobalSetting list
			GlobalSettingList list = new GlobalSettingList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					GlobalSetting globalSettingObject = new GlobalSetting();
					FillObject(globalSettingObject, reader);

					list.Add(globalSettingObject);
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