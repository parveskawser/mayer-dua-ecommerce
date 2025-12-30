using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static MDUA.Entities.ProductVariant;
using System.Net;

namespace MDUA.Facade
{
    public class ProductFacade : IProductFacade
    {
        private readonly IAttributeNameDataAccess _attributeNameDataAccess;
        private readonly IProductDataAccess _ProductDataAccess;
        private readonly IProductImageDataAccess _ProductImageDataAccess;
        private readonly IProductReviewDataAccess _ProductReviewDataAccess;
        private readonly IProductVariantDataAccess _ProductVariantDataAccess;
        private readonly IProductDiscountDataAccess _ProductDiscountDataAccess;
        private readonly IProductCategoryDataAccess _categoryDataAccess;
        private readonly IProductAttributeDataAccess _productAttributeDataAccess;
        private readonly IVariantPriceStockDataAccess _variantPriceStockDataAccess;
        private readonly IVariantImageDataAccess _variantImageDataAccess;
        private readonly ICompanyDataAccess _companyDataAccess;
        private readonly IProductVideoDataAccess _productVideoDataAccess;
        private readonly IGlobalSettingDataAccess _globalSettingDataAccess;
        private readonly IProductCategoryDataAccess _productCategoryDataAccess;

        private static readonly List<string> _sizeSortOrder = new List<string>

        {
            "XXXS", "XXS", "XS", "S", "M", "L", "XL", "XXL", "2XL", "XXXL", "3XL", "4XL", "5XL"
        };

        public ProductFacade(
            IProductDataAccess productDataAccess,
            IProductImageDataAccess productImageDataAccess,
            IProductReviewDataAccess productReviewDataAccess,
            IProductVariantDataAccess productVariantDataAccess,
            IProductDiscountDataAccess productDiscountDataAccess,
            IProductCategoryDataAccess categoryDataAccess,
            IAttributeNameDataAccess attributeNameDataAccess,
            IProductAttributeDataAccess productAttributeDataAccess,
            IVariantPriceStockDataAccess variantPriceStockDataAccess,
            IVariantImageDataAccess variantImageDataAccess,
            ICompanyDataAccess companyDataAccess,
            IGlobalSettingDataAccess globalSettingDataAccess,
            IProductVideoDataAccess productVideoDataAccess,
            IProductCategoryDataAccess productCategoryDataAccess)
        {
            _ProductDataAccess = productDataAccess;
            _ProductImageDataAccess = productImageDataAccess;
            _ProductReviewDataAccess = productReviewDataAccess;
            _ProductVariantDataAccess = productVariantDataAccess;
            _ProductDiscountDataAccess = productDiscountDataAccess;
            _categoryDataAccess = categoryDataAccess;
            _attributeNameDataAccess = attributeNameDataAccess;
            _productAttributeDataAccess = productAttributeDataAccess;
            _variantPriceStockDataAccess = variantPriceStockDataAccess;
            _variantImageDataAccess = variantImageDataAccess;
            _companyDataAccess = companyDataAccess;
            _globalSettingDataAccess = globalSettingDataAccess;
            _productVideoDataAccess = productVideoDataAccess;
            _productCategoryDataAccess = productCategoryDataAccess;

        }

        #region Common Implementation
        public long Delete(int id) => _ProductDataAccess.Delete(id);

        public Product Get(int id) => _ProductDataAccess.Get(id);

        public ProductList GetAll() => _ProductDataAccess.GetAll();

        public ProductList GetByQuery(string query) => _ProductDataAccess.GetByQuery(query);

        public long Insert(ProductBase obj) => _ProductDataAccess.Insert(obj);

        public long Update(ProductBase obj) => _ProductDataAccess.Update(obj);
        #endregion

        #region Extended Implementation

        public Product GetProductDetailsForWebBySlug(string slug)
        {
            Product product = _ProductDataAccess.GetBySlug(slug);

            if (product == null || !product.IsActive)
                return null;

            // 1. Load related entities
            product.Images = _ProductImageDataAccess.GetByProductId(product.Id);
            product.Reviews = _ProductReviewDataAccess.GetByProductId(product.Id);
            product.Variants = _ProductVariantDataAccess.GetByProductId(product.Id);
            // Note: We don't fetch Active
            //
            //
            // here anymore, we calculate BestDiscount below

            // 2. Load Dynamic Specifications (Truth-Based)
            product.Specifications = _attributeNameDataAccess.GetSpecificationsByProductId(product.Id);

            // 3. Populate Available Sizes
            var sizeKey = product.Specifications.Keys
                .FirstOrDefault(k => k.Contains("size", StringComparison.OrdinalIgnoreCase)
                                  || k.Contains("dimension", StringComparison.OrdinalIgnoreCase));

            if (sizeKey != null)
            {
                product.AvailableSizes = product.Specifications[sizeKey];
            }
            else
            {
                product.AvailableSizes = product.Specifications.Values.FirstOrDefault() ?? new List<string>();
            }

            // 4. Sort AvailableSizes (S, M, L, XL...)
            product.AvailableSizes = product.AvailableSizes
                .OrderBy(s =>
                {
                    var safeSize = (s ?? "").ToUpper().Trim();
                    var index = _sizeSortOrder.IndexOf(safeSize);
                    return index == -1 ? 999 : index;
                })
                .ThenBy(s => s)
                .ToList();

            // 5. Update Specifications with the SORTED list
            if (sizeKey != null)
            {
                product.Specifications[sizeKey] = product.AvailableSizes;
            }

            // 6. Calculate Best Discount Strategy (ONCE)
            decimal basePrice = product.BasePrice ?? 0;
            var bestDiscount = GetBestDiscount(product.Id, basePrice);

            // 7. Loop Variants (Handle Stock, Images, and Pricing together)
            int totalStock = 0;
            foreach (var v in product.Variants)
            {
                // A. Stock Logic
                try
                {
                    var stockInfo = _variantPriceStockDataAccess.Get(v.Id);
                    if (stockInfo != null)
                    {
                        v.StockQty = stockInfo.StockQty;
                        totalStock += v.StockQty;
                        // Sync price if missing in variant table
                        if (v.VariantPrice == 0 || v.VariantPrice == null) v.VariantPrice = stockInfo.Price;
                    }
                }
                catch { }

                // B. Image Logic
                var images = _variantImageDataAccess.GetImagesForVariant(v.Id);
                if (images != null && images.Count > 0)
                {
                    string rawPath = images[0].ImageUrl;
                    if (!string.IsNullOrEmpty(rawPath))
                    {
                        v.VariantImageUrl = rawPath.Replace("\\", "/");
                        if (!v.VariantImageUrl.StartsWith("/")) v.VariantImageUrl = "/" + v.VariantImageUrl;
                    }
                }



                // C. Pricing Logic
                decimal vPrice = v.VariantPrice ?? 0;
                decimal vSellingPrice = vPrice;

                if (bestDiscount != null)
                {
                    if (bestDiscount.DiscountType.Equals("Flat", StringComparison.OrdinalIgnoreCase))
                        vSellingPrice -= bestDiscount.DiscountValue;
                    else if (bestDiscount.DiscountType.Equals("Percentage", StringComparison.OrdinalIgnoreCase))
                        vSellingPrice -= (vPrice * (bestDiscount.DiscountValue / 100));
                }

                v.DiscountedPrice = Math.Max(vSellingPrice, 0);
            }
            var videos = _productVideoDataAccess.GetByProductId(product.Id);

            // Find Primary or fallback to first
            var primaryVideo = videos.FirstOrDefault(v => v.IsPrimary) ?? videos.FirstOrDefault();

            if (primaryVideo != null)
            {
                product.ProductVideoUrl = primaryVideo.VideoUrl;
            }
            product.TotalStockQuantity = totalStock;

            // 8. Calculate Main Product Selling Price (Using Best Discount)
            decimal sellingPrice = basePrice;

            if (bestDiscount != null)
            {
                if (bestDiscount.DiscountType.Equals("Flat", StringComparison.OrdinalIgnoreCase))
                    sellingPrice -= bestDiscount.DiscountValue;
                else if (bestDiscount.DiscountType.Equals("Percentage", StringComparison.OrdinalIgnoreCase))
                    sellingPrice -= (basePrice * (bestDiscount.DiscountValue / 100));
            }

            product.SellingPrice = Math.Max(sellingPrice, 0);
            product.ActiveDiscount = bestDiscount; // Assign to model for UI

            // 9. Load Company Info
            var company = _companyDataAccess.GetCompanySafe(product.CompanyId);
            if (company != null)
            {
                string dbLogo = company.LogoImg;
                if (string.IsNullOrEmpty(dbLogo))
                {
                    product.CompanyLogoUrl = "/images/logo.jpg";
                }
                else if (dbLogo.StartsWith("/images/") || dbLogo.StartsWith("\\images\\"))
                {
                    product.CompanyLogoUrl = dbLogo;
                }
                else
                {
                    product.CompanyLogoUrl = $"/images/logo/{company.Id}/{dbLogo}";
                }
                product.CompanyName = company.CompanyName;
            }
            else
            {
                product.CompanyLogoUrl = "/images/logo.jpg";
            }

            string dhakaStr = _globalSettingDataAccess.GetValue(product.CompanyId, "DeliveryCharge_Dhaka");
            string outsideStr = _globalSettingDataAccess.GetValue(product.CompanyId, "DeliveryCharge_Outside");

            int.TryParse(dhakaStr, out int dhakaCharge);
            int.TryParse(outsideStr, out int outsideCharge);

            // Fallback defaults if DB is empty
            if (dhakaCharge == 0) dhakaCharge = 60;
            if (outsideCharge == 0) outsideCharge = 120;

            product.DeliveryCharges = new Dictionary<string, int>
    {
        { "dhaka", dhakaCharge },
        { "outside", outsideCharge }
    };

            return product;
        }
        public long AddProduct(Product product, string username, int companyId)

        {

            if (product == null)

                throw new ArgumentNullException(nameof(product));

            product.CompanyId = companyId;

            product.CreatedBy = username;

            product.UpdatedBy = username;

            product.CreatedAt = DateTime.UtcNow;

            product.UpdatedAt = DateTime.UtcNow;

            product.ReorderLevel = product.ReorderLevel < 0 ? 0 : product.ReorderLevel;

            // 1️⃣ INSERT PRODUCT

            long productId = _ProductDataAccess.Insert(product);

            if (productId <= 0)

                return productId;

            // 2️⃣ INSERT ATTRIBUTES (If any selected)

            if (product.Attributes != null && product.Attributes.Count > 0)

            {

                int displayOrder = 1;

                foreach (var attr in product.Attributes)

                {

                    attr.ProductId = (int)productId;

                    attr.DisplayOrder = displayOrder++;

                    _productAttributeDataAccess.Insert(attr);

                }

            }

            // 3️⃣ HANDLE VARIANTS

            // If user provided variants (e.g. Size: S, M, L), insert them normally.

            if (product.Variants != null && product.Variants.Count > 0)

            {

                foreach (var variant in product.Variants)

                {

                    variant.ProductId = (int)productId;

                    variant.CreatedBy = username;

                    variant.CreatedAt = DateTime.UtcNow;

                    int variantId = _ProductVariantDataAccess.Insert(variant);

                    if (variantId > 0)

                    {

                        var vps = new VariantPriceStock

                        {

                            Id = variantId,

                            Price = (decimal)(variant.VariantPrice ?? product.BasePrice), // Fallback to Base Price

                            CompareAtPrice = null,

                            CostPrice = null,

                            StockQty = 0,

                            TrackInventory = true,

                            AllowBackorder = false,

                            WeightGrams = null

                        };

                        _variantPriceStockDataAccess.Insert(vps);

                        // Insert Attribute Values for this specific variant

                        if (variant.AttributeValueIds != null)

                        {

                            int displayOrder = 1;

                            foreach (int valueId in variant.AttributeValueIds)

                            {

                                _ProductVariantDataAccess.InsertVariantAttributeValue(

                                    variantId, valueId, displayOrder++);

                            }

                        }

                    }

                }

            }

            else

            {

                // ✅ DEFAULT VARIANT LOGIC (For Simple Products)

                // If NO variants were provided, create one "Standard" variant automatically.

                // This ensures SalesOrderDetail and Inventory tables can link to a Variant ID.

                var defaultVariant = new ProductVariant

                {

                    ProductId = (int)productId,

                    VariantName = "Standard", // Internal name for simple products
                    VariantPrice = (decimal)(product.BasePrice), // Fallback to Base Price

                    IsActive = true,

                    CreatedBy = username,

                    CreatedAt = DateTime.UtcNow

                };

                int defaultVariantId = _ProductVariantDataAccess.Insert(defaultVariant);

                if (defaultVariantId > 0)

                {

                    // Link Price & Stock to this default variant

                    var vps = new VariantPriceStock

                    {

                        Id = defaultVariantId,

                        Price = (decimal)product.BasePrice, // Use the product's main price

                        CompareAtPrice = null,

                        CostPrice = null,

                        StockQty = 0, // Default stock 0, updated via Purchase later

                        TrackInventory = true,

                        AllowBackorder = false,

                        WeightGrams = null

                    };

                    _variantPriceStockDataAccess.Insert(vps);

                }

            }

            return productId;

        }
        public List<VariantAttributeDto> GetVariantAttributes(int productId)
        {
            return _ProductVariantDataAccess.GetVariantAttributesByProductId(productId);
        }
        public ProductList GetLastFiveProducts()
        {
            return _ProductDataAccess.GetLastFiveProducts();
        }

        public List<Product> GetAllProductsWithCategory()
        {
            var products = _ProductDataAccess.GetAll(); // returns List<Product> or ProductList

            // Get all categories in one query
            var categories = _categoryDataAccess.GetAll().ToDictionary(c => c.Id, c => c.Name);

            // Fill CategoryName for each product
            foreach (var p in products)
            {
                if (p.CategoryId.HasValue && categories.ContainsKey(p.CategoryId.Value))
                    p.CategoryName = categories[p.CategoryId.Value];
                else
                    p.CategoryName = "N/A";

                decimal basePrice = p.BasePrice ?? 0;

                // ✅ NEW: Get the Single Best Discount
                var bestDiscount = GetBestDiscount(p.Id, basePrice);

                decimal sellingPrice = basePrice;

                if (bestDiscount != null)
                {
                    if (bestDiscount.DiscountType == "Flat")
                        sellingPrice -= bestDiscount.DiscountValue;
                    else if (bestDiscount.DiscountType == "Percentage")
                        sellingPrice -= (basePrice * (bestDiscount.DiscountValue / 100));
                }

                p.SellingPrice = Math.Max(sellingPrice, 0);
                p.ActiveDiscount = bestDiscount; // The View will show this specific discount
            }

            return products.ToList();
        }

        public UserLoginResult GetAddProductData(int userId)
        {
            var result = new UserLoginResult
            {
                Categories = _categoryDataAccess.GetAll()?.ToList() ?? new List<ProductCategory>(),
                Attributes = _attributeNameDataAccess.GetAll()?.ToList() ?? new List<AttributeName>()
            };

            return result;
        }

        public List<AttributeValue> GetAttributeValues(int attributeId)
        {
            return _attributeNameDataAccess.GetValuesByAttributeId(attributeId);
        }

        public ProductVariantList GetVariantsByProductId(int productId)
        {
            return _ProductVariantDataAccess.GetProductVariantsByProductId(productId);
        }

        public bool? ToggleProductStatus(int productId)
        {
            return _ProductDataAccess.ToggleStatus(productId);
        }

        public Product GetProductDetails(int productId)
        {
            // 1. Get the base product
            // We use GetProductById, which you already have.
            Product product = _ProductDataAccess.GetProductById(productId);
            if (product == null)
            {
                return null;
            }

            // 2. Get its variants
            // We use GetProductVariantsByProductId, which you also have.
            product.Variants = _ProductVariantDataAccess.GetProductVariantsByProductId(productId).ToList();

            // 3. Get its category name
            if (product.CategoryId.HasValue)
            {
                // Assuming _categoryDataAccess.Get(id) exists
                var category = _categoryDataAccess.Get(product.CategoryId.Value);
                product.CategoryName = category?.Name ?? "N/A";
            }
            else
            {
                product.CategoryName = "N/A";
            }

            return product;
        }

        public ProductResult GetProductForEdit(int productId)
        {
            var model = new ProductResult
            {
                // ✅ THIS IS THE FIX
                Product = _ProductDataAccess.Get(productId),

                Categories = _categoryDataAccess.GetAll()?.ToList() ?? new List<ProductCategory>()
            };

            return model;
        }

        public long UpdateProduct(Product product, string username)
        {
            product.UpdatedBy = username;
            product.UpdatedAt = DateTime.UtcNow;// [cite_start]// [cite: 179]
            return _ProductDataAccess.Update(product);
        }
        public long UpdateVariantPrice(int variantId, decimal newPrice, string newSku)
        {
            return _variantPriceStockDataAccess.UpdatePrice(variantId, newPrice, newSku);
        }
        public long DeleteVariant(int variantId)
        {

            return _ProductVariantDataAccess.Delete(variantId);
        }

        public List<AttributeName> GetAttributesForProduct(int productId)
        {
            return _attributeNameDataAccess.GetByProductId(productId);
        }

        public long AddVariantToExistingProduct(ProductVariant variant)
        {
            // 1. Insert Variant
            int variantId = _ProductVariantDataAccess.Insert(variant);

            if (variantId > 0)
            {
                // 2. Insert Price/Stock
                var vps = new VariantPriceStock
                {
                    Id = variantId,
                    Price = variant.VariantPrice ?? 0,
                    StockQty = variant.StockQty,
                    TrackInventory = true,
                    AllowBackorder = false
                };
                _variantPriceStockDataAccess.Insert(vps);

                // 3. Insert Attributes
                if (variant.AttributeValueIds != null)
                {
                    int displayOrder = 1;
                    foreach (int valueId in variant.AttributeValueIds)
                    {
                        _ProductVariantDataAccess.InsertVariantAttributeValue(variantId, valueId, displayOrder++);
                    }
                }
            }
            return variantId;
        }

        public ProductVariantResult GetVariantsWithAttributes(int productId)
        {
            var result = new ProductVariantResult();
            result.ProductId = productId;
            var product = _ProductDataAccess.Get(productId);
            decimal basePrice = product?.BasePrice ?? 0;
            result.BasePrice = basePrice;

            var variantList = _ProductVariantDataAccess.GetProductVariantsByProductId(productId);
            result.Variants = new List<ProductVariant>(variantList);

            var bestDiscount = GetBestDiscount(productId, basePrice);

            // Apply to variants
            foreach (var v in result.Variants)
            {
                decimal vPrice = v.VariantPrice ?? 0;
                decimal vSellingPrice = vPrice;

                if (bestDiscount != null)
                {
                    if (bestDiscount.DiscountType == "Flat")
                        vSellingPrice -= bestDiscount.DiscountValue;
                    else if (bestDiscount.DiscountType == "Percentage")
                        vSellingPrice -= (vPrice * (bestDiscount.DiscountValue / 100));
                }

                v.DiscountedPrice = vSellingPrice < 0 ? 0 : vSellingPrice;
            }       // MUST USE GetAll() to fetch attributes for the dropdown list
            result.AvailableAttributes = _attributeNameDataAccess.GetAll()?.ToList()
                                                 ?? new List<AttributeName>();
            result.ReorderLevel = product?.ReorderLevel ?? 0;
            return result;
        }

        public List<AttributeName> GetMissingAttributesForVariant(int productId, int variantId)
        {
            return _attributeNameDataAccess.GetMissingAttributesForVariant(productId, variantId);
        }

        // 2. Add Attribute & Auto-Update Name

        public void AddAttributeToVariant(int variantId, int attributeValueId)
        {
            // 1. Insert the link (Attribute -> Variant)
            _ProductVariantDataAccess.InsertVariantAttributeValue(variantId, attributeValueId, 99);

            // 2. Get the text for the value we just added (e.g., "Cotton")
            string valueName = _attributeNameDataAccess.GetValueName(attributeValueId);

            // 3. Get the current Variant to see its old name
            var variant = _ProductVariantDataAccess.Get(variantId);

            if (variant != null && !string.IsNullOrEmpty(valueName))
            {
                // 4. Append the new value to the existing name
                // Old: "Product - Red" -> New: "Product - Red - Cotton"
                string newVariantName = $"{variant.VariantName} - {valueName}";

                // 5. Save the new name safely
                _ProductVariantDataAccess.UpdateVariantName(variantId, newVariantName);
            }
        }

        // 3. Implement the Methods
        public List<ProductDiscount> GetDiscountsByProductId(int productId)
        {
            // Using the method from your DAL
            return _ProductDiscountDataAccess.GetByProductId(productId).ToList();
        }

        public long AddDiscount(ProductDiscount discount)
        {
            // Set defaults if needed
            discount.CreatedAt = DateTime.UtcNow;
            discount.UpdatedAt = DateTime.UtcNow;
            return _ProductDiscountDataAccess.Insert(discount);
        }

        public long UpdateDiscount(ProductDiscount discount)
        {
            discount.UpdatedAt = DateTime.UtcNow;
            return _ProductDiscountDataAccess.Update(discount);
        }

        public long DeleteDiscount(int id)
        {
            return _ProductDiscountDataAccess.Delete(id);
        }

        public Product GetProductWithPrice(int productId)
        {
            var product = _ProductDataAccess.Get(productId);
            if (product == null) return null;

            decimal basePrice = product.BasePrice ?? 0;
            var bestDiscount = GetBestDiscount(productId, basePrice);

            decimal sellingPrice = basePrice;

            if (bestDiscount != null)
            {
                if (bestDiscount.DiscountType == "Flat")
                    sellingPrice -= bestDiscount.DiscountValue;
                else if (bestDiscount.DiscountType == "Percentage")
                    sellingPrice -= (basePrice * (bestDiscount.DiscountValue / 100));
            }

            product.SellingPrice = Math.Max(sellingPrice, 0);
            product.ActiveDiscount = bestDiscount;

            return product;
        }

        // Helper to find the single best discount from a list of potential discounts
        // In src/MDUA.Facade/ProductFacade.cs

        public ProductDiscount GetBestDiscount(int productId, decimal basePrice)
        {
            var allDiscounts = _ProductDiscountDataAccess.GetByProductId(productId);

            DateTime now = DateTime.UtcNow;

            // 3. Filter discounts
            var validDiscounts = allDiscounts
                .Where(d => d.IsActive
                         && d.EffectiveFrom <= now
                         && (d.EffectiveTo == null || d.EffectiveTo >= now))
                .ToList();

            if (!validDiscounts.Any()) return null;

            // 4. Calculate the Best Price Strategy (lowest price wins)
            ProductDiscount bestDiscount = null;
            decimal lowestPriceFound = basePrice;

            foreach (var d in validDiscounts)
            {
                decimal calculatedPrice = basePrice;

                if (string.Equals(d.DiscountType, "Flat", StringComparison.OrdinalIgnoreCase))
                {
                    calculatedPrice -= d.DiscountValue;
                }
                else if (string.Equals(d.DiscountType, "Percentage", StringComparison.OrdinalIgnoreCase))
                {
                    calculatedPrice -= (basePrice * (d.DiscountValue / 100));
                }

                // Ensure price doesn't drop below zero
                calculatedPrice = Math.Max(calculatedPrice, 0);

                if (calculatedPrice < lowestPriceFound)
                {
                    lowestPriceFound = calculatedPrice;
                    bestDiscount = d;
                }
            }

            return bestDiscount;
        }
        public List<ProductImage> GetProductImages(int productId)
        {
            // Calls the Data Access layer
            return _ProductImageDataAccess.GetByProductId(productId).ToList();
        }
        public long AddProductImage(int productId, string imageUrl, bool isPrimary, string username)
        {
            // 1. Check if any images already exist for this product
            var existingImages = _ProductImageDataAccess.GetByProductId(productId);
            bool isFirstImage = (existingImages.Count == 0);

            var img = new ProductImage
            {
                ProductId = productId,
                ImageUrl = imageUrl,

                // ✅ LOGIC: If User said Primary OR it's the First Image -> True
                IsPrimary = isPrimary || isFirstImage,

                SortOrder = existingImages.Count + 1, // Auto-increment sort order
                AltText = "Product Image",
                CreatedBy = username,
                CreatedAt = DateTime.UtcNow
            };
            return _ProductImageDataAccess.Insert(img);
        }

        public List<VariantImage> GetVariantImages(int variantId)
        {
            return _variantImageDataAccess.GetImagesForVariant(variantId).ToList();
        }

        public long AddVariantImage(int variantId, string imageUrl, string username)
        {
            var img = new VariantImage
            {
                VariantId = variantId,
                ImageUrl = imageUrl,
                AltText = "Variant Image",
                DisplayOrder = 1
            };
            return _variantImageDataAccess.Insert(img);
        }

        public long DeleteVariantImage(int imageId)
        {
            return _variantImageDataAccess.Delete(imageId);
        }

        public long DeleteProductImage(int imageId)
        {
            // Calls the Data Access delete method
            return _ProductImageDataAccess.Delete(imageId);
        }

        public ProductImage GetProductImage(int id)
        {
            // Uses the existing .Get(id) method in your DataAccess
            return _ProductImageDataAccess.Get(id);
        }

        public VariantImage GetVariantImage(int id)
        {
            // Uses the existing .Get(id) method in your DataAccess
            return _variantImageDataAccess.Get(id);
        }

        public void SetProductImageAsPrimary(int imageId, int productId)
        {
            // 1. Get all images for this product
            var allImages = _ProductImageDataAccess.GetByProductId(productId);

            foreach (var img in allImages)
            {
                if (img.Id == imageId)
                {
                    // Set this one as Primary
                    if (!img.IsPrimary)
                    {
                        img.IsPrimary = true;
                        _ProductImageDataAccess.Update(img);
                    }
                }
                else
                {
                    // Unset others
                    if (img.IsPrimary)
                    {
                        img.IsPrimary = false;
                        _ProductImageDataAccess.Update(img);
                    }
                }
            }
        }

        public void UpdateProductImageSortOrder(int imageId, int sortOrder)
        {
            var img = _ProductImageDataAccess.Get(imageId);
            if (img != null)
            {
                img.SortOrder = sortOrder;
                _ProductImageDataAccess.Update(img);
            }
        }
        public void UpdateVariantImageDisplayOrder(int imageId, int displayOrder)
        {
            // 1. Get existing image
            var img = _variantImageDataAccess.Get(imageId);

            if (img != null)
            {
                // 2. Update property
                img.DisplayOrder = displayOrder;

                // 3. Save to DB using existing Update method
                _variantImageDataAccess.Update(img);
            }
        }

        public ProductList SearchProducts(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new ProductList();
            }

            // 1. Get matching products from Database
            var products = _ProductDataAccess.SearchProducts(searchTerm);

            // 2. Filter Variants inside the results
            foreach (var product in products)
            {
                if (product.IsVariantBased == true)
                {
                    // Fetch ALL variants for this product first
                    var allVariants = _ProductVariantDataAccess.GetProductVariantsByProductId(product.Id);

                    // Determine if the search matched the Parent Product Name
                    bool isParentMatch = product.ProductName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (isParentMatch)
                    {
                        // SCENARIO 1: User searched "T-Shirt" (Parent Name).
                        // Action: Show ALL variants (S, M, L) so they see full availability.
                        product.Variants = allVariants;
                    }
                    else
                    {
                        // SCENARIO 2: User searched "Small" (Variant Name).
                        // Action: Filter list to show ONLY "Small". Hide "Medium" and "Large".
                        product.Variants = allVariants.Where(v =>
                            (!string.IsNullOrEmpty(v.VariantName) && v.VariantName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (!string.IsNullOrEmpty(v.SKU) && v.SKU.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                        ).ToList();
                    }
                }
            }

            return products;
        }


        #endregion


        // =========================================================
        // VIDEO MANAGEMENT REGION
        // =========================================================
        public List<ProductVideo> GetProductVideos(int productId)
        {
            // Get list ordered by SortOrder
            return _productVideoDataAccess.GetByProductId(productId)
                                          .OrderBy(v => v.SortOrder)
                                          .ToList();
        }
        public async Task<long> AddProductVideo(ProductVideo video, string username)
        {
            // A. Validate generic requirements
            if (video == null) throw new ArgumentNullException(nameof(video));
            if (string.IsNullOrWhiteSpace(video.VideoUrl)) throw new ArgumentException("Video URL is required.");

            // B. SERVER-SIDE VALIDATION & CONVERSION
            string embedUrl = await ConvertToEmbedUrl(video.VideoUrl);

            // If the conversion returned null/empty, it means the URL was invalid.
            if (string.IsNullOrEmpty(embedUrl))
            {
                throw new ArgumentException("Invalid Video URL. Only YouTube, Vimeo, and Facebook are supported.");
            }

            // C. Set the validated/converted URL
            video.VideoUrl = embedUrl;

            // D. Audit Fields
            video.CreatedBy = username;
            video.CreatedAt = DateTime.UtcNow;
            video.UpdatedBy = username;
            video.UpdatedAt = DateTime.UtcNow;

            if (string.IsNullOrEmpty(video.ThumbnailUrl)) video.ThumbnailUrl = "";

            // E. Sort Order Logic
            var existingVideos = _productVideoDataAccess.GetByProductId(video.ProductId);
            if (existingVideos != null && existingVideos.Count > 0)
            {
                video.SortOrder = existingVideos.Max(v => v.SortOrder) + 1;
            }
            else
            {
                video.SortOrder = 1;
                video.IsPrimary = true; // First video is always primary
            }

            // F. Primary Logic
            if (video.IsPrimary && existingVideos != null && existingVideos.Count > 0)
            {
                var currentPrimary = existingVideos.FirstOrDefault(v => v.IsPrimary);
                if (currentPrimary != null)
                {
                    currentPrimary.IsPrimary = false;
                    currentPrimary.UpdatedBy = username;
                    currentPrimary.UpdatedAt = DateTime.UtcNow;
                    _productVideoDataAccess.Update(currentPrimary);
                }
            }

            return _productVideoDataAccess.Insert(video);
        }
        public async Task<string> ConvertToEmbedUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;

            // 1. Resolve Redirects (Share links) - CRITICAL for loading content
            if (url.Contains("facebook.com/share/") || url.Contains("fb.watch"))
            {
                // Now awaiting the async method
                string resolvedUrl = await ResolveRedirect(url);
                if (!string.IsNullOrEmpty(resolvedUrl)) url = resolvedUrl;
            }

            // 2. Already Embedded Check
            if (url.Contains("facebook.com/plugins/video.php") ||
                url.Contains("player.vimeo.com/video/") ||
                url.Contains("youtube.com/embed/"))
            {
                return url;
            }

            // 3. YouTube (Handles Standard, Shorts, Embed, Youtu.be)
            var ytMatch = System.Text.RegularExpressions.Regex.Match(url, @"(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?|shorts)\/|.*[?&]v=)|youtu\.be\/)([^""&?\/\s]{11})", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (ytMatch.Success)
            {
                return $"https://www.youtube.com/embed/{ytMatch.Groups[1].Value}";
            }

            // 4. Vimeo (Handles vimeo.com/ID and player.vimeo.com/video/ID)
            var vimeoMatch = System.Text.RegularExpressions.Regex.Match(url, @"(?:vimeo\.com\/|player\.vimeo\.com\/video\/)(\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (vimeoMatch.Success)
            {
                return $"https://player.vimeo.com/video/{vimeoMatch.Groups[1].Value}";
            }

            // 5. Facebook
            if (url.Contains("facebook.com"))
            {
                var encodedUrl = System.Net.WebUtility.UrlEncode(url);
                return $"https://www.facebook.com/plugins/video.php?href={encodedUrl}&show_text=false&width=560";
            }

            return null;
        }

        private async Task<string> ResolveRedirect(string url)
        {
            try
            {
                using (var handler = new HttpClientHandler { AllowAutoRedirect = true })
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                    var request = new HttpRequestMessage(HttpMethod.Head, url);

                    var response = await client.SendAsync(request);

                    // will contain the final URI after the redirect chain.
                    return response.RequestMessage?.RequestUri?.AbsoluteUri ?? url;
                }
            }
            catch
            {
                // If resolution fails (e.g. timeout), return original url and hope for the best
                return url;
            }
        }

        public long DeleteProductVideo(int videoId)
        {
            // 1. Fetch the video first to check its status
            var videoToDelete = _productVideoDataAccess.Get(videoId);

            // Safety check: if video doesn't exist, stop
            if (videoToDelete == null) return 0;

            // 2. Logic: If we are deleting the PRIMARY video...
            if (videoToDelete.IsPrimary)
            {
                // Fetch all videos for this product
                var allVideos = _productVideoDataAccess.GetByProductId(videoToDelete.ProductId);

                // Find the "Next Best" video:
                // - Exclude the one we are deleting
                // - Order by SortOrder (ascending) so the next one in the list takes over
                var nextPrimary = allVideos
                                    .Where(v => v.Id != videoId)
                                    .OrderBy(v => v.SortOrder)
                                    .ThenBy(v => v.Id)
                                    .FirstOrDefault();

                // If a candidate exists, promote it
                if (nextPrimary != null)
                {
                    nextPrimary.IsPrimary = true;
                    nextPrimary.UpdatedAt = DateTime.UtcNow;
                    // Note: We don't have the username passed to this method signature, 
                    // but since it's an auto-system action, null/empty UpdatedBy is often acceptable.

                    _productVideoDataAccess.Update(nextPrimary);
                }
            }

            // 3. Finally, delete the target video
            return _productVideoDataAccess.Delete(videoId);
        }
        public void SetPrimaryProductVideo(int videoId, int productId, string username)
        {
            // 1. Get all videos
            var allVideos = _productVideoDataAccess.GetByProductId(productId);

            // 2. Loop and update to ensure ONLY ONE is primary
            foreach (var v in allVideos)
            {
                // Determine if this is the chosen one
                bool shouldBePrimary = (v.Id == videoId);

                // Only update the database if the status is actually changing
                if (v.IsPrimary != shouldBePrimary)
                {
                    v.IsPrimary = shouldBePrimary;
                    v.UpdatedBy = username;
                    v.UpdatedAt = DateTime.UtcNow;

                    _productVideoDataAccess.Update(v);
                }
            }
        }
        public List<LowStockItem> GetLowStockVariants(int topN)
        {
            return _variantPriceStockDataAccess.GetLowStockVariants(topN);
        }

        public LandingPageViewModel GetHomepageData()
        {
            var model = new LandingPageViewModel();

            // 1. Fetch Categories (Top 10 active)
            // Assuming you have a DAO method for this, or use GetByQuery
            // model.Categories = _productCategoryDataAccess.GetTopCategories(10); 
            // Mocking for now if DAO method missing:
            model.Categories = _productCategoryDataAccess.GetAll().Take(10).ToList();

            // 2. Fetch Products (Using a raw SQL query in DA is best, but here is logic)
            var products = _ProductDataAccess.GetRecentProductsWithImages(20); // You need to implement this in DA

            // Map to ViewModel
            foreach (var p in products)
            {
                var vm = new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.ProductName,
                    // Logic to get primary image or default
                    ImageUrl = _ProductImageDataAccess.GetPrimaryImage(p.Id) ?? "/images/default-book.png",
                    // Logic to get base price or lowest variant price
                    Price = p.BasePrice ?? 0
                };
                model.NewArrivals.Add(vm);
            }

            // Just shuffling for "Featured" for now
            model.FeaturedProducts = model.NewArrivals.OrderBy(x => Guid.NewGuid()).ToList();

            return model;
        }
    }
}
