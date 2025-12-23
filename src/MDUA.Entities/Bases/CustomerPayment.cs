using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "CustomerPayment", Namespace = "http://www.piistech.com//entities")]
	public partial class CustomerPayment : CustomerPaymentBase
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
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(CustomerPayment))
            {
                return false;
            }			
			
			 CustomerPayment _paramObj = obj as CustomerPayment;
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
