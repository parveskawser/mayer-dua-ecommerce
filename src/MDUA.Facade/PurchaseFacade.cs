using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MDUA.DataAccess.Interface; // Use Interfaces
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using System.Linq;

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

        // 2. Inject them in the Constructor
        public PurchaseFacade(
            IPoRequestedDataAccess poDataAccess,
            IVendorDataAccess vendorDataAccess,
            IPoReceivedDataAccess poReceiveDA,
            IVariantPriceStockDataAccess stockDA,
            IInventoryTransactionDataAccess invTransDA,
            IConfiguration config)
        {
            _poDataAccess = poDataAccess;
            _vendorDataAccess = vendorDataAccess;
            _poReceiveDA = poReceiveDA;
            _stockDA = stockDA;
            _invTransDA = invTransDA;
            _config = config;
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
            po.RequestDate = DateTime.Now;
            po.Status = "Pending";
            po.CreatedAt = DateTime.Now;
            po.UpdatedAt = DateTime.Now;
            if (string.IsNullOrEmpty(po.CreatedBy)) po.CreatedBy = "Admin";
            if (po.Remarks == null) po.Remarks = "";
            if (po.ReferenceNo == null) po.ReferenceNo = "";

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

            // (Dictionary cast because 'dynamic' case-sensitivity)
            var poData = (IDictionary<string, object>)pendingPO;

            // Retrieve values 
            int poReqId = (int)poData["id"];
            int requestedQty = (int)poData["quantity"];

            //  Validate Quantity (Received cannot exceed Requested)
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
                        // Use the retrieved poReqId
                        int receivedId = _poReceiveDA.Insert(poReqId, qty, price, invoice, remarks, trans);

                        if (receivedId <= 0)
                        {
                            throw new Exception("Failed to save Receipt. The database returned an invalid ID.");
                        }

                        // B. Update Stock 
                        _stockDA.AddStock(variantId, qty, trans);

                        // C. Log Transaction
                        _invTransDA.InsertInTransaction(receivedId, variantId, qty, price, remarks, trans);

                        // D. Close PO Request
                        _poDataAccess.UpdateStatus(poReqId, "Received", trans);

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
        public List<dynamic> GetVariantsForDropdown()
        {
            return _poDataAccess.GetVariantsSortedByStock();
        }

// File: MDUA.Facade/PurchaseFacade.cs

        public void CreateBulkPurchaseOrder(List<PoRequested> orders)
        {
            // Loop through every item sent from the JavaScript
            foreach (var order in orders)
            {
                // Set default values that weren't in the JS payload
                order.RequestDate = DateTime.Now;
                order.Status = "Pending";
                order.CreatedAt = DateTime.Now;
                order.CreatedBy = "Admin"; // Or use _httpContextAccessor to get current user
                if (order.ReferenceNo == null) order.ReferenceNo = "";
        
                // Save to Database using your existing DataAccess
                _poDataAccess.Insert(order);
            }
        } 
        #endregion
    }
}