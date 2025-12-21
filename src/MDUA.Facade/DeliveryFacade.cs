using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Facade.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MDUA.Facade
{
    public class DeliveryFacade : IDeliveryFacade
    {
        private readonly IDeliveryDataAccess _deliveryDataAccess;
        // We might need Order DataAccess if we need to cross-reference, 
        // but typically DeliveryDataAccess handles the join.
        
        public DeliveryFacade(IDeliveryDataAccess deliveryDataAccess)
        {
            _deliveryDataAccess = deliveryDataAccess;
        }

        public IList<Delivery> GetAllDeliveries()
        {
            // Call the Data Layer to get the list
            // Ensure your DataAccess layer returns the "Graph" (Delivery + Items + Order Header)
            // If your DataAccess only returns flat tables, you might need to stitch them here.
            
            return _deliveryDataAccess.LoadAllWithDetails();
        }

    }
}