using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;             // Required for SqlConnection
using Microsoft.Extensions.Configuration; // ✅ Required for appsettings.json access

namespace MDUA.Facade
{
    public class OrderFacade : IOrderFacade
    {
        private readonly ISalesOrderHeaderDataAccess _salesOrderHeaderDataAccess;
        private readonly ISalesOrderDetailDataAccess _salesOrderDetailDataAccess;
        private readonly ICustomerDataAccess _customerDataAccess;
        private readonly ICompanyCustomerDataAccess _companyCustomerDataAccess;
        private readonly IAddressDataAccess _addressDataAccess;
        private readonly IProductVariantDataAccess _productVariantDataAccess;
        private readonly IProductFacade _productFacade;
        private readonly IPostalCodesDataAccess _postalCodesDataAccess;
        private readonly ISettingsFacade _settingsFacade;

        // ✅ 1. Declare Configuration to access appsettings.json
        private readonly IConfiguration _configuration;
        private readonly IDeliveryDataAccess _deliveryDataAccess;
        public OrderFacade(
            ISalesOrderHeaderDataAccess salesOrderHeaderDataAccess,
            ISalesOrderDetailDataAccess salesOrderDetailDataAccess,
            ICustomerDataAccess customerDataAccess,
            ICompanyCustomerDataAccess companyCustomerDataAccess,
            IAddressDataAccess addressDataAccess,
            IProductVariantDataAccess productVariantDataAccess,
            IProductFacade productFacade,
            IPostalCodesDataAccess postalCodesDataAccess,

            IConfiguration configuration
            ,
            ISettingsFacade settingsFacade,
            IDeliveryDataAccess deliveryDataAccess)
        {
            _salesOrderHeaderDataAccess = salesOrderHeaderDataAccess;
            _salesOrderDetailDataAccess = salesOrderDetailDataAccess;
            _customerDataAccess = customerDataAccess;
            _companyCustomerDataAccess = companyCustomerDataAccess;
            _addressDataAccess = addressDataAccess;
            _productVariantDataAccess = productVariantDataAccess;
            _productFacade = productFacade;
            _postalCodesDataAccess = postalCodesDataAccess;
            _configuration = configuration;
            _settingsFacade = settingsFacade;
            _deliveryDataAccess = deliveryDataAccess;

        }

        #region Common Implementation
        public long Delete(int id) => _salesOrderHeaderDataAccess.Delete(id);
        public SalesOrderHeader Get(int id) => _salesOrderHeaderDataAccess.Get(id);
        public SalesOrderHeaderList GetAll() => _salesOrderHeaderDataAccess.GetAll();
        public SalesOrderHeaderList GetByQuery(string query) => _salesOrderHeaderDataAccess.GetByQuery(query);
        public long Insert(SalesOrderHeaderBase obj) => _salesOrderHeaderDataAccess.Insert(obj);
        public long Update(SalesOrderHeaderBase obj) => _salesOrderHeaderDataAccess.Update(obj);
        #endregion

        #region Extended Implementation

        public Customer GetCustomerByPhone(string phone) => _customerDataAccess.GetByPhone(phone);
        public PostalCodes GetPostalCodeDetails(string code) => _postalCodesDataAccess.GetPostalCodeDetails(code);
        public Customer GetCustomerByEmail(string email) => _customerDataAccess.GetByEmail(email);
        public List<string> GetDivisions() => _postalCodesDataAccess.GetDivisions();

        public List<string> GetDistricts(string division) => _postalCodesDataAccess.GetDistricts(division);

        public List<string> GetThanas(string district) => _postalCodesDataAccess.GetThanas(district);

        public List<dynamic> GetSubOffices(string thana) => _postalCodesDataAccess.GetSubOffices(thana);


        public string PlaceGuestOrder(SalesOrderHeader orderData)
        {
            // 1. PRE-CALCULATION (Read-Only)
            var variant = _productVariantDataAccess.GetWithStock(orderData.ProductVariantId);
            if (variant == null) throw new Exception("Variant not found.");

            if (variant.StockQty == 0)
                throw new Exception("The selected product variant is currently out of stock.");

            if (variant.StockQty < orderData.OrderQuantity)
                throw new Exception($"Requested amount {orderData.OrderQuantity} exceeds available amount {variant.StockQty}.");

            decimal baseVariantPrice = variant.VariantPrice ?? 0;
            int quantity = orderData.OrderQuantity;

            // Discount Calculation
            var bestDiscount = _productFacade.GetBestDiscount(variant.ProductId, baseVariantPrice);
            decimal finalUnitPrice = baseVariantPrice;
            decimal totalDiscountAmount = 0;

            if (bestDiscount != null)
            {
                if (bestDiscount.DiscountType == "Flat")
                {
                    decimal discountPerItem = bestDiscount.DiscountValue;
                    finalUnitPrice -= discountPerItem;
                    totalDiscountAmount = discountPerItem * quantity;
                }
                else if (bestDiscount.DiscountType == "Percentage")
                {
                    decimal discountRate = bestDiscount.DiscountValue / 100;
                    decimal discountPerItem = baseVariantPrice * discountRate;
                    finalUnitPrice -= discountPerItem;
                    totalDiscountAmount = discountPerItem * quantity;
                }
            }

            // ✅ CALCULATION LOGIC FOR COMPUTED COLUMN
            // 1. Calculate Pure Product Cost
            decimal totalProductPrice = baseVariantPrice * quantity;

            // 2. Add Delivery to Total (This hacks the DB to make NetAmount correct)
            // DB Formula: NetAmount = TotalAmount - DiscountAmount
            // We want: NetAmount = (Product + Delivery) - Discount
            // Therefore: TotalAmount MUST BE = (Product + Delivery)

            decimal deliveryCharge = orderData.DeliveryCharge; // Captured from UI
            orderData.TotalAmount = totalProductPrice + deliveryCharge;
            orderData.DiscountAmount = totalDiscountAmount;

            // 2. TRANSACTIONAL SAVE
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var transCustomerDA = new CustomerDataAccess(transaction);
                        var transCompanyCustomerDA = new CompanyCustomerDataAccess(transaction);
                        var transAddressDA = new AddressDataAccess(transaction);
                        var transOrderDA = new SalesOrderHeaderDataAccess(transaction);
                        var transDetailDA = new SalesOrderDetailDataAccess(transaction);

                        int companyId = orderData.TargetCompanyId;
                        int customerId = 0;

                        // A. Customer Logic
                        var customer = transCustomerDA.GetByPhone(orderData.CustomerPhone);
                        if (customer == null)
                        {
                            string emailToCheck = !string.IsNullOrEmpty(orderData.CustomerEmail)
                                ? orderData.CustomerEmail
                                : $"{orderData.CustomerPhone}@guest.local";

                            if (transCustomerDA.GetByEmail(emailToCheck) != null)
                                throw new Exception($"Email {emailToCheck} is already registered.");

                            var newCust = new Customer
                            {
                                CustomerName = orderData.CustomerName,
                                Phone = orderData.CustomerPhone,
                                Email = emailToCheck,
                                IsActive = true,
                                CreatedAt = DateTime.Now,
                                CreatedBy = "System_Order"
                            };
                            transCustomerDA.Insert(newCust);
                            customer = transCustomerDA.GetByPhone(orderData.CustomerPhone);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(orderData.CustomerEmail) &&
                                customer.Email != orderData.CustomerEmail &&
                                (string.IsNullOrEmpty(customer.Email) || customer.Email.EndsWith("@guest.local")))
                            {
                                customer.Email = orderData.CustomerEmail;
                                transCustomerDA.Update(customer);
                            }
                        }
                        customerId = customer.Id;

                        // B. Link Logic
                        if (!transCompanyCustomerDA.IsLinked(companyId, customerId))
                        {
                            transCompanyCustomerDA.Insert(new CompanyCustomer { CompanyId = companyId, CustomerId = customerId });
                        }

                        // C. Address Logic
                        var addr = new Address
                        {
                            CustomerId = customerId,
                            Street = orderData.Street,
                            City = orderData.City,
                            Divison = orderData.Divison,
                            Thana = orderData.Thana,
                            SubOffice = orderData.SubOffice,
                            Country = "Bangladesh",
                            AddressType = "Shipping",
                            CreatedBy = "System_Order",
                            CreatedAt = DateTime.Now,
                            PostalCode = orderData.PostalCode ?? "0000",
                            ZipCode = (orderData.ZipCode ?? orderData.PostalCode ?? "0000").ToCharArray()
                        };

                        var existingAddress = transAddressDA.CheckExistingAddress(customerId, addr);
                        int addressId = (existingAddress != null) ? existingAddress.Id : (int)transAddressDA.InsertAddressSafe(addr);

                        // D. Save Order Header
                        orderData.CompanyCustomerId = transCompanyCustomerDA.GetId(companyId, customerId);
                        orderData.AddressId = addressId;
                        orderData.SalesChannelId = 1;
                        orderData.OrderDate = DateTime.Now;
                        orderData.Status = "Draft";
                        orderData.IsActive = true;
                        orderData.CreatedBy = "System_Order";
                        orderData.CreatedAt = DateTime.Now;
                        orderData.Confirmed = false;

                        // Call InsertSafe (This uses the TotalAmount calculated above)
                        int orderId = (int)transOrderDA.InsertSalesOrderHeaderSafe(orderData);

                        if (orderId <= 0) throw new Exception("Failed to create Order Header.");

                        // E. Save Order Detail
                        var detail = new SalesOrderDetail
                        {
                            SalesOrderId = orderId,
                            ProductVariantId = orderData.ProductVariantId,
                            Quantity = orderData.OrderQuantity,
                            UnitPrice = finalUnitPrice,
                            CreatedBy = "System_Order",
                            CreatedAt = DateTime.Now
                        };
                        transDetailDA.InsertSalesOrderDetailSafe(detail);

                        transaction.Commit();
                        return "ON" + orderId.ToString("D8");
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public (Customer customer, Address address) GetCustomerDetailsForAutofill(string phone)
        {
            var customer = _customerDataAccess.GetByPhone(phone);
            Address address = null;

            if (customer != null)
            {
                address = _addressDataAccess.GetLatestByCustomerId(customer.Id);
            }
            return (customer, address);
        }

        public List<object> GetOrderReceiptByOnlineId(string onlineOrderId)
        {
            if (string.IsNullOrEmpty(onlineOrderId))
            {
                throw new ArgumentException("Online Order ID cannot be null or empty.", nameof(onlineOrderId));
            }
            return _salesOrderHeaderDataAccess.GetOrderReceiptByOnlineId(onlineOrderId);
        }

        // ==========================================================================
        // ✅ 1. CORRECTED: GetAllOrdersForAdmin (Revenue Stability Logic)
        // ==========================================================================
        public List<SalesOrderHeader> GetAllOrdersForAdmin()
        {
            var orders = _salesOrderHeaderDataAccess.GetAllSalesOrderHeaders().ToList();

            foreach (var order in orders)
            {
                // 1. Calculate Sum of Discounted Items (Net Product Total)
                decimal productNetTotal = _salesOrderHeaderDataAccess.GetProductTotalFromDetails(order.Id);

                if (order.TotalAmount > 0)
                {
                    // ✅ CORRECT FORMULA: 
                    // Delivery = Total(Gross) - Products(Net) - Discount
                    // Example: 1675 - 1101 - 449 = 125
                    order.DeliveryCharge = order.TotalAmount - productNetTotal - order.DiscountAmount;
                }
                else
                {
                    order.DeliveryCharge = 0;
                }

                // --- PROFIT CALCULATION (Optional, for internal use) ---
                // order.ActualLogisticsCost is already populated by DataAccess

                // --- DUE CALCULATION ---
                decimal net = order.NetAmount ?? 0m;
                decimal paid = order.PaidAmount;
                order.DueAmount = net - paid;
            }

            return orders;
        }        // ==========================================================================
                 // ✅ 2. CORRECTED: UpdateOrderConfirmation (Expense Snapshot Logic)
                 // ==========================================================================
        public string UpdateOrderConfirmation(int orderId, bool isConfirmed)
        {
            // 1. Update Header (We know this works)
            string dbStatus = isConfirmed ? "Confirmed" : "Draft";
            _salesOrderHeaderDataAccess.UpdateStatusSafe(orderId, dbStatus, isConfirmed);

            // 2. Delivery Logic with DEBUGGING
            if (isConfirmed)
            {
                try
                {
                    // DEBUG LINE: Force a check to see if we are about to call the DA
                    // If this throws, we know we entered the block.
                    // throw new Exception("DEBUG: Entered Delivery Block"); 

                    // Call the Extended method
                    var existingDelivery = _deliveryDataAccess.GetBySalesOrderIdExtended(orderId);

                    if (existingDelivery == null)
                    {
                        // ... (Your existing settings logic) ...
                        // For debug simplicity, let's hardcode cost momentarily to rule out SettingsFacade errors
                        decimal actualCost = 60;

                        var delivery = new Delivery
                        {
                            SalesOrderId = orderId,
                            TrackingNumber = "TRK-" + DateTime.Now.Ticks.ToString().Substring(12),
                            Status = "Pending",
                            ShippingCost = actualCost,
                            CreatedBy = "System_Confirm",
                            CreatedAt = DateTime.Now
                        };

                        _deliveryDataAccess.InsertExtended(delivery);
                    }
                }
                catch (Exception ex)
                {
                    // This captures the specific error inside delivery logic
                    throw new Exception($"DELIVERY_CRASH: {ex.Message} | Stack: {ex.StackTrace}");
                }
            }

            return isConfirmed ? "Confirmed" : "Pending";
        }
        // ==========================================================================
        // ✅ 3. NEW: UpdateDeliveryStatus (Status Sync Logic)
        // ==========================================================================
        // In src/MDUA.Facade/OrderFacade.cs

        public void UpdateDeliveryStatus(int deliveryId, string newStatus)
        {
            // 1. Fetch using EXTENDED method (Safe mapping)
            // 🛑 WAS: var delivery = _deliveryDataAccess.Get(deliveryId); 
            // ✅ FIX: Use GetExtended to avoid "DateTime to String" casting errors
            var delivery = _deliveryDataAccess.GetExtended(deliveryId);

            if (delivery == null) throw new Exception("Delivery record not found.");

            // 2. Update Logistical Status
            delivery.Status = newStatus;

            // Auto-set ActualDeliveryDate if delivered
            if (newStatus.Equals("Delivered", StringComparison.OrdinalIgnoreCase))
            {
                delivery.ActualDeliveryDate = DateTime.Now;
            }

            _deliveryDataAccess.UpdateExtended(delivery);

            // 3. Map to Commercial Status (SalesOrderHeader)
            string parentStatus = null;
            switch (newStatus.ToLower())
            {
                case "shipped":
                case "in transit":
                case "out for delivery":
                    parentStatus = "Shipped";
                    break;
                case "delivered":
                    parentStatus = "Delivered";
                    break;
                case "returned":
                case "returned to hub":
                    parentStatus = "Returned";
                    break;
                case "cancelled":
                    parentStatus = "Cancelled";
                    break;
            }

            // 4. Sync Parent
            if (parentStatus != null)
            {
                _salesOrderHeaderDataAccess.UpdateStatusSafe(delivery.SalesOrderId, parentStatus, true);
            }
        }

        //facade
        public void UpdateOrderStatus(int orderId, string newStatus)

        {

            // 1. REMOVE THIS LINE (This is what crashes):

            // var order = _salesOrderHeaderDataAccess.Get(orderId);

            // 2. Determine 'Confirmed' status logically

            // If we are Cancelling, unconfirm it. Otherwise, keep it confirmed (or confirm it).

            bool confirmedState = true;

            if (newStatus == "Cancelled" || newStatus == "Draft")

            {

                confirmedState = false;

            }

            // 3. Call the safe update method directly (Just like ToggleConfirmation does)

            _salesOrderHeaderDataAccess.UpdateStatusSafe(orderId, newStatus, confirmedState);

        }



        public List<dynamic> GetProductVariantsForAdmin()
        {
            var rawList = _salesOrderHeaderDataAccess.GetVariantsForDropdown();

            // Loop through and attach discount info from ProductFacade
            foreach (var item in rawList)
            {
                if (item.ContainsKey("ProductId") && item.ContainsKey("Price"))
                {
                    int pId = (int)item["ProductId"];
                    decimal price = (decimal)item["Price"];

                    var bestDiscount = _productFacade.GetBestDiscount(pId, price);

                    if (bestDiscount != null)
                    {
                        item["DiscountType"] = bestDiscount.DiscountType; // "Flat" or "Percentage"
                        item["DiscountValue"] = bestDiscount.DiscountValue;
                    }
                    else
                    {
                        item["DiscountType"] = "None";
                        item["DiscountValue"] = 0m;
                    }
                }
            }

            return new List<dynamic>(rawList);
        }

        // Change
        public dynamic PlaceAdminOrder(SalesOrderHeader orderData)
        {
            // 1. Stock Check
            var variantInfo = _salesOrderHeaderDataAccess.GetVariantStockAndPrice(orderData.ProductVariantId);
            if (variantInfo == null) throw new Exception("Variant not found.");

            if (variantInfo.Value.StockQty < orderData.OrderQuantity)
                throw new Exception($"Stock Error: Only {variantInfo.Value.StockQty} available.");

            decimal basePrice = variantInfo.Value.Price;

            // 2. Discount Calculation
            var variantBasic = _productVariantDataAccess.Get(orderData.ProductVariantId);
            decimal finalUnitPrice = basePrice;
            decimal totalDiscount = 0;

            var bestDiscount = _productFacade.GetBestDiscount(variantBasic.ProductId, basePrice);
            if (bestDiscount != null)
            {
                if (bestDiscount.DiscountType == "Flat")
                {
                    finalUnitPrice -= bestDiscount.DiscountValue;
                    totalDiscount = bestDiscount.DiscountValue * orderData.OrderQuantity;
                }
                else if (bestDiscount.DiscountType == "Percentage")
                {
                    decimal disc = basePrice * (bestDiscount.DiscountValue / 100);
                    finalUnitPrice -= disc;
                    totalDiscount = disc * orderData.OrderQuantity;
                }
            }

            // -------------------------------------------------------------
            // ✅ DELIVERY FEE LOGIC (Fetch from DB Settings)
            // -------------------------------------------------------------
            int companyId = orderData.TargetCompanyId > 0 ? orderData.TargetCompanyId : 1;
            var settings = _settingsFacade.GetDeliverySettings(companyId) ?? new Dictionary<string, int>();

            // A. Detect "In-Store" vs "Home Delivery"
            // (We rely on the Street naming convention from your JS)
            bool isStoreSale = !string.IsNullOrEmpty(orderData.Street) &&
                               orderData.Street.IndexOf("Counter Sale", StringComparison.OrdinalIgnoreCase) >= 0;

            decimal deliveryFeeToCharge = 0; // Default for Store Sale

            // B. Calculate Fee if Home Delivery
            if (!isStoreSale)
            {
                bool isDhaka = (!string.IsNullOrEmpty(orderData.Divison) &&
                                orderData.Divison.IndexOf("dhaka", StringComparison.OrdinalIgnoreCase) >= 0)
                               || (!string.IsNullOrEmpty(orderData.City) &&
                                orderData.City.IndexOf("dhaka", StringComparison.OrdinalIgnoreCase) >= 0);

                // Fetch Customer Price (Revenue) from Settings
                int dhakaFee = settings.ContainsKey("dhaka") ? settings["dhaka"] : 60;
                int outsideFee = settings.ContainsKey("outside") ? settings["outside"] : 120;

                deliveryFeeToCharge = isDhaka ? dhakaFee : outsideFee;
            }

            // C. Apply to Totals
            decimal grossProductCost = basePrice * orderData.OrderQuantity;

            // DB Logic: [NetAmount] = [TotalAmount] - [DiscountAmount]
            // We want: [NetAmount] = (Product + Delivery) - Discount
            orderData.TotalAmount = grossProductCost + deliveryFeeToCharge;
            orderData.DiscountAmount = totalDiscount;

            // For Return Object
            decimal netAmount = orderData.TotalAmount - totalDiscount;

            // -------------------------------------------------------------
            // 3. SAVE TO DATABASE
            // -------------------------------------------------------------
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var transCustomerDA = new CustomerDataAccess(transaction);
                        var transCompanyCustomerDA = new CompanyCustomerDataAccess(transaction);
                        var transAddressDA = new AddressDataAccess(transaction);
                        var transOrderDA = new SalesOrderHeaderDataAccess(transaction);
                        var transDetailDA = new SalesOrderDetailDataAccess(transaction);

                        // ✅ We need DeliveryDataAccess in the transaction context for the snapshot
                        var transDeliveryDA = new DeliveryDataAccess(transaction);

                        // A. Customer Logic
                        int customerId = 0;
                        var customer = transCustomerDA.GetByPhone(orderData.CustomerPhone);
                        string finalEmail = !string.IsNullOrEmpty(orderData.CustomerEmail)
                            ? orderData.CustomerEmail
                            : $"{orderData.CustomerPhone}@direct.local";

                        if (customer == null)
                        {
                            var newCust = new Customer
                            {
                                CustomerName = orderData.CustomerName,
                                Phone = orderData.CustomerPhone,
                                Email = finalEmail,
                                IsActive = true,
                                CreatedAt = DateTime.Now,
                                CreatedBy = "Admin"
                            };
                            transCustomerDA.Insert(newCust);
                            customer = transCustomerDA.GetByPhone(orderData.CustomerPhone);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(orderData.CustomerEmail) &&
                                (string.IsNullOrEmpty(customer.Email) || customer.Email.EndsWith(".local")))
                            {
                                customer.Email = orderData.CustomerEmail;
                                transCustomerDA.Update(customer);
                            }
                        }
                        customerId = customer.Id;

                        // B. Link
                        if (!transCompanyCustomerDA.IsLinked(orderData.TargetCompanyId, customerId))
                        {
                            transCompanyCustomerDA.Insert(new CompanyCustomer { CompanyId = orderData.TargetCompanyId, CustomerId = customerId });
                        }

                        // C. Address
                        var addr = new Address
                        {
                            CustomerId = customerId,
                            Street = orderData.Street,
                            City = orderData.City,
                            Divison = orderData.Divison,
                            Thana = orderData.Thana,
                            SubOffice = orderData.SubOffice,
                            Country = "Bangladesh",
                            AddressType = "Shipping",
                            CreatedBy = "Admin",
                            CreatedAt = DateTime.Now,
                            PostalCode = orderData.PostalCode ?? "0000",
                            ZipCode = (orderData.ZipCode ?? "0000").ToCharArray()
                        };
                        var existingAddr = transAddressDA.CheckExistingAddress(customerId, addr);
                        int addressId = (existingAddr != null) ? existingAddr.Id : (int)transAddressDA.InsertAddressSafe(addr);

                        // D. Order Header
                        orderData.CompanyCustomerId = transCompanyCustomerDA.GetId(orderData.TargetCompanyId, customerId);
                        orderData.AddressId = addressId;
                        orderData.SalesChannelId = 2; // Direct
                        orderData.OrderDate = DateTime.Now;
                        orderData.Status = orderData.Confirmed ? "Confirmed" : "Draft";
                        orderData.IsActive = true;
                        orderData.CreatedBy = "Admin";
                        orderData.CreatedAt = DateTime.Now;

                        int orderId = (int)transOrderDA.InsertSalesOrderHeaderSafe(orderData);

                        // E. Order Detail
                        transDetailDA.InsertSalesOrderDetailSafe(new SalesOrderDetail
                        {
                            SalesOrderId = orderId,
                            ProductVariantId = orderData.ProductVariantId,
                            Quantity = orderData.OrderQuantity,
                            UnitPrice = finalUnitPrice, // Store Net Unit Price
                            CreatedBy = "Admin",
                            CreatedAt = DateTime.Now
                        });

                        // ------------------------------------------------------------------
                        // ✅ 4. EXPENSE SNAPSHOT (If Confirmed, Freeze Cost in Delivery Table)
                        // ------------------------------------------------------------------
                        // Note: Only create delivery record if it's NOT a Store Pickup
                        if (orderData.Confirmed && !isStoreSale)
                        {
                            bool isDhaka = (!string.IsNullOrEmpty(orderData.Divison) &&
                                           orderData.Divison.IndexOf("dhaka", StringComparison.OrdinalIgnoreCase) >= 0);

                            // Fetch ACTUAL COST (Expense) from Settings
                            decimal costInside = settings.ContainsKey("Cost_InsideDhaka") ? settings["Cost_InsideDhaka"] : 60;
                            decimal costOutside = settings.ContainsKey("Cost_OutsideDhaka") ? settings["Cost_OutsideDhaka"] : 120;

                            decimal actualCost = isDhaka ? costInside : costOutside;

                            var delivery = new Delivery
                            {
                                SalesOrderId = orderId,
                                TrackingNumber = "DO-" + DateTime.Now.Ticks.ToString().Substring(12),
                                Status = "Pending",
                                ShippingCost = actualCost, // ✅ EXPENSE FROZEN
                                CreatedBy = "Admin_Direct",
                                CreatedAt = DateTime.Now
                            };

                            // Use the Extended Insert logic we fixed earlier
                            transDeliveryDA.InsertExtended(delivery);
                        }

                        transaction.Commit();

                        return new
                        {
                            OrderId = "DO" + orderId.ToString("D8"),
                            NetAmount = netAmount,
                            DiscountAmount = totalDiscount,
                            TotalAmount = orderData.TotalAmount,
                            DeliveryFee = deliveryFeeToCharge // Return what we calculated
                        };
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }        //new
        public DashboardStats GetDashboardMetrics()
        {
            return _salesOrderHeaderDataAccess.GetDashboardStats();
        }

        //new
        public List<SalesOrderHeader> GetRecentOrders()
        {
            return _salesOrderHeaderDataAccess.GetRecentOrders(5); // Get top 5
        }
        //new
        public List<ChartDataPoint> GetSalesTrend()
        {
            return _salesOrderHeaderDataAccess.GetSalesTrend(6);
        }
//new
        public List<ChartDataPoint> GetOrderStatusCounts()
        {
            return _salesOrderHeaderDataAccess.GetOrderStatusCounts();
        }
        #endregion
    }
}