using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	public partial class DeliveryStatusLog 
	{

    }
    public class DeliveryLogViewModel
    {
        // Filters
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string SearchTerm { get; set; } // Order ID or Username
        public string EntityType { get; set; } // "All", "Delivery", "SalesOrderHeader"

        // Data
        public List<DeliveryStatusLog> Logs { get; set; } = new List<DeliveryStatusLog>();

        // Pagination (Optional but recommended for logs)
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
    }

}