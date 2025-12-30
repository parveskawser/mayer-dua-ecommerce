using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "SalesOrderDetail", Namespace = "http://www.piistech.com//entities")]
	public partial class SalesOrderDetail : SalesOrderDetailBase
	{
		#region Exernal Properties
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
		
		private ProductVariant _ProductIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="ProductVariant"/>.
		/// </summary>
		/// <value>The source ProductVariant for _ProductIdObject.</value>
		[DataMember]
		public ProductVariant ProductIdObject
      	{
            get { return this._ProductIdObject; }
            set { this._ProductIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(SalesOrderDetail))
            {
                return false;
            }			
			
			 SalesOrderDetail _paramObj = obj as SalesOrderDetail;
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