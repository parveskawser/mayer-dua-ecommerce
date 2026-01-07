using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace MDUA.Entities
{
	public partial class Vendor 
	{
        
            [Required(ErrorMessage = "Please select a vendor.")]
            public int VendorId { get; set; }

            [Required]
            [Range(0.01, 1000000, ErrorMessage = "Amount must be greater than 0.")]
            public decimal Amount { get; set; }

            [Required]
            public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

            [StringLength(500)]
            public string Remarks { get; set; }

            // For the dropdown list in the UI
            public List<SelectListItem> VendorList { get; set; } = new List<SelectListItem>();

        // --- NEW AGGREGATE PROPERTIES ---
        // These are not in the database table 'Vendor', but populated via SQL query
        [DataMember]
        public int TotalRequestedCount { get; set; }

        [DataMember]
        public int TotalRequestedQty { get; set; }

        [DataMember]
        public int TotalReceivedCount { get; set; }

        [DataMember]
        public int TotalReceivedQty { get; set; }

        [DataMember]
        public int TotalUnpaidCount { get; set; }

        [DataMember]
        public decimal TotalAmount { get; set; }

        [DataMember]
        public decimal TotalPaidAmount { get; set; }

        [DataMember]
        public decimal TotalDueAmount { get; set; }
    }
    }

