using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "InventoryTransactionList", Namespace = "http://www.piistech.com//list")]	
	public class InventoryTransactionList : BaseCollection<InventoryTransaction>
	{
		#region Constructors
	    public InventoryTransactionList() : base() { }
        public InventoryTransactionList(InventoryTransaction[] list) : base(list) { }
        public InventoryTransactionList(List<InventoryTransaction> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
