using System;
using System.Runtime.Serialization;
using System.Collections.Generic; // Required for List and Dictionary
using MDUA.Entities.Bases;

namespace MDUA.Entities
{
    // ✅ PARTIAL CLASS START
    public partial class Product : ProductBase
    {
        [DataMember]
        public string CompanyName { get; set; }

        // ✅ THIS IS THE PROPERTY THE ERROR IS COMPLAINING ABOUT
        [DataMember]
        public string CompanyLogoUrl { get; set; }

        [DataMember]
        public List<ProductImage> Images { get; set; } = new List<ProductImage>();

        [DataMember]
        public List<ProductReview> Reviews { get; set; } = new List<ProductReview>();

        [DataMember]
        public ProductDiscount ActiveDiscount { get; set; }

        [DataMember]
        public decimal SellingPrice { get; set; }

        [DataMember]
        public List<string> AvailableSizes { get; set; } = new List<string>();

        [DataMember]
        public Dictionary<string, int> DeliveryCharges { get; set; }

        [DataMember]
        public int TotalStockQuantity { get; set; }

        [DataMember]
        public decimal StockBarPercentage =>
            ReorderLevel > 0 ? Math.Min(100, (TotalStockQuantity / (decimal)ReorderLevel) * 100) : 0;

        [DataMember]
        public string ScarcityMessage =>
            TotalStockQuantity <= ReorderLevel
                ? $"Only {TotalStockQuantity} left in stock!"
                : $"In stock: {TotalStockQuantity}";

        [DataMember]
        public string CategoryName { get; set; }

        [DataMember]
        public List<int> AttributeValueIds { get; set; } = new List<int>();

        [DataMember]
        public List<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

        [DataMember]
        public List<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();

        [DataMember]
        public bool? IsVariantBased { get; set; } = true;

        [DataMember]
        public string AttributeName { get; set; }

        [DataMember]
        public List<AttributeValue> PossibleValues { get; set; } = new List<AttributeValue>();

        [DataMember]
        public Dictionary<string, List<string>> Specifications { get; set; } = new Dictionary<string, List<string>>();

    } // ✅ CLASS END (Make sure this brace is here)

    // ✅ RESULT CLASS (Outside Product, Inside Namespace)
    public class ProductResult
    {
        public Product Product { get; set; }
        public List<int> RelatedIds { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public bool IsActive { get; set; }
        public string ErrorMessage { get; set; }
        public string CategoryName { get; set; }
        public List<ProductAttribute> AttributesForView { get; set; } = new List<ProductAttribute>();
        public List<ProductCategory> Categories { get; set; } = new List<ProductCategory>();
    }

} // ✅ NAMESPACE END