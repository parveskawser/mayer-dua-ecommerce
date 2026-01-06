//using mdua.entities;
//using mdua.facade;
//using mdua.facade.interface;
//using microsoft.extensions.configuration;
//using newtonsoft.json;
//using system;
//using system.collections.generic;
//using system.linq;
//using system.net.http;
//using system.text;
//using system.text.regularexpressions;
//using system.threading.tasks;

//namespace mdua.web.ui.services
//{
//    public class smartgeminichatservice : iaichatservice
//    {
//        private readonly string _apikey;
//        private readonly httpclient _httpclient;
//        private readonly iproductfacade _productfacade;
//        private readonly iorderfacade _orderfacade;
//        private readonly ichatfacade _chatfacade;
//        private readonly isettingsfacade _settingsfacade;
//        private readonly ipaymentfacade _paymentfacade;
//        private const string modelurl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generatecontent";

//        public smartgeminichatservice(
//            iconfiguration config,
//            httpclient httpclient,
//            iproductfacade productfacade,
//            iorderfacade orderfacade,
//            ichatfacade chatfacade,
//            isettingsfacade settingsfacade,
//            ipaymentfacade paymentfacade)
//        {
//            _httpclient = httpclient;
//            _productfacade = productfacade;
//            _orderfacade = orderfacade;
//            _chatfacade = chatfacade;

//            _apikey = config["gemini_api_key"];

//            if (!string.isnullorempty(_apikey))
//                _apikey = _apikey.trim();

//            if (string.isnullorempty(_apikey))
//                throw new exception("gemini api key is missing.");
//            _settingsfacade = settingsfacade;
//            _paymentfacade = paymentfacade;
//        }

//        public async task<string> getresponseasync(string usermessage, list<string> history, int? contextproductid = null)
//        {
//            var sb = new stringbuilder();

//            // 🧠 system prompt: instructions for ordering and tool usage
//            sb.appendline(@"you are mdua assistant. use 'real-time data' to help users.
//⛔ critical operational rules:
//1. ordering form & autofill:
//   - phone numbers: customers can enter any format (e.g., 01780..., +88017..., or 17...). reassure them that our system cleans and accepts all these formats automatically.
//   - welcome back: tell users that entering their registered phone number will automatically fill in their name and email.
//   - postal code: if they enter a 4-digit postal code, our system will automatically find their division, district, and thana for them.
//2. email & security:
//   - every phone number must have a unique email. if an email is already used by someone else, the system will ask for a new one.
//3. pricing:
//   - always use the 'calculated price' provided in the data. this price already includes active discounts.
//4. process:
//   - encourage users to fill out the form on the page for the fastest checkout.
//   - we send a confirmation email and sms after the order is placed.
//⛔ ordering rules:
//1. when a user wants to buy, collect: name, phone, address (street, city, division, thana, suboffice), variant id, and quantity.
//2. if they provide a 4-digit postal code, tell them you've automatically identified their location.
//3. once all info is collected, use the 'place_guest_order' tool.
//4. inform them they will receive an email/sms confirmation after the order is placed.");

//            string contextdata = await getrelevantcontext(usermessage, contextproductid);
//            if (!string.isnullorempty(contextdata))
//            {
//                sb.appendline("\n--- real-time data from database ---");
//                sb.appendline(contextdata);
//                sb.appendline("--- end data ---\n");
//            }

//            // --- build the gemini request with tools ---
//            var requestbody = new
//            {
//                contents = new[] {
//                    new { role = "user", parts = new[] { new { text = sb.tostring() + "\n" + string.join("\n", history) + "\ncustomer: " + usermessage } } }
//                },
//                tools = new[] {
//                    new {
//                        function_declarations = new[] {
//                            new {
//                                name = "place_guest_order",
//                                description = "creates a new guest order in the system.",
//                                parameters = new {
//                                    type = "object",
//                                    properties = new {
//                                        customername = new { type = "string" },
//                                        customerphone = new { type = "string" },
//                                        customeremail = new { type = "string" },
//                                        productvariantid = new { type = "integer" },
//                                        orderquantity = new { type = "integer" },
//                                        street = new { type = "string" },
//                                        city = new { type = "string" },
//                                        division = new { type = "string" },
//                                        thana = new { type = "string" },
//                                        suboffice = new { type = "string" },
//                                        postalcode = new { type = "string" },
//                                        paymentmethod = new { type = "string", @enum = new[] { "cod", "bkash" } }
//                                    },
//                                    required = new[] { "customername", "customerphone", "productvariantid", "orderquantity", "street", "city", "division", "thana", "suboffice" }
//                                }
//                            }
//                        }
//                    }
//                }
//            };

//            var response = await _httpclient.postasync($"{modelurl}?key={_apikey}",
//                new stringcontent(jsonconvert.serializeobject(requestbody), encoding.utf8, "application/json"));

//            if (response.issuccessstatuscode)
//            {
//                var responsestring = await response.content.readasstringasync();
//                dynamic jsonres = jsonconvert.deserializeobject(responsestring);
//                var part = jsonres?.candidates?[0]?.content?.parts?[0];

//                // 🔧 check for function call
//                if (part?.functioncall != null)
//                {
//                    string functionname = part.functioncall.name;
//                    var args = part.functioncall.args;

//                    if (functionname == "place_guest_order")
//                    {
//                        return await handleordertoolcall(args);
//                    }
//                }

//                return part?.text ?? "i'm here to help with your order!";
//            }

//            return "system is currently busy. please try again.";
//        }
//        private async task<string> handleordertoolcall(dynamic args)
//        {
//            try
//            {
//                // note: paymentmethod is ignored here because salesorderheader doesn't contain it,
//                // but the ai has already collected the user's preference.
//                var order = new salesorderheader
//                {
//                    customername = (string)args.customername,
//                    customerphone = (string)args.customerphone,
//                    customeremail = (string)args.customeremail,
//                    productvariantid = (int)args.productvariantid,
//                    orderquantity = (int)args.orderquantity,
//                    street = (string)args.street,
//                    city = (string)args.city,
//                    divison = (string)args.division,
//                    thana = (string)args.thana,
//                    suboffice = (string)args.suboffice,
//                    postalcode = (string)args.postalcode,
//                    targetcompanyid = 1
//                };

//                // calculate delivery charge based on division [cite: 122, 273, 863-864]
//                var settings = _settingsfacade.getdeliverysettings(order.targetcompanyid);
//                bool isdhaka = order.divison.tolower().contains("dhaka") || order.city.tolower().contains("dhaka");
//                order.deliverycharge = isdhaka ? settings["dhaka"] : settings["outside"];

//                // execute business logic 
//                string orderno = await _orderfacade.placeguestorder(order);

//                return $"✅ success! i have placed your order. your order id is **{orderno}**. you will receive a confirmation sms/email shortly.";
//            }
//            catch (exception ex)
//            {
//                return $"❌ i encountered an error while placing the order: {ex.message}. please check your details and try again.";
//            }
//        }
//        private async task<string> getrelevantcontext(string message, int? activeproductid)
//        {
//            var lowermsg = message.tolower();
//            var context = new stringbuilder();

//            try
//            {
//                // resolve companyid (default to 1)
//                int companyid = 1;

//                // 1️ dynamic business knowledge (delivery & payment methods)
//                // this ensures the ai always knows the current prices and methods set in admin settings
//                var delivery = _settingsfacade.getdeliverysettings(companyid);
//                context.appendline("🚚 shipping information:");
//                context.appendline($"- inside dhaka: ৳{delivery["dhaka"]}");
//                context.appendline($"- outside dhaka: ৳{delivery["outside"]}");

//                var paymentmethods = _paymentfacade.getactivepaymentmethods(companyid);
//                if (paymentmethods != null && paymentmethods.any())
//                {
//                    context.appendline("\n💳 available payment methods:");
//                    foreach (var pm in paymentmethods)
//                    {
//                        context.appendline($"- {pm.methodname}");
//                    }
//                }

//                // 2️ page specific context (preserved existing logic)
//                if (activeproductid.hasvalue && activeproductid.value > 0)
//                {
//                    var pagecontext = await getpagespecificcontext(activeproductid.value);
//                    if (!string.isnullorempty(pagecontext))
//                    {
//                        context.appendline("\n🔴 current page context (the product the user is seeing):");
//                        context.appendline(pagecontext);
//                    }
//                }

//                // 3️ search logic 
//                if (!lowermsg.contains("this") && !lowermsg.contains("it") &&
//                    containsany(lowermsg, "product", "item", "search", "find", "price", "stock"))
//                {
//                    var productinfo = await getproductcontext(message);
//                    if (!string.isnullorempty(productinfo)) context.appendline(productinfo);
//                }

//                // 4️ order tracking (preserved existing logic)
//                if (regex.ismatch(lowermsg, @"(on|do)\d{8}"))
//                {
//                    var orderinfo = await getordercontext(message);
//                    if (!string.isnullorempty(orderinfo)) context.appendline(orderinfo);
//                }
//            }
//            catch (exception ex)
//            {
//                context.appendline($"note: dynamic data lookup limited ({ex.message})");
//            }

//            return context.tostring();
//        }
//        private async task<string> getpagespecificcontext(int productid)
//        {
//            var p = _productfacade.getproductdetails(productid);
//            if (p == null) return null;

//            var sb = new stringbuilder();
//            sb.appendline($"product: {p.productname}");

//            // ✅ fetch discount once for product
//            var bestdiscount = _productfacade.getbestdiscount(p.id, p.baseprice ?? 0);

//            var allattributes = _productfacade.getvariantattributes(productid);
//            if (p.variants != null && p.variants.any())
//            {
//                sb.appendline("variations:");
//                foreach (var v in p.variants)
//                {
//                    var myattrs = allattributes.where(a => a.variantid == v.id)
//                                               .select(a => $"{a.attributename}: {a.attributevalue}");

//                    // ✅ dynamic price calculation (sync with orderfacade) [cite: 23-26]
//                    decimal baseprice = v.variantprice ?? p.sellingprice;
//                    decimal calculatedprice = baseprice;

//                    if (bestdiscount != null)
//                    {
//                        if (bestdiscount.discounttype == "flat")
//                            calculatedprice -= bestdiscount.discountvalue;
//                        else if (bestdiscount.discounttype == "percentage")
//                            calculatedprice -= (baseprice * (bestdiscount.discountvalue / 100));
//                    }
//                    calculatedprice = math.max(calculatedprice, 0);

//                    string name = myattrs.any() ? string.join(", ", myattrs) : v.variantname;
//                    sb.appendline($"- {name}: ৳{calculatedprice:n0} [stock: {v.stockqty}, id: {v.id}]");
//                }
//            }
//            return sb.tostring();
//        }
//        private async task<string> getbusinessrulescontext(int companyid)
//        {
//            var sb = new stringbuilder();

//            // ✅ fetch delivery charges dynamically from db [cite: 698]
//            var delivery = _settingsfacade.getdeliverysettings(companyid);
//            sb.appendline("🚚 delivery charges:");
//            sb.appendline($"- inside dhaka: ৳{delivery["dhaka"]}");
//            sb.appendline($"- outside dhaka: ৳{delivery["outside"]}");

//            // ✅ fetch payment methods dynamically from db 
//            var payments = _settingsfacade.getcompanypaymentsettings(companyid);
//            var enabledpayments = payments.where(p => p.isenabled).tolist();

//            if (enabledpayments.any())
//            {
//                sb.appendline("\n💳 accepted payment methods:");
//                foreach (var pm in enabledpayments)
//                {
//                    string mode = pm.ismanualenabled && pm.isgatewayenabled ? "mobile banking & online gateway" :
//                                 pm.isgatewayenabled ? "online gateway" : "manual/cash";
//                    sb.appendline($"- {pm.methodname} ({mode})");
//                    if (!string.isnullorempty(pm.custominstruction))
//                        sb.appendline($"  instruction: {pm.custominstruction}"); // this shows the bkash number [cite: 729]
//                }
//            }

//            return sb.tostring();
//        }
//        // 📦 product context builder (corrected with attribute lookup)
//        private async task<string> getproductcontext(string query)
//        {
//            try
//            {
//                var searchterm = extractsearchterm(query);

//                // ✅ fix: pass currentcompanyid to restrict the search
//                var products = _productfacade.searchproducts(searchterm, currentcompanyid);

//                if (products == null || products.count == 0) return "";

//                var sb = new stringbuilder();
//                sb.appendline($"📦 **search results for '{searchterm}':**\n");

//                foreach (var p in products.take(3))
//                {
//                    // now 'p' is guaranteed to be from the correct company.
//                    // we can safely use p.id for variants/discounts.

//                    var bestdiscount = _productfacade.getbestdiscount(p.id, p.baseprice ?? 0);
//                    var variants = _productfacade.getvariantsbyproductid(p.id);
//                    var allattributes = _productfacade.getvariantattributes(p.id);

//                    sb.appendline($"product: {p.productname}");
//                    foreach (var v in variants)
//                    {
//                        var myattributes = allattributes
//                            .where(a => a.variantid == v.id)
//                            .select(a => $"{a.attributename}: {a.attributevalue}");

//                        // ⚠️ logic: calculate price (same as before)
//                        decimal baseprice = v.variantprice ?? p.baseprice ?? 0;
//                        decimal discountedprice = baseprice;

//                        if (bestdiscount != null)
//                        {
//                            if (bestdiscount.discounttype == "flat")
//                                discountedprice -= bestdiscount.discountvalue;
//                            else if (bestdiscount.discounttype == "percentage")
//                                discountedprice -= (baseprice * (bestdiscount.discountvalue / 100));
//                        }
//                        discountedprice = math.max(discountedprice, 0);

//                        string name = myattributes.any() ? string.join(", ", myattributes) : (v.variantname ?? "option");
//                        sb.appendline($" - [{name}]: ৳{discountedprice:n0} (stock: {v.stockqty})");
//                    }
//                    sb.appendline();
//                }
//                return sb.tostring();
//            }
//            catch (exception ex)
//            {
//                return $"error fetching price details: {ex.message}";
//            }
//        }        // 📋 order tracking context
//        private async task<string> getordercontext(string message)
//        {
//            try
//            {
//                // extract order id (format: on12345678 or do12345678)
//                var orderidmatch = regex.match(
//                    message,
//                    @"(on|do)\d{8}",
//                    regexoptions.ignorecase);

//                if (!orderidmatch.success)
//                    return "💡 to track your order, please provide your order id (e.g., on12345678 or do12345678)";

//                string orderid = orderidmatch.value.toupper();

//                // fetch order details
//                var orderdetails = _orderfacade.getorderreceiptbyonlineid(orderid);

//                if (orderdetails == null || orderdetails.count == 0)
//                    return $"❌ order {orderid} not found. please verify the order id.";

//                var order = orderdetails[0] as dynamic;

//                var sb = new stringbuilder();
//                sb.appendline($"📦 **order {orderid} status:**\n");
//                sb.appendline($"status: {order.status}");
//                sb.appendline($"order date: {convert.todatetime(order.orderdate):dd mmm yyyy}");
//                sb.appendline($"total amount: ৳{order.totalamount:n0}");

//                if (order.status == "shipped" || order.status == "delivered")
//                    sb.appendline($"delivery: expected in 2-5 business days");

//                return sb.tostring();
//            }
//            catch (exception ex)
//            {
//                return $"error tracking order: {ex.message}";
//            }
//        }

//        // 🔥 trending products
//        private string gettrendingproducts()
//        {
//            try
//            {
//                var sb = new stringbuilder();

//                // ✅ fix: use currentcompanyid to ensure we only get this shop's products.
//                // also, stop using searchproducts(" ") hack. use the direct method instead.
//                var products = _productfacade.getallproductswithcategory(currentcompanyid);

//                if (products == null || products.count == 0)
//                    return "";

//                sb.appendline("🔥 **top available products:**\n");

//                // take top 10 recent products
//                foreach (var p in products.take(10))
//                {
//                    // note: sellingprice is already calculated inside getallproductswithcategory
//                    sb.appendline($"• {p.productname} - ৳{p.sellingprice:n0}");
//                }

//                return sb.tostring();
//            }
//            catch (exception ex)
//            {
//                return $"error fetching list: {ex.message}";
//            }
//        }
//        private string extractsearchterm(string message)
//        {
//            // 1. clean the message first (to lower + remove punctuation)
//            var sb = new stringbuilder();
//            foreach (char c in message.tolower())
//            {
//                sb.append(char.ispunctuation(c) ? ' ' : c);
//            }
//            string cleanmessage = sb.tostring();

//            // 2. define the "stop words" (words to completely delete)
//            var stopwords = new hashset<string>
//            {
//                "show", "me", "find", "search", "looking", "look", "for", "want", "need", "get",
//                "do", "you", "have", "is", "are", "can", "i", "buy", "purchase", "shop",
//                "price", "cost", "rate", "amount", "how", "much",
//                "stock", "available", "availability", "status", "count", "left", "many",
//                "details", "info", "information", "about", "desc", "description",
//                "product", "item", "unit", "article", "of", "the", "a", "an", "this", "that"
//            };

//            // 3. split into words and filter
//            var words = cleanmessage.split(new[] { ' ' }, stringsplitoptions.removeemptyentries);

//            var validwords = new list<string>();
//            foreach (var word in words)
//            {
//                if (!stopwords.contains(word))
//                {
//                    validwords.add(word);
//                }
//            }

//            return string.join(" ", validwords).trim();
//        }

//        private bool containsany(string text, params string[] keywords)
//        {
//            return keywords.any(k => text.contains(k));
//        }

//        // 🚨 helper: detect if ai wants human takeover
//        private bool containshandofftrigger(string airesponse)
//        {
//            if (string.isnullorempty(airesponse)) return false;

//            var triggers = new[] {
//                "support team",
//                "human agent",
//                "connect you",
//                "speak with someone",
//                "can't help with that",
//                "beyond my capability"
//            };

//            return triggers.any(t => airesponse.tolower().contains(t));
//        }
//    }
//}