using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
    [Serializable]
    [DataContract(Name = "UserPasskey", Namespace = "http://www.piistech.com//entities")]
    public partial class UserPasskey : UserPasskeyBase
    {
        #region Exernal Properties
        private Company _UserLoginObject = null;
        #endregion
        [DataMember]
        public Company UserLoginObject
        {
            get { return this._UserLoginObject; }
            set { this._UserLoginObject = value; }
        }

        #region Orverride Equals
        public override bool Equals(Object obj)
        {
            if (obj.GetType() != typeof(UserPasskey))
            {
                return false;
            }

            UserPasskey _paramObj = obj as UserPasskey;
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