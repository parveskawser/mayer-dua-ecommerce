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

        // ✅ 1. Declare Configuration to access appsettings.json
        private readonly IConfiguration _configuration;

        public OrderFacade(
            ISalesOrderHeaderDataAccess salesOrderHeaderDataAccess,
            ISalesOrderDetailDataAccess salesOrderDetailDataAccess,
            ICustomerDataAccess customerDataAccess,
            ICompanyCustomerDataAccess companyCustomerDataAccess,
            IAddressDataAccess addressDataAccess,
            IProductVariantDataAccess productVariantDataAccess,
            IProductFacade productFacade,
            IPostalCodesDataAccess postalCodesDataAccess,
            // ✅ 2. Inject Configuration
            IConfiguration configuration)
        {
            _salesOrderHeaderDataAccess = salesOrderHeaderDataAccess;
            _salesOrderDetailDataAccess = salesOrderDetailDataAccess;
            _customerDataAccess = customerDataAccess;
            _companyCustomerDataAccess = companyCustomerDataAccess;
            _addressDataAccess = addressDataAccess;
            _productVariantDataAccess = productVariantDataAccess;
            _productFacade = productFacade;
            _postalCodesDataAccess = postalCodesDataAccess;

            // ✅ 3. Assign Configuration
            _configuration = configuration;
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
            // 1. PRE-CALCULATION (Read-Only, outside transaction)
            var variant = _productVariantDataAccess.GetWithStock(orderData.ProductVariantId); 
            if (variant == null) throw new Exception("Variant not found.");

            if (variant.StockQty == 0)
            {
                throw new Exception("The selected product variant is currently out of stock.");
            }

            if (variant.StockQty < orderData.OrderQuantity)
            {
                throw new Exception($"Requested amount {orderData.OrderQuantity} exceeds available amount {variant.StockQty}.");
            }

            decimal baseVariantPrice = variant.VariantPrice ?? 0;
            int quantity = orderData.OrderQuantity;

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

            // 2. TRANSACTIONAL SAVE (The Fix)
            // ✅ Fetch connection string safely from appsettings.json
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // ✅ Start ONE transaction for all DA classes to share
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 3. MANUAL DA INSTANTIATION
                        // We create temporary DA instances that use our SHARED transaction.
                        // This bypasses the "Distributed Transaction" error.

                        var transCustomerDA = new CustomerDataAccess(transaction);
                        var transCompanyCustomerDA = new CompanyCustomerDataAccess(transaction);
                        var transAddressDA = new AddressDataAccess(transaction);
                        var transOrderDA = new SalesOrderHeaderDataAccess(transaction);
                        var transDetailDA = new SalesOrderDetailDataAccess(transaction);

                        // --- LOGIC STARTS HERE ---

                        int companyId = orderData.TargetCompanyId;
                        int customerId = 0;

                        // A. Customer Logic
                        var customer = transCustomerDA.GetByPhone(orderData.CustomerPhone);

                        if (customer == null)
                        {
                            string emailToCheck = !string.IsNullOrEmpty(orderData.CustomerEmail)
                                ? orderData.CustomerEmail
                                : $"{orderData.CustomerPhone}@guest.local";

                            var existingCustomerByEmail = transCustomerDA.GetByEmail(emailToCheck);
                            if (existingCustomerByEmail != null)
                            {
                                throw new Exception($"Email {emailToCheck} is already registered with a different phone number.");
                            }

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

                            // Refresh to get the new ID
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
                        bool isLinked = transCompanyCustomerDA.IsLinked(companyId, customerId);
                        if (!isLinked)
                        {
                            transCompanyCustomerDA.Insert(new CompanyCustomer
                            {
                                CompanyId = companyId,
                                CustomerId = customerId
                            });
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
                        int addressId;

                        if (existingAddress != null)
                        {
                            addressId = existingAddress.Id;
                        }
                        else
                        {
                            // ✅ FIX: Capture the returned ID!
                            // Do NOT rely on addr.Id being updated automatically.
                            long newAddressId = transAddressDA.InsertAddressSafe(addr);

                            if (newAddressId <= 0) throw new Exception("Failed to insert Address. ID returned was 0.");

                            addressId = (int)newAddressId;
                        }
                        // D. Save Order Header
                        orderData.CompanyCustomerId = transCompanyCustomerDA.GetId(companyId, customerId);
                        orderData.AddressId = addressId;
                        orderData.SalesChannelId = 1;
                        orderData.OrderDate = DateTime.Now;
                        orderData.DiscountAmount = totalDiscountAmount;
                        orderData.TotalAmount = baseVariantPrice * quantity;
                        orderData.Status = "Draft";
                        orderData.IsActive = true;
                        orderData.CreatedBy = "System_Order";
                        orderData.CreatedAt = DateTime.Now;
                        orderData.Confirmed = false;

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

                        // 4. COMMIT TRANSACTION
                        transaction.Commit();

                        return "ON" + orderId.ToString("D8");
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw; // Re-throw the error so the Controller sees it
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
        
        public List<SalesOrderHeader> GetAllOrdersForAdmin()
        {
            // Assuming the existing _salesOrderHeaderDataAccess.GetAll() calls the 
            // [dbo].[GetAllSalesOrderHeader] stored procedure, or equivalent.
            return _salesOrderHeaderDataAccess.GetAllSalesOrderHeaders().ToList();
        }


        //new
        public string UpdateOrderConfirmation(int orderId, bool isConfirmed)
        {
            // 1. Determine DB Status (Must be "Draft" to satisfy SQL Check Constraint)
            string dbStatus = isConfirmed ? "Confirmed" : "Draft";

            // 2. Call the safe update method to save to Database
            _salesOrderHeaderDataAccess.UpdateStatusSafe(orderId, dbStatus, isConfirmed);

            // 3. Return "Pending" to the UI for display purposes
            // This ensures the badge immediately shows "Pending" instead of "Draft"
            return isConfirmed ? "Confirmed" : "Pending";
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

        // ✅ FIXED: Using Tuple access (.Item1, .Item2 or .StockQty, .Price)
        public string PlaceAdminOrder(SalesOrderHeader orderData)
        {
            // 1. Stock & Price Check (From VariantPriceStock)
            var variantInfo = _salesOrderHeaderDataAccess.GetVariantStockAndPrice(orderData.ProductVariantId);

            if (variantInfo == null) throw new Exception("Variant not found in Stock System.");

            int currentStock = variantInfo.Value.StockQty;
            decimal basePrice = variantInfo.Value.Price; // The selling price from VPS

            if (currentStock < orderData.OrderQuantity)
                throw new Exception($"Stock Error: Only {currentStock} available.");

            // 2. Discount Calculation
            // We need the ProductId to check for discounts, so we fetch the basic definition
            var variantBasic = _productVariantDataAccess.Get(orderData.ProductVariantId);
            if (variantBasic == null) throw new Exception("Variant definition not found.");

            decimal finalUnitPrice = basePrice;
            decimal totalDiscount = 0;
            int quantity = orderData.OrderQuantity;

            // Call the Product Facade to check for active discounts
            var bestDiscount = _productFacade.GetBestDiscount(variantBasic.ProductId, basePrice);

            if (bestDiscount != null)
            {
                if (bestDiscount.DiscountType == "Flat")
                {
                    decimal discountPerItem = bestDiscount.DiscountValue;
                    finalUnitPrice -= discountPerItem;
                    totalDiscount = discountPerItem * quantity;
                }
                else if (bestDiscount.DiscountType == "Percentage")
                {
                    decimal discountRate = bestDiscount.DiscountValue / 100;
                    decimal discountPerItem = basePrice * discountRate;
                    finalUnitPrice -= discountPerItem;
                    totalDiscount = discountPerItem * quantity;
                }
            }

            // 3. Transactional Save
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

                        // A. Customer Logic
                        int customerId = 0;
                        var customer = transCustomerDA.GetByPhone(orderData.CustomerPhone);

                        if (customer == null)
                        {
                            var newCust = new Customer
                            {
                                CustomerName = orderData.CustomerName,
                                Phone = orderData.CustomerPhone,
                                Email = $"{orderData.CustomerPhone}@direct.local",
                                IsActive = true,
                                CreatedAt = DateTime.Now,
                                CreatedBy = "Admin"
                            };
                            transCustomerDA.Insert(newCust);
                            customer = transCustomerDA.GetByPhone(orderData.CustomerPhone);
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

                        // D. Header
                        orderData.CompanyCustomerId = transCompanyCustomerDA.GetId(orderData.TargetCompanyId, customerId);
                        orderData.AddressId = addressId;
                        orderData.SalesChannelId = 2; // Direct
                        orderData.OrderDate = DateTime.Now;

                        // ✅ APPLY CALCULATED DISCOUNT
                        orderData.DiscountAmount = totalDiscount;
                        orderData.TotalAmount = basePrice * quantity; // Gross Amount
                                                                      // DB will calculate NetAmount = TotalAmount - DiscountAmount

                        orderData.Status = orderData.Confirmed ? "Confirmed" : "Draft";
                        orderData.IsActive = true;
                        orderData.CreatedBy = "Admin";
                        orderData.CreatedAt = DateTime.Now;

                        int orderId = (int)transOrderDA.InsertSalesOrderHeaderSafe(orderData);

                        // E. Detail
                        transDetailDA.InsertSalesOrderDetailSafe(new SalesOrderDetail
                        {
                            SalesOrderId = orderId,
                            ProductVariantId = orderData.ProductVariantId,
                            Quantity = orderData.OrderQuantity,
                            // ✅ SAVE DISCOUNTED UNIT PRICE
                            UnitPrice = finalUnitPrice,
                            CreatedBy = "Admin",
                            CreatedAt = DateTime.Now
                        });

                        transaction.Commit();
                        return "DO" + orderId.ToString("D8");
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        #endregion
    }
}