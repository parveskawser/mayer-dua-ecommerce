using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "PermissionGroupMap", Namespace = "http://www.piistech.com//entities")]
	public partial class PermissionGroupMap : PermissionGroupMapBase
	{
		#region Exernal Properties
		private Permission _PermissionIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="Permission"/>.
		/// </summary>
		/// <value>The source Permission for _PermissionIdObject.</value>
		[DataMember]
		public Permission PermissionIdObject
      	{
            get { return this._PermissionIdObject; }
            set { this._PermissionIdObject = value; }
      	}
		
		private PermissionGroup _PermissionGroupIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="PermissionGroup"/>.
		/// </summary>
		/// <value>The source PermissionGroup for _PermissionGroupIdObject.</value>
		[DataMember]
		public PermissionGroup PermissionGroupIdObject
      	{
            get { return this._PermissionGroupIdObject; }
            set { this._PermissionGroupIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(PermissionGroupMap))
            {
                return false;
            }			
			
			 PermissionGroupMap _paramObj = obj as PermissionGroupMap;
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