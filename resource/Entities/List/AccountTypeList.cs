using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "AccountTypeList", Namespace = "http://www.piistech.com//list")]	
	public class AccountTypeList : BaseCollection<AccountType>
	{
		#region Constructors
	    public AccountTypeList() : base() { }
        public AccountTypeList(AccountType[] list) : base(list) { }
        public AccountTypeList(List<AccountType> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
