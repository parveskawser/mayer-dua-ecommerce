using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "CustomerList", Namespace = "http://www.piistech.com//list")]	
	public class CustomerList : BaseCollection<Customer>
	{
		#region Constructors
	    public CustomerList() : base() { }
        public CustomerList(Customer[] list) : base(list) { }
        public CustomerList(List<Customer> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
