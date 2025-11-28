using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "VariantAttributeValueList", Namespace = "http://www.piistech.com//list")]	
	public class VariantAttributeValueList : BaseCollection<VariantAttributeValue>
	{
		#region Constructors
	    public VariantAttributeValueList() : base() { }
        public VariantAttributeValueList(VariantAttributeValue[] list) : base(list) { }
        public VariantAttributeValueList(List<VariantAttributeValue> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
