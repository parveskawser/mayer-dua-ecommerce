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
	public partial class ProductAttributeDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTPRODUCTATTRIBUTE = "InsertProductAttribute";
		private const string UPDATEPRODUCTATTRIBUTE = "UpdateProductAttribute";
		private const string DELETEPRODUCTATTRIBUTE = "DeleteProductAttribute";
		private const string GETPRODUCTATTRIBUTEBYID = "GetProductAttributeById";
		private const string GETALLPRODUCTATTRIBUTE = "GetAllProductAttribute";
		private const string GETPAGEDPRODUCTATTRIBUTE = "GetPagedProductAttribute";
		private const string GETPRODUCTATTRIBUTEBYPRODUCTID = "GetProductAttributeByProductId";
		private const string GETPRODUCTATTRIBUTEBYATTRIBUTEID = "GetProductAttributeByAttributeId";
		private const string GETPRODUCTATTRIBUTEMAXIMUMID = "GetProductAttributeMaximumId";
		private const string GETPRODUCTATTRIBUTEROWCOUNT = "GetProductAttributeRowCount";	
		private const string GETPRODUCTATTRIBUTEBYQUERY = "GetProductAttributeByQuery";
		#endregion
		
		#region Constructors
		public ProductAttributeDataAccess(IConfiguration configuration) : base(configuration) { }
		public ProductAttributeDataAccess(ClientContext context) : base(context) { }
		public ProductAttributeDataAccess(SqlTransaction transaction) : base(transaction) { }
		public ProductAttributeDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="productAttributeObject"></param>
		private void AddCommonParams(SqlCommand cmd, ProductAttributeBase productAttributeObject)
		{	
			AddParameter(cmd, pInt32(ProductAttributeBase.Property_ProductId, productAttributeObject.ProductId));
			AddParameter(cmd, pInt32(ProductAttributeBase.Property_AttributeId, productAttributeObject.AttributeId));
			AddParameter(cmd, pInt32(ProductAttributeBase.Property_DisplayOrder, productAttributeObject.DisplayOrder));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts ProductAttribute
        /// </summary>
        /// <param name="productAttributeObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(ProductAttributeBase productAttributeObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTPRODUCTATTRIBUTE);
	
				AddParameter(cmd, pInt32Out(ProductAttributeBase.Property_Id));
				AddCommonParams(cmd, productAttributeObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					productAttributeObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					productAttributeObject.Id = (Int32)GetOutParameter(cmd, ProductAttributeBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(productAttributeObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates ProductAttribute
        /// </summary>
        /// <param name="productAttributeObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(ProductAttributeBase productAttributeObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEPRODUCTATTRIBUTE);
				
				AddParameter(cmd, pInt32(ProductAttributeBase.Property_Id, productAttributeObject.Id));
				AddCommonParams(cmd, productAttributeObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					productAttributeObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(productAttributeObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes ProductAttribute
        /// </summary>
        /// <param name="Id">Id of the ProductAttribute object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEPRODUCTATTRIBUTE);	
				
				AddParameter(cmd, pInt32(ProductAttributeBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(ProductAttribute), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves ProductAttribute object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the ProductAttribute object to retrieve</param>
        /// <returns>ProductAttribute object, null if not found</returns>
		public ProductAttribute Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTATTRIBUTEBYID))
			{
				AddParameter( cmd, pInt32(ProductAttributeBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all ProductAttribute objects 
        /// </summary>
        /// <returns>A list of ProductAttribute objects</returns>
		public ProductAttributeList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLPRODUCTATTRIBUTE))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all ProductAttribute objects by ProductId
        /// </summary>
        /// <returns>A list of ProductAttribute objects</returns>
		public ProductAttributeList GetByProductId(Int32 _ProductId)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTATTRIBUTEBYPRODUCTID))
			{
				
				AddParameter( cmd, pInt32(ProductAttributeBase.Property_ProductId, _ProductId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all ProductAttribute objects by AttributeId
        /// </summary>
        /// <returns>A list of ProductAttribute objects</returns>
		public ProductAttributeList GetByAttributeId(Int32 _AttributeId)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTATTRIBUTEBYATTRIBUTEID))
			{
				
				AddParameter( cmd, pInt32(ProductAttributeBase.Property_AttributeId, _AttributeId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all ProductAttribute objects by PageRequest
        /// </summary>
        /// <returns>A list of ProductAttribute objects</returns>
		public ProductAttributeList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDPRODUCTATTRIBUTE))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				ProductAttributeList _ProductAttributeList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _ProductAttributeList;
			}
		}
		
		/// <summary>
        /// Retrieves all ProductAttribute objects by query String
        /// </summary>
        /// <returns>A list of ProductAttribute objects</returns>
		public ProductAttributeList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTATTRIBUTEBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get ProductAttribute Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ProductAttribute
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTATTRIBUTEMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get ProductAttribute Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ProductAttribute
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _ProductAttributeRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPRODUCTATTRIBUTEROWCOUNT))
			{
				SqlDataReader reader;
				_ProductAttributeRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _ProductAttributeRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills ProductAttribute object
        /// </summary>
        /// <param name="productAttributeObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(ProductAttributeBase productAttributeObject, SqlDataReader reader, int start)
		{
			
				productAttributeObject.Id = reader.GetInt32( start + 0 );			
				productAttributeObject.ProductId = reader.GetInt32( start + 1 );			
				productAttributeObject.AttributeId = reader.GetInt32( start + 2 );			
				productAttributeObject.DisplayOrder = reader.GetInt32( start + 3 );			
			FillBaseObject(productAttributeObject, reader, (start + 4));

			
			productAttributeObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills ProductAttribute object
        /// </summary>
        /// <param name="productAttributeObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(ProductAttributeBase productAttributeObject, SqlDataReader reader)
		{
			FillObject(productAttributeObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves ProductAttribute object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>ProductAttribute object</returns>
		private ProductAttribute GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					ProductAttribute productAttributeObject= new ProductAttribute();
					FillObject(productAttributeObject, reader);
					return productAttributeObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of ProductAttribute objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of ProductAttribute objects</returns>
		private ProductAttributeList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//ProductAttribute list
			ProductAttributeList list = new ProductAttributeList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					ProductAttribute productAttributeObject = new ProductAttribute();
					FillObject(productAttributeObject, reader);

					list.Add(productAttributeObject);
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