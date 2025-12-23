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
                var salesDetailDa = new SalesOrderDetailDataAccess(transaction);
                var invDa = new InventoryTransactionDataAccess(transaction);
                var paymentDa = new CustomerPaymentDataAccess(transaction);
                var orderDa = new SalesOrderHeaderDataAccess(transaction);

                // =====================================================
                // 1. FETCH ORDER HEADER (BY SALES ORDER REF)
                // =====================================================
                var orderHeader = orderDa.GetBySalesOrderRef(payment.TransactionReference);
                if (orderHeader == null)
                    throw new Exception("Invalid order reference.");

                // =====================================================
                // 2. CALCULATE CURRENT DUE (DB TRUTH)
                // =====================================================
                decimal alreadyPaid = paymentDa.GetTotalPaidByOrderRef(payment.TransactionReference);
                decimal netAmount = (decimal)orderHeader.NetAmount;
                decimal dueAmount = netAmount - alreadyPaid;

                // ❌ FULLY PAID
                if (dueAmount <= 0)
                    throw new Exception("Order is already fully paid. Payment not allowed.");

                // ❌ OVERPAYMENT
                if (payment.Amount > dueAmount)
                    throw new Exception($"Payment exceeds due amount. Due: {dueAmount}");

                // =====================================================
                // 3. UPDATE DELIVERY (OPTIONAL)
                // =====================================================
                if (deliveryCharge.HasValue)
                {
                    orderDa.UpdateOrderDeliveryCharge(orderHeader.Id, deliveryCharge.Value);

                    // Recalculate due AFTER delivery update
                    orderHeader = orderDa.GetBySalesOrderRef(payment.TransactionReference);
                    netAmount = (decimal)orderHeader.NetAmount;
                    dueAmount = netAmount - alreadyPaid;

                    if (payment.Amount > dueAmount)
                        throw new Exception($"Payment exceeds due amount after delivery update. Due: {dueAmount}");
                }

                // =====================================================
                // 4. INVENTORY TRANSACTION (IF PRODUCT EXISTS)
                // =====================================================
                var orderDetail = salesDetailDa.GetFirstDetailByOrderRef(payment.TransactionReference);
                int? inventoryTransactionId = null;

                if (orderDetail != null && orderDetail.ProductVariantId > 0)
                {
                    var invTrx = new InventoryTransaction
                    {
                        SalesOrderDetailId = orderDetail.Id,
                        ProductVariantId = orderDetail.ProductVariantId,
                        Quantity = orderDetail.Quantity,
                        Price = payment.Amount,
                        InOut = "IN",
                        Date = DateTime.UtcNow,
                        Remarks = "Payment: " + payment.Notes,
                        CreatedBy = payment.CreatedBy,
                        CreatedAt = DateTime.UtcNow
                    };

                    long invId = invDa.Insert(invTrx);
                    if (invId > 0) inventoryTransactionId = (int)invId;
                }

                payment.InventoryTransactionId = inventoryTransactionId;

                // =====================================================
                // 5. INSERT PAYMENT
                // =====================================================
                long paymentId = paymentDa.Insert(payment);

                BaseDataAccess.CloseTransaction(true, transaction);
                return paymentId;
            }
            catch
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