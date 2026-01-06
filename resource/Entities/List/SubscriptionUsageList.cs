using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "SubscriptionUsageList", Namespace = "http://www.piistech.com//list")]	
	public class SubscriptionUsageList : BaseCollection<SubscriptionUsage>
	{
		#region Constructors
	    public SubscriptionUsageList() : base() { }
        public SubscriptionUsageList(SubscriptionUsage[] list) : base(list) { }
        public SubscriptionUsageList(List<SubscriptionUsage> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
