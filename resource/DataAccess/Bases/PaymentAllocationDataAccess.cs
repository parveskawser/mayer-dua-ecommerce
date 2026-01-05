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
	public partial class PaymentAllocationDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTPAYMENTALLOCATION = "InsertPaymentAllocation";
		private const string UPDATEPAYMENTALLOCATION = "UpdatePaymentAllocation";
		private const string DELETEPAYMENTALLOCATION = "DeletePaymentAllocation";
		private const string GETPAYMENTALLOCATIONBYID = "GetPaymentAllocationById";
		private const string GETALLPAYMENTALLOCATION = "GetAllPaymentAllocation";
		private const string GETPAGEDPAYMENTALLOCATION = "GetPagedPaymentAllocation";
		private const string GETPAYMENTALLOCATIONBYCUSTOMERPAYMENTID = "GetPaymentAllocationByCustomerPaymentId";
		private const string GETPAYMENTALLOCATIONBYSALESORDERID = "GetPaymentAllocationBySalesOrderId";
		private const string GETPAYMENTALLOCATIONMAXIMUMID = "GetPaymentAllocationMaximumId";
		private const string GETPAYMENTALLOCATIONROWCOUNT = "GetPaymentAllocationRowCount";	
		private const string GETPAYMENTALLOCATIONBYQUERY = "GetPaymentAllocationByQuery";
		#endregion
		
		#region Constructors
		public PaymentAllocationDataAccess(IConfiguration configuration) : base(configuration) { }
		public PaymentAllocationDataAccess(ClientContext context) : base(context) { }
		public PaymentAllocationDataAccess(SqlTransaction transaction) : base(transaction) { }
		public PaymentAllocationDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="paymentAllocationObject"></param>
		private void AddCommonParams(SqlCommand cmd, PaymentAllocationBase paymentAllocationObject)
		{	
			AddParameter(cmd, pInt32(PaymentAllocationBase.Property_CustomerPaymentId, paymentAllocationObject.CustomerPaymentId));
			AddParameter(cmd, pInt32(PaymentAllocationBase.Property_SalesOrderId, paymentAllocationObject.SalesOrderId));
			AddParameter(cmd, pDecimal(PaymentAllocationBase.Property_AllocatedAmount, 9, paymentAllocationObject.AllocatedAmount));
			AddParameter(cmd, pDateTime(PaymentAllocationBase.Property_AllocatedDate, paymentAllocationObject.AllocatedDate));
			AddParameter(cmd, pNVarChar(PaymentAllocationBase.Property_Notes, 500, paymentAllocationObject.Notes));
			AddParameter(cmd, pNVarChar(PaymentAllocationBase.Property_CreatedBy, 100, paymentAllocationObject.CreatedBy));
			AddParameter(cmd, pDateTime(PaymentAllocationBase.Property_CreatedAt, paymentAllocationObject.CreatedAt));
			AddParameter(cmd, pNVarChar(PaymentAllocationBase.Property_UpdatedBy, 100, paymentAllocationObject.UpdatedBy));
			AddParameter(cmd, pDateTime(PaymentAllocationBase.Property_UpdatedAt, paymentAllocationObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts PaymentAllocation
        /// </summary>
        /// <param name="paymentAllocationObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(PaymentAllocationBase paymentAllocationObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTPAYMENTALLOCATION);
	
				AddParameter(cmd, pInt32Out(PaymentAllocationBase.Property_Id));
				AddCommonParams(cmd, paymentAllocationObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					paymentAllocationObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					paymentAllocationObject.Id = (Int32)GetOutParameter(cmd, PaymentAllocationBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(paymentAllocationObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates PaymentAllocation
        /// </summary>
        /// <param name="paymentAllocationObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(PaymentAllocationBase paymentAllocationObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEPAYMENTALLOCATION);
				
				AddParameter(cmd, pInt32(PaymentAllocationBase.Property_Id, paymentAllocationObject.Id));
				AddCommonParams(cmd, paymentAllocationObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					paymentAllocationObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(paymentAllocationObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes PaymentAllocation
        /// </summary>
        /// <param name="Id">Id of the PaymentAllocation object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEPAYMENTALLOCATION);	
				
				AddParameter(cmd, pInt32(PaymentAllocationBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(PaymentAllocation), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves PaymentAllocation object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the PaymentAllocation object to retrieve</param>
        /// <returns>PaymentAllocation object, null if not found</returns>
		public PaymentAllocation Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAYMENTALLOCATIONBYID))
			{
				AddParameter( cmd, pInt32(PaymentAllocationBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all PaymentAllocation objects 
        /// </summary>
        /// <returns>A list of PaymentAllocation objects</returns>
		public PaymentAllocationList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLPAYMENTALLOCATION))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all PaymentAllocation objects by CustomerPaymentId
        /// </summary>
        /// <returns>A list of PaymentAllocation objects</returns>
		public PaymentAllocationList GetByCustomerPaymentId(Int32 _CustomerPaymentId)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAYMENTALLOCATIONBYCUSTOMERPAYMENTID))
			{
				
				AddParameter( cmd, pInt32(PaymentAllocationBase.Property_CustomerPaymentId, _CustomerPaymentId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all PaymentAllocation objects by SalesOrderId
        /// </summary>
        /// <returns>A list of PaymentAllocation objects</returns>
		public PaymentAllocationList GetBySalesOrderId(Nullable<Int32> _SalesOrderId)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAYMENTALLOCATIONBYSALESORDERID))
			{
				
				AddParameter( cmd, pInt32(PaymentAllocationBase.Property_SalesOrderId, _SalesOrderId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all PaymentAllocation objects by PageRequest
        /// </summary>
        /// <returns>A list of PaymentAllocation objects</returns>
		public PaymentAllocationList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDPAYMENTALLOCATION))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				PaymentAllocationList _PaymentAllocationList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _PaymentAllocationList;
			}
		}
		
		/// <summary>
        /// Retrieves all PaymentAllocation objects by query String
        /// </summary>
        /// <returns>A list of PaymentAllocation objects</returns>
		public PaymentAllocationList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAYMENTALLOCATIONBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get PaymentAllocation Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of PaymentAllocation
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPAYMENTALLOCATIONMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get PaymentAllocation Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of PaymentAllocation
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _PaymentAllocationRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPAYMENTALLOCATIONROWCOUNT))
			{
				SqlDataReader reader;
				_PaymentAllocationRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _PaymentAllocationRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills PaymentAllocation object
        /// </summary>
        /// <param name="paymentAllocationObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(PaymentAllocationBase paymentAllocationObject, SqlDataReader reader, int start)
		{
			
				paymentAllocationObject.Id = reader.GetInt32( start + 0 );			
				paymentAllocationObject.CustomerPaymentId = reader.GetInt32( start + 1 );			
				if(!reader.IsDBNull(2)) paymentAllocationObject.SalesOrderId = reader.GetInt32( start + 2 );			
				paymentAllocationObject.AllocatedAmount = reader.GetDecimal( start + 3 );			
				paymentAllocationObject.AllocatedDate = reader.GetDateTime( start + 4 );			
				if(!reader.IsDBNull(5)) paymentAllocationObject.Notes = reader.GetString( start + 5 );			
				if(!reader.IsDBNull(6)) paymentAllocationObject.CreatedBy = reader.GetString( start + 6 );			
				if(!reader.IsDBNull(7)) paymentAllocationObject.CreatedAt = reader.GetDateTime( start + 7 );			
				if(!reader.IsDBNull(8)) paymentAllocationObject.UpdatedBy = reader.GetString( start + 8 );			
				if(!reader.IsDBNull(9)) paymentAllocationObject.UpdatedAt = reader.GetDateTime( start + 9 );			
			FillBaseObject(paymentAllocationObject, reader, (start + 10));

			
			paymentAllocationObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills PaymentAllocation object
        /// </summary>
        /// <param name="paymentAllocationObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(PaymentAllocationBase paymentAllocationObject, SqlDataReader reader)
		{
			FillObject(paymentAllocationObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves PaymentAllocation object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>PaymentAllocation object</returns>
		private PaymentAllocation GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					PaymentAllocation paymentAllocationObject= new PaymentAllocation();
					FillObject(paymentAllocationObject, reader);
					return paymentAllocationObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of PaymentAllocation objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of PaymentAllocation objects</returns>
		private PaymentAllocationList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//PaymentAllocation list
			PaymentAllocationList list = new PaymentAllocationList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					PaymentAllocation paymentAllocationObject = new PaymentAllocation();
					FillObject(paymentAllocationObject, reader);

					list.Add(paymentAllocationObject);
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