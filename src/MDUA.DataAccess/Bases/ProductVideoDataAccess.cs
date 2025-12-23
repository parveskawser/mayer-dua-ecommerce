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
	public partial class ProductVideoDataAccess : BaseDataAccess, IProductVideoDataAccess
	{
		#region Constants
		private const string INSERTPRODUCTVIDEO = "InsertProductVideo";
		private const string UPDATEPRODUCTVIDEO = "UpdateProductVideo";
		private const string DELETEPRODUCTVIDEO = "DeleteProductVideo";
		private const string GETPRODUCTVIDEOBYID = "GetProductVideoById";
		private const string GETALLPRODUCTVIDEO = "GetAllProductVideo";
		private const string GETPAGEDPRODUCTVIDEO = "GetPagedProductVideo";
		private const string GETPRODUCTVIDEOBYPRODUCTID = "GetProductVideoByProductId";
		private const string GETPRODUCTVIDEOMAXIMUMID = "GetProductVideoMaximumId";
		private const string GETPRODUCTVIDEOROWCOUNT = "GetProductVideoRowCount";	
		private const string GETPRODUCTVIDEOBYQUERY = "GetProductVideoByQuery";
		#endregion
		
		#region Constructors
		public ProductVideoDataAccess(IConfiguration configuration) : base(configuration) { }
		public ProductVideoDataAccess(ClientContext context) : base(context) { }
		public ProductVideoDataAccess(SqlTransaction transaction) : base(transaction) { }
		public ProductVideoDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="productVideoObject"></param>
		private void AddCommonParams(SqlCommand cmd, ProductVideoBase productVideoObject)
		{	
			AddParameter(cmd, pInt32(ProductVideoBase.Property_ProductId, productVideoObject.ProductId));
			AddParameter(cmd, pNVarChar(ProductVideoBase.Property_VideoUrl, 500, productVideoObject.VideoUrl));
			AddParameter(cmd, pNVarChar(ProductVideoBase.Property_ThumbnailUrl, 300, productVideoObject.ThumbnailUrl));
			AddParameter(cmd, pBool(ProductVideoBase.Property_IsPrimary, productVideoObject.IsPrimary));
			AddParameter(cmd, pInt32(ProductVideoBase.Property_SortOrder, productVideoObject.SortOrder));
			AddParameter(cmd, pNVarChar(ProductVideoBase.Property_Title, 200, productVideoObject.Title));
			AddParameter(cmd, pNVarChar(ProductVideoBase.Property_CreatedBy, 100, productVideoObject.CreatedBy));
			AddParameter(cmd, pDateTime(ProductVideoBase.Property_CreatedAt, productVideoObject.CreatedAt));
			AddParameter(cmd, pNVarChar(ProductVideoBase.Property_UpdatedBy, 100, productVideoObject.UpdatedBy));
			AddParameter(cmd, pDateTime(ProductVideoBase.Property_UpdatedAt, productVideoObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts ProductVideo
        /// </summary>
        /// <param name="productVideoObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(ProductVideoBase productVideoObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTPRODUCTVIDEO);
	
				AddParameter(cmd, pInt32Out(ProductVideoBase.Property_Id));
				AddCommonParams(cmd, productVideoObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					productVideoObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					productVideoObject.Id = (Int32)GetOutParameter(cmd, ProductVideoBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(productVideoObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates ProductVideo
        /// </summary>
        /// <param name="productVideoObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(ProductVideoBase productVideoObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEPRODUCTVIDEO);
				
				AddParameter(cmd, pInt32(ProductVideoBase.Property_Id, productVideoObject.Id));
				AddCommonParams(cmd, productVideoObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					productVideoObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(productVideoObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes ProductVideo
        /// </summary>
        /// <param name="Id">Id of the ProductVideo object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEPRODUCTVIDEO);	
				
				AddParameter(cmd, pInt32(ProductVideoBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(ProductVideo), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves ProductVideo object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the ProductVideo object to retrieve</param>
        /// <returns>ProductVideo object, null if not found</returns>
		public ProductVideo Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTVIDEOBYID))
			{
				AddParameter( cmd, pInt32(ProductVideoBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all ProductVideo objects 
        /// </summary>
        /// <returns>A list of ProductVideo objects</returns>
		public ProductVideoList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLPRODUCTVIDEO))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all ProductVideo objects by ProductId
        /// </summary>
        /// <returns>A list of ProductVideo objects</returns>
		public ProductVideoList GetByProductId(Int32 _ProductId)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTVIDEOBYPRODUCTID))
			{
				
				AddParameter( cmd, pInt32(ProductVideoBase.Property_ProductId, _ProductId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all ProductVideo objects by PageRequest
        /// </summary>
        /// <returns>A list of ProductVideo objects</returns>
		public ProductVideoList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDPRODUCTVIDEO))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				ProductVideoList _ProductVideoList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _ProductVideoList;
			}
		}
		
		/// <summary>
        /// Retrieves all ProductVideo objects by query String
        /// </summary>
        /// <returns>A list of ProductVideo objects</returns>
		public ProductVideoList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTVIDEOBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get ProductVideo Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ProductVideo
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTVIDEOMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get ProductVideo Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ProductVideo
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _ProductVideoRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTVIDEOROWCOUNT))
			{
				SqlDataReader reader;
				_ProductVideoRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _ProductVideoRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills ProductVideo object
        /// </summary>
        /// <param name="productVideoObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(ProductVideoBase productVideoObject, SqlDataReader reader, int start)
		{
			
				productVideoObject.Id = reader.GetInt32( start + 0 );			
				productVideoObject.ProductId = reader.GetInt32( start + 1 );			
				productVideoObject.VideoUrl = reader.GetString( start + 2 );			
				if(!reader.IsDBNull(3)) productVideoObject.ThumbnailUrl = reader.GetString( start + 3 );			
				productVideoObject.IsPrimary = reader.GetBoolean( start + 4 );			
				productVideoObject.SortOrder = reader.GetInt32( start + 5 );			
				if(!reader.IsDBNull(6)) productVideoObject.Title = reader.GetString( start + 6 );			
				if(!reader.IsDBNull(7)) productVideoObject.CreatedBy = reader.GetString( start + 7 );			
				productVideoObject.CreatedAt = reader.GetDateTime( start + 8 );			
				if(!reader.IsDBNull(9)) productVideoObject.UpdatedBy = reader.GetString( start + 9 );			
				if(!reader.IsDBNull(10)) productVideoObject.UpdatedAt = reader.GetDateTime( start + 10 );			
			FillBaseObject(productVideoObject, reader, (start + 11));

			
			productVideoObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills ProductVideo object
        /// </summary>
        /// <param name="productVideoObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(ProductVideoBase productVideoObject, SqlDataReader reader)
		{
			FillObject(productVideoObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves ProductVideo object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>ProductVideo object</returns>
		private ProductVideo GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					ProductVideo productVideoObject= new ProductVideo();
					FillObject(productVideoObject, reader);
					return productVideoObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of ProductVideo objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of ProductVideo objects</returns>
		private ProductVideoList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//ProductVideo list
			ProductVideoList list = new ProductVideoList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					ProductVideo productVideoObject = new ProductVideo();
					FillObject(productVideoObject, reader);

					list.Add(productVideoObject);
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
