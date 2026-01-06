using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "SubscriptionUsage", Namespace = "http://www.piistech.com//entities")]
	public partial class SubscriptionUsage : SubscriptionUsageBase
	{
		#region Exernal Properties
		private CompanySubscription _SubscriptionIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="CompanySubscription"/>.
		/// </summary>
		/// <value>The source CompanySubscription for _SubscriptionIdObject.</value>
		[DataMember]
		public CompanySubscription SubscriptionIdObject
      	{
            get { return this._SubscriptionIdObject; }
            set { this._SubscriptionIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(SubscriptionUsage))
            {
                return false;
            }			
			
			 SubscriptionUsage _paramObj = obj as SubscriptionUsage;
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