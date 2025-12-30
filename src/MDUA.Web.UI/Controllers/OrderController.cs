using MDUA.Entities;
using MDUA.Facade;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;
using MDUA.Web.UI.Controllers;


namespace MDUA.Web.UI.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderFacade _orderFacade;
        private readonly IUserLoginFacade _userLoginFacade;
        private readonly IPaymentFacade _paymentFacade; // <--- ADD THIS
        private readonly ISettingsFacade _settingsFacade;
        private readonly IDeliveryStatusLogFacade _logFacade;
        public OrderController(IOrderFacade orderFacade, IUserLoginFacade userLoginFacade, IPaymentFacade paymentFacade, ISettingsFacade settingsFacade, IDeliveryStatusLogFacade logFacade)
        {
            _orderFacade = orderFacade;
            _userLoginFacade = userLoginFacade;
            _paymentFacade = paymentFacade;
            _settingsFacade = settingsFacade;
            _logFacade = logFacade;

        }



        [HttpGet]

        public IActionResult GetOrderStatus([FromQuery] string orderId)

        {

            if (string.IsNullOrEmpty(orderId))

            {

                return BadRequest(new { message = "Order ID is required." });

            }

            try

            {

                // --- 1. Facade Call ---

                var receiptLines = _orderFacade.GetOrderReceiptByOnlineId(orderId);

                if (receiptLines == null || !receiptLines.Any())

                {

                    return NotFound(new { message = $"Order {orderId} not found.", orderFound = false });

                }

                // --- 2. Robust Data Extraction and Casting ---

                var headerData = (IDictionary<string, object>)receiptLines.First();

                // Helper function for safer decimal retrieval (Handles DBNull and casting errors)

                Func<string, decimal> GetDecimal = (key) =>

                    headerData.TryGetValue(key, out var val) && val != null && val != DBNull.Value

                    ? Convert.ToDecimal(val, CultureInfo.InvariantCulture) : 0M;

                // Helper function for safer string retrieval

                Func<string, string> GetString = (key) =>

                    headerData.TryGetValue(key, out var val) && val != null && val != DBNull.Value

                    ? val.ToString() : string.Empty;

                // Helper function for safer DateTime retrieval

                Func<string, DateTime> GetDateTime = (key) =>

                    headerData.TryGetValue(key, out var val) && val is DateTime dt ? dt : DateTime.MinValue;


                // --- Extract Data Using Helpers ---

                string retrievedOrderId = GetString("OnlineOrderId") ?? orderId;

                string orderStatusRaw = GetString("OrderStatus")?.ToLower()?.Trim();

                string orderStatus = orderStatusRaw switch

                {

                    "draft" => "Pending",

                    _ => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(orderStatusRaw ?? "Unknown")

                };

                decimal netAmount = GetDecimal("NetAmount");

                decimal discountAmount = GetDecimal("DiscountAmount");

                string deliveryDivision = GetString("Divison").ToLower();

                DateTime orderDate = GetDateTime("OrderDate");

 

                const decimal DHAKA_CHARGE = 50M;

                const decimal OUTSIDE_CHARGE = 100M;

                const int DHAKA_DAYS = 3;

                const int OUTSIDE_DAYS = 5;

                decimal deliveryCharge = OUTSIDE_CHARGE;

                int deliveryDays = OUTSIDE_DAYS;

                // Determine charge and days based on Division

                if (deliveryDivision.Contains("dhaka"))

                {

                    deliveryCharge = DHAKA_CHARGE;

                    deliveryDays = DHAKA_DAYS;

                }

                string estimatedDelivery = "N/A";

                string formattedOrderDate = "N/A";

                if (orderDate != DateTime.MinValue)

                {

                    // The date is valid, calculate both outputs

                    estimatedDelivery = orderDate.AddDays(deliveryDays).ToString("yyyy-MM-dd");

                    formattedOrderDate = orderDate.ToString("yyyy-MM-dd HH:mm:ss");

                }

                decimal finalGrandTotal = netAmount + deliveryCharge;

                // --- 4. Process Line Items ---

                var lineItems = receiptLines.Select(line =>

                {

                    var lineDict = (IDictionary<string, object>)line;

                    // Helper for decimal within line item

                    Func<string, decimal> GetLineDecimal = (key) =>

                        lineDict.TryGetValue(key, out var val) && val != null && val != DBNull.Value

                        ? Convert.ToDecimal(val, CultureInfo.InvariantCulture) : 0M;

                    // Helper for string within line item

                    Func<string, string> GetLineString = (key) =>

                        lineDict.TryGetValue(key, out var val) && val != null && val != DBNull.Value

                        ? val.ToString() : string.Empty;

                    return new

                    {

                        productName = GetLineString("ProductName"),

                        qty = lineDict.TryGetValue("OrderQuantity", out var qty) && qty != null ? Convert.ToInt32(qty) : 0,

                        price = GetLineDecimal("UnitPrice"),

                        lineTotal = GetLineDecimal("Price")

                    };

                }).ToList();


                // --- 5. Prepare the final JSON response ---

                var response = new

                {

                    orderFound = true,

                    orderId = retrievedOrderId,

                    status = orderStatus,

                    grandTotal = finalGrandTotal,

                    estimatedDelivery = estimatedDelivery,

                    deliveryCharge = deliveryCharge,

                    discountAmount = discountAmount,

                    netAmount = netAmount,

                    formattedOrderDate = formattedOrderDate,

                    customerName = GetString("CustomerName"),

                    customerAddress = GetString("CustomerAddress"),

                    lineItems = lineItems

                };

                return Ok(response);

            }

            catch (Exception ex)

            {

                // 🛑 Log the exception (ex) here to see the actual error (e.g., InvalidCastException) 🛑

                // Example logging (replace with your actual logger):

                // _logger.LogError(ex, "Failed to retrieve order status for {OrderId}", orderId);

                return StatusCode(500, new { message = "Internal server error during tracking lookup. See server logs for details." });

            }

        }




        [HttpGet]
        [Route("order/check-email")]
        public IActionResult CheckEmail(string email, string phone) // <--- ADD string phone
        {
            if (string.IsNullOrWhiteSpace(email))
                return Json(new { exists = false });

            // 1. Check if email exists in DB
            var customer = _orderFacade.GetCustomerByEmail(email.Trim());

            if (customer == null)
            {
                // Email does not exist at all -> Available
                return Json(new { exists = false });
            }

            // 2. Email exists. Now check if it belongs to the CURRENT user (by Phone)
            if (!string.IsNullOrWhiteSpace(phone))
            {
                // Helper to normalize phone numbers (remove +88, spaces, etc)
                string Normalize(string p)
                {
                    if (string.IsNullOrEmpty(p)) return "";
                    p = p.Trim().Replace("-", "").Replace(" ", "").Replace("+", "").Replace("(", "").Replace(")", "");
                    if (p.StartsWith("88")) p = p.Substring(2);
                    return p;
                }

                string inputPhone = Normalize(phone);
                string dbPhone = Normalize(customer.Phone);

                if (inputPhone == dbPhone)
                {
                    // ✅ MATCH! The email belongs to the phone number being used.
                    // Return 'exists: false' so the frontend allows it.
                    return Json(new { exists = false });
                }
            }

            // 3. Email exists AND belongs to a DIFFERENT phone -> Conflict
            return Json(new { exists = true });
        }


        [HttpGet]
        [Route("order/check-customer")]
        public IActionResult CheckCustomer(string phone)
        {
            // 1. Validate Null/Empty
            if (string.IsNullOrWhiteSpace(phone))
            {
                return Json(new { found = false, message = "Phone number is empty." });
            }

            phone = phone.Trim();
            if (!phone.StartsWith("+"))
            {
                phone = "+" + phone;
            }

            // Call Facade logic
            var (customer, address) = _orderFacade.GetCustomerDetailsForAutofill(phone);

            if (customer != null)
            {
                Func<char[], string> CharArrayToString = (char[] arr) =>
                    arr != null ? new string(arr).Trim() : null;

                return Json(new
                {
                    found = true,
                    name = customer.CustomerName ?? "",
                    email = customer.Email ?? "",
                    addressData = (address != null) ? new
                    {
                        street = address.Street ?? "",
                        divison = address.Divison ?? "",
                        city = address.City ?? "",
                        postalCode = address.PostalCode,
                        zipCode = CharArrayToString(address.ZipCode),
                    } : null
                });
            }
            return Json(new { found = false });
        }

        [HttpGet]
        [Route("order/check-postal-code")]
        public IActionResult CheckPostalCode(string code)
        {
            if (string.IsNullOrEmpty(code)) return Json(new { found = false });

            var info = _orderFacade.GetPostalCodeDetails(code);

            if (info != null)
            {
                return Json(new
                {
                    found = true,
                    division = info.DivisionEn,
                    district = info.DistrictEn,
                    thana = info.ThanaEn,
                    subOffice = info.SubOfficeEn
                });
            }
            return Json(new { found = false });
        }
        // In OrderController.cs

        [HttpGet]
        [Route("order/get-divisions")]
        public IActionResult GetDivisions()
        {
            // Ensure _orderFacade.GetDivisions() is implemented
            var data = _orderFacade.GetDivisions();
            return Json(data);
        }

        [HttpGet]
        [Route("order/get-districts")]
        public IActionResult GetDistricts(string division)
        {
            var data = _orderFacade.GetDistricts(division);
            return Json(data);
        }

        [HttpGet]
        [Route("order/get-thanas")]
        public IActionResult GetThanas(string district)
        {
            var data = _orderFacade.GetThanas(district);
            return Json(data);
        }

        [HttpGet]
        [Route("order/get-suboffices")]
        public IActionResult GetSubOffices(string thana)
        {
            var data = _orderFacade.GetSubOffices(thana);
            return Json(data);
        }

        [Route("/order/all")]
        [HttpGet]
        public IActionResult AllOrders(int page = 1,int pageSize = 10,string status = "all", string payStatus = "all", string orderType = "all", string dateRange = "all",  DateTime? fromDate = null,  DateTime? toDate = null,     double? minAmount = null, double? maxAmount = null 
            ){
            if (!HasPermission("Order.View")) return HandleAccessDenied();

            try
            {
                // --- 1. Dynamic Company ID from Claims ---
                int companyId = 1; // Default fallback
                var companyClaim = User.FindFirst("CompanyId") ?? User.FindFirst("companyId");
                if (companyClaim != null && int.TryParse(companyClaim.Value, out int cId))
                {
                    companyId = cId;
                }

                // --- 2. Build Where Clause ---
                var whereBuilder = new System.Text.StringBuilder("1=1");

                // A. Filter by Status
                if (!string.IsNullOrEmpty(status) && status != "all")
                {
                    string cleanStatus = status.Replace("'", "''");
                    whereBuilder.Append($" AND Status = '{cleanStatus}'");
                }

                // B. Filter by Payment
                if (!string.IsNullOrEmpty(payStatus) && payStatus != "all")
                {
                    if (payStatus == "Paid") whereBuilder.Append(" AND DueAmount <= 0");
                    else if (payStatus == "Partial") whereBuilder.Append(" AND PaidAmount > 0 AND DueAmount > 0");
                    else if (payStatus == "Unpaid") whereBuilder.Append(" AND PaidAmount = 0");
                }

                // C. Filter by Order Type (SalesChannelId)
                if (!string.IsNullOrEmpty(orderType) && orderType != "all")
                {
                    // Assuming 1 = Online, 2 = Direct (Adjust IDs based on your DB seed)
                    if (orderType == "Online") whereBuilder.Append(" AND SalesChannelId = 1");
                    else if (orderType == "Direct") whereBuilder.Append(" AND SalesChannelId <> 1");
                }
                // NEW: Filter by Net Amount Range
                if (minAmount.HasValue)
                {
                    whereBuilder.Append($" AND NetAmount >= {minAmount.Value}");
                }
                if (maxAmount.HasValue)
                {
                    whereBuilder.Append($" AND NetAmount <= {maxAmount.Value}");
                }
                // D. Filter by Date Range
                if (!string.IsNullOrEmpty(dateRange) && dateRange != "all")
                {
                    DateTime today = DateTime.Today;
                    DateTime? start = null;
                    DateTime? end = null;

                    switch (dateRange)
                    {
                        case "today":
                            start = today;
                            end = today.AddDays(1).AddTicks(-1);
                            break;
                        case "yesterday":
                            start = today.AddDays(-1);
                            end = today.AddDays(-1).AddDays(1).AddTicks(-1);
                            break;
                        case "thisWeek":
                            // Assuming Sunday is start of week
                            int diff = (7 + (today.DayOfWeek - DayOfWeek.Sunday)) % 7;
                            start = today.AddDays(-1 * diff).Date;
                            end = today.AddDays(1).AddTicks(-1); // Up to now
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
                        case "thisYear":
                            start = new DateTime(today.Year, 1, 1);
                            end = today.AddDays(1).AddTicks(-1);
                            break;
                        case "lastYear":
                            start = new DateTime(today.Year - 1, 1, 1);
                            end = new DateTime(today.Year, 1, 1).AddTicks(-1);
                            break;
                        case "custom":
                            if (fromDate.HasValue) start = fromDate.Value.Date;
                            if (toDate.HasValue) end = toDate.Value.Date.AddDays(1).AddTicks(-1);
                            break;
                    }

                    if (start.HasValue)
                        whereBuilder.Append($" AND OrderDate >= '{start.Value:yyyy-MM-dd HH:mm:ss}'");

                    if (end.HasValue)
                        whereBuilder.Append($" AND OrderDate <= '{end.Value:yyyy-MM-dd HH:mm:ss}'");
                }

                string whereClause = whereBuilder.ToString();

                // --- 3. Execute Query ---
                int totalRows;
                var orders = _orderFacade.GetPagedOrdersForAdmin(page, pageSize, whereClause, out totalRows);

                var viewModel = new MDUA.Web.UI.Models.PagedOrderViewModel
                {
                    Orders = orders,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalRows = totalRows,
                    TotalPages = (int)Math.Ceiling((double)totalRows / pageSize)
                };
                if (viewModel.TotalPages == 0) viewModel.TotalPages = 1;

                // --- 4. Pass Filter State to View ---
                ViewData["CurrentStatus"] = status;
                ViewData["CurrentPayStatus"] = payStatus;
                ViewData["CurrentOrderType"] = orderType;
                ViewData["CurrentDateRange"] = dateRange;
                ViewData["CurrentFromDate"] = fromDate?.ToString("yyyy-MM-dd");
                ViewData["CurrentToDate"] = toDate?.ToString("yyyy-MM-dd");
                ViewData["CurrentMinAmount"] = minAmount;
                ViewData["CurrentMaxAmount"] = maxAmount;
                // Load Settings using Dynamic CompanyID
                var deliverySettings = _settingsFacade.GetDeliverySettings(companyId);
                ViewBag.DeliveryDhaka = deliverySettings["dhaka"];
                ViewBag.DeliveryOutside = deliverySettings["outside"];

                var paymentMethods = _paymentFacade.GetActivePaymentMethods(companyId);
                ViewBag.PaymentMethods = paymentMethods;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Failed to load: " + ex.Message;
                return View(new MDUA.Web.UI.Models.PagedOrderViewModel());
            }
        }


        [HttpPost]
        [Route("order/place")]
        public async Task<IActionResult> PlaceOrder([FromBody] SalesOrderHeader model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid Data: Please select a product variant and fill all required fields."
                });
            }

            try
            {
                // ---------------------------------------------------------
                // 1. CAPTURE IP ADDRESS
                // ---------------------------------------------------------
                // The Middleware above has already populated RemoteIpAddress with the real user IP
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                // Handle Loopback (Localhost) scenarios for display purposes
                if (ipAddress == "::1") ipAddress = "127.0.0.1";

                // Optional: Remove the port number if present (IPv6 often includes it)
                if (ipAddress != null && ipAddress.Contains("%"))
                {
                    ipAddress = ipAddress.Split('%')[0];
                }

                model.IPAddress = ipAddress;

                // ---------------------------------------------------------
                // 2. CAPTURE SESSION ID
                // ---------------------------------------------------------
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("IsActive")))
                {
                    HttpContext.Session.SetString("IsActive", "true");
                }

                model.SessionId = HttpContext.Session.Id;


                model.CreatedBy = model.CustomerName;
                // ---------------------------------------------------------
                // 3. PROCEED (✅ AWAIT REQUIRED)
                // ---------------------------------------------------------
                string orderId = await _orderFacade.PlaceGuestOrder(model);

                return Json(new
                {
                    success = true,
                    orderId = orderId
                });
            }
            catch (Exception ex)
            {
                var realError = ex.InnerException != null
                    ? ex.InnerException.Message
                    : ex.Message;

                return Json(new
                {
                    success = false,
                    message = realError
                });
            }
        }


        [HttpPost]
        [Route("SalesOrder/ToggleConfirmation")]
        public IActionResult ToggleConfirmation(int id, bool isConfirmed)
        {
            if (!HasPermission("Order.Place")) return HandleAccessDenied();

            try
            {
                string username = User.Identity.Name ?? "Unknown_User";
                Console.WriteLine($"Toggling Confirmation for Order ID: {id} to {isConfirmed} by {username}");
                // 1. Calculate Old State (Logic Fallback)
                // Since it's a toggle, if we are setting to TRUE, old was likely FALSE.
                bool oldConfirmed = !isConfirmed;

                // 2. CRITICAL: Perform Update First
                string newStatus = _orderFacade.UpdateOrderConfirmation(id, isConfirmed, username);

                // 3. LOGGING (SAFE MODE)
                try
                {
                    // Optional: Try to fetch real DB state if needed, but risky if DA is broken
                    // var realOrder = _orderFacade.GetOrderById(id); 

                    _logFacade.LogStatusChange(
                        entityId: id,
                        entityType: "SalesOrderHeader",
                        oldStatus: oldConfirmed ? "Confirmed" : "Unconfirmed",
                        newStatus: isConfirmed ? "Confirmed" : "Unconfirmed",
                        changedBy: username,
                        reason: isConfirmed ? "Order Confirmed Manually" : "Order Unconfirmed Manually"
                    );
                }
                catch { /* Ignore logging errors */ }

                return Json(new { success = true, newStatus = newStatus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [Route("order/add")]
        [HttpGet]
        public IActionResult Add()
        {
            if (!HasPermission("Order.Place")) return HandleAccessDenied();

            try
            {
                // 1. Fetch Products
                var products = _orderFacade.GetProductVariantsForAdmin();
                ViewBag.ProductVariants = products;

                int companyId = 1; // Default or fetch from User Claims
                var claim = User.FindFirst("CompanyId") ?? User.FindFirst("TargetCompanyId");
                if (claim != null && int.TryParse(claim.Value, out int parsedId))
                {
                    companyId = parsedId;
                }

                var settings = _settingsFacade.GetDeliverySettings(companyId) ?? new Dictionary<string, int>();

                if (!settings.ContainsKey("dhaka")) settings["dhaka"] = 0;
                if (!settings.ContainsKey("outside")) settings["outside"] = 0;

                ViewBag.DeliverySettings = settings; 

                return View();
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Error loading products: " + ex.Message;
                return View();
            }
        }


        [HttpPost]
        [Route("SalesOrder/UpdateStatus")]
        public IActionResult UpdateStatus(int id, string status)
        {
            try
            {
                // 1. Validate Status
                var allowedStatuses = new[] { "Draft", "Confirmed", "Shipped", "Delivered", "Cancelled", "Returned" };
                if (!allowedStatuses.Contains(status))
                {
                    return Json(new { success = false, message = "Invalid Status" });
                }

                // 2. Try to Get Old Status (SAFE MODE)
                // We wrap this in try-catch so DB mapping errors don't stop the update
                string oldStatus = "Unknown";
                try
                {
                    var order = _orderFacade.GetOrderById(id);
                    if (order != null) oldStatus = order.Status;
                }
                catch
                {
                    // Mapping failed (DateTime vs String issue). 
                    // We ignore it to ensure the Update still happens.
                    Console.WriteLine($"[Warning] Failed to fetch old status for Order {id}");
                }

                // 3. CRITICAL: Perform the Update
                _orderFacade.UpdateOrderStatus(id, status);

                // 4. INSERT LOG (SAFE MODE)
                try
                {
                    if (oldStatus != status)
                    {
                        _logFacade.LogStatusChange(
                            entityId: id,
                            entityType: "SalesOrderHeader",
                            oldStatus: oldStatus,
                            newStatus: status,
                            changedBy: User.Identity.Name ?? "Admin",
                            reason: "Manual Status Update"
                        );
                    }
                }
                catch { /* Log insertion failed, but Order is updated, so we suppress error */ }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        [Route("order/place-direct")]
        [HttpPost]
        public IActionResult PlaceDirectOrder([FromBody] SalesOrderHeader model)
        {
            if (!HasPermission("Order.Place")) return HandleAccessDenied();

            try
            {
                // ✅ FIX: Enforce valid Company ID from UI.
                if (model.TargetCompanyId <= 0)
                {
                    return Json(new { success = false, message = "Target Company ID is required." });
                }

                // ---------------------------------------------------------
                // 1. CAPTURE IP ADDRESS
                // ---------------------------------------------------------
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                // Check if behind a proxy (like Nginx/Cloudflare/IIS)
                if (Request.Headers.ContainsKey("X-Forwarded-For"))
                {
                    ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault();
                }

                // Handle Localhost IPv6
                if (ipAddress == "::1") ipAddress = "127.0.0.1";

                // Truncate to 45 chars to fit database schema
                if (!string.IsNullOrEmpty(ipAddress) && ipAddress.Length > 45)
                {
                    ipAddress = ipAddress.Substring(0, 45);
                }

                model.IPAddress = ipAddress;

                // ---------------------------------------------------------
                // 2. CAPTURE SESSION ID
                // ---------------------------------------------------------
                // "Kickstart" session if empty to ensure the ID is stable
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("IsActive")))
                {
                    HttpContext.Session.SetString("IsActive", "true");
                }

                model.SessionId = HttpContext.Session.Id;
                string loggedInUser = User.Identity?.Name ?? "Admin";
                model.CreatedBy = loggedInUser;
                // ---------------------------------------------------------
                // 3. EXECUTE ORDER
                // ---------------------------------------------------------
                // 🛑 FIX: Use 'var' or 'dynamic' because PlaceAdminOrder returns an object, not a string
                var result = _orderFacade.PlaceAdminOrder(model);

                // Pass the whole result object to the frontend. 
                // Your admin-order.js is already written to handle this object in the 'orderId' field.
                return Json(new { success = true, orderId = result, message = "Direct Order placed successfully!" });
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                if (string.IsNullOrEmpty(msg)) msg = ex.ToString();

                return Json(new { success = false, message = msg });
            }
        }

        [HttpGet]
        [Route("order/get-products")]
        public IActionResult GetProductsForAdmin()
        {
            if (!HasPermission("Order.Place")) return HandleAccessDenied();

            try
            {
                var products = _orderFacade.GetProductVariantsForAdmin();
                return Json(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        public class PaymentRequestModel : CustomerPayment
        {
            public decimal DeliveryCharge { get; set; }
        }

        [HttpPost]
        [Route("/order/add-payment")]
        public IActionResult AddAdvancePayment([FromBody] PaymentRequestModel model)
        {
            if (model.CustomerId <= 0 || model.Amount <= 0)
            {
                return Json(new { success = false, message = "Invalid Amount or Customer" });
            }

            if (model.PaymentMethodId <= 0)
            {
                return Json(new { success = false, message = "Please select a valid Payment Method." });
            }

            try
            {
                model.PaymentDate = DateTime.Now;
                model.CreatedBy = User.Identity?.Name ?? "Admin";
                model.CreatedAt = DateTime.Now;
                model.Status = "Completed";

                // ✅ CHANGE: Pass the DeliveryCharge to the Facade
                long newId = _paymentFacade.AddPayment(model, model.DeliveryCharge);

                if (newId > 0)
                {
                    return Json(new { success = true, message = "Payment recorded successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Database insertion failed." });
                }
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                return Json(new { success = false, message = msg });
            }
        }



    }
    }
