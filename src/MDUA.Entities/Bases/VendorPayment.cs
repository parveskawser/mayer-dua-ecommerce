using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "VendorPayment", Namespace = "http://www.piistech.com//entities")]
	public partial class VendorPayment : VendorPaymentBase
	{
		#region Exernal Properties
		private InventoryTransaction _InventoryTransactionIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="InventoryTransaction"/>.
		/// </summary>
		/// <value>The source InventoryTransaction for _InventoryTransactionIdObject.</value>
		[DataMember]
		public InventoryTransaction InventoryTransactionIdObject
      	{
            get { return this._InventoryTransactionIdObject; }
            set { this._InventoryTransactionIdObject = value; }
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
		
		private CompanyVendor _VendorIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="CompanyVendor"/>.
		/// </summary>
		/// <value>The source CompanyVendor for _VendorIdObject.</value>
		[DataMember]
		public CompanyVendor VendorIdObject
      	{
            get { return this._VendorIdObject; }
            set { this._VendorIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(VendorPayment))
            {
                return false;
            }			
			
			 VendorPayment _paramObj = obj as VendorPayment;
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
