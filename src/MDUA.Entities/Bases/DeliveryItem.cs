using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "DeliveryItem", Namespace = "http://www.piistech.com//entities")]
	public partial class DeliveryItem : DeliveryItemBase
	{
		#region Exernal Properties
		private Delivery _DeliveryIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="Delivery"/>.
		/// </summary>
		/// <value>The source Delivery for _DeliveryIdObject.</value>
		[DataMember]
		public Delivery DeliveryIdObject
      	{
            get { return this._DeliveryIdObject; }
            set { this._DeliveryIdObject = value; }
      	}
		
		private SalesOrderDetail _SalesOrderDetailIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="SalesOrderDetail"/>.
		/// </summary>
		/// <value>The source SalesOrderDetail for _SalesOrderDetailIdObject.</value>
		[DataMember]
		public SalesOrderDetail SalesOrderDetailIdObject
      	{
            get { return this._SalesOrderDetailIdObject; }
            set { this._SalesOrderDetailIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(DeliveryItem))
            {
                return false;
            }			
			
			 DeliveryItem _paramObj = obj as DeliveryItem;
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
