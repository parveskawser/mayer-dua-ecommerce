using MDUA.Entities;
using System.Collections.Generic;

namespace MDUA.Facade.Interface
{
    public interface IDeliveryFacade
    {
        /// <summary>
        /// Retrieves all deliveries including their items and order details.
        /// </summary>
        IList<Delivery> GetAllDeliveries();

        /// <summary>
        /// Updates the status and tracking number of a specific delivery.
        /// </summary>
    }
}