using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "CompanyList", Namespace = "http://www.piistech.com//list")]	
	public class CompanyList : BaseCollection<Company>
	{
		#region Constructors
	    public CompanyList() : base() { }
        public CompanyList(Company[] list) : base(list) { }
        public CompanyList(List<Company> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
