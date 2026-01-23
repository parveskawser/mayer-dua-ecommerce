using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "BackInStockRequest", Namespace = "http://www.piistech.com//entities")]
	public partial class BackInStockRequest : BackInStockRequestBase
	{
		#region Exernal Properties
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
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(BackInStockRequest))
            {
                return false;
            }			
			
			 BackInStockRequest _paramObj = obj as BackInStockRequest;
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
