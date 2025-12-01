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
	public partial class ProductInventoryDataAccess : BaseDataAccess, IProductInventoryDataAccess
	{
		#region Constants
		private const string INSERTPRODUCTINVENTORY = "InsertProductInventory";
		private const string UPDATEPRODUCTINVENTORY = "UpdateProductInventory";
		private const string DELETEPRODUCTINVENTORY = "DeleteProductInventory";
		private const string GETPRODUCTINVENTORYBYID = "GetProductInventoryById";
		private const string GETALLPRODUCTINVENTORY = "GetAllProductInventory";
		private const string GETPAGEDPRODUCTINVENTORY = "GetPagedProductInventory";
		private const string GETPRODUCTINVENTORYBYPRODUCTID = "GetProductInventoryByProductId";
		private const string GETPRODUCTINVENTORYMAXIMUMID = "GetProductInventoryMaximumId";
		private const string GETPRODUCTINVENTORYROWCOUNT = "GetProductInventoryRowCount";	
		private const string GETPRODUCTINVENTORYBYQUERY = "GetProductInventoryByQuery";
		#endregion
		
		#region Constructors
		public ProductInventoryDataAccess(IConfiguration configuration) : base(configuration) { }
		public ProductInventoryDataAccess(ClientContext context) : base(context) { }
		public ProductInventoryDataAccess(SqlTransaction transaction) : base(transaction) { }
		public ProductInventoryDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="productInventoryObject"></param>
		private void AddCommonParams(SqlCommand cmd, ProductInventoryBase productInventoryObject)
		{	
			AddParameter(cmd, pInt32(ProductInventoryBase.Property_ProductId, productInventoryObject.ProductId));
			AddParameter(cmd, pInt32(ProductInventoryBase.Property_CurrentStock, productInventoryObject.CurrentStock));
			AddParameter(cmd, pDecimal(ProductInventoryBase.Property_AverageCost, 9, productInventoryObject.AverageCost));
			AddParameter(cmd, pDateTime(ProductInventoryBase.Property_UpdatedAt, productInventoryObject.UpdatedAt));
			AddParameter(cmd, pDecimal(ProductInventoryBase.Property_SuggestedSellingPrice, 9, productInventoryObject.SuggestedSellingPrice));
			AddParameter(cmd, pDateTime(ProductInventoryBase.Property_LastRestockedAt, productInventoryObject.LastRestockedAt));
			AddParameter(cmd, pBool(ProductInventoryBase.Property_ReorderNeeded, productInventoryObject.ReorderNeeded));
			AddParameter(cmd, pNVarChar(ProductInventoryBase.Property_CreatedBy, 100, productInventoryObject.CreatedBy));
			AddParameter(cmd, pDateTime(ProductInventoryBase.Property_CreatedAt, productInventoryObject.CreatedAt));
			AddParameter(cmd, pNVarChar(ProductInventoryBase.Property_UpdatedBy, 100, productInventoryObject.UpdatedBy));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts ProductInventory
        /// </summary>
        /// <param name="productInventoryObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(ProductInventoryBase productInventoryObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTPRODUCTINVENTORY);
	
				AddParameter(cmd, pInt32Out(ProductInventoryBase.Property_Id));
				AddCommonParams(cmd, productInventoryObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					productInventoryObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					productInventoryObject.Id = (Int32)GetOutParameter(cmd, ProductInventoryBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(productInventoryObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates ProductInventory
        /// </summary>
        /// <param name="productInventoryObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(ProductInventoryBase productInventoryObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEPRODUCTINVENTORY);
				
				AddParameter(cmd, pInt32(ProductInventoryBase.Property_Id, productInventoryObject.Id));
				AddCommonParams(cmd, productInventoryObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					productInventoryObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(productInventoryObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes ProductInventory
        /// </summary>
        /// <param name="Id">Id of the ProductInventory object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEPRODUCTINVENTORY);	
				
				AddParameter(cmd, pInt32(ProductInventoryBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(ProductInventory), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves ProductInventory object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the ProductInventory object to retrieve</param>
        /// <returns>ProductInventory object, null if not found</returns>
		public ProductInventory Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTINVENTORYBYID))
			{
				AddParameter( cmd, pInt32(ProductInventoryBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all ProductInventory objects 
        /// </summary>
        /// <returns>A list of ProductInventory objects</returns>
		public ProductInventoryList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLPRODUCTINVENTORY))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all ProductInventory objects by ProductId
        /// </summary>
        /// <returns>A list of ProductInventory objects</returns>
		public ProductInventoryList GetByProductId(Int32 _ProductId)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTINVENTORYBYPRODUCTID))
			{
				
				AddParameter( cmd, pInt32(ProductInventoryBase.Property_ProductId, _ProductId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all ProductInventory objects by PageRequest
        /// </summary>
        /// <returns>A list of ProductInventory objects</returns>
		public ProductInventoryList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDPRODUCTINVENTORY))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				ProductInventoryList _ProductInventoryList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _ProductInventoryList;
			}
		}
		
		/// <summary>
        /// Retrieves all ProductInventory objects by query String
        /// </summary>
        /// <returns>A list of ProductInventory objects</returns>
		public ProductInventoryList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTINVENTORYBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get ProductInventory Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ProductInventory
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTINVENTORYMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get ProductInventory Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ProductInventory
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _ProductInventoryRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTINVENTORYROWCOUNT))
			{
				SqlDataReader reader;
				_ProductInventoryRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _ProductInventoryRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills ProductInventory object
        /// </summary>
        /// <param name="productInventoryObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(ProductInventoryBase productInventoryObject, SqlDataReader reader, int start)
		{
			
				productInventoryObject.Id = reader.GetInt32( start + 0 );			
				productInventoryObject.ProductId = reader.GetInt32( start + 1 );			
				productInventoryObject.CurrentStock = reader.GetInt32( start + 2 );			
				productInventoryObject.AverageCost = reader.GetDecimal( start + 3 );			
				productInventoryObject.UpdatedAt = reader.GetDateTime( start + 4 );			
				productInventoryObject.SuggestedSellingPrice = reader.GetDecimal( start + 5 );			
				if(!reader.IsDBNull(6)) productInventoryObject.LastRestockedAt = reader.GetDateTime( start + 6 );			
				productInventoryObject.ReorderNeeded = reader.GetBoolean( start + 7 );			
				if(!reader.IsDBNull(8)) productInventoryObject.CreatedBy = reader.GetString( start + 8 );			
				if(!reader.IsDBNull(9)) productInventoryObject.CreatedAt = reader.GetDateTime( start + 9 );			
				if(!reader.IsDBNull(10)) productInventoryObject.UpdatedBy = reader.GetString( start + 10 );			
			FillBaseObject(productInventoryObject, reader, (start + 11));

			
			productInventoryObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills ProductInventory object
        /// </summary>
        /// <param name="productInventoryObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(ProductInventoryBase productInventoryObject, SqlDataReader reader)
		{
			FillObject(productInventoryObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves ProductInventory object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>ProductInventory object</returns>
		private ProductInventory GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					ProductInventory productInventoryObject= new ProductInventory();
					FillObject(productInventoryObject, reader);
					return productInventoryObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of ProductInventory objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of ProductInventory objects</returns>
		private ProductInventoryList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//ProductInventory list
			ProductInventoryList list = new ProductInventoryList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					ProductInventory productInventoryObject = new ProductInventory();
					FillObject(productInventoryObject, reader);

					list.Add(productInventoryObject);
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
