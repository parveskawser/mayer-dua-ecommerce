using System;
using System.Collections.Generic;
using System.Linq; // ✅ Fix for .Select(), .Where(), .Any()
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq; // ✅ Fix for JObject, JArray
using MDUA.Entities; 
using MDUA.Facade.Interface;
using MDUA.Web.UI.Services.Interface;

namespace MDUA.Web.UI.Controllers
{
    public class ReportController : BaseController
    {
        private readonly IDeliveryStatusLogFacade _deliveryStatusLogFacade;
        private readonly IOrderFacade _orderFacade;
        private readonly IExportService _exportService;
        private readonly IVendorFacade _vendorFacade;

        public ReportController(
            IDeliveryStatusLogFacade deliveryStatusLogFacade,
            IOrderFacade orderFacade,
            IExportService exportService,
            IVendorFacade vendorFacade
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
            if (!HasPermission("Report.DeliveryLog")) return HandleAccessDenied();

            DateTime fromDate = from ?? DateTime.UtcNow.AddDays(-7);
            DateTime toDate = to ?? DateTime.UtcNow;

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
                model.Logs = _deliveryStatusLogFacade.GetLogsForReport(fromDate, toDate, search, type);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReportError] Failed to load delivery logs: {ex.Message}");
                ViewData["ErrorMessage"] = "Failed to load report data. Please try again later.";
            }

            return View("DeliveryLogs", model);
        }

        [HttpPost]
        [Route("Report/ExportData")]
        public IActionResult ExportData(string jsonPayload)
        {
            var request = Newtonsoft.Json.JsonConvert.DeserializeObject<ExportRequest>(jsonPayload);

            if (request.EntityType == "Order")
            {
                var data = _orderFacade.GetExportData(request);
                byte[] fileBytes = _exportService.GenerateFile(data, request.Format, request.Columns);

                string contentType = "application/octet-stream";
                string fileExtension = request.Format;

                switch (request.Format.ToLower())
                {
                    case "csv": contentType = "text/csv"; fileExtension = "csv"; break;
                    case "excel":
                    case "xlsx": contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; fileExtension = "xlsx"; break;
                    case "pdf": contentType = "application/pdf"; fileExtension = "pdf"; break;
                }

                string fileName = $"Orders_Export_{DateTime.Now:yyyyMMdd_HHmmss}.{fileExtension}";
                return File(fileBytes, contentType, fileName);
            }

            return BadRequest("Unknown Entity");
        }

        [HttpGet]
        [Route("report/order-history-partial")]
        public IActionResult GetOrderHistoryPartial(string orderId)
        {
            var logs = _deliveryStatusLogFacade.GetLogsForReport(null, null, orderId, "All");
            return PartialView("_OrderHistoryTable", logs);
        }

        // ============================================================
        // ✅ NEW DEDICATED METHOD FOR VENDOR HISTORY EXPORT
        // ============================================================
        [HttpPost]
        [Route("Report/ExportVendorHistory")]
        public IActionResult ExportVendorHistory(string jsonPayload)
        {
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
            var result = _vendorFacade.GetVendorOrderHistory(vendorId, 1, 100000, search, status, type, fromDate, toDate);
            var dataList = result.Items; // This is List<dynamic>

            // 4. Apply "Selected Rows" Scope
            if (scope == "selected" && request.selectedIds != null)
            {
                var selectedIds = ((JArray)request.selectedIds).ToObject<List<int>>();
                if (selectedIds.Any())
                {
                    dataList = dataList.Where(x => selectedIds.Contains((int)((IDictionary<string, object>)x)["PoId"])).ToList();
                }
            }

            // ✅ FIX: Convert List<dynamic> to List<Dictionary<string, object>>
            // IExportService requires strict Dictionary types, but dynamic objects (ExpandoObject) 
            // need explicit casting/conversion to match the signature.
            var exportData = dataList
                .Select(item => new Dictionary<string, object>((IDictionary<string, object>)item))
                .ToList();

            // 5. Generate File using Export Service
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
    }
}