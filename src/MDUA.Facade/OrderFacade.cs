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
            // ✅ 0. VALIDATION: Ensure Customer Name is present
            if (string.IsNullOrWhiteSpace(orderData.CustomerName))
                throw new Exception("Customer Name is required to place an order.");

            // Clean the input
            orderData.CustomerName = orderData.CustomerName.Trim();

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
                                CustomerName = orderData.CustomerName, // ✅ Use Name Here
                                Phone = orderData.CustomerPhone,
                                Email = emailToCheck,
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = orderData.CustomerName, // ✅ Audit: Created by the Guest Name
                            };
                            transCustomerDA.Insert(newCust);
                            customer = transCustomerDA.GetByPhone(orderData.CustomerPhone);
                        }
                        else
                        {
                            bool needUpdate = false;

                            // ✅ Update Logic: If existing customer has no name, update it with current name
                            if (string.IsNullOrEmpty(customer.CustomerName) || customer.CustomerName == "Unknown")
                            {
                                customer.CustomerName = orderData.CustomerName;
                                needUpdate = true;
                            }

                            if (!string.IsNullOrEmpty(orderData.CustomerEmail) &&
                                customer.Email != orderData.CustomerEmail &&
                                (string.IsNullOrEmpty(customer.Email) || customer.Email.EndsWith("@guest.local")))
                            {
                                customer.Email = orderData.CustomerEmail;
                                needUpdate = true;
                            }

                            if (needUpdate)
                            {
                                customer.UpdatedBy = orderData.CustomerName; // ✅ Audit update
                                customer.UpdatedAt = DateTime.UtcNow;
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
                            CreatedBy = orderData.CustomerName, // ✅ Audit: Use Name instead of "System_Order"
                            CreatedAt = DateTime.UtcNow,
                            PostalCode = orderData.PostalCode ?? "0000",
                            ZipCode = (orderData.ZipCode ?? orderData.PostalCode ?? "0000").ToCharArray()
                        };

                        var existingAddress = transAddressDA.CheckExistingAddress(customerId, addr);
                        int addressId = (existingAddress != null) ? existingAddress.Id : (int)transAddressDA.InsertAddressSafe(addr);

                        // D. Save Order Header
                        orderData.CompanyCustomerId = transCompanyCustomerDA.GetId(companyId, customerId);
                        orderData.AddressId = addressId;
                        orderData.SalesChannelId = 1;
                        orderData.OrderDate = DateTime.UtcNow;
                        orderData.Status = "Draft";
                        orderData.IsActive = true;
                        orderData.CreatedBy = orderData.CustomerName; // ✅ Audit: Use Name instead of "System_Order"
                        orderData.CreatedAt = DateTime.UtcNow;
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
                            CreatedBy = orderData.CustomerName, // ✅ Audit: Use Name instead of "System_Order"
                            CreatedAt = DateTime.UtcNow
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
                 // Inside MDUA.Facade/OrderFacade.cs

        // 1. Update signature to accept 'username'
        public string UpdateOrderConfirmation(int orderId, bool isConfirmed, string username)
        {
            string dbStatus = isConfirmed ? "Confirmed" : "Draft";

            using (var scope = new System.Transactions.TransactionScope())
            {
                try
                {
                    // Update Status
                    _salesOrderHeaderDataAccess.UpdateStatusSafe(orderId, dbStatus, isConfirmed);

                    if (isConfirmed)
                    {
                        var existingDelivery = _deliveryDataAccess.GetBySalesOrderIdExtended(orderId);

                        if (existingDelivery == null)
                        {
                            // --- 1. CALCULATE SHIPPING COST ---

                            // A. Get the Order Header to access TotalAmount & DiscountAmount
                            var order = _salesOrderHeaderDataAccess.GetOrderTotalsSafe(orderId);
                            if (order == null) throw new Exception("Order header not found.");

                            // B. Get Product Net Total (Sum of items)
                            // Assuming this method exists in your DAL based on your snippet
                            decimal productNetTotal = _salesOrderHeaderDataAccess.GetProductTotalFromDetails(orderId);

                            // C. Apply Formula: Delivery = Total - Products - Discount
                            decimal calculatedDeliveryCost = order.TotalAmount - productNetTotal - order.DiscountAmount;

                            // Safety check: Cost shouldn't be negative
                            if (calculatedDeliveryCost < 0) calculatedDeliveryCost = 0;

                            // --- 2. CREATE DELIVERY RECORD ---
                            var delivery = new Delivery
                            {
                                SalesOrderId = orderId,
                                TrackingNumber = "TRK-" + DateTime.UtcNow.Ticks.ToString().Substring(12),
                                Status = "Pending",

                                // ✅ Use Calculated Cost
                                ShippingCost = calculatedDeliveryCost,

                                // ✅ Use Logged-in User
                                CreatedBy = username,
                                CreatedAt = DateTime.UtcNow
                            };

                            long newDeliveryId = _deliveryDataAccess.InsertExtended(delivery);

                            // --- 3. INSERT ITEMS ---
                            if (newDeliveryId > 0)
                            {
                                // ✅ TRICK: Cast the interface to the concrete class
                                // This bypasses the "Interface does not contain definition" error
                                // because we are telling the compiler "Trust me, it's this specific class."
                                var concreteDetailDA = (SalesOrderDetailDataAccess)_salesOrderDetailDataAccess;

                                // Now you can call the method you added to the class!
                                var orderItems = concreteDetailDA.GetOrderDetailsSafe(orderId);
                                foreach (var item in orderItems)
                                {
                                    _deliveryDataAccess.InsertDeliveryItem(
                                        (int)newDeliveryId,
                                        item.Id,
                                        item.Quantity
                                    );
                                }
                            }
                        }
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw new Exception($"ORDER_CONFIRM_ERROR: {ex.Message}");
                }
            }

            return dbStatus;
        }

        // ✅ 3. NEW: UpdateDeliveryStatus (Status Sync Logic)
        // ==========================================================================
        // In src/MDUA.Facade/OrderFacade.cs

        public void UpdateDeliveryStatus(int deliveryId, string newStatus)
        {

            var delivery = _deliveryDataAccess.GetExtended(deliveryId);

            if (delivery == null) throw new Exception("Delivery not found");
            if (delivery.SalesOrderId <= 0) throw new Exception("Data Error: Delivery has no Sales Order ID.");

            // 2) Update Delivery
            delivery.Status = newStatus;
            if (newStatus.Equals("Delivered", StringComparison.OrdinalIgnoreCase))
                delivery.ActualDeliveryDate = DateTime.UtcNow;

            _deliveryDataAccess.UpdateExtended(delivery);

            // 3) Map Delivery -> SOH.Status
            string parentStatus = null;
            string cleanStatus = (newStatus ?? "").ToLower().Trim();

            if (cleanStatus == "pending") parentStatus = "Draft";
            else if (cleanStatus == "shipped" || cleanStatus == "in transit" || cleanStatus == "out for delivery") parentStatus = "Shipped";
            else if (cleanStatus == "delivered") parentStatus = "Delivered";
            else if (cleanStatus == "returned" || cleanStatus == "returned to hub") parentStatus = "Returned";
            else if (cleanStatus == "cancelled") parentStatus = "Cancelled";


            // 4) Sync SalesOrderHeader
            // 4. Update SalesOrderHeader (Sync)
            if (parentStatus != null)
            {
                bool confirmedState = false;

                if (_salesOrderHeaderDataAccess is MDUA.DataAccess.SalesOrderHeaderDataAccess concrete)
                    confirmedState = concrete.GetConfirmedFlag(delivery.SalesOrderId);


                try
                {
                    // ✅ use the diagnostic updater (proves DB + rowsAffected)
                    if (_salesOrderHeaderDataAccess is MDUA.DataAccess.SalesOrderHeaderDataAccess concrete2)
                        concrete2.UpdateStatusSafeLogged(delivery.SalesOrderId, parentStatus, confirmedState);
                    else
                        _salesOrderHeaderDataAccess.UpdateStatusSafe(delivery.SalesOrderId, parentStatus, confirmedState);

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
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
            // ✅ DELIVERY FEE LOGIC (Revenue)
            // -------------------------------------------------------------
            // We trust the "DeliveryCharge" sent from the UI (Editable Input)
            decimal deliveryFeeToCharge = orderData.DeliveryCharge;

            // A. Detect "In-Store" based on street naming convention from JS
            bool isStoreSale = !string.IsNullOrEmpty(orderData.Street) &&
                                orderData.Street.IndexOf("Counter Sale", StringComparison.OrdinalIgnoreCase) >= 0;

            // B. Apply to Totals
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

                        // ✅ Delivery DA for snapshot
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
                                CreatedAt = DateTime.UtcNow,
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
                            CreatedAt = DateTime.UtcNow,
                            PostalCode = orderData.PostalCode ?? "0000",
                            ZipCode = (orderData.ZipCode ?? "0000").ToCharArray()
                        };
                        var existingAddr = transAddressDA.CheckExistingAddress(customerId, addr);
                        int addressId = (existingAddr != null) ? existingAddr.Id : (int)transAddressDA.InsertAddressSafe(addr);

                        // D. Order Header
                        orderData.CompanyCustomerId = transCompanyCustomerDA.GetId(orderData.TargetCompanyId, customerId);
                        orderData.AddressId = addressId;
                        orderData.SalesChannelId = 2; // Direct
                        orderData.OrderDate = DateTime.UtcNow;
                        orderData.Status = orderData.Confirmed ? "Confirmed" : "Draft";
                        orderData.IsActive = true;
                        orderData.CreatedBy = "Admin";
                        orderData.CreatedAt = DateTime.UtcNow;

                        int orderId = (int)transOrderDA.InsertSalesOrderHeaderSafe(orderData);

                        // E. Order Detail
                        transDetailDA.InsertSalesOrderDetailSafe(new SalesOrderDetail
                        {
                            SalesOrderId = orderId,
                            ProductVariantId = orderData.ProductVariantId,
                            Quantity = orderData.OrderQuantity,
                            UnitPrice = finalUnitPrice, // Store Net Unit Price
                            CreatedBy = "Admin",
                            CreatedAt = DateTime.UtcNow
                        });

                        // ------------------------------------------------------------------
                        // ✅ 4. EXPENSE SNAPSHOT (If Confirmed, Freeze Cost in Delivery Table)
                        // ------------------------------------------------------------------
                        // Note: Only create delivery record if it's NOT a Store Pickup
                        if (orderData.Confirmed && !isStoreSale)
                        {
                            // Logic to retrieve Standard Cost from Settings (Expense Tracking)
                            int companyId = orderData.TargetCompanyId > 0 ? orderData.TargetCompanyId : 1;
                            var settings = _settingsFacade.GetDeliverySettings(companyId) ?? new Dictionary<string, int>();

                            bool isDhaka = (!string.IsNullOrEmpty(orderData.Divison) &&
                                            orderData.Divison.IndexOf("dhaka", StringComparison.OrdinalIgnoreCase) >= 0)
                                            || (!string.IsNullOrEmpty(orderData.City) &&
                                            orderData.City.IndexOf("dhaka", StringComparison.OrdinalIgnoreCase) >= 0);

                            // Fetch ACTUAL COST (Expense) from Settings
                            // If keys don't exist, fallback to revenue defaults or 0
                            decimal costInside = settings.ContainsKey("Cost_InsideDhaka") ? settings["Cost_InsideDhaka"] : (settings.ContainsKey("dhaka") ? settings["dhaka"] : 60);
                            decimal costOutside = settings.ContainsKey("Cost_OutsideDhaka") ? settings["Cost_OutsideDhaka"] : (settings.ContainsKey("outside") ? settings["outside"] : 120);

                            decimal actualCost = isDhaka ? costInside : costOutside;

                            var delivery = new Delivery
                            {
                                SalesOrderId = orderId,
                                TrackingNumber = "DO-" + DateTime.UtcNow.Ticks.ToString().Substring(12),
                                Status = "Pending",
                                ShippingCost = actualCost, // ✅ EXPENSE (Standard Cost)
                                CreatedBy = "Admin_Direct",
                                CreatedAt = DateTime.UtcNow
                            };

                            // Use the Extended Insert logic
                            transDeliveryDA.InsertExtended(delivery);
                        }

                        transaction.Commit();

                        return new
                        {
                            OrderId = "DO" + orderId.ToString("D8"),
                            NetAmount = netAmount,
                            DiscountAmount = totalDiscount,
                            TotalAmount = orderData.TotalAmount,
                            DeliveryFee = deliveryFeeToCharge // Return the charged amount
                        };
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        //new
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