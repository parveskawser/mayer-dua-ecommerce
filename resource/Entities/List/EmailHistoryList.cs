using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "EmailHistoryList", Namespace = "http://www.piistech.com//list")]	
	public class EmailHistoryList : BaseCollection<EmailHistory>
	{
		#region Constructors
	    public EmailHistoryList() : base() { }
        public EmailHistoryList(EmailHistory[] list) : base(list) { }
        public EmailHistoryList(List<EmailHistory> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
