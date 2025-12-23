using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "GlobalSettingList", Namespace = "http://www.piistech.com//list")]	
	public class GlobalSettingList : BaseCollection<GlobalSetting>
	{
		#region Constructors
	    public GlobalSettingList() : base() { }
        public GlobalSettingList(GlobalSetting[] list) : base(list) { }
        public GlobalSettingList(List<GlobalSetting> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
