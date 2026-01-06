using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;
using Microsoft.Extensions.Configuration; // ✅ Required for appsettings.json access
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;             // Required for SqlConnection
               
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
        private readonly IDeliveryItemDataAccess _deliveryItemDataAccess;
        private readonly IEmailService _emailService;


        private readonly IConfiguration _configuration;
        private readonly IDeliveryDataAccess _deliveryDataAccess;
        private readonly ISmsService _smsService;
        private readonly INotificationService _notificationService;
        public OrderFacade(
            ISalesOrderHeaderDataAccess salesOrderHeaderDataAccess,
            ISalesOrderDetailDataAccess salesOrderDetailDataAccess,
            ICustomerDataAccess customerDataAccess,
            ICompanyCustomerDataAccess companyCustomerDataAccess,
            IAddressDataAccess addressDataAccess,
            IProductVariantDataAccess productVariantDataAccess,
            IProductFacade productFacade,
            IPostalCodesDataAccess postalCodesDataAccess,
            IConfiguration configuration,
            ISettingsFacade settingsFacade,
            IDeliveryDataAccess deliveryDataAccess,IDeliveryItemDataAccess deliveryItemDataAccess,
            ISmsService smsService,
            INotificationService notificationService, IEmailService emailService)
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
            _deliveryItemDataAccess = deliveryItemDataAccess;
            _smsService = smsService;
            _notificationService = notificationService;
            _emailService = emailService;

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


        public async Task<string> PlaceGuestOrder(SalesOrderHeader orderData)
        {
            // ==============================================================================
            // 1. VALIDATION & PRE-CALCULATION
            // ==============================================================================
            if (string.IsNullOrWhiteSpace(orderData.CustomerName))
                throw new Exception("Customer Name is required.");

            if (string.IsNullOrWhiteSpace(orderData.CustomerPhone))
                throw new Exception("Phone number is required.");

            string actionUser = !string.IsNullOrEmpty(orderData.CreatedBy) ? orderData.CreatedBy : orderData.CustomerName;

            // Sanitize Inputs
            orderData.CustomerName = orderData.CustomerName.Trim();
            orderData.CustomerPhone = orderData.CustomerPhone.Trim();
            if (!string.IsNullOrEmpty(orderData.CustomerEmail))
                orderData.CustomerEmail = orderData.CustomerEmail.Trim();

            // ✅ FIX 1: Determine Company ID from the Product (Security)
            // We do this BEFORE starting the transaction
            var variant = _productVariantDataAccess.GetWithStock(orderData.ProductVariantId);
            if (variant == null) throw new Exception("Variant not found.");

            var product = _productFacade.Get(variant.ProductId);
            if (product == null) throw new Exception("Product not found.");

            // Override frontend data with real owner ID
            int companyId = product.CompanyId;
            orderData.TargetCompanyId = companyId;

            // Stock & Price Logic
            if (variant.StockQty == 0) throw new Exception("Out of stock.");
            if (variant.StockQty < orderData.OrderQuantity) throw new Exception("Insufficient stock.");

            decimal basePrice = variant.VariantPrice ?? 0;
            int qty = orderData.OrderQuantity;
            var discount = _productFacade.GetBestDiscount(variant.ProductId, basePrice);

            decimal finalPrice = basePrice;
            decimal totalDiscount = 0;

            if (discount != null)
            {
                if (discount.DiscountType == "Flat")
                {
                    finalPrice -= discount.DiscountValue;
                    totalDiscount = discount.DiscountValue * qty;
                }
                else
                {
                    finalPrice -= (basePrice * (discount.DiscountValue / 100));
                    totalDiscount = (basePrice * (discount.DiscountValue / 100)) * qty;
                }
            }
            finalPrice = Math.Max(finalPrice, 0);

            // Set Financials
            orderData.TotalAmount = (basePrice * qty) + orderData.DeliveryCharge;
            orderData.DiscountAmount = totalDiscount;
            orderData.NetAmount = orderData.TotalAmount - orderData.DiscountAmount;

            // ==============================================================================
            // 2. TRANSACTION EXECUTION
            // ==============================================================================
            string connStr = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connStr))
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

                        // --- A. CUSTOMER ---
                        int customerId;
                        var customer = transCustomerDA.GetByPhone(orderData.CustomerPhone);

                        if (customer == null)
                        {
                            string email = !string.IsNullOrEmpty(orderData.CustomerEmail)
                                ? orderData.CustomerEmail
                                : $"{orderData.CustomerPhone}@guest.local";

                            if (!string.IsNullOrEmpty(orderData.CustomerEmail) && transCustomerDA.GetByEmail(email) != null)
                                throw new Exception("Email already registered.");

                            var newCust = new Customer
                            {
                                CustomerName = orderData.CustomerName,
                                Phone = orderData.CustomerPhone,
                                Email = email,
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = actionUser
                            };
                            transCustomerDA.Insert(newCust);
                            // Fetch to be safe
                            customer = transCustomerDA.GetByPhone(orderData.CustomerPhone);
                        }
                        else
                        {
                            // Update if needed
                            if (!customer.CustomerName.Equals(orderData.CustomerName, StringComparison.OrdinalIgnoreCase))
                            {
                                customer.CustomerName = orderData.CustomerName;
                                transCustomerDA.Update(customer);
                            }
                        }
                        customerId = customer.Id;

                        // --- B. COMPANY LINK ---
                        // Uses the 'EnsureLinkAndGetId' logic we fixed earlier
                        int companyCustomerId = transCompanyCustomerDA.EnsureLinkAndGetId(companyId, customerId);
                        if (companyCustomerId <= 0) throw new Exception("Failed to link customer.");

                        // --- C. ADDRESS ---
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
                            CreatedBy = actionUser,
                            CreatedAt = DateTime.UtcNow,
                            PostalCode = orderData.PostalCode ?? "0000",
                            ZipCode = (orderData.ZipCode ?? "0000").ToCharArray()
                        };

                        var existingAddr = transAddressDA.CheckExistingAddress(customerId, addr);
                        int addressId;

                        if (existingAddr != null)
                        {
                            addressId = existingAddr.Id;
                        }
                        else
                        {
                            transAddressDA.InsertAddressSafe(addr);
                            addressId = addr.Id; // ✅ Correctly reading ID from object
                        }

                        if (addressId <= 0) throw new Exception("Failed to generate Address ID.");

                        // --- D. ORDER HEADER ---
                        orderData.CompanyCustomerId = companyCustomerId;
                        orderData.AddressId = addressId;
                        orderData.SalesChannelId = 1;
                        orderData.OrderDate = DateTime.UtcNow;
                        orderData.Status = "Draft";
                        orderData.IsActive = true;
                        orderData.CreatedBy = actionUser;
                        orderData.CreatedAt = DateTime.UtcNow;
                        orderData.Confirmed = false;

                        int orderId = (int)transOrderDA.InsertSalesOrderHeaderSafe(orderData);

                        // Optional: Update the object property for consistency
                        orderData.Id = orderId;

                        if (orderId <= 0) throw new Exception("Failed to create Order Header.");

                        // --- E. ORDER DETAIL ---
                        transDetailDA.InsertSalesOrderDetailSafe(new SalesOrderDetail
                        {
                            SalesOrderId = orderId,
                            ProductVariantId = orderData.ProductVariantId,
                            Quantity = qty,
                            UnitPrice = finalPrice,
                            CreatedBy = actionUser,
                            CreatedAt = DateTime.UtcNow
                        });

                        transaction.Commit();

                        // --- F. NOTIFICATIONS ---
                        string orderNo = "ON" + orderId.ToString("D8");
                        try
                        {
                            // Fire and forget notifications
                            _ = Task.Run(async () => {
                                try
                                {
                                    if (!string.IsNullOrEmpty(orderData.CustomerEmail))
                                    {
                                        var p = new System.Collections.Hashtable {
                                    {"ToEmail", orderData.CustomerEmail}, {"UserName", orderData.CustomerName},
                                    {"OrderId", orderNo}, {"OrderQty", qty}, {"OrderTotal", orderData.TotalAmount}
                                };
                                        await _emailService.SendEmail(p, "mailToOrderPlace");
                                    }
                                }
                                catch { }
                            });
                        }
                        catch { }

                        return orderNo;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }




        public dynamic PlaceAdminOrder(SalesOrderHeader orderData)
        {
            // ==============================================================================
            // 1. VALIDATION & PRE-CALCULATION
            // ==============================================================================

            // A. Stock Check
            var variantInfo = _salesOrderHeaderDataAccess.GetVariantStockAndPrice(orderData.ProductVariantId);
            if (variantInfo == null) throw new Exception("Variant not found.");

            if (variantInfo.Value.StockQty < orderData.OrderQuantity)
                throw new Exception($"Stock Error: Only {variantInfo.Value.StockQty} available.");

            // ✅ FIX 1: SECURITY - Derive Company ID from the Product
            // We strictly use the Product's Company ID, ignoring what the frontend sent.
            var variantBasic = _productVariantDataAccess.Get(orderData.ProductVariantId);
            var product = _productFacade.Get(variantBasic.ProductId);

            if (product == null) throw new Exception("Product not found.");

            // OVERWRITE the CompanyId
            int realCompanyId = product.CompanyId;
            orderData.TargetCompanyId = realCompanyId;

            decimal basePrice = variantInfo.Value.Price;
            string actionUser = !string.IsNullOrEmpty(orderData.CreatedBy) ? orderData.CreatedBy : "Admin";

            // 2. Discount Calculation
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
            finalUnitPrice = Math.Max(finalUnitPrice, 0);

            // DELIVERY FEE
            decimal deliveryFeeToCharge = orderData.DeliveryCharge;
            bool isStoreSale = !string.IsNullOrEmpty(orderData.Street) &&
                                orderData.Street.IndexOf("Counter Sale", StringComparison.OrdinalIgnoreCase) >= 0;

            decimal grossProductCost = basePrice * orderData.OrderQuantity;
            orderData.TotalAmount = grossProductCost + deliveryFeeToCharge;
            orderData.DiscountAmount = totalDiscount;

            // 3. SAVE TO DATABASE
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Initialize DAs
                        var transCustomerDA = new CustomerDataAccess(transaction);
                        var transCompanyCustomerDA = new CompanyCustomerDataAccess(transaction);
                        var transAddressDA = new AddressDataAccess(transaction);
                        var transOrderDA = new SalesOrderHeaderDataAccess(transaction);
                        var transDetailDA = new SalesOrderDetailDataAccess(transaction);
                        var transDeliveryDA = new DeliveryDataAccess(transaction);

                        // --- A. CUSTOMER LOGIC ---
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
                                CreatedBy = actionUser
                            };
                            transCustomerDA.Insert(newCust);
                            customer = transCustomerDA.GetByPhone(orderData.CustomerPhone);
                        }
                        else
                        {
                            bool isUpdated = false;
                            if (!string.IsNullOrWhiteSpace(orderData.CustomerName) &&
                                !string.Equals(customer.CustomerName, orderData.CustomerName, StringComparison.OrdinalIgnoreCase))
                            {
                                customer.CustomerName = orderData.CustomerName;
                                isUpdated = true;
                            }
                            if (!string.IsNullOrEmpty(orderData.CustomerEmail) &&
                                !string.Equals(customer.Email, orderData.CustomerEmail, StringComparison.OrdinalIgnoreCase))
                            {
                                customer.Email = orderData.CustomerEmail;
                                isUpdated = true;
                            }

                            if (isUpdated)
                            {
                                customer.UpdatedBy = actionUser;
                                customer.UpdatedAt = DateTime.UtcNow;
                                transCustomerDA.Update(customer);
                            }
                        }
                        customerId = customer.Id;

                        // --- B. COMPANY LINK LOGIC ---
                        // ✅ FIX 2: Use the Robust Link Method with the REAL Company ID
                        int companyCustomerId = transCompanyCustomerDA.EnsureLinkAndGetId(realCompanyId, customerId);
                        if (companyCustomerId <= 0)
                            throw new Exception($"Failed to link Customer to Company {realCompanyId}");

                        // --- C. ADDRESS LOGIC ---
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
                            CreatedBy = actionUser,
                            CreatedAt = DateTime.UtcNow,
                            PostalCode = orderData.PostalCode ?? "0000",
                            ZipCode = (orderData.ZipCode ?? "0000").ToCharArray()
                        };

                        var existingAddr = transAddressDA.CheckExistingAddress(customerId, addr);
                        int addressId;

                        if (existingAddr != null)
                        {
                            addressId = existingAddr.Id;
                        }
                        else
                        {
                            // ✅ FIX 3: Capture the RETURN VALUE as the ID
                            // (Assuming InsertAddressSafe returns SCOPE_IDENTITY like SalesOrderHeader)
                            addressId = (int)transAddressDA.InsertAddressSafe(addr);
                        }

                        if (addressId <= 0) throw new Exception("Failed to generate valid Address ID.");

                        // --- D. ORDER HEADER ---
                        orderData.CompanyCustomerId = companyCustomerId; // Assigned correctly
                        orderData.AddressId = addressId;
                        orderData.SalesChannelId = 2; // Direct
                        orderData.OrderDate = DateTime.UtcNow;
                        orderData.Status = orderData.Confirmed ? "Confirmed" : "Draft";
                        orderData.IsActive = true;
                        orderData.CreatedBy = actionUser;
                        orderData.CreatedAt = DateTime.UtcNow;

                        // ✅ FIX 4: Capture the returned Order ID
                        int orderId = (int)transOrderDA.InsertSalesOrderHeaderSafe(orderData);

                        if (orderId <= 0) throw new Exception("Failed to create Order Header.");

                        // Update object property for consistency
                        orderData.Id = orderId;

                        // --- E. ORDER DETAIL ---
                        transDetailDA.InsertSalesOrderDetailSafe(new SalesOrderDetail
                        {
                            SalesOrderId = orderId,
                            ProductVariantId = orderData.ProductVariantId,
                            Quantity = orderData.OrderQuantity,
                            UnitPrice = finalUnitPrice,
                            CreatedBy = actionUser,
                            CreatedAt = DateTime.UtcNow
                        });

                        // --- F. DELIVERY SNAPSHOT ---
                        if (orderData.Confirmed && !isStoreSale)
                        {
                            var settings = _settingsFacade.GetDeliverySettings(realCompanyId) ?? new Dictionary<string, int>();

                            bool isDhaka = (!string.IsNullOrEmpty(orderData.Divison) &&
                                            orderData.Divison.IndexOf("dhaka", StringComparison.OrdinalIgnoreCase) >= 0)
                                            || (!string.IsNullOrEmpty(orderData.City) &&
                                            orderData.City.IndexOf("dhaka", StringComparison.OrdinalIgnoreCase) >= 0);

                            decimal costInside = settings.ContainsKey("Cost_InsideDhaka") ? settings["Cost_InsideDhaka"] : (settings.ContainsKey("dhaka") ? settings["dhaka"] : 60);
                            decimal costOutside = settings.ContainsKey("Cost_OutsideDhaka") ? settings["Cost_OutsideDhaka"] : (settings.ContainsKey("outside") ? settings["outside"] : 120);
                            decimal actualCost = isDhaka ? costInside : costOutside;

                            var delivery = new Delivery
                            {
                                SalesOrderId = orderId,
                                TrackingNumber = "DO-" + DateTime.UtcNow.Ticks.ToString().Substring(12),
                                Status = "Pending",
                                ShippingCost = actualCost,
                                CreatedBy = actionUser,
                                CreatedAt = DateTime.UtcNow
                            };
                            transDeliveryDA.InsertExtended(delivery);
                        }

                        transaction.Commit();

                        return new
                        {
                            OrderId = "DO" + orderId.ToString("D8"),
                            NetAmount = orderData.NetAmount, // Use calculated NetAmount
                            DiscountAmount = totalDiscount,
                            TotalAmount = orderData.TotalAmount,
                            DeliveryFee = deliveryFeeToCharge
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
        }        // ==========================================================================


        public void UpdateDeliveryStatus(int deliveryId, string newStatus)
        {

            var delivery = _deliveryDataAccess.GetExtended(deliveryId);

            if (delivery == null) throw new Exception("Delivery not found");
            if (delivery.SalesOrderId <= 0) throw new Exception("Data Error: Delivery has no Sales Order ID.");
            bool isOrderConfirmed = false;
            if (_salesOrderHeaderDataAccess is MDUA.DataAccess.SalesOrderHeaderDataAccess concreteDA)
            {
                isOrderConfirmed = concreteDA.GetConfirmedFlag(delivery.SalesOrderId);
            }
            else
            {
                // Fallback if interface doesn't support GetConfirmedFlag
                var order = _salesOrderHeaderDataAccess.Get(delivery.SalesOrderId);
                isOrderConfirmed = order != null && (order.Confirmed || order.Status == "Confirmed");
            }

            if (!isOrderConfirmed)
            {
                throw new Exception("Action Denied: You cannot update delivery status while the Sales Order is still Draft/Pending. Please confirm the order first.");
            }
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



        public List<dynamic> GetProductVariantsForAdmin(int companyId) // ✅ Added Parameter
        {
            // Pass companyId to DataAccess
            var rawList = _salesOrderHeaderDataAccess.GetVariantsForDropdown(companyId);

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
                        item["DiscountType"] = bestDiscount.DiscountType;
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

        //new
        public DashboardStats GetDashboardMetrics(int companyId)
        {
            return _salesOrderHeaderDataAccess.GetDashboardMetrics(companyId);
        }

        public List<SalesOrderHeader> GetRecentOrders(int companyId)
        {
            return _salesOrderHeaderDataAccess.GetRecentOrders(companyId, 5);
        }

        public List<ChartDataPoint> GetSalesTrend(int companyId)
        {
            return _salesOrderHeaderDataAccess.GetSalesTrend(companyId, 6);
        }

        public List<ChartDataPoint> GetOrderStatusCounts(int companyId)
        {
            return _salesOrderHeaderDataAccess.GetOrderStatusCounts(companyId);
        }

        private string GetValidBangladeshiNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            // 1. Clean garbage characters
            string clean = input.Trim()
                                .Replace(" ", "")
                                .Replace("-", "")
                                .Replace("(", "")
                                .Replace(")", "");

            // 2. Check for BD Prefix variations
            if (clean.StartsWith("+8801"))
            {
                clean = clean.Substring(3); // Remove +88 -> 01...
            }
            else if (clean.StartsWith("8801"))
            {
                clean = clean.Substring(2); // Remove 88 -> 01...
            }

            // 3. Final Check: Must start with '01' and be 11 digits total
            // Regex: Starts with 01, followed by 3-9, followed by 8 digits
            if (System.Text.RegularExpressions.Regex.IsMatch(clean, @"^01[3-9]\d{8}$"))
            {
                return "+88" + clean; // Return ready for TextBee/GreenWeb
            }

            // Return null if it's international or garbage (Order succeeds, just no SMS)
            return null;
        }


        public SalesOrderHeaderList GetPagedOrdersForAdmin(int pageIndex, int pageSize, string whereClause, int companyId, out int totalRows)
        {
            if (_salesOrderHeaderDataAccess is MDUA.DataAccess.SalesOrderHeaderDataAccess concreteDA)
            {
                return concreteDA.GetPagedOrdersExtended(pageIndex, pageSize, whereClause, companyId, out totalRows);
            }
            totalRows = 0;
            return new SalesOrderHeaderList();
        }
        public int GetOrderPageNumber(int orderId, int pageSize)
        {
            // No casting needed anymore!
            return _salesOrderHeaderDataAccess.GetOrderPageNumber(orderId, pageSize);
        }

        // Inside MDUA.Facade/OrderFacade.cs

        // Implement the method
        public SalesOrderHeader GetOrderById(int id)
        {
          

            return _salesOrderHeaderDataAccess.Get(id);
        }
        public List<Dictionary<string, object>> GetExportData(MDUA.Entities.ExportRequest request)
        {
            var sb = new System.Text.StringBuilder("1=1");

            // --- SCOPE 1: Selected Rows (Specific IDs) ---
            if (request.Scope == "selected" && request.SelectedIds != null && request.SelectedIds.Any())
            {
                string ids = string.Join(",", request.SelectedIds);
                sb.Append($" AND soh.Id IN ({ids})");
            }
            // --- SCOPE 2: Filtered Rows (Re-use your Controller filter logic) ---
            else if (request.Scope == "filtered")
            {
                // 1. Status
                if (!string.IsNullOrEmpty(request.Status) && request.Status != "all")
                {
                    string status = (request.Status == "Pending") ? "Draft" : request.Status;
                    sb.Append($" AND soh.Status = '{status}'");
                }

                // 2. Payment Status
                if (!string.IsNullOrEmpty(request.PayStatus) && request.PayStatus != "all")
                {
                    if (request.PayStatus == "Paid")
                        sb.Append(" AND (soh.NetAmount - ISNULL((SELECT SUM(Amount) FROM CustomerPayment WHERE TransactionReference = soh.SalesOrderId), 0)) <= 0");
                    else if (request.PayStatus == "Partial")
                        sb.Append(" AND (SELECT SUM(Amount) FROM CustomerPayment WHERE TransactionReference = soh.SalesOrderId) > 0 AND (soh.NetAmount - ISNULL((SELECT SUM(Amount) FROM CustomerPayment WHERE TransactionReference = soh.SalesOrderId), 0)) > 0");
                    else if (request.PayStatus == "Unpaid")
                        sb.Append(" AND ISNULL((SELECT SUM(Amount) FROM CustomerPayment WHERE TransactionReference = soh.SalesOrderId), 0) = 0");
                }

                // 3. Order Type
                if (!string.IsNullOrEmpty(request.OrderType) && request.OrderType != "all")
                {
                    if (request.OrderType == "Online") sb.Append(" AND soh.SalesChannelId = 1");
                    else if (request.OrderType == "Direct") sb.Append(" AND soh.SalesChannelId <> 1");
                }

                // 4. Amount Range
                if (request.MinAmount.HasValue) sb.Append($" AND soh.NetAmount >= {request.MinAmount}");
                if (request.MaxAmount.HasValue) sb.Append($" AND soh.NetAmount <= {request.MaxAmount}");

                // 5. Search (ID)
                if (!string.IsNullOrEmpty(request.Search))
                {
                    string cleanSearch = request.Search.Replace("'", "''");
                    sb.Append($" AND (soh.SalesOrderId LIKE '%{cleanSearch}%' OR CAST(soh.Id AS NVARCHAR) LIKE '%{cleanSearch}%')");
                }

                // 6. Date Range
                if (request.FromDate.HasValue)
                    sb.Append($" AND soh.OrderDate >= '{request.FromDate.Value:yyyy-MM-dd HH:mm:ss}'");

                if (request.ToDate.HasValue)
                    sb.Append($" AND soh.OrderDate <= '{request.ToDate.Value:yyyy-MM-dd HH:mm:ss}'");
            }

            // Call Data Access
            return _salesOrderHeaderDataAccess.GetExportDataDynamic(sb.ToString(), request.Columns);
        }
    }
    #endregion
}
