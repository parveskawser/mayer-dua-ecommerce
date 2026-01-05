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
	public partial class ProductImageDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTPRODUCTIMAGE = "InsertProductImage";
		private const string UPDATEPRODUCTIMAGE = "UpdateProductImage";
		private const string DELETEPRODUCTIMAGE = "DeleteProductImage";
		private const string GETPRODUCTIMAGEBYID = "GetProductImageById";
		private const string GETALLPRODUCTIMAGE = "GetAllProductImage";
		private const string GETPAGEDPRODUCTIMAGE = "GetPagedProductImage";
		private const string GETPRODUCTIMAGEBYPRODUCTID = "GetProductImageByProductId";
		private const string GETPRODUCTIMAGEMAXIMUMID = "GetProductImageMaximumId";
		private const string GETPRODUCTIMAGEROWCOUNT = "GetProductImageRowCount";	
		private const string GETPRODUCTIMAGEBYQUERY = "GetProductImageByQuery";
		#endregion
		
		#region Constructors
		public ProductImageDataAccess(IConfiguration configuration) : base(configuration) { }
		public ProductImageDataAccess(ClientContext context) : base(context) { }
		public ProductImageDataAccess(SqlTransaction transaction) : base(transaction) { }
		public ProductImageDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="productImageObject"></param>
		private void AddCommonParams(SqlCommand cmd, ProductImageBase productImageObject)
		{	
			AddParameter(cmd, pInt32(ProductImageBase.Property_ProductId, productImageObject.ProductId));
			AddParameter(cmd, pNVarChar(ProductImageBase.Property_ImageUrl, 300, productImageObject.ImageUrl));
			AddParameter(cmd, pBool(ProductImageBase.Property_IsPrimary, productImageObject.IsPrimary));
			AddParameter(cmd, pInt32(ProductImageBase.Property_SortOrder, productImageObject.SortOrder));
			AddParameter(cmd, pNVarChar(ProductImageBase.Property_AltText, 100, productImageObject.AltText));
			AddParameter(cmd, pNVarChar(ProductImageBase.Property_CreatedBy, 100, productImageObject.CreatedBy));
			AddParameter(cmd, pDateTime(ProductImageBase.Property_CreatedAt, productImageObject.CreatedAt));
			AddParameter(cmd, pNVarChar(ProductImageBase.Property_UpdatedBy, 100, productImageObject.UpdatedBy));
			AddParameter(cmd, pDateTime(ProductImageBase.Property_UpdatedAt, productImageObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts ProductImage
        /// </summary>
        /// <param name="productImageObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(ProductImageBase productImageObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTPRODUCTIMAGE);
	
				AddParameter(cmd, pInt32Out(ProductImageBase.Property_Id));
				AddCommonParams(cmd, productImageObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					productImageObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					productImageObject.Id = (Int32)GetOutParameter(cmd, ProductImageBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(productImageObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates ProductImage
        /// </summary>
        /// <param name="productImageObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(ProductImageBase productImageObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEPRODUCTIMAGE);
				
				AddParameter(cmd, pInt32(ProductImageBase.Property_Id, productImageObject.Id));
				AddCommonParams(cmd, productImageObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					productImageObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(productImageObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes ProductImage
        /// </summary>
        /// <param name="Id">Id of the ProductImage object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEPRODUCTIMAGE);	
				
				AddParameter(cmd, pInt32(ProductImageBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(ProductImage), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves ProductImage object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the ProductImage object to retrieve</param>
        /// <returns>ProductImage object, null if not found</returns>
		public ProductImage Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTIMAGEBYID))
			{
				AddParameter( cmd, pInt32(ProductImageBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all ProductImage objects 
        /// </summary>
        /// <returns>A list of ProductImage objects</returns>
		public ProductImageList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLPRODUCTIMAGE))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all ProductImage objects by ProductId
        /// </summary>
        /// <returns>A list of ProductImage objects</returns>
		public ProductImageList GetByProductId(Int32 _ProductId)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTIMAGEBYPRODUCTID))
			{
				
				AddParameter( cmd, pInt32(ProductImageBase.Property_ProductId, _ProductId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all ProductImage objects by PageRequest
        /// </summary>
        /// <returns>A list of ProductImage objects</returns>
		public ProductImageList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDPRODUCTIMAGE))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				ProductImageList _ProductImageList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _ProductImageList;
			}
		}
		
		/// <summary>
        /// Retrieves all ProductImage objects by query String
        /// </summary>
        /// <returns>A list of ProductImage objects</returns>
		public ProductImageList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTIMAGEBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get ProductImage Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ProductImage
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTIMAGEMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get ProductImage Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ProductImage
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _ProductImageRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTIMAGEROWCOUNT))
			{
				SqlDataReader reader;
				_ProductImageRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _ProductImageRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills ProductImage object
        /// </summary>
        /// <param name="productImageObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(ProductImageBase productImageObject, SqlDataReader reader, int start)
		{
			
				productImageObject.Id = reader.GetInt32( start + 0 );			
				productImageObject.ProductId = reader.GetInt32( start + 1 );			
				productImageObject.ImageUrl = reader.GetString( start + 2 );			
				productImageObject.IsPrimary = reader.GetBoolean( start + 3 );			
				productImageObject.SortOrder = reader.GetInt32( start + 4 );			
				if(!reader.IsDBNull(5)) productImageObject.AltText = reader.GetString( start + 5 );			
				if(!reader.IsDBNull(6)) productImageObject.CreatedBy = reader.GetString( start + 6 );			
				productImageObject.CreatedAt = reader.GetDateTime( start + 7 );			
				if(!reader.IsDBNull(8)) productImageObject.UpdatedBy = reader.GetString( start + 8 );			
				if(!reader.IsDBNull(9)) productImageObject.UpdatedAt = reader.GetDateTime( start + 9 );			
			FillBaseObject(productImageObject, reader, (start + 10));

			
			productImageObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills ProductImage object
        /// </summary>
        /// <param name="productImageObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(ProductImageBase productImageObject, SqlDataReader reader)
		{
			FillObject(productImageObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves ProductImage object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>ProductImage object</returns>
		private ProductImage GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					ProductImage productImageObject= new ProductImage();
					FillObject(productImageObject, reader);
					return productImageObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of ProductImage objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of ProductImage objects</returns>
		private ProductImageList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//ProductImage list
			ProductImageList list = new ProductImageList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					ProductImage productImageObject = new ProductImage();
					FillObject(productImageObject, reader);

					list.Add(productImageObject);
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