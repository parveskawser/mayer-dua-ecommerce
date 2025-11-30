using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "LanguageResource", Namespace = "http://www.piistech.com//entities")]
	public partial class LanguageResource : LanguageResourceBase
	{
		#region Exernal Properties
		private Company _CompanyIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="Company"/>.
		/// </summary>
		/// <value>The source Company for _CompanyIdObject.</value>
		[DataMember]
		public Company CompanyIdObject
      	{
            get { return this._CompanyIdObject; }
            set { this._CompanyIdObject = value; }
      	}
		
		private Language _LanguageIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="Language"/>.
		/// </summary>
		/// <value>The source Language for _LanguageIdObject.</value>
		[DataMember]
		public Language LanguageIdObject
      	{
            get { return this._LanguageIdObject; }
            set { this._LanguageIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(LanguageResource))
            {
                return false;
            }			
			
			 LanguageResource _paramObj = obj as LanguageResource;
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
