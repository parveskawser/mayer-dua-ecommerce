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
	public partial class ProductDiscountDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTPRODUCTDISCOUNT = "InsertProductDiscount";
		private const string UPDATEPRODUCTDISCOUNT = "UpdateProductDiscount";
		private const string DELETEPRODUCTDISCOUNT = "DeleteProductDiscount";
		private const string GETPRODUCTDISCOUNTBYID = "GetProductDiscountById";
		private const string GETALLPRODUCTDISCOUNT = "GetAllProductDiscount";
		private const string GETPAGEDPRODUCTDISCOUNT = "GetPagedProductDiscount";
		private const string GETPRODUCTDISCOUNTBYPRODUCTID = "GetProductDiscountByProductId";
		private const string GETPRODUCTDISCOUNTMAXIMUMID = "GetProductDiscountMaximumId";
		private const string GETPRODUCTDISCOUNTROWCOUNT = "GetProductDiscountRowCount";	
		private const string GETPRODUCTDISCOUNTBYQUERY = "GetProductDiscountByQuery";
		#endregion
		
		#region Constructors
		public ProductDiscountDataAccess(IConfiguration configuration) : base(configuration) { }
		public ProductDiscountDataAccess(ClientContext context) : base(context) { }
		public ProductDiscountDataAccess(SqlTransaction transaction) : base(transaction) { }
		public ProductDiscountDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="productDiscountObject"></param>
		private void AddCommonParams(SqlCommand cmd, ProductDiscountBase productDiscountObject)
		{	
			AddParameter(cmd, pInt32(ProductDiscountBase.Property_ProductId, productDiscountObject.ProductId));
			AddParameter(cmd, pNVarChar(ProductDiscountBase.Property_DiscountType, 50, productDiscountObject.DiscountType));
			AddParameter(cmd, pDecimal(ProductDiscountBase.Property_DiscountValue, 9, productDiscountObject.DiscountValue));
			AddParameter(cmd, pInt32(ProductDiscountBase.Property_MinQuantity, productDiscountObject.MinQuantity));
			AddParameter(cmd, pDateTime(ProductDiscountBase.Property_EffectiveFrom, productDiscountObject.EffectiveFrom));
			AddParameter(cmd, pDateTime(ProductDiscountBase.Property_EffectiveTo, productDiscountObject.EffectiveTo));
			AddParameter(cmd, pBool(ProductDiscountBase.Property_IsActive, productDiscountObject.IsActive));
			AddParameter(cmd, pNVarChar(ProductDiscountBase.Property_CreatedBy, 100, productDiscountObject.CreatedBy));
			AddParameter(cmd, pDateTime(ProductDiscountBase.Property_CreatedAt, productDiscountObject.CreatedAt));
			AddParameter(cmd, pNVarChar(ProductDiscountBase.Property_UpdatedBy, 100, productDiscountObject.UpdatedBy));
			AddParameter(cmd, pDateTime(ProductDiscountBase.Property_UpdatedAt, productDiscountObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts ProductDiscount
        /// </summary>
        /// <param name="productDiscountObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(ProductDiscountBase productDiscountObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTPRODUCTDISCOUNT);
	
				AddParameter(cmd, pInt32Out(ProductDiscountBase.Property_Id));
				AddCommonParams(cmd, productDiscountObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					productDiscountObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					productDiscountObject.Id = (Int32)GetOutParameter(cmd, ProductDiscountBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(productDiscountObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates ProductDiscount
        /// </summary>
        /// <param name="productDiscountObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(ProductDiscountBase productDiscountObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEPRODUCTDISCOUNT);
				
				AddParameter(cmd, pInt32(ProductDiscountBase.Property_Id, productDiscountObject.Id));
				AddCommonParams(cmd, productDiscountObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					productDiscountObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(productDiscountObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes ProductDiscount
        /// </summary>
        /// <param name="Id">Id of the ProductDiscount object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEPRODUCTDISCOUNT);	
				
				AddParameter(cmd, pInt32(ProductDiscountBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(ProductDiscount), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves ProductDiscount object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the ProductDiscount object to retrieve</param>
        /// <returns>ProductDiscount object, null if not found</returns>
		public ProductDiscount Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTDISCOUNTBYID))
			{
				AddParameter( cmd, pInt32(ProductDiscountBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all ProductDiscount objects 
        /// </summary>
        /// <returns>A list of ProductDiscount objects</returns>
		public ProductDiscountList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLPRODUCTDISCOUNT))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all ProductDiscount objects by ProductId
        /// </summary>
        /// <returns>A list of ProductDiscount objects</returns>
		public ProductDiscountList GetByProductId(Int32 _ProductId)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTDISCOUNTBYPRODUCTID))
			{
				
				AddParameter( cmd, pInt32(ProductDiscountBase.Property_ProductId, _ProductId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all ProductDiscount objects by PageRequest
        /// </summary>
        /// <returns>A list of ProductDiscount objects</returns>
		public ProductDiscountList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDPRODUCTDISCOUNT))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				ProductDiscountList _ProductDiscountList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _ProductDiscountList;
			}
		}
		
		/// <summary>
        /// Retrieves all ProductDiscount objects by query String
        /// </summary>
        /// <returns>A list of ProductDiscount objects</returns>
		public ProductDiscountList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTDISCOUNTBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get ProductDiscount Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ProductDiscount
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTDISCOUNTMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get ProductDiscount Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ProductDiscount
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _ProductDiscountRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTDISCOUNTROWCOUNT))
			{
				SqlDataReader reader;
				_ProductDiscountRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _ProductDiscountRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills ProductDiscount object
        /// </summary>
        /// <param name="productDiscountObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(ProductDiscountBase productDiscountObject, SqlDataReader reader, int start)
		{
			
				productDiscountObject.Id = reader.GetInt32( start + 0 );			
				productDiscountObject.ProductId = reader.GetInt32( start + 1 );			
				productDiscountObject.DiscountType = reader.GetString( start + 2 );			
				productDiscountObject.DiscountValue = reader.GetDecimal( start + 3 );			
				if(!reader.IsDBNull(4)) productDiscountObject.MinQuantity = reader.GetInt32( start + 4 );			
				productDiscountObject.EffectiveFrom = reader.GetDateTime( start + 5 );			
				if(!reader.IsDBNull(6)) productDiscountObject.EffectiveTo = reader.GetDateTime( start + 6 );			
				productDiscountObject.IsActive = reader.GetBoolean( start + 7 );			
				if(!reader.IsDBNull(8)) productDiscountObject.CreatedBy = reader.GetString( start + 8 );			
				productDiscountObject.CreatedAt = reader.GetDateTime( start + 9 );			
				if(!reader.IsDBNull(10)) productDiscountObject.UpdatedBy = reader.GetString( start + 10 );			
				if(!reader.IsDBNull(11)) productDiscountObject.UpdatedAt = reader.GetDateTime( start + 11 );			
			FillBaseObject(productDiscountObject, reader, (start + 12));

			
			productDiscountObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills ProductDiscount object
        /// </summary>
        /// <param name="productDiscountObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(ProductDiscountBase productDiscountObject, SqlDataReader reader)
		{
			FillObject(productDiscountObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves ProductDiscount object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>ProductDiscount object</returns>
		private ProductDiscount GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					ProductDiscount productDiscountObject= new ProductDiscount();
					FillObject(productDiscountObject, reader);
					return productDiscountObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of ProductDiscount objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of ProductDiscount objects</returns>
		private ProductDiscountList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//ProductDiscount list
			ProductDiscountList list = new ProductDiscountList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					ProductDiscount productDiscountObject = new ProductDiscount();
					FillObject(productDiscountObject, reader);

					list.Add(productDiscountObject);
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