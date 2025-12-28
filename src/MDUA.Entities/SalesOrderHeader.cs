using System;
using System.Runtime.Serialization;
using MDUA.Entities.Bases;

namespace MDUA.Entities
{
    public partial class SalesOrderHeader
    {
        // Customer Info
        [DataMember] public string CustomerName { get; set; }
        [DataMember] public string CustomerPhone { get; set; }

        // ✅ Issue 3 Fix: Added Email
        [DataMember] public string CustomerEmail { get; set; }

        // ✅ Issue 5 Fix: All Address Columns
        [DataMember] public string Street { get; set; }
        [DataMember] public string City { get; set; } // Mapped to District dropdown

        [DataMember]
        public decimal DeliveryCharge { get; set; }
        [DataMember] public string Divison { get; set; }
        [DataMember] public string PostalCode { get; set; }
        [DataMember] public string ZipCode { get; set; } // Often same as Postal, but we'll handle both
        [DataMember] public string Country { get; set; } = "Bangladesh"; // Default
        [DataMember] public string AddressType { get; set; }
        // Order Item
        [DataMember] public int ProductVariantId { get; set; }
        [DataMember] public int OrderQuantity { get; set; }
        [DataMember] public int TargetCompanyId { get; set; }

        //new
        [DataMember] public string Thana { get; set; }
        [DataMember] public string SubOffice { get; set; }

        [DataMember] public string IPAddress { get; set; }
        [DataMember] public string SessionId { get; set; }

        [DataMember]
        public decimal PaidAmount { get; set; }

        [DataMember]
        public decimal DueAmount { get; set; }

        [DataMember]
        public decimal ActualLogisticsCost { get; set; }
        [DataMember]
        public virtual CompanyCustomer CompanyCustomer { get; set; }
    }
   
        public class EmailResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string MessageId { get; set; }
        }

        public class NotificationResult
        {
            public bool EmailSent { get; set; }
            public bool SmsSent { get; set; }
            public string EmailMessage { get; set; }
            public string SmsMessage { get; set; }
            public bool IsSuccess => EmailSent || SmsSent; // At least one succeeded
        }
    }



