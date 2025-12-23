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
    public partial class PoRequestedDataAccess : BaseDataAccess, IPoRequestedDataAccess
    {
        #region Constants
        private const string INSERTPOREQUESTED = "InsertPoRequested";
        private const string UPDATEPOREQUESTED = "UpdatePoRequested";
        private const string DELETEPOREQUESTED = "DeletePoRequested";
        private const string GETPOREQUESTEDBYID = "GetPoRequestedById";
        private const string GETALLPOREQUESTED = "GetAllPoRequested";
        private const string GETPAGEDPOREQUESTED = "GetPagedPoRequested";
        private const string GETPOREQUESTEDBYVENDORID = "GetPoRequestedByVendorId";
        // Ideally rename SP too, but for C# code:
        private const string GETPOREQUESTEDBYPRODUCTVARIANTID = "GetPoRequestedByProductVariantId";
        private const string GETPOREQUESTEDMAXIMUMID = "GetPoRequestedMaximumId";
        private const string GETPOREQUESTEDROWCOUNT = "GetPoRequestedRowCount";
        private const string GETPOREQUESTEDBYQUERY = "GetPoRequestedByQuery";
        #endregion

        #region Constructors
        public PoRequestedDataAccess(IConfiguration configuration) : base(configuration) { }
        public PoRequestedDataAccess(ClientContext context) : base(context) { }
        public PoRequestedDataAccess(SqlTransaction transaction) : base(transaction) { }
        public PoRequestedDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion

        #region AddCommonParams Method
        private void AddCommonParams(SqlCommand cmd, PoRequestedBase poRequestedObject)
        {
            AddParameter(cmd, pInt32(PoRequestedBase.Property_VendorId, poRequestedObject.VendorId));
            // ✅ CHANGE: Use ProductVariantId
            AddParameter(cmd, pInt32(PoRequestedBase.Property_ProductVariantId, poRequestedObject.ProductVariantId));
            AddParameter(cmd, pInt32(PoRequestedBase.Property_Quantity, poRequestedObject.Quantity));
            AddParameter(cmd, pDateTime(PoRequestedBase.Property_RequestDate, poRequestedObject.RequestDate));
            AddParameter(cmd, pNVarChar(PoRequestedBase.Property_Status, 50, poRequestedObject.Status));
            AddParameter(cmd, pNVarChar(PoRequestedBase.Property_CreatedBy, 100, poRequestedObject.CreatedBy));
            AddParameter(cmd, pDateTime(PoRequestedBase.Property_CreatedAt, poRequestedObject.CreatedAt));
            AddParameter(cmd, pNVarChar(PoRequestedBase.Property_UpdatedBy, 100, poRequestedObject.UpdatedBy));
            AddParameter(cmd, pDateTime(PoRequestedBase.Property_UpdatedAt, poRequestedObject.UpdatedAt));
            AddParameter(cmd, pNVarChar(PoRequestedBase.Property_Remarks, 500, poRequestedObject.Remarks));
            AddParameter(cmd, pNVarChar(PoRequestedBase.Property_ReferenceNo, 100, poRequestedObject.ReferenceNo));
        }
        #endregion

        #region Insert Method
        public long Insert(PoRequestedBase poRequestedObject)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(INSERTPOREQUESTED);

                AddParameter(cmd, pInt32Out(PoRequestedBase.Property_Id));
                AddCommonParams(cmd, poRequestedObject);

                long result = InsertRecord(cmd);
                if (result > 0)
                {
                    poRequestedObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
                    poRequestedObject.Id = (Int32)GetOutParameter(cmd, PoRequestedBase.Property_Id);
                }
                return result;
            }
            catch (SqlException x)
            {
                throw new ObjectInsertException(poRequestedObject, x);
            }
        }
        #endregion

        #region Update Method
        public long Update(PoRequestedBase poRequestedObject)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(UPDATEPOREQUESTED);

                AddParameter(cmd, pInt32(PoRequestedBase.Property_Id, poRequestedObject.Id));
                AddCommonParams(cmd, poRequestedObject);

                long result = UpdateRecord(cmd);
                if (result > 0)
                    poRequestedObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
                return result;
            }
            catch (SqlException x)
            {
                throw new ObjectUpdateException(poRequestedObject, x);
            }
        }
        #endregion

        #region Delete Method
        public long Delete(Int32 _Id)
        {
            try
            {
                SqlCommand cmd = GetSPCommand(DELETEPOREQUESTED);

                AddParameter(cmd, pInt32(PoRequestedBase.Property_Id, _Id));

                return DeleteRecord(cmd);
            }
            catch (SqlException x)
            {
                throw new ObjectDeleteException(typeof(PoRequested), _Id, x);
            }

        }
        #endregion

        #region Get By Id Method
        public PoRequested Get(Int32 _Id)
        {
            using (SqlCommand cmd = GetSPCommand(GETPOREQUESTEDBYID))
            {
                AddParameter(cmd, pInt32(PoRequestedBase.Property_Id, _Id));

                return GetObject(cmd);
            }
        }
        #endregion

        #region GetAll Method
        public PoRequestedList GetAll()
        {
            using (SqlCommand cmd = GetSPCommand(GETALLPOREQUESTED))
            {
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }

        public PoRequestedList GetByVendorId(Int32 _VendorId)
        {
            using (SqlCommand cmd = GetSPCommand(GETPOREQUESTEDBYVENDORID))
            {

                AddParameter(cmd, pInt32(PoRequestedBase.Property_VendorId, _VendorId));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }

        /// <summary>
                /// Retrieves all PoRequested objects by ProductVariantId
                /// </summary>
        // ✅ CHANGE: Renamed method and parameter
        public PoRequestedList GetByProductVariantId(Int32 _ProductVariantId)
        {
            // Note: You likely need to update the SP name in SQL too if it was 'GetPoRequestedByProductId'
            using (SqlCommand cmd = GetSPCommand(GETPOREQUESTEDBYPRODUCTVARIANTID))
            {
                AddParameter(cmd, pInt32(PoRequestedBase.Property_ProductVariantId, _ProductVariantId));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }


        public PoRequestedList GetPaged(PagedRequest request)
        {
            using (SqlCommand cmd = GetSPCommand(GETPAGEDPOREQUESTED))
            {
                AddParameter(cmd, pInt32Out("TotalRows"));
                AddParameter(cmd, pInt32("PageIndex", request.PageIndex));
                AddParameter(cmd, pInt32("RowPerPage", request.RowPerPage));
                AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause));
                AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn));
                AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder));

                PoRequestedList _PoRequestedList = GetList(cmd, ALL_AVAILABLE_RECORDS);
                request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
                return _PoRequestedList;
            }
        }

        public PoRequestedList GetByQuery(String query)
        {
            using (SqlCommand cmd = GetSPCommand(GETPOREQUESTEDBYQUERY))
            {
                AddParameter(cmd, pNVarChar("Query", 4000, query));
                return GetList(cmd, ALL_AVAILABLE_RECORDS); ;
            }
        }

        #endregion


        #region Get PoRequested Maximum Id Method
        public Int32 GetMaxId()
        {
            Int32 _MaximumId = 0;
            using (SqlCommand cmd = GetSPCommand(GETPOREQUESTEDMAXIMUMID))
            {
                SqlDataReader reader;
                _MaximumId = (Int32)SelectRecords(cmd, out reader);
                reader.Close();
                reader.Dispose();
            }
            return _MaximumId;
        }

        #endregion

        #region Get PoRequested Row Count Method
        public Int32 GetRowCount()
        {
            Int32 _PoRequestedRowCount = 0;
            using (SqlCommand cmd = GetSPCommand(GETPOREQUESTEDROWCOUNT))
            {
                SqlDataReader reader;
                _PoRequestedRowCount = (Int32)SelectRecords(cmd, out reader);
                reader.Close();
                reader.Dispose();
            }
            return _PoRequestedRowCount;
        }

        #endregion

        #region Fill Methods
        protected void FillObject(PoRequestedBase poRequestedObject, SqlDataReader reader, int start)
        {

            poRequestedObject.Id = reader.GetInt32(start + 0);
            poRequestedObject.VendorId = reader.GetInt32(start + 1);
            // ✅ CHANGE: Map to ProductVariantId
            poRequestedObject.ProductVariantId = reader.GetInt32(start + 2);
            poRequestedObject.Quantity = reader.GetInt32(start + 3);
            poRequestedObject.RequestDate = reader.GetDateTime(start + 4);
            poRequestedObject.Status = reader.GetString(start + 5);
            if (!reader.IsDBNull(6)) poRequestedObject.CreatedBy = reader.GetString(start + 6);
            poRequestedObject.CreatedAt = reader.GetDateTime(start + 7);
            if (!reader.IsDBNull(8)) poRequestedObject.UpdatedBy = reader.GetString(start + 8);
            if (!reader.IsDBNull(9)) poRequestedObject.UpdatedAt = reader.GetDateTime(start + 9);
            if (!reader.IsDBNull(10)) poRequestedObject.Remarks = reader.GetString(start + 10);
            if (!reader.IsDBNull(11)) poRequestedObject.ReferenceNo = reader.GetString(start + 11);
            FillBaseObject(poRequestedObject, reader, (start + 12));


            poRequestedObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
        }

        protected void FillObject(PoRequestedBase poRequestedObject, SqlDataReader reader)
        {
            FillObject(poRequestedObject, reader, 0);
        }

        private PoRequested GetObject(SqlCommand cmd)
        {
            SqlDataReader reader;
            long rows = SelectRecords(cmd, out reader);

            using (reader)
            {
                if (reader.Read())
                {
                    PoRequested poRequestedObject = new PoRequested();
                    FillObject(poRequestedObject, reader);
                    return poRequestedObject;
                }
                else
                {
                    return null;
                }
            }
        }

        private PoRequestedList GetList(SqlCommand cmd, long rows)
        {
            // Select multiple records
            SqlDataReader reader;
            long result = SelectRecords(cmd, out reader);

            //PoRequested list
            PoRequestedList list = new PoRequestedList();

            using (reader)
            {
                // Read rows until end of result or number of rows specified is reached
                while (reader.Read() && rows-- != 0)
                {
                    PoRequested poRequestedObject = new PoRequested();
                    FillObject(poRequestedObject, reader);

                    list.Add(poRequestedObject);
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