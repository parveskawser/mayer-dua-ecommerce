using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDUA.Entities
{
    public class ExportRequest
    {
        // Configuration Options
        public string EntityType { get; set; } // "Order"
        public string Format { get; set; }     // "excel", "pdf", "csv"
        public string Scope { get; set; }      // "all", "filtered", "selected"
        public List<string> Columns { get; set; }
        public List<int> SelectedIds { get; set; }

        // ✅ Order List Specific Filters (Must match Controller Action parameters)
        public string Status { get; set; }
        public string PayStatus { get; set; }
        public string OrderType { get; set; }
        public string DateRange { get; set; }
        public string Search { get; set; }

        // Nullables for precise filtering
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public double? MinAmount { get; set; }
        public double? MaxAmount { get; set; }
        public int? MinId { get; set; }
        public int? MaxId { get; set; }
    }
}
