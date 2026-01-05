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
	public partial class PaymentMethodDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTPAYMENTMETHOD = "InsertPaymentMethod";
		private const string UPDATEPAYMENTMETHOD = "UpdatePaymentMethod";
		private const string DELETEPAYMENTMETHOD = "DeletePaymentMethod";
		private const string GETPAYMENTMETHODBYID = "GetPaymentMethodById";
		private const string GETALLPAYMENTMETHOD = "GetAllPaymentMethod";
		private const string GETPAGEDPAYMENTMETHOD = "GetPagedPaymentMethod";
		private const string GETPAYMENTMETHODMAXIMUMID = "GetPaymentMethodMaximumId";
		private const string GETPAYMENTMETHODROWCOUNT = "GetPaymentMethodRowCount";	
		private const string GETPAYMENTMETHODBYQUERY = "GetPaymentMethodByQuery";
		#endregion
		
		#region Constructors
		public PaymentMethodDataAccess(IConfiguration configuration) : base(configuration) { }
		public PaymentMethodDataAccess(ClientContext context) : base(context) { }
		public PaymentMethodDataAccess(SqlTransaction transaction) : base(transaction) { }
		public PaymentMethodDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="paymentMethodObject"></param>
		private void AddCommonParams(SqlCommand cmd, PaymentMethodBase paymentMethodObject)
		{	
			AddParameter(cmd, pNVarChar(PaymentMethodBase.Property_Name, 50, paymentMethodObject.Name));
			AddParameter(cmd, pBool(PaymentMethodBase.Property_IsActive, paymentMethodObject.IsActive));
			AddParameter(cmd, pNVarChar(PaymentMethodBase.Property_CreatedBy, 100, paymentMethodObject.CreatedBy));
			AddParameter(cmd, pDateTime(PaymentMethodBase.Property_CreatedAt, paymentMethodObject.CreatedAt));
			AddParameter(cmd, pNVarChar(PaymentMethodBase.Property_UpdatedBy, 100, paymentMethodObject.UpdatedBy));
			AddParameter(cmd, pDateTime(PaymentMethodBase.Property_UpdatedAt, paymentMethodObject.UpdatedAt));
			AddParameter(cmd, pNVarChar(PaymentMethodBase.Property_SystemCode, 20, paymentMethodObject.SystemCode));
			AddParameter(cmd, pNVarChar(PaymentMethodBase.Property_LogoUrl, 255, paymentMethodObject.LogoUrl));
			AddParameter(cmd, pInt32(PaymentMethodBase.Property_DisplayOrder, paymentMethodObject.DisplayOrder));
			AddParameter(cmd, pBool(PaymentMethodBase.Property_SupportsManual, paymentMethodObject.SupportsManual));
			AddParameter(cmd, pBool(PaymentMethodBase.Property_SupportsGateway, paymentMethodObject.SupportsGateway));
			AddParameter(cmd, pNVarChar(PaymentMethodBase.Property_ManualInstruction, paymentMethodObject.ManualInstruction));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts PaymentMethod
        /// </summary>
        /// <param name="paymentMethodObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(PaymentMethodBase paymentMethodObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTPAYMENTMETHOD);
	
				AddParameter(cmd, pInt32Out(PaymentMethodBase.Property_Id));
				AddCommonParams(cmd, paymentMethodObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					paymentMethodObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					paymentMethodObject.Id = (Int32)GetOutParameter(cmd, PaymentMethodBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(paymentMethodObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates PaymentMethod
        /// </summary>
        /// <param name="paymentMethodObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(PaymentMethodBase paymentMethodObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEPAYMENTMETHOD);
				
				AddParameter(cmd, pInt32(PaymentMethodBase.Property_Id, paymentMethodObject.Id));
				AddCommonParams(cmd, paymentMethodObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					paymentMethodObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(paymentMethodObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes PaymentMethod
        /// </summary>
        /// <param name="Id">Id of the PaymentMethod object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEPAYMENTMETHOD);	
				
				AddParameter(cmd, pInt32(PaymentMethodBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(PaymentMethod), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves PaymentMethod object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the PaymentMethod object to retrieve</param>
        /// <returns>PaymentMethod object, null if not found</returns>
		public PaymentMethod Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAYMENTMETHODBYID))
			{
				AddParameter( cmd, pInt32(PaymentMethodBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all PaymentMethod objects 
        /// </summary>
        /// <returns>A list of PaymentMethod objects</returns>
		public PaymentMethodList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLPAYMENTMETHOD))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all PaymentMethod objects by PageRequest
        /// </summary>
        /// <returns>A list of PaymentMethod objects</returns>
		public PaymentMethodList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDPAYMENTMETHOD))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				PaymentMethodList _PaymentMethodList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _PaymentMethodList;
			}
		}
		
		/// <summary>
        /// Retrieves all PaymentMethod objects by query String
        /// </summary>
        /// <returns>A list of PaymentMethod objects</returns>
		public PaymentMethodList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAYMENTMETHODBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get PaymentMethod Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of PaymentMethod
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPAYMENTMETHODMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get PaymentMethod Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of PaymentMethod
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _PaymentMethodRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPAYMENTMETHODROWCOUNT))
			{
				SqlDataReader reader;
				_PaymentMethodRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _PaymentMethodRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills PaymentMethod object
        /// </summary>
        /// <param name="paymentMethodObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(PaymentMethodBase paymentMethodObject, SqlDataReader reader, int start)
		{
			
				paymentMethodObject.Id = reader.GetInt32( start + 0 );			
				paymentMethodObject.Name = reader.GetString( start + 1 );			
				paymentMethodObject.IsActive = reader.GetBoolean( start + 2 );			
				if(!reader.IsDBNull(3)) paymentMethodObject.CreatedBy = reader.GetString( start + 3 );			
				if(!reader.IsDBNull(4)) paymentMethodObject.CreatedAt = reader.GetDateTime( start + 4 );			
				if(!reader.IsDBNull(5)) paymentMethodObject.UpdatedBy = reader.GetString( start + 5 );			
				if(!reader.IsDBNull(6)) paymentMethodObject.UpdatedAt = reader.GetDateTime( start + 6 );			
				if(!reader.IsDBNull(7)) paymentMethodObject.SystemCode = reader.GetString( start + 7 );			
				if(!reader.IsDBNull(8)) paymentMethodObject.LogoUrl = reader.GetString( start + 8 );			
				if(!reader.IsDBNull(9)) paymentMethodObject.DisplayOrder = reader.GetInt32( start + 9 );			
				paymentMethodObject.SupportsManual = reader.GetBoolean( start + 10 );			
				paymentMethodObject.SupportsGateway = reader.GetBoolean( start + 11 );			
				if(!reader.IsDBNull(12)) paymentMethodObject.ManualInstruction = reader.GetString( start + 12 );			
			FillBaseObject(paymentMethodObject, reader, (start + 13));

			
			paymentMethodObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills PaymentMethod object
        /// </summary>
        /// <param name="paymentMethodObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(PaymentMethodBase paymentMethodObject, SqlDataReader reader)
		{
			FillObject(paymentMethodObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves PaymentMethod object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>PaymentMethod object</returns>
		private PaymentMethod GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					PaymentMethod paymentMethodObject= new PaymentMethod();
					FillObject(paymentMethodObject, reader);
					return paymentMethodObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of PaymentMethod objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of PaymentMethod objects</returns>
		private PaymentMethodList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//PaymentMethod list
			PaymentMethodList list = new PaymentMethodList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					PaymentMethod paymentMethodObject = new PaymentMethod();
					FillObject(paymentMethodObject, reader);

					list.Add(paymentMethodObject);
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