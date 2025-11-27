using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;
using System;
using System.Collections.Generic; // Added for List<object>
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration; // ✅ 1. Required for appsettings.json

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

        // ✅ 2. Declare the Configuration field
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
            // ✅ 3. Inject IConfiguration
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

            // ✅ 4. Assign it
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

        public string PlaceGuestOrder(SalesOrderHeader orderData)
        {
            // 1. PRE-CALCULATION (Read-Only, outside transaction)
            var variant = _productVariantDataAccess.Get(orderData.ProductVariantId);
            if (variant == null) throw new Exception("Variant not found.");

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

        #endregion
    }
}