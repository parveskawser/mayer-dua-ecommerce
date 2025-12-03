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
	public partial class ProductReviewDataAccess : BaseDataAccess, IProductReviewDataAccess
	{
		#region Constants
		private const string INSERTPRODUCTREVIEW = "InsertProductReview";
		private const string UPDATEPRODUCTREVIEW = "UpdateProductReview";
		private const string DELETEPRODUCTREVIEW = "DeleteProductReview";
		private const string GETPRODUCTREVIEWBYID = "GetProductReviewById";
		private const string GETALLPRODUCTREVIEW = "GetAllProductReview";
		private const string GETPAGEDPRODUCTREVIEW = "GetPagedProductReview";
		private const string GETPRODUCTREVIEWBYPRODUCTID = "GetProductReviewByProductId";
		private const string GETPRODUCTREVIEWMAXIMUMID = "GetProductReviewMaximumId";
		private const string GETPRODUCTREVIEWROWCOUNT = "GetProductReviewRowCount";	
		private const string GETPRODUCTREVIEWBYQUERY = "GetProductReviewByQuery";
		#endregion
		
		#region Constructors
		public ProductReviewDataAccess(IConfiguration configuration) : base(configuration) { }
		public ProductReviewDataAccess(ClientContext context) : base(context) { }
		public ProductReviewDataAccess(SqlTransaction transaction) : base(transaction) { }
		public ProductReviewDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="productReviewObject"></param>
		private void AddCommonParams(SqlCommand cmd, ProductReviewBase productReviewObject)
		{	
			AddParameter(cmd, pInt32(ProductReviewBase.Property_ProductId, productReviewObject.ProductId));
			AddParameter(cmd, pNVarChar(ProductReviewBase.Property_CustomerName, 150, productReviewObject.CustomerName));
			AddParameter(cmd, pInt32(ProductReviewBase.Property_Rating, productReviewObject.Rating));
			AddParameter(cmd, pNVarChar(ProductReviewBase.Property_ReviewText, 500, productReviewObject.ReviewText));
			AddParameter(cmd, pBool(ProductReviewBase.Property_IsApproved, productReviewObject.IsApproved));
			AddParameter(cmd, pNVarChar(ProductReviewBase.Property_CreatedBy, 100, productReviewObject.CreatedBy));
			AddParameter(cmd, pDateTime(ProductReviewBase.Property_CreatedAt, productReviewObject.CreatedAt));
			AddParameter(cmd, pNVarChar(ProductReviewBase.Property_UpdatedBy, 100, productReviewObject.UpdatedBy));
			AddParameter(cmd, pDateTime(ProductReviewBase.Property_UpdatedAt, productReviewObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts ProductReview
        /// </summary>
        /// <param name="productReviewObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(ProductReviewBase productReviewObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTPRODUCTREVIEW);
	
				AddParameter(cmd, pInt32Out(ProductReviewBase.Property_Id));
				AddCommonParams(cmd, productReviewObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					productReviewObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					productReviewObject.Id = (Int32)GetOutParameter(cmd, ProductReviewBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(productReviewObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates ProductReview
        /// </summary>
        /// <param name="productReviewObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(ProductReviewBase productReviewObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEPRODUCTREVIEW);
				
				AddParameter(cmd, pInt32(ProductReviewBase.Property_Id, productReviewObject.Id));
				AddCommonParams(cmd, productReviewObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					productReviewObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(productReviewObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes ProductReview
        /// </summary>
        /// <param name="Id">Id of the ProductReview object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEPRODUCTREVIEW);	
				
				AddParameter(cmd, pInt32(ProductReviewBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(ProductReview), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves ProductReview object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the ProductReview object to retrieve</param>
        /// <returns>ProductReview object, null if not found</returns>
		public ProductReview Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTREVIEWBYID))
			{
				AddParameter( cmd, pInt32(ProductReviewBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all ProductReview objects 
        /// </summary>
        /// <returns>A list of ProductReview objects</returns>
		public ProductReviewList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLPRODUCTREVIEW))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all ProductReview objects by ProductId
        /// </summary>
        /// <returns>A list of ProductReview objects</returns>
		public ProductReviewList GetByProductId(Int32 _ProductId)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTREVIEWBYPRODUCTID))
			{
				
				AddParameter( cmd, pInt32(ProductReviewBase.Property_ProductId, _ProductId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all ProductReview objects by PageRequest
        /// </summary>
        /// <returns>A list of ProductReview objects</returns>
		public ProductReviewList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDPRODUCTREVIEW))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				ProductReviewList _ProductReviewList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _ProductReviewList;
			}
		}
		
		/// <summary>
        /// Retrieves all ProductReview objects by query String
        /// </summary>
        /// <returns>A list of ProductReview objects</returns>
		public ProductReviewList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTREVIEWBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get ProductReview Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ProductReview
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTREVIEWMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get ProductReview Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ProductReview
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _ProductReviewRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTREVIEWROWCOUNT))
			{
				SqlDataReader reader;
				_ProductReviewRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _ProductReviewRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills ProductReview object
        /// </summary>
        /// <param name="productReviewObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(ProductReviewBase productReviewObject, SqlDataReader reader, int start)
		{
			
				productReviewObject.Id = reader.GetInt32( start + 0 );			
				productReviewObject.ProductId = reader.GetInt32( start + 1 );			
				if(!reader.IsDBNull(2)) productReviewObject.CustomerName = reader.GetString( start + 2 );			
				productReviewObject.Rating = reader.GetInt32( start + 3 );			
				if(!reader.IsDBNull(4)) productReviewObject.ReviewText = reader.GetString( start + 4 );			
				productReviewObject.IsApproved = reader.GetBoolean( start + 5 );			
				if(!reader.IsDBNull(6)) productReviewObject.CreatedBy = reader.GetString( start + 6 );			
				productReviewObject.CreatedAt = reader.GetDateTime( start + 7 );			
				if(!reader.IsDBNull(8)) productReviewObject.UpdatedBy = reader.GetString( start + 8 );			
				if(!reader.IsDBNull(9)) productReviewObject.UpdatedAt = reader.GetDateTime( start + 9 );			
			FillBaseObject(productReviewObject, reader, (start + 10));

			
			productReviewObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills ProductReview object
        /// </summary>
        /// <param name="productReviewObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(ProductReviewBase productReviewObject, SqlDataReader reader)
		{
			FillObject(productReviewObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves ProductReview object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>ProductReview object</returns>
		private ProductReview GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					ProductReview productReviewObject= new ProductReview();
					FillObject(productReviewObject, reader);
					return productReviewObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of ProductReview objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of ProductReview objects</returns>
		private ProductReviewList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//ProductReview list
			ProductReviewList list = new ProductReviewList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					ProductReview productReviewObject = new ProductReview();
					FillObject(productReviewObject, reader);

					list.Add(productReviewObject);
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
