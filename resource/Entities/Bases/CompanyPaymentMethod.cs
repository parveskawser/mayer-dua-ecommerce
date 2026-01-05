using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "CompanyPaymentMethod", Namespace = "http://www.piistech.com//entities")]
	public partial class CompanyPaymentMethod : CompanyPaymentMethodBase
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
		
		private PaymentMethod _PaymentMethodIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="PaymentMethod"/>.
		/// </summary>
		/// <value>The source PaymentMethod for _PaymentMethodIdObject.</value>
		[DataMember]
		public PaymentMethod PaymentMethodIdObject
      	{
            get { return this._PaymentMethodIdObject; }
            set { this._PaymentMethodIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(CompanyPaymentMethod))
            {
                return false;
            }			
			
			 CompanyPaymentMethod _paramObj = obj as CompanyPaymentMethod;
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