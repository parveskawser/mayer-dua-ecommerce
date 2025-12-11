using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using System.Collections.Generic;

namespace MDUA.Facade
{
    public class PaymentFacade : IPaymentFacade
    {
        private readonly ICustomerPaymentDataAccess _customerPaymentDataAccess;
        private readonly IInventoryTransactionDataAccess _inventoryTransactionDataAccess;
        private readonly ISalesOrderDetailDataAccess _salesOrderDetailDataAccess;
        private readonly ICompanyPaymentMethodDataAccess _companyPaymentMethodDataAccess;

        public PaymentFacade(
            ICustomerPaymentDataAccess customerPaymentDA,
            IInventoryTransactionDataAccess inventoryTransactionDA,
            ISalesOrderDetailDataAccess salesOrderDetailDA,
            ICompanyPaymentMethodDataAccess companyPaymentMethodDA)
        {
            _customerPaymentDataAccess = customerPaymentDA;
            _inventoryTransactionDataAccess = inventoryTransactionDA;
            _salesOrderDetailDataAccess = salesOrderDetailDA;
            _companyPaymentMethodDataAccess = companyPaymentMethodDA;
        }

        // 1. Helper to populate Dropdown
        public List<CompanyPaymentMethod> GetActivePaymentMethods(int companyId)
        {
            // Implement query to join CompanyPaymentMethod + PaymentMethod tables
            return _companyPaymentMethodDataAccess.GetActiveByCompany(companyId);
        }

        // 2. The Main Logic
        public long AddPayment(CustomerPayment payment)
        {
            // 1. Get Order Details
            var orderDetail = _salesOrderDetailDataAccess.GetFirstDetailByOrderRef(payment.TransactionReference);

            int? invTrxId = null;

            // 2. Logic: Only create Inventory Transaction if we have a valid Product ID
            if (orderDetail != null && orderDetail.ProductVariantId > 0)
            {
                var invTrx = new InventoryTransaction
                {
                    SalesOrderDetailId = orderDetail.Id,
                    PoReceivedId = null,
                    InOut = "IN",
                    Date = DateTime.Now,
                    Price = payment.Amount,
                    Quantity = orderDetail.Quantity,
                    ProductVariantId = orderDetail.ProductVariantId, // Matches C# property
                    Remarks = "Payment: " + payment.Notes,
                    CreatedBy = payment.CreatedBy,
                    CreatedAt = DateTime.Now
                };

                long newId = _inventoryTransactionDataAccess.Insert(invTrx);
                if (newId > 0) invTrxId = (int)newId;
            }

            // 3. Assign ID (This handles the 0 vs Null issue)
            payment.InventoryTransactionId = invTrxId;

            // ✅ CRITICAL DEBUG: If invTrxId is null, make sure your DA handles it!
            // If your CustomerPayment object defines this as 'int' (not nullable), 
            // it will become 0 and crash. It MUST be nullable in the Entity or handled in DA.

            return _customerPaymentDataAccess.Insert(payment);
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