using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "AttributeNameList", Namespace = "http://www.piistech.com//list")]	
	public class AttributeNameList : BaseCollection<AttributeName>
	{
		#region Constructors
	    public AttributeNameList() : base() { }
        public AttributeNameList(AttributeName[] list) : base(list) { }
        public AttributeNameList(List<AttributeName> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
