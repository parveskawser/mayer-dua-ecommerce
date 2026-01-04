using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
    [Serializable]
    [DataContract(Name = "SalesOrderHeaderBase", Namespace = "http://www.piistech.com//entities")]
    public class SalesOrderHeaderBase : BaseBusinessEntity
    {

        #region Enum Collection
        public enum Columns
        {
            Id = 0,
            CompanyCustomerId = 1,
            AddressId = 2,
            SalesChannelId = 3,
            OnlineOrderId = 4,
            DirectOrderId = 5,
            OrderDate = 6,
            TotalAmount = 7,
            DiscountAmount = 8,
            NetAmount = 9,
            SessionId = 10,
            IPAddress = 11,
            Status = 12,
            IsActive = 13,
            Confirmed = 14,
            CreatedBy = 15,
            CreatedAt = 16,
            UpdatedBy = 17,
            UpdatedAt = 18,
            SalesOrderId = 19
        }
        #endregion

        #region Constants
        public const string Property_Id = "Id";
        public const string Property_CompanyCustomerId = "CompanyCustomerId";
        public const string Property_AddressId = "AddressId";
        public const string Property_SalesChannelId = "SalesChannelId";
        public const string Property_OnlineOrderId = "OnlineOrderId";
        public const string Property_DirectOrderId = "DirectOrderId";
        public const string Property_OrderDate = "OrderDate";
        public const string Property_TotalAmount = "TotalAmount";
        public const string Property_DiscountAmount = "DiscountAmount";
        public const string Property_NetAmount = "NetAmount";
        public const string Property_SessionId = "SessionId";
        public const string Property_IPAddress = "IPAddress";
        public const string Property_Status = "Status";
        public const string Property_IsActive = "IsActive";
        public const string Property_Confirmed = "Confirmed";
        public const string Property_CreatedBy = "CreatedBy";
        public const string Property_CreatedAt = "CreatedAt";
        public const string Property_UpdatedBy = "UpdatedBy";
        public const string Property_UpdatedAt = "UpdatedAt";
        public const string Property_SalesOrderId = "SalesOrderId";
        #endregion

        #region Private Data Types
        private Int32 _Id;
        private Int32 _CompanyCustomerId;
        private Int32 _AddressId;
        private Int32 _SalesChannelId;
        private String _OnlineOrderId;
        private String _DirectOrderId;
        private DateTime _OrderDate;
        private Decimal _TotalAmount;
        private Decimal _DiscountAmount;
        private Nullable<Decimal> _NetAmount;
        private String _SessionId;
        private String _IPAddress;
        private String _Status;
        private Boolean _IsActive;
        private Boolean _Confirmed;
        private String _CreatedBy;
        private DateTime _CreatedAt;
        private String _UpdatedBy;
        private Nullable<DateTime> _UpdatedAt;
        private String _SalesOrderId;
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
        public Int32 CompanyCustomerId
        {
            get { return _CompanyCustomerId; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CompanyCustomerId, value, _CompanyCustomerId);
                if (PropertyChanging(args))
                {
                    _CompanyCustomerId = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public Int32 AddressId
        {
            get { return _AddressId; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AddressId, value, _AddressId);
                if (PropertyChanging(args))
                {
                    _AddressId = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public Int32 SalesChannelId
        {
            get { return _SalesChannelId; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SalesChannelId, value, _SalesChannelId);
                if (PropertyChanging(args))
                {
                    _SalesChannelId = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public String OnlineOrderId
        {
            get { return _OnlineOrderId; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_OnlineOrderId, value, _OnlineOrderId);
                if (PropertyChanging(args))
                {
                    _OnlineOrderId = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public String DirectOrderId
        {
            get { return _DirectOrderId; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_DirectOrderId, value, _DirectOrderId);
                if (PropertyChanging(args))
                {
                    _DirectOrderId = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public DateTime OrderDate
        {
            get { return _OrderDate; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_OrderDate, value, _OrderDate);
                if (PropertyChanging(args))
                {
                    _OrderDate = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public Decimal TotalAmount
        {
            get { return _TotalAmount; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_TotalAmount, value, _TotalAmount);
                if (PropertyChanging(args))
                {
                    _TotalAmount = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public Decimal DiscountAmount
        {
            get { return _DiscountAmount; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_DiscountAmount, value, _DiscountAmount);
                if (PropertyChanging(args))
                {
                    _DiscountAmount = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public Nullable<Decimal> NetAmount
        {
            get { return _NetAmount; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_NetAmount, value, _NetAmount);
                if (PropertyChanging(args))
                {
                    _NetAmount = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public String SessionId
        {
            get { return _SessionId; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SessionId, value, _SessionId);
                if (PropertyChanging(args))
                {
                    _SessionId = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public String IPAddress
        {
            get { return _IPAddress; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IPAddress, value, _IPAddress);
                if (PropertyChanging(args))
                {
                    _IPAddress = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public String Status
        {
            get { return _Status; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Status, value, _Status);
                if (PropertyChanging(args))
                {
                    _Status = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public Boolean IsActive
        {
            get { return _IsActive; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IsActive, value, _IsActive);
                if (PropertyChanging(args))
                {
                    _IsActive = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public Boolean Confirmed
        {
            get { return _Confirmed; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Confirmed, value, _Confirmed);
                if (PropertyChanging(args))
                {
                    _Confirmed = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public String CreatedBy
        {
            get { return _CreatedBy; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CreatedBy, value, _CreatedBy);
                if (PropertyChanging(args))
                {
                    _CreatedBy = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public DateTime CreatedAt
        {
            get { return _CreatedAt; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CreatedAt, value, _CreatedAt);
                if (PropertyChanging(args))
                {
                    _CreatedAt = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public String UpdatedBy
        {
            get { return _UpdatedBy; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_UpdatedBy, value, _UpdatedBy);
                if (PropertyChanging(args))
                {
                    _UpdatedBy = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public Nullable<DateTime> UpdatedAt
        {
            get { return _UpdatedAt; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_UpdatedAt, value, _UpdatedAt);
                if (PropertyChanging(args))
                {
                    _UpdatedAt = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public String SalesOrderId
        {
            get { return _SalesOrderId; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SalesOrderId, value, _SalesOrderId);
                if (PropertyChanging(args))
                {
                    _SalesOrderId = value;
                    PropertyChanged(args);
                }
            }
        }

        #endregion

        #region Cloning Base Objects
        public SalesOrderHeaderBase Clone()
        {
            SalesOrderHeaderBase newObj = new SalesOrderHeaderBase();
            base.CloneBase(newObj);
            newObj.Id = this.Id;
            newObj.CompanyCustomerId = this.CompanyCustomerId;
            newObj.AddressId = this.AddressId;
            newObj.SalesChannelId = this.SalesChannelId;
            newObj.OnlineOrderId = this.OnlineOrderId;
            newObj.DirectOrderId = this.DirectOrderId;
            newObj.OrderDate = this.OrderDate;
            newObj.TotalAmount = this.TotalAmount;
            newObj.DiscountAmount = this.DiscountAmount;
            newObj.NetAmount = this.NetAmount;
            newObj.SessionId = this.SessionId;
            newObj.IPAddress = this.IPAddress;
            newObj.Status = this.Status;
            newObj.IsActive = this.IsActive;
            newObj.Confirmed = this.Confirmed;
            newObj.CreatedBy = this.CreatedBy;
            newObj.CreatedAt = this.CreatedAt;
            newObj.UpdatedBy = this.UpdatedBy;
            newObj.UpdatedAt = this.UpdatedAt;
            newObj.SalesOrderId = this.SalesOrderId;

            return newObj;
        }
        #endregion

        #region Getting object by adding value of that properties 
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(SalesOrderHeaderBase.Property_Id, Id);
            info.AddValue(SalesOrderHeaderBase.Property_CompanyCustomerId, CompanyCustomerId);
            info.AddValue(SalesOrderHeaderBase.Property_AddressId, AddressId);
            info.AddValue(SalesOrderHeaderBase.Property_SalesChannelId, SalesChannelId);
            info.AddValue(SalesOrderHeaderBase.Property_OnlineOrderId, OnlineOrderId);
            info.AddValue(SalesOrderHeaderBase.Property_DirectOrderId, DirectOrderId);
            info.AddValue(SalesOrderHeaderBase.Property_OrderDate, OrderDate);
            info.AddValue(SalesOrderHeaderBase.Property_TotalAmount, TotalAmount);
            info.AddValue(SalesOrderHeaderBase.Property_DiscountAmount, DiscountAmount);
            info.AddValue(SalesOrderHeaderBase.Property_NetAmount, NetAmount);
            info.AddValue(SalesOrderHeaderBase.Property_SessionId, SessionId);
            info.AddValue(SalesOrderHeaderBase.Property_IPAddress, IPAddress);
            info.AddValue(SalesOrderHeaderBase.Property_Status, Status);
            info.AddValue(SalesOrderHeaderBase.Property_IsActive, IsActive);
            info.AddValue(SalesOrderHeaderBase.Property_Confirmed, Confirmed);
            info.AddValue(SalesOrderHeaderBase.Property_CreatedBy, CreatedBy);
            info.AddValue(SalesOrderHeaderBase.Property_CreatedAt, CreatedAt);
            info.AddValue(SalesOrderHeaderBase.Property_UpdatedBy, UpdatedBy);
            info.AddValue(SalesOrderHeaderBase.Property_UpdatedAt, UpdatedAt);
            info.AddValue(SalesOrderHeaderBase.Property_SalesOrderId, SalesOrderId);
        }
        #endregion


    }
}