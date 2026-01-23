using System;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.List;
using MDUA.Facade.Interface;

namespace MDUA.Facade
{
    public class PaymentMethodFacade : IPaymentMethodFacade
    {
        private readonly IPaymentMethodDataAccess _paymentMethodDataAccess;

        // Constructor Injection
        public PaymentMethodFacade(IPaymentMethodDataAccess paymentMethodDataAccess)
        {
            _paymentMethodDataAccess = paymentMethodDataAccess;
        }

        public PaymentMethodList GetAll()
        {
            // Delegates the call to the Data Access Layer
            return _paymentMethodDataAccess.GetAll();
        }

        public PaymentMethod Get(int id)
        {
            return _paymentMethodDataAccess.Get(id);
        }
    }
}