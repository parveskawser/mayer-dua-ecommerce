using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "ProductCategoryList", Namespace = "http://www.piistech.com//list")]	
	public class ProductCategoryList : BaseCollection<ProductCategory>
	{
		#region Constructors
	    public ProductCategoryList() : base() { }
        public ProductCategoryList(ProductCategory[] list) : base(list) { }
        public ProductCategoryList(List<ProductCategory> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
