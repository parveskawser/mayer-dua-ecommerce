using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "CmsPageList", Namespace = "http://www.piistech.com//list")]	
	public class CmsPageList : BaseCollection<CmsPage>
	{
		#region Constructors
	    public CmsPageList() : base() { }
        public CmsPageList(CmsPage[] list) : base(list) { }
        public CmsPageList(List<CmsPage> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
