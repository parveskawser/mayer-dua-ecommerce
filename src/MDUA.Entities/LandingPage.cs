using System.Collections.Generic;

namespace MDUA.Entities
{
    public class LandingPageViewModel
    {
        // For the Sidebar Menu
        public List<ProductCategory> Categories { get; set; } = new List<ProductCategory>();

        // For the Main Slider
        public List<string> SliderImages { get; set; } = new List<string>();

        // For "New Arrivals" Section
        public List<ProductViewModel> NewArrivals { get; set; } = new List<ProductViewModel>();

        // For "Featured" Section
        public List<ProductViewModel> FeaturedProducts { get; set; } = new List<ProductViewModel>();

        // UI States
        public bool IsUserLoggedIn { get; set; }
        public string UserName { get; set; }
    }

    // ✅ Define ProductViewModel here so Facade can use it
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string Author { get; set; }
    }
}