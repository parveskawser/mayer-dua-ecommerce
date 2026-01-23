using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "VariantPriceStock", Namespace = "http://www.piistech.com//entities")]
	public partial class VariantPriceStock : VariantPriceStockBase
	{
		#region Exernal Properties
		private ProductVariant _IdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="ProductVariant"/>.
		/// </summary>
		/// <value>The source ProductVariant for _IdObject.</value>
		[DataMember]
		public ProductVariant IdObject
      	{
            get { return this._IdObject; }
            set { this._IdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(VariantPriceStock))
            {
                return false;
            }			
			
			 VariantPriceStock _paramObj = obj as VariantPriceStock;
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
