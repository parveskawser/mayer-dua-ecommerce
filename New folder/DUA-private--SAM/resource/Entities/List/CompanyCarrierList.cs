using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "CompanyCarrierList", Namespace = "http://www.piistech.com//list")]	
	public class CompanyCarrierList : BaseCollection<CompanyCarrier>
	{
		#region Constructors
	    public CompanyCarrierList() : base() { }
        public CompanyCarrierList(CompanyCarrier[] list) : base(list) { }
        public CompanyCarrierList(List<CompanyCarrier> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
