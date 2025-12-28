using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using MDUA.Framework;

namespace MDUA.Entities.Bases
{
    [Serializable]
    [DataContract(Name = "UserPasskeyBase", Namespace = "http://www.piistech.com//entities")]
    public class UserPasskeyBase : BaseBusinessEntity
    {
        #region Enum Collection
        public enum Columns
        {
            Id = 0,
            UserId = 1,
            CredentialId = 2,
            PublicKey = 3,
            SignatureCounter = 4,
            CredType = 5,
            RegDate = 6,
            AaGuid = 7
        }
        #endregion

        #region Constants
        public const string Property_Id = "Id";
        public const string Property_UserId = "UserId";
        public const string Property_CredentialId = "CredentialId";
        public const string Property_PublicKey = "PublicKey";
        public const string Property_SignatureCounter = "SignatureCounter";
        public const string Property_CredType = "CredType";
        public const string Property_RegDate = "RegDate";
        public const string Property_AaGuid = "AaGuid";
        #endregion

        #region Private Data Types
        private Int32 _Id;
        private Int32 _UserId;
        private byte[] _CredentialId;
        private byte[] _PublicKey;
        private Int32 _SignatureCounter;
        private String _CredType;
        private Nullable<DateTime> _RegDate;
        private Nullable<Guid> _AaGuid;
        #endregion

        #region Properties        
        [DataMember]
        public Int32 Id
        {
            get { return _Id; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Id, value, _Id);
                if (PropertyChanging(args))
                {
                    _Id = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public Int32 UserId
        {
            get { return _UserId; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_UserId, value, _UserId);
                if (PropertyChanging(args))
                {
                    _UserId = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public byte[] CredentialId
        {
            get { return _CredentialId; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CredentialId, value, _CredentialId);
                if (PropertyChanging(args))
                {
                    _CredentialId = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public byte[] PublicKey
        {
            get { return _PublicKey; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PublicKey, value, _PublicKey);
                if (PropertyChanging(args))
                {
                    _PublicKey = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public Int32 SignatureCounter
        {
            get { return _SignatureCounter; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SignatureCounter, value, _SignatureCounter);
                if (PropertyChanging(args))
                {
                    _SignatureCounter = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public String CredType
        {
            get { return _CredType; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CredType, value, _CredType);
                if (PropertyChanging(args))
                {
                    _CredType = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public Nullable<DateTime> RegDate
        {
            get { return _RegDate; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_RegDate, value, _RegDate);
                if (PropertyChanging(args))
                {
                    _RegDate = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public Nullable<Guid> AaGuid
        {
            get { return _AaGuid; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AaGuid, value, _AaGuid);
                if (PropertyChanging(args))
                {
                    _AaGuid = value;
                    PropertyChanged(args);
                }
            }
        }
        #endregion

        #region Cloning Base Objects
        public UserPasskeyBase Clone()
        {
            UserPasskeyBase newObj = new UserPasskeyBase();
            base.CloneBase(newObj);
            newObj.Id = this.Id;
            newObj.UserId = this.UserId;
            newObj.CredentialId = this.CredentialId;
            newObj.PublicKey = this.PublicKey;
            newObj.SignatureCounter = this.SignatureCounter;
            newObj.CredType = this.CredType;
            newObj.RegDate = this.RegDate;
            newObj.AaGuid = this.AaGuid;

            return newObj;
        }
        #endregion

        #region Getting object by adding value of that properties 
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(UserPasskeyBase.Property_Id, Id);
            info.AddValue(UserPasskeyBase.Property_UserId, UserId);
            info.AddValue(UserPasskeyBase.Property_CredentialId, CredentialId);
            info.AddValue(UserPasskeyBase.Property_PublicKey, PublicKey);
            info.AddValue(UserPasskeyBase.Property_SignatureCounter, SignatureCounter);
            info.AddValue(UserPasskeyBase.Property_CredType, CredType);
            info.AddValue(UserPasskeyBase.Property_RegDate, RegDate);
            info.AddValue(UserPasskeyBase.Property_AaGuid, AaGuid);
        }
        #endregion
    }
}