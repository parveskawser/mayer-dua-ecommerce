using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework;
using MDUA.Framework.DataAccess;
using MDUA.Framework.Exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;

namespace MDUA.DataAccess
{
	public partial class CmsAssetDataAccess : BaseDataAccess, ICmsAssetDataAccess
    {
		#region Constants
		private const string INSERTCMSASSET = "InsertCmsAsset";
		private const string UPDATECMSASSET = "UpdateCmsAsset";
		private const string DELETECMSASSET = "DeleteCmsAsset";
		private const string GETCMSASSETBYID = "GetCmsAssetById";
		private const string GETALLCMSASSET = "GetAllCmsAsset";
		private const string GETPAGEDCMSASSET = "GetPagedCmsAsset";
		private const string GETCMSASSETBYPAGEID = "GetCmsAssetByPageId";
		private const string GETCMSASSETMAXIMUMID = "GetCmsAssetMaximumId";
		private const string GETCMSASSETROWCOUNT = "GetCmsAssetRowCount";	
		private const string GETCMSASSETBYQUERY = "GetCmsAssetByQuery";
		#endregion
		
		#region Constructors
		public CmsAssetDataAccess(IConfiguration configuration) : base(configuration) { }
		public CmsAssetDataAccess(ClientContext context) : base(context) { }
		public CmsAssetDataAccess(SqlTransaction transaction) : base(transaction) { }
		public CmsAssetDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="cmsAssetObject"></param>
		private void AddCommonParams(SqlCommand cmd, CmsAssetBase cmsAssetObject)
		{	
			AddParameter(cmd, pInt32(CmsAssetBase.Property_CompanyId, cmsAssetObject.CompanyId));
			AddParameter(cmd, pInt32(CmsAssetBase.Property_PageId, cmsAssetObject.PageId));
			AddParameter(cmd, pNVarChar(CmsAssetBase.Property_FileName, 255, cmsAssetObject.FileName));
			AddParameter(cmd, pNVarChar(CmsAssetBase.Property_FilePath, 500, cmsAssetObject.FilePath));
			AddParameter(cmd, pNVarChar(CmsAssetBase.Property_FileType, 20, cmsAssetObject.FileType));
			AddParameter(cmd, pInt32(CmsAssetBase.Property_Version, cmsAssetObject.Version));
			AddParameter(cmd, pBool(CmsAssetBase.Property_IsActive, cmsAssetObject.IsActive));
			AddParameter(cmd, pDateTime(CmsAssetBase.Property_CreatedAt, cmsAssetObject.CreatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts CmsAsset
        /// </summary>
        /// <param name="cmsAssetObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(CmsAssetBase cmsAssetObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTCMSASSET);
	
				AddParameter(cmd, pInt32Out(CmsAssetBase.Property_Id));
				AddCommonParams(cmd, cmsAssetObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					cmsAssetObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					cmsAssetObject.Id = (Int32)GetOutParameter(cmd, CmsAssetBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(cmsAssetObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates CmsAsset
        /// </summary>
        /// <param name="cmsAssetObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(CmsAssetBase cmsAssetObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATECMSASSET);
				
				AddParameter(cmd, pInt32(CmsAssetBase.Property_Id, cmsAssetObject.Id));
				AddCommonParams(cmd, cmsAssetObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					cmsAssetObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(cmsAssetObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes CmsAsset
        /// </summary>
        /// <param name="Id">Id of the CmsAsset object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETECMSASSET);	
				
				AddParameter(cmd, pInt32(CmsAssetBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(CmsAsset), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves CmsAsset object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the CmsAsset object to retrieve</param>
        /// <returns>CmsAsset object, null if not found</returns>
		public CmsAsset Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETCMSASSETBYID))
			{
				AddParameter( cmd, pInt32(CmsAssetBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all CmsAsset objects 
        /// </summary>
        /// <returns>A list of CmsAsset objects</returns>
		public CmsAssetList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLCMSASSET))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all CmsAsset objects by PageId
        /// </summary>
        /// <returns>A list of CmsAsset objects</returns>
		public CmsAssetList GetByPageId(Nullable<Int32> _PageId)
		{
			using( SqlCommand cmd = GetSPCommand(GETCMSASSETBYPAGEID))
			{
				
				AddParameter( cmd, pInt32(CmsAssetBase.Property_PageId, _PageId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all CmsAsset objects by PageRequest
        /// </summary>
        /// <returns>A list of CmsAsset objects</returns>
		public CmsAssetList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDCMSASSET))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				CmsAssetList _CmsAssetList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _CmsAssetList;
			}
		}
		
		/// <summary>
        /// Retrieves all CmsAsset objects by query String
        /// </summary>
        /// <returns>A list of CmsAsset objects</returns>
		public CmsAssetList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETCMSASSETBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get CmsAsset Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of CmsAsset
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCMSASSETMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get CmsAsset Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of CmsAsset
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _CmsAssetRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCMSASSETROWCOUNT))
			{
				SqlDataReader reader;
				_CmsAssetRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _CmsAssetRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills CmsAsset object
        /// </summary>
        /// <param name="cmsAssetObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(CmsAssetBase cmsAssetObject, SqlDataReader reader, int start)
		{
			
				cmsAssetObject.Id = reader.GetInt32( start + 0 );			
				cmsAssetObject.CompanyId = reader.GetInt32( start + 1 );			
				if(!reader.IsDBNull(2)) cmsAssetObject.PageId = reader.GetInt32( start + 2 );			
				cmsAssetObject.FileName = reader.GetString( start + 3 );			
				cmsAssetObject.FilePath = reader.GetString( start + 4 );			
				cmsAssetObject.FileType = reader.GetString( start + 5 );			
				cmsAssetObject.Version = reader.GetInt32( start + 6 );			
				cmsAssetObject.IsActive = reader.GetBoolean( start + 7 );			
				cmsAssetObject.CreatedAt = reader.GetDateTime( start + 8 );			
			FillBaseObject(cmsAssetObject, reader, (start + 9));

			
			cmsAssetObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills CmsAsset object
        /// </summary>
        /// <param name="cmsAssetObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(CmsAssetBase cmsAssetObject, SqlDataReader reader)
		{
			FillObject(cmsAssetObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves CmsAsset object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>CmsAsset object</returns>
		private CmsAsset GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					CmsAsset cmsAssetObject= new CmsAsset();
					FillObject(cmsAssetObject, reader);
					return cmsAssetObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of CmsAsset objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of CmsAsset objects</returns>
		private CmsAssetList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//CmsAsset list
			CmsAssetList list = new CmsAssetList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					CmsAsset cmsAssetObject = new CmsAsset();
					FillObject(cmsAssetObject, reader);

					list.Add(cmsAssetObject);
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