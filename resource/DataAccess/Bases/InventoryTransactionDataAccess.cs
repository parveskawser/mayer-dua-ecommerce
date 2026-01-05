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
	public partial class InventoryTransactionDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTINVENTORYTRANSACTION = "InsertInventoryTransaction";
		private const string UPDATEINVENTORYTRANSACTION = "UpdateInventoryTransaction";
		private const string DELETEINVENTORYTRANSACTION = "DeleteInventoryTransaction";
		private const string GETINVENTORYTRANSACTIONBYID = "GetInventoryTransactionById";
		private const string GETALLINVENTORYTRANSACTION = "GetAllInventoryTransaction";
		private const string GETPAGEDINVENTORYTRANSACTION = "GetPagedInventoryTransaction";
		private const string GETINVENTORYTRANSACTIONBYSALESORDERDETAILID = "GetInventoryTransactionBySalesOrderDetailId";
		private const string GETINVENTORYTRANSACTIONBYPORECEIVEDID = "GetInventoryTransactionByPoReceivedId";
		private const string GETINVENTORYTRANSACTIONBYPRODUCTVARIANTID = "GetInventoryTransactionByProductVariantId";
		private const string GETINVENTORYTRANSACTIONMAXIMUMID = "GetInventoryTransactionMaximumId";
		private const string GETINVENTORYTRANSACTIONROWCOUNT = "GetInventoryTransactionRowCount";	
		private const string GETINVENTORYTRANSACTIONBYQUERY = "GetInventoryTransactionByQuery";
		#endregion
		
		#region Constructors
		public InventoryTransactionDataAccess(IConfiguration configuration) : base(configuration) { }
		public InventoryTransactionDataAccess(ClientContext context) : base(context) { }
		public InventoryTransactionDataAccess(SqlTransaction transaction) : base(transaction) { }
		public InventoryTransactionDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="inventoryTransactionObject"></param>
		private void AddCommonParams(SqlCommand cmd, InventoryTransactionBase inventoryTransactionObject)
		{	
			AddParameter(cmd, pInt32(InventoryTransactionBase.Property_SalesOrderDetailId, inventoryTransactionObject.SalesOrderDetailId));
			AddParameter(cmd, pInt32(InventoryTransactionBase.Property_PoReceivedId, inventoryTransactionObject.PoReceivedId));
			AddParameter(cmd, pNVarChar(InventoryTransactionBase.Property_InOut, 3, inventoryTransactionObject.InOut));
			AddParameter(cmd, pDateTime(InventoryTransactionBase.Property_Date, inventoryTransactionObject.Date));
			AddParameter(cmd, pDecimal(InventoryTransactionBase.Property_Price, 9, inventoryTransactionObject.Price));
			AddParameter(cmd, pInt32(InventoryTransactionBase.Property_Quantity, inventoryTransactionObject.Quantity));
			AddParameter(cmd, pNVarChar(InventoryTransactionBase.Property_CreatedBy, 100, inventoryTransactionObject.CreatedBy));
			AddParameter(cmd, pDateTime(InventoryTransactionBase.Property_CreatedAt, inventoryTransactionObject.CreatedAt));
			AddParameter(cmd, pNVarChar(InventoryTransactionBase.Property_UpdatedBy, 100, inventoryTransactionObject.UpdatedBy));
			AddParameter(cmd, pDateTime(InventoryTransactionBase.Property_UpdatedAt, inventoryTransactionObject.UpdatedAt));
			AddParameter(cmd, pNVarChar(InventoryTransactionBase.Property_Remarks, 255, inventoryTransactionObject.Remarks));
			AddParameter(cmd, pInt32(InventoryTransactionBase.Property_ProductVariantId, inventoryTransactionObject.ProductVariantId));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts InventoryTransaction
        /// </summary>
        /// <param name="inventoryTransactionObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(InventoryTransactionBase inventoryTransactionObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTINVENTORYTRANSACTION);
	
				AddParameter(cmd, pInt32Out(InventoryTransactionBase.Property_Id));
				AddCommonParams(cmd, inventoryTransactionObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					inventoryTransactionObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					inventoryTransactionObject.Id = (Int32)GetOutParameter(cmd, InventoryTransactionBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(inventoryTransactionObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates InventoryTransaction
        /// </summary>
        /// <param name="inventoryTransactionObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(InventoryTransactionBase inventoryTransactionObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEINVENTORYTRANSACTION);
				
				AddParameter(cmd, pInt32(InventoryTransactionBase.Property_Id, inventoryTransactionObject.Id));
				AddCommonParams(cmd, inventoryTransactionObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					inventoryTransactionObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(inventoryTransactionObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes InventoryTransaction
        /// </summary>
        /// <param name="Id">Id of the InventoryTransaction object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEINVENTORYTRANSACTION);	
				
				AddParameter(cmd, pInt32(InventoryTransactionBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(InventoryTransaction), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves InventoryTransaction object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the InventoryTransaction object to retrieve</param>
        /// <returns>InventoryTransaction object, null if not found</returns>
		public InventoryTransaction Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETINVENTORYTRANSACTIONBYID))
			{
				AddParameter( cmd, pInt32(InventoryTransactionBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all InventoryTransaction objects 
        /// </summary>
        /// <returns>A list of InventoryTransaction objects</returns>
		public InventoryTransactionList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLINVENTORYTRANSACTION))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all InventoryTransaction objects by SalesOrderDetailId
        /// </summary>
        /// <returns>A list of InventoryTransaction objects</returns>
		public InventoryTransactionList GetBySalesOrderDetailId(Nullable<Int32> _SalesOrderDetailId)
		{
			using( SqlCommand cmd = GetSPCommand(GETINVENTORYTRANSACTIONBYSALESORDERDETAILID))
			{
				
				AddParameter( cmd, pInt32(InventoryTransactionBase.Property_SalesOrderDetailId, _SalesOrderDetailId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all InventoryTransaction objects by PoReceivedId
        /// </summary>
        /// <returns>A list of InventoryTransaction objects</returns>
		public InventoryTransactionList GetByPoReceivedId(Nullable<Int32> _PoReceivedId)
		{
			using( SqlCommand cmd = GetSPCommand(GETINVENTORYTRANSACTIONBYPORECEIVEDID))
			{
				
				AddParameter( cmd, pInt32(InventoryTransactionBase.Property_PoReceivedId, _PoReceivedId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all InventoryTransaction objects by ProductVariantId
        /// </summary>
        /// <returns>A list of InventoryTransaction objects</returns>
		public InventoryTransactionList GetByProductVariantId(Int32 _ProductVariantId)
		{
			using( SqlCommand cmd = GetSPCommand(GETINVENTORYTRANSACTIONBYPRODUCTVARIANTID))
			{
				
				AddParameter( cmd, pInt32(InventoryTransactionBase.Property_ProductVariantId, _ProductVariantId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all InventoryTransaction objects by PageRequest
        /// </summary>
        /// <returns>A list of InventoryTransaction objects</returns>
		public InventoryTransactionList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDINVENTORYTRANSACTION))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				InventoryTransactionList _InventoryTransactionList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _InventoryTransactionList;
			}
		}
		
		/// <summary>
        /// Retrieves all InventoryTransaction objects by query String
        /// </summary>
        /// <returns>A list of InventoryTransaction objects</returns>
		public InventoryTransactionList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETINVENTORYTRANSACTIONBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get InventoryTransaction Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of InventoryTransaction
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETINVENTORYTRANSACTIONMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get InventoryTransaction Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of InventoryTransaction
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _InventoryTransactionRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETINVENTORYTRANSACTIONROWCOUNT))
			{
				SqlDataReader reader;
				_InventoryTransactionRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _InventoryTransactionRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills InventoryTransaction object
        /// </summary>
        /// <param name="inventoryTransactionObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(InventoryTransactionBase inventoryTransactionObject, SqlDataReader reader, int start)
		{
			
				inventoryTransactionObject.Id = reader.GetInt32( start + 0 );			
				if(!reader.IsDBNull(1)) inventoryTransactionObject.SalesOrderDetailId = reader.GetInt32( start + 1 );			
				if(!reader.IsDBNull(2)) inventoryTransactionObject.PoReceivedId = reader.GetInt32( start + 2 );			
				inventoryTransactionObject.InOut = reader.GetString( start + 3 );			
				inventoryTransactionObject.Date = reader.GetDateTime( start + 4 );			
				if(!reader.IsDBNull(5)) inventoryTransactionObject.Price = reader.GetDecimal( start + 5 );			
				inventoryTransactionObject.Quantity = reader.GetInt32( start + 6 );			
				inventoryTransactionObject.CreatedBy = reader.GetString( start + 7 );			
				inventoryTransactionObject.CreatedAt = reader.GetDateTime( start + 8 );			
				if(!reader.IsDBNull(9)) inventoryTransactionObject.UpdatedBy = reader.GetString( start + 9 );			
				if(!reader.IsDBNull(10)) inventoryTransactionObject.UpdatedAt = reader.GetDateTime( start + 10 );			
				if(!reader.IsDBNull(11)) inventoryTransactionObject.Remarks = reader.GetString( start + 11 );			
				inventoryTransactionObject.ProductVariantId = reader.GetInt32( start + 12 );			
			FillBaseObject(inventoryTransactionObject, reader, (start + 13));

			
			inventoryTransactionObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills InventoryTransaction object
        /// </summary>
        /// <param name="inventoryTransactionObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(InventoryTransactionBase inventoryTransactionObject, SqlDataReader reader)
		{
			FillObject(inventoryTransactionObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves InventoryTransaction object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>InventoryTransaction object</returns>
		private InventoryTransaction GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					InventoryTransaction inventoryTransactionObject= new InventoryTransaction();
					FillObject(inventoryTransactionObject, reader);
					return inventoryTransactionObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of InventoryTransaction objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of InventoryTransaction objects</returns>
		private InventoryTransactionList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//InventoryTransaction list
			InventoryTransactionList list = new InventoryTransactionList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					InventoryTransaction inventoryTransactionObject = new InventoryTransaction();
					FillObject(inventoryTransactionObject, reader);

					list.Add(inventoryTransactionObject);
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