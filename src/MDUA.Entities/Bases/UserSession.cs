using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "UserSession", Namespace = "http://www.piistech.com//entities")]
	public partial class UserSession : UserSessionBase
	{
		#region Exernal Properties
		private UserLogin _UserIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="UserLogin"/>.
		/// </summary>
		/// <value>The source UserLogin for _UserIdObject.</value>
		[DataMember]
		public UserLogin UserIdObject
      	{
            get { return this._UserIdObject; }
            set { this._UserIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(UserSession))
            {
                return false;
            }			
			
			 UserSession _paramObj = obj as UserSession;
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