using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "ProductImageList", Namespace = "http://www.piistech.com//list")]	
	public class ProductImageList : BaseCollection<ProductImage>
	{
		#region Constructors
	    public ProductImageList() : base() { }
        public ProductImageList(ProductImage[] list) : base(list) { }
        public ProductImageList(List<ProductImage> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
