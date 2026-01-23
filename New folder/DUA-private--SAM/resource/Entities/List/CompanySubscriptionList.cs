using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "CompanySubscriptionList", Namespace = "http://www.piistech.com//list")]	
	public class CompanySubscriptionList : BaseCollection<CompanySubscription>
	{
		#region Constructors
	    public CompanySubscriptionList() : base() { }
        public CompanySubscriptionList(CompanySubscription[] list) : base(list) { }
        public CompanySubscriptionList(List<CompanySubscription> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
