using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework.Exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace MDUA.Facade
{
    public class PurchaseFacade : IPurchaseFacade
    {
        private readonly IPoRequestedDataAccess _poDataAccess;
        private readonly IVendorDataAccess _vendorDataAccess;
        private readonly IPoReceivedDataAccess _poReceiveDA;
        private readonly IVariantPriceStockDataAccess _stockDA;
        private readonly IInventoryTransactionDataAccess _invTransDA;
        private readonly IConfiguration _config;
        private readonly IBulkPurchaseOrderDataAccess _bulkOrderDA;

        // Added Vendor Payment DataAccess
        private readonly IVendorPaymentDataAccess _vendorPaymentDataAccess;

        public PurchaseFacade(
            IPoRequestedDataAccess poDataAccess,
            IVendorDataAccess vendorDataAccess,
            IPoReceivedDataAccess poReceiveDA,
            IVariantPriceStockDataAccess stockDA,
            IInventoryTransactionDataAccess invTransDA,
            IBulkPurchaseOrderDataAccess bulkOrderDA,
            IConfiguration config,
            IVendorPaymentDataAccess vendorPaymentDataAccess)
        {
            _poDataAccess = poDataAccess;
            _vendorDataAccess = vendorDataAccess;
            _poReceiveDA = poReceiveDA;
            _stockDA = stockDA;
            _invTransDA = invTransDA;
            _config = config;
            _bulkOrderDA = bulkOrderDA;
            _vendorPaymentDataAccess = vendorPaymentDataAccess;
        }

        #region ICommonFacade Implementation
        public long Delete(int id) => _poDataAccess.Delete(id);
        public PoRequested Get(int id) => _poDataAccess.Get(id);
        public PoRequestedList GetAll() => _poDataAccess.GetAll();
        public PoRequestedList GetByQuery(string query) => _poDataAccess.GetByQuery(query);
        public long Insert(PoRequestedBase obj) => _poDataAccess.Insert(obj);
        public long Update(PoRequestedBase obj) => _poDataAccess.Update(obj);
        public PoReceivedList GetAllReceived()
        {
            // Uses the injected IPoReceivedDataAccess to get the list
            return _poReceiveDA.GetAll();
        }
        #endregion

        #region Extended Methods

        // =========================================================================
        //  RECEIVE STOCK LOGIC (Purchase Order -> Stock + Payment)
        // =========================================================================
        public void ReceiveStock(int variantId, int qty, decimal price, string invoice, string remarks,
                           decimal paidAmount = 0, int? paymentMethodId = null, string paymentRef = null)
        {
            // 1. Validation Logic
            if (qty <= 0) throw new WorkflowException("Received quantity must be greater than zero.");
            if (price < 0) throw new WorkflowException("Buying price cannot be negative.");

            // Get Pending PO Data
            var pendingPO = _poDataAccess.GetPendingRequestByVariant(variantId);
            if (pendingPO == null)
                throw new WorkflowException("No pending Purchase Request found for this product variant.");

            var poData = new Dictionary<string, object>(
                (IDictionary<string, object>)pendingPO,
                StringComparer.OrdinalIgnoreCase
            );

            int poReqId = Convert.ToInt32(poData["Id"]);
            int requestedQty = Convert.ToInt32(poData["Quantity"]);
            int vendorId = Convert.ToInt32(poData["VendorId"]);

            // Check for over-receiving
            if (qty > requestedQty)
            {
                throw new WorkflowException($"Error: You cannot receive more stock ({qty}) than was requested ({requestedQty}). Please update the request first.");
            }

            // 2. Transaction Execution
            string connStr = _config.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // -------------------------------------------------------------
                        // Step A: Insert PO Receipt (GRN)
                        // -------------------------------------------------------------
                        // Note: We do NOT pass VendorId here. The SP looks it up automatically.
                        int receivedId = _poReceiveDA.Insert(poReqId, qty, price, invoice, remarks, trans, 0, "Unpaid");

                        if (receivedId <= 0) throw new WorkflowException("System Error: Failed to generate Receipt ID.");

                        // -------------------------------------------------------------
                        // Step B: Update Physical Stock
                        // -------------------------------------------------------------
                        _stockDA.AddStock(variantId, qty, trans);

                        // -------------------------------------------------------------
                        // Step C: Log Inventory Transaction
                        // -------------------------------------------------------------
                        _invTransDA.InsertInTransaction(receivedId, variantId, qty, price, remarks, trans);

                        // -------------------------------------------------------------
                        // Step D: Update Original Request Status
                        // -------------------------------------------------------------
                        _poDataAccess.UpdateStatus(poReqId, "Received", trans);

                        // -------------------------------------------------------------
                        // Step E: Bulk Order Logic (Handled by SQL Trigger)
                        // -------------------------------------------------------------

                        // -------------------------------------------------------------
                        // Step F: Record Vendor Payment (If applicable)
                        // -------------------------------------------------------------
                        if (paidAmount > 0)
                        {
                            if (!paymentMethodId.HasValue || paymentMethodId.Value == 0)
                                throw new WorkflowException("A valid Payment Method is required to record a payment.");

                            var payment = new VendorPayment
                            {
                                VendorId = vendorId,
                                PaymentMethodId = paymentMethodId.Value,
                                PoReceivedId = receivedId,
                                Amount = paidAmount,
                                ReferenceNumber = paymentRef,
                                PaymentType = "Purchase",
                                Status = "Completed",
                                PaymentDate = DateTime.UtcNow,
                                CreatedBy = "System",
                                Notes = "Immediate payment upon receipt"
                            };

                            _vendorPaymentDataAccess.InsertPayment(payment, trans);
                        }

                        trans.Commit();
                    }
                    catch (SqlException sqlEx)
                    {
                        trans.Rollback();

                        // ✅ Catch Duplicate Invoice Error specifically
                        // Error 2627 = Violation of PRIMARY KEY constraint
                        // Error 2601 = Violation of UNIQUE KEY constraint
                        if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                        {
                            throw new WorkflowException($"Error: The Invoice Number '{invoice}' has already been used for this Vendor. Please check your records.");
                        }

                        // Throw other database errors as normal
                        throw new WorkflowException($"Database Error: {sqlEx.Message}", sqlEx);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw new WorkflowException($"Transaction Failed: {ex.Message}", ex);
                    }
                }
            }
        }

        // =========================================================================
        //  BULK ORDER LOGIC
        // =========================================================================
        public void CreateBulkOrder(BulkPurchaseOrder bulkOrder, List<PoRequested> items)
        {
            if (items == null || !items.Any())
                throw new WorkflowException("No items selected for bulk order.");

            string connStr = _config.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Header
                        bulkOrder.AgreementDate = DateTime.UtcNow;
                        bulkOrder.CreatedAt = DateTime.UtcNow;
                        bulkOrder.UpdatedAt = DateTime.UtcNow;
                        bulkOrder.TotalTargetQuantity = items.Sum(x => x.Quantity);
                        bulkOrder.TotalTargetAmount = 0;
                        bulkOrder.Status = "Active";

                        long rows = _bulkOrderDA.Insert(bulkOrder, trans);
                        if (rows <= 0) throw new WorkflowException("Failed to create Bulk Order Header.");

                        int bulkId = bulkOrder.Id;

                        // 2. Details (Requests)
                        foreach (var item in items)
                        {
                            item.BulkPurchaseOrderId = bulkId;
                            item.VendorId = bulkOrder.VendorId;
                            item.Status = "Pending";
                            item.RequestDate = DateTime.UtcNow;
                            item.CreatedAt = DateTime.UtcNow;
                            item.CreatedBy = bulkOrder.CreatedBy;
                            item.ReferenceNo = bulkOrder.AgreementNumber;

                            // InsertPoRequested SP now has guardrails to prevent inserting 
                            // if it exceeds the Bulk Order limit.
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

        // =========================================================================
        //  HELPER / READ METHODS
        // =========================================================================

        public long CreatePurchaseOrder(PoRequested po)
        {
            try
            {
                // 1. Set Defaults
                po.RequestDate = DateTime.UtcNow;
                po.Status = "Pending";
                po.CreatedAt = DateTime.UtcNow;
                po.UpdatedAt = DateTime.UtcNow;
                po.CreatedBy = string.IsNullOrEmpty(po.CreatedBy) ? "System" : po.CreatedBy;
                po.Remarks ??= "";
                po.ReferenceNo ??= "";

                // 2. Call Data Access
                // Note: If this links to a Bulk Order, the SQL SP will check the limit.
                return _poDataAccess.Insert(po);
            }
            catch (SqlException ex)
            {
                // 3. Catch the custom SQL error (Error 51000)
                if (ex.Number == 51000)
                {
                    // Throw a clean error message to the UI (e.g. "Error: Exceeds Bulk Limit...")
                    throw new WorkflowException(ex.Message);
                }
                throw; // Throw other database errors normally
            }
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
                        // 1. Update the status to 'Rejected'
                        // The SQL Trigger [TR_PoRequested_Update_BulkOrder] listens to this update.
                        // It will automatically recalculate the Bulk Order's consumed quantity.
                        _poDataAccess.UpdateStatus(poRequestId, "Rejected", trans);

                        // 2. Commit
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
        public List<dynamic> GetInventoryStatus(int companyId) //  param
        {
            return _poDataAccess.GetInventoryStatus(companyId);
        }
        public dynamic GetPendingRequestInfo(int variantId) => _poDataAccess.GetPendingRequestByVariant(variantId);
        public dynamic GetVariantStatus(int variantId) => _poDataAccess.GetVariantStatus(variantId);
        public List<Vendor> GetAllVendors(int companyId) // Added param
        {
            // ✅ Call the new method in DataAccess
            return ((VendorDataAccess)_vendorDataAccess).GetByCompany(companyId);
        }
        public List<dynamic> GetInventorySortedByStockAsc(int companyId) // Added param
        {
            var allStock = _poDataAccess.GetInventoryStatus(companyId);
            return allStock.OrderBy(x => (int)x.CurrentStock).ToList();
        }
       
        
        public List<BulkPurchaseOrder> GetBulkOrdersReceivedList(int companyId) // Added param
        {
            return ((BulkPurchaseOrderDataAccess)_bulkOrderDA).GetByCompanyId(companyId);
        }
        
        
        public void ReceiveBulkStock(List<dynamic> items, string invoice, string remarks, decimal totalPaid, int? paymentMethodId, int vendorId, string username)
        {
            // Delegate strictly to DataAccess
            ((PoReceivedDataAccess)_poReceiveDA).ReceiveBulkStock(items, invoice, remarks, totalPaid, paymentMethodId, vendorId, username);
        }

        public void RejectBulkRemaining(int bulkOrderId)
        {
            // Delegate strictly to DataAccess
            ((PoRequestedDataAccess)_poDataAccess).RejectBulkRemaining(bulkOrderId);
        }

        public int? GetExistingInvoiceBulkId(string invoice, int vendorId)
        {
            return ((PoReceivedDataAccess)_poReceiveDA).GetBulkOrderIdByInvoice(invoice, vendorId);
        }
        public List<BulkOrderItemViewModel> GetBulkOrderItems(int bulkOrderId)
        {
            // Now calling DataAccess instead of raw SQL in Facade
            return ((BulkPurchaseOrderDataAccess)_bulkOrderDA).GetBulkOrderItems(bulkOrderId);
        }
        #endregion
    }
}