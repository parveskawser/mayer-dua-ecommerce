using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "UserSessionList", Namespace = "http://www.piistech.com//list")]	
	public class UserSessionList : BaseCollection<UserSession>
	{
		#region Constructors
	    public UserSessionList() : base() { }
        public UserSessionList(UserSession[] list) : base(list) { }
        public UserSessionList(List<UserSession> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
