using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "PermissionGroupMapList", Namespace = "http://www.piistech.com//list")]	
	public class PermissionGroupMapList : BaseCollection<PermissionGroupMap>
	{
		#region Constructors
	    public PermissionGroupMapList() : base() { }
        public PermissionGroupMapList(PermissionGroupMap[] list) : base(list) { }
        public PermissionGroupMapList(List<PermissionGroupMap> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
