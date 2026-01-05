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
	public partial class ProductVariantDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTPRODUCTVARIANT = "InsertProductVariant";
		private const string UPDATEPRODUCTVARIANT = "UpdateProductVariant";
		private const string DELETEPRODUCTVARIANT = "DeleteProductVariant";
		private const string GETPRODUCTVARIANTBYID = "GetProductVariantById";
		private const string GETALLPRODUCTVARIANT = "GetAllProductVariant";
		private const string GETPAGEDPRODUCTVARIANT = "GetPagedProductVariant";
		private const string GETPRODUCTVARIANTBYPRODUCTID = "GetProductVariantByProductId";
		private const string GETPRODUCTVARIANTMAXIMUMID = "GetProductVariantMaximumId";
		private const string GETPRODUCTVARIANTROWCOUNT = "GetProductVariantRowCount";	
		private const string GETPRODUCTVARIANTBYQUERY = "GetProductVariantByQuery";
		#endregion
		
		#region Constructors
		public ProductVariantDataAccess(IConfiguration configuration) : base(configuration) { }
		public ProductVariantDataAccess(ClientContext context) : base(context) { }
		public ProductVariantDataAccess(SqlTransaction transaction) : base(transaction) { }
		public ProductVariantDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="productVariantObject"></param>
		private void AddCommonParams(SqlCommand cmd, ProductVariantBase productVariantObject)
		{	
			AddParameter(cmd, pInt32(ProductVariantBase.Property_ProductId, productVariantObject.ProductId));
			AddParameter(cmd, pNVarChar(ProductVariantBase.Property_VariantName, 150, productVariantObject.VariantName));
			AddParameter(cmd, pNVarChar(ProductVariantBase.Property_SKU, 50, productVariantObject.SKU));
			AddParameter(cmd, pNVarChar(ProductVariantBase.Property_Barcode, 100, productVariantObject.Barcode));
			AddParameter(cmd, pDecimal(ProductVariantBase.Property_VariantPrice, 9, productVariantObject.VariantPrice));
			AddParameter(cmd, pBool(ProductVariantBase.Property_IsActive, productVariantObject.IsActive));
			AddParameter(cmd, pNVarChar(ProductVariantBase.Property_CreatedBy, 100, productVariantObject.CreatedBy));
			AddParameter(cmd, pDateTime(ProductVariantBase.Property_CreatedAt, productVariantObject.CreatedAt));
			AddParameter(cmd, pNVarChar(ProductVariantBase.Property_UpdatedBy, 100, productVariantObject.UpdatedBy));
			AddParameter(cmd, pDateTime(ProductVariantBase.Property_UpdatedAt, productVariantObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts ProductVariant
        /// </summary>
        /// <param name="productVariantObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(ProductVariantBase productVariantObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTPRODUCTVARIANT);
	
				AddParameter(cmd, pInt32Out(ProductVariantBase.Property_Id));
				AddCommonParams(cmd, productVariantObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					productVariantObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					productVariantObject.Id = (Int32)GetOutParameter(cmd, ProductVariantBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(productVariantObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates ProductVariant
        /// </summary>
        /// <param name="productVariantObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(ProductVariantBase productVariantObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEPRODUCTVARIANT);
				
				AddParameter(cmd, pInt32(ProductVariantBase.Property_Id, productVariantObject.Id));
				AddCommonParams(cmd, productVariantObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					productVariantObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(productVariantObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes ProductVariant
        /// </summary>
        /// <param name="Id">Id of the ProductVariant object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEPRODUCTVARIANT);	
				
				AddParameter(cmd, pInt32(ProductVariantBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(ProductVariant), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves ProductVariant object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the ProductVariant object to retrieve</param>
        /// <returns>ProductVariant object, null if not found</returns>
		public ProductVariant Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTVARIANTBYID))
			{
				AddParameter( cmd, pInt32(ProductVariantBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all ProductVariant objects 
        /// </summary>
        /// <returns>A list of ProductVariant objects</returns>
		public ProductVariantList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLPRODUCTVARIANT))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all ProductVariant objects by ProductId
        /// </summary>
        /// <returns>A list of ProductVariant objects</returns>
		public ProductVariantList GetByProductId(Int32 _ProductId)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTVARIANTBYPRODUCTID))
			{
				
				AddParameter( cmd, pInt32(ProductVariantBase.Property_ProductId, _ProductId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all ProductVariant objects by PageRequest
        /// </summary>
        /// <returns>A list of ProductVariant objects</returns>
		public ProductVariantList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDPRODUCTVARIANT))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				ProductVariantList _ProductVariantList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _ProductVariantList;
			}
		}
		
		/// <summary>
        /// Retrieves all ProductVariant objects by query String
        /// </summary>
        /// <returns>A list of ProductVariant objects</returns>
		public ProductVariantList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTVARIANTBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get ProductVariant Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ProductVariant
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTVARIANTMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get ProductVariant Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ProductVariant
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _ProductVariantRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTVARIANTROWCOUNT))
			{
				SqlDataReader reader;
				_ProductVariantRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _ProductVariantRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills ProductVariant object
        /// </summary>
        /// <param name="productVariantObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(ProductVariantBase productVariantObject, SqlDataReader reader, int start)
		{
			
				productVariantObject.Id = reader.GetInt32( start + 0 );			
				productVariantObject.ProductId = reader.GetInt32( start + 1 );			
				productVariantObject.VariantName = reader.GetString( start + 2 );			
				if(!reader.IsDBNull(3)) productVariantObject.SKU = reader.GetString( start + 3 );			
				if(!reader.IsDBNull(4)) productVariantObject.Barcode = reader.GetString( start + 4 );			
				if(!reader.IsDBNull(5)) productVariantObject.VariantPrice = reader.GetDecimal( start + 5 );			
				productVariantObject.IsActive = reader.GetBoolean( start + 6 );			
				if(!reader.IsDBNull(7)) productVariantObject.CreatedBy = reader.GetString( start + 7 );			
				productVariantObject.CreatedAt = reader.GetDateTime( start + 8 );			
				if(!reader.IsDBNull(9)) productVariantObject.UpdatedBy = reader.GetString( start + 9 );			
				if(!reader.IsDBNull(10)) productVariantObject.UpdatedAt = reader.GetDateTime( start + 10 );			
			FillBaseObject(productVariantObject, reader, (start + 11));

			
			productVariantObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills ProductVariant object
        /// </summary>
        /// <param name="productVariantObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(ProductVariantBase productVariantObject, SqlDataReader reader)
		{
			FillObject(productVariantObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves ProductVariant object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>ProductVariant object</returns>
		private ProductVariant GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					ProductVariant productVariantObject= new ProductVariant();
					FillObject(productVariantObject, reader);
					return productVariantObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of ProductVariant objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of ProductVariant objects</returns>
		private ProductVariantList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//ProductVariant list
			ProductVariantList list = new ProductVariantList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					ProductVariant productVariantObject = new ProductVariant();
					FillObject(productVariantObject, reader);

					list.Add(productVariantObject);
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