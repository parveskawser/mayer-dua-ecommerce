using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	public partial class VendorPayment 
	{
        public int Id { get; set; }
        public int VendorId { get; set; }

        // New properties for the updated SP
        public int PaymentMethodId { get; set; }
        public int? PoReceivedId { get; set; }
        public int? PoRequestedId { get; set; }
        public string ReferenceNo { get; set; }

        // Mapped to @Notes in SP
        public string Remarks { get; set; }

        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }

        // Helper properties for UI Display
        public string VendorName { get; set; }
        public string PaymentMethodName { get; set; }
    }
}
