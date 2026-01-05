using Microsoft.AspNetCore.Mvc;
using MDUA.Facade.Interface;
using MDUA.Entities;
using MDUA.Entities.List;
using MDUA.Framework.Exceptions;
using MDUA.Web.UI.Controllers;
using System;

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

        [HttpGet]
        public IActionResult Index()
        {
            var list = _vendorFacade.GetAll();
            return View(list);
        }

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
                if (string.IsNullOrEmpty(vendor.VendorName))
                {
                    TempData["Error"] = "Vendor Name is required.";
                    return View("Add", vendor);
                }

                if (vendor.Id > 0)
                {
                    vendor.UpdatedBy = CurrentUserName;
                    _vendorFacade.Update(vendor);
                    TempData["Success"] = "Vendor updated successfully.";
                }
                else
                {
                    vendor.CreatedBy = CurrentUserName;
                    _vendorFacade.Insert(vendor);
                    TempData["Success"] = "Vendor added successfully.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error saving vendor: " + ex.Message;
                return View("Add", vendor);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _vendorFacade.Delete(id);
                return Json(new { success = true, message = "Vendor deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting vendor." });
            }
        }

        [HttpGet]
        public IActionResult GetHistory(int id)
        {
            try
            {
                var history = _vendorFacade.GetVendorOrderHistory(id);
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
                var bills = _vendorFacade.GetPendingBills(vendorId);
                return Json(bills);
            }
            catch (Exception ex)
            {
                return Json(new List<dynamic>());
            }
        }
        private void LoadPaymentDropdowns()
        {
            ViewBag.VendorList = _vendorFacade.GetAll();
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
        [Route("Vendor/History")]
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
            var result = _vendorFacade.GetVendorOrderHistory(id, page, pageSize, search, status, type, start, end);

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

        // NOTE: Update ExportHistory similarly to accept dateRange/fromDate/toDate and perform the same logic before calling Facade.//

        [HttpPost]
        [Route("Vendor/ExportHistory")]
        public IActionResult ExportHistory(int id, string search, string status, string type, string dateRange, DateTime? fromDate, DateTime? toDate, string scope, string selectedIds, string format)
        {
            // 1. Replicate Date Logic (Same as GetHistoryData)
            DateTime today = DateTime.UtcNow.Date;
            DateTime? start = null;
            DateTime? end = null;

            if (dateRange != "all" && !string.IsNullOrEmpty(dateRange))
            {
                switch (dateRange)
                {
                    case "today":
                        start = today;
                        end = today.AddDays(1).AddTicks(-1);
                        break;
                    case "yesterday":
                        start = today.AddDays(-1);
                        end = today.AddDays(1).AddTicks(-1);
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

            // 2. Fetch Data (Get ALL records matching filter)
            // Pass 10000 or a large number for pageSize to get all rows
            var result = _vendorFacade.GetVendorOrderHistory(id, 1, 100000, search, status, type, start, end);
            var dataToExport = result.Items;

            // 3. Apply "Selected Rows" Logic
            if (scope == "selected" && !string.IsNullOrEmpty(selectedIds))
            {
                var idList = selectedIds.Split(',').Select(int.Parse).ToList();
                // Filter the fetched list in memory
                dataToExport = dataToExport.Where(x => idList.Contains((int)((IDictionary<string, object>)x)["PoId"])).ToList();
            }

            // 4. Generate CSV (Or Excel if you have EPPlus installed)
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Date,Product,Type,Status,Requested Qty,Received Qty");

            foreach (dynamic item in dataToExport)
            {
                var dict = (IDictionary<string, object>)item;
                sb.AppendLine($"{dict["RequestDate"]},{dict["ProductName"].ToString().Replace(",", " ")},{(Convert.ToBoolean(dict["IsBulkOrder"]) ? "Bulk" : "Standard")},{dict["Status"]},{dict["RequestedQty"]},{dict["ReceivedQty"]}");
            }

            string fileName = $"VendorHistory_{id}_{DateTime.Now:yyyyMMdd}.csv";
            return File(System.Text.Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", fileName);
        }
    }
}