using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	public partial class UserPermission 
	{
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? PermissionId { get; set; }          // optional extra feature
        public int? PermissionGroupId { get; set; }

    }
}
