using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "ProductAttributeList", Namespace = "http://www.piistech.com//list")]	
	public class ProductAttributeList : BaseCollection<ProductAttribute>
	{
		#region Constructors
	    public ProductAttributeList() : base() { }
        public ProductAttributeList(ProductAttribute[] list) : base(list) { }
        public ProductAttributeList(List<ProductAttribute> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
