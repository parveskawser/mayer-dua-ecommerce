using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "InventoryTransaction", Namespace = "http://www.piistech.com//entities")]
	public partial class InventoryTransaction : InventoryTransactionBase
	{
		#region Exernal Properties
		private PoReceived _PoReceivedIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="PoReceived"/>.
		/// </summary>
		/// <value>The source PoReceived for _PoReceivedIdObject.</value>
		[DataMember]
		public PoReceived PoReceivedIdObject
      	{
            get { return this._PoReceivedIdObject; }
            set { this._PoReceivedIdObject = value; }
      	}
		
		private ProductVariant _ProductVariantIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="ProductVariant"/>.
		/// </summary>
		/// <value>The source ProductVariant for _ProductVariantIdObject.</value>
		[DataMember]
		public ProductVariant ProductVariantIdObject
      	{
            get { return this._ProductVariantIdObject; }
            set { this._ProductVariantIdObject = value; }
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
			if (obj.GetType() != typeof(InventoryTransaction))
            {
                return false;
            }			
			
			 InventoryTransaction _paramObj = obj as InventoryTransaction;
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