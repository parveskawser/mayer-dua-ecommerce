using MDUA.Entities;
using MDUA.Facade;
using MDUA.Facade.Interface;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MDUA.Web.UI.Services
{
    public class SmartGeminiChatService : IAiChatService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly IProductFacade _productFacade;
        private readonly IOrderFacade _orderFacade;
        private readonly IChatFacade _chatFacade;
        private readonly ISettingsFacade _settingsFacade;
        private readonly IPaymentFacade _paymentFacade;

        private const string ModelUrl =
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

        public SmartGeminiChatService(
            IConfiguration config,
            HttpClient httpClient,
            IProductFacade productFacade,
            IOrderFacade orderFacade,
            IChatFacade chatFacade,
            ISettingsFacade settingsFacade,
            IPaymentFacade paymentFacade)
        {
            _httpClient = httpClient;
            _productFacade = productFacade;
            _orderFacade = orderFacade;
            _chatFacade = chatFacade;
            _settingsFacade = settingsFacade;
            _paymentFacade = paymentFacade;

            _apiKey = config["gemini_api_key"];

            if (!string.IsNullOrEmpty(_apiKey))
                _apiKey = _apiKey.Trim();

            if (string.IsNullOrEmpty(_apiKey))
            {
                // Optionally log this warning if you have a logger, but do not throw.
                Console.WriteLine("WARNING: Gemini API Key is missing. AI Chat will be disabled.");
            }
        }

        public async Task<string> GetResponseAsync(
            string userMessage,
            List<string> history,

           
            int currentCompanyId = 1,
             int? contextProductId = null)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                return "I'm sorry, my AI brain is currently offline (API Key missing). Please contact human support.";
            }
            var sb = new StringBuilder();

            sb.AppendLine(@"You are MDUA assistant. Use 'real-time data' to help users.
⛔ Critical Operational Rules:
1. Ordering form & autofill:
   - Phone numbers: customers can enter any format (e.g., 01780..., +88017..., or 17...). reassure them that our system cleans and accepts all these formats automatically.
   - Welcome back: tell users that entering their registered phone number will automatically fill in their name and email.
   - Postal code: if they enter a 4-digit postal code, our system will automatically find their division, district, and thana for them.
2. Email & security:
   - Every phone number must have a unique email. if an email is already used by someone else, the system will ask for a new one.
3. Pricing:
   - Always use the 'calculated price' provided in the data. this price already includes active discounts.
4. Process:
   - Encourage users to fill out the form on the page for the fastest checkout.
   - We send a confirmation email and sms after the order is placed.
⛔ Ordering Rules:
1. When a user wants to buy, collect: name, phone, address (street, city, division, thana, suboffice), variant id, and quantity.
2. If they provide a 4-digit postal code, tell them you've automatically identified their location.
3. Once all info is collected, use the 'place_guest_order' tool.
4. Inform them they will receive an email/sms confirmation after the order is placed.");

            string contextData = await GetRelevantContext(userMessage, contextProductId, currentCompanyId);
            if (!string.IsNullOrEmpty(contextData))
            {
                sb.AppendLine("\n--- real-time data from database ---");
                sb.AppendLine(contextData);
                sb.AppendLine("--- end data ---\n");
            }

            var requestBody = new
            {
                contents = new[] {
                    new {
                        role = "user",
                        parts = new[] {
                            new {
                                text = sb.ToString()
                                     + "\n"
                                     + string.Join("\n", history ?? new List<string>())
                                     + "\nCustomer: "
                                     + userMessage
                            }
                        }
                    }
                },
                tools = new[] {
                    new {
                        function_declarations = new[] {
                            new {
                                name = "place_guest_order",
                                description = "Creates a new guest order in the system.",
                                parameters = new {
                                    type = "object",
                                    properties = new {
                                        customername = new { type = "string" },
                                        customerphone = new { type = "string" },
                                        customeremail = new { type = "string" },
                                        productvariantid = new { type = "integer" },
                                        orderquantity = new { type = "integer" },
                                        street = new { type = "string" },
                                        city = new { type = "string" },
                                        division = new { type = "string" },
                                        thana = new { type = "string" },
                                        suboffice = new { type = "string" },
                                        postalcode = new { type = "string" },
                                        paymentmethod = new { type = "string", @enum = new[] { "cod", "bkash" } }
                                    },
                                    required = new[] {
                                        "customername", "customerphone", "productvariantid", "orderquantity",
                                        "street", "city", "division", "thana", "suboffice"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var httpResponse = await _httpClient.PostAsync(
                $"{ModelUrl}?key={_apiKey}",
                new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
            );

            if (!httpResponse.IsSuccessStatusCode)
                return "System is currently busy. Please try again.";

            var responseString = await httpResponse.Content.ReadAsStringAsync();
            dynamic jsonRes = JsonConvert.DeserializeObject(responseString);
            var part = jsonRes?.candidates?[0]?.content?.parts?[0];

            if (part?.functionCall != null)
            {
                string functionName = part.functionCall.name;
                var args = part.functionCall.args;

                if (functionName == "place_guest_order")
                    return await HandleOrderToolCall(args, currentCompanyId);
            }

            return (string)(part?.text) ?? "I'm here to help with your order!";
        }

        private async Task<string> HandleOrderToolCall(dynamic args, int companyId)
        {
            try
            {
                var order = new SalesOrderHeader
                {
                    CustomerName = (string)args.customername,
                    CustomerPhone = (string)args.customerphone,
                    CustomerEmail = (string)args.customeremail,
                    ProductVariantId = (int)args.productvariantid,
                    OrderQuantity = (int)args.orderquantity,
                    Street = (string)args.street,
                    City = (string)args.city,
                    Divison = (string)args.division,
                    Thana = (string)args.thana,
                    SubOffice = (string)args.suboffice,
                    PostalCode = (string)args.postalcode,
                    TargetCompanyId = companyId
                };

                var settings = _settingsFacade.GetDeliverySettings(order.TargetCompanyId);

                bool isDhaka =
                    (!string.IsNullOrEmpty(order.Divison) && order.Divison.ToLower().Contains("dhaka")) ||
                    (!string.IsNullOrEmpty(order.City) && order.City.ToLower().Contains("dhaka"));

                order.DeliveryCharge = isDhaka ? settings["dhaka"] : settings["outside"];

                string orderNo = await _orderFacade.PlaceGuestOrder(order);

                return $"✅ Success! I have placed your order. Your Order ID is **{orderNo}**. You will receive a confirmation SMS/Email shortly.";
            }
            catch (Exception ex)
            {
                return $"❌ I encountered an error while placing the order: {ex.Message}. Please check your details and try again.";
            }
        }

        private async Task<string> GetRelevantContext(string message, int? activeProductId, int companyId)
        {
            var lowerMsg = (message ?? "").ToLower();
            var context = new StringBuilder();

            try
            {
                var delivery = _settingsFacade.GetDeliverySettings(companyId);
                context.AppendLine("🚚 Shipping information:");
                context.AppendLine($"- Inside Dhaka: ৳{delivery["dhaka"]}");
                context.AppendLine($"- Outside Dhaka: ৳{delivery["outside"]}");

                var paymentMethods = _paymentFacade.GetActivePaymentMethods(companyId);
                if (paymentMethods != null && paymentMethods.Any())
                {
                    context.AppendLine("\n💳 Available payment methods:");
                    foreach (var pm in paymentMethods)
                        context.AppendLine($"- {pm.MethodName}");
                }

                if (activeProductId.HasValue && activeProductId.Value > 0)
                {
                    var pageContext = await GetPageSpecificContext(activeProductId.Value);
                    if (!string.IsNullOrEmpty(pageContext))
                    {
                        context.AppendLine("\n🔴 Current page context (the product the user is seeing):");
                        context.AppendLine(pageContext);
                    }
                }

                if (!lowerMsg.Contains("this") && !lowerMsg.Contains("it") &&
                    ContainsAny(lowerMsg, "product", "item", "search", "find", "price", "stock"))
                {
                    var productInfo = await GetProductContext(message, companyId);
                    if (!string.IsNullOrEmpty(productInfo)) context.AppendLine(productInfo);
                }

                if (Regex.IsMatch(lowerMsg, @"(on|do)\d{8}"))
                {
                    var orderInfo = await GetOrderContext(message);
                    if (!string.IsNullOrEmpty(orderInfo)) context.AppendLine(orderInfo);
                }
            }
            catch (Exception ex)
            {
                context.AppendLine($"Note: dynamic data lookup limited ({ex.Message})");
            }

            return context.ToString();
        }

        private Task<string> GetPageSpecificContext(int productId)
        {
            var p = _productFacade.GetProductDetails(productId);
            if (p == null) return Task.FromResult<string>(null);

            var sb = new StringBuilder();
            sb.AppendLine($"Product: {p.ProductName}");

            var bestDiscount = _productFacade.GetBestDiscount(p.Id, p.BasePrice ?? 0);

            var allAttributes = _productFacade.GetVariantAttributes(productId);

            if (p.Variants != null && p.Variants.Any())
            {
                sb.AppendLine("Variations:");
                foreach (var v in p.Variants)
                {
                    var myAttrs = allAttributes
                        .Where(a => a.VariantId == v.Id)
                        .Select(a => $"{a.AttributeName}: {a.AttributeValue}");

                    decimal basePrice = v.VariantPrice ?? p.SellingPrice;
                    decimal calculatedPrice = basePrice;

                    if (bestDiscount != null)
                    {
                        if (bestDiscount.DiscountType == "flat")
                            calculatedPrice -= bestDiscount.DiscountValue;
                        else if (bestDiscount.DiscountType == "percentage")
                            calculatedPrice -= (basePrice * (bestDiscount.DiscountValue / 100m));
                    }

                    calculatedPrice = Math.Max(calculatedPrice, 0);

                    string name = myAttrs.Any()
                        ? string.Join(", ", myAttrs)
                        : v.VariantName;

                    sb.AppendLine($"- {name}: ৳{calculatedPrice:n0} [Stock: {v.StockQty}, Id: {v.Id}]");
                }
            }

            return Task.FromResult(sb.ToString());
        }

        private async Task<string> GetProductContext(string query, int currentCompanyId)
        {
            try
            {
                var searchTerm = ExtractSearchTerm(query);

                var products = _productFacade.SearchProducts(searchTerm, currentCompanyId);
                if (products == null || products.Count == 0) return "";

                var sb = new StringBuilder();
                sb.AppendLine($"📦 **Search results for '{searchTerm}':**\n");

                foreach (var p in products.Take(3))
                {
                    var bestDiscount = _productFacade.GetBestDiscount(p.Id, p.BasePrice ?? 0);
                    var variants = _productFacade.GetVariantsByProductId(p.Id);
                    var allAttributes = _productFacade.GetVariantAttributes(p.Id);

                    sb.AppendLine($"Product: {p.ProductName}");

                    foreach (var v in variants)
                    {
                        var myAttributes = allAttributes
                            .Where(a => a.VariantId == v.Id)
                            .Select(a => $"{a.AttributeName}: {a.AttributeValue}");

                        decimal basePrice = v.VariantPrice ?? p.BasePrice ?? 0;
                        decimal discountedPrice = basePrice;

                        if (bestDiscount != null)
                        {
                            if (bestDiscount.DiscountType == "flat")
                                discountedPrice -= bestDiscount.DiscountValue;
                            else if (bestDiscount.DiscountType == "percentage")
                                discountedPrice -= (basePrice * (bestDiscount.DiscountValue / 100m));
                        }

                        discountedPrice = Math.Max(discountedPrice, 0);

                        string name = myAttributes.Any()
                            ? string.Join(", ", myAttributes)
                            : (v.VariantName ?? "option");

                        sb.AppendLine($" - [{name}]: ৳{discountedPrice:n0} (Stock: {v.StockQty})");
                    }

                    sb.AppendLine();
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"Error fetching price details: {ex.Message}";
            }
        }

        private Task<string> GetOrderContext(string message)
        {
            try
            {
                var orderIdMatch = Regex.Match(
                    message ?? "",
                    @"(on|do)\d{8}",
                    RegexOptions.IgnoreCase);

                if (!orderIdMatch.Success)
                    return Task.FromResult("💡 To track your order, please provide your Order ID (e.g., ON12345678 or DO12345678)");

                string orderId = orderIdMatch.Value.ToUpper();

                var orderDetails = _orderFacade.GetOrderReceiptByOnlineId(orderId);

                if (orderDetails == null || orderDetails.Count == 0)
                    return Task.FromResult($"❌ Order {orderId} not found. Please verify the Order ID.");

                dynamic order = orderDetails[0];

                var sb = new StringBuilder();
                sb.AppendLine($"📦 **Order {orderId} status:**\n");
                sb.AppendLine($"Status: {order.status}");
                sb.AppendLine($"Order date: {Convert.ToDateTime(order.orderdate):dd MMM yyyy}");
                sb.AppendLine($"Total amount: ৳{order.totalamount:n0}");

                if ((string)order.status == "shipped" || (string)order.status == "delivered")
                    sb.AppendLine("Delivery: expected in 2–5 business days");

                return Task.FromResult(sb.ToString());
            }
            catch (Exception ex)
            {
                return Task.FromResult($"Error tracking order: {ex.Message}");
            }
        }

        private string ExtractSearchTerm(string message)
        {
            var sb = new StringBuilder();
            foreach (char c in (message ?? "").ToLower())
                sb.Append(char.IsPunctuation(c) ? ' ' : c);

            string cleanMessage = sb.ToString();

            var stopWords = new HashSet<string>
            {
                "show","me","find","search","looking","look","for","want","need","get",
                "do","you","have","is","are","can","i","buy","purchase","shop",
                "price","cost","rate","amount","how","much",
                "stock","available","availability","status","count","left","many",
                "details","info","information","about","desc","description",
                "product","item","unit","article","of","the","a","an","this","that"
            };

            var words = cleanMessage.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var validWords = new List<string>();
            foreach (var word in words)
                if (!stopWords.Contains(word))
                    validWords.Add(word);

            return string.Join(" ", validWords).Trim();
        }

        private bool ContainsAny(string text, params string[] keywords)
        {
            return keywords != null && keywords.Any(k => (text ?? "").Contains(k));
        }

        private bool ContainsHandOffTrigger(string aiResponse)
        {
            if (string.IsNullOrEmpty(aiResponse)) return false;

            var triggers = new[]
            {
                "support team",
                "human agent",
                "connect you",
                "speak with someone",
                "can't help with that",
                "beyond my capability"
            };

            return triggers.Any(t => aiResponse.ToLower().Contains(t));
        }
    }
}
