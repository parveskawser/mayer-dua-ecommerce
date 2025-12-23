using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "SalesChannelList", Namespace = "http://www.piistech.com//list")]	
	public class SalesChannelList : BaseCollection<SalesChannel>
	{
		#region Constructors
	    public SalesChannelList() : base() { }
        public SalesChannelList(SalesChannel[] list) : base(list) { }
        public SalesChannelList(List<SalesChannel> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
