using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "VendorPaymentList", Namespace = "http://www.piistech.com//list")]	
	public class VendorPaymentList : BaseCollection<VendorPayment>
	{
		#region Constructors
	    public VendorPaymentList() : base() { }
        public VendorPaymentList(VendorPayment[] list) : base(list) { }
        public VendorPaymentList(List<VendorPayment> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
