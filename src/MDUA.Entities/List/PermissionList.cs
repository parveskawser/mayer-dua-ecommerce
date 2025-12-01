using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "PermissionList", Namespace = "http://www.piistech.com//list")]	
	public class PermissionList : BaseCollection<Permission>
	{
		#region Constructors
	    public PermissionList() : base() { }
        public PermissionList(Permission[] list) : base(list) { }
        public PermissionList(List<Permission> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
