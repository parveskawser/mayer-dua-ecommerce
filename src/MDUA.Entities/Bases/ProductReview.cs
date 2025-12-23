using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "ProductReview", Namespace = "http://www.piistech.com//entities")]
	public partial class ProductReview : ProductReviewBase
	{
		#region Exernal Properties
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
			if (obj.GetType() != typeof(ProductReview))
            {
                return false;
            }			
			
			 ProductReview _paramObj = obj as ProductReview;
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
