using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "LanguageResourceList", Namespace = "http://www.piistech.com//list")]	
	public class LanguageResourceList : BaseCollection<LanguageResource>
	{
		#region Constructors
	    public LanguageResourceList() : base() { }
        public LanguageResourceList(LanguageResource[] list) : base(list) { }
        public LanguageResourceList(List<LanguageResource> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
