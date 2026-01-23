using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.DataAccess.Interface;
using MDUA.Facade.Interface;
using MDUA.Framework.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MDUA.Web.UI.Controllers
{
    [Authorize]
    public class ProductController : BaseController
    {
        private readonly IProductFacade _productFacade;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPaymentMethodFacade _paymentMethodFacade;
        private readonly ISettingsFacade _settingsFacade;
        private readonly ISubscriptionSystemFacade _subscriptionFacade;
        private readonly IBackInStockRequestDataAccess _backInStockRequestDataAccess;

        public ProductController(
            IProductFacade productFacade,
            IWebHostEnvironment webHostEnvironment,
            IPaymentMethodFacade paymentMethodFacade,
            ISettingsFacade settingsFacade,
            ISubscriptionSystemFacade subscriptionFacade,
            IBackInStockRequestDataAccess backInStockRequestDataAccess)
        {
            _productFacade = productFacade;
            _webHostEnvironment = webHostEnvironment;
            _paymentMethodFacade = paymentMethodFacade;
            _settingsFacade = settingsFacade;
            _subscriptionFacade = subscriptionFacade;
            _backInStockRequestDataAccess = backInStockRequestDataAccess;

        }

        #region Products
        [AllowAnonymous]
        [Route("product/{slug}")]
        public IActionResult Index(string slug)
        {

            if (string.IsNullOrWhiteSpace(slug)) return BadRequest();

            Product model = _productFacade.GetProductDetailsForWebBySlug(slug);

            if (model == null) return NotFound();
            // --- FETCH PAYMENT METHODS ---
            var companyId = model.CompanyId; 

            // 1. Get all settings (merged)
            var allSettings = _settingsFacade.GetCompanyPaymentSettings(companyId);

            // 2. Filter only ENABLED methods for the View
            var enabledMethods = allSettings
                                    .Where(x => x.IsEnabled)
                                    .OrderBy(x => x.PaymentMethodId) // Or DisplayOrder if available in Result
                                    .ToList();

            ViewBag.PaymentMethods = enabledMethods;
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost("product/back-in-stock-request")]
        [ValidateAntiForgeryToken]
        public IActionResult CreateBackInStockRequest([FromBody] BackInStockRequestInput input)
        {
            if (input == null || input.ProductVariantId <= 0)
            {
                return Json(new { success = false, message = "Invalid product selection." });
            }

            string contactNumber = input.ContactNumber?.Trim();
            if (string.IsNullOrWhiteSpace(contactNumber))
            {
                return Json(new { success = false, message = "Please provide a valid contact number." });
            }

            if (contactNumber.Length > 50)
            {
                return Json(new { success = false, message = "Contact number is too long." });
            }

            var variant = _productFacade.GetVariant(input.ProductVariantId);
            if (variant == null)
            {
                return Json(new { success = false, message = "Variant not found." });
            }

            if (variant.StockQty > 0)
            {
                return Json(new { success = false, message = "This item is already in stock." });
            }

            if (_backInStockRequestDataAccess.HasPendingRequest(variant.Id, contactNumber))
            {
                return Json(new { success = true, alreadyRequested = true, message = "You are already on the notification list." });
            }

            var request = new BackInStockRequest
            {
                ProductVariantId = variant.Id,
                ContactNumber = contactNumber,
                RequestDate = DateTime.UtcNow,
                IsNotified = false,
                CreatedBy = "Guest",
                CreatedAt = DateTime.UtcNow
            };

            _backInStockRequestDataAccess.Insert(request);

            return Json(new { success = true, alreadyRequested = false, message = "We will notify you when this item is back in stock." });
        }

        public class BackInStockRequestInput
        {
            public int ProductVariantId { get; set; }
            public string ContactNumber { get; set; }
        }

        [Route("product/add")]
        [HttpGet]
        public IActionResult Add()
        {
            // Permission Check
            if (!HasPermission("Product.Add")) return HandleAccessDenied();
            // 🛑  IsProductLocked  (Not IsSubscriptionLocked)
           if (_subscriptionFacade.IsProductLocked(CurrentCompanyId, out int current, out int limit))
             {
                 // Pass "Product" as the feature
                 return RedirectToAction("LimitReached", "Subscription", new
                 {
                     current = current,
                     limit = limit,
                     feature = "Product" // <--- Important
                 });
             }

            // 2. Get Company Context
            int companyId = CurrentCompanyId;
            if (companyId <= 0) return RedirectToAction("Login", "Account");
            var model = _productFacade.GetAddProductData(companyId);

            return View(model);
        }

        [Route("product/add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProduct(Product product)
        {
            // Permission Check
            if (!HasPermission("Product.Add")) return HandleAccessDenied();
            if (product.Attributes != null)
            {
                // Remove attributes where AttributeId is 0 (invalid/unselected)
                product.Attributes = product.Attributes
                    .Where(a => a.AttributeId > 0)
                    .ToList();
            }
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
        [HttpGet]
        public IActionResult AllProducts(string search)
        {
            if (!HasPermission("Product.View")) return HandleAccessDenied();

            IEnumerable<Product> products;
            ViewData["CurrentSearch"] = search;

            // ✅ FIX: Pass CurrentCompanyId to the methods
            if (!string.IsNullOrWhiteSpace(search))
            {
                // Pass CompanyId here
                products = _productFacade.SearchProducts(search, CurrentCompanyId);
            }
            else
            {
                // Pass CompanyId here
                products = _productFacade.GetAllProductsWithCategory(CurrentCompanyId);
            }

            return View(products);
        }

        // Add this inside your ProductController class
        [HttpGet]
        [Route("product/check-slug")]
        public IActionResult CheckSlugAvailability(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return Json(new { exists = false });

            // Reuse your existing facade method
            var product = _productFacade.GetProductDetailsForWebBySlug(slug);

            return Json(new { exists = product != null });
        }

        [HttpGet]
        [Route("product/search-ajax")]
        public IActionResult SearchProductsAjax(string query)
        {
            IEnumerable<Product> model;

            if (string.IsNullOrWhiteSpace(query))
            {
                // ✅ FIX: Pass CurrentCompanyId to restrict to logged-in user's data
                model = _productFacade.GetAllProductsWithCategory(CurrentCompanyId);
            }
            else
            {
                // ✅ FIX: Pass CurrentCompanyId to restrict search results
                model = _productFacade.SearchProducts(query, CurrentCompanyId);
            }

            // This returns ONLY the table rows to the JavaScript
            return PartialView("_ProductTableRows", model);
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
            catch (WorkflowException wfEx)
            {
                // BUSINESS RULE FAILURE (Soft Delete Triggered)
                return Json(new
                {
                    success = false,
                    message = wfEx.Message,
                    wasDeactivated = true // <--- ADD THIS FLAG
                });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Something went wrong..." });
            }
        }
        [AllowAnonymous]
        [HttpGet("/product/variant-stocks")]
        public IActionResult GetVariantStocks(string ids)
        {
            // ids: "12,15,18"
            var list = new List<int>();

            if (!string.IsNullOrWhiteSpace(ids))
            {
                list = ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(x => x.Trim())
                          .Where(x => int.TryParse(x, out _))
                          .Select(int.Parse)
                          .Distinct()
                          .ToList();
            }

            var items = _productFacade.GetVariantStocks(list);

            return Json(new { items });
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
            newVariant.CreatedAt = DateTime.UtcNow;
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
    // 1. Permission Check
    if (!HasPermission("VariantImage.Add")) return HandleAccessDenied();

    if (file == null || file.Length == 0)
        return Json(new { success = false, message = "No file received" });

    // 2. Save File to Disk
    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
    // We save to a folder based on the specific variantId initially
    var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "variants", variantId.ToString());

    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

    var filePath = Path.Combine(folderPath, fileName);
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    string dbPath = $"/images/variants/{variantId}/{fileName}";

    // 3. Save to Database (Target Variant)
    _productFacade.AddVariantImage(variantId, dbPath, CurrentUserName);

    // =================================================================================
    // SMART SYNC START: Apply this image to all siblings with same Primary Attribute
    // =================================================================================
    try
    {
        // A. Get details of the current variant to find ProductId
        var variantInfo = _productFacade.GetVariant(variantId); 
        
        if (variantInfo != null)
        {
            int productId = variantInfo.ProductId;

            // B. Get ALL variants and attributes for this product
            // This returns the structure: { Attributes: [...], Variants: [...] }
            var fullData = _productFacade.GetVariantsWithAttributes(productId);

            if (fullData != null && fullData.Attributes != null && fullData.Variants != null)
            {
                // C. Identify Primary Attribute (DisplayOrder 1)
                var primaryAttribute = fullData.Attributes.FirstOrDefault(); 

                if (primaryAttribute != null)
                {
                    // D. Find the Primary Value of the CURRENT variant
                    // First need to find which AttributeValueId corresponds to the Primary Attribute
                    var currentVariantData = fullData.Variants.FirstOrDefault(v => v.Id == variantId);
                    
                    if (currentVariantData != null && currentVariantData.AttributeValueIds != null)
                    {
                        //  'AttributeValueIds' is a list ordered by the Attribute Display Order.
                        // So index 0 corresponds to the Primary Attribute.
                        var primaryValueId = currentVariantData.AttributeValueIds.FirstOrDefault();

                        if (primaryValueId > 0)
                        {
                            // E. Loop through SIBLINGS and Sync
                            foreach (var sibling in fullData.Variants)
                            {
                                // Skip self
                                if (sibling.Id == variantId) continue;

                                // Check if Sibling has the SAME Primary Value (e.g., Both are "Red")
                                var siblingPrimaryValue = sibling.AttributeValueIds.FirstOrDefault();

                                if (siblingPrimaryValue == primaryValueId)
                                {
                                    // COPY THE IMAGE RECORD
                                    //  use the same 'dbPath' so they share the physical file
                                    _productFacade.AddVariantImage(sibling.Id, dbPath, CurrentUserName);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        // Fail silently on sync errors so  doesn't block the main upload
         Console.WriteLine(ex.Message);
    }
    // =================================================================================
    //  SMART SYNC END
    // =================================================================================

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

        [HttpGet]
        [Route("product/get-attribute-values")]
        public IActionResult GetAttributeValues(int attributeId)
        {
            //  Add permission check
            // if (!HasPermission("Product.Add")) return HandleAccessDenied();

            // Fetch data from  Facade
            var values = _productFacade.GetAttributeValues(attributeId);

            // Return as JSON. 
            return Json(values.Select(v => new {
                id = v.Id,
                value = v.Value 
            }));
        }



        #endregion
       

        #region Product Videos

        [HttpGet]
        [Route("product/get-videos")]
        public IActionResult GetVideosPartial(int productId)
        {
            if (!HasPermission("Product.View")) return HandleAccessDenied();

            var videos = _productFacade.GetProductVideos(productId);
            ViewData["ProductId"] = productId;
            return PartialView("_ProductVideosPartial", videos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/add-video")]
        public IActionResult AddVideo(ProductVideo video)
        {
            if (!HasPermission("Product.Edit")) return HandleAccessDenied();

            try
            {
                _productFacade.AddProductVideo(video, CurrentUserName);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/delete-video")]
        public IActionResult DeleteVideo(int id)
        {
            if (!HasPermission("Product.Edit")) return HandleAccessDenied();

            try
            {
                _productFacade.DeleteProductVideo(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/set-primary-video")]
        public IActionResult SetPrimaryVideo(int videoId, int productId)
        {
            if (!HasPermission("Product.Edit")) return HandleAccessDenied();

            try
            {
                _productFacade.SetPrimaryProductVideo(videoId, productId, CurrentUserName);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion
        #region Product SEO
        [HttpGet]
        [Route("product/get-seo")]
        public IActionResult GetSEOPartial(int productId)
        {
            if (!HasPermission("Product.Edit")) return HandleAccessDenied();

            // Fetch existing or create new blank object with ProductId
            var model = _productFacade.GetProductSEO(productId) ?? new ProductSEO { ProductId = productId };

            return PartialView("_ProductSEOPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/save-seo")]
        public IActionResult SaveSEO(ProductSEO seo)
        {
            if (!HasPermission("Product.Edit")) return HandleAccessDenied();

            try
            {
                if (seo.Id == 0) seo.CreatedBy = CurrentUserName;
                seo.UpdatedBy = CurrentUserName;

                _productFacade.SaveProductSEO(seo);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/upload-seo-image")]
        public async Task<IActionResult> UploadSEOImage(int productId, IFormFile file)
        {
            if (!HasPermission("Product.Edit")) return HandleAccessDenied();

            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "No file received" });

            try
            {
                // Path: /images/products/{id}/seo/
                var folderName = Path.Combine("images", "products", productId.ToString(), "seo");
                var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, folderName);

                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                var fileName = $"og_{DateTime.UtcNow.Ticks}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return the relative path so JS can set it in the input field
                string dbPath = $"/{folderName.Replace("\\", "/")}/{fileName}";
                return Json(new { success = true, filePath = dbPath });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

#endregion


    }
}
