using MDUA.Entities;
using MDUA.Facade;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json;
using static MDUA.Entities.BulkPurchaseOrder;

namespace MDUA.Web.UI.Controllers
{
    public class PurchaseController : BaseController
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

                // ✅ Set the CreatedBy to current logged-in user
                model.CreatedBy = CurrentUserName;

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

        #region bulk order
      
        [HttpGet]
        public IActionResult BulkOrder()
        {
            // 1. Get raw inventory data
            var rawInventory = _purchaseFacade.GetInventorySortedByStockAsc();

            // 2. Group by Product for the UI
            var groupedInventory = rawInventory
                .GroupBy(x => (string)x.ProductName)
                .OrderBy(g => g.Min(v => (int)v.CurrentStock)) // Show most critical products first
                .Select(g => new
                {
                    ProductName = g.Key,
                    IsCritical = g.Any(v => (bool)v.IsLowStock),
                    // Use a safe unique ID for the UI accordion (HashCode can sometimes be negative or unreliable)
                    UiId = "prod-" + Math.Abs(g.Key.GetHashCode()),
                    Variants = g.OrderBy(v => (int)v.CurrentStock).ToList()
                })
                .ToList();

            ViewBag.Vendors = _purchaseFacade.GetAllVendors();
            return View(groupedInventory);
        }

        [HttpPost]
        public IActionResult CreateBulkOrder(BulkPurchaseOrder bulkOrder, List<int> selectedVariants, Dictionary<int, int> quantities)
        {
            // === DEBUGGER START ===
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("DEBUG: CreateBulkOrder Hit");

            // 1. Check Raw Form Data (In case Model Binding fails)
            try
            {
                Console.WriteLine($"DEBUG: Raw Form Key Count: {Request.Form.Keys.Count}");
                if (Request.Form.ContainsKey("selectedVariants"))
                {
                    Console.WriteLine($"DEBUG: Raw 'selectedVariants' count: {Request.Form["selectedVariants"].Count}");
                }
                else
                {
                    Console.WriteLine("DEBUG: CRITICAL - 'selectedVariants' missing from Request.Form");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DEBUG: Error reading Request.Form: " + ex.Message);
            }

            // 2. Check Model Bound Data
            if (selectedVariants == null)
            {
                Console.WriteLine("DEBUG: selectedVariants is NULL");
                selectedVariants = new List<int>(); // Prevent crash
            }
            else
            {
                Console.WriteLine($"DEBUG: selectedVariants Count: {selectedVariants.Count}");
                foreach (var id in selectedVariants) Console.WriteLine($"   -> Variant ID: {id}");
            }

            if (quantities == null) Console.WriteLine("DEBUG: quantities is NULL");
            else Console.WriteLine($"DEBUG: quantities Count: {quantities.Count}");

            // === DEBUGGER END ===

            try
            {
                // 1. Validation
                if (selectedVariants == null || !selectedVariants.Any())
                {
                    // Log the error specifically
                    Console.WriteLine("ERROR: Validation Failed - No items selected.");
                    TempData["Error"] = "Debug: Server received 0 items. Please check VS Output window.";
                    return RedirectToAction("BulkOrder");
                }

                var poList = new List<PoRequested>();

                foreach (var variantId in selectedVariants)
                {
                    // Default to 0 if key missing
                    int qty = quantities.ContainsKey(variantId) ? quantities[variantId] : 0;

                    // Allow qty > 0 check
                    if (qty > 0)
                    {
                        poList.Add(new PoRequested
                        {
                            ProductVariantId = variantId,
                            Quantity = qty
                        });
                    }
                    else
                    {
                        Console.WriteLine($"DEBUG: Skipped Variant {variantId} because Quantity was {qty}");
                    }
                }

                if (!poList.Any())
                {
                    Console.WriteLine("ERROR: No valid items after Quantity check.");
                    TempData["Error"] = "Selected items must have a quantity greater than 0.";
                    return RedirectToAction("BulkOrder");
                }

                bulkOrder.CreatedBy = User.Identity?.Name ?? "Admin";

                _purchaseFacade.CreateBulkOrder(bulkOrder, poList);

                TempData["Success"] = "Bulk Order created successfully!";
                return RedirectToAction("BulkOrderReceivedList");
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.ToString()); // Log full stack trace
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("BulkOrder");
            }
        }

         [HttpGet]
        public IActionResult BulkOrderReceivedList()
        {
            var list = _purchaseFacade.GetBulkOrdersReceivedList();
            return View(list);
        }

        [HttpGet]
        public IActionResult GetBulkOrderDetails(int id)
        {
            try
            {
                // Add logging to track execution
                System.Diagnostics.Debug.WriteLine($"[GetBulkOrderDetails] Request for Order ID: {id}");

                if (id <= 0)
                {
                    return PartialView("_BulkOrderDetailsPartial", new List<BulkOrderItemViewModel>());
                }

                // Get items from facade with timeout consideration
                var orderItems = _purchaseFacade.GetBulkOrderItems(id);

                System.Diagnostics.Debug.WriteLine($"[GetBulkOrderDetails] Found {orderItems?.Count() ?? 0} items");

                // Handle null or empty results gracefully
                if (orderItems == null)
                {
                    orderItems = new List<BulkOrderItemViewModel>();
                }

                return PartialView("_BulkOrderDetailsPartial", orderItems);
            }
            catch (TimeoutException tex)
            {
                System.Diagnostics.Debug.WriteLine($"[GetBulkOrderDetails] Timeout: {tex.Message}");
                return Content($@"
            <div class='alert alert-warning m-3'>
                <strong>Timeout Error:</strong> The database query took too long. 
                Please try again or contact support if this persists.
            </div>");
            }
            catch (Exception ex)
            {
                // Log the full exception for debugging
                System.Diagnostics.Debug.WriteLine($"[GetBulkOrderDetails] Error: {ex}");

                return Content($@"
            <div class='alert alert-danger m-3'>
                <strong>Error:</strong> {System.Web.HttpUtility.HtmlEncode(ex.Message)}
                <hr>
                <small class='text-muted'>Order ID: {id}</small>
            </div>");
            }
        }

        [HttpPost]
        [Route("purchase/reject-item")]
        public IActionResult RejectItem([FromBody] JsonElement model)
        {
            try
            {
                // Safe extraction
                if (model.TryGetProperty("PoRequestId", out var idProp))
                {
                    int poId = idProp.GetInt32();
                    _purchaseFacade.RejectPurchaseOrder(poId);
                    return Json(new { success = true, message = "Item rejected successfully." });
                }
                return Json(new { success = false, message = "Invalid ID." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }


        #endregion 

    }
}