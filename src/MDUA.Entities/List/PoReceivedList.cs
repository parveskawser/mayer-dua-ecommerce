using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "PoReceivedList", Namespace = "http://www.piistech.com//list")]	
	public class PoReceivedList : BaseCollection<PoReceived>
	{
		#region Constructors
	    public PoReceivedList() : base() { }
        public PoReceivedList(PoReceived[] list) : base(list) { }
        public PoReceivedList(List<PoReceived> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
