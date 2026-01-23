using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "PaymentAllocation", Namespace = "http://www.piistech.com//entities")]
	public partial class PaymentAllocation : PaymentAllocationBase
	{
		#region Exernal Properties
		private CustomerPayment _CustomerPaymentIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="CustomerPayment"/>.
		/// </summary>
		/// <value>The source CustomerPayment for _CustomerPaymentIdObject.</value>
		[DataMember]
		public CustomerPayment CustomerPaymentIdObject
      	{
            get { return this._CustomerPaymentIdObject; }
            set { this._CustomerPaymentIdObject = value; }
      	}
		
		private SalesOrderHeader _SalesOrderIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="SalesOrderHeader"/>.
		/// </summary>
		/// <value>The source SalesOrderHeader for _SalesOrderIdObject.</value>
		[DataMember]
		public SalesOrderHeader SalesOrderIdObject
      	{
            get { return this._SalesOrderIdObject; }
            set { this._SalesOrderIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(PaymentAllocation))
            {
                return false;
            }			
			
			 PaymentAllocation _paramObj = obj as PaymentAllocation;
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