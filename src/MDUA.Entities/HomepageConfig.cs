using System;
using System.Collections.Generic;
using System.Text.Json.Serialization; // ✅ Required namespace
namespace MDUA.Entities
{
    // ✅ The "LEGO Block"
    public class HomepageSection
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Type { get; set; } // "Hero", "ProductGrid"
        public int Order { get; set; }

        // This is the property missing in your error
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
        // ✅ NEW: This holds the actual products for the homepage
        // [JsonIgnore] prevents it from being saved to the database JSON string
        [JsonIgnore]
        public List<Product> LoadedProducts { get; set; } = new List<Product>();
    }

    // ✅ The "Page"
    public class HomepageConfig
    {
        public List<HomepageSection> Sections { get; set; } = new List<HomepageSection>();
        public List<ProductCategory> Categories { get; set; } = new List<ProductCategory>();
    }
}