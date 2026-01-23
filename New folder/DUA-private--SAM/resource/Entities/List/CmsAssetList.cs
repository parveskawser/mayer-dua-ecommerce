using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "CmsAssetList", Namespace = "http://www.piistech.com//list")]	
	public class CmsAssetList : BaseCollection<CmsAsset>
	{
		#region Constructors
	    public CmsAssetList() : base() { }
        public CmsAssetList(CmsAsset[] list) : base(list) { }
        public CmsAssetList(List<CmsAsset> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
