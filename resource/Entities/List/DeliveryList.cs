using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "DeliveryList", Namespace = "http://www.piistech.com//list")]	
	public class DeliveryList : BaseCollection<Delivery>
	{
		#region Constructors
	    public DeliveryList() : base() { }
        public DeliveryList(Delivery[] list) : base(list) { }
        public DeliveryList(List<Delivery> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
