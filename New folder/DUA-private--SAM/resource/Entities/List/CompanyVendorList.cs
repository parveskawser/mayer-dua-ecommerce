using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "CompanyVendorList", Namespace = "http://www.piistech.com//list")]	
	public class CompanyVendorList : BaseCollection<CompanyVendor>
	{
		#region Constructors
	    public CompanyVendorList() : base() { }
        public CompanyVendorList(CompanyVendor[] list) : base(list) { }
        public CompanyVendorList(List<CompanyVendor> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
