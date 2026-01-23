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
	public partial class CmsPageDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTCMSPAGE = "InsertCmsPage";
		private const string UPDATECMSPAGE = "UpdateCmsPage";
		private const string DELETECMSPAGE = "DeleteCmsPage";
		private const string GETCMSPAGEBYID = "GetCmsPageById";
		private const string GETALLCMSPAGE = "GetAllCmsPage";
		private const string GETPAGEDCMSPAGE = "GetPagedCmsPage";
		private const string GETCMSPAGEMAXIMUMID = "GetCmsPageMaximumId";
		private const string GETCMSPAGEROWCOUNT = "GetCmsPageRowCount";	
		private const string GETCMSPAGEBYQUERY = "GetCmsPageByQuery";
		#endregion
		
		#region Constructors
		public CmsPageDataAccess(IConfiguration configuration) : base(configuration) { }
		public CmsPageDataAccess(ClientContext context) : base(context) { }
		public CmsPageDataAccess(SqlTransaction transaction) : base(transaction) { }
		public CmsPageDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="cmsPageObject"></param>
		private void AddCommonParams(SqlCommand cmd, CmsPageBase cmsPageObject)
		{	
			AddParameter(cmd, pInt32(CmsPageBase.Property_CompanyId, cmsPageObject.CompanyId));
			AddParameter(cmd, pNVarChar(CmsPageBase.Property_Title, 200, cmsPageObject.Title));
			AddParameter(cmd, pNVarChar(CmsPageBase.Property_Slug, 200, cmsPageObject.Slug));
			AddParameter(cmd, pNVarChar(CmsPageBase.Property_ContentHtml, cmsPageObject.ContentHtml));
			AddParameter(cmd, pNVarChar(CmsPageBase.Property_SidebarContentHtml, cmsPageObject.SidebarContentHtml));
			AddParameter(cmd, pNVarChar(CmsPageBase.Property_LayoutView, 100, cmsPageObject.LayoutView));
			AddParameter(cmd, pNVarChar(CmsPageBase.Property_MetaTitle, 200, cmsPageObject.MetaTitle));
			AddParameter(cmd, pNVarChar(CmsPageBase.Property_MetaDescription, 500, cmsPageObject.MetaDescription));
			AddParameter(cmd, pNVarChar(CmsPageBase.Property_CustomCss, cmsPageObject.CustomCss));
			AddParameter(cmd, pNVarChar(CmsPageBase.Property_CustomJs, cmsPageObject.CustomJs));
			AddParameter(cmd, pBool(CmsPageBase.Property_IsActive, cmsPageObject.IsActive));
			AddParameter(cmd, pInt32(CmsPageBase.Property_Version, cmsPageObject.Version));
			AddParameter(cmd, pDateTime(CmsPageBase.Property_PublishedAt, cmsPageObject.PublishedAt));
			AddParameter(cmd, pNVarChar(CmsPageBase.Property_CreatedBy, 100, cmsPageObject.CreatedBy));
			AddParameter(cmd, pDateTime(CmsPageBase.Property_CreatedAt, cmsPageObject.CreatedAt));
			AddParameter(cmd, pNVarChar(CmsPageBase.Property_UpdatedBy, 100, cmsPageObject.UpdatedBy));
			AddParameter(cmd, pDateTime(CmsPageBase.Property_UpdatedAt, cmsPageObject.UpdatedAt));
			AddParameter(cmd, pNVarChar(CmsPageBase.Property_CustomHeaderTags, cmsPageObject.CustomHeaderTags));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts CmsPage
        /// </summary>
        /// <param name="cmsPageObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(CmsPageBase cmsPageObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTCMSPAGE);
	
				AddParameter(cmd, pInt32Out(CmsPageBase.Property_Id));
				AddCommonParams(cmd, cmsPageObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					cmsPageObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					cmsPageObject.Id = (Int32)GetOutParameter(cmd, CmsPageBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(cmsPageObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates CmsPage
        /// </summary>
        /// <param name="cmsPageObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(CmsPageBase cmsPageObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATECMSPAGE);
				
				AddParameter(cmd, pInt32(CmsPageBase.Property_Id, cmsPageObject.Id));
				AddCommonParams(cmd, cmsPageObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					cmsPageObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(cmsPageObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes CmsPage
        /// </summary>
        /// <param name="Id">Id of the CmsPage object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETECMSPAGE);	
				
				AddParameter(cmd, pInt32(CmsPageBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(CmsPage), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves CmsPage object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the CmsPage object to retrieve</param>
        /// <returns>CmsPage object, null if not found</returns>
		public CmsPage Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETCMSPAGEBYID))
			{
				AddParameter( cmd, pInt32(CmsPageBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all CmsPage objects 
        /// </summary>
        /// <returns>A list of CmsPage objects</returns>
		public CmsPageList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLCMSPAGE))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all CmsPage objects by PageRequest
        /// </summary>
        /// <returns>A list of CmsPage objects</returns>
		public CmsPageList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDCMSPAGE))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				CmsPageList _CmsPageList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _CmsPageList;
			}
		}
		
		/// <summary>
        /// Retrieves all CmsPage objects by query String
        /// </summary>
        /// <returns>A list of CmsPage objects</returns>
		public CmsPageList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETCMSPAGEBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get CmsPage Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of CmsPage
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCMSPAGEMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get CmsPage Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of CmsPage
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _CmsPageRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCMSPAGEROWCOUNT))
			{
				SqlDataReader reader;
				_CmsPageRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _CmsPageRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills CmsPage object
        /// </summary>
        /// <param name="cmsPageObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(CmsPageBase cmsPageObject, SqlDataReader reader, int start)
		{
			
				cmsPageObject.Id = reader.GetInt32( start + 0 );			
				cmsPageObject.CompanyId = reader.GetInt32( start + 1 );			
				cmsPageObject.Title = reader.GetString( start + 2 );			
				cmsPageObject.Slug = reader.GetString( start + 3 );			
				if(!reader.IsDBNull(4)) cmsPageObject.ContentHtml = reader.GetString( start + 4 );			
				if(!reader.IsDBNull(5)) cmsPageObject.SidebarContentHtml = reader.GetString( start + 5 );			
				cmsPageObject.LayoutView = reader.GetString( start + 6 );			
				if(!reader.IsDBNull(7)) cmsPageObject.MetaTitle = reader.GetString( start + 7 );			
				if(!reader.IsDBNull(8)) cmsPageObject.MetaDescription = reader.GetString( start + 8 );			
				if(!reader.IsDBNull(9)) cmsPageObject.CustomCss = reader.GetString( start + 9 );			
				if(!reader.IsDBNull(10)) cmsPageObject.CustomJs = reader.GetString( start + 10 );			
				cmsPageObject.IsActive = reader.GetBoolean( start + 11 );			
				cmsPageObject.Version = reader.GetInt32( start + 12 );			
				if(!reader.IsDBNull(13)) cmsPageObject.PublishedAt = reader.GetDateTime( start + 13 );			
				if(!reader.IsDBNull(14)) cmsPageObject.CreatedBy = reader.GetString( start + 14 );			
				cmsPageObject.CreatedAt = reader.GetDateTime( start + 15 );			
				if(!reader.IsDBNull(16)) cmsPageObject.UpdatedBy = reader.GetString( start + 16 );			
				if(!reader.IsDBNull(17)) cmsPageObject.UpdatedAt = reader.GetDateTime( start + 17 );			
				if(!reader.IsDBNull(18)) cmsPageObject.CustomHeaderTags = reader.GetString( start + 18 );			
			FillBaseObject(cmsPageObject, reader, (start + 19));

			
			cmsPageObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills CmsPage object
        /// </summary>
        /// <param name="cmsPageObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(CmsPageBase cmsPageObject, SqlDataReader reader)
		{
			FillObject(cmsPageObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves CmsPage object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>CmsPage object</returns>
		private CmsPage GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					CmsPage cmsPageObject= new CmsPage();
					FillObject(cmsPageObject, reader);
					return cmsPageObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of CmsPage objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of CmsPage objects</returns>
		private CmsPageList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//CmsPage list
			CmsPageList list = new CmsPageList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					CmsPage cmsPageObject = new CmsPage();
					FillObject(cmsPageObject, reader);

					list.Add(cmsPageObject);
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