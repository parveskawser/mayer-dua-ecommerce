using MDUA.Entities; // ✅ Ensure ExportRequest is visible
using MDUA.Facade.Interface;
using MDUA.Web.UI.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace MDUA.Web.UI.Controllers
{
    public class ReportController : BaseController
    {
        private readonly IDeliveryStatusLogFacade _deliveryStatusLogFacade;

        // ✅ 1. Define missing dependencies
        private readonly IOrderFacade _orderFacade;
        private readonly IExportService _exportService;

        // ✅ 2. Update Constructor to accept them
        public ReportController(
            IDeliveryStatusLogFacade deliveryStatusLogFacade,
            IOrderFacade orderFacade,
            IExportService exportService
        )
        {
            _deliveryStatusLogFacade = deliveryStatusLogFacade;
            _orderFacade = orderFacade;
            _exportService = exportService;
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
                // 4. Fetch Data from Facade
                model.Logs = _deliveryStatusLogFacade.GetLogsForReport(fromDate, toDate, search, type);
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

                fileName = $"Orders_Export_{DateTime.Now:yyyyMMdd_HHmmss}.{fileExtension}";

                return File(fileBytes, contentType, fileName);
            }

            return BadRequest("Unknown Entity");
        }


        [HttpGet]
        [Route("report/order-history-partial")]
        public IActionResult GetOrderHistoryPartial(string orderId)
        {
            // Dates are null (fetch all time), EntityType is "All" (fetch both Order & Delivery logs)
            var logs = _deliveryStatusLogFacade.GetLogsForReport(null, null, orderId, "All");

            return PartialView("_OrderHistoryTable", logs);
        }

    }
}
