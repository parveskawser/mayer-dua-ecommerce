using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "CompanyCustomerList", Namespace = "http://www.piistech.com//list")]	
	public class CompanyCustomerList : BaseCollection<CompanyCustomer>
	{
		#region Constructors
	    public CompanyCustomerList() : base() { }
        public CompanyCustomerList(CompanyCustomer[] list) : base(list) { }
        public CompanyCustomerList(List<CompanyCustomer> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
