using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "CmsAsset", Namespace = "http://www.piistech.com//entities")]
	public partial class CmsAsset : CmsAssetBase
	{
		#region Exernal Properties
		private CmsPage _PageIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="CmsPage"/>.
		/// </summary>
		/// <value>The source CmsPage for _PageIdObject.</value>
		[DataMember]
		public CmsPage PageIdObject
      	{
            get { return this._PageIdObject; }
            set { this._PageIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(CmsAsset))
            {
                return false;
            }			
			
			 CmsAsset _paramObj = obj as CmsAsset;
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