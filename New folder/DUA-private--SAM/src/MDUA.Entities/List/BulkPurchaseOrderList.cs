using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "BulkPurchaseOrderList", Namespace = "http://www.piistech.com//list")]	
	public class BulkPurchaseOrderList : BaseCollection<BulkPurchaseOrder>
	{
		#region Constructors
	    public BulkPurchaseOrderList() : base() { }
        public BulkPurchaseOrderList(BulkPurchaseOrder[] list) : base(list) { }
        public BulkPurchaseOrderList(List<BulkPurchaseOrder> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
