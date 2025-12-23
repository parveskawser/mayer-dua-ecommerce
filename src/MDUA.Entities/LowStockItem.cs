using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDUA.Entities
{
    public class LowStockItem
    {
        public int VariantId { get; set; } 
        public string ProductName { get; set; }
        public string VariantName { get; set; }
        public int StockQty { get; set; }
        public decimal Price { get; set; }

    }
}