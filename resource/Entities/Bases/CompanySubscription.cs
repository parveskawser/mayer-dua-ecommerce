using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "CompanySubscription", Namespace = "http://www.piistech.com//entities")]
	public partial class CompanySubscription : CompanySubscriptionBase
	{
		#region Exernal Properties
		private Company _CompanyIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="Company"/>.
		/// </summary>
		/// <value>The source Company for _CompanyIdObject.</value>
		[DataMember]
		public Company CompanyIdObject
      	{
            get { return this._CompanyIdObject; }
            set { this._CompanyIdObject = value; }
      	}
		
		private SubscriptionPlan _SubscriptionPlanIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="SubscriptionPlan"/>.
		/// </summary>
		/// <value>The source SubscriptionPlan for _SubscriptionPlanIdObject.</value>
		[DataMember]
		public SubscriptionPlan SubscriptionPlanIdObject
      	{
            get { return this._SubscriptionPlanIdObject; }
            set { this._SubscriptionPlanIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(CompanySubscription))
            {
                return false;
            }			
			
			 CompanySubscription _paramObj = obj as CompanySubscription;
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