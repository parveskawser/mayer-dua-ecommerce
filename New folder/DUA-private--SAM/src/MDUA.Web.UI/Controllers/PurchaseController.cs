using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Facade;
using MDUA.Facade.Interface;
using MDUA.Framework.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static MDUA.Entities.BulkPurchaseOrder;

namespace MDUA.Web.UI.Controllers
{
    public class PurchaseController : BaseController
    {
        private readonly IPurchaseFacade _purchaseFacade;
        private readonly IPaymentMethodFacade _paymentMethodFacade;
        private readonly IBackInStockRequestDataAccess _backInStockRequestDataAccess;
        private readonly INotificationService _notificationService;
        private readonly IProductVariantDataAccess _productVariantDataAccess;
        private readonly IProductDataAccess _productDataAccess;

        public PurchaseController(
            IPurchaseFacade purchaseFacade,
            IPaymentMethodFacade paymentMethodFacade,
            IBackInStockRequestDataAccess backInStockRequestDataAccess,
            INotificationService notificationService,
            IProductVariantDataAccess productVariantDataAccess,
            IProductDataAccess productDataAccess)
        {
            _purchaseFacade = purchaseFacade;
            _paymentMethodFacade = paymentMethodFacade;
            _backInStockRequestDataAccess = backInStockRequestDataAccess;
            _notificationService = notificationService;
            _productVariantDataAccess = productVariantDataAccess;
            _productDataAccess = productDataAccess;
        }
        [Route("purchase/stock-status")]

        [HttpGet]
        public IActionResult StockStatus(int? highlightId)
        {
            try
            {
                int companyId = CurrentCompanyId; // Ensure BaseController provides this
                                                  // ✅ Get ALL items (sorted by low stock first)
                var inventory = _purchaseFacade.GetInventoryStatus(companyId);
                var vendors = _purchaseFacade.GetAllVendors(companyId);
                ViewBag.Vendors = vendors;
                ViewBag.HighlightVariantId = highlightId;
                return View("LowStockReport", inventory);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("LowStockReport", new List<dynamic>());
            }
        }

        [HttpGet]
        [Route("purchase/back-in-stock-requests")]
        public IActionResult BackInStockRequests()
        {
            try
            {
                int companyId = CurrentCompanyId;
                var requests = _backInStockRequestDataAccess.GetPendingRequestDetails(companyId);
                return View("BackInStockRequests", requests);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("BackInStockRequests", new List<BackInStockRequestDetail>());
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
        public async Task<IActionResult> ReceiveStock([FromBody] JsonElement model)
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

                // 3. Extract Payment Fields
                decimal paidAmount = model.TryGetProperty("PaidAmount", out var pa) ? pa.GetDecimal() : 0;
                int? paymentMethodId = null;
                if (model.TryGetProperty("PaymentMethodId", out var pm) && pm.ValueKind != JsonValueKind.Null)
                {
                    paymentMethodId = pm.GetInt32();
                }
                string paymentRef = model.TryGetProperty("PaymentReference", out var pr) ? pr.GetString() : null;

                // ==========================================================
                // 🛡️ GUARDRAIL: PREVENT ACCIDENTAL INVOICE REUSE
                // ==========================================================
                if (!string.IsNullOrWhiteSpace(invoice))
                {
                    // Note: This logic assumes you have implemented GetPendingRequestInfo in Facade
                    // as discussed previously. If not, this block will need Facade updates.

                    // A. Get Info about the Item we are receiving (to find its Vendor & BulkID)
                    var pendingInfo = _purchaseFacade.GetPendingRequestInfo(variantId); // Returns dynamic/dict

                    if (pendingInfo != null)
                    {
                        var infoDict = (IDictionary<string, object>)pendingInfo;
                        int vendorId = Convert.ToInt32(infoDict["VendorId"]);
                        int? currentItemBulkId = infoDict["BulkPurchaseOrderId"] as int?;

                        // B. Check if this Invoice Number is already used by this Vendor
                        int? existingInvoiceBulkId = _purchaseFacade.GetExistingInvoiceBulkId(invoice, vendorId);

                        if (existingInvoiceBulkId.HasValue) // Invoice ALREADY EXISTS
                        {
                            // Rule 1: Cannot reuse invoice on a Standard Order (Standard = 0 or Null)
                            if (currentItemBulkId == null || currentItemBulkId == 0)
                            {
                                return Json(new { success = false, message = $"Error: Invoice '{invoice}' has already been used. Standard Orders must have unique invoices." });
                            }

                            // Rule 2: Cannot reuse invoice from a DIFFERENT Bulk Order
                            if (existingInvoiceBulkId != currentItemBulkId)
                            {
                                return Json(new { success = false, message = $"Error: Invoice '{invoice}' is already linked to a DIFFERENT Bulk Order. Accidental reuse prevented." });
                            }
                        }
                    }
                }
                // ==========================================================

                // 4. Call Facade
                _purchaseFacade.ReceiveStock(variantId, qty, price, invoice, remarks, paidAmount, paymentMethodId, paymentRef);

                await NotifyBackInStockRequestsAsync(variantId);

                return Json(new { success = true, message = "Stock Received Successfully!" });
            }
            catch (WorkflowException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server Error: " + ex.Message });
            }
        }

        private async Task NotifyBackInStockRequestsAsync(int variantId)
        {
            var pendingRequests = _backInStockRequestDataAccess.GetPendingByProductVariantId(variantId);
            if (pendingRequests == null || pendingRequests.Count == 0)
            {
                return;
            }

            var variant = _productVariantDataAccess.Get(variantId);
            string itemName = variant?.VariantName ?? "Your item";

            string productUrl = null;
            if (variant != null)
            {
                var product = _productDataAccess.Get(variant.ProductId);
                if (product != null && !string.IsNullOrWhiteSpace(product.Slug))
                {
                    productUrl = Url.Action("Index", "Product", new { slug = product.Slug }, Request.Scheme);
                    if (string.IsNullOrWhiteSpace(itemName))
                    {
                        itemName = product.ProductName;
                    }
                }
            }

            string message = string.IsNullOrWhiteSpace(productUrl)
                ? $"Good news! {itemName} is back in stock. Visit our store to order now."
                : $"Good news! {itemName} is back in stock. Order now: {productUrl}";

            var notifiedIds = new List<int>();

            foreach (var group in pendingRequests
                         .Where(r => !string.IsNullOrWhiteSpace(r.ContactNumber))
                         .GroupBy(r => r.ContactNumber.Trim()))
            {
                bool sent = await _notificationService.SendSmsOnlyAsync(group.Key, message);
                if (sent)
                {
                    notifiedIds.AddRange(group.Select(r => r.Id));
                }
            }

            if (notifiedIds.Count > 0)
            {
                _backInStockRequestDataAccess.MarkNotified(notifiedIds, User?.Identity?.Name ?? "System");
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
            int companyId = CurrentCompanyId;
            // 1. Get raw inventory data
            var rawInventory = _purchaseFacade.GetInventorySortedByStockAsc(companyId);
            // 2. Group by Product for the UI
            var groupedInventory = rawInventory
             .GroupBy(x => (string)x.ProductName)
             .OrderBy(g => g.Min(v => (int)v.CurrentStock))
             .Select(g => new
             {
                 ProductName = g.Key,
                 IsCritical = g.Any(v => (bool)v.IsLowStock),
                 UiId = "prod-" + Math.Abs(g.Key.GetHashCode()),
                 Variants = g.OrderBy(v => (int)v.CurrentStock).ToList()
             })
             .ToList();

            ViewBag.Vendors = _purchaseFacade.GetAllVendors(companyId);
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
            int companyId = CurrentCompanyId;
            var list = _purchaseFacade.GetBulkOrdersReceivedList(companyId);
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
        #region Bulk Order Actions 

        [HttpPost]
        [Route("purchase/receive-bulk-stock")]
        public IActionResult ReceiveBulkStock([FromBody] JsonElement model)
        {
            try
            {
                // 1. Extract Header Fields
                string invoice = model.GetProperty("InvoiceNo").GetString();
                string remarks = model.TryGetProperty("Remarks", out var rem) ? rem.GetString() : "";
                decimal totalPaid = model.TryGetProperty("TotalPaid", out var tp) ? tp.GetDecimal() : 0;
                int vendorId = model.GetProperty("VendorId").GetInt32();

                int? paymentMethodId = null;
                if (model.TryGetProperty("PaymentMethodId", out var pm) && pm.ValueKind != JsonValueKind.Null)
                {
                    int pmVal = pm.GetInt32();
                    if (pmVal > 0) paymentMethodId = pmVal;
                }

                // ==========================================================
                // 🛡️ GUARDRAIL: BULK INVOICE CHECK
                // ==========================================================
                if (!string.IsNullOrWhiteSpace(invoice))
                {
                    // Check if this invoice is already used by a DIFFERENT Bulk Order
                    // (We assume the items being received belong to the SAME Bulk Order, 
                    //  so we ideally check against the first item's BulkID, or just warn if it exists generally)

                    int? existingInvoiceBulkId = _purchaseFacade.GetExistingInvoiceBulkId(invoice, vendorId);

                    // If invoice exists and we are starting a "New Batch" (Receive All), 
                    // usually we want a UNIQUE invoice.
                    // However, the user might be doing "Receive All" for the *remaining* 10 items of an existing invoice.
                    // So we only Block if it belongs to a COMPLETELY DIFFERENT Agreement.

                    // (Simplified check: If you want strict uniqueness for "Receive All", uncomment below)
                    /*
                    if (existingInvoiceBulkId.HasValue)
                    {
                         // Check if it matches the current Bulk Order (requires passing BulkOrderId in JSON)
                         // If payload has BulkOrderId:
                         // if (existingInvoiceBulkId != payload.BulkOrderId) return Error...
                    }
                    */
                }
                // ==========================================================

                // 2. Extract Items List
                var itemsList = new List<dynamic>();
                if (model.TryGetProperty("Items", out var itemsElement) && itemsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in itemsElement.EnumerateArray())
                    {
                        itemsList.Add(new
                        {
                            PoRequestId = item.GetProperty("PoRequestId").GetInt32(),
                            ProductVariantId = item.GetProperty("ProductVariantId").GetInt32(),
                            Quantity = item.GetProperty("Quantity").GetInt32(),
                            Price = item.GetProperty("Price").GetDecimal()
                        });
                    }
                }

                if (itemsList.Count == 0) return Json(new { success = false, message = "No items selected." });
                if (string.IsNullOrWhiteSpace(invoice)) return Json(new { success = false, message = "Invoice Number is required." });

                // 3. Execute via Facade
                string currentUser = User.Identity?.Name ?? "System";

                _purchaseFacade.ReceiveBulkStock(itemsList, invoice, remarks, totalPaid, paymentMethodId, vendorId, currentUser);

                return Json(new { success = true, message = "Bulk Order items received successfully!" });
            }
            catch (WorkflowException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("purchase/reject-bulk-remaining")]
        public IActionResult RejectBulkRemaining([FromBody] JsonElement model)
        {
            try
            {
                if (model.TryGetProperty("BulkOrderId", out var idProp) && idProp.ValueKind == JsonValueKind.Number)
                {
                    int bulkId = idProp.GetInt32();

                    _purchaseFacade.RejectBulkRemaining(bulkId);

                    return Json(new { success = true, message = "All remaining pending items have been rejected." });
                }
                return Json(new { success = false, message = "Invalid Bulk Order ID." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server Error: " + ex.Message });
            }
        }

        #endregion
    }
}