using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "DeliveryStatusLogList", Namespace = "http://www.piistech.com//list")]	
	public class DeliveryStatusLogList : BaseCollection<DeliveryStatusLog>
	{
		#region Constructors
	    public DeliveryStatusLogList() : base() { }
        public DeliveryStatusLogList(DeliveryStatusLog[] list) : base(list) { }
        public DeliveryStatusLogList(List<DeliveryStatusLog> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
