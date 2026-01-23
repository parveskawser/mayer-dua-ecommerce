using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "ProductReviewList", Namespace = "http://www.piistech.com//list")]	
	public class ProductReviewList : BaseCollection<ProductReview>
	{
		#region Constructors
	    public ProductReviewList() : base() { }
        public ProductReviewList(ProductReview[] list) : base(list) { }
        public ProductReviewList(List<ProductReview> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
