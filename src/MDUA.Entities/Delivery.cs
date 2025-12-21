using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	public partial class Delivery 
	{
        [DataMember] public string CarrierName { get; set; }
        [DataMember] public DateTime? ShipDate { get; set; }
        [DataMember] public DateTime? EstimatedArrival { get; set; }
        [DataMember] public DateTime? ActualDeliveryDate { get; set; }

        // ✅ The Key Field for Profit Calculation
        [DataMember] public decimal? ShippingCost { get; set; }
        [DataMember]
        public virtual SalesOrderHeader SalesOrderHeader { get; set; }

        // Links to the items inside this delivery
        [DataMember]
        public virtual IList<DeliveryItem> DeliveryItems { get; set; }
    }
}
