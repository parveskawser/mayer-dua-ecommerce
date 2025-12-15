using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
    public partial class BulkPurchaseOrder 
    {
        // ... your existing BulkPurchaseOrder properties ...
    }

    // MOVED OUTSIDE the class, but kept inside the namespace
    public class BulkOrderItemViewModel
    {
        public int PoRequestId { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; }
        public string VariantName { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
    }
}