using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "PaymentAllocationList", Namespace = "http://www.piistech.com//list")]	
	public class PaymentAllocationList : BaseCollection<PaymentAllocation>
	{
		#region Constructors
	    public PaymentAllocationList() : base() { }
        public PaymentAllocationList(PaymentAllocation[] list) : base(list) { }
        public PaymentAllocationList(List<PaymentAllocation> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
