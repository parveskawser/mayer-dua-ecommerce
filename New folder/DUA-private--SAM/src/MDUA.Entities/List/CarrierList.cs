using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "CarrierList", Namespace = "http://www.piistech.com//list")]	
	public class CarrierList : BaseCollection<Carrier>
	{
		#region Constructors
	    public CarrierList() : base() { }
        public CarrierList(Carrier[] list) : base(list) { }
        public CarrierList(List<Carrier> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
