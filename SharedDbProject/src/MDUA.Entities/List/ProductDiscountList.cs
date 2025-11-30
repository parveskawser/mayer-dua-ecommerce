using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "ProductDiscountList", Namespace = "http://www.piistech.com//list")]	
	public class ProductDiscountList : BaseCollection<ProductDiscount>
	{
		#region Constructors
	    public ProductDiscountList() : base() { }
        public ProductDiscountList(ProductDiscount[] list) : base(list) { }
        public ProductDiscountList(List<ProductDiscount> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
