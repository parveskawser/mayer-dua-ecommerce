using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "SalesReturnList", Namespace = "http://www.piistech.com//list")]	
	public class SalesReturnList : BaseCollection<SalesReturn>
	{
		#region Constructors
	    public SalesReturnList() : base() { }
        public SalesReturnList(SalesReturn[] list) : base(list) { }
        public SalesReturnList(List<SalesReturn> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
