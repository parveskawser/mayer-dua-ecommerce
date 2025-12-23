using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "ChatMessageList", Namespace = "http://www.piistech.com//list")]	
	public class ChatMessageList : BaseCollection<ChatMessage>
	{
		#region Constructors
	    public ChatMessageList() : base() { }
        public ChatMessageList(ChatMessage[] list) : base(list) { }
        public ChatMessageList(List<ChatMessage> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
