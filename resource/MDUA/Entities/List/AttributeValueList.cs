using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "AttributeValueList", Namespace = "http://www.piistech.com//list")]	
	public class AttributeValueList : BaseCollection<AttributeValue>
	{
		#region Constructors
	    public AttributeValueList() : base() { }
        public AttributeValueList(AttributeValue[] list) : base(list) { }
        public AttributeValueList(List<AttributeValue> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
