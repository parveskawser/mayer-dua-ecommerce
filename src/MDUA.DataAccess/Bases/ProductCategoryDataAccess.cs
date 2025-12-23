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
	public partial class ProductCategoryDataAccess : BaseDataAccess, IProductCategoryDataAccess
	{
		#region Constants
		private const string INSERTPRODUCTCATEGORY = "InsertProductCategory";
		private const string UPDATEPRODUCTCATEGORY = "UpdateProductCategory";
		private const string DELETEPRODUCTCATEGORY = "DeleteProductCategory";
		private const string GETPRODUCTCATEGORYBYID = "GetProductCategoryById";
		private const string GETALLPRODUCTCATEGORY = "GetAllProductCategory";
		private const string GETPAGEDPRODUCTCATEGORY = "GetPagedProductCategory";
		private const string GETPRODUCTCATEGORYMAXIMUMID = "GetProductCategoryMaximumId";
		private const string GETPRODUCTCATEGORYROWCOUNT = "GetProductCategoryRowCount";	
		private const string GETPRODUCTCATEGORYBYQUERY = "GetProductCategoryByQuery";
		#endregion
		
		#region Constructors
		public ProductCategoryDataAccess(IConfiguration configuration) : base(configuration) { }
		public ProductCategoryDataAccess(ClientContext context) : base(context) { }
		public ProductCategoryDataAccess(SqlTransaction transaction) : base(transaction) { }
		public ProductCategoryDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="productCategoryObject"></param>
		private void AddCommonParams(SqlCommand cmd, ProductCategoryBase productCategoryObject)
		{	
			AddParameter(cmd, pNVarChar(ProductCategoryBase.Property_Name, 50, productCategoryObject.Name));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts ProductCategory
        /// </summary>
        /// <param name="productCategoryObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(ProductCategoryBase productCategoryObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTPRODUCTCATEGORY);
	
				AddParameter(cmd, pInt32Out(ProductCategoryBase.Property_Id));
				AddCommonParams(cmd, productCategoryObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					productCategoryObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					productCategoryObject.Id = (Int32)GetOutParameter(cmd, ProductCategoryBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(productCategoryObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates ProductCategory
        /// </summary>
        /// <param name="productCategoryObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(ProductCategoryBase productCategoryObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEPRODUCTCATEGORY);
				
				AddParameter(cmd, pInt32(ProductCategoryBase.Property_Id, productCategoryObject.Id));
				AddCommonParams(cmd, productCategoryObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					productCategoryObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(productCategoryObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes ProductCategory
        /// </summary>
        /// <param name="Id">Id of the ProductCategory object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEPRODUCTCATEGORY);	
				
				AddParameter(cmd, pInt32(ProductCategoryBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(ProductCategory), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves ProductCategory object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the ProductCategory object to retrieve</param>
        /// <returns>ProductCategory object, null if not found</returns>
		public ProductCategory Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTCATEGORYBYID))
			{
				AddParameter( cmd, pInt32(ProductCategoryBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all ProductCategory objects 
        /// </summary>
        /// <returns>A list of ProductCategory objects</returns>
		public ProductCategoryList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLPRODUCTCATEGORY))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all ProductCategory objects by PageRequest
        /// </summary>
        /// <returns>A list of ProductCategory objects</returns>
		public ProductCategoryList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDPRODUCTCATEGORY))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				ProductCategoryList _ProductCategoryList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _ProductCategoryList;
			}
		}
		
		/// <summary>
        /// Retrieves all ProductCategory objects by query String
        /// </summary>
        /// <returns>A list of ProductCategory objects</returns>
		public ProductCategoryList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTCATEGORYBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get ProductCategory Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ProductCategory
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTCATEGORYMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get ProductCategory Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ProductCategory
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _ProductCategoryRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTCATEGORYROWCOUNT))
			{
				SqlDataReader reader;
				_ProductCategoryRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _ProductCategoryRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills ProductCategory object
        /// </summary>
        /// <param name="productCategoryObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(ProductCategoryBase productCategoryObject, SqlDataReader reader, int start)
		{
			
				productCategoryObject.Id = reader.GetInt32( start + 0 );			
				productCategoryObject.Name = reader.GetString( start + 1 );			
			FillBaseObject(productCategoryObject, reader, (start + 2));

			
			productCategoryObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills ProductCategory object
        /// </summary>
        /// <param name="productCategoryObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(ProductCategoryBase productCategoryObject, SqlDataReader reader)
		{
			FillObject(productCategoryObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves ProductCategory object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>ProductCategory object</returns>
		private ProductCategory GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					ProductCategory productCategoryObject= new ProductCategory();
					FillObject(productCategoryObject, reader);
					return productCategoryObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of ProductCategory objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of ProductCategory objects</returns>
		private ProductCategoryList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//ProductCategory list
			ProductCategoryList list = new ProductCategoryList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					ProductCategory productCategoryObject = new ProductCategory();
					FillObject(productCategoryObject, reader);

					list.Add(productCategoryObject);
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
