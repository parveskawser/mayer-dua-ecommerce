using Microsoft.AspNetCore.Mvc;
using MDUA.Web.UI.Models;
using MDUA.Facade.Interface; // ✅ Add this
using MDUA.Entities;         // ✅ Add this
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Linq;

namespace MDUA.Web.UI.Controllers
{
    public class CartController : BaseController
    {
        private readonly IProductFacade _productFacade; // ✅ Inject Facade

        public CartController(IProductFacade productFacade)
        {
            _productFacade = productFacade;
        }

        public IActionResult Index()
        {
            // 1. Get Cart IDs from Session
            var cartJson = HttpContext.Session.GetString("Cart");
            var cartIds = string.IsNullOrEmpty(cartJson)
                ? new List<int>()
                : JsonSerializer.Deserialize<List<int>>(cartJson);

            // 2. Group IDs to handle Quantity (e.g., ID 5 appears twice = Qty 2)
            var cartItems = new List<CartItemViewModel>();

            var groupedIds = cartIds.GroupBy(id => id);

            foreach (var group in groupedIds)
            {
                int productId = group.Key;
                int quantity = group.Count();

                // 3. Fetch Product Details
                var product = _productFacade.GetProductDetails(productId); // Or _productFacade.Get(productId)

                if (product != null)
                {
                    // Calculate Price (Use SellingPrice logic or BasePrice)
                    // Reusing your existing discount logic would be best, 
                    // but for now, we'll fetch the calculated price via Facade if possible.
                    // If GetProductDetails doesn't calc price, use GetProductWithPrice(id) if you have it.
                    // Assuming 'product' has standard properties:

                    var img = _productFacade.GetProductImages(productId).FirstOrDefault();

                    cartItems.Add(new CartItemViewModel
                    {
                        ProductId = product.Id,
                        ProductName = product.ProductName,
                        Price = product.BasePrice ?? 0, // Ideally use calculated SellingPrice here
                        Quantity = quantity,
                        ImageUrl = img?.ImageUrl ?? "/images/default-book.png"
                    });
                }
            }

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult Add(int productId)
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            var cart = string.IsNullOrEmpty(cartJson)
                ? new List<int>()
                : JsonSerializer.Deserialize<List<int>>(cartJson);

            cart.Add(productId);

            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
            HttpContext.Session.SetInt32("CartCount", cart.Count);

            return Json(new { success = true, count = cart.Count });
        }

        // Optional: Clear Cart
        // GET: /Cart/Remove?productId=5
        public IActionResult Remove(int productId)
        {
            // 1. Get Cart
            var cartJson = HttpContext.Session.GetString("Cart");
            if (!string.IsNullOrEmpty(cartJson))
            {
                var cart = JsonSerializer.Deserialize<List<int>>(cartJson);

                // 2. Remove ALL instances of this item (e.g., if qty was 2, removes both)
                cart.RemoveAll(id => id == productId);

                // 3. Save back to Session
                HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
                HttpContext.Session.SetInt32("CartCount", cart.Count);
            }

            // 4. Reload Page
            return RedirectToAction("Index");
        }
        // GET: /Cart/Clear
        public IActionResult Clear()
        {
            // 1. Remove the Cart items list
            HttpContext.Session.Remove("Cart");

            // 2. Remove the Cart Count (updates the badge in navbar)
            HttpContext.Session.Remove("CartCount");

            // 3. Reload the Index page (which will now render the "Empty Cart" view)
            return RedirectToAction("Index");
        }
    }
}