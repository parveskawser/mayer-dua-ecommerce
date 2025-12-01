using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "ProductInventoryList", Namespace = "http://www.piistech.com//list")]	
	public class ProductInventoryList : BaseCollection<ProductInventory>
	{
		#region Constructors
	    public ProductInventoryList() : base() { }
        public ProductInventoryList(ProductInventory[] list) : base(list) { }
        public ProductInventoryList(List<ProductInventory> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
