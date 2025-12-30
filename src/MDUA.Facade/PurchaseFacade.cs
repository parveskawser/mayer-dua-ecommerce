using MDUA.DataAccess.Interface; // Use Interfaces
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using static MDUA.Entities.BulkPurchaseOrder;

namespace MDUA.Facade
{
    public class PurchaseFacade : IPurchaseFacade
    {
        // 1. Declare ALL required dependencies
        private readonly IPoRequestedDataAccess _poDataAccess;
        private readonly IVendorDataAccess _vendorDataAccess;
        private readonly IPoReceivedDataAccess _poReceiveDA;
        private readonly IVariantPriceStockDataAccess _stockDA;
        private readonly IInventoryTransactionDataAccess _invTransDA;
        private readonly IConfiguration _config;
        private readonly IBulkPurchaseOrderDataAccess _bulkOrderDA; // ✅ NEW

        // 2. Inject them in the Constructor
        public PurchaseFacade(
            IPoRequestedDataAccess poDataAccess,
            IVendorDataAccess vendorDataAccess,
            IPoReceivedDataAccess poReceiveDA,
            IVariantPriceStockDataAccess stockDA,
            IInventoryTransactionDataAccess invTransDA,
            IBulkPurchaseOrderDataAccess bulkOrderDA, // ✅ INJECTED
            IConfiguration config)
        
        
        {
            _poDataAccess = poDataAccess;
            _vendorDataAccess = vendorDataAccess;
            _poReceiveDA = poReceiveDA;
            _stockDA = stockDA;
            _invTransDA = invTransDA;
            _config = config;
            _bulkOrderDA = bulkOrderDA;
        }

        #region ICommonFacade Implementation
        public long Delete(int id) => _poDataAccess.Delete(id);
        public PoRequested Get(int id) => _poDataAccess.Get(id);
        public PoRequestedList GetAll() => _poDataAccess.GetAll();
        public PoRequestedList GetByQuery(string query) => _poDataAccess.GetByQuery(query);
        public long Insert(PoRequestedBase obj) => _poDataAccess.Insert(obj);
        public long Update(PoRequestedBase obj) => _poDataAccess.Update(obj);
        #endregion

        #region Extended Methods

        private void CheckAndCloseBulkOrder(int bulkOrderId, SqlConnection conn, SqlTransaction trans)
        {
            // 1. Debug Log: Check if method is hit (Check Output Window in VS)
            System.Diagnostics.Debug.WriteLine($"Checking closure for Bulk Order ID: {bulkOrderId}");

            // 2. Count how many items are still strictly 'Pending'
            // We do this inside the transaction so it sees the update we just made.
            string countSql = "SELECT COUNT(*) FROM PoRequested WHERE BulkPurchaseOrderId = @BulkId AND Status = 'Pending'";

            using (var cmdCount = new SqlCommand(countSql, conn, trans))
            {
                cmdCount.Parameters.AddWithValue("@BulkId", bulkOrderId);
                int pendingCount = (int)cmdCount.ExecuteScalar();

                System.Diagnostics.Debug.WriteLine($"Pending Items Remaining: {pendingCount}");

                // 3. If ZERO items are pending, close the order
                if (pendingCount == 0)
                {
                    string updateSql = @"
                UPDATE BulkPurchaseOrder 
                SET Status = 'Inactive', UpdatedAt = GETUTCDATE()
                WHERE Id = @BulkId";

                    using (var cmdUpdate = new SqlCommand(updateSql, conn, trans))
                    {
                        cmdUpdate.Parameters.AddWithValue("@BulkId", bulkOrderId);
                        cmdUpdate.ExecuteNonQuery();
                    }
                    System.Diagnostics.Debug.WriteLine("Bulk Order Updated to INACTIVE");
                }
            }
        }
        public List<dynamic> GetInventoryStatus()
        {
            return _poDataAccess.GetInventoryStatus();
        }

        public dynamic GetPendingRequestInfo(int variantId)
        {
            return _poDataAccess.GetPendingRequestByVariant(variantId);
        }

        public long CreatePurchaseOrder(PoRequested po)
        {
            po.RequestDate = DateTime.UtcNow;
            po.Status = "Pending";
            po.CreatedAt = DateTime.UtcNow;
            po.UpdatedAt = DateTime.UtcNow;

            // ✅ Use empty string as fallback if CreatedBy is still null
            if (string.IsNullOrEmpty(po.CreatedBy))
                po.CreatedBy = "System"; // Better than empty string

            if (po.Remarks == null)
                po.Remarks = "";

            if (po.ReferenceNo == null)
                po.ReferenceNo = "";

            return _poDataAccess.Insert(po);
        }
        public List<Vendor> GetAllVendors()
        {
            return _vendorDataAccess.GetAll().ToList();
        }

        // Implementation with Transaction
        public void ReceiveStock(int variantId, int qty, decimal price, string invoice, string remarks)
        {
            // 1. Get Pending PO
            var pendingPO = _poDataAccess.GetPendingRequestByVariant(variantId);

            if (pendingPO == null)
                throw new Exception("No pending request found for this item.");

            // ✅ FIX: Convert dynamic object to a Case-Insensitive Dictionary
            // This solves the error: "key 'Id' does not exist" if the DB returns 'id' or 'ID'
            var poData = new Dictionary<string, object>(
                (IDictionary<string, object>)pendingPO,
                StringComparer.OrdinalIgnoreCase
            );

            // ✅ Safety Check: Verify ID exists before crashing
            if (!poData.ContainsKey("Id"))
            {
                // Debug info to help you see what columns ARE returned
                var availableKeys = string.Join(", ", poData.Keys);
                throw new Exception($"Server Error: The database query did not return an 'Id' column. Available columns are: {availableKeys}");
            }

            // ✅ Robust Extraction
            int poReqId = Convert.ToInt32(poData["Id"]);
            int requestedQty = Convert.ToInt32(poData["Quantity"]);

            // Check if this belongs to a Bulk Order (Case-Insensitive check)
            int? bulkOrderId = null;
            if (poData.ContainsKey("BulkPurchaseOrderId") && poData["BulkPurchaseOrderId"] != null)
            {
                bulkOrderId = Convert.ToInt32(poData["BulkPurchaseOrderId"]);
            }

            if (qty > requestedQty)
            {
                throw new Exception($"Error: You cannot receive more stock ({qty}) than was requested ({requestedQty}).");
            }

            string connStr = _config.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // A. Insert PO Received
                        int receivedId = _poReceiveDA.Insert(poReqId, qty, price, invoice, remarks, trans);
                        if (receivedId <= 0) throw new Exception("Failed to save Receipt.");

                        // B. Update Stock 
                        _stockDA.AddStock(variantId, qty, trans);

                        // C. Log Transaction
                        _invTransDA.InsertInTransaction(receivedId, variantId, qty, price, remarks, trans);

                        // D. Close PO Request
                        _poDataAccess.UpdateStatus(poReqId, "Received", trans);

                        // E. Check if Bulk Order is fully processed
                        if (bulkOrderId.HasValue)
                        {
                            CheckAndCloseBulkOrder(bulkOrderId.Value, conn, trans);
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        public dynamic GetVariantStatus(int variantId)
        {
            return _poDataAccess.GetVariantStatus(variantId);
        }

        public List<dynamic> GetInventorySortedByStockAsc()
        {
            // Reuse the existing DA method but sort it strictly in memory
            var allStock = _poDataAccess.GetInventoryStatus();
            return allStock.OrderBy(x => (int)x.CurrentStock).ToList();
        }

        public void CreateBulkOrder(BulkPurchaseOrder bulkOrder, List<PoRequested> items)
        {
            if (items == null || !items.Any()) throw new Exception("No items selected for bulk order.");

            string connStr = _config.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Insert Bulk Order Header
                        bulkOrder.AgreementDate = DateTime.UtcNow;
                        bulkOrder.CreatedAt = DateTime.UtcNow;
                        bulkOrder.UpdatedAt = DateTime.UtcNow;
                        bulkOrder.TotalTargetQuantity = items.Sum(x => x.Quantity);
                        bulkOrder.TotalTargetAmount = 0;
                        bulkOrder.Status = "Active";

                        // === CHANGE 1: We don't rely on the return value for the ID ===
                        long rowsAffected = _bulkOrderDA.Insert(bulkOrder, trans);

                        // Check rows affected to ensure insert happened
                        if (rowsAffected <= 0) throw new Exception("Failed to create Bulk Order Header.");

                        // === CRITICAL FIX ===
                        // The DA updates bulkOrder.Id with the real Output Parameter from SQL
                        int realNewId = bulkOrder.Id;

                        // 2. Insert All Child Requests
                        foreach (var item in items)
                        {
                            // === CHANGE 2: Use the Object's ID, not the return value ===
                            item.BulkPurchaseOrderId = realNewId;

                            item.VendorId = bulkOrder.VendorId;
                            item.Status = "Pending";
                            item.RequestDate = DateTime.UtcNow;
                            item.CreatedAt = DateTime.UtcNow;
                            item.UpdatedAt = DateTime.UtcNow;
                            item.CreatedBy = bulkOrder.CreatedBy;
                            item.ReferenceNo = bulkOrder.AgreementNumber;

                            _poDataAccess.Insert(item, trans);
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        public List<BulkPurchaseOrder> GetBulkOrdersReceivedList()
        {
            var list = _bulkOrderDA.GetAll();
            return list.OrderByDescending(x => x.CreatedAt).ToList();
        }

        // Implementation matches the Interface and the ViewModel
        public List<BulkOrderItemViewModel> GetBulkOrderItems(int bulkOrderId)
        {
            var items = new List<BulkOrderItemViewModel>();
            string connStr = _config.GetConnectionString("DefaultConnection");

            using (var conn = new SqlConnection(connStr))
            {
                // ✅ UPDATED SQL: Added PR.Id and PR.ProductVariantId
                string query = @"
            SELECT 
                PR.Id AS PoRequestId, 
                PR.ProductVariantId,
                P.ProductName,
                PV.VariantName,
                PR.Quantity,
                PR.Status,
                PR.RequestDate
            FROM PoRequested PR
            JOIN ProductVariant PV ON PR.ProductVariantId = PV.Id
            JOIN Product P ON PV.ProductId = P.Id
            WHERE PR.BulkPurchaseOrderId = @BulkOrderId
            ORDER BY PR.Id DESC";

                var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BulkOrderId", bulkOrderId);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new BulkOrderItemViewModel
                        {
                            // ✅ Map the new IDs
                            PoRequestId = Convert.ToInt32(reader["PoRequestId"]),
                            ProductVariantId = Convert.ToInt32(reader["ProductVariantId"]),

                            ProductName = reader["ProductName"].ToString(),
                            VariantName = reader["VariantName"].ToString(),
                            Quantity = reader["Quantity"] != DBNull.Value ? Convert.ToInt32(reader["Quantity"]) : 0,
                            Status = reader["Status"].ToString(),
                            RequestDate = reader["RequestDate"] != DBNull.Value ? Convert.ToDateTime(reader["RequestDate"]) : DateTime.MinValue
                        };
                        items.Add(item);
                    }
                }
            }
            return items;
        }


        public void RejectPurchaseOrder(int poRequestId)
        {
            string connStr = _config.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Get the PO first to find the BulkOrderId
                        // We use a raw query here to be safe and fast within the transaction
                        int? bulkOrderId = null;
                        string getBulkIdQuery = "SELECT BulkPurchaseOrderId FROM PoRequested WHERE Id = @Id";
                        using (var cmd = new SqlCommand(getBulkIdQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@Id", poRequestId);
                            var result = cmd.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                            {
                                bulkOrderId = Convert.ToInt32(result);
                            }
                        }

                        // 2. Reject the Item
                        _poDataAccess.UpdateStatus(poRequestId, "Rejected", trans);

                        // ✅ 3. NEW LOGIC: Check if Bulk Order is fully processed
                        if (bulkOrderId.HasValue)
                        {
                            CheckAndCloseBulkOrder(bulkOrderId.Value, conn, trans);
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        
        #endregion




    }
}