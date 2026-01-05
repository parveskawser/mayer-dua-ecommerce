using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "ProductAttribute", Namespace = "http://www.piistech.com//entities")]
	public partial class ProductAttribute : ProductAttributeBase
	{
		#region Exernal Properties
		private AttributeName _AttributeIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="AttributeName"/>.
		/// </summary>
		/// <value>The source AttributeName for _AttributeIdObject.</value>
		[DataMember]
		public AttributeName AttributeIdObject
      	{
            get { return this._AttributeIdObject; }
            set { this._AttributeIdObject = value; }
      	}
		
		private Product _ProductIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="Product"/>.
		/// </summary>
		/// <value>The source Product for _ProductIdObject.</value>
		[DataMember]
		public Product ProductIdObject
      	{
            get { return this._ProductIdObject; }
            set { this._ProductIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(ProductAttribute))
            {
                return false;
            }			
			
			 ProductAttribute _paramObj = obj as ProductAttribute;
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