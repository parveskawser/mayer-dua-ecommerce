using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        #region Products
        [AllowAnonymous]
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
            // Permission Check
            if (!HasPermission("Product.Add")) return HandleAccessDenied();

            var model = _productFacade.GetAddProductData(CurrentUserId);

            return View(model);
        }

        [Route("product/add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProduct(Product product)
        {
            // Permission Check
            if (!HasPermission("Product.Add")) return HandleAccessDenied();

            if (string.IsNullOrWhiteSpace(product.Slug))
                product.Slug = GenerateSlug(product.ProductName);
            else
                product.Slug = GenerateSlug(product.Slug);

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
            // Permission Check
            if (!HasPermission("Product.View")) return HandleAccessDenied();

            var products = _productFacade.GetAllProductsWithCategory();
            return View(products);
        }

        [HttpGet]
        [Route("product/get-attribute-values")]
        public IActionResult GetAttributeValues(int attributeId)
        {
            // Permission Check 
            if (!HasPermission("Product.View")) return HandleAccessDenied();

            var values = _productFacade.GetAttributeValues(attributeId);
            var result = values.Select(v => new { id = v.Id, value = v.Value }).ToList();
            return new JsonResult(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/toggle-status")]
        public IActionResult ToggleStatus(int productId)
        {
            // Permission Check
            if (!HasPermission("Product.Edit")) return HandleAccessDenied();

            bool? newStatus = _productFacade.ToggleProductStatus(productId);

            if (newStatus == null)
                return Json(new { success = false, message = "Product not found." });

            return Json(new { success = true, newIsActive = newStatus.Value });
        }

        [HttpGet]
        [Route("product/get-details-partial")]
        public IActionResult GetProductDetailsPartial(int productId)
        {
            // Permission Check
            if (!HasPermission("Product.Details")) return HandleAccessDenied();

            Product model = _productFacade.GetProductDetails(productId);
            if (model == null) return NotFound();

            return PartialView("_ProductDetailsPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/delete-confirmed")]
        public IActionResult DeleteConfirmed(int productId)
        {
            // Permission Check
            if (!HasPermission("Product.Delete")) return HandleAccessDenied();

            try
            {
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
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        #endregion

        #region parital edits and updates for product and its variants
        [HttpGet]
        [Route("product/get-edit-partial")]
        public IActionResult GetEditPartial(int productId)
        {
            // Permission Check
            if (!HasPermission("Product.Edit")) return HandleAccessDenied();

            var model = _productFacade.GetProductForEdit(productId);
            if (model.Product == null) return NotFound();

            return PartialView("_EditProductPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/edit-product")]
        public IActionResult EditProduct(Product product)
        {
            // Permission Check
            if (!HasPermission("Product.Edit")) return HandleAccessDenied();

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
            // Permission Check
            if (!HasPermission("Product.Edit")) return HandleAccessDenied();

            long result = _productFacade.UpdateVariantPrice(variantId, newPrice, newSku);
            return Json(new { success = result > 0, message = result > 0 ? "" : "Update failed." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/delete-variant")]
        public IActionResult DeleteVariant(int variantId)
        {
            // Permission Check
            if (!HasPermission("Variant.Delete")) return HandleAccessDenied();

            long result = _productFacade.DeleteVariant(variantId);
            return Json(new { success = result > 0, message = result > 0 ? "" : "Delete failed." });
        }

        [HttpGet]
        [Route("product/get-variants")]
        public IActionResult GetVariantsPartial(int productId)
        {
            // Permission Check
            if (!HasPermission("Variant.View")) return HandleAccessDenied();

            ProductVariantResult model = _productFacade.GetVariantsWithAttributes(productId);
            return PartialView("_ProductVariantsPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/add-single-variant")]
        public IActionResult AddSingleVariant(ProductVariant newVariant)
        {
            // Permission Check
            if (!HasPermission("Variant.Add")) return HandleAccessDenied();

            newVariant.CreatedBy = CurrentUserName;
            newVariant.CreatedAt = DateTime.Now;
            newVariant.IsActive = true;

            long result = _productFacade.AddVariantToExistingProduct(newVariant);
            return Json(new { success = result > 0, message = result > 0 ? "" : "Failed to add variant." });
        }

        #endregion

        #region attributes
        [HttpGet]
        [Route("product/get-missing-attributes")]
        public IActionResult GetMissingAttributes(int productId, int variantId)
        {
            // Permission Check 
            if (!HasPermission("Product.Add")) return HandleAccessDenied();

            var list = _productFacade.GetMissingAttributesForVariant(productId, variantId);
            return Json(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/add-attribute-to-variant")]
        public IActionResult AddAttributeToVariant(int variantId, int attributeValueId)
        {
            // Permission Check
            if (!HasPermission("Variant.Edit")) return HandleAccessDenied();

            _productFacade.AddAttributeToVariant(variantId, attributeValueId);
            return Json(new { success = true });
        }
        #endregion

        #region discounts and pricing
        [HttpGet]
        [Route("product/get-discounts")]
        public IActionResult GetDiscountsPartial(int productId)
        {
            // Permission Check
            if (!HasPermission("Discount.View")) return HandleAccessDenied();

            var discounts = _productFacade.GetDiscountsByProductId(productId);
            ViewBag.ProductId = productId;
            return PartialView("_ProductDiscountsPartial", discounts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/add-discount")]
        public IActionResult AddDiscount(ProductDiscount discount)
        {
            // Permission Check
            if (!HasPermission("Discount.Add")) return HandleAccessDenied();

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
            // Permission Check
            if (!HasPermission("Discount.Edit")) return HandleAccessDenied();

            discount.UpdatedBy = CurrentUserName;
            long result = _productFacade.UpdateDiscount(discount);
            return Json(new { success = result > 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/delete-discount")]
        public IActionResult DeleteDiscount(int id)
        {
            // Permission Check
            if (!HasPermission("Discount.Delete")) return HandleAccessDenied();

            long result = _productFacade.DeleteDiscount(id);
            return Json(new { success = result > 0 });
        }

        [HttpGet]
        [Route("product/get-updated-price")]
        public IActionResult GetUpdatedPrice(int productId)
        {
            // Permission Check
            if (!HasPermission("Product.View")) return HandleAccessDenied();

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

        #endregion

        #region product and variant images
        [HttpGet]
        [Route("product/get-images")]
        public IActionResult GetImagesPartial(int productId)
        {
            // Permission Check
            if (!HasPermission("ProductImage.View")) return HandleAccessDenied();

            var images = _productFacade.GetProductImages(productId);
            ViewBag.ProductId = productId;
            return PartialView("_ProductImagesPartial", images);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/upload-image")]
        public async Task<IActionResult> UploadImage(int productId, IFormFile file)
        {
            // Permission Check
            if (!HasPermission("ProductImage.Add")) return HandleAccessDenied();

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
            // Permission Check
            if (!HasPermission("ProductImage.SetPrimary")) return HandleAccessDenied();

            _productFacade.SetProductImageAsPrimary(imageId, productId);
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/update-image-order")]
        public IActionResult UpdateImageOrder(int imageId, int sortOrder)
        {
            // Permission Check
            if (!HasPermission("ProductImage.SetOrder")) return HandleAccessDenied();

            _productFacade.UpdateProductImageSortOrder(imageId, sortOrder);
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/delete-image")]
        public IActionResult DeleteImage(int id)
        {
            // Permission Check
            if (!HasPermission("ProductImage.Delete")) return HandleAccessDenied();

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
            // Permission Check
            if (!HasPermission("VariantImage.View")) return HandleAccessDenied();

            var images = _productFacade.GetVariantImages(variantId);
            ViewBag.VariantId = variantId;
            return PartialView("_VariantImagesPartial", images);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/upload-variant-image")]
        public async Task<IActionResult> UploadVariantImage(int variantId, IFormFile file)
        {
            // Permission Check
            if (!HasPermission("VariantImage.Add")) return HandleAccessDenied();

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
            // Permission Check
            if (!HasPermission("VariantImage.Delete")) return HandleAccessDenied();

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
            // Permission Check
            if (!HasPermission("VariantImage.SetOrder")) return HandleAccessDenied();

            _productFacade.UpdateVariantImageDisplayOrder(imageId, displayOrder);
            return Json(new { success = true });
        }
        #endregion

        #region Helpers & Slug

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


     
        #endregion
    }
}