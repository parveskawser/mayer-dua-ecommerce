using System;
using System.Collections.Generic; // <--- ADD THIS LINE (Required for List<dynamic>)
using MDUA.Entities;
using MDUA.Entities.List;
using MDUA.Framework;

namespace MDUA.Facade.Interface
{
    public interface IVendorFacade
    {
        long Insert(Vendor vendor);
        long Update(Vendor vendor);
        long Delete(int id);
        Vendor Get(int id);
        VendorList GetAll();
        VendorList GetPaged(PagedRequest request);

        List<dynamic> GetVendorOrderHistory(int vendorId);
        long AddPayment(VendorPayment payment);
        List<dynamic> GetPendingBills(int vendorId);

        void ApplyCredit(int creditPaymentId, int billId, decimal amount, string username);
        List<dynamic> GetAvailableCredits(int vendorId);

        (List<dynamic> Items, int TotalCount) GetVendorOrderHistory(int vendorId, int page, int pageSize);
        (List<dynamic> Items, int TotalCount) GetVendorOrderHistory(int vendorId, int page, int pageSize, string search, string status, string type);

        (List<dynamic> Items, int TotalCount) GetVendorOrderHistory(int vendorId, int page, int pageSize, string search, string status, string type, DateTime? fromDate, DateTime? toDate);


    }
}