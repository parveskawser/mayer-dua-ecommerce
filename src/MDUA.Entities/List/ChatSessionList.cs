using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "ChatSessionList", Namespace = "http://www.piistech.com//list")]	
	public class ChatSessionList : BaseCollection<ChatSession>
	{
		#region Constructors
	    public ChatSessionList() : base() { }
        public ChatSessionList(ChatSession[] list) : base(list) { }
        public ChatSessionList(List<ChatSession> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
