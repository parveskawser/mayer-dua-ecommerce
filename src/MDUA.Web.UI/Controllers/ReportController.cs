using MDUA.Entities;
using MDUA.Facade.Interface; 
using MDUA.Web.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using static MDUA.Entities.DeliveryStatusLog;

namespace MDUA.Web.UI.Controllers
{
    public class ReportController : BaseController
    {
        private readonly IDeliveryStatusLogFacade _deliveryStatusLogFacade;

        public ReportController(IDeliveryStatusLogFacade deliveryStatusLogFacade)
        {
            _deliveryStatusLogFacade = deliveryStatusLogFacade;
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
    }
}