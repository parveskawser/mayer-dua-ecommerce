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
	public partial class ProductSEODataAccess : BaseDataAccess,IProductSEODataAccess
	{
		#region Constants
		private const string INSERTPRODUCTSEO = "InsertProductSEO";
		private const string UPDATEPRODUCTSEO = "UpdateProductSEO";
		private const string DELETEPRODUCTSEO = "DeleteProductSEO";
		private const string GETPRODUCTSEOBYID = "GetProductSEOById";
		private const string GETALLPRODUCTSEO = "GetAllProductSEO";
		private const string GETPAGEDPRODUCTSEO = "GetPagedProductSEO";
		private const string GETPRODUCTSEOBYPRODUCTID = "GetProductSEOByProductId";
		private const string GETPRODUCTSEOMAXIMUMID = "GetProductSEOMaximumId";
		private const string GETPRODUCTSEOROWCOUNT = "GetProductSEORowCount";	
		private const string GETPRODUCTSEOBYQUERY = "GetProductSEOByQuery";
		#endregion
		
		#region Constructors
		public ProductSEODataAccess(IConfiguration configuration) : base(configuration) { }
		public ProductSEODataAccess(ClientContext context) : base(context) { }
		public ProductSEODataAccess(SqlTransaction transaction) : base(transaction) { }
		public ProductSEODataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="productSEOObject"></param>
		private void AddCommonParams(SqlCommand cmd, ProductSEOBase productSEOObject)
		{	
			AddParameter(cmd, pInt32(ProductSEOBase.Property_ProductId, productSEOObject.ProductId));
			AddParameter(cmd, pNVarChar(ProductSEOBase.Property_MetaTitle, 100, productSEOObject.MetaTitle));
			AddParameter(cmd, pNVarChar(ProductSEOBase.Property_MetaKeywords, 255, productSEOObject.MetaKeywords));
			AddParameter(cmd, pNVarChar(ProductSEOBase.Property_MetaDescription, 300, productSEOObject.MetaDescription));
			AddParameter(cmd, pNVarChar(ProductSEOBase.Property_CanonicalUrl, 500, productSEOObject.CanonicalUrl));
			AddParameter(cmd, pNVarChar(ProductSEOBase.Property_OGTitle, 150, productSEOObject.OGTitle));
			AddParameter(cmd, pNVarChar(ProductSEOBase.Property_OGDescription, 300, productSEOObject.OGDescription));
			AddParameter(cmd, pNVarChar(ProductSEOBase.Property_OGImage, 500, productSEOObject.OGImage));
			AddParameter(cmd, pNVarChar(ProductSEOBase.Property_CreatedBy, 100, productSEOObject.CreatedBy));
			AddParameter(cmd, pDateTime(ProductSEOBase.Property_CreatedAt, productSEOObject.CreatedAt));
			AddParameter(cmd, pNVarChar(ProductSEOBase.Property_UpdatedBy, 100, productSEOObject.UpdatedBy));
			AddParameter(cmd, pDateTime(ProductSEOBase.Property_UpdatedAt, productSEOObject.UpdatedAt));
			AddParameter(cmd, pNVarChar(ProductSEOBase.Property_CustomHeaderTags, productSEOObject.CustomHeaderTags));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts ProductSEO
        /// </summary>
        /// <param name="productSEOObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(ProductSEOBase productSEOObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTPRODUCTSEO);
	
				AddParameter(cmd, pInt32Out(ProductSEOBase.Property_Id));
				AddCommonParams(cmd, productSEOObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					productSEOObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					productSEOObject.Id = (Int32)GetOutParameter(cmd, ProductSEOBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(productSEOObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates ProductSEO
        /// </summary>
        /// <param name="productSEOObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(ProductSEOBase productSEOObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEPRODUCTSEO);
				
				AddParameter(cmd, pInt32(ProductSEOBase.Property_Id, productSEOObject.Id));
				AddCommonParams(cmd, productSEOObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					productSEOObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(productSEOObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes ProductSEO
        /// </summary>
        /// <param name="Id">Id of the ProductSEO object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEPRODUCTSEO);	
				
				AddParameter(cmd, pInt32(ProductSEOBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(ProductSEO), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves ProductSEO object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the ProductSEO object to retrieve</param>
        /// <returns>ProductSEO object, null if not found</returns>
		public ProductSEO Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTSEOBYID))
			{
				AddParameter( cmd, pInt32(ProductSEOBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all ProductSEO objects 
        /// </summary>
        /// <returns>A list of ProductSEO objects</returns>
		public ProductSEOList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLPRODUCTSEO))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all ProductSEO objects by ProductId
        /// </summary>
        /// <returns>A list of ProductSEO objects</returns>
		public ProductSEOList GetByProductId(Int32 _ProductId)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTSEOBYPRODUCTID))
			{
				
				AddParameter( cmd, pInt32(ProductSEOBase.Property_ProductId, _ProductId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all ProductSEO objects by PageRequest
        /// </summary>
        /// <returns>A list of ProductSEO objects</returns>
		public ProductSEOList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDPRODUCTSEO))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				ProductSEOList _ProductSEOList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _ProductSEOList;
			}
		}
		
		/// <summary>
        /// Retrieves all ProductSEO objects by query String
        /// </summary>
        /// <returns>A list of ProductSEO objects</returns>
		public ProductSEOList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTSEOBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get ProductSEO Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ProductSEO
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTSEOMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get ProductSEO Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ProductSEO
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _ProductSEORowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTSEOROWCOUNT))
			{
				SqlDataReader reader;
				_ProductSEORowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _ProductSEORowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills ProductSEO object
        /// </summary>
        /// <param name="productSEOObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(ProductSEOBase productSEOObject, SqlDataReader reader, int start)
		{
			
				productSEOObject.Id = reader.GetInt32( start + 0 );			
				productSEOObject.ProductId = reader.GetInt32( start + 1 );			
				if(!reader.IsDBNull(2)) productSEOObject.MetaTitle = reader.GetString( start + 2 );			
				if(!reader.IsDBNull(3)) productSEOObject.MetaKeywords = reader.GetString( start + 3 );			
				if(!reader.IsDBNull(4)) productSEOObject.MetaDescription = reader.GetString( start + 4 );			
				if(!reader.IsDBNull(5)) productSEOObject.CanonicalUrl = reader.GetString( start + 5 );			
				if(!reader.IsDBNull(6)) productSEOObject.OGTitle = reader.GetString( start + 6 );			
				if(!reader.IsDBNull(7)) productSEOObject.OGDescription = reader.GetString( start + 7 );			
				if(!reader.IsDBNull(8)) productSEOObject.OGImage = reader.GetString( start + 8 );			
				if(!reader.IsDBNull(9)) productSEOObject.CreatedBy = reader.GetString( start + 9 );			
				productSEOObject.CreatedAt = reader.GetDateTime( start + 10 );			
				if(!reader.IsDBNull(11)) productSEOObject.UpdatedBy = reader.GetString( start + 11 );			
				if(!reader.IsDBNull(12)) productSEOObject.UpdatedAt = reader.GetDateTime( start + 12 );
                 if (!reader.IsDBNull(13)) productSEOObject.CustomHeaderTags = reader.GetString(start + 13);

                 FillBaseObject(productSEOObject, reader, (start + 14));

			
			productSEOObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills ProductSEO object
        /// </summary>
        /// <param name="productSEOObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(ProductSEOBase productSEOObject, SqlDataReader reader)
		{
			FillObject(productSEOObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves ProductSEO object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>ProductSEO object</returns>
		private ProductSEO GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					ProductSEO productSEOObject= new ProductSEO();
					FillObject(productSEOObject, reader);
					return productSEOObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of ProductSEO objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of ProductSEO objects</returns>
		private ProductSEOList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//ProductSEO list
			ProductSEOList list = new ProductSEOList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					ProductSEO productSEOObject = new ProductSEO();
					FillObject(productSEOObject, reader);

					list.Add(productSEOObject);
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