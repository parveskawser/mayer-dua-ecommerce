using Microsoft.AspNetCore.Mvc;
using MDUA.Facade.Interface;
using MDUA.Entities;
using MDUA.Entities.List;
using MDUA.Framework.Exceptions;
using MDUA.Web.UI.Controllers;
using System;
using Newtonsoft.Json;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDUA.Web.Controllers
{
    public class VendorController : BaseController
    {
        private readonly IVendorFacade _vendorFacade;

        // Injected dependencies for the dropdown lists required by the View
        private readonly IPaymentMethodFacade _paymentMethodFacade;
        private readonly IPurchaseFacade _purchaseFacade;

        public VendorController(
            IVendorFacade vendorFacade,
            IPaymentMethodFacade paymentMethodFacade,
            IPurchaseFacade purchaseFacade
         )
        {
            _vendorFacade = vendorFacade;
            _paymentMethodFacade = paymentMethodFacade;
            _purchaseFacade = purchaseFacade;

        }

        // MDUA.Web/Controllers/VendorController.cs

        [Route("vendor/all")]
        [HttpGet]
        public IActionResult Index()
        {
            // 1. Get Current Company ID
            // Ensure this matches how you store the ID in your Claims (e.g., "CompanyId")
            int companyId = Convert.ToInt32(User.FindFirst("CompanyId")?.Value ?? "1");

            // 2. Call the new filtered method
            var list = _vendorFacade.GetByCompany(companyId);

            return View(list);
        }

        [Route("vendor/add")]

        [HttpGet]
        public IActionResult Add(int? id)
        {
            Vendor model = new Vendor();
            if (id.HasValue && id > 0)
            {
                model = _vendorFacade.Get(id.Value);
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(Vendor vendor)
        {
            try
            {
                // 1. Remove validation for System Fields
                ModelState.Remove("CreatedBy");
                ModelState.Remove("UpdatedBy");
                ModelState.Remove("CreatedAt");
                ModelState.Remove("UpdatedAt");
                ModelState.Remove("Amount");
                ModelState.Remove("Remarks");
                ModelState.Remove("CustomProperties");

                // 2. Check Name Requirement
                if (string.IsNullOrWhiteSpace(vendor.VendorName))
                {
                    ModelState.AddModelError("VendorName", "Vendor Name is required.");
                }

                // =========================================================================
                // IMPORTANT: I REMOVED THE DUPLICATE CHECKS HERE (existingVendors.Any...)
                // We rely on the Facade to handle duplicates/linking now.
                // =========================================================================

                if (!ModelState.IsValid) return View("Add", vendor);

                // 3. Save Logic
                if (vendor.Id > 0)
                {
                    // UPDATE EXISTING
                    vendor.UpdatedBy = User.Identity?.Name ?? "Admin";
                    _vendorFacade.Update(vendor);
                    TempData["Success"] = "Vendor updated successfully.";
                }
                else
                {
                    // INSERT NEW (Or Link Existing)
                    vendor.CreatedBy = User.Identity?.Name ?? "Admin";

                    // Get Company ID
                    int companyId = Convert.ToInt32(User.FindFirst("CompanyId")?.Value ?? "1");

                    // CALL FACADE (It will handle the "Unique Constraint" internally)
                    _vendorFacade.Insert(vendor, companyId);

                    TempData["Success"] = "Vendor added successfully.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // This catches the error if Facade fails. 
                // With the fix above, SQL errors regarding unique keys should stop.
                TempData["Error"] = "System Error: " + ex.Message;
                return View("Add", vendor);
            }
        }

        // MDUA.Web/Controllers/VendorController.cs

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                // 1. Get Current Company ID
                int companyId = Convert.ToInt32(User.FindFirst("CompanyId")?.Value ?? "1");

                // 2. Call the new Facade logic
                _vendorFacade.Delete(id, companyId);

                return Json(new { success = true, message = "Vendor deleted successfully." });
            }
            catch (Exception ex)
            {
                // 1. Get the actual error
                string actualError = ex.InnerException?.Message ?? ex.Message;

                // 2. Check for Foreign Key Constraint Violation (SQL Error 547)
                // This catches the case where we tried to delete the Main Vendor record 
                // but SQL stopped us because of existing Purchase Orders.
                if (actualError.Contains("REFERENCE constraint") ||
                    actualError.Contains("conflicted with the REFERENCE") ||
                    actualError.Contains("FK_"))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Cannot delete this Vendor because they have connected records (Purchase Orders, Bills, etc)."
                    });
                }

                return Json(new { success = false, message = "System Error: " + actualError });
            }
        }

        [HttpGet]
        public IActionResult GetHistory(int id)
        {
            int companyId = Convert.ToInt32(User.FindFirst("CompanyId")?.Value ?? "1");

            try
            {
                var history = _vendorFacade.GetVendorOrderHistory(id, companyId);
                return Json(new { success = true, data = history });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =============================================================
        // PAYMENT SECTION
        // =============================================================
        [Route("vendor/payment")]

        [HttpGet]

        public IActionResult AddPayment()
        {
            try
            {
                // Load all dropdown data required for the view
                LoadPaymentDropdowns();

                var model = new VendorPayment
                {
                    PaymentDate = DateTime.UtcNow
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Could not load payment form: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult SavePayment(VendorPayment payment)
        {
            try
            {
                // 1. Basic Validation
                if (payment.VendorId <= 0)
                    throw new WorkflowException("Please select a valid Vendor.");

                if (payment.Amount <= 0)
                    throw new WorkflowException("Payment amount must be greater than zero.");

                // 2. STRICT LOGIC: Map to allowed DB values ('Purchase', 'Advance', 'Refund')
                if (payment.PoReceivedId.HasValue && payment.PoReceivedId.Value > 0)
                {
                    // Paying against a specific Bill (Invoice)
                    payment.PaymentType = "Purchase";
                }
                else if (payment.PoRequestedId.HasValue && payment.PoRequestedId.Value > 0)
                {
                    // Paying against a Purchase Request (Before receiving goods)
                    payment.PaymentType = "Advance";
                }
                else
                {
                    // Fallback for general payments (Assuming it's for a purchase)
                    payment.PaymentType = "Purchase";
                }

                // 3. Set System Fields
                payment.CreatedBy = User.Identity.Name ?? "Admin";
                payment.Status = "Completed"; // Default to Completed so Trigger updates the balance immediately

                // 4. Save to Database
                _vendorFacade.AddPayment(payment);

                TempData["Success"] = "Payment recorded successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;

                // Reload dropdowns so the page doesn't break on error
                LoadPaymentDropdowns();
                return View("AddPayment", payment);
            }
        }


        [HttpGet]
        public JsonResult GetPendingBills(int vendorId)
        {
            try
            {
                // 1. Get Current Company ID
                int companyId = Convert.ToInt32(User.FindFirst("CompanyId")?.Value ?? "1");

                // 2. Pass companyId to the Facade
                var bills = _vendorFacade.GetPendingBills(vendorId, companyId);

                return Json(bills);
            }
            catch (Exception ex)
            {
                return Json(new List<dynamic>());
            }
        }
        private void LoadPaymentDropdowns()
        {
            // 1. Get Current Company ID
            int companyId = Convert.ToInt32(User.FindFirst("CompanyId")?.Value ?? "1");

            // 2. Use GetByCompany instead of GetAll
            ViewBag.VendorList = _vendorFacade.GetByCompany(companyId);

            ViewBag.PaymentMethodList = _paymentMethodFacade.GetAll();
        }

        [HttpPost]
        public IActionResult ApplyCredit(int creditId, int billId, decimal amount)
        {
            try
            {
                string username = User.Identity.Name ?? "System"; // Get current user

                // Pass username to the Facade
                _vendorFacade.ApplyCredit(creditId, billId, amount, username);

                return Json(new { success = true, message = "Credit applied successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetVendorCredits(int vendorId)
        {
            var credits = _vendorFacade.GetAvailableCredits(vendorId);
            return Json(credits);
        }





        [HttpPost]
        [Route("vendor/history")]
        public IActionResult History(int id)
        {
            // Load the base View. We will fetch data via AJAX immediately after load.
            var vendor = _vendorFacade.Get(id);

            if (vendor == null) return RedirectToAction("Index");

            ViewBag.VendorName = vendor.VendorName;
            ViewBag.VendorId = id;

            // Initial Filter States
            ViewBag.StatusList = new List<string> { "Pending", "Received", "Cancelled" }; // Or fetch from DB

            return View();
        }

        [HttpPost]
        [Route("Vendor/GetHistoryData")]

        public IActionResult GetHistoryData(int id, int page = 1, int pageSize = 10, string search = "", string status = "all", string type = "all", string dateRange = "all", DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (page < 1) page = 1;
            int companyId = Convert.ToInt32(User.FindFirst("CompanyId")?.Value ?? "1");
            // --- Date Logic (Copied from OrderController logic) ---
            DateTime today = DateTime.UtcNow.Date;
            DateTime? start = null;
            DateTime? end = null;

            if (dateRange != "all")
            {
                switch (dateRange)
                {
                    case "today":
                        start = today;
                        end = today.AddDays(1).AddTicks(-1);
                        break;
                    case "yesterday":
                        start = today.AddDays(-1);
                        end = today.AddDays(1).AddTicks(-1); // Yesterday covers 24h
                        break;
                    case "thisWeek":
                        int diff = (7 + (today.DayOfWeek - DayOfWeek.Sunday)) % 7;
                        start = today.AddDays(-1 * diff).Date;
                        end = today.AddDays(1).AddTicks(-1);
                        break;
                    case "lastWeek":
                        int diffLast = (7 + (today.DayOfWeek - DayOfWeek.Sunday)) % 7;
                        start = today.AddDays(-1 * diffLast).AddDays(-7).Date;
                        end = start.Value.AddDays(7).AddTicks(-1);
                        break;
                    case "thisMonth":
                        start = new DateTime(today.Year, today.Month, 1);
                        end = today.AddDays(1).AddTicks(-1);
                        break;
                    case "lastMonth":
                        var lastMonth = today.AddMonths(-1);
                        start = new DateTime(lastMonth.Year, lastMonth.Month, 1);
                        end = new DateTime(today.Year, today.Month, 1).AddTicks(-1);
                        break;
                    case "custom":
                        if (fromDate.HasValue) start = fromDate.Value.Date;
                        if (toDate.HasValue) end = toDate.Value.Date.AddDays(1).AddTicks(-1);
                        break;
                }
            }


            // 1. Fetch Data
            var result = _vendorFacade.GetVendorOrderHistory(id, companyId, page, pageSize, search, status, type, start, end);
            // 2. Return JSON
            int totalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);

            return Json(new
            {
                success = true,
                data = result.Items,
                totalRows = result.TotalCount,
                totalPages = totalPages,
                currentPage = page
            });
        }

   
        [HttpGet]
        public JsonResult CheckUnique(string field, string value, int id)
        {
            try
            {
                int companyId = Convert.ToInt32(User.FindFirst("CompanyId")?.Value ?? "1");
                var allVendors = _vendorFacade.GetAll();
                Vendor existingGlobal = null;

                // === FIX IS HERE: ADD NULL CHECKS ===

                if (string.Equals(field, "VendorName", StringComparison.OrdinalIgnoreCase))
                {
                    existingGlobal = allVendors.FirstOrDefault(v => v.Id != id && !string.IsNullOrEmpty(v.VendorName) && v.VendorName.Trim().Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
                }
                else if (string.Equals(field, "Email", StringComparison.OrdinalIgnoreCase))
                {
                    // Check v.Email != null before Trimming
                    existingGlobal = allVendors.FirstOrDefault(v => v.Id != id && v.Email != null && v.Email.Trim().Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
                }
                else if (string.Equals(field, "Phone", StringComparison.OrdinalIgnoreCase))
                {
                    // Check v.Phone != null before Trimming
                    existingGlobal = allVendors.FirstOrDefault(v => v.Id != id && v.Phone != null && v.Phone.Trim().Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
                }

                if (existingGlobal == null)
                {
                    return Json(new { isUnique = true });
                }
                else
                {
                    bool isAlreadyMyVendor = _vendorFacade.IsVendorLinkedToCompany(existingGlobal.Id, companyId);
                    if (isAlreadyMyVendor)
                    {
                        return Json(new { isUnique = false });
                    }
                    else
                    {
                        return Json(new { isUnique = true });
                    }
                }
            }
            catch
            {
                return Json(new { isUnique = true });
            }
        }


        // Add this helper method to your Controller or Service
        // Helper to unpack the JSON children into real list items
    }
}