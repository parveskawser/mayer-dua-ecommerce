using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
    [Serializable]
    [CollectionDataContract(Name = "UserPasskeyList", Namespace = "http://www.piistech.com//list")]
    public class UserPasskeyList : BaseCollection<UserPasskey>
    {
        #region Constructors
        public UserPasskeyList() : base() { }
        public UserPasskeyList(UserPasskey[] list) : base(list) { }
        public UserPasskeyList(List<UserPasskey> list) : base(list) { }
        #endregion

        #region Custom Methods
        #endregion
    }
}