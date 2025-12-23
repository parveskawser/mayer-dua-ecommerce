using System;
using System.Collections.Generic;
using MDUA.Entities;
using MDUA.Entities.List;

namespace MDUA.Facade.Interface
{
    public interface IPaymentMethodFacade
    {
        /// <summary>
        /// Retrieves all Payment Methods from the database
        /// </summary>
        PaymentMethodList GetAll();

        /// <summary>
        /// Retrieves a specific Payment Method by ID
        /// </summary>
        PaymentMethod Get(int id);

    }
}