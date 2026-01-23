using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "BackInStockRequestList", Namespace = "http://www.piistech.com//list")]	
	public class BackInStockRequestList : BaseCollection<BackInStockRequest>
	{
		#region Constructors
	    public BackInStockRequestList() : base() { }
        public BackInStockRequestList(BackInStockRequest[] list) : base(list) { }
        public BackInStockRequestList(List<BackInStockRequest> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
