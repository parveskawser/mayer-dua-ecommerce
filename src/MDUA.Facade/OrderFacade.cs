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
            // ✅ 0. VALIDATION
            if (string.IsNullOrWhiteSpace(orderData.CustomerName))
                throw new Exception("Customer Name is required to place an order.");

            if (string.IsNullOrWhiteSpace(orderData.CustomerPhone))
                throw new Exception("Phone number is required.");

            // Determine who is performing the action (System/Admin or the Guest themselves)
            string actionUser = !string.IsNullOrEmpty(orderData.CreatedBy)
                ? orderData.CreatedBy
                : orderData.CustomerName;

            orderData.CustomerName = orderData.CustomerName.Trim();
            orderData.CustomerPhone = orderData.CustomerPhone.Trim();

            // Trim email if provided
            if (!string.IsNullOrEmpty(orderData.CustomerEmail))
            {
                orderData.CustomerEmail = orderData.CustomerEmail.Trim();
            }

            // 1. PRE-CALCULATION
            var variant = _productVariantDataAccess.GetWithStock(orderData.ProductVariantId);
            if (variant == null) throw new Exception("Variant not found.");

            if (variant.StockQty == 0)
                throw new Exception("The selected product variant is currently out of stock.");

            if (variant.StockQty < orderData.OrderQuantity)
                throw new Exception($"Requested amount {orderData.OrderQuantity} exceeds available amount {variant.StockQty}.");

            decimal baseVariantPrice = variant.VariantPrice ?? 0;
            int quantity = orderData.OrderQuantity;

            // Calculate Discounts
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

            // Calculate Totals
            decimal totalProductPrice = baseVariantPrice * quantity;
            decimal deliveryCharge = orderData.DeliveryCharge;

            // Explicitly set calculations
            orderData.DeliveryCharge = deliveryCharge;
            orderData.TotalAmount = totalProductPrice + deliveryCharge;
            orderData.DiscountAmount = totalDiscountAmount;
            orderData.NetAmount = orderData.TotalAmount - orderData.DiscountAmount;

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
                        int customerId;

                        // 2. CUSTOMER LOGIC (Check, Create, or Update)
                        var customer = transCustomerDA.GetByPhone(orderData.CustomerPhone);
                        if (customer == null)
                        {
                            // CREATE NEW CUSTOMER
                            string emailToCheck = !string.IsNullOrEmpty(orderData.CustomerEmail)
                                ? orderData.CustomerEmail
                                : $"{orderData.CustomerPhone}@guest.local";

                            if (!string.IsNullOrEmpty(orderData.CustomerEmail) && transCustomerDA.GetByEmail(emailToCheck) != null)
                                throw new Exception($"Email {emailToCheck} is already registered to another number.");

                            transCustomerDA.Insert(new Customer
                            {
                                CustomerName = orderData.CustomerName,
                                Phone = orderData.CustomerPhone,
                                Email = emailToCheck,
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = actionUser
                            });

                            customer = transCustomerDA.GetByPhone(orderData.CustomerPhone);
                        }
                        else
                        {
                            // UPDATE EXISTING CUSTOMER
                            bool isUpdated = false;

                            if (!string.IsNullOrWhiteSpace(orderData.CustomerName) &&
                                !string.Equals(customer.CustomerName, orderData.CustomerName, StringComparison.OrdinalIgnoreCase))
                            {
                                customer.CustomerName = orderData.CustomerName;
                                isUpdated = true;
                            }

                            if (!string.IsNullOrWhiteSpace(orderData.CustomerEmail) &&
                                !string.Equals(customer.Email, orderData.CustomerEmail, StringComparison.OrdinalIgnoreCase))
                            {
                                customer.Email = orderData.CustomerEmail;
                                isUpdated = true;
                            }

                            if (isUpdated)
                            {
                                customer.UpdatedAt = DateTime.UtcNow;
                                customer.UpdatedBy = actionUser;
                                transCustomerDA.Update(customer);
                            }
                        }

                        customerId = customer.Id;

                        // Link to Company
                        if (!transCompanyCustomerDA.IsLinked(companyId, customerId))
                        {
                            transCompanyCustomerDA.Insert(new CompanyCustomer
                            {
                                CompanyId = companyId,
                                CustomerId = customerId
                            });
                        }

                        // 3. ADDRESS LOGIC
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
                            ZipCode = (orderData.ZipCode ?? orderData.PostalCode ?? "0000").ToCharArray()
                        };

                        var existingAddress = transAddressDA.CheckExistingAddress(customerId, addr);
                        int addressId = existingAddress != null
                            ? existingAddress.Id
                            : (int)transAddressDA.InsertAddressSafe(addr);

                        // 4. PREPARE ORDER HEADER
                        orderData.CompanyCustomerId = transCompanyCustomerDA.GetId(companyId, customerId);
                        orderData.AddressId = addressId;
                        orderData.SalesChannelId = 1; // Online/Guest
                        orderData.OrderDate = DateTime.UtcNow;
                        orderData.Status = "Draft";
                        orderData.IsActive = true;
                        orderData.CreatedBy = actionUser;
                        orderData.CreatedAt = DateTime.UtcNow;
                        orderData.Confirmed = false;

                        // 5. INSERT ORDER
                        int orderId = (int)transOrderDA.InsertSalesOrderHeaderSafe(orderData);
                        if (orderId <= 0)
                            throw new Exception("Failed to create Order Header.");

                        // 6. INSERT ORDER DETAILS
                        transDetailDA.InsertSalesOrderDetailSafe(new SalesOrderDetail
                        {
                            SalesOrderId = orderId,
                            ProductVariantId = orderData.ProductVariantId,
                            Quantity = orderData.OrderQuantity,
                            UnitPrice = finalUnitPrice,
                            CreatedBy = actionUser,
                            CreatedAt = DateTime.UtcNow
                        });

                        transaction.Commit(); // ✅ DB Transaction Complete

                        string orderNo = "ON" + orderId.ToString("D8");

                        // =================================================================
                        // 🚀 UNIFIED NOTIFICATION (Direct Await - No Task.Run)
                        // =================================================================
                        try
                        {
                            bool emailSuccess = false;

                            // 1. EMAIL: Check if we have an email to send to
                            if (!string.IsNullOrWhiteSpace(orderData.CustomerEmail))
                            {
                                // MAP DATA TO DATABASE TEMPLATE (mailToOrderPlace)
                                var templateParams = new Hashtable
                                {
                                    { "ToEmail", orderData.CustomerEmail },
                                    { "UserName", orderData.CustomerName },
                                    { "OrderId", orderNo },
                                    { "OrderQty", orderData.OrderQuantity },
                                    { "OrderTotal", orderData.TotalAmount }
                                };

                                // CALL THE SERVICE DIRECTLY
                                emailSuccess = await _emailService.SendEmail(templateParams, "mailToOrderPlace");

                                if (emailSuccess)
                                    Console.WriteLine($"[Order {orderNo}] Template Email sent successfully.");
                                else
                                    Console.WriteLine($"[Order {orderNo}] ❌ Template Email failed to send.");
                            }

                            // 2. SMS FALLBACK
                            if (!emailSuccess && !string.IsNullOrWhiteSpace(orderData.CustomerPhone))
                            {
                                Console.WriteLine($"[Order {orderNo}] Email failed or missing. Attempting SMS Fallback...");

                                await _notificationService.SendSmsOnlyAsync(
                                    orderData.CustomerPhone,
                                    $"Order {orderNo} Confirmed. Total: {orderData.TotalAmount}"
                                );
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("========== Notification Error ==========");
                            Console.WriteLine(ex);                 // includes stack trace
                            Console.WriteLine(ex.InnerException);   // if any
                            Console.WriteLine("========================================");
                        }


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
            // 1. Stock Check
            var variantInfo = _salesOrderHeaderDataAccess.GetVariantStockAndPrice(orderData.ProductVariantId);
            if (variantInfo == null) throw new Exception("Variant not found.");

            if (variantInfo.Value.StockQty < orderData.OrderQuantity)
                throw new Exception($"Stock Error: Only {variantInfo.Value.StockQty} available.");

            decimal basePrice = variantInfo.Value.Price;
            string actionUser = !string.IsNullOrEmpty(orderData.CreatedBy) ? orderData.CreatedBy : "Admin";
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

            // DELIVERY FEE
            decimal deliveryFeeToCharge = orderData.DeliveryCharge;
            bool isStoreSale = !string.IsNullOrEmpty(orderData.Street) &&
                                orderData.Street.IndexOf("Counter Sale", StringComparison.OrdinalIgnoreCase) >= 0;

            decimal grossProductCost = basePrice * orderData.OrderQuantity;
            orderData.TotalAmount = grossProductCost + deliveryFeeToCharge;
            orderData.DiscountAmount = totalDiscount;

            decimal netAmount = orderData.TotalAmount - totalDiscount;

            // 3. SAVE TO DATABASE
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
                                CreatedBy = actionUser // ✅ Use Logged In User
                            };
                            transCustomerDA.Insert(newCust);
                            customer = transCustomerDA.GetByPhone(orderData.CustomerPhone);
                        }
                        else
                        {
                            // ✅ UPDATE LOGIC FOR ADMIN (Force Update Name and Email)
                            bool isUpdated = false;

                            // Update Name
                            if (!string.IsNullOrWhiteSpace(orderData.CustomerName) &&
                                !string.Equals(customer.CustomerName, orderData.CustomerName, StringComparison.OrdinalIgnoreCase))
                            {
                                customer.CustomerName = orderData.CustomerName;
                                isUpdated = true;
                            }

                            // Update Email (If provided and different)
                            if (!string.IsNullOrEmpty(orderData.CustomerEmail) &&
                                !string.Equals(customer.Email, orderData.CustomerEmail, StringComparison.OrdinalIgnoreCase))
                            {
                                customer.Email = orderData.CustomerEmail;
                                isUpdated = true;
                            }

                            if (isUpdated)
                            {
                                customer.UpdatedBy = actionUser; // ✅ Use Logged In User
                                customer.UpdatedAt = DateTime.UtcNow;
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
                            CreatedBy = actionUser, // ✅ Use Logged In User
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
                        orderData.CreatedBy = actionUser;
                        orderData.CreatedAt = DateTime.UtcNow;

                        int orderId = (int)transOrderDA.InsertSalesOrderHeaderSafe(orderData);

                        // E. Order Detail
                        transDetailDA.InsertSalesOrderDetailSafe(new SalesOrderDetail
                        {
                            SalesOrderId = orderId,
                            ProductVariantId = orderData.ProductVariantId,
                            Quantity = orderData.OrderQuantity,
                            UnitPrice = finalUnitPrice,
                            CreatedBy = actionUser,
                            CreatedAt = DateTime.UtcNow
                        });

                        // 4. EXPENSE SNAPSHOT
                        if (orderData.Confirmed && !isStoreSale)
                        {
                            int companyId = orderData.TargetCompanyId > 0 ? orderData.TargetCompanyId : 1;
                            var settings = _settingsFacade.GetDeliverySettings(companyId) ?? new Dictionary<string, int>();

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
                            NetAmount = netAmount,
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
      
        public List<ChartDataPoint> GetSalesTrend()
        {
            return _salesOrderHeaderDataAccess.GetSalesTrend(6);
        }
     
        public List<ChartDataPoint> GetOrderStatusCounts()
        {
            return _salesOrderHeaderDataAccess.GetOrderStatusCounts();
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


        public SalesOrderHeaderList GetPagedOrdersForAdmin(int pageIndex, int pageSize, string whereClause, out int totalRows)
        {
            if (_salesOrderHeaderDataAccess is MDUA.DataAccess.SalesOrderHeaderDataAccess concreteDA)
            {
                // Pass the whereClause to the extended method
                return concreteDA.GetPagedOrdersExtended(pageIndex, pageSize, whereClause, out totalRows);
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
