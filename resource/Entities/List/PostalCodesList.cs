using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "PostalCodesList", Namespace = "http://www.piistech.com//list")]	
	public class PostalCodesList : BaseCollection<PostalCodes>
	{
		#region Constructors
	    public PostalCodesList() : base() { }
        public PostalCodesList(PostalCodes[] list) : base(list) { }
        public PostalCodesList(List<PostalCodes> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
