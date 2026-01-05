using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "VariantImageList", Namespace = "http://www.piistech.com//list")]	
	public class VariantImageList : BaseCollection<VariantImage>
	{
		#region Constructors
	    public VariantImageList() : base() { }
        public VariantImageList(VariantImage[] list) : base(list) { }
        public VariantImageList(List<VariantImage> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
