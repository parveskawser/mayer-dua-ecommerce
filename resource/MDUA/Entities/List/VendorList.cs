using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "VendorList", Namespace = "http://www.piistech.com//list")]	
	public class VendorList : BaseCollection<Vendor>
	{
		#region Constructors
	    public VendorList() : base() { }
        public VendorList(Vendor[] list) : base(list) { }
        public VendorList(List<Vendor> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
