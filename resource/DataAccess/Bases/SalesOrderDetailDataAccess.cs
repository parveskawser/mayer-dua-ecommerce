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
	public partial class SalesOrderDetailDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTSALESORDERDETAIL = "InsertSalesOrderDetail";
		private const string UPDATESALESORDERDETAIL = "UpdateSalesOrderDetail";
		private const string DELETESALESORDERDETAIL = "DeleteSalesOrderDetail";
		private const string GETSALESORDERDETAILBYID = "GetSalesOrderDetailById";
		private const string GETALLSALESORDERDETAIL = "GetAllSalesOrderDetail";
		private const string GETPAGEDSALESORDERDETAIL = "GetPagedSalesOrderDetail";
		private const string GETSALESORDERDETAILBYSALESORDERID = "GetSalesOrderDetailBySalesOrderId";
		private const string GETSALESORDERDETAILBYPRODUCTID = "GetSalesOrderDetailByProductId";
		private const string GETSALESORDERDETAILMAXIMUMID = "GetSalesOrderDetailMaximumId";
		private const string GETSALESORDERDETAILROWCOUNT = "GetSalesOrderDetailRowCount";	
		private const string GETSALESORDERDETAILBYQUERY = "GetSalesOrderDetailByQuery";
		#endregion
		
		#region Constructors
		public SalesOrderDetailDataAccess(IConfiguration configuration) : base(configuration) { }
		public SalesOrderDetailDataAccess(ClientContext context) : base(context) { }
		public SalesOrderDetailDataAccess(SqlTransaction transaction) : base(transaction) { }
		public SalesOrderDetailDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="salesOrderDetailObject"></param>
		private void AddCommonParams(SqlCommand cmd, SalesOrderDetailBase salesOrderDetailObject)
		{	
			AddParameter(cmd, pInt32(SalesOrderDetailBase.Property_SalesOrderId, salesOrderDetailObject.SalesOrderId));
			AddParameter(cmd, pInt32(SalesOrderDetailBase.Property_ProductId, salesOrderDetailObject.ProductId));
			AddParameter(cmd, pInt32(SalesOrderDetailBase.Property_Quantity, salesOrderDetailObject.Quantity));
			AddParameter(cmd, pDecimal(SalesOrderDetailBase.Property_UnitPrice, 9, salesOrderDetailObject.UnitPrice));
			AddParameter(cmd, pDecimal(SalesOrderDetailBase.Property_LineTotal, 17, salesOrderDetailObject.LineTotal));
			AddParameter(cmd, pDecimal(SalesOrderDetailBase.Property_ProfitAmount, 9, salesOrderDetailObject.ProfitAmount));
			AddParameter(cmd, pNVarChar(SalesOrderDetailBase.Property_CreatedBy, 100, salesOrderDetailObject.CreatedBy));
			AddParameter(cmd, pDateTime(SalesOrderDetailBase.Property_CreatedAt, salesOrderDetailObject.CreatedAt));
			AddParameter(cmd, pNVarChar(SalesOrderDetailBase.Property_UpdatedBy, 100, salesOrderDetailObject.UpdatedBy));
			AddParameter(cmd, pDateTime(SalesOrderDetailBase.Property_UpdatedAt, salesOrderDetailObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts SalesOrderDetail
        /// </summary>
        /// <param name="salesOrderDetailObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(SalesOrderDetailBase salesOrderDetailObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTSALESORDERDETAIL);
	
				AddParameter(cmd, pInt32Out(SalesOrderDetailBase.Property_Id));
				AddCommonParams(cmd, salesOrderDetailObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					salesOrderDetailObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					salesOrderDetailObject.Id = (Int32)GetOutParameter(cmd, SalesOrderDetailBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(salesOrderDetailObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates SalesOrderDetail
        /// </summary>
        /// <param name="salesOrderDetailObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(SalesOrderDetailBase salesOrderDetailObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATESALESORDERDETAIL);
				
				AddParameter(cmd, pInt32(SalesOrderDetailBase.Property_Id, salesOrderDetailObject.Id));
				AddCommonParams(cmd, salesOrderDetailObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					salesOrderDetailObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(salesOrderDetailObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes SalesOrderDetail
        /// </summary>
        /// <param name="Id">Id of the SalesOrderDetail object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETESALESORDERDETAIL);	
				
				AddParameter(cmd, pInt32(SalesOrderDetailBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(SalesOrderDetail), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves SalesOrderDetail object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the SalesOrderDetail object to retrieve</param>
        /// <returns>SalesOrderDetail object, null if not found</returns>
		public SalesOrderDetail Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETSALESORDERDETAILBYID))
			{
				AddParameter( cmd, pInt32(SalesOrderDetailBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all SalesOrderDetail objects 
        /// </summary>
        /// <returns>A list of SalesOrderDetail objects</returns>
		public SalesOrderDetailList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLSALESORDERDETAIL))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all SalesOrderDetail objects by SalesOrderId
        /// </summary>
        /// <returns>A list of SalesOrderDetail objects</returns>
		public SalesOrderDetailList GetBySalesOrderId(Int32 _SalesOrderId)
		{
			using( SqlCommand cmd = GetSPCommand(GETSALESORDERDETAILBYSALESORDERID))
			{
				
				AddParameter( cmd, pInt32(SalesOrderDetailBase.Property_SalesOrderId, _SalesOrderId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all SalesOrderDetail objects by ProductId
        /// </summary>
        /// <returns>A list of SalesOrderDetail objects</returns>
		public SalesOrderDetailList GetByProductId(Int32 _ProductId)
		{
			using( SqlCommand cmd = GetSPCommand(GETSALESORDERDETAILBYPRODUCTID))
			{
				
				AddParameter( cmd, pInt32(SalesOrderDetailBase.Property_ProductId, _ProductId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all SalesOrderDetail objects by PageRequest
        /// </summary>
        /// <returns>A list of SalesOrderDetail objects</returns>
		public SalesOrderDetailList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDSALESORDERDETAIL))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				SalesOrderDetailList _SalesOrderDetailList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _SalesOrderDetailList;
			}
		}
		
		/// <summary>
        /// Retrieves all SalesOrderDetail objects by query String
        /// </summary>
        /// <returns>A list of SalesOrderDetail objects</returns>
		public SalesOrderDetailList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETSALESORDERDETAILBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get SalesOrderDetail Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of SalesOrderDetail
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETSALESORDERDETAILMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get SalesOrderDetail Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of SalesOrderDetail
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _SalesOrderDetailRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETSALESORDERDETAILROWCOUNT))
			{
				SqlDataReader reader;
				_SalesOrderDetailRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _SalesOrderDetailRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills SalesOrderDetail object
        /// </summary>
        /// <param name="salesOrderDetailObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(SalesOrderDetailBase salesOrderDetailObject, SqlDataReader reader, int start)
		{
			
				salesOrderDetailObject.Id = reader.GetInt32( start + 0 );			
				salesOrderDetailObject.SalesOrderId = reader.GetInt32( start + 1 );			
				salesOrderDetailObject.ProductId = reader.GetInt32( start + 2 );			
				salesOrderDetailObject.Quantity = reader.GetInt32( start + 3 );			
				salesOrderDetailObject.UnitPrice = reader.GetDecimal( start + 4 );			
				if(!reader.IsDBNull(5)) salesOrderDetailObject.LineTotal = reader.GetDecimal( start + 5 );			
				if(!reader.IsDBNull(6)) salesOrderDetailObject.ProfitAmount = reader.GetDecimal( start + 6 );			
				if(!reader.IsDBNull(7)) salesOrderDetailObject.CreatedBy = reader.GetString( start + 7 );			
				salesOrderDetailObject.CreatedAt = reader.GetDateTime( start + 8 );			
				if(!reader.IsDBNull(9)) salesOrderDetailObject.UpdatedBy = reader.GetString( start + 9 );			
				if(!reader.IsDBNull(10)) salesOrderDetailObject.UpdatedAt = reader.GetDateTime( start + 10 );			
			FillBaseObject(salesOrderDetailObject, reader, (start + 11));

			
			salesOrderDetailObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills SalesOrderDetail object
        /// </summary>
        /// <param name="salesOrderDetailObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(SalesOrderDetailBase salesOrderDetailObject, SqlDataReader reader)
		{
			FillObject(salesOrderDetailObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves SalesOrderDetail object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>SalesOrderDetail object</returns>
		private SalesOrderDetail GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					SalesOrderDetail salesOrderDetailObject= new SalesOrderDetail();
					FillObject(salesOrderDetailObject, reader);
					return salesOrderDetailObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of SalesOrderDetail objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of SalesOrderDetail objects</returns>
		private SalesOrderDetailList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//SalesOrderDetail list
			SalesOrderDetailList list = new SalesOrderDetailList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					SalesOrderDetail salesOrderDetailObject = new SalesOrderDetail();
					FillObject(salesOrderDetailObject, reader);

					list.Add(salesOrderDetailObject);
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