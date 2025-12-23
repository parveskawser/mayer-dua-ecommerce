using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
    [Serializable]
    [DataContract(Name = "ChatMessageBase", Namespace = "http://www.piistech.com//entities")]
    public class ChatMessageBase : BaseBusinessEntity
    {

        #region Enum Collection
        public enum Columns
        {
            Id = 0,
            ChatSessionId = 1,
            SenderId = 2,
            SenderName = 3,
            MessageText = 4,
            IsFromAdmin = 5,
            IsRead = 6,
            SentAt = 7,
            SenderType = 8
        }
        #endregion

        #region Constants
        public const string Property_Id = "Id";
        public const string Property_ChatSessionId = "ChatSessionId";
        public const string Property_SenderId = "SenderId";
        public const string Property_SenderName = "SenderName";
        public const string Property_MessageText = "MessageText";
        public const string Property_IsFromAdmin = "IsFromAdmin";
        public const string Property_IsRead = "IsRead";
        public const string Property_SentAt = "SentAt";
        public const string Property_SenderType = "SenderType";
        #endregion

        #region Private Data Types
        private Int32 _Id;
        private Int32 _ChatSessionId;
        private Nullable<Int32> _SenderId;
        private String _SenderName;
        private String _MessageText;
        private Boolean _IsFromAdmin;
        private Boolean _IsRead;
        private DateTime _SentAt;
        private String _SenderType;
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
        public Int32 ChatSessionId
        {
            get { return _ChatSessionId; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ChatSessionId, value, _ChatSessionId);
                if (PropertyChanging(args))
                {
                    _ChatSessionId = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public Nullable<Int32> SenderId
        {
            get { return _SenderId; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SenderId, value, _SenderId);
                if (PropertyChanging(args))
                {
                    _SenderId = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public String SenderName
        {
            get { return _SenderName; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SenderName, value, _SenderName);
                if (PropertyChanging(args))
                {
                    _SenderName = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public String MessageText
        {
            get { return _MessageText; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_MessageText, value, _MessageText);
                if (PropertyChanging(args))
                {
                    _MessageText = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public Boolean IsFromAdmin
        {
            get { return _IsFromAdmin; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IsFromAdmin, value, _IsFromAdmin);
                if (PropertyChanging(args))
                {
                    _IsFromAdmin = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public Boolean IsRead
        {
            get { return _IsRead; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IsRead, value, _IsRead);
                if (PropertyChanging(args))
                {
                    _IsRead = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public DateTime SentAt
        {
            get { return _SentAt; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SentAt, value, _SentAt);
                if (PropertyChanging(args))
                {
                    _SentAt = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public String SenderType
        {
            get { return _SenderType; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SenderType, value, _SenderType);
                if (PropertyChanging(args))
                {
                    _SenderType = value;
                    PropertyChanged(args);
                }
            }
        }

        #endregion

        #region Cloning Base Objects
        public ChatMessageBase Clone()
        {
            ChatMessageBase newObj = new ChatMessageBase();
            base.CloneBase(newObj);
            newObj.Id = this.Id;
            newObj.ChatSessionId = this.ChatSessionId;
            newObj.SenderId = this.SenderId;
            newObj.SenderName = this.SenderName;
            newObj.MessageText = this.MessageText;
            newObj.IsFromAdmin = this.IsFromAdmin;
            newObj.IsRead = this.IsRead;
            newObj.SentAt = this.SentAt;
            newObj.SenderType = this.SenderType;

            return newObj;
        }
        #endregion

        #region Getting object by adding value of that properties 
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(ChatMessageBase.Property_Id, Id);
            info.AddValue(ChatMessageBase.Property_ChatSessionId, ChatSessionId);
            info.AddValue(ChatMessageBase.Property_SenderId, SenderId);
            info.AddValue(ChatMessageBase.Property_SenderName, SenderName);
            info.AddValue(ChatMessageBase.Property_MessageText, MessageText);
            info.AddValue(ChatMessageBase.Property_IsFromAdmin, IsFromAdmin);
            info.AddValue(ChatMessageBase.Property_IsRead, IsRead);
            info.AddValue(ChatMessageBase.Property_SentAt, SentAt);
            info.AddValue(ChatMessageBase.Property_SenderType, SenderType);
        }
        #endregion


    }
}