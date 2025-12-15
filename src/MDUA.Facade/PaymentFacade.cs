using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework.DataAccess;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MDUA.Facade
{
    public class PaymentFacade : IPaymentFacade
    {
        private readonly ICustomerPaymentDataAccess _customerPaymentDataAccess;
        private readonly IInventoryTransactionDataAccess _inventoryTransactionDataAccess;
        private readonly ISalesOrderDetailDataAccess _salesOrderDetailDataAccess;
        private readonly ICompanyPaymentMethodDataAccess _companyPaymentMethodDataAccess;
        private readonly IConfiguration _configuration;
         private readonly ISalesOrderHeaderDataAccess _orderDA;
         private readonly ICustomerPaymentDataAccess _paymentDA;


        public PaymentFacade(
            ICustomerPaymentDataAccess customerPaymentDA,
            IInventoryTransactionDataAccess inventoryTransactionDA,
            ISalesOrderDetailDataAccess salesOrderDetailDA,
            ICompanyPaymentMethodDataAccess companyPaymentMethodDA, IConfiguration configuration, ISalesOrderHeaderDataAccess orderDA, ICustomerPaymentDataAccess paymentDA)
        {
            _customerPaymentDataAccess = customerPaymentDA;
            _inventoryTransactionDataAccess = inventoryTransactionDA;
            _salesOrderDetailDataAccess = salesOrderDetailDA;
            _companyPaymentMethodDataAccess = companyPaymentMethodDA;
            _configuration = configuration;
            _orderDA = orderDA;
            _paymentDA = paymentDA;
        }

        // 1. Helper to populate Dropdown
        public List<CompanyPaymentMethod> GetActivePaymentMethods(int companyId)
        {
            // Implement query to join CompanyPaymentMethod + PaymentMethod tables
            return _companyPaymentMethodDataAccess.GetActiveByCompany(companyId);
        }

        // 2. The Main Logic
        public long AddPayment(CustomerPayment payment, decimal? deliveryCharge = null)
        {
            string connString = _configuration.GetConnectionString("DefaultConnection");
            SqlTransaction transaction = BaseDataAccess.BeginTransaction(connString);

            try
            {
                var salesDa = new SalesOrderDetailDataAccess(transaction);
                var invDa = new InventoryTransactionDataAccess(transaction);
                var custPayDa = new CustomerPaymentDataAccess(transaction);

                // ✅ NEW: Initialize Order Header DA for update
                var orderDa = new SalesOrderHeaderDataAccess(transaction);

                // --- Business Logic ---

                // 1. Get Order Detail to link Inventory & find Order ID
                var orderDetail = salesDa.GetFirstDetailByOrderRef(payment.TransactionReference);
                int? invTrxId = null;

                if (orderDetail != null)
                {
                    // 2. ✅ Update Delivery Charge & Total Amount Logic
                    if (deliveryCharge.HasValue)
                    {
                        int orderId = orderDetail.SalesOrderId; // FK from detail

                        // A. Calculate Product Cost (Sum of all lines)
                        // Note: Using orderDA with the ACTIVE transaction
                        decimal productTotal = orderDa.GetProductTotalFromDetails(orderId);

                        // B. New Total = Product Cost + New Delivery
                        decimal newTotalAmount = productTotal + deliveryCharge.Value;

                        // C. Update DB
                        orderDa.UpdateTotalAmountSafe(orderId, newTotalAmount);
                    }

                    // 3. Inventory Transaction Logic (Existing)
                    if (orderDetail.ProductVariantId > 0)
                    {
                        var invTrx = new InventoryTransaction
                        {
                            SalesOrderDetailId = orderDetail.Id,
                            InOut = "IN",
                            Date = DateTime.Now,
                            Price = payment.Amount,
                            Quantity = orderDetail.Quantity,
                            ProductVariantId = orderDetail.ProductVariantId,
                            Remarks = "Payment: " + payment.Notes,
                            CreatedBy = payment.CreatedBy,
                            CreatedAt = DateTime.Now
                        };
                        long newId = invDa.Insert(invTrx);
                        if (newId > 0) invTrxId = (int)newId;
                    }
                }

                payment.InventoryTransactionId = invTrxId;

                // 4. Insert Payment
                long paymentId = custPayDa.Insert(payment);

                BaseDataAccess.CloseTransaction(true, transaction);
                return paymentId;
            }
            catch (Exception ex)
            {
                BaseDataAccess.CloseTransaction(false, transaction);
                throw;
            }
        }

        public long Insert(CustomerPaymentBase entity)
        {
            return _customerPaymentDataAccess.Insert(entity);
        }

        public long Update(CustomerPaymentBase entity)
        {
            return _customerPaymentDataAccess.Update(entity);
        }

        public long Delete(int id)
        {
            return _customerPaymentDataAccess.Delete(id);
        }

        public CustomerPayment Get(int id)
        {
            return _customerPaymentDataAccess.Get(id);
        }

        public CustomerPaymentList GetAll()
        {
            return _customerPaymentDataAccess.GetAll();
        }

        public CustomerPaymentList GetByQuery(string query)
        {
            return _customerPaymentDataAccess.GetByQuery(query);
        }


    }
}