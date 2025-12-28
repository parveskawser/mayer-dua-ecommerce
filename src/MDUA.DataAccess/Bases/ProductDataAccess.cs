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
    public partial class ProductDataAccess : BaseDataAccess, IProductDataAccess
    {
		#region Constants
		private const string INSERTPRODUCT = "InsertProduct";
		private const string UPDATEPRODUCT = "UpdateProduct";
		private const string DELETEPRODUCT = "DeleteProduct";
		private const string GETPRODUCTBYID = "GetProductById";
		private const string GETALLPRODUCT = "GetAllProduct";
		private const string GETPAGEDPRODUCT = "GetPagedProduct";
		private const string GETPRODUCTBYCOMPANYID = "GetProductByCompanyId";
		private const string GETPRODUCTMAXIMUMID = "GetProductMaximumId";
		private const string GETPRODUCTROWCOUNT = "GetProductRowCount";	
		private const string GETPRODUCTBYQUERY = "GetProductByQuery";
		#endregion
		
		#region Constructors
		public ProductDataAccess(IConfiguration configuration) : base(configuration) { }
		public ProductDataAccess(ClientContext context) : base(context) { }
		public ProductDataAccess(SqlTransaction transaction) : base(transaction) { }
		public ProductDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="productObject"></param>
		private void AddCommonParams(SqlCommand cmd, ProductBase productObject)
		{	
			AddParameter(cmd, pInt32(ProductBase.Property_CompanyId, productObject.CompanyId));
			AddParameter(cmd, pNVarChar(ProductBase.Property_ProductName, 200, productObject.ProductName));
			AddParameter(cmd, pInt32(ProductBase.Property_ReorderLevel, productObject.ReorderLevel));
			AddParameter(cmd, pNVarChar(ProductBase.Property_Barcode, 100, productObject.Barcode));
			AddParameter(cmd, pInt32(ProductBase.Property_CategoryId, productObject.CategoryId));
			AddParameter(cmd, pNVarChar(ProductBase.Property_Description, productObject.Description));
			AddParameter(cmd, pNVarChar(ProductBase.Property_Slug, 400, productObject.Slug));
			AddParameter(cmd, pDecimal(ProductBase.Property_BasePrice, 9, productObject.BasePrice));
			AddParameter(cmd, pBool(ProductBase.Property_IsVariantBased, productObject.IsVariantBased));
			AddParameter(cmd, pBool(ProductBase.Property_IsActive, productObject.IsActive));
			AddParameter(cmd, pNVarChar(ProductBase.Property_CreatedBy, 100, productObject.CreatedBy));
			AddParameter(cmd, pDateTime(ProductBase.Property_CreatedAt, productObject.CreatedAt));
			AddParameter(cmd, pNVarChar(ProductBase.Property_UpdatedBy, 100, productObject.UpdatedBy));
			AddParameter(cmd, pDateTime(ProductBase.Property_UpdatedAt, productObject.UpdatedAt));
			AddParameter(cmd, pNVarChar(ProductBase.Property_ExtraInfo, productObject.ExtraInfo));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts Product
        /// </summary>
        /// <param name="productObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(ProductBase productObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTPRODUCT);
	
				AddParameter(cmd, pInt32Out(ProductBase.Property_Id));
				AddCommonParams(cmd, productObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					productObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					productObject.Id = (Int32)GetOutParameter(cmd, ProductBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(productObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates Product
        /// </summary>
        /// <param name="productObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(ProductBase productObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEPRODUCT);
				
				AddParameter(cmd, pInt32(ProductBase.Property_Id, productObject.Id));
				AddCommonParams(cmd, productObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					productObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(productObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes Product
        /// </summary>
        /// <param name="Id">Id of the Product object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEPRODUCT);	
				
				AddParameter(cmd, pInt32(ProductBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(Product), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves Product object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the Product object to retrieve</param>
        /// <returns>Product object, null if not found</returns>
		public Product Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTBYID))
			{
				AddParameter( cmd, pInt32(ProductBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all Product objects 
        /// </summary>
        /// <returns>A list of Product objects</returns>
		public ProductList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLPRODUCT))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all Product objects by CompanyId
        /// </summary>
        /// <returns>A list of Product objects</returns>
		public ProductList GetByCompanyId(Int32 _CompanyId)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTBYCOMPANYID))
			{
				
				AddParameter( cmd, pInt32(ProductBase.Property_CompanyId, _CompanyId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all Product objects by PageRequest
        /// </summary>
        /// <returns>A list of Product objects</returns>
		public ProductList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDPRODUCT))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				ProductList _ProductList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _ProductList;
			}
		}
		
		/// <summary>
        /// Retrieves all Product objects by query String
        /// </summary>
        /// <returns>A list of Product objects</returns>
		public ProductList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get Product Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Product
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get Product Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Product
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _ProductRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTROWCOUNT))
			{
				SqlDataReader reader;
				_ProductRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _ProductRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills Product object
        /// </summary>
        /// <param name="productObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(ProductBase productObject, SqlDataReader reader, int start)
		{
			
				productObject.Id = reader.GetInt32( start + 0 );			
				productObject.CompanyId = reader.GetInt32( start + 1 );			
				productObject.ProductName = reader.GetString( start + 2 );			
				productObject.ReorderLevel = reader.GetInt32( start + 3 );			
				if(!reader.IsDBNull(4)) productObject.Barcode = reader.GetString( start + 4 );			
				if(!reader.IsDBNull(5)) productObject.CategoryId = reader.GetInt32( start + 5 );			
				if(!reader.IsDBNull(6)) productObject.Description = reader.GetString( start + 6 );			
				if(!reader.IsDBNull(7)) productObject.Slug = reader.GetString( start + 7 );			
				if(!reader.IsDBNull(8)) productObject.BasePrice = reader.GetDecimal( start + 8 );			
				if(!reader.IsDBNull(9)) productObject.IsVariantBased = reader.GetBoolean( start + 9 );			
				productObject.IsActive = reader.GetBoolean( start + 10 );			
				if(!reader.IsDBNull(11)) productObject.CreatedBy = reader.GetString( start + 11 );			
				if(!reader.IsDBNull(12)) productObject.CreatedAt = reader.GetDateTime( start + 12 );			
				if(!reader.IsDBNull(13)) productObject.UpdatedBy = reader.GetString( start + 13 );			
				if(!reader.IsDBNull(14)) productObject.UpdatedAt = reader.GetDateTime( start + 14 );			
				if(!reader.IsDBNull(15)) productObject.ExtraInfo = reader.GetString( start + 15 );			
			FillBaseObject(productObject, reader, (start + 16));

			
			productObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills Product object
        /// </summary>
        /// <param name="productObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(ProductBase productObject, SqlDataReader reader)
		{
			FillObject(productObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves Product object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>Product object</returns>
		private Product GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					Product productObject= new Product();
					FillObject(productObject, reader);
					return productObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of Product objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of Product objects</returns>
		private ProductList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//Product list
			ProductList list = new ProductList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					Product productObject = new Product();
					FillObject(productObject, reader);

					list.Add(productObject);
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