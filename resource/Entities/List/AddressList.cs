using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "AddressList", Namespace = "http://www.piistech.com//list")]	
	public class AddressList : BaseCollection<Address>
	{
		#region Constructors
	    public AddressList() : base() { }
        public AddressList(Address[] list) : base(list) { }
        public AddressList(List<Address> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
