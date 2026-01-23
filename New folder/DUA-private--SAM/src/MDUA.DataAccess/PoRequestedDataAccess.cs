using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Dynamic;
using MDUA.Framework;
using MDUA.Entities;
using MDUA.Entities.Bases;        // Required for PoRequestedBase
using MDUA.Framework.Exceptions;  // Required for ObjectInsertException
using MDUA.DataAccess.Interface; // Ensure this is present
namespace MDUA.DataAccess
{
    public partial class PoRequestedDataAccess
    {
        // ============================================================================
        // ✅ THE FIX: Custom Insert Method supporting SqlTransaction
        // ============================================================================
        public long Insert(PoRequestedBase obj, SqlTransaction trans)
        {
            try
            {
                // 1. Get the Stored Procedure Command
                SqlCommand cmd = GetSPCommand("InsertPoRequested");

                // 2. Attach the Transaction
                if (trans != null)
                {
                    cmd.Connection = trans.Connection;
                    cmd.Transaction = trans;
                }

                // 3. Add Parameters
                // We add the output ID parameter
                AddParameter(cmd, pInt32Out(PoRequestedBase.Property_Id));

                // We add the rest of the parameters. 
                // NOTE: Ensure these property names match your generated Base class constants exactly.
                AddParameter(cmd, pInt32(PoRequestedBase.Property_ProductVariantId, obj.ProductVariantId));
                AddParameter(cmd, pInt32(PoRequestedBase.Property_VendorId, obj.VendorId));
                AddParameter(cmd, pInt32(PoRequestedBase.Property_Quantity, obj.Quantity));
                AddParameter(cmd, pDateTime(PoRequestedBase.Property_RequestDate, obj.RequestDate));
                AddParameter(cmd, pNVarChar(PoRequestedBase.Property_Status, 50, obj.Status));
                AddParameter(cmd, pNVarChar(PoRequestedBase.Property_Remarks, 500, obj.Remarks));
                AddParameter(cmd, pNVarChar(PoRequestedBase.Property_ReferenceNo, 50, obj.ReferenceNo));

                // Add BulkOrderId (This is required for your Bulk Order feature)
                AddParameter(cmd, pInt32(PoRequestedBase.Property_BulkPurchaseOrderId, obj.BulkPurchaseOrderId));

                // Audit fields
                AddParameter(cmd, pNVarChar(PoRequestedBase.Property_CreatedBy, 100, obj.CreatedBy));
                AddParameter(cmd, pDateTime(PoRequestedBase.Property_CreatedAt, obj.CreatedAt));
                AddParameter(cmd, pNVarChar(PoRequestedBase.Property_UpdatedBy, 100, obj.UpdatedBy));
                AddParameter(cmd, pDateTime(PoRequestedBase.Property_UpdatedAt, obj.UpdatedAt));

                // 4. Execute
                long result = InsertRecord(cmd);

                if (result > 0)
                {
                    obj.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
                    obj.Id = (Int32)GetOutParameter(cmd, PoRequestedBase.Property_Id);
                }
                return result;
            }
            catch (SqlException x)
            {
                throw new ObjectInsertException(obj, x);
            }
        }

        // ============================================================================
        // YOUR EXISTING CUSTOM METHODS
        // ============================================================================

        public void UpdateStatus(int poId, string status, SqlTransaction transaction)
        {
            string SQL = "UPDATE PoRequested SET Status = @Status, UpdatedAt = GETUTCDATE() WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(SQL, transaction.Connection, transaction))
            {
                cmd.Parameters.AddWithValue("@Id", poId);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.ExecuteNonQuery();
            }
        }

        public List<dynamic> GetInventoryStatus(int companyId)
        {
            var list = new List<dynamic>();

            // We filter p.CompanyId = @CompanyId
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
            WHERE p.IsActive = 1 
              AND v.IsActive = 1
              AND p.CompanyId = @CompanyId  -- ✅ TENANT FILTER
            ORDER BY (CASE WHEN ISNULL(vps.StockQty, 0) <= p.ReorderLevel THEN 0 ELSE 1 END), p.ProductName";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId)); // Pass the ID

                if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dynamic item = new ExpandoObject();
                        // ... (Keep your existing mapping logic) ...
                        ((IDictionary<string, object>)item)["VariantId"] = reader.GetInt32(0);
                        ((IDictionary<string, object>)item)["ProductName"] = reader.GetString(1);
                        ((IDictionary<string, object>)item)["VariantName"] = reader.GetString(2);
                        ((IDictionary<string, object>)item)["CurrentStock"] = reader.GetInt32(3);
                        ((IDictionary<string, object>)item)["ReorderLevel"] = reader.GetInt32(4);
                        ((IDictionary<string, object>)item)["IsLowStock"] = reader.GetInt32(5) == 1;
                        ((IDictionary<string, object>)item)["IsHealthyStock"] = reader.GetInt32(6) == 1;
                        ((IDictionary<string, object>)item)["SuggestedQty"] = reader.GetInt32(7) > 0 ? reader.GetInt32(7) : 10;
                        ((IDictionary<string, object>)item)["HasPendingPO"] = reader.GetInt32(8) > 0;

                        list.Add(item);
                    }
                }
            }
            return list;
        }
        public dynamic GetPendingRequestByVariant(int variantId)
        {
            string SQL = @"
        SELECT TOP 1 
            po.Id, 
            po.Quantity, 
            po.RequestDate, 
            v.VendorName, 
            po.Remarks,
            po.VendorId,           -- 1. ADDED THIS
            po.BulkPurchaseOrderId -- 2. ADDED THIS (Required for your Bulk logic later)
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
                        dynamic info = new ExpandoObject();
                        // Cast to Dictionary to set keys explicitly
                        var dict = (IDictionary<string, object>)info;

                        dict["Id"] = reader.GetInt32(0);
                        dict["Quantity"] = reader.GetInt32(1);
                        dict["RequestDate"] = reader.GetDateTime(2).ToString("dd MMM yyyy");
                        dict["VendorName"] = reader.GetString(3);
                        dict["Remarks"] = reader.IsDBNull(4) ? "" : reader.GetString(4);

                        // 3. MAP THE MISSING KEYS
                        dict["VendorId"] = reader.GetInt32(5);
                        dict["BulkPurchaseOrderId"] = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6);

                        return info;
                    }
                }
                cmd.Connection.Close();
            }
            return null;
        }
        public dynamic GetVariantStatus(int variantId)
        {
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
                WHERE v.Id = @VariantId";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("VariantId", variantId));
                if (cmd.Connection.State != System.Data.ConnectionState.Open) cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        dynamic item = new System.Dynamic.ExpandoObject();
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

        public List<dynamic> GetVendorHistory(int vendorId, int companyId) // Add Parameter
        {
            var list = new List<dynamic>();
            // Update SQL to filter by Company
            string SQL = @"
        SELECT 
            pr.Id,
            pr.RequestDate,
            pr.Quantity AS RequestedQty,
            pr.Status,
            CASE WHEN pr.BulkPurchaseOrderId IS NOT NULL AND pr.BulkPurchaseOrderId > 0 THEN 1 ELSE 0 END AS IsBulkOrder,
            p.ProductName,
            ISNULL(pv.VariantName, 'Standard') AS VariantName,
            ISNULL((SELECT SUM(rcv.ReceivedQuantity) FROM PoReceived rcv WHERE rcv.PoRequestedId = pr.Id), 0) AS ReceivedQty
        FROM PoRequested pr
        JOIN ProductVariant pv ON pr.ProductVariantId = pv.Id
        JOIN Product p ON pv.ProductId = p.Id
        WHERE pr.VendorId = @VendorId AND p.CompanyId = @CompanyId  -- ADDED FILTER
        ORDER BY pr.RequestDate DESC";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("VendorId", vendorId));
                AddParameter(cmd, pInt32("CompanyId", companyId));

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dynamic item = new ExpandoObject();
                        ((IDictionary<string, object>)item)["PoId"] = reader.GetInt32(0);
                        ((IDictionary<string, object>)item)["RequestDate"] = reader.GetDateTime(1).ToString("dd MMM yyyy");
                        ((IDictionary<string, object>)item)["RequestedQty"] = reader.GetInt32(2);
                        ((IDictionary<string, object>)item)["Status"] = reader.GetString(3);
                        ((IDictionary<string, object>)item)["IsBulkOrder"] = reader.GetInt32(4) == 1;
                        ((IDictionary<string, object>)item)["ProductName"] = reader.GetString(5) + " (" + reader.GetString(6) + ")";
                        ((IDictionary<string, object>)item)["ReceivedQty"] = reader.GetInt32(7);

                        list.Add(item);
                    }
                }
                cmd.Connection.Close();
            }
            return list;
        }
        
        
        
        public (List<dynamic>, int) GetVendorHistoryPaged(int vendorId, int companyId, int pageIndex, int pageSize, string search, string status, string type, DateTime? fromDate, DateTime? toDate)
        {
            var list = new List<dynamic>();
            int totalRows = 0;
            int offset = (pageIndex - 1) * pageSize;

            using (SqlCommand cmd = GetSQLCommand("")) // Initialize command first to add params easily
            {
                // --- 1. Filter Logic Construction ---
                // Filters for PoRequested (Standard)
                var sbStandard = new System.Text.StringBuilder("WHERE pr.VendorId = @VendorId AND p.CompanyId = @CompanyId AND pr.BulkPurchaseOrderId IS NULL");

                // Filters for BulkPurchaseOrder (Parent)
                // Note: We ensure the Bulk Order is relevant to this company by checking if it has children in this company
                var sbBulk = new System.Text.StringBuilder(@"
            WHERE bpo.VendorId = @VendorId 
            AND EXISTS (
                SELECT 1 FROM PoRequested child 
                JOIN ProductVariant pv ON child.ProductVariantId = pv.Id 
                JOIN Product p ON pv.ProductId = p.Id 
                WHERE child.BulkPurchaseOrderId = bpo.Id AND p.CompanyId = @CompanyId
            )");

                // Apply Search/Date/Status filters dynamically to both sets
                if (!string.IsNullOrEmpty(search))
                {
                    sbStandard.Append(" AND ((p.ProductName) LIKE @Search OR pr.Remarks LIKE @Search)");
                    sbBulk.Append(" AND (bpo.AgreementNumber LIKE @Search OR bpo.Title LIKE @Search)");
                }

                if (fromDate.HasValue)
                {
                    sbStandard.Append(" AND pr.RequestDate >= @FromDate");
                    sbBulk.Append(" AND bpo.AgreementDate >= @FromDate");
                }
                if (toDate.HasValue)
                {
                    sbStandard.Append(" AND pr.RequestDate <= @ToDate");
                    sbBulk.Append(" AND bpo.AgreementDate <= @ToDate");
                }

                // --- 2. The Mighty SQL Query ---
                string SQL = $@"
        -- A. Calculate Total Count (Union of Standard + Bulk Parents)
        SELECT COUNT(*) FROM (
            SELECT pr.Id FROM PoRequested pr 
            JOIN ProductVariant pv ON pr.ProductVariantId = pv.Id
            JOIN Product p ON pv.ProductId = p.Id 
            {sbStandard}
            UNION ALL
            SELECT bpo.Id FROM BulkPurchaseOrder bpo
            {sbBulk}
        ) AS TotalCount;

        -- B. Fetch Data (Union -> Sort -> Paginate)
        SELECT * FROM (
            
            -- 1. Standard Orders (No Children)
            SELECT 
                'Standard' AS RowType,
                pr.Id AS Id,
                pr.RequestDate AS Date,
                '-' AS AgreementNumber,
                p.ProductName + ' (' + ISNULL(pv.VariantName, 'Standard') + ')' AS Title,
                pr.Status,
                pr.Quantity AS ReqQty,
                ISNULL((SELECT SUM(rcv.ReceivedQuantity) FROM PoReceived rcv WHERE rcv.PoRequestedId = pr.Id), 0) AS RecQty,
                ISNULL((SELECT SUM(rcv.TotalPaymentDue) FROM PoReceived rcv WHERE rcv.PoRequestedId = pr.Id), 0) AS TotalAmount,
                ISNULL((SELECT SUM(rcv.TotalPaid) FROM PoReceived rcv WHERE rcv.PoRequestedId = pr.Id), 0) AS PaidAmount,
                NULL AS ChildrenJson -- No children for standard orders
            FROM PoRequested pr
            JOIN ProductVariant pv ON pr.ProductVariantId = pv.Id
            JOIN Product p ON pv.ProductId = p.Id
            {sbStandard}

            UNION ALL

            -- 2. Bulk Orders (Parents)
            SELECT 
                'Bulk' AS RowType,
                bpo.Id AS Id,
                bpo.AgreementDate AS Date,
                bpo.AgreementNumber,
                ISNULL(bpo.Title, 'Bulk Agreement') AS Title,
                bpo.Status,
                bpo.TotalTargetQuantity AS ReqQty,
                bpo.ConsumedQuantity AS RecQty,
                bpo.ConsumedAmount AS TotalAmount, -- Use Table Column directly
                -- Calculate Total Paid for all children in this bulk order
                ISNULL((
                    SELECT SUM(rcv.TotalPaid) 
                    FROM PoReceived rcv 
                    JOIN PoRequested child ON rcv.PoRequestedId = child.Id 
                    WHERE child.BulkPurchaseOrderId = bpo.Id
                ), 0) AS PaidAmount,
                
                -- FETCH CHILDREN AS JSON
                (
                    SELECT 
                        child.Id AS PoId, 
                        child.RequestDate, 
                        p.ProductName,
                        child.Status,
                        child.Quantity AS ReqQty,
                        ISNULL((SELECT SUM(r.ReceivedQuantity) FROM PoReceived r WHERE r.PoRequestedId = child.Id), 0) AS RecQty,
                        ISNULL((SELECT SUM(r.TotalPaymentDue) FROM PoReceived r WHERE r.PoRequestedId = child.Id), 0) AS TotalAmt,
                        ISNULL((SELECT SUM(r.TotalPaid) FROM PoReceived r WHERE r.PoRequestedId = child.Id), 0) AS PaidAmt
                    FROM PoRequested child
                    JOIN ProductVariant pv ON child.ProductVariantId = pv.Id
                    JOIN Product p ON pv.ProductId = p.Id 
                    WHERE child.BulkPurchaseOrderId = bpo.Id
                    ORDER BY child.RequestDate DESC
                    FOR JSON PATH
                ) AS ChildrenJson

            FROM BulkPurchaseOrder bpo
            {sbBulk}

        ) AS UnifiedHistory
        ORDER BY Date DESC
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                cmd.CommandText = SQL;

                // --- 3. Add Parameters ---
                AddParameter(cmd, pInt32("VendorId", vendorId));
                AddParameter(cmd, pInt32("CompanyId", companyId));
                AddParameter(cmd, pInt32("Offset", offset));
                AddParameter(cmd, pInt32("PageSize", pageSize));
                if (!string.IsNullOrEmpty(search)) AddParameter(cmd, pNVarChar("Search", 100, $"%{search}%"));
                if (fromDate.HasValue) AddParameter(cmd, pDateTime("FromDate", fromDate.Value));
                if (toDate.HasValue) AddParameter(cmd, pDateTime("ToDate", toDate.Value));

                if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) totalRows = reader.GetInt32(0);

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            dynamic item = new ExpandoObject();
                            var dict = (IDictionary<string, object>)item;

                            // --- 1. NEW KEYS (For the Hierarchy UI) ---
                            string rowType = reader["RowType"].ToString();
                            dict["RowType"] = rowType;
                            dict["Id"] = reader["Id"];
                            dict["Date"] = Convert.ToDateTime(reader["Date"]).ToString("dd MMM yyyy");
                            dict["AgreementNumber"] = reader["AgreementNumber"].ToString();
                            dict["Title"] = reader["Title"].ToString();
                            dict["Status"] = reader["Status"].ToString();

                            // Financials
                            dict["ReqQty"] = reader["ReqQty"];
                            dict["RecQty"] = reader["RecQty"];
                            dict["TotalAmount"] = reader["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(reader["TotalAmount"]) : 0;
                            dict["PaidAmount"] = reader["PaidAmount"] != DBNull.Value ? Convert.ToDecimal(reader["PaidAmount"]) : 0;
                            dict["DueAmount"] = (decimal)dict["TotalAmount"] - (decimal)dict["PaidAmount"];

                            // Children JSON
                            dict["Children"] = reader["ChildrenJson"] != DBNull.Value ? reader["ChildrenJson"].ToString() : null;

                            // --- 2. LEGACY KEYS (CRITICAL FOR EXPORT) ---
                            // We map the new columns back to the old names your Export expects
                            dict["PoId"] = reader["Id"]; // ✅ Fixes KeyNotFoundException
                            dict["RequestDate"] = dict["Date"];
                            dict["ProductName"] = dict["Title"]; // Maps Title (Bulk or Product) to ProductName
                            dict["RequestedQty"] = dict["ReqQty"];
                            dict["ReceivedQty"] = dict["RecQty"];
                            dict["AgreementNumber"] = dict["AgreementNumber"];

                            // ✅ Fix 2: Create a friendly "Type" string instead of boolean
                            dict["Type"] = rowType; // Returns "Bulk" or "Standard"

                            // Keep IsBulkOrder just in case other logic needs it, but Export will use "Type"
                            dict["IsBulkOrder"] = (rowType == "Bulk");
                            list.Add(item);
                        }
                    }
                }
            }
            return (list, totalRows);
        }        // We ADDED 'int companyId' here


        public void RejectBulkRemaining(int bulkOrderId)
        {
            using (SqlCommand cmd = GetSPCommand("RejectBulkOrderRemaining"))
            {
                AddParameter(cmd, pInt32("BulkOrderId", bulkOrderId));

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                cmd.ExecuteNonQuery();
            }
        }
        public (List<dynamic>, int) GetVendorHistoryPaged(int vendorId, int companyId, int pageIndex, int pageSize)
        {
            // Pass companyId to the main method
            return GetVendorHistoryPaged(vendorId, companyId, pageIndex, pageSize, "", "all", "all", null, null);
        }

        // 2. Fix for: GetVendorHistoryPaged (With Filters)
        // We ADDED 'int companyId' here
        public (List<dynamic>, int) GetVendorHistoryPaged(int vendorId, int companyId, int pageIndex, int pageSize, string search, string status, string type)
        {
            // Pass companyId to the main method
            return GetVendorHistoryPaged(vendorId, companyId, pageIndex, pageSize, search, status, type, null, null);
        }
    }
}