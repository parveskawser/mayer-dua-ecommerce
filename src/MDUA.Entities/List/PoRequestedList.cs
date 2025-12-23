using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "PoRequestedList", Namespace = "http://www.piistech.com//list")]	
	public class PoRequestedList : BaseCollection<PoRequested>
	{
		#region Constructors
	    public PoRequestedList() : base() { }
        public PoRequestedList(PoRequested[] list) : base(list) { }
        public PoRequestedList(List<PoRequested> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
