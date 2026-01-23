using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "ProductSEOList", Namespace = "http://www.piistech.com//list")]	
	public class ProductSEOList : BaseCollection<ProductSEO>
	{
		#region Constructors
	    public ProductSEOList() : base() { }
        public ProductSEOList(ProductSEO[] list) : base(list) { }
        public ProductSEOList(List<ProductSEO> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
