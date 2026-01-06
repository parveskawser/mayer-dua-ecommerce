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
        }
    }

