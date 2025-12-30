using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "CustomerPaymentList", Namespace = "http://www.piistech.com//list")]	
	public class CustomerPaymentList : BaseCollection<CustomerPayment>
	{
		#region Constructors
	    public CustomerPaymentList() : base() { }
        public CustomerPaymentList(CustomerPayment[] list) : base(list) { }
        public CustomerPaymentList(List<CustomerPayment> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
