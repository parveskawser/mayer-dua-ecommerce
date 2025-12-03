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


    }
}