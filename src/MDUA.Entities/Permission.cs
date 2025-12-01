using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	public partial class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ActionName { get; set; } // <-- add this
        public int? PermissionId { get; set; }


    }
}
