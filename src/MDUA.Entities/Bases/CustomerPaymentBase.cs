using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
    [Serializable]
    [DataContract(Name = "CustomerPaymentBase", Namespace = "http://www.piistech.com//entities")]
    public class CustomerPaymentBase : BaseBusinessEntity
    {

        #region Enum Collection
        public enum Columns
        {
            Id = 0,
            CustomerId = 1,
            PaymentMethodId = 2,
            InventoryTransactionId = 3,
            PaymentType = 4,
            Amount = 5,
            PaymentDate = 6,
            Status = 7,
            Notes = 8,
            CreatedBy = 9,
            CreatedAt = 10,
            UpdatedBy = 11,
            UpdatedAt = 12,
            // --- NEW COLUMN ---
            TransactionReference = 13
        }
        #endregion

        #region Constants
        public const string Property_Id = "Id";
        public const string Property_CustomerId = "CustomerId";
        public const string Property_PaymentMethodId = "PaymentMethodId";
        public const string Property_InventoryTransactionId = "InventoryTransactionId";
        public const string Property_PaymentType = "PaymentType";
        public const string Property_Amount = "Amount";
        public const string Property_PaymentDate = "PaymentDate";
        public const string Property_Status = "Status";
        public const string Property_Notes = "Notes";
        public const string Property_CreatedBy = "CreatedBy";
        public const string Property_CreatedAt = "CreatedAt";
        public const string Property_UpdatedBy = "UpdatedBy";
        public const string Property_UpdatedAt = "UpdatedAt";
        // --- NEW CONSTANT ---
        public const string Property_TransactionReference = "TransactionReference";
        #endregion

        #region Private Data Types
        private Int32 _Id;
        private Int32 _CustomerId;
        private Int32 _PaymentMethodId;
        private Nullable<Int32> _InventoryTransactionId;
        private String _PaymentType;
        private Decimal _Amount;
        private DateTime _PaymentDate;
        private String _Status;
        private String _Notes;
        private String _CreatedBy;
        private DateTime _CreatedAt;
        private String _UpdatedBy;
        private Nullable<DateTime> _UpdatedAt;
        // --- NEW FIELD ---
        private String _TransactionReference;
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
        public Int32 CustomerId
        {
            get { return _CustomerId; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CustomerId, value, _CustomerId);
                if (PropertyChanging(args))
                {
                    _CustomerId = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public Int32 PaymentMethodId
        {
            get { return _PaymentMethodId; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PaymentMethodId, value, _PaymentMethodId);
                if (PropertyChanging(args))
                {
                    _PaymentMethodId = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public Nullable<Int32> InventoryTransactionId
        {
            get { return _InventoryTransactionId; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_InventoryTransactionId, value, _InventoryTransactionId);
                if (PropertyChanging(args))
                {
                    _InventoryTransactionId = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public String PaymentType
        {
            get { return _PaymentType; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PaymentType, value, _PaymentType);
                if (PropertyChanging(args))
                {
                    _PaymentType = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public Decimal Amount
        {
            get { return _Amount; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Amount, value, _Amount);
                if (PropertyChanging(args))
                {
                    _Amount = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public DateTime PaymentDate
        {
            get { return _PaymentDate; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PaymentDate, value, _PaymentDate);
                if (PropertyChanging(args))
                {
                    _PaymentDate = value;
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
        public String Notes
        {
            get { return _Notes; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Notes, value, _Notes);
                if (PropertyChanging(args))
                {
                    _Notes = value;
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

        // --- NEW PROPERTY ---
        [DataMember]
        public String TransactionReference
        {
            get { return _TransactionReference; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_TransactionReference, value, _TransactionReference);
                if (PropertyChanging(args))
                {
                    _TransactionReference = value;
                    PropertyChanged(args);
                }
            }
        }

        #endregion

        #region Cloning Base Objects
        public CustomerPaymentBase Clone()
        {
            CustomerPaymentBase newObj = new CustomerPaymentBase();
            base.CloneBase(newObj);
            newObj.Id = this.Id;
            newObj.CustomerId = this.CustomerId;
            newObj.PaymentMethodId = this.PaymentMethodId;
            newObj.InventoryTransactionId = this.InventoryTransactionId;
            newObj.PaymentType = this.PaymentType;
            newObj.Amount = this.Amount;
            newObj.PaymentDate = this.PaymentDate;
            newObj.Status = this.Status;
            newObj.Notes = this.Notes;
            newObj.CreatedBy = this.CreatedBy;
            newObj.CreatedAt = this.CreatedAt;
            newObj.UpdatedBy = this.UpdatedBy;
            newObj.UpdatedAt = this.UpdatedAt;
            // --- Clone New Property ---
            newObj.TransactionReference = this.TransactionReference;

            return newObj;
        }
        #endregion

        #region Getting object by adding value of that properties 
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(CustomerPaymentBase.Property_Id, Id);
            info.AddValue(CustomerPaymentBase.Property_CustomerId, CustomerId);
            info.AddValue(CustomerPaymentBase.Property_PaymentMethodId, PaymentMethodId);
            info.AddValue(CustomerPaymentBase.Property_InventoryTransactionId, InventoryTransactionId);
            info.AddValue(CustomerPaymentBase.Property_PaymentType, PaymentType);
            info.AddValue(CustomerPaymentBase.Property_Amount, Amount);
            info.AddValue(CustomerPaymentBase.Property_PaymentDate, PaymentDate);
            info.AddValue(CustomerPaymentBase.Property_Status, Status);
            info.AddValue(CustomerPaymentBase.Property_Notes, Notes);
            info.AddValue(CustomerPaymentBase.Property_CreatedBy, CreatedBy);
            info.AddValue(CustomerPaymentBase.Property_CreatedAt, CreatedAt);
            info.AddValue(CustomerPaymentBase.Property_UpdatedBy, UpdatedBy);
            info.AddValue(CustomerPaymentBase.Property_UpdatedAt, UpdatedAt);
            // --- Add New Property to Serialization ---
            info.AddValue(CustomerPaymentBase.Property_TransactionReference, TransactionReference);
        }
        #endregion
    }
}