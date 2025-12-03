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
        public List<dynamic> GetVariantsSortedByStock()
        {
            var list = new List<dynamic>();

            // SQL: Joins Product and Variant, Sorts by StockQty ASC
            string SQL = @"
        SELECT 
            v.Id as VariantId,
            p.ProductName + ' - ' + ISNULL(v.VariantName, 'Standard') as FullName,
            ISNULL(vps.StockQty, 0) as CurrentStock
        FROM ProductVariant v
        JOIN Product p ON v.ProductId = p.Id
        LEFT JOIN VariantPriceStock vps ON v.Id = vps.Id
        WHERE p.IsActive = 1 AND v.IsActive = 1
        ORDER BY ISNULL(vps.StockQty, 0) ASC, p.ProductName ASC"; 

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dynamic item = new ExpandoObject();
                        item.VariantId = reader.GetInt32(0);
                        item.FullName = reader.GetString(1);
                        item.CurrentStock = reader.GetInt32(2);
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
                        //  lowercase property names for JS compatibility
                        dynamic info = new ExpandoObject();
                        // Map to lowercase keys explicitly
                        ((IDictionary<string, object>)info)["id"] = reader.GetInt32(0);
                        ((IDictionary<string, object>)info)["quantity"] = reader.GetInt32(1);
                        ((IDictionary<string, object>)info)["requestDate"] = reader.GetDateTime(2).ToString("dd MMM yyyy");
                        ((IDictionary<string, object>)info)["vendorName"] = reader.GetString(3);
                        ((IDictionary<string, object>)info)["remarks"] = reader.IsDBNull(4) ? "" : reader.GetString(4);

                        return info;
                    }
                }
                cmd.Connection.Close();
            }
            return null;
        }


        public dynamic GetVariantStatus(int variantId)
        {
            // Reusing your exact query logic, just filtered by ID
            string SQL = @"
        SELECT 
            v.Id as VariantId,
            p.ProductName,
            ISNULL(v.VariantName, 'Standard') as VariantName,
            ISNULL(vps.StockQty, 0) as CurrentStock,
            p.ReorderLevel,
            CASE WHEN ISNULL(vps.StockQty, 0) <= p.ReorderLevel THEN 1 ELSE 0 END as IsLowStock,
            CASE WHEN ISNULL(vps.StockQty, 0) > p.ReorderLevel THEN 1 ELSE 0 END as IsHealthyStock,
            (p.ReorderLevel * 2) - ISNULL(vps.StockQty, 0) as SuggestedQty,
            (SELECT COUNT(*) FROM PoRequested po WHERE po.ProductVariantId = v.Id AND po.Status = 'Pending') as PendingCount
        FROM ProductVariant v
        JOIN Product p ON v.ProductId = p.Id
        LEFT JOIN VariantPriceStock vps ON v.Id = vps.Id
        WHERE v.Id = @VariantId"; // Filter applied here

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("VariantId", variantId));
                if (cmd.Connection.State != System.Data.ConnectionState.Open) cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        dynamic item = new System.Dynamic.ExpandoObject();
                        // Explicitly mapping to match your View's expectations
                        ((IDictionary<string, object>)item)["VariantId"] = reader.GetInt32(0);
                        ((IDictionary<string, object>)item)["ProductName"] = reader.GetString(1);
                        ((IDictionary<string, object>)item)["VariantName"] = reader.GetString(2);
                        ((IDictionary<string, object>)item)["CurrentStock"] = reader.GetInt32(3);
                        ((IDictionary<string, object>)item)["ReorderLevel"] = reader.GetInt32(4);
                        ((IDictionary<string, object>)item)["IsHealthyStock"] = reader.GetInt32(6) == 1;
                        ((IDictionary<string, object>)item)["SuggestedQty"] = reader.GetInt32(7) > 0 ? reader.GetInt32(7) : 10;
                        ((IDictionary<string, object>)item)["HasPendingPO"] = reader.GetInt32(8) > 0;
                        return item;
                    }
                }
            }
            return null;
        }

    }

}