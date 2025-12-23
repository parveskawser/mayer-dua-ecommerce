using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "VariantPriceStockList", Namespace = "http://www.piistech.com//list")]	
	public class VariantPriceStockList : BaseCollection<VariantPriceStock>
	{
		#region Constructors
	    public VariantPriceStockList() : base() { }
        public VariantPriceStockList(VariantPriceStock[] list) : base(list) { }
        public VariantPriceStockList(List<VariantPriceStock> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
