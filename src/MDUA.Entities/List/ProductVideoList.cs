using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "ProductVideoList", Namespace = "http://www.piistech.com//list")]	
	public class ProductVideoList : BaseCollection<ProductVideo>
	{
		#region Constructors
	    public ProductVideoList() : base() { }
        public ProductVideoList(ProductVideo[] list) : base(list) { }
        public ProductVideoList(List<ProductVideo> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
