using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static MDUA.Entities.ProductVariant;

namespace MDUA.Web.UI.Controllers
{
    [Authorize]
    public class ProductController : BaseController
    {
        private readonly IProductFacade _productFacade;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductFacade productFacade, IWebHostEnvironment webHostEnvironment)
        {
            _productFacade = productFacade;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("product/{slug}")]
        public IActionResult Index(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return BadRequest();

            Product model = _productFacade.GetProductDetailsForWebBySlug(slug);

            if (model == null) return NotFound();

            return View(model);
        }

        [Route("product/add")]
        [HttpGet]
        public IActionResult Add()
        {
            // 3. Use HasPermission from BaseController (Checks Cookie, not DB)
            if (!HasPermission("Product.Add"))
                return RedirectToAction("AccessDenied", "Account");

            // 4. Use CurrentUserId from BaseController
            var model = _productFacade.GetAddProductData(CurrentUserId);

            return View(model);
        }

        [Route("product/add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProduct(Product product)
        {
            if (!HasPermission("Product.Add"))
                return RedirectToAction("AccessDenied", "Account");

            if (string.IsNullOrWhiteSpace(product.Slug))
                product.Slug = GenerateSlug(product.ProductName);
            else
                product.Slug = GenerateSlug(product.Slug);

            // 5. Pass CurrentUserName and CurrentCompanyId from Claims
            long newProductId = _productFacade.AddProduct(
                product,
                CurrentUserName,
                CurrentCompanyId
            );

            TempData[newProductId > 0 ? "Success" : "Error"] =
                newProductId > 0 ? "Product added successfully!" : "Failed to add product.";

            return RedirectToAction("AllProducts");
        }

        [Route("product/all")]
        public IActionResult AllProducts()
        {
            if (!HasPermission("Product.View"))
                return RedirectToAction("AccessDenied", "Account");

            var products = _productFacade.GetAllProductsWithCategory();
            return View(products);
        }

        [HttpGet]
        [Route("product/get-attribute-values")]
        public IActionResult GetAttributeValues(int attributeId)
        {
            var values = _productFacade.GetAttributeValues(attributeId);
            var result = values.Select(v => new { id = v.Id, value = v.Value }).ToList();
            return new JsonResult(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/toggle-status")]
        public IActionResult ToggleStatus(int productId)
        {
            if (!HasPermission("Product.Edit")) return RedirectToAction("AccessDenied", "Account");


            bool? newStatus = _productFacade.ToggleProductStatus(productId);

            if (newStatus == null)
                return Json(new { success = false, message = "Product not found." });

            return Json(new { success = true, newIsActive = newStatus.Value });
        }

        [HttpGet]
        [Route("product/get-details-partial")]
        public IActionResult GetProductDetailsPartial(int productId)
        {
            if (!HasPermission("Product.Details")) return RedirectToAction("AccessDenied", "Account");


            Product model = _productFacade.GetProductDetails(productId);
            if (model == null) return NotFound();

            return PartialView("_ProductDetailsPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/delete-confirmed")]
        public IActionResult DeleteConfirmed(int productId)
        {
            // *** MODIFIED BLOCK ***
            if (!HasPermission("Product.Delete"))
            {
                // Return a JSON object indicating failure and a specific unauthorized status/message.
                // You can use a standard HTTP 403 Forbidden status for better client-side handling.
                Response.StatusCode = 403; // Set the HTTP status code
                return Json(new { success = false, message = "Access Denied. You do not have permission to delete this product." });
            }
            // **********************

            try
            {
                // ... rest of the successful deletion logic ...
                long result = _productFacade.Delete(productId);
                if (result > 0)
                {
                    TempData["Success"] = "Product deleted successfully!";
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Product not found." });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500; // Set the HTTP status code for server error
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpGet]
        [Route("product/get-edit-partial")]
        public IActionResult GetEditPartial(int productId)
        {
            if (!HasPermission("Product.Edit")) return RedirectToAction("AccessDenied", "Account");


            var model = _productFacade.GetProductForEdit(productId);
            if (model.Product == null) return NotFound();

            return PartialView("_EditProductPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/edit-product")]
        public IActionResult EditProduct(Product product)
        {
            if (!HasPermission("Product.Edit")) return RedirectToAction("AccessDenied", "Account");


            long result = _productFacade.UpdateProduct(product, CurrentUserName);

            if (result > 0) TempData["Success"] = "Product updated successfully!";
            else TempData["Error"] = "Failed to update product.";

            return RedirectToAction("AllProducts");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/update-variant-price")]
        public IActionResult UpdateVariantPrice(int variantId, decimal newPrice, string newSku)
        {
            if (!HasPermission("Variant.Edit")) return RedirectToAction("AccessDenied", "Account");


            long result = _productFacade.UpdateVariantPrice(variantId, newPrice, newSku);
            return Json(new { success = result > 0, message = result > 0 ? "" : "Update failed." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/delete-variant")]
        public IActionResult DeleteVariant(int variantId)
        {
            if (!HasPermission("Variant.Delete")) return RedirectToAction("AccessDenied", "Account");


            long result = _productFacade.DeleteVariant(variantId);
            return Json(new { success = result > 0, message = result > 0 ? "" : "Delete failed." });
        }

        [HttpGet]
        [Route("product/get-variants")]
        public IActionResult GetVariantsPartial(int productId)
        {
            // Assuming View permission is enough to see variants
            if (!HasPermission("Variant.View")) return RedirectToAction("AccessDenied", "Account");


            ProductVariantResult model = _productFacade.GetVariantsWithAttributes(productId);
            return PartialView("_ProductVariantsPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/add-single-variant")]
        public IActionResult AddSingleVariant(ProductVariant newVariant)
        {
            if (!HasPermission("Variant.Add")) return RedirectToAction("AccessDenied", "Account");


            newVariant.CreatedBy = CurrentUserName;
            newVariant.CreatedAt = DateTime.Now;
            newVariant.IsActive = true;

            long result = _productFacade.AddVariantToExistingProduct(newVariant);
            return Json(new { success = result > 0, message = result > 0 ? "" : "Failed to add variant." });
        }

        [HttpGet]
        [Route("product/get-missing-attributes")]
        public IActionResult GetMissingAttributes(int productId, int variantId)
        {
            var list = _productFacade.GetMissingAttributesForVariant(productId, variantId);
            return Json(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/add-attribute-to-variant")]
        public IActionResult AddAttributeToVariant(int variantId, int attributeValueId)
        {
            if (!HasPermission("Variant.Edit")) return RedirectToAction("AccessDenied", "Account");


            _productFacade.AddAttributeToVariant(variantId, attributeValueId);
            return Json(new { success = true });
        }

        [HttpGet]
        [Route("product/get-discounts")]
        public IActionResult GetDiscountsPartial(int productId)
        {
            /// if (!HasPermission("Discount.View")) return RedirectToAction("AccessDenied", "Account");

            var discounts = _productFacade.GetDiscountsByProductId(productId);
            ViewBag.ProductId = productId;
            return PartialView("_ProductDiscountsPartial", discounts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/add-discount")]
        public IActionResult AddDiscount(ProductDiscount discount)
        {
            if (!HasPermission("Discount.Add")) return RedirectToAction("AccessDenied", "Account");


            discount.CreatedBy = CurrentUserName;
            discount.IsActive = true;
            long result = _productFacade.AddDiscount(discount);
            return Json(new { success = result > 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/update-discount")]
        public IActionResult UpdateDiscount(ProductDiscount discount)
        {
            if (!HasPermission("Discount.Edit")) return RedirectToAction("AccessDenied", "Account");


            discount.UpdatedBy = CurrentUserName;
            long result = _productFacade.UpdateDiscount(discount);
            return Json(new { success = result > 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/delete-discount")]
        public IActionResult DeleteDiscount(int id)
        {
            if (!HasPermission("Discount.Delete")) return RedirectToAction("AccessDenied", "Account");


            long result = _productFacade.DeleteDiscount(id);
            return Json(new { success = result > 0 });
        }

        [HttpGet]
        [Route("product/get-updated-price")]
        public IActionResult GetUpdatedPrice(int productId)
        {
            var p = _productFacade.GetProductWithPrice(productId);
            if (p == null) return NotFound();

            return Json(new
            {
                success = true,
                hasDiscount = p.ActiveDiscount != null,
                originalPrice = "Tk. " + (p.BasePrice ?? 0).ToString("0.00"),
                sellingPrice = "Tk. " + p.SellingPrice.ToString("0.00")
            });
        }

        [HttpGet]
        [Route("product/get-images")]
        public IActionResult GetImagesPartial(int productId)
        {
            if (!HasPermission("ProductImage.View")) return RedirectToAction("AccessDenied", "Account");

            var images = _productFacade.GetProductImages(productId);
            ViewBag.ProductId = productId;
            return PartialView("_ProductImagesPartial", images);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/upload-image")]
        public async Task<IActionResult> UploadImage(int productId, IFormFile file)
        {
            if (!HasPermission("ProductImage.Add")) return RedirectToAction("AccessDenied", "Account");


            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "No file received" });

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var uploads = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products", productId.ToString());

            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save using relative path for DB
            string dbPath = $"/images/products/{productId}/{fileName}";
            _productFacade.AddProductImage(productId, dbPath, false, CurrentUserName);

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/set-primary-image")]
        public IActionResult SetPrimaryImage(int imageId, int productId)
        {
            if (!HasPermission("ProductImage.SetPrimary")) return RedirectToAction("AccessDenied", "Account");


            _productFacade.SetProductImageAsPrimary(imageId, productId);
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/update-image-order")]
        public IActionResult UpdateImageOrder(int imageId, int sortOrder)
        {
            if (!HasPermission("ProductImage.SetOrder")) return RedirectToAction("AccessDenied", "Account");


            _productFacade.UpdateProductImageSortOrder(imageId, sortOrder);
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/delete-image")]
        public IActionResult DeleteImage(int id)
        {
            if (!HasPermission("ProductImage.Delete")) return RedirectToAction("AccessDenied", "Account");


            var img = _productFacade.GetProductImage(id);
            if (img != null && !string.IsNullOrEmpty(img.ImageUrl))
            {
                try
                {
                    string relativePath = img.ImageUrl.TrimStart('/', '\\');
                    string physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);
                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }
                }
                catch { /* Log error */ }
            }

            _productFacade.DeleteProductImage(id);
            return Json(new { success = true });
        }

        [HttpGet]
        [Route("product/get-variant-images")]
        public IActionResult GetVariantImagesPartial(int variantId)
        {
            if (!HasPermission("VariantImage.View")) return RedirectToAction("AccessDenied", "Account");

            var images = _productFacade.GetVariantImages(variantId);
            ViewBag.VariantId = variantId;
            return PartialView("_VariantImagesPartial", images);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/upload-variant-image")]
        public async Task<IActionResult> UploadVariantImage(int variantId, IFormFile file)
        {
            if (!HasPermission("VariantImage.Add")) return RedirectToAction("AccessDenied", "Account");

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "variants", variantId.ToString());

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string dbPath = $"/images/variants/{variantId}/{fileName}";
            _productFacade.AddVariantImage(variantId, dbPath, CurrentUserName);

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/delete-variant-image")]
        public IActionResult DeleteVariantImage(int id)
        {
            if (!HasPermission("VariantImage.Delete")) return RedirectToAction("AccessDenied", "Account");


            var img = _productFacade.GetVariantImage(id);
            if (img != null && !string.IsNullOrEmpty(img.ImageUrl))
            {
                try
                {
                    string relativePath = img.ImageUrl.TrimStart('/', '\\');
                    string physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);
                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }
                }
                catch { }
            }

            _productFacade.DeleteVariantImage(id);
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/update-variant-image-order")]
        public IActionResult UpdateVariantImageOrder(int imageId, int displayOrder)
        {
            if (!HasPermission("VariantImage.SetOrder")) return RedirectToAction("AccessDenied", "Account");


            _productFacade.UpdateVariantImageDisplayOrder(imageId, displayOrder);
            return Json(new { success = true });
        }

        // Helper (Internal logic, doesn't need auth check)
        private string GenerateSlug(string phrase)
        {
            if (string.IsNullOrEmpty(phrase)) return string.Empty;
            string str = phrase.ToLowerInvariant();
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", " ").Trim();
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s", "-");
            return str;
        }
    }
}