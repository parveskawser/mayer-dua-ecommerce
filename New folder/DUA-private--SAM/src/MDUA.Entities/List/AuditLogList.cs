using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "AuditLogList", Namespace = "http://www.piistech.com//list")]	
	public class AuditLogList : BaseCollection<AuditLog>
	{
		#region Constructors
	    public AuditLogList() : base() { }
        public AuditLogList(AuditLog[] list) : base(list) { }
        public AuditLogList(List<AuditLog> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
