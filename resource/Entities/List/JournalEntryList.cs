using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "JournalEntryList", Namespace = "http://www.piistech.com//list")]	
	public class JournalEntryList : BaseCollection<JournalEntry>
	{
		#region Constructors
	    public JournalEntryList() : base() { }
        public JournalEntryList(JournalEntry[] list) : base(list) { }
        public JournalEntryList(List<JournalEntry> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
