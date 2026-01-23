using System;
using System.Data;
using System.Data.SqlClient;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Framework;             // <--- FIX: Added this
using MDUA.Framework.Exceptions;

namespace MDUA.DataAccess
{
    public partial class BulkPurchaseOrderDataAccess
    {
        /// <summary>
        /// Custom Insert implementation to support SqlTransaction.
        /// </summary>
        public long Insert(BulkPurchaseOrder obj, SqlTransaction trans = null)
        {
            try
            {
                // Note: Make sure "InsertBulkPurchaseOrder" matches the const in the generated file
                // If the const is private, hardcoding string here is fine.
                SqlCommand cmd = GetSPCommand("InsertBulkPurchaseOrder");

                // Apply the transaction if provided
                if (trans != null)
                {
                    cmd.Connection = trans.Connection;
                    cmd.Transaction = trans;
                }

                // Output Parameter
                AddParameter(cmd, pInt32Out(BulkPurchaseOrderBase.Property_Id));

                // Common Parameters
                AddParameter(cmd, pInt32(BulkPurchaseOrderBase.Property_VendorId, obj.VendorId));
                AddParameter(cmd, pNVarChar(BulkPurchaseOrderBase.Property_AgreementNumber, 50, obj.AgreementNumber));
                AddParameter(cmd, pNVarChar(BulkPurchaseOrderBase.Property_Title, 200, obj.Title));
                AddParameter(cmd, pDateTime(BulkPurchaseOrderBase.Property_AgreementDate, obj.AgreementDate));
                AddParameter(cmd, pDateTime(BulkPurchaseOrderBase.Property_ExpiryDate, obj.ExpiryDate));
                AddParameter(cmd, pInt32(BulkPurchaseOrderBase.Property_TotalTargetQuantity, obj.TotalTargetQuantity));
                AddParameter(cmd, pDecimal(BulkPurchaseOrderBase.Property_TotalTargetAmount, 9, obj.TotalTargetAmount));
                AddParameter(cmd, pNVarChar(BulkPurchaseOrderBase.Property_Status, 20, obj.Status));
                AddParameter(cmd, pNVarChar(BulkPurchaseOrderBase.Property_Remarks, 500, obj.Remarks));
                AddParameter(cmd, pNVarChar(BulkPurchaseOrderBase.Property_CreatedBy, 100, obj.CreatedBy));
                AddParameter(cmd, pDateTime(BulkPurchaseOrderBase.Property_CreatedAt, obj.CreatedAt));
                AddParameter(cmd, pNVarChar(BulkPurchaseOrderBase.Property_UpdatedBy, 100, obj.UpdatedBy));
                AddParameter(cmd, pDateTime(BulkPurchaseOrderBase.Property_UpdatedAt, obj.UpdatedAt));
                AddParameter(cmd, pInt32(BulkPurchaseOrderBase.Property_ConsumedQuantity, obj.ConsumedQuantity));
                AddParameter(cmd, pDecimal(BulkPurchaseOrderBase.Property_ConsumedAmount, 9, obj.ConsumedAmount));

                long result = InsertRecord(cmd);

                if (result > 0)
                {
                    obj.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
                    obj.Id = (Int32)GetOutParameter(cmd, BulkPurchaseOrderBase.Property_Id);
                }
                return result;
            }
            catch (SqlException x)
            {
                throw new ObjectInsertException(obj, x);
            }
        }

        public List<BulkPurchaseOrder> GetByCompanyId(int companyId)
        {
            // FIX: Join through PoRequested -> ProductVariant -> Product
            // This ensures we only fetch Bulk Orders that contain products belonging to THIS Company.
            string SQL = @"
    SELECT DISTINCT bpo.* FROM BulkPurchaseOrder bpo
    INNER JOIN PoRequested pr ON bpo.Id = pr.BulkPurchaseOrderId
    INNER JOIN ProductVariant pv ON pr.ProductVariantId = pv.Id
    INNER JOIN Product p ON pv.ProductId = p.Id
    WHERE p.CompanyId = @CompanyId
    ORDER BY bpo.CreatedAt DESC";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));

                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }

        public List<BulkOrderItemViewModel> GetBulkOrderItems(int bulkOrderId)
        {
            var items = new List<BulkOrderItemViewModel>();

            // ✅ FIX: Joined 'BulkPurchaseOrder' (BPO) to get the correct VendorId
            string SQL = @"
        SELECT 
            PR.Id AS PoRequestId, 
            PR.ProductVariantId,
            P.ProductName,
            PV.VariantName,
            PR.Quantity,
            PR.Status,
            PR.RequestDate,
            BPO.VendorId,                  -- <--- CHANGED: Fetch from Parent Table (BPO)
            ISNULL(VPS.CostPrice, 0) AS SuggestedPrice
        FROM PoRequested PR
        JOIN BulkPurchaseOrder BPO ON PR.BulkPurchaseOrderId = BPO.Id -- <--- ✅ ADDED JOIN
        JOIN ProductVariant PV ON PR.ProductVariantId = PV.Id
        JOIN Product P ON PV.ProductId = P.Id
        LEFT JOIN VariantPriceStock VPS ON PV.Id = VPS.Id
        WHERE PR.BulkPurchaseOrderId = @BulkOrderId
        ORDER BY PR.Id DESC";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("BulkOrderId", bulkOrderId));

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new BulkOrderItemViewModel
                        {
                            PoRequestId = Convert.ToInt32(reader["PoRequestId"]),
                            ProductVariantId = Convert.ToInt32(reader["ProductVariantId"]),
                            ProductName = reader["ProductName"].ToString(),
                            VariantName = reader["VariantName"].ToString(),
                            Quantity = reader["Quantity"] != DBNull.Value ? Convert.ToInt32(reader["Quantity"]) : 0,
                            Status = reader["Status"].ToString(),
                            RequestDate = reader["RequestDate"] != DBNull.Value ? Convert.ToDateTime(reader["RequestDate"]) : DateTime.MinValue,

                            SuggestedPrice = reader["SuggestedPrice"] != DBNull.Value ? Convert.ToDecimal(reader["SuggestedPrice"]) : 0,

                            // Now this will contain the real VendorId (e.g., 55) instead of 0
                            VendorId = reader["VendorId"] != DBNull.Value ? Convert.ToInt32(reader["VendorId"]) : 0
                        });
                    }
                }
            }
            return items;
        }
    }

}
