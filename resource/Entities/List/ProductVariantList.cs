using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "ProductVariantList", Namespace = "http://www.piistech.com//list")]	
	public class ProductVariantList : BaseCollection<ProductVariant>
	{
		#region Constructors
	    public ProductVariantList() : base() { }
        public ProductVariantList(ProductVariant[] list) : base(list) { }
        public ProductVariantList(List<ProductVariant> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
