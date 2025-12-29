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
        private const string ModelUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

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

            _apiKey = config["GEMINI_API_KEY"];

            if (!string.IsNullOrEmpty(_apiKey))
                _apiKey = _apiKey.Trim();

            if (string.IsNullOrEmpty(_apiKey))
                throw new Exception("Gemini API Key is missing.");
            _settingsFacade = settingsFacade;
            _paymentFacade = paymentFacade;
        }

        public async Task<string> GetResponseAsync(string userMessage, List<string> history, int? contextProductId = null)
        {
            var sb = new StringBuilder();

            // 🧠 SYSTEM PROMPT: Instructions for ordering and tool usage
            sb.AppendLine(@"You are MDUA Assistant. Use 'REAL-TIME DATA' to help users.
⛔ CRITICAL OPERATIONAL RULES:
1. ORDERING FORM & AUTOFILL:
   - Phone Numbers: Customers can enter any format (e.g., 01780..., +88017..., or 17...). Reassure them that our system cleans and accepts all these formats automatically.
   - Welcome Back: Tell users that entering their registered phone number will automatically fill in their Name and Email.
   - Postal Code: If they enter a 4-digit Postal Code, our system will automatically find their Division, District, and Thana for them.
2. EMAIL & SECURITY:
   - Every phone number must have a unique email. If an email is already used by someone else, the system will ask for a new one.
3. PRICING:
   - Always use the 'Calculated Price' provided in the data. This price already includes active discounts.
4. PROCESS:
   - Encourage users to fill out the form on the page for the fastest checkout.
   - We send a confirmation Email and SMS after the order is placed.
⛔ ORDERING RULES:
1. When a user wants to buy, collect: Name, Phone, Address (Street, City, Division, Thana, SubOffice), Variant ID, and Quantity.
2. If they provide a 4-digit Postal Code, tell them you've automatically identified their location.
3. Once ALL info is collected, use the 'place_guest_order' tool.
4. Inform them they will receive an Email/SMS confirmation after the order is placed.");

            string contextData = await GetRelevantContext(userMessage, contextProductId);
            if (!string.IsNullOrEmpty(contextData))
            {
                sb.AppendLine("\n--- REAL-TIME DATA FROM DATABASE ---");
                sb.AppendLine(contextData);
                sb.AppendLine("--- END DATA ---\n");
            }

            // --- BUILD THE GEMINI REQUEST WITH TOOLS ---
            var requestBody = new
            {
                contents = new[] {
                    new { role = "user", parts = new[] { new { text = sb.ToString() + "\n" + string.Join("\n", history) + "\nCustomer: " + userMessage } } }
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
                                        customerName = new { type = "string" },
                                        customerPhone = new { type = "string" },
                                        customerEmail = new { type = "string" },
                                        productVariantId = new { type = "integer" },
                                        orderQuantity = new { type = "integer" },
                                        street = new { type = "string" },
                                        city = new { type = "string" },
                                        division = new { type = "string" },
                                        thana = new { type = "string" },
                                        subOffice = new { type = "string" },
                                        postalCode = new { type = "string" },
                                        paymentMethod = new { type = "string", @enum = new[] { "cod", "bkash" } }
                                    },
                                    required = new[] { "customerName", "customerPhone", "productVariantId", "orderQuantity", "street", "city", "division", "thana", "subOffice" }
                                }
                            }
                        }
                    }
                }
            };

            var response = await _httpClient.PostAsync($"{ModelUrl}?key={_apiKey}",
                new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                dynamic jsonRes = JsonConvert.DeserializeObject(responseString);
                var part = jsonRes?.candidates?[0]?.content?.parts?[0];

                // 🔧 CHECK FOR FUNCTION CALL
                if (part?.functionCall != null)
                {
                    string functionName = part.functionCall.name;
                    var args = part.functionCall.args;

                    if (functionName == "place_guest_order")
                    {
                        return await HandleOrderToolCall(args);
                    }
                }

                return part?.text ?? "I'm here to help with your order!";
            }

            return "System is currently busy. Please try again.";
        }
        private async Task<string> HandleOrderToolCall(dynamic args)
        {
            try
            {
                // Note: PaymentMethod is ignored here because SalesOrderHeader doesn't contain it,
                // but the AI has already collected the user's preference.
                var order = new SalesOrderHeader
                {
                    CustomerName = (string)args.customerName,
                    CustomerPhone = (string)args.customerPhone,
                    CustomerEmail = (string)args.customerEmail,
                    ProductVariantId = (int)args.productVariantId,
                    OrderQuantity = (int)args.orderQuantity,
                    Street = (string)args.street,
                    City = (string)args.city,
                    Divison = (string)args.division, 
                    Thana = (string)args.thana,
                    SubOffice = (string)args.subOffice,
                    PostalCode = (string)args.postalCode,
                    TargetCompanyId = 1
                };

               // Calculate Delivery Charge based on Division [cite: 122, 273, 863-864]
                var settings = _settingsFacade.GetDeliverySettings(order.TargetCompanyId);
                bool isDhaka = order.Divison.ToLower().Contains("dhaka") || order.City.ToLower().Contains("dhaka");
                order.DeliveryCharge = isDhaka ? settings["dhaka"] : settings["outside"];

                // EXECUTE BUSINESS LOGIC 
                string orderNo = await _orderFacade.PlaceGuestOrder(order);

                return $"✅ Success! I have placed your order. Your Order ID is **{orderNo}**. You will receive a confirmation SMS/Email shortly.";
            }
            catch (Exception ex)
            {
                return $"❌ I encountered an error while placing the order: {ex.Message}. Please check your details and try again.";
            }
        }
        private async Task<string> GetRelevantContext(string message, int? activeProductId)
        {
            var lowerMsg = message.ToLower();
            var context = new StringBuilder();

            try
            {
                // Resolve CompanyId (Default to 1)
                int companyId = 1;

                // 1️ DYNAMIC BUSINESS KNOWLEDGE (Delivery & Payment Methods)
                // This ensures the AI always knows the CURRENT prices and methods set in Admin settings
                var delivery = _settingsFacade.GetDeliverySettings(companyId);
                context.AppendLine("🚚 SHIPPING INFORMATION:");
                context.AppendLine($"- Inside Dhaka: ৳{delivery["dhaka"]}");
                context.AppendLine($"- Outside Dhaka: ৳{delivery["outside"]}");

                var paymentMethods = _paymentFacade.GetActivePaymentMethods(companyId);
                if (paymentMethods != null && paymentMethods.Any())
                {
                    context.AppendLine("\n💳 AVAILABLE PAYMENT METHODS:");
                    foreach (var pm in paymentMethods)
                    {
                        context.AppendLine($"- {pm.MethodName}");
                    }
                }

                // 2️ PAGE SPECIFIC CONTEXT (Preserved existing logic)
                if (activeProductId.HasValue && activeProductId.Value > 0)
                {
                    var pageContext = await GetPageSpecificContext(activeProductId.Value);
                    if (!string.IsNullOrEmpty(pageContext))
                    {
                        context.AppendLine("\n🔴 CURRENT PAGE CONTEXT (The product the user is seeing):");
                        context.AppendLine(pageContext);
                    }
                }

                // 3️ SEARCH LOGIC 
                if (!lowerMsg.Contains("this") && !lowerMsg.Contains("it") &&
                    ContainsAny(lowerMsg, "product", "item", "search", "find", "price", "stock"))
                {
                    var productInfo = await GetProductContext(message);
                    if (!string.IsNullOrEmpty(productInfo)) context.AppendLine(productInfo);
                }

                // 4️ ORDER TRACKING (Preserved existing logic)
                if (Regex.IsMatch(lowerMsg, @"(on|do)\d{8}"))
                {
                    var orderInfo = await GetOrderContext(message);
                    if (!string.IsNullOrEmpty(orderInfo)) context.AppendLine(orderInfo);
                }
            }
            catch (Exception ex)
            {
                context.AppendLine($"Note: Dynamic data lookup limited ({ex.Message})");
            }

            return context.ToString();
        }
        private async Task<string> GetPageSpecificContext(int productId)
        {
            var p = _productFacade.GetProductDetails(productId);
            if (p == null) return null;

            var sb = new StringBuilder();
            sb.AppendLine($"Product: {p.ProductName}");

           // ✅ Fetch discount once for product
            var bestDiscount = _productFacade.GetBestDiscount(p.Id, p.BasePrice ?? 0);

            var allAttributes = _productFacade.GetVariantAttributes(productId);
            if (p.Variants != null && p.Variants.Any())
            {
                sb.AppendLine("Variations:");
                foreach (var v in p.Variants)
                {
                    var myAttrs = allAttributes.Where(a => a.VariantId == v.Id)
                                               .Select(a => $"{a.AttributeName}: {a.AttributeValue}");

                   // ✅ DYNAMIC PRICE CALCULATION (Sync with OrderFacade) [cite: 23-26]
                    decimal basePrice = v.VariantPrice ?? p.SellingPrice;
                    decimal calculatedPrice = basePrice;

                    if (bestDiscount != null)
                    {
                        if (bestDiscount.DiscountType == "Flat")
                            calculatedPrice -= bestDiscount.DiscountValue;
                        else if (bestDiscount.DiscountType == "Percentage")
                            calculatedPrice -= (basePrice * (bestDiscount.DiscountValue / 100));
                    }
                    calculatedPrice = Math.Max(calculatedPrice, 0);

                    string name = myAttrs.Any() ? string.Join(", ", myAttrs) : v.VariantName;
                    sb.AppendLine($"- {name}: ৳{calculatedPrice:N0} [Stock: {v.StockQty}, ID: {v.Id}]");
                }
            }
            return sb.ToString();
        }
        private async Task<string> GetBusinessRulesContext(int companyId)
        {
            var sb = new StringBuilder();

            // ✅ Fetch Delivery Charges dynamically from DB [cite: 698]
            var delivery = _settingsFacade.GetDeliverySettings(companyId);
            sb.AppendLine("🚚 DELIVERY CHARGES:");
            sb.AppendLine($"- Inside Dhaka: ৳{delivery["dhaka"]}");
            sb.AppendLine($"- Outside Dhaka: ৳{delivery["outside"]}");

           // ✅ Fetch Payment Methods dynamically from DB 
            var payments = _settingsFacade.GetCompanyPaymentSettings(companyId);
            var enabledPayments = payments.Where(p => p.IsEnabled).ToList();

            if (enabledPayments.Any())
            {
                sb.AppendLine("\n💳 ACCEPTED PAYMENT METHODS:");
                foreach (var pm in enabledPayments)
                {
                    string mode = pm.IsManualEnabled && pm.IsGatewayEnabled ? "Mobile Banking & Online Gateway" :
                                 pm.IsGatewayEnabled ? "Online Gateway" : "Manual/Cash";
                    sb.AppendLine($"- {pm.MethodName} ({mode})");
                    if (!string.IsNullOrEmpty(pm.CustomInstruction))
                        sb.AppendLine($"  Instruction: {pm.CustomInstruction}"); // This shows the Bkash number [cite: 729]
                }
            }

            return sb.ToString();
        }
        // 📦 PRODUCT CONTEXT BUILDER (Corrected with Attribute Lookup)
        private async Task<string> GetProductContext(string query)
        {
            try
            {
                var searchTerm = ExtractSearchTerm(query);
                var products = _productFacade.SearchProducts(searchTerm);
                if (products == null || products.Count == 0) return "";

                var sb = new StringBuilder();
                sb.AppendLine($"📦 **Search Results for '{searchTerm}':**\n");

                foreach (var p in products.Take(3))
                {
                    // Fetch discount once per product
                    var bestDiscount = _productFacade.GetBestDiscount(p.Id, p.BasePrice ?? 0);
                    var variants = _productFacade.GetVariantsByProductId(p.Id);
                    var allAttributes = _productFacade.GetVariantAttributes(p.Id);

                    sb.AppendLine($"Product: {p.ProductName}");
                    foreach (var v in variants)
                    {
                        var myAttributes = allAttributes.Where(a => a.VariantId == v.Id)
                                                        .Select(a => $"{a.AttributeName}: {a.AttributeValue}");

                        // Apply the same calculation used in PlaceGuestOrder
                        decimal basePrice = v.VariantPrice ?? p.BasePrice ?? 0;
                        decimal discountedPrice = basePrice;

                        if (bestDiscount != null)
                        {
                            if (bestDiscount.DiscountType == "Flat")
                                discountedPrice -= bestDiscount.DiscountValue;
                            else if (bestDiscount.DiscountType == "Percentage")
                                discountedPrice -= (basePrice * (bestDiscount.DiscountValue / 100));
                        }
                        discountedPrice = Math.Max(discountedPrice, 0);

                        string name = myAttributes.Any() ? string.Join(", ", myAttributes) : (v.VariantName ?? "Option");
                        sb.AppendLine($" - [{name}]: ৳{discountedPrice:N0} (Stock: {v.StockQty}, ID: {v.Id})");
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
        // 📋 ORDER TRACKING CONTEXT
        private async Task<string> GetOrderContext(string message)
        {
            try
            {
                // Extract Order ID (format: ON12345678 or DO12345678)
                var orderIdMatch = Regex.Match(
                    message,
                    @"(ON|DO)\d{8}",
                    RegexOptions.IgnoreCase);

                if (!orderIdMatch.Success)
                    return "💡 To track your order, please provide your Order ID (e.g., ON12345678 or DO12345678)";

                string orderId = orderIdMatch.Value.ToUpper();

                // Fetch order details
                var orderDetails = _orderFacade.GetOrderReceiptByOnlineId(orderId);

                if (orderDetails == null || orderDetails.Count == 0)
                    return $"❌ Order {orderId} not found. Please verify the Order ID.";

                var order = orderDetails[0] as dynamic;

                var sb = new StringBuilder();
                sb.AppendLine($"📦 **Order {orderId} Status:**\n");
                sb.AppendLine($"Status: {order.Status}");
                sb.AppendLine($"Order Date: {Convert.ToDateTime(order.OrderDate):dd MMM yyyy}");
                sb.AppendLine($"Total Amount: ৳{order.TotalAmount:N0}");

                if (order.Status == "Shipped" || order.Status == "Delivered")
                    sb.AppendLine($"Delivery: Expected in 2-5 business days");

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"Error tracking order: {ex.Message}";
            }
        }

        // 🔥 TRENDING PRODUCTS
        private string GetTrendingProducts()
        {
            try
            {
                var sb = new StringBuilder();

                // 1. Try to get ALL products (using a space " " to bypass empty-string checks)
                var products = _productFacade.SearchProducts(" ");

                // 2. If " " didn't work, try empty string
                if (products == null || products.Count == 0)
                    products = _productFacade.SearchProducts("");

                if (products == null || products.Count == 0)
                    return ""; // Database is truly empty or search is broken

                sb.AppendLine("🔥 **Top Available Products:**\n");

                foreach (var p in products.Take(10))
                {
                    sb.AppendLine($"• {p.ProductName} - ৳{p.SellingPrice:N0}");
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"Error fetching list: {ex.Message}";
            }
        }

        private string ExtractSearchTerm(string message)
        {
            // 1. Clean the message first (To Lower + Remove Punctuation)
            var sb = new StringBuilder();
            foreach (char c in message.ToLower())
            {
                sb.Append(char.IsPunctuation(c) ? ' ' : c);
            }
            string cleanMessage = sb.ToString();

            // 2. Define the "Stop Words" (Words to completely delete)
            var stopWords = new HashSet<string>
            {
                "show", "me", "find", "search", "looking", "look", "for", "want", "need", "get",
                "do", "you", "have", "is", "are", "can", "i", "buy", "purchase", "shop",
                "price", "cost", "rate", "amount", "how", "much",
                "stock", "available", "availability", "status", "count", "left", "many",
                "details", "info", "information", "about", "desc", "description",
                "product", "item", "unit", "article", "of", "the", "a", "an", "this", "that"
            };

            // 3. Split into words and filter
            var words = cleanMessage.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var validWords = new List<string>();
            foreach (var word in words)
            {
                if (!stopWords.Contains(word))
                {
                    validWords.Add(word);
                }
            }

            return string.Join(" ", validWords).Trim();
        }

        private bool ContainsAny(string text, params string[] keywords)
        {
            return keywords.Any(k => text.Contains(k));
        }

        // 🚨 HELPER: Detect if AI wants human takeover
        private bool ContainsHandoffTrigger(string aiResponse)
        {
            if (string.IsNullOrEmpty(aiResponse)) return false;

            var triggers = new[] {
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