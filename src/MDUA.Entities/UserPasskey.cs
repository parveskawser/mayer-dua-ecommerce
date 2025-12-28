using System;

using System.Runtime.Serialization;

using System.ServiceModel;

using MDUA.Framework;

using MDUA.Entities.Bases;

using MDUA.Entities.List;

namespace MDUA.Entities

{

    public partial class UserPasskey

    {



    }

    public class UserPasskeyResult
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public byte[] PublicKey { get; set; }
        public int SignatureCounter { get; set; }
        public byte[] CredentialId { get; set; }
        public Guid? AaGuid { get; set; }
    }
}
