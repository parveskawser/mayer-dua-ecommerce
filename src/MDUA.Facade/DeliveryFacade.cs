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
       

        public DeliveryFacade(IDeliveryDataAccess deliveryDataAccess)
        {
            _deliveryDataAccess = deliveryDataAccess;
        }
        // Inside MDUA.Facade/DeliveryFacade.cs

        public Delivery Get(int id)
        {
         
            if (_deliveryDataAccess is MDUA.DataAccess.DeliveryDataAccess concreteDA)
            {
                return concreteDA.GetExtended(id);
            }

            return _deliveryDataAccess.Get(id);
        }
        public IList<Delivery> GetAllDeliveries()
        {
            

            return _deliveryDataAccess.LoadAllWithDetails();
        }
    
    }
}