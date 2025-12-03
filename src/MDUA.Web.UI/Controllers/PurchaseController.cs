using MDUA.Entities;
using MDUA.Facade;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace MDUA.Web.UI.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly IPurchaseFacade _purchaseFacade;

        public PurchaseController(IPurchaseFacade purchaseFacade)
        {
            _purchaseFacade = purchaseFacade;
        }

        [HttpGet]
        public IActionResult StockStatus()
        {
            try
            {
                // ✅ Get ALL items (sorted by low stock first)
                var inventory = _purchaseFacade.GetInventoryStatus();
                var vendors = _purchaseFacade.GetAllVendors();

                ViewBag.Vendors = vendors;
                return View("LowStockReport", inventory); // Reusing the view file but with new data
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("LowStockReport", new List<dynamic>());
            }
        }

        [HttpPost]
        [Route("purchase/create-request")]
        public IActionResult CreateRequest([FromBody] PoRequested model)
        {
            try
            {
                // 1. Validate Payload
                if (model == null)
                {
                    return Json(new { success = false, message = "Invalid Data: Request payload is null. Check JSON format." });
                }

                if (model.VendorId <= 0 || model.Quantity <= 0)
                {
                    return Json(new { success = false, message = "Invalid Vendor or Quantity selected." });
                }

                // 2. Execute
                long id = _purchaseFacade.CreatePurchaseOrder(model);

                // 3. Validate Result
                if (id <= 0)
                {
                    return Json(new { success = false, message = "Database Insert Failed (ID <= 0). Check Stored Procedure logic." });
                }

                return Json(new { success = true, message = "PO Requested Successfully!", id = id });
            }
            catch (Exception ex)
            {
                // ✅ FIX: Capture the INNER exception which usually holds the SQL error
                string errorMessage = ex.Message;

                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                {
                    errorMessage += " | Inner: " + ex.InnerException.Message;
                }

                // If still empty, dump the whole stack trace so we can debug
                if (string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = ex.ToString();
                }

                return Json(new { success = false, message = errorMessage });
            }
        }

        // ✅ MISSING ENDPOINT #1: Get Info for Modal
        [HttpGet]
        [Route("purchase/get-pending-info")]
        public IActionResult GetPendingInfo(int variantId)
        {
            try
            {
                var info = _purchaseFacade.GetPendingRequestInfo(variantId);
                if (info != null) return Json(new { success = true, data = info });
                return Json(new { success = false, message = "No pending request found." });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        // ✅ MISSING ENDPOINT #2: Receive Stock
        [HttpPost]
        [Route("purchase/receive-stock")]
        public IActionResult ReceiveStock([FromBody] JsonElement model)
        {
            try
            {
                // Safe extraction from JSON
                int variantId = model.GetProperty("ProductVariantId").GetInt32();
                int qty = model.GetProperty("Quantity").GetInt32();
                decimal price = model.GetProperty("BuyingPrice").GetDecimal();

                string invoice = model.TryGetProperty("InvoiceNo", out var inv) ? inv.GetString() : "";
                string remarks = model.TryGetProperty("Remarks", out var rem) ? rem.GetString() : "";

                _purchaseFacade.ReceiveStock(variantId, qty, price, invoice, remarks);

                return Json(new { success = true, message = "Stock Received & Updated!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server Error: " + ex.Message });
            }
        }
        [HttpGet]
        [Route("purchase/get-variant-row")]
        public IActionResult GetVariantRow(int variantId)
        {
            var variantData = _purchaseFacade.GetVariantStatus(variantId);
            if (variantData == null) return NotFound();
            return PartialView("_InventoryRow", variantData);
        }

        [HttpGet]
        [HttpGet]
        [Route("purchase/bulk-order")]
        public IActionResult BulkOrder()
        {
            try
            {
                // 1. Fetch rich inventory data (Already sorted by Low Stock in the SQL Query)
                var inventory = _purchaseFacade.GetInventoryStatus();

                // 2. Get Vendors
                var vendors = _purchaseFacade.GetAllVendors();

                ViewBag.VendorList = vendors;
        
                // Pass the inventory list to the view
                return View(inventory); 
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View(new List<dynamic>());
            }
        }
        [HttpPost]
        [Route("purchase/bulk-create")]
        public IActionResult BulkCreateRequest([FromBody] List<PoRequested> model)
        {
            try
            {
                if (model == null || model.Count == 0)
                {
                    return Json(new { success = false, message = "No items to order." });
                }

                _purchaseFacade.CreateBulkPurchaseOrder(model);

                return Json(new { success = true, message = "Bulk Order Processed Successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

    }
}