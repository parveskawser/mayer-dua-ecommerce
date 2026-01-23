using MDUA.Entities; // ✅ Ensure ExportRequest is visible
using MDUA.Facade;
using MDUA.Facade.Interface;
using MDUA.Web.UI.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace MDUA.Web.UI.Controllers
{
    public class ReportController : BaseController
    {
        private readonly IDeliveryStatusLogFacade _deliveryStatusLogFacade;

        // ✅ 1. Define missing dependencies
        private readonly IOrderFacade _orderFacade;
        private readonly IExportService _exportService;
        private readonly IVendorFacade _vendorFacade;

        // ✅ 2. Update Constructor to accept them
        public ReportController(
            IDeliveryStatusLogFacade deliveryStatusLogFacade,
            IOrderFacade orderFacade,
            IExportService exportService, IVendorFacade vendorFacade
        )
        {
            _deliveryStatusLogFacade = deliveryStatusLogFacade;
            _orderFacade = orderFacade;
            _exportService = exportService;
            _vendorFacade = vendorFacade;
        }

        // ============================================================
        // REPORT 1: Delivery & Order Audit Logs
        // ============================================================
        [HttpGet]
        [Route("report/delivery-logs")]
        public IActionResult DeliveryLogs(DateTime? from, DateTime? to, string search, string type = "All")
        {
            // 1. Permission Check
            if (!HasPermission("Report.DeliveryLog")) return HandleAccessDenied();

            // 2. Get Current Company ID
            int companyId = Convert.ToInt32(User.FindFirst("CompanyId")?.Value ?? "1");

            DateTime fromDate = from ?? DateTime.UtcNow.AddDays(-7);
            DateTime toDate = to ?? DateTime.UtcNow;

            // 3. Prepare ViewModel
            var model = new DeliveryLogViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                SearchTerm = search,
                EntityType = type,
                Logs = new List<MDUA.Entities.DeliveryStatusLog>()
            };

            try
            {
                // 4. Fetch Data from Facade (Added companyId parameter)
                model.Logs = _deliveryStatusLogFacade.GetLogsForReport(companyId, fromDate, toDate, search, type);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReportError] Failed to load delivery logs: {ex.Message}");
                ViewData["ErrorMessage"] = "Failed to load report data. Please try again later.";
            }

            return View("DeliveryLogs", model);
        }
        // ============================================================
        // REPORT 2: Sales Summary 
        // ============================================================
        /*
        [HttpGet]
        [Route("report/sales-summary")]
        public IActionResult SalesSummary()
        {
            if (!HasPermission("Report.Sales")) return HandleAccessDenied();
            return View();
        }
        */




        // ... [DeliveryLogs Action remains the same] ...

        [HttpPost]
        [Route("Report/ExportData")]
        public IActionResult ExportData(string jsonPayload)
        {
            var request = Newtonsoft.Json.JsonConvert.DeserializeObject<ExportRequest>(jsonPayload);

            if (request.EntityType == "Order")
            {
                var data = _orderFacade.GetExportData(request);
                byte[] fileBytes = _exportService.GenerateFile(data, request.Format, request.Columns);

                // Set proper MIME types and file extensions
                string contentType;
                string fileExtension;
                string fileName;

                switch (request.Format.ToLower())
                {
                    case "csv":
                        contentType = "text/csv";
                        fileExtension = "csv";
                        break;
                    case "excel":
                    case "xlsx":
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        fileExtension = "xlsx";
                        break;
                    case "pdf":
                        contentType = "application/pdf";
                        fileExtension = "pdf";
                        break;
                    default:
                        contentType = "application/octet-stream";
                        fileExtension = request.Format;
                        break;
                }

                fileName = $"Orders_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{fileExtension}";

                return File(fileBytes, contentType, fileName);
            }

            return BadRequest("Unknown Entity");
        }


        [HttpGet]
        [Route("report/order-history-partial")]
        public IActionResult GetOrderHistoryPartial(string orderId)
        {
            int companyId = Convert.ToInt32(User.FindFirst("CompanyId")?.Value ?? "1");
            // Dates are null (fetch all time), EntityType is "All" (fetch both Order & Delivery logs)
            var logs = _deliveryStatusLogFacade.GetLogsForReport(companyId, null, null, orderId, "All");

            return PartialView("_OrderHistoryTable", logs);
        }



        [HttpPost]

        [Route("Report/ExportVendorHistory")]

        public IActionResult ExportVendorHistory(string jsonPayload)

        {
            int companyId = Convert.ToInt32(User.FindFirst("CompanyId")?.Value ?? "1");

            // 1. Parse Payload

            dynamic request = JObject.Parse(jsonPayload);

            // Extract Standard Fields

            string format = (string)request.format ?? "csv";

            var columns = ((JArray)request.columns).ToObject<List<string>>();

            string scope = (string)request.scope;

            // Extract Filter Fields

            int vendorId = (int)request.vendorId;

            string search = (string)request.search ?? "";

            string status = (string)request.status ?? "all";

            string type = (string)request.type ?? "all";

            string dateRange = (string)request.dateRange ?? "all";

            DateTime? fromDate = (DateTime?)request.fromDate;

            DateTime? toDate = (DateTime?)request.toDate;

            // 2. Process Date Logic

            DateTime today = DateTime.UtcNow.Date;

            if (dateRange != "all")

            {

                switch (dateRange)

                {

                    case "today":

                        fromDate = today;

                        toDate = today.AddDays(1).AddTicks(-1);

                        break;

                    case "yesterday":

                        fromDate = today.AddDays(-1);

                        toDate = today.AddDays(1).AddTicks(-1);

                        break;

                    case "thisWeek":

                        int diff = (7 + (today.DayOfWeek - DayOfWeek.Sunday)) % 7;

                        fromDate = today.AddDays(-1 * diff).Date;

                        toDate = today.AddDays(1).AddTicks(-1);

                        break;

                    case "lastWeek":

                        int diffLast = (7 + (today.DayOfWeek - DayOfWeek.Sunday)) % 7;

                        fromDate = today.AddDays(-1 * diffLast).AddDays(-7).Date;

                        toDate = fromDate.Value.AddDays(7).AddTicks(-1);

                        break;

                    case "thisMonth":

                        fromDate = new DateTime(today.Year, today.Month, 1);

                        toDate = today.AddDays(1).AddTicks(-1);

                        break;

                    case "lastMonth":

                        var lastMonth = today.AddMonths(-1);

                        fromDate = new DateTime(lastMonth.Year, lastMonth.Month, 1);

                        toDate = new DateTime(today.Year, today.Month, 1).AddTicks(-1);

                        break;

                }

            }

            // 3. Fetch Data from Facade

            // Added '1' as the page number before '100000'
            var result = _vendorFacade.GetVendorOrderHistory(vendorId, companyId, 1, 100000, search, status, type, fromDate, toDate);
            var dataList = result.Items; // This is List<dynamic>

            // 4. Apply "Selected Rows" Scope

            if (scope == "selected" && request.selectedIds != null)
            {
                var selectedIds = ((JArray)request.selectedIds).ToObject<List<int>>();

                if (selectedIds.Any())
                {
                    dataList = dataList.Where(x =>
                    {
                        var dict = (IDictionary<string, object>)x;

                        // Prefer Id, fallback to PoId
                        if (dict.ContainsKey("Id") && int.TryParse(dict["Id"]?.ToString(), out int id))
                            return selectedIds.Contains(id);

                        if (dict.ContainsKey("PoId") && int.TryParse(dict["PoId"]?.ToString(), out int poId))
                            return selectedIds.Contains(poId);

                        return false;
                    }).ToList();
                }
            }


            // ✅ FIX: Convert List<dynamic> to List<Dictionary<string, object>>

            // IExportService requires strict Dictionary types, but dynamic objects (ExpandoObject) 

            // need explicit casting/conversion to match the signature.

            // Convert List<dynamic> to List<Dictionary<string, object>>
            var exportParents = dataList
                .Select(item => new Dictionary<string, object>((IDictionary<string, object>)item))
                .ToList();

            // ✅ NEW: flatten children into extra export rows
            var exportData = FlattenVendorHistoryForExport(exportParents, columns);

            // Generate file
            byte[] fileBytes = _exportService.GenerateFile(exportData, format, columns);


            // 6. Return File

            string contentType = "application/octet-stream";

            string fileExtension = format;

            switch (format.ToLower())

            {

                case "csv": contentType = "text/csv"; fileExtension = "csv"; break;

                case "excel":

                case "xlsx": contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; fileExtension = "xlsx"; break;

                case "pdf": contentType = "application/pdf"; fileExtension = "pdf"; break;

            }

            string fileName = $"VendorHistory_{vendorId}_{DateTime.Now:yyyyMMdd_HHmmss}.{fileExtension}";

            return File(fileBytes, contentType, fileName);

        }


        private List<Dictionary<string, object>> FlattenVendorHistoryForExport(
      List<Dictionary<string, object>> parents,
      List<string> columns
  )
        {
            var flattened = new List<Dictionary<string, object>>();

            foreach (var parent in parents)
            {
                // 1. Add Parent Row
                flattened.Add(parent);

                // 2. Check for Children
                if (!parent.TryGetValue("Children", out var childrenObj) || childrenObj == null)
                    continue;

                var childrenJson = childrenObj.ToString();
                if (string.IsNullOrWhiteSpace(childrenJson))
                    continue;

                try
                {
                    // Deserialize Children
                    var children = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(childrenJson);

                    if (children != null && children.Count > 0)
                    {
                        foreach (var child in children)
                        {
                            // 3. Create Child Row & MANUALLY MAP KEYS
                            // This fixes the "Data Incomplete" issue because SQL keys (ReqQty) differ from View keys (RequestedQty)
                            var childRow = new Dictionary<string, object>();

                            // --- A. DATE & TITLES ---
                            if (child.TryGetValue("RequestDate", out var rDate))
                                childRow["RequestDate"] = Convert.ToDateTime(rDate).ToString("dd MMM yyyy");
                            else
                                childRow["RequestDate"] = "";

                            // ✅ PDF SAFE INDENTATION
                            // We use ">>" instead of "↳" to prevent font issues in PDF files
                            if (child.TryGetValue("ProductName", out var pName))
                                childRow["ProductName"] = "    >> " + pName;
                            else
                                childRow["ProductName"] = "";

                            childRow["AgreementNumber"] = ""; // Empty for child
                            childRow["Type"] = "Order";       // Hardcode type
                            childRow["Status"] = child.ContainsKey("Status") ? child["Status"] : "";

                            // --- B. QUANTITIES (Mapping SQL Alias to View Column) ---
                            childRow["RequestedQty"] = child.ContainsKey("ReqQty") ? child["ReqQty"] : 0;
                            childRow["ReceivedQty"] = child.ContainsKey("RecQty") ? child["RecQty"] : 0;

                            // --- C. FINANCIALS ---
                            decimal total = 0;
                            decimal paid = 0;

                            if (child.ContainsKey("TotalAmt") && child["TotalAmt"] != null)
                                total = Convert.ToDecimal(child["TotalAmt"]);

                            childRow["TotalAmount"] = total;

                            if (child.ContainsKey("PaidAmt") && child["PaidAmt"] != null)
                                paid = Convert.ToDecimal(child["PaidAmt"]);

                            childRow["PaidAmount"] = paid;

                            childRow["DueAmount"] = total - paid;

                            // --- D. FILL MISSING COLUMNS ---
                            // Ensures table alignment in PDF/Excel
                            foreach (var col in columns)
                            {
                                if (!childRow.ContainsKey(col))
                                {
                                    childRow[col] = "";
                                }
                            }

                            flattened.Add(childRow);
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            return flattened;
        }
    }
}
