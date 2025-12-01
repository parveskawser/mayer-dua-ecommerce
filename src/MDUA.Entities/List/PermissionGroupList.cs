using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "PermissionGroupList", Namespace = "http://www.piistech.com//list")]	
	public class PermissionGroupList : BaseCollection<PermissionGroup>
	{
		#region Constructors
	    public PermissionGroupList() : base() { }
        public PermissionGroupList(PermissionGroup[] list) : base(list) { }
        public PermissionGroupList(List<PermissionGroup> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
