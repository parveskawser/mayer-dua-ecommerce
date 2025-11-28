using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "VariantAttributeValue", Namespace = "http://www.piistech.com//entities")]
	public partial class VariantAttributeValue : VariantAttributeValueBase
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
		
		private AttributeValue _AttributeValueIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="AttributeValue"/>.
		/// </summary>
		/// <value>The source AttributeValue for _AttributeValueIdObject.</value>
		[DataMember]
		public AttributeValue AttributeValueIdObject
      	{
            get { return this._AttributeValueIdObject; }
            set { this._AttributeValueIdObject = value; }
      	}
		
		private ProductVariant _VariantIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="ProductVariant"/>.
		/// </summary>
		/// <value>The source ProductVariant for _VariantIdObject.</value>
		[DataMember]
		public ProductVariant VariantIdObject
      	{
            get { return this._VariantIdObject; }
            set { this._VariantIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(VariantAttributeValue))
            {
                return false;
            }			
			
			 VariantAttributeValue _paramObj = obj as VariantAttributeValue;
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
