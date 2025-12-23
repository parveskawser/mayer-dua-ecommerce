using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "ChatMessage", Namespace = "http://www.piistech.com//entities")]
	public partial class ChatMessage : ChatMessageBase
	{
		#region Exernal Properties
		private ChatSession _ChatSessionIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="ChatSession"/>.
		/// </summary>
		/// <value>The source ChatSession for _ChatSessionIdObject.</value>
		[DataMember]
		public ChatSession ChatSessionIdObject
      	{
            get { return this._ChatSessionIdObject; }
            set { this._ChatSessionIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(ChatMessage))
            {
                return false;
            }			
			
			 ChatMessage _paramObj = obj as ChatMessage;
            if (_paramObj != null)
            {			
                return (_paramObj.Id == this.Id && _paramObj.CustomPropertyMatch(this));
            }
            else
            {
                return base.Equals(obj);
            }
		}
		#endregion
		
		#region Orverride HashCode
		 public override int GetHashCode()
        {
            return base.Id.GetHashCode();
        }
		#endregion		
	}
}