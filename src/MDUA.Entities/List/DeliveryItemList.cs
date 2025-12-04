using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "DeliveryItemList", Namespace = "http://www.piistech.com//list")]	
	public class DeliveryItemList : BaseCollection<DeliveryItem>
	{
		#region Constructors
	    public DeliveryItemList() : base() { }
        public DeliveryItemList(DeliveryItem[] list) : base(list) { }
        public DeliveryItemList(List<DeliveryItem> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
