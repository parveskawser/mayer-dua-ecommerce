using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
    [Serializable]
    [DataContract(Name = "PoRequested", Namespace = "http://www.piistech.com//entities")]
    public partial class PoRequested : PoRequestedBase
    {
        #region External Properties

        // ✅ CHANGE 1: Point to ProductVariant instead of Product
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

        private Vendor _VendorIdObject = null;

        /// <summary>
        /// Gets or sets the source <see cref="Vendor"/>.
        /// </summary>
        /// <value>The source Vendor for _VendorIdObject.</value>
        [DataMember]
        public Vendor VendorIdObject
        {
            get { return this._VendorIdObject; }
            set { this._VendorIdObject = value; }
        }

        #endregion

        #region Override Equals
        public override bool Equals(Object obj)
        {
            if (obj.GetType() != typeof(PoRequested))
            {
                return false;
            }

            PoRequested _paramObj = obj as PoRequested;
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

        #region Override HashCode
        public override int GetHashCode()
        {
            return base.Id.GetHashCode();
        }
        #endregion
    }
}