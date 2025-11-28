using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "ProductList", Namespace = "http://www.piistech.com//list")]	
	public class ProductList : BaseCollection<Product>
	{
		#region Constructors
	    public ProductList() : base() { }
        public ProductList(Product[] list) : base(list) { }
        public ProductList(List<Product> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
