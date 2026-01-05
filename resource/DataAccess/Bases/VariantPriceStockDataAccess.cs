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
	public partial class VariantPriceStockDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTVARIANTPRICESTOCK = "InsertVariantPriceStock";
		private const string UPDATEVARIANTPRICESTOCK = "UpdateVariantPriceStock";
		private const string DELETEVARIANTPRICESTOCK = "DeleteVariantPriceStock";
		private const string GETVARIANTPRICESTOCKBYID = "GetVariantPriceStockById";
		private const string GETALLVARIANTPRICESTOCK = "GetAllVariantPriceStock";
		private const string GETPAGEDVARIANTPRICESTOCK = "GetPagedVariantPriceStock";
		private const string GETVARIANTPRICESTOCKMAXIMUMID = "GetVariantPriceStockMaximumId";
		private const string GETVARIANTPRICESTOCKROWCOUNT = "GetVariantPriceStockRowCount";	
		private const string GETVARIANTPRICESTOCKBYQUERY = "GetVariantPriceStockByQuery";
		#endregion
		
		#region Constructors
		public VariantPriceStockDataAccess(IConfiguration configuration) : base(configuration) { }
		public VariantPriceStockDataAccess(ClientContext context) : base(context) { }
		public VariantPriceStockDataAccess(SqlTransaction transaction) : base(transaction) { }
		public VariantPriceStockDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="variantPriceStockObject"></param>
		private void AddCommonParams(SqlCommand cmd, VariantPriceStockBase variantPriceStockObject)
		{	
			AddParameter(cmd, pDecimal(VariantPriceStockBase.Property_Price, 9, variantPriceStockObject.Price));
			AddParameter(cmd, pDecimal(VariantPriceStockBase.Property_CompareAtPrice, 9, variantPriceStockObject.CompareAtPrice));
			AddParameter(cmd, pDecimal(VariantPriceStockBase.Property_CostPrice, 9, variantPriceStockObject.CostPrice));
			AddParameter(cmd, pInt32(VariantPriceStockBase.Property_StockQty, variantPriceStockObject.StockQty));
			AddParameter(cmd, pBool(VariantPriceStockBase.Property_TrackInventory, variantPriceStockObject.TrackInventory));
			AddParameter(cmd, pBool(VariantPriceStockBase.Property_AllowBackorder, variantPriceStockObject.AllowBackorder));
			AddParameter(cmd, pInt32(VariantPriceStockBase.Property_WeightGrams, variantPriceStockObject.WeightGrams));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts VariantPriceStock
        /// </summary>
        /// <param name="variantPriceStockObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(VariantPriceStockBase variantPriceStockObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTVARIANTPRICESTOCK);
	
				AddParameter(cmd, pInt32(VariantPriceStockBase.Property_Id, variantPriceStockObject.Id));
				AddCommonParams(cmd, variantPriceStockObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					variantPriceStockObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(variantPriceStockObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates VariantPriceStock
        /// </summary>
        /// <param name="variantPriceStockObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(VariantPriceStockBase variantPriceStockObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEVARIANTPRICESTOCK);
				
				AddParameter(cmd, pInt32(VariantPriceStockBase.Property_Id, variantPriceStockObject.Id));
				AddCommonParams(cmd, variantPriceStockObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					variantPriceStockObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(variantPriceStockObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes VariantPriceStock
        /// </summary>
        /// <param name="Id">Id of the VariantPriceStock object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEVARIANTPRICESTOCK);	
				
				AddParameter(cmd, pInt32(VariantPriceStockBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(VariantPriceStock), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves VariantPriceStock object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the VariantPriceStock object to retrieve</param>
        /// <returns>VariantPriceStock object, null if not found</returns>
		public VariantPriceStock Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETVARIANTPRICESTOCKBYID))
			{
				AddParameter( cmd, pInt32(VariantPriceStockBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all VariantPriceStock objects 
        /// </summary>
        /// <returns>A list of VariantPriceStock objects</returns>
		public VariantPriceStockList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLVARIANTPRICESTOCK))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all VariantPriceStock objects by PageRequest
        /// </summary>
        /// <returns>A list of VariantPriceStock objects</returns>
		public VariantPriceStockList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDVARIANTPRICESTOCK))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				VariantPriceStockList _VariantPriceStockList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _VariantPriceStockList;
			}
		}
		
		/// <summary>
        /// Retrieves all VariantPriceStock objects by query String
        /// </summary>
        /// <returns>A list of VariantPriceStock objects</returns>
		public VariantPriceStockList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETVARIANTPRICESTOCKBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get VariantPriceStock Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of VariantPriceStock
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETVARIANTPRICESTOCKMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get VariantPriceStock Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of VariantPriceStock
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _VariantPriceStockRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETVARIANTPRICESTOCKROWCOUNT))
			{
				SqlDataReader reader;
				_VariantPriceStockRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _VariantPriceStockRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills VariantPriceStock object
        /// </summary>
        /// <param name="variantPriceStockObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(VariantPriceStockBase variantPriceStockObject, SqlDataReader reader, int start)
		{
			
				variantPriceStockObject.Id = reader.GetInt32( start + 0 );			
				variantPriceStockObject.Price = reader.GetDecimal( start + 1 );			
				if(!reader.IsDBNull(2)) variantPriceStockObject.CompareAtPrice = reader.GetDecimal( start + 2 );			
				if(!reader.IsDBNull(3)) variantPriceStockObject.CostPrice = reader.GetDecimal( start + 3 );			
				variantPriceStockObject.StockQty = reader.GetInt32( start + 4 );			
				variantPriceStockObject.TrackInventory = reader.GetBoolean( start + 5 );			
				variantPriceStockObject.AllowBackorder = reader.GetBoolean( start + 6 );			
				if(!reader.IsDBNull(7)) variantPriceStockObject.WeightGrams = reader.GetInt32( start + 7 );			
			FillBaseObject(variantPriceStockObject, reader, (start + 8));

			
			variantPriceStockObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills VariantPriceStock object
        /// </summary>
        /// <param name="variantPriceStockObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(VariantPriceStockBase variantPriceStockObject, SqlDataReader reader)
		{
			FillObject(variantPriceStockObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves VariantPriceStock object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>VariantPriceStock object</returns>
		private VariantPriceStock GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					VariantPriceStock variantPriceStockObject= new VariantPriceStock();
					FillObject(variantPriceStockObject, reader);
					return variantPriceStockObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of VariantPriceStock objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of VariantPriceStock objects</returns>
		private VariantPriceStockList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//VariantPriceStock list
			VariantPriceStockList list = new VariantPriceStockList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					VariantPriceStock variantPriceStockObject = new VariantPriceStock();
					FillObject(variantPriceStockObject, reader);

					list.Add(variantPriceStockObject);
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