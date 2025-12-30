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
	public partial class VendorPaymentDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTVENDORPAYMENT = "InsertVendorPayment";
		private const string UPDATEVENDORPAYMENT = "UpdateVendorPayment";
		private const string DELETEVENDORPAYMENT = "DeleteVendorPayment";
		private const string GETVENDORPAYMENTBYID = "GetVendorPaymentById";
		private const string GETALLVENDORPAYMENT = "GetAllVendorPayment";
		private const string GETPAGEDVENDORPAYMENT = "GetPagedVendorPayment";
		private const string GETVENDORPAYMENTBYVENDORID = "GetVendorPaymentByVendorId";
		private const string GETVENDORPAYMENTBYPAYMENTMETHODID = "GetVendorPaymentByPaymentMethodId";
		private const string GETVENDORPAYMENTBYINVENTORYTRANSACTIONID = "GetVendorPaymentByInventoryTransactionId";
		private const string GETVENDORPAYMENTMAXIMUMID = "GetVendorPaymentMaximumId";
		private const string GETVENDORPAYMENTROWCOUNT = "GetVendorPaymentRowCount";	
		private const string GETVENDORPAYMENTBYQUERY = "GetVendorPaymentByQuery";
		#endregion
		
		#region Constructors
		public VendorPaymentDataAccess(IConfiguration configuration) : base(configuration) { }
		public VendorPaymentDataAccess(ClientContext context) : base(context) { }
		public VendorPaymentDataAccess(SqlTransaction transaction) : base(transaction) { }
		public VendorPaymentDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="vendorPaymentObject"></param>
		private void AddCommonParams(SqlCommand cmd, VendorPaymentBase vendorPaymentObject)
		{	
			AddParameter(cmd, pInt32(VendorPaymentBase.Property_VendorId, vendorPaymentObject.VendorId));
			AddParameter(cmd, pInt32(VendorPaymentBase.Property_PaymentMethodId, vendorPaymentObject.PaymentMethodId));
			AddParameter(cmd, pInt32(VendorPaymentBase.Property_InventoryTransactionId, vendorPaymentObject.InventoryTransactionId));
			AddParameter(cmd, pNVarChar(VendorPaymentBase.Property_ReferenceNumber, 100, vendorPaymentObject.ReferenceNumber));
			AddParameter(cmd, pNVarChar(VendorPaymentBase.Property_PaymentType, 20, vendorPaymentObject.PaymentType));
			AddParameter(cmd, pDecimal(VendorPaymentBase.Property_Amount, 9, vendorPaymentObject.Amount));
			AddParameter(cmd, pDateTime(VendorPaymentBase.Property_PaymentDate, vendorPaymentObject.PaymentDate));
			AddParameter(cmd, pNVarChar(VendorPaymentBase.Property_Status, 20, vendorPaymentObject.Status));
			AddParameter(cmd, pNVarChar(VendorPaymentBase.Property_Notes, 500, vendorPaymentObject.Notes));
			AddParameter(cmd, pNVarChar(VendorPaymentBase.Property_CreatedBy, 100, vendorPaymentObject.CreatedBy));
			AddParameter(cmd, pDateTime(VendorPaymentBase.Property_CreatedAt, vendorPaymentObject.CreatedAt));
			AddParameter(cmd, pNVarChar(VendorPaymentBase.Property_UpdatedBy, 100, vendorPaymentObject.UpdatedBy));
			AddParameter(cmd, pDateTime(VendorPaymentBase.Property_UpdatedAt, vendorPaymentObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts VendorPayment
        /// </summary>
        /// <param name="vendorPaymentObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(VendorPaymentBase vendorPaymentObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTVENDORPAYMENT);
	
				AddParameter(cmd, pInt32Out(VendorPaymentBase.Property_Id));
				AddCommonParams(cmd, vendorPaymentObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					vendorPaymentObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					vendorPaymentObject.Id = (Int32)GetOutParameter(cmd, VendorPaymentBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(vendorPaymentObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates VendorPayment
        /// </summary>
        /// <param name="vendorPaymentObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(VendorPaymentBase vendorPaymentObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEVENDORPAYMENT);
				
				AddParameter(cmd, pInt32(VendorPaymentBase.Property_Id, vendorPaymentObject.Id));
				AddCommonParams(cmd, vendorPaymentObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					vendorPaymentObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(vendorPaymentObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes VendorPayment
        /// </summary>
        /// <param name="Id">Id of the VendorPayment object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEVENDORPAYMENT);	
				
				AddParameter(cmd, pInt32(VendorPaymentBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(VendorPayment), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves VendorPayment object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the VendorPayment object to retrieve</param>
        /// <returns>VendorPayment object, null if not found</returns>
		public VendorPayment Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETVENDORPAYMENTBYID))
			{
				AddParameter( cmd, pInt32(VendorPaymentBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all VendorPayment objects 
        /// </summary>
        /// <returns>A list of VendorPayment objects</returns>
		public VendorPaymentList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLVENDORPAYMENT))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all VendorPayment objects by VendorId
        /// </summary>
        /// <returns>A list of VendorPayment objects</returns>
		public VendorPaymentList GetByVendorId(Int32 _VendorId)
		{
			using( SqlCommand cmd = GetSPCommand(GETVENDORPAYMENTBYVENDORID))
			{
				
				AddParameter( cmd, pInt32(VendorPaymentBase.Property_VendorId, _VendorId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all VendorPayment objects by PaymentMethodId
        /// </summary>
        /// <returns>A list of VendorPayment objects</returns>
		public VendorPaymentList GetByPaymentMethodId(Int32 _PaymentMethodId)
		{
			using( SqlCommand cmd = GetSPCommand(GETVENDORPAYMENTBYPAYMENTMETHODID))
			{
				
				AddParameter( cmd, pInt32(VendorPaymentBase.Property_PaymentMethodId, _PaymentMethodId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all VendorPayment objects by InventoryTransactionId
        /// </summary>
        /// <returns>A list of VendorPayment objects</returns>
		public VendorPaymentList GetByInventoryTransactionId(Nullable<Int32> _InventoryTransactionId)
		{
			using( SqlCommand cmd = GetSPCommand(GETVENDORPAYMENTBYINVENTORYTRANSACTIONID))
			{
				
				AddParameter( cmd, pInt32(VendorPaymentBase.Property_InventoryTransactionId, _InventoryTransactionId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all VendorPayment objects by PageRequest
        /// </summary>
        /// <returns>A list of VendorPayment objects</returns>
		public VendorPaymentList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDVENDORPAYMENT))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				VendorPaymentList _VendorPaymentList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _VendorPaymentList;
			}
		}
		
		/// <summary>
        /// Retrieves all VendorPayment objects by query String
        /// </summary>
        /// <returns>A list of VendorPayment objects</returns>
		public VendorPaymentList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETVENDORPAYMENTBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get VendorPayment Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of VendorPayment
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETVENDORPAYMENTMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get VendorPayment Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of VendorPayment
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _VendorPaymentRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETVENDORPAYMENTROWCOUNT))
			{
				SqlDataReader reader;
				_VendorPaymentRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _VendorPaymentRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills VendorPayment object
        /// </summary>
        /// <param name="vendorPaymentObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(VendorPaymentBase vendorPaymentObject, SqlDataReader reader, int start)
		{
			
				vendorPaymentObject.Id = reader.GetInt32( start + 0 );			
				vendorPaymentObject.VendorId = reader.GetInt32( start + 1 );			
				vendorPaymentObject.PaymentMethodId = reader.GetInt32( start + 2 );			
				if(!reader.IsDBNull(3)) vendorPaymentObject.InventoryTransactionId = reader.GetInt32( start + 3 );			
				if(!reader.IsDBNull(4)) vendorPaymentObject.ReferenceNumber = reader.GetString( start + 4 );			
				vendorPaymentObject.PaymentType = reader.GetString( start + 5 );			
				vendorPaymentObject.Amount = reader.GetDecimal( start + 6 );			
				vendorPaymentObject.PaymentDate = reader.GetDateTime( start + 7 );			
				vendorPaymentObject.Status = reader.GetString( start + 8 );			
				if(!reader.IsDBNull(9)) vendorPaymentObject.Notes = reader.GetString( start + 9 );			
				if(!reader.IsDBNull(10)) vendorPaymentObject.CreatedBy = reader.GetString( start + 10 );			
				if(!reader.IsDBNull(11)) vendorPaymentObject.CreatedAt = reader.GetDateTime( start + 11 );			
				if(!reader.IsDBNull(12)) vendorPaymentObject.UpdatedBy = reader.GetString( start + 12 );			
				if(!reader.IsDBNull(13)) vendorPaymentObject.UpdatedAt = reader.GetDateTime( start + 13 );			
			FillBaseObject(vendorPaymentObject, reader, (start + 14));

			
			vendorPaymentObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills VendorPayment object
        /// </summary>
        /// <param name="vendorPaymentObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(VendorPaymentBase vendorPaymentObject, SqlDataReader reader)
		{
			FillObject(vendorPaymentObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves VendorPayment object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>VendorPayment object</returns>
		private VendorPayment GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					VendorPayment vendorPaymentObject= new VendorPayment();
					FillObject(vendorPaymentObject, reader);
					return vendorPaymentObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of VendorPayment objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of VendorPayment objects</returns>
		private VendorPaymentList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//VendorPayment list
			VendorPaymentList list = new VendorPaymentList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					VendorPayment vendorPaymentObject = new VendorPayment();
					FillObject(vendorPaymentObject, reader);

					list.Add(vendorPaymentObject);
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