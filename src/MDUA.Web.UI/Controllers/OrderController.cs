using MDUA.Entities;
using MDUA.Facade;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace MDUA.Web.UI.Controllers
{
     public class OrderController : Controller
    {
        private readonly IOrderFacade _orderFacade; 
        private readonly IUserLoginFacade _userLoginFacade;

        public OrderController(IOrderFacade orderFacade, IUserLoginFacade userLoginFacade)
        {
            _orderFacade = orderFacade;
            _userLoginFacade = userLoginFacade;

        }

        // The Route must match what the JavaScript is calling (e.g., /Order/GetOrderStatus?orderId=ON...)
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
                string orderStatus = GetString("OrderStatus");

                decimal netAmount = GetDecimal("NetAmount");
                decimal discountAmount = GetDecimal("DiscountAmount");

                string deliveryDivision = GetString("Divison").ToLower();

                DateTime orderDate = GetDateTime("OrderDate");

                // --- 3. Delivery and Total Calculation ---
                // Inside GetOrderStatus:
                // ... (after DateTime orderDate = GetDateTime("OrderDate");)

                // --- 3. Delivery and Total Calculation ---

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
        public IActionResult CheckEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Json(new { exists = false });

            var customer = _orderFacade.GetCustomerByEmail(email);
            bool exists = customer != null;

            return Json(new { exists = exists });
        }
        [HttpGet]
        [Route("order/check-customer")]
        // ✅ FIX: Remove [FromQuery] and rely on standard parameter binding for simple string
        public IActionResult CheckCustomer(string phone)
        {
            // NO try/catch here, we assume the global error handler is active.

            // We must check for null/empty string here manually, as the binder might allow it
            if (string.IsNullOrEmpty(phone))
            {
                // Return success=false if phone is empty, not a 400 error
                return Json(new { found = false, message = "Phone number is empty." });
            }

            // Call Facade logic
            var (customer, address) = _orderFacade.GetCustomerDetailsForAutofill(phone);

            if (customer != null)
            {
                // Helper to convert char[] to string safely
                Func<char[], string> CharArrayToString = (char[] arr) =>
                    arr != null ? new string(arr).Trim() : null;

                return Json(new
                {
                    found = true,
                    name = customer.CustomerName ?? "",
                    email = customer.Email ?? "",

                    // Safely map all address fields (even if address is null)
                    addressData = (address != null) ? new
                    {
                        street = address.Street ?? "",
                        divison = address.Divison ?? "",
                        city = address.City ?? "",
                        postalCode = address.PostalCode, // Assumed string/varchar
                        zipCode = CharArrayToString(address.ZipCode), // Convert char[] to string
                                                                      // Note: Thana/SubOffice must be mapped here if needed
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
         //new
 [HttpGet]
 public IActionResult AllOrders()
 {
     try
     {
         // 1. Fetch all orders from the Facade
         List<SalesOrderHeader> orders = _orderFacade.GetAllOrdersForAdmin();

         // 2. Pass the list to the view. 
         // The view should be strongly typed to List<MDUA.Entities.SalesOrderHeader>.
         return View(orders);
     }
     catch (Exception ex)
     {
         // Log the error and show an empty list or error view
         ViewData["ErrorMessage"] = "Failed to load order list: " + ex.Message;
         return View(new List<SalesOrderHeader>());
     }
 }
        [HttpPost]
        [Route("order/place")]
        public IActionResult PlaceOrder([FromBody] SalesOrderHeader model)
        {
            try
            {
                var orderId = _orderFacade.PlaceGuestOrder(model);
                return Json(new { success = true, orderId = orderId });
            }
            catch (Exception ex)
            {
                // ✅ FIX: Get the Inner Exception if available (This usually holds the SQL error)
                var realError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                // If both are empty, dump the whole object
                if (string.IsNullOrEmpty(realError)) realError = ex.ToString();

                return Json(new { success = false, message = realError });
            }
        }      
    }
}
