using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "CompanyDomainList", Namespace = "http://www.piistech.com//list")]	
	public class CompanyDomainList : BaseCollection<CompanyDomain>
	{
		#region Constructors
	    public CompanyDomainList() : base() { }
        public CompanyDomainList(CompanyDomain[] list) : base(list) { }
        public CompanyDomainList(List<CompanyDomain> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
