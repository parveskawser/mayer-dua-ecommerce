using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;
using MDUA.Framework.Exceptions;
using System;
using System.Collections.Generic;

namespace MDUA.Facade
{
    public class VendorFacade : IVendorFacade
    {
        private readonly IVendorDataAccess _vendorDataAccess;
        private readonly IPoRequestedDataAccess _poRequestedDataAccess;

        public VendorFacade(IVendorDataAccess vendorDataAccess, IPoRequestedDataAccess poRequestedDataAccess)
        {
            _vendorDataAccess = vendorDataAccess;

            _poRequestedDataAccess = poRequestedDataAccess;
        }

        public long Insert(Vendor vendor)
        {
            vendor.CreatedAt = DateTime.UtcNow;
            return _vendorDataAccess.Insert(vendor);
        }

        public long Update(Vendor vendor)
        {
            vendor.UpdatedAt = DateTime.UtcNow;
            return _vendorDataAccess.Update(vendor);
        }

        public long Delete(int id)
        {
            return _vendorDataAccess.Delete(id);
        }

        public Vendor Get(int id)
        {
            return _vendorDataAccess.Get(id);
        }

        public VendorList GetAll()
        {
            return _vendorDataAccess.GetAll();
        }

        public VendorList GetPaged(PagedRequest request)
        {
            return _vendorDataAccess.GetPaged(request);
        }

        public List<dynamic> GetVendorOrderHistory(int vendorId)
        {
            return _poRequestedDataAccess.GetVendorHistory(vendorId);
        }

        // ... existing code ...

        public long AddPayment(VendorPayment payment)
        {
            // 1. Validation
            if (payment.Amount <= 0)
                throw new WorkflowException("Payment amount must be greater than zero.");

            if (payment.VendorId <= 0)
                throw new WorkflowException("Invalid Vendor selected.");

            // 2. Defaults
            payment.CreatedAt = DateTime.UtcNow;
            if (payment.PaymentDate == DateTime.MinValue)
                payment.PaymentDate = DateTime.UtcNow;

            // Ensure Status is set
            if (string.IsNullOrEmpty(payment.Status))
                payment.Status = "Completed";

            // 3. Call DA
            return _vendorDataAccess.InsertPayment(payment);
        }

        public void ApplyCredit(int creditPaymentId, int billId, decimal amount, string username)
        {
            if (amount <= 0) throw new WorkflowException("Amount must be greater than zero.");

            // Pass the username down to DataAccess
            _vendorDataAccess.ApplyCredit(creditPaymentId, billId, amount, username);
        }

        public List<dynamic> GetAvailableCredits(int vendorId)
        {
            return _vendorDataAccess.GetAvailableCredits(vendorId);
        }
        // ✅ NEW Method implementation
        public List<dynamic> GetPendingBills(int vendorId)
        {
            return _vendorDataAccess.GetPendingBills(vendorId);
        }

        public (List<dynamic> Items, int TotalCount) GetVendorOrderHistory(int vendorId, int page, int pageSize)
        {
            return _poRequestedDataAccess.GetVendorHistoryPaged(vendorId, page, pageSize);
        }

        // Update Interface first: 
        // (List<dynamic>, int) GetVendorOrderHistory(int vendorId, int page, int pageSize, string search, string status, string type);

        public (List<dynamic> Items, int TotalCount) GetVendorOrderHistory(int vendorId, int page, int pageSize, string search, string status, string type)
        {
            // Pass default "all" if null
            return _poRequestedDataAccess.GetVendorHistoryPaged(vendorId, page, pageSize, search ?? "", status ?? "all", type ?? "all");
        }

        // Update Interface: 
        // (List<dynamic> Items, int TotalCount) GetVendorOrderHistory(int vendorId, int page, int pageSize, string search, string status, string type, DateTime? fromDate, DateTime? toDate);

        public (List<dynamic> Items, int TotalCount) GetVendorOrderHistory(int vendorId, int page, int pageSize, string search, string status, string type, DateTime? fromDate, DateTime? toDate)
        {
            return _poRequestedDataAccess.GetVendorHistoryPaged(vendorId, page, pageSize, search ?? "", status ?? "all", type ?? "all", fromDate, toDate);
        }
    }
}