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
	public partial class SalesReturnDataAccess : BaseDataAccess, ISalesReturnDataAccess
	{
		#region Constants
		private const string INSERTSALESRETURN = "InsertSalesReturn";
		private const string UPDATESALESRETURN = "UpdateSalesReturn";
		private const string DELETESALESRETURN = "DeleteSalesReturn";
		private const string GETSALESRETURNBYID = "GetSalesReturnById";
		private const string GETALLSALESRETURN = "GetAllSalesReturn";
		private const string GETPAGEDSALESRETURN = "GetPagedSalesReturn";
		private const string GETSALESRETURNBYSALESORDERDETAILID = "GetSalesReturnBySalesOrderDetailId";
		private const string GETSALESRETURNMAXIMUMID = "GetSalesReturnMaximumId";
		private const string GETSALESRETURNROWCOUNT = "GetSalesReturnRowCount";	
		private const string GETSALESRETURNBYQUERY = "GetSalesReturnByQuery";
		#endregion
		
		#region Constructors
		public SalesReturnDataAccess(IConfiguration configuration) : base(configuration) { }
		public SalesReturnDataAccess(ClientContext context) : base(context) { }
		public SalesReturnDataAccess(SqlTransaction transaction) : base(transaction) { }
		public SalesReturnDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="salesReturnObject"></param>
		private void AddCommonParams(SqlCommand cmd, SalesReturnBase salesReturnObject)
		{	
			AddParameter(cmd, pInt32(SalesReturnBase.Property_SalesOrderDetailId, salesReturnObject.SalesOrderDetailId));
			AddParameter(cmd, pDateTime(SalesReturnBase.Property_ReturnDate, salesReturnObject.ReturnDate));
			AddParameter(cmd, pInt32(SalesReturnBase.Property_Quantity, salesReturnObject.Quantity));
			AddParameter(cmd, pNVarChar(SalesReturnBase.Property_Reason, 50, salesReturnObject.Reason));
			AddParameter(cmd, pBool(SalesReturnBase.Property_RestockToInventory, salesReturnObject.RestockToInventory));
			AddParameter(cmd, pDecimal(SalesReturnBase.Property_RefundAmount, 9, salesReturnObject.RefundAmount));
			AddParameter(cmd, pNVarChar(SalesReturnBase.Property_Status, 20, salesReturnObject.Status));
			AddParameter(cmd, pNVarChar(SalesReturnBase.Property_Remarks, 500, salesReturnObject.Remarks));
			AddParameter(cmd, pNVarChar(SalesReturnBase.Property_CreatedBy, 100, salesReturnObject.CreatedBy));
			AddParameter(cmd, pDateTime(SalesReturnBase.Property_CreatedAt, salesReturnObject.CreatedAt));
			AddParameter(cmd, pNVarChar(SalesReturnBase.Property_UpdatedBy, 100, salesReturnObject.UpdatedBy));
			AddParameter(cmd, pDateTime(SalesReturnBase.Property_UpdatedAt, salesReturnObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts SalesReturn
        /// </summary>
        /// <param name="salesReturnObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(SalesReturnBase salesReturnObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTSALESRETURN);
	
				AddParameter(cmd, pInt32Out(SalesReturnBase.Property_Id));
				AddCommonParams(cmd, salesReturnObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					salesReturnObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					salesReturnObject.Id = (Int32)GetOutParameter(cmd, SalesReturnBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(salesReturnObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates SalesReturn
        /// </summary>
        /// <param name="salesReturnObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(SalesReturnBase salesReturnObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATESALESRETURN);
				
				AddParameter(cmd, pInt32(SalesReturnBase.Property_Id, salesReturnObject.Id));
				AddCommonParams(cmd, salesReturnObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					salesReturnObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(salesReturnObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes SalesReturn
        /// </summary>
        /// <param name="Id">Id of the SalesReturn object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETESALESRETURN);	
				
				AddParameter(cmd, pInt32(SalesReturnBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(SalesReturn), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves SalesReturn object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the SalesReturn object to retrieve</param>
        /// <returns>SalesReturn object, null if not found</returns>
		public SalesReturn Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETSALESRETURNBYID))
			{
				AddParameter( cmd, pInt32(SalesReturnBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all SalesReturn objects 
        /// </summary>
        /// <returns>A list of SalesReturn objects</returns>
		public SalesReturnList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLSALESRETURN))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all SalesReturn objects by SalesOrderDetailId
        /// </summary>
        /// <returns>A list of SalesReturn objects</returns>
		public SalesReturnList GetBySalesOrderDetailId(Int32 _SalesOrderDetailId)
		{
			using( SqlCommand cmd = GetSPCommand(GETSALESRETURNBYSALESORDERDETAILID))
			{
				
				AddParameter( cmd, pInt32(SalesReturnBase.Property_SalesOrderDetailId, _SalesOrderDetailId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all SalesReturn objects by PageRequest
        /// </summary>
        /// <returns>A list of SalesReturn objects</returns>
		public SalesReturnList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDSALESRETURN))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				SalesReturnList _SalesReturnList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _SalesReturnList;
			}
		}
		
		/// <summary>
        /// Retrieves all SalesReturn objects by query String
        /// </summary>
        /// <returns>A list of SalesReturn objects</returns>
		public SalesReturnList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETSALESRETURNBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get SalesReturn Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of SalesReturn
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETSALESRETURNMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get SalesReturn Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of SalesReturn
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _SalesReturnRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETSALESRETURNROWCOUNT))
			{
				SqlDataReader reader;
				_SalesReturnRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _SalesReturnRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills SalesReturn object
        /// </summary>
        /// <param name="salesReturnObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(SalesReturnBase salesReturnObject, SqlDataReader reader, int start)
		{
			
				salesReturnObject.Id = reader.GetInt32( start + 0 );			
				salesReturnObject.SalesOrderDetailId = reader.GetInt32( start + 1 );			
				salesReturnObject.ReturnDate = reader.GetDateTime( start + 2 );			
				salesReturnObject.Quantity = reader.GetInt32( start + 3 );			
				salesReturnObject.Reason = reader.GetString( start + 4 );			
				salesReturnObject.RestockToInventory = reader.GetBoolean( start + 5 );			
				salesReturnObject.RefundAmount = reader.GetDecimal( start + 6 );			
				salesReturnObject.Status = reader.GetString( start + 7 );			
				if(!reader.IsDBNull(8)) salesReturnObject.Remarks = reader.GetString( start + 8 );			
				if(!reader.IsDBNull(9)) salesReturnObject.CreatedBy = reader.GetString( start + 9 );			
				salesReturnObject.CreatedAt = reader.GetDateTime( start + 10 );			
				if(!reader.IsDBNull(11)) salesReturnObject.UpdatedBy = reader.GetString( start + 11 );			
				if(!reader.IsDBNull(12)) salesReturnObject.UpdatedAt = reader.GetDateTime( start + 12 );			
			FillBaseObject(salesReturnObject, reader, (start + 13));

			
			salesReturnObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills SalesReturn object
        /// </summary>
        /// <param name="salesReturnObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(SalesReturnBase salesReturnObject, SqlDataReader reader)
		{
			FillObject(salesReturnObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves SalesReturn object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>SalesReturn object</returns>
		private SalesReturn GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					SalesReturn salesReturnObject= new SalesReturn();
					FillObject(salesReturnObject, reader);
					return salesReturnObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of SalesReturn objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of SalesReturn objects</returns>
		private SalesReturnList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//SalesReturn list
			SalesReturnList list = new SalesReturnList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					SalesReturn salesReturnObject = new SalesReturn();
					FillObject(salesReturnObject, reader);

					list.Add(salesReturnObject);
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
