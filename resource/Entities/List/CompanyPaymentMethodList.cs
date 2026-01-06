using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "CompanyPaymentMethodList", Namespace = "http://www.piistech.com//list")]	
	public class CompanyPaymentMethodList : BaseCollection<CompanyPaymentMethod>
	{
		#region Constructors
	    public CompanyPaymentMethodList() : base() { }
        public CompanyPaymentMethodList(CompanyPaymentMethod[] list) : base(list) { }
        public CompanyPaymentMethodList(List<CompanyPaymentMethod> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
