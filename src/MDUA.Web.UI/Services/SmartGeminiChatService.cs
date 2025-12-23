using MDUA.Facade.Interface;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        private const string ModelUrl ="https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";
        public SmartGeminiChatService(
            IConfiguration config,
            HttpClient httpClient,
            IProductFacade productFacade,
            IOrderFacade orderFacade,
            IChatFacade chatFacade)
        {
            _httpClient = httpClient;
            _productFacade = productFacade;
            _orderFacade = orderFacade;
            _chatFacade = chatFacade;

            // Inside SmartGeminiChatService constructor
            _apiKey = config["GEMINI_API_KEY"];

            // ADD THIS CLEANUP:
            if (!string.IsNullOrEmpty(_apiKey))
                _apiKey = _apiKey.Trim();

            if (string.IsNullOrEmpty(_apiKey))
                throw new Exception("Gemini API Key is missing.");
        }

        public async Task<string> GetResponseAsync(string userMessage, List<string> history)
        {
            var sb = new StringBuilder();

            // 🔥 SYSTEM PROMPT with Instructions
            // 🔥 MERGED SYSTEM PROMPT
            sb.AppendLine(@"You are MDUA Assistant, a helpful AI for MDUA - an e-commerce platform in Bangladesh.

⛔ STRICT DATA RULES (CRITICAL):
1. You have access to a section called 'REAL-TIME DATA' below.
2. ONLY recommend products listed in that data. Do NOT invent or hallucinate product names.
3. If a product is NOT in the 'REAL-TIME DATA', explicitly say: 'I couldn't find that item in our catalog.'
4. If data says 'Stock: 0', you MUST say it is currently out of stock.

CAPABILITIES:
✅ Search and recommend products (from provided data only)
✅ Check stock availability
✅ Explain prices and discounts
✅ Track orders by Order ID
✅ Guide customers through checkout
✅ Answer delivery questions

BUSINESS RULES & PRICING:
1. Delivery Charge: Inside Dhaka: ৳60 | Outside Dhaka: ৳120 
2. Prices: Always format as ৳1,500 (use commas).
3. If a product is out of stock, you SHOULD still mention its price if known, but clearly state it is unavailable.
4. Order Tracking: Ask for Order ID (format: ONXXXXXXXX or DOXXXXXXXX)[cite: 145].
5. Human Handoff: If you cannot help, say: 'Let me connect you with our support team for personalized assistance.'

RESPONSE STYLE:
- Be friendly, concise, and use emojis occasionally 😊
- Keep answers under 3 sentences when possible
- Use bullet points for product lists
- Be conversational, not robotic");

            // 🆕 DYNAMIC CONTEXT INJECTION
            string contextData = await GetRelevantContext(userMessage);
            if (!string.IsNullOrEmpty(contextData))
            {
                sb.AppendLine("\n--- REAL-TIME DATA FROM DATABASE ---");
                sb.AppendLine(contextData);
                sb.AppendLine("--- END DATA ---\n");
            }

            // Add conversation history
            if (history != null && history.Count > 0)
            {
                sb.AppendLine("Conversation history:");
                foreach (var line in history.Take(5)) // Last 5 messages only
                {
                    sb.AppendLine(line);
                }
            }

            sb.AppendLine($"\nCustomer: {userMessage}");
            sb.AppendLine("AI:");

            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = sb.ToString() } } } }
            };

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{ModelUrl}?key={_apiKey}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);
                string aiText = jsonResponse?.candidates?[0]?.content?.parts?[0]?.text;

                // 🔍 Check if AI is requesting human help
                if (ContainsHandoffTrigger(aiText))
                {
                    return aiText + "\n\n🔔 *I've notified our team. A support agent will join shortly.*";
                }

                return aiText ?? "I'm sorry, I couldn't generate a response.";
            }

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                return $"Gemini error {(int)response.StatusCode}: {err}";
            }

            return $"AI is currently unavailable. Please try again or contact support.";
        }

        // 🧠 INTELLIGENCE ENGINE: Detects intent and fetches relevant data
        private async Task<string> GetRelevantContext(string message)
        {
            var lowerMsg = message.ToLower();
            var context = new StringBuilder();

            try
            {
                // =========================================================
                // 🆕 REPLACE THE OLD "PRODUCT SEARCH" BLOCK WITH YOUR NEW CODE HERE
                // =========================================================

                // 0️⃣ CHECK FOR GENERIC "SHOW PRODUCTS" QUERY
                // If user asks "Show me available products" or "What do you have?", don't run a keyword search.
                // Inside GetRelevantContext ...

                // 0️⃣ GENERIC REQUEST (e.g. "List", "What is available")
                bool isGenericRequest = ContainsAny(lowerMsg, "available products", "list", "what do you have", "show me items", "catalogue");

                if (isGenericRequest)
                {
                    var trending = GetTrendingProducts();
                    if (!string.IsNullOrEmpty(trending))
                    {
                        context.AppendLine("User asked for a list. Here are our top items:");
                        context.AppendLine(trending);
                    }
                    else
                    {
                        // FAILSAFE: If trending is empty, tell AI to apologize
                        context.AppendLine("User asked for a list, but no trending data was returned from DB.");
                    }
                }
                // 1️⃣ SPECIFIC SEARCH (Only if NOT generic)
                else if (ContainsAny(lowerMsg, "product", "item", "buy", "purchase", "find", "search", "price", "stock", "is "))
                {
                    // Added "is " to the keywords trigger
                    var productInfo = await GetProductContext(message);
                    if (!string.IsNullOrEmpty(productInfo))
                        context.AppendLine(productInfo);
                }

                // =========================================================
                // 👇 KEEP THE REST OF THE EXISTING CODE BELOW UNTOUCHED
                // =========================================================

                // 2️⃣ ORDER TRACKING
                if (ContainsAny(lowerMsg, "order", "track", "delivery", "shipped", "on", "do") &&
                    (lowerMsg.Contains("on") || lowerMsg.Contains("do")))
                {
                    var orderInfo = await GetOrderContext(message);
                    if (!string.IsNullOrEmpty(orderInfo))
                        context.AppendLine(orderInfo);
                }

                // 3️⃣ LOW STOCK ALERTS (Optional: You can keep this for specific "recommend" keywords)
                if (ContainsAny(lowerMsg, "recommend", "suggest", "popular", "trending", "best"))
                {
                    var trending = GetTrendingProducts();
                    if (!string.IsNullOrEmpty(trending))
                        context.AppendLine(trending);
                }
            }
            catch (Exception ex)
            {
                context.AppendLine($"Note: Some data couldn't be retrieved ({ex.Message})");
            }

            return context.ToString();
        }
        // 📦 PRODUCT CONTEXT BUILDER
        private async Task<string> GetProductContext(string query)
        {
            try
            {
                var searchTerm = ExtractSearchTerm(query);
                var products = _productFacade.SearchProducts(searchTerm);

                if (products == null || products.Count == 0)
                    return $"❌ No products found matching '{searchTerm}'";

                var sb = new StringBuilder();
                sb.AppendLine($"📦 **Search Results for '{searchTerm}':**\n");

                int count = 0;
                foreach (var p in products.Take(5))
                {
                    try
                    {
                        count++;

                        // 1. Get Variants
                        var variants = _productFacade.GetVariantsByProductId(p.Id);

                        // 2. Calculate Total Stock Safely
                        int totalStock = 0;
                        string variantDetails = "";

                        if (variants != null && variants.Count > 0)
                        {
                            totalStock = variants.Sum(v => v.StockQty);

                            // 3. DYNAMIC NAME BUILDING (Prevents Crashes)
                            // We iterate and try to find a valid name property dynamically
                            var vList = new List<string>();
                            foreach (var v in variants)
                            {
                                // Try to read "VariantName", "Name", or "Title" dynamically
                                // This fixes the "ProductVariant does not contain..." error
                                string name = "Option";

                                // Reflection check for properties
                                var props = v.GetType().GetProperties();
                                var nameProp = props.FirstOrDefault(x =>
                                    x.Name.Equals("VariantName", StringComparison.OrdinalIgnoreCase) ||
                                    x.Name.Equals("Name", StringComparison.OrdinalIgnoreCase) ||
                                    x.Name.Equals("Title", StringComparison.OrdinalIgnoreCase) ||
                                    x.Name.Equals("Size", StringComparison.OrdinalIgnoreCase)); // Fallback to Size if it exists

                                if (nameProp != null)
                                {
                                    var val = nameProp.GetValue(v);
                                    if (val != null) name = val.ToString();
                                }

                                vList.Add($"{name} ({v.StockQty})");
                            }
                            variantDetails = string.Join(", ", vList);
                        }
                        else
                        {
                            // No variants? Use product stock if available, or default to 0
                            // Check if Product entity has a generic Stock property
                            var pStock = p.GetType().GetProperty("StockQty")?.GetValue(p);
                            if (pStock != null) totalStock = (int)pStock;
                        }

                        // 4. Formatting Output
                        var discount = _productFacade.GetBestDiscount(p.Id, p.BasePrice ?? 0);
                        string priceDisplay = discount != null
                            ? $"৳{p.SellingPrice:N0} (was ৳{p.BasePrice:N0})"
                            : $"৳{p.BasePrice:N0}";

                        string stockStatus = totalStock > 0
                            ? $"✅ In Stock ({totalStock} available)"
                            : "❌ Out of Stock";

                        sb.AppendLine($"{count}. **{p.ProductName}**");
                        sb.AppendLine($"   Price: {priceDisplay}");
                        sb.AppendLine($"   Stock: {stockStatus}");

                        if (!string.IsNullOrEmpty(variantDetails))
                            sb.AppendLine($"   Details: {variantDetails}");

                        sb.AppendLine();
                    }
                    catch (Exception innerEx)
                    {
                        // If one product fails, log it but DON'T stop the loop
                        sb.AppendLine($"   (Error loading details for {p.ProductName}: {innerEx.Message})\n");
                    }
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"Error fetching products: {ex.Message}";
            }
        }
        // 📋 ORDER TRACKING CONTEXT
        private async Task<string> GetOrderContext(string message)
        {
            try
            {
                // Extract Order ID (format: ON12345678 or DO12345678)
                var orderIdMatch = System.Text.RegularExpressions.Regex.Match(
                    message,
                    @"(ON|DO)\d{8}",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

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
        // 🔥 TRENDING / ALL PRODUCTS LIST
        private string GetTrendingProducts()
        {
            try
            {
                var sb = new StringBuilder();

                // 1. Try to get ALL products (using a space " " to bypass empty-string checks)
                var products = _productFacade.SearchProducts(" ");

                // 2. If " " didn't work, try a very common letter like "a" or just null
                if (products == null || products.Count == 0)
                    products = _productFacade.SearchProducts("");

                if (products == null || products.Count == 0)
                    return ""; // Database is truly empty or search is broken

                sb.AppendLine("🔥 **Top Available Products:**\n");

                foreach (var p in products.Take(10))
                {
                    // Simple display for the list
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
            // We replace punctuation with spaces to ensure words don't get stuck together (e.g., "stock?iphone" -> "stock iphone")
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

            // 4. Join back together
            // Result: "dsadsa231 stock?" -> "dsadsa231"
            // Result: "stock of dsadsa231" -> "dsadsa231"
            // Result: "price for iphone 15" -> "iphone 15"
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