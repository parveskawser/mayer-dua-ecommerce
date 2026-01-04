using Microsoft.AspNetCore.Mvc;
using MDUA.Facade.Interface;
using MDUA.Entities;
using System;

namespace MDUA.Web.Controllers
{
    public class VendorController : Controller
    {
        private readonly IVendorFacade _vendorFacade;

        public VendorController(IVendorFacade vendorFacade)
        {
            _vendorFacade = vendorFacade;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // For simplicity, fetching all. Implement PagedRequest here if list is large.
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
                // Basic Server Validation
                if (string.IsNullOrEmpty(vendor.VendorName))
                {
                    TempData["Error"] = "Vendor Name is required.";
                    return View("Add", vendor);
                }

                if (vendor.Id > 0)
                {
                    // Update
                    vendor.UpdatedBy = User.Identity.Name ?? "Admin"; // Adjust based on your auth
                    _vendorFacade.Update(vendor);
                    TempData["Success"] = "Vendor updated successfully.";
                }
                else
                {
                    // Insert
                    vendor.CreatedBy = User.Identity.Name ?? "Admin";
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