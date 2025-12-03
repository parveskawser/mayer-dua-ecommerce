using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "Address", Namespace = "http://www.piistech.com//entities")]
	public partial class Address : AddressBase
	{
		#region Exernal Properties
		private Customer _CustomerIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="Customer"/>.
		/// </summary>
		/// <value>The source Customer for _CustomerIdObject.</value>
		[DataMember]
		public Customer CustomerIdObject
      	{
            get { return this._CustomerIdObject; }
            set { this._CustomerIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(Address))
            {
                return false;
            }			
			
			 Address _paramObj = obj as Address;
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
