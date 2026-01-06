using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "SubscriptionPlanList", Namespace = "http://www.piistech.com//list")]	
	public class SubscriptionPlanList : BaseCollection<SubscriptionPlan>
	{
		#region Constructors
	    public SubscriptionPlanList() : base() { }
        public SubscriptionPlanList(SubscriptionPlan[] list) : base(list) { }
        public SubscriptionPlanList(List<SubscriptionPlan> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
