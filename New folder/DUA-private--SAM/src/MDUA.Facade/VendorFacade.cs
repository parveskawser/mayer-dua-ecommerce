using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;
using MDUA.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace MDUA.Facade
{
    public class VendorFacade : IVendorFacade
    {
        private readonly IVendorDataAccess _vendorDataAccess;
        private readonly IPoRequestedDataAccess _poRequestedDataAccess;
        private readonly ICompanyVendorDataAccess _companyVendorDataAccess; // 1. Add this dependency
        public VendorFacade(IVendorDataAccess vendorDataAccess, IPoRequestedDataAccess poRequestedDataAccess, ICompanyVendorDataAccess companyVendorDataAccess)
        {
            _vendorDataAccess = vendorDataAccess;

            _poRequestedDataAccess = poRequestedDataAccess;
            _companyVendorDataAccess = companyVendorDataAccess;

        }

        public long Insert(Vendor vendor, int companyId)
        {
            // Define transaction options to prevent locking issues (ReadCommitted is standard)
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.MaximumTimeout
            };

            // Wrap the ENTIRE operation in a TransactionScope
            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                // 1. Check if this vendor exists GLOBALLY (Null-Safe Check)
                var allVendors = _vendorDataAccess.GetAll();

                var existingVendor = allVendors.FirstOrDefault(v =>
                    (!string.IsNullOrEmpty(vendor.Phone) && v.Phone != null && v.Phone.Trim().Equals(vendor.Phone.Trim(), StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(vendor.Email) && v.Email != null && v.Email.Trim().Equals(vendor.Email.Trim(), StringComparison.OrdinalIgnoreCase))
                );

                long vendorIdToLink = 0;

                if (existingVendor != null)
                {
                    // --- CASE A: VENDOR ALREADY EXISTS ---
                    vendorIdToLink = existingVendor.Id;

                    // Check if already linked to YOUR Company
                    var companyLinks = _companyVendorDataAccess.GetByVendorId((int)vendorIdToLink);
                    bool isAlreadyLinked = companyLinks.Any(x => x.CompanyId == companyId);

                    if (isAlreadyLinked)
                    {
                        scope.Complete(); // <--- MARK SUCCESS BEFORE RETURNING
                        return vendorIdToLink;
                    }
                }
                else
                {
                    // --- CASE B: TOTALLY NEW VENDOR ---
                    vendor.CreatedAt = DateTime.UtcNow;

                    // This inserts into DB, but if logic crashes later, TransactionScope will ROLLBACK this.
                    _vendorDataAccess.Insert(vendor);

                    if (vendor.Id > 0)
                    {
                        vendorIdToLink = vendor.Id;
                    }
                }

                // 2. Create the Link (Only if we have a valid Vendor ID)
                if (vendorIdToLink > 0)
                {
                    // Safety check to prevent duplicate key errors
                    var currentLinks = _companyVendorDataAccess.GetByVendorId((int)vendorIdToLink);
                    if (!currentLinks.Any(x => x.CompanyId == companyId))
                    {
                        var link = new CompanyVendorBase
                        {
                            CompanyId = companyId,
                            VendorId = (int)vendorIdToLink
                        };
                        _companyVendorDataAccess.Insert(link);
                    }

                    // --- EVERYTHING SUCCEEDED ---
                    scope.Complete(); // <--- COMMITS THE TRANSACTION
                    return vendorIdToLink;
                }

                return 0;
            } // End of using(scope) - If scope.Complete() wasn't called, everything here Auto-Rollbacks
        }
        public bool IsVendorLinkedToCompany(int vendorId, int companyId)
        {
            // 1. Get all company links for this specific vendor
            var links = _companyVendorDataAccess.GetByVendorId(vendorId);

            // 2. Check if your specific companyId is in that list
            return links.Any(x => x.CompanyId == companyId);
        }

        public long Update(Vendor vendor)
        {
            vendor.UpdatedAt = DateTime.UtcNow;
            return _vendorDataAccess.Update(vendor);
        }

        public long Delete(int vendorId, int companyId)
        {
            // 1. Find the specific link for this company
            // We need the Primary Key (Id) of the CompanyVendor table to delete the link.
            var allLinks = _companyVendorDataAccess.GetByVendorId(vendorId);

            var myLink = allLinks.FirstOrDefault(x => x.CompanyId == companyId);

            if (myLink == null) return 0; // Already deleted or doesn't exist for this company

            // 2. Check if this vendor is SHARED (Linked to other companies)
            bool isShared = allLinks.Count > 1;

            if (isShared)
            {
                // --- SCENARIO 1: SHARED VENDOR ---
                // Just unlink from my company. Do NOT touch the main Vendor table.
                return _companyVendorDataAccess.Delete(myLink.Id);
            }
            else
            {
                // --- SCENARIO 2: EXCLUSIVE VENDOR ---
                // Vendor belongs ONLY to me. Attempt to delete everything.

                // Use Transaction to ensure we don't accidentally unlink a vendor 
                // that we can't fully delete (e.g., if they have Purchase Orders).
                using (var scope = new TransactionScope())
                {
                    // A. Delete the Link first 
                    // (Otherwise FK constraint might prevent deleting Vendor parent)
                    _companyVendorDataAccess.Delete(myLink.Id);

                    // B. Try to Delete the Vendor Record
                    // If this fails (FK Constraint: "Used in Purchase Order"), 
                    // the Exception will bubble up, Transaction will abort, 
                    // and the Link (Step A) will be restored (Rolled Back).
                    _vendorDataAccess.Delete(vendorId);

                    scope.Complete();
                    return 1; // Success
                }
            }
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

        public List<dynamic> GetVendorOrderHistory(int vendorId, int companyId)
        {
            return _poRequestedDataAccess.GetVendorHistory(vendorId, companyId);
        }


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
        // MDUA.Facade/VendorFacade.cs

        public List<dynamic> GetPendingBills(int vendorId, int companyId)
        {
            // Pass companyId to DataAccess
            return _vendorDataAccess.GetPendingBills(vendorId, companyId);
        }

        public (List<dynamic> Items, int TotalCount) GetVendorOrderHistory(int vendorId, int companyId, int page, int pageSize)
        {
            // Pass companyId to DataAccess
            return _poRequestedDataAccess.GetVendorHistoryPaged(vendorId, companyId, page, pageSize);
        }

        // 2. Filtered Method (Without Date)
        public (List<dynamic> Items, int TotalCount) GetVendorOrderHistory(int vendorId, int companyId, int page, int pageSize, string search, string status, string type)
        {
            // Pass companyId to DataAccess
            return _poRequestedDataAccess.GetVendorHistoryPaged(vendorId, companyId, page, pageSize, search ?? "", status ?? "all", type ?? "all");
        }

        // Update Interface: 
        // (List<dynamic> Items, int TotalCount) GetVendorOrderHistory(int vendorId, int page, int pageSize, string search, string status, string type, DateTime? fromDate, DateTime? toDate);

        public (List<dynamic> Items, int TotalCount) GetVendorOrderHistory(int vendorId, int companyId, int page, int pageSize, string search, string status, string type, DateTime? fromDate, DateTime? toDate)
        {
            // Pass companyId to the Data Access
            return _poRequestedDataAccess.GetVendorHistoryPaged(vendorId, companyId, page, pageSize, search ?? "", status ?? "all", type ?? "all", fromDate, toDate);
        }
        public VendorList GetByCompany(int companyId)
        {
            return _vendorDataAccess.GetByCompany(companyId);
        }

    }
}