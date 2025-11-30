using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "AccountList", Namespace = "http://www.piistech.com//list")]	
	public class AccountList : BaseCollection<Account>
	{
		#region Constructors
	    public AccountList() : base() { }
        public AccountList(Account[] list) : base(list) { }
        public AccountList(List<Account> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
