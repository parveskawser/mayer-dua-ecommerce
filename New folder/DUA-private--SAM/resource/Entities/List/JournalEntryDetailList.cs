using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "JournalEntryDetailList", Namespace = "http://www.piistech.com//list")]	
	public class JournalEntryDetailList : BaseCollection<JournalEntryDetail>
	{
		#region Constructors
	    public JournalEntryDetailList() : base() { }
        public JournalEntryDetailList(JournalEntryDetail[] list) : base(list) { }
        public JournalEntryDetailList(List<JournalEntryDetail> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
