using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "ChatSession", Namespace = "http://www.piistech.com//entities")]
	public partial class ChatSession : ChatSessionBase
	{
		#region Exernal Properties
		private UserLogin _UserLoginIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="UserLogin"/>.
		/// </summary>
		/// <value>The source UserLogin for _UserLoginIdObject.</value>
		[DataMember]
		public UserLogin UserLoginIdObject
      	{
            get { return this._UserLoginIdObject; }
            set { this._UserLoginIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(ChatSession))
            {
                return false;
            }			
			
			 ChatSession _paramObj = obj as ChatSession;
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