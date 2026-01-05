using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "Account", Namespace = "http://www.piistech.com//entities")]
	public partial class Account : AccountBase
	{
		#region Exernal Properties
		private AccountType _AccountTypeIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="AccountType"/>.
		/// </summary>
		/// <value>The source AccountType for _AccountTypeIdObject.</value>
		[DataMember]
		public AccountType AccountTypeIdObject
      	{
            get { return this._AccountTypeIdObject; }
            set { this._AccountTypeIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(Account))
            {
                return false;
            }			
			
			 Account _paramObj = obj as Account;
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