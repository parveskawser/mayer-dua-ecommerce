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
    public partial class CustomerPaymentDataAccess : BaseDataAccess, ICustomerPaymentDataAccess
    {
        #region Constants
        private const string INSERTCUSTOMERPAYMENT = "InsertCustomerPayment";
        private const string UPDATECUSTOMERPAYMENT = "UpdateCustomerPayment";
        private const string DELETECUSTOMERPAYMENT = "DeleteCustomerPayment";
        private const string GETCUSTOMERPAYMENTBYID = "GetCustomerPaymentById";
        private const string GETALLCUSTOMERPAYMENT = "GetAllCustomerPayment";
        private const string GETPAGEDCUSTOMERPAYMENT = "GetPagedCustomerPayment";
        private const string GETCUSTOMERPAYMENTBYCUSTOMERID = "GetCustomerPaymentByCustomerId";
        private const string GETCUSTOMERPAYMENTBYPAYMENTMETHODID = "GetCustomerPaymentByPaymentMethodId";
        private const string GETCUSTOMERPAYMENTBYINVENTORYTRANSACTIONID = "GetCustomerPaymentByInventoryTransactionId";
        private const string GETCUSTOMERPAYMENTMAXIMUMID = "GetCustomerPaymentMaximumId";
        private const string GETCUSTOMERPAYMENTROWCOUNT = "GetCustomerPaymentRowCount";
        private const string GETCUSTOMERPAYMENTBYQUERY = "GetCustomerPaymentByQuery";
        #endregion

        #region Constructors
        public CustomerPaymentDataAccess(IConfiguration configuration) : base(configuration) { }
        public CustomerPaymentDataAccess(ClientContext context) : base(context) { }
        public CustomerPaymentDataAccess(SqlTransaction transaction) : base(transaction) { }
        public CustomerPaymentDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion

        #region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="customerPaymentObject"></param>
        private void AddCommonParams(SqlCommand cmd, CustomerPaymentBase customerPaymentObject)
        {
            AddParameter(cmd, pInt32(CustomerPaymentBase.Property_CustomerId, customerPaymentObject.CustomerId));
            AddParameter(cmd, pInt32(CustomerPaymentBase.Property_PaymentMethodId, customerPaymentObject.PaymentMethodId));
            AddParameter(cmd, pInt32(CustomerPaymentBase.Property_InventoryTransactionId, customerPaymentObject.InventoryTransactionId));
            AddParameter(cmd, pNVarChar(CustomerPaymentBase.Property_PaymentType, 20, customerPaymentObject.PaymentType));
            AddParameter(cmd, pDecimal(CustomerPaymentBase.Property_Amount, 9, customerPaymentObject.Amount));
            AddParameter(cmd, pDateTime(CustomerPaymentBase.Property_PaymentDate, customerPaymentObject.PaymentDate));
            AddParameter(cmd, pNVarChar(CustomerPaymentBase.Property_Status, 20, customerPaymentObject.Status));
            AddParameter(cmd, pNVarChar(CustomerPaymentBase.Property_Notes, 500, customerPaymentObject.Notes));
            AddParameter(cmd, pNVarChar(CustomerPaymentBase.Property_CreatedBy, 100, customerPaymentObject.CreatedBy));
            AddParameter(cmd, pDateTime(CustomerPaymentBase.Property_CreatedAt, customerPaymentObject.CreatedAt));
            AddParameter(cmd, pNVarChar(CustomerPaymentBase.Property_UpdatedBy, 100, customerPaymentObject.UpdatedBy));
            AddParameter(cmd, pDateTime(CustomerPaymentBase.Property_UpdatedAt, customerPaymentObject.UpdatedAt));

            // --- NEW PARAMETER ---
            AddParameter(cmd, pNVarChar(CustomerPaymentBase.Property_TransactionReference, 100, customerPaymentObject.TransactionReference));
        }
        #endregion

        #region Insert Method
        /// <summary>
        /// Inserts CustomerPayment
        /// </summary>
        /// <param name="customerPaymentObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
        public long Insert(CustomerPaymentBase customerPaymentObject)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(INSERTCUSTOMERPAYMENT);

                AddParameter(cmd, pInt32Out(CustomerPaymentBase.Property_Id));
                AddCommonParams(cmd, customerPaymentObject);

                long result = InsertRecord(cmd);
                if (result > 0)
                {
                    customerPaymentObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
                    customerPaymentObject.Id = (Int32)GetOutParameter(cmd, CustomerPaymentBase.Property_Id);
                }
                return result;
            }
            catch (SqlException x)
            {
                throw new ObjectInsertException(customerPaymentObject, x);
            }
        }
        #endregion

        #region Update Method
        /// <summary>
        /// Updates CustomerPayment
        /// </summary>
        /// <param name="customerPaymentObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
        public long Update(CustomerPaymentBase customerPaymentObject)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(UPDATECUSTOMERPAYMENT);

                AddParameter(cmd, pInt32(CustomerPaymentBase.Property_Id, customerPaymentObject.Id));
                AddCommonParams(cmd, customerPaymentObject);

                long result = UpdateRecord(cmd);
                if (result > 0)
                    customerPaymentObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
                return result;
            }
            catch (SqlException x)
            {
                throw new ObjectUpdateException(customerPaymentObject, x);
            }
        }
        #endregion

        #region Delete Method
        /// <summary>
        /// Deletes CustomerPayment
        /// </summary>
        /// <param name="Id">Id of the CustomerPayment object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
        public long Delete(Int32 _Id)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(DELETECUSTOMERPAYMENT);

                AddParameter(cmd, pInt32(CustomerPaymentBase.Property_Id, _Id));

                return DeleteRecord(cmd);
            }
            catch (SqlException x)
            {
                throw new ObjectDeleteException(typeof(CustomerPayment), _Id, x);
            }

        }
        #endregion

        #region Get By Id Method
        /// <summary>
        /// Retrieves CustomerPayment object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the CustomerPayment object to retrieve</param>
        /// <returns>CustomerPayment object, null if not found</returns>
        public CustomerPayment Get(Int32 _Id)
        {
            using (SqlCommand cmd = GetSPCommand(GETCUSTOMERPAYMENTBYID))
            {
                AddParameter(cmd, pInt32(CustomerPaymentBase.Property_Id, _Id));

                return GetObject(cmd);
            }
        }
        #endregion

        #region GetAll Method
        /// <summary>
        /// Retrieves all CustomerPayment objects 
        /// </summary>
        /// <returns>A list of CustomerPayment objects</returns>
        public CustomerPaymentList GetAll()
        {
            using (SqlCommand cmd = GetSPCommand(GETALLCUSTOMERPAYMENT))
            {
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }

        /// <summary>
        /// Retrieves all CustomerPayment objects by CustomerId
        /// </summary>
        /// <returns>A list of CustomerPayment objects</returns>
        public CustomerPaymentList GetByCustomerId(Int32 _CustomerId)
        {
            using (SqlCommand cmd = GetSPCommand(GETCUSTOMERPAYMENTBYCUSTOMERID))
            {

                AddParameter(cmd, pInt32(CustomerPaymentBase.Property_CustomerId, _CustomerId));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }

        /// <summary>
        /// Retrieves all CustomerPayment objects by PaymentMethodId
        /// </summary>
        /// <returns>A list of CustomerPayment objects</returns>
        public CustomerPaymentList GetByPaymentMethodId(Int32 _PaymentMethodId)
        {
            using (SqlCommand cmd = GetSPCommand(GETCUSTOMERPAYMENTBYPAYMENTMETHODID))
            {

                AddParameter(cmd, pInt32(CustomerPaymentBase.Property_PaymentMethodId, _PaymentMethodId));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }

        /// <summary>
        /// Retrieves all CustomerPayment objects by InventoryTransactionId
        /// </summary>
        /// <returns>A list of CustomerPayment objects</returns>
        public CustomerPaymentList GetByInventoryTransactionId(Nullable<Int32> _InventoryTransactionId)
        {
            using (SqlCommand cmd = GetSPCommand(GETCUSTOMERPAYMENTBYINVENTORYTRANSACTIONID))
            {

                AddParameter(cmd, pInt32(CustomerPaymentBase.Property_InventoryTransactionId, _InventoryTransactionId));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }


        /// <summary>
        /// Retrieves all CustomerPayment objects by PageRequest
        /// </summary>
        /// <returns>A list of CustomerPayment objects</returns>
        public CustomerPaymentList GetPaged(PagedRequest request)
        {
            using (SqlCommand cmd = GetSPCommand(GETPAGEDCUSTOMERPAYMENT))
            {
                AddParameter(cmd, pInt32Out("TotalRows"));
                AddParameter(cmd, pInt32("PageIndex", request.PageIndex));
                AddParameter(cmd, pInt32("RowPerPage", request.RowPerPage));
                AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause));
                AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn));
                AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder));

                CustomerPaymentList _CustomerPaymentList = GetList(cmd, ALL_AVAILABLE_RECORDS);
                request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
                return _CustomerPaymentList;
            }
        }

        /// <summary>
        /// Retrieves all CustomerPayment objects by query String
        /// </summary>
        /// <returns>A list of CustomerPayment objects</returns>
        public CustomerPaymentList GetByQuery(String query)
        {
            using (SqlCommand cmd = GetSPCommand(GETCUSTOMERPAYMENTBYQUERY))
            {
                AddParameter(cmd, pNVarChar("Query", 4000, query));
                return GetList(cmd, ALL_AVAILABLE_RECORDS); ;
            }
        }

        #endregion


        #region Get CustomerPayment Maximum Id Method
        /// <summary>
        /// Retrieves Get Maximum Id of CustomerPayment
        /// </summary>
        /// <returns>Int32 type object</returns>
        public Int32 GetMaxId()
        {
            Int32 _MaximumId = 0;
            using (SqlCommand cmd = GetSPCommand(GETCUSTOMERPAYMENTMAXIMUMID))
            {
                SqlDataReader reader;
                _MaximumId = (Int32)SelectRecords(cmd, out reader);
                reader.Close();
                reader.Dispose();
            }
            return _MaximumId;
        }

        #endregion

        #region Get CustomerPayment Row Count Method
        /// <summary>
        /// Retrieves Get Total Rows of CustomerPayment
        /// </summary>
        /// <returns>Int32 type object</returns>
        public Int32 GetRowCount()
        {
            Int32 _CustomerPaymentRowCount = 0;
            using (SqlCommand cmd = GetSPCommand(GETCUSTOMERPAYMENTROWCOUNT))
            {
                SqlDataReader reader;
                _CustomerPaymentRowCount = (Int32)SelectRecords(cmd, out reader);
                reader.Close();
                reader.Dispose();
            }
            return _CustomerPaymentRowCount;
        }

        #endregion

        #region Fill Methods
        /// <summary>
        /// Fills CustomerPayment object
        /// </summary>
        /// <param name="customerPaymentObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
        protected void FillObject(CustomerPaymentBase customerPaymentObject, SqlDataReader reader, int start)
        {

            customerPaymentObject.Id = reader.GetInt32(start + 0);
            customerPaymentObject.CustomerId = reader.GetInt32(start + 1);
            customerPaymentObject.PaymentMethodId = reader.GetInt32(start + 2);
            if (!reader.IsDBNull(start + 3)) customerPaymentObject.InventoryTransactionId = reader.GetInt32(start + 3);
            customerPaymentObject.PaymentType = reader.GetString(start + 4);
            customerPaymentObject.Amount = reader.GetDecimal(start + 5);
            customerPaymentObject.PaymentDate = reader.GetDateTime(start + 6);
            customerPaymentObject.Status = reader.GetString(start + 7);
            if (!reader.IsDBNull(start + 8)) customerPaymentObject.Notes = reader.GetString(start + 8);
            customerPaymentObject.CreatedBy = reader.GetString(start + 9);
            customerPaymentObject.CreatedAt = reader.GetDateTime(start + 10);
            if (!reader.IsDBNull(start + 11)) customerPaymentObject.UpdatedBy = reader.GetString(start + 11);
            if (!reader.IsDBNull(start + 12)) customerPaymentObject.UpdatedAt = reader.GetDateTime(start + 12);

            // --- NEW COLUMN ---
            if (!reader.IsDBNull(start + 13)) customerPaymentObject.TransactionReference = reader.GetString(start + 13);

            FillBaseObject(customerPaymentObject, reader, (start + 14));


            customerPaymentObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
        }

        /// <summary>
        /// Fills CustomerPayment object
        /// </summary>
        /// <param name="customerPaymentObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        protected void FillObject(CustomerPaymentBase customerPaymentObject, SqlDataReader reader)
        {
            FillObject(customerPaymentObject, reader, 0);
        }

        /// <summary>
        /// Retrieves CustomerPayment object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>CustomerPayment object</returns>
        private CustomerPayment GetObject(SqlCommand cmd)
        {
            SqlDataReader reader;
            long rows = SelectRecords(cmd, out reader);

            using (reader)
            {
                if (reader.Read())
                {
                    CustomerPayment customerPaymentObject = new CustomerPayment();
                    FillObject(customerPaymentObject, reader);
                    return customerPaymentObject;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Retrieves list of CustomerPayment objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of CustomerPayment objects</returns>
        private CustomerPaymentList GetList(SqlCommand cmd, long rows)
        {
            // Select multiple records
            SqlDataReader reader;
            long result = SelectRecords(cmd, out reader);

            //CustomerPayment list
            CustomerPaymentList list = new CustomerPaymentList();

            using (reader)
            {
                // Read rows until end of result or number of rows specified is reached
                while (reader.Read() && rows-- != 0)
                {
                    CustomerPayment customerPaymentObject = new CustomerPayment();
                    FillObject(customerPaymentObject, reader);

                    list.Add(customerPaymentObject);
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