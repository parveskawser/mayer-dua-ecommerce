using MDUA.Entities;
using MDUA.Facade;
using MDUA.Facade.Interface;
using MDUA.Framework.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.Json;
using static MDUA.Entities.BulkPurchaseOrder;

namespace MDUA.Web.UI.Controllers
{
    public class PurchaseController : BaseController
    {
        private readonly IPurchaseFacade _purchaseFacade;
        private readonly IPaymentMethodFacade _paymentMethodFacade;

        public PurchaseController(IPurchaseFacade purchaseFacade, IPaymentMethodFacade paymentMethodFacade)
        {
            _purchaseFacade = purchaseFacade;
            _paymentMethodFacade = paymentMethodFacade;
        }
        [Route("purchase/stock-status")]

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
                if (model == null || model.VendorId <= 0 || model.Quantity <= 0)
                {
                    return Json(new { success = false, message = "Invalid Data. Check Vendor and Quantity." });
                }

                model.CreatedBy = User.Identity?.Name ?? "System";

                // 2. Execute
                long id = _purchaseFacade.CreatePurchaseOrder(model);

                return Json(new { success = true, message = "PO Requested Successfully!", id = id });
            }
            //  Catch Business Logic Errors (Like Bulk Limit Exceeded)
            catch (WorkflowException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (SqlException ex)
            {
                // If the SQL "THROW 51000" triggers, it comes here
                if (ex.Number == 51000)
                {
                    return Json(new { success = false, message = ex.Message });
                }
                return Json(new { success = false, message = "Database Error: " + ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server Error: " + ex.Message });
            }
        }

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

        [HttpPost]
        [Route("purchase/receive-stock")]
        public IActionResult ReceiveStock([FromBody] JsonElement model)
        {
            try
            {
                // 1. Standard Fields Extraction
                int variantId = model.GetProperty("ProductVariantId").GetInt32();
                int qty = model.GetProperty("Quantity").GetInt32();
                decimal price = model.GetProperty("BuyingPrice").GetDecimal();

                // 2. Safe String Extraction
                string invoice = model.TryGetProperty("InvoiceNo", out var inv) ? inv.GetString() : "";
                string remarks = model.TryGetProperty("Remarks", out var rem) ? rem.GetString() : "";

                // 3. Extract Payment Fields (Safe Null Handling)
                decimal paidAmount = model.TryGetProperty("PaidAmount", out var pa) ? pa.GetDecimal() : 0;

                int? paymentMethodId = null;
                if (model.TryGetProperty("PaymentMethodId", out var pm) && pm.ValueKind != JsonValueKind.Null)
                {
                    paymentMethodId = pm.GetInt32();
                }

                string paymentRef = model.TryGetProperty("PaymentReference", out var pr) ? pr.GetString() : null;

                // 4. Call Facade
                _purchaseFacade.ReceiveStock(variantId, qty, price, invoice, remarks, paidAmount, paymentMethodId, paymentRef);

                return Json(new { success = true, message = "Stock Received Successfully!" });
            }
            //  Catch specific Business Logic errors (Like Duplicate Invoice)
            catch (WorkflowException ex)
            {
                // Returns clean message: "Error: The Invoice Number 'ABC' has already been used..."
                return Json(new { success = false, message = ex.Message });
            }
            //  Catch unexpected errors
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
        [Route("purchase/bulk-order")]

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
            // === DEBUGGER START (Preserved) ===
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("DEBUG: CreateBulkOrder Hit");

            // 1. Check Raw Form Data
            try
            {
                Console.WriteLine($"DEBUG: Raw Form Key Count: {Request.Form.Keys.Count}");
                if (Request.Form.ContainsKey("selectedVariants"))
                    Console.WriteLine($"DEBUG: Raw 'selectedVariants' count: {Request.Form["selectedVariants"].Count}");
                else
                    Console.WriteLine("DEBUG: CRITICAL - 'selectedVariants' missing from Request.Form");
            }
            catch (Exception ex)
            {
                Console.WriteLine("DEBUG: Error reading Request.Form: " + ex.Message);
            }

            // 2. Check Model Bound Data
            if (selectedVariants == null)
            {
                Console.WriteLine("DEBUG: selectedVariants is NULL");
                selectedVariants = new List<int>();
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
                    Console.WriteLine("ERROR: Validation Failed - No items selected.");
                    TempData["Error"] = "Please select at least one product.";
                    return RedirectToAction("BulkOrder");
                }

                var poList = new List<PoRequested>();

                foreach (var variantId in selectedVariants)
                {
                    int qty = quantities.ContainsKey(variantId) ? quantities[variantId] : 0;

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

                // 2. Execute
                bulkOrder.CreatedBy = User.Identity?.Name ?? "Admin";

                // This calls the Facade -> DataAccess -> SQL Stored Procedure
                // If SQL logic fails (e.g., limit exceeded), it throws an exception here.
                _purchaseFacade.CreateBulkOrder(bulkOrder, poList);

                TempData["Success"] = "Bulk Order created successfully!";
                return RedirectToAction("BulkOrderReceivedList");
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.ToString());

                // ✅ REAL-WORLD FIX: Capture specific Business Logic Errors from SQL
                string msg = ex.Message;

                // Check if the error is related to the Bulk Limit Guardrail we added in SQL
                if (msg.Contains("Exceeds Bulk Limit") || (ex.InnerException != null && ex.InnerException.Message.Contains("Exceeds Bulk Limit")))
                {
                    msg = "Validation Error: The items selected exceed the allowed Bulk Order limit.";
                }
                // Check for Expiry
                else if (msg.Contains("expired") || (ex.InnerException != null && ex.InnerException.Message.Contains("expired")))
                {
                    msg = "Validation Error: This Bulk Contract has expired.";
                }

                TempData["Error"] = "Error: " + msg;
                return RedirectToAction("BulkOrder");
            }
        }
        [Route("purchase/bulk-order-received")]

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
                if (id <= 0) return PartialView("_BulkOrderDetailsPartial", new List<BulkOrderItemViewModel>());

                var orderItems = _purchaseFacade.GetBulkOrderItems(id) ?? new List<BulkOrderItemViewModel>();

                // 2. Populate Payment Methods for the Dropdown inside the Modal
                ViewBag.PaymentMethods = _paymentMethodFacade.GetAll();

                return PartialView("_BulkOrderDetailsPartial", orderItems);
            }
            catch (Exception ex)
            {
                return Content($"<div class='alert alert-danger'>Error: {ex.Message}</div>");
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