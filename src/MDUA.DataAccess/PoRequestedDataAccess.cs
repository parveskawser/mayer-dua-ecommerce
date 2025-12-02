using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Dynamic;
using MDUA.Framework;
using MDUA.Entities;

namespace MDUA.DataAccess
{
    public partial class PoRequestedDataAccess
    {
        public void UpdateStatus(int poId, string status, SqlTransaction transaction)
        {
            string SQL = "UPDATE PoRequested SET Status = @Status, UpdatedAt = GETDATE() WHERE Id = @Id";

            // Note: When using a transaction, we must use the connection associated with it
            using (SqlCommand cmd = new SqlCommand(SQL, transaction.Connection, transaction))
            {
                cmd.Parameters.AddWithValue("@Id", poId);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.ExecuteNonQuery();
            }
        }

        public List<dynamic> GetInventoryStatus()
        {
            var list = new List<dynamic>();

            string SQL = @"
                SELECT 
                    v.Id as VariantId,
                    p.ProductName,
                    ISNULL(v.VariantName, 'Standard') as VariantName,
                    ISNULL(vps.StockQty, 0) as CurrentStock,
                    p.ReorderLevel,
                    -- Calculate if stock is low (1 = True, 0 = False)
                    CASE WHEN ISNULL(vps.StockQty, 0) <= p.ReorderLevel THEN 1 ELSE 0 END as IsLowStock,
                    -- Calculate if stock is healthy (Above reorder level)
                    CASE WHEN ISNULL(vps.StockQty, 0) > p.ReorderLevel THEN 1 ELSE 0 END as IsHealthyStock,
                    -- Suggest reorder qty
                    (p.ReorderLevel * 2) - ISNULL(vps.StockQty, 0) as SuggestedQty,
                    -- Check for Pending POs
                    (SELECT COUNT(*) FROM PoRequested po WHERE po.ProductVariantId = v.Id AND po.Status = 'Pending') as PendingCount
                FROM ProductVariant v
                JOIN Product p ON v.ProductId = p.Id
                LEFT JOIN VariantPriceStock vps ON v.Id = vps.Id
                WHERE p.IsActive = 1 AND v.IsActive = 1
                ORDER BY (CASE WHEN ISNULL(vps.StockQty, 0) <= p.ReorderLevel THEN 0 ELSE 1 END), p.ProductName";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dynamic item = new ExpandoObject();

                        item.VariantId = reader.GetInt32(0);
                        item.ProductName = reader.GetString(1);
                        item.VariantName = reader.GetString(2);
                        item.CurrentStock = reader.GetInt32(3);
                        item.ReorderLevel = reader.GetInt32(4);
                        item.IsLowStock = reader.GetInt32(5) == 1;
                        item.IsHealthyStock = reader.GetInt32(6) == 1;
                        item.SuggestedQty = reader.GetInt32(7) > 0 ? reader.GetInt32(7) : 10;
                        item.HasPendingPO = reader.GetInt32(8) > 0;

                        list.Add(item);
                    }
                }
                cmd.Connection.Close();
            }
            return list;
        }

        // 2. Get Pending Request Details (For Info Modal & Autofill)
        public dynamic GetPendingRequestByVariant(int variantId)
        {
            string SQL = @"
                SELECT TOP 1 
                    po.Id, 
                    po.Quantity, 
                    po.RequestDate, 
                    v.VendorName, 
                    po.Remarks
                FROM PoRequested po
                JOIN Vendor v ON po.VendorId = v.Id
                WHERE po.ProductVariantId = @VariantId 
                  AND po.Status = 'Pending'
                ORDER BY po.RequestDate DESC";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("VariantId", variantId));
                if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // ✅ Change: Use ExpandoObject
                        dynamic info = new ExpandoObject();
                        info.Id = reader.GetInt32(0);
                        info.Quantity = reader.GetInt32(1);
                        info.RequestDate = reader.GetDateTime(2).ToString("dd MMM yyyy");
                        info.VendorName = reader.GetString(3);
                        info.Remarks = reader.IsDBNull(4) ? "" : reader.GetString(4);

                        return info;
                    }
                }
                cmd.Connection.Close();
            }
            return null;
        }
    }
}