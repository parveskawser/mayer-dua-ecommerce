using System;
using System.Collections.Generic; // Ensure this is here
using System.Runtime.Serialization;
using MDUA.Entities.Bases;

namespace MDUA.Entities
{
    // 1. The Partial Class
    public partial class ProductVariant : ProductVariantBase
    {
        [DataMember]
        public int StockQty { get; set; }
        [DataMember]
        public List<int> AttributeValueIds { get; set; } = new List<int>();
        [DataMember]
        public decimal DiscountedPrice { get; set; }
        [DataMember]
        public string VariantImageUrl { get; set; }
        public int ImageCount { get; set; }
        [DataMember]
        public virtual Product Product { get; set; }
   

    } // ⬅️ CLOSE THE CLASS HERE

    // 2. The Result Class (Outside the partial class, inside the namespace)
    public class ProductVariantResult
    {
        public int ProductId { get; set; }
        public decimal BasePrice { get; set; }
        public List<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public List<AttributeName> AvailableAttributes { get; set; } = new List<AttributeName>();
        public int ReorderLevel { get; set; }
    }

} // End of Namespace