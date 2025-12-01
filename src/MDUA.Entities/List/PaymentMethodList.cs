using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "PaymentMethodList", Namespace = "http://www.piistech.com//list")]	
	public class PaymentMethodList : BaseCollection<PaymentMethod>
	{
		#region Constructors
	    public PaymentMethodList() : base() { }
        public PaymentMethodList(PaymentMethod[] list) : base(list) { }
        public PaymentMethodList(List<PaymentMethod> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
