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
using static MDUA.Entities.ProductVariant;

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
            IProductVideoDataAccess productVideoDataAccess)
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
            // Note: We don't fetch ActiveDiscount here anymore, we calculate BestDiscount below

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
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;
            product.ReorderLevel = product.ReorderLevel < 0 ? 0 : product.ReorderLevel;

            // 2️⃣ INSERT PRODUCT
            long productId = _ProductDataAccess.Insert(product);

            if (productId <= 0)
                return productId;

            if (product.Attributes != null)
            {
                int displayOrder = 1;
                foreach (var attr in product.Attributes)
                {
                    // The binder only set AttributeId, we set the rest
                    attr.ProductId = (int)productId;
                    attr.DisplayOrder = displayOrder++;

                    // Call the new DA class to insert
                    _productAttributeDataAccess.Insert(attr);
                }
            }

            // 3️⃣ INSERT VARIANTS
            foreach (var variant in product.Variants)
            {
                variant.ProductId = (int)productId;
                variant.CreatedBy = username;
                variant.CreatedAt = DateTime.Now;

                // Insert ProductVariant row
                int variantId = _ProductVariantDataAccess.Insert(variant);
                if (variantId > 0)
                {
                    var vps = new VariantPriceStock
                    {
                        Id = variantId, // Use the new Variant ID
                        Price = variant.VariantPrice ?? 0, // Get price from the form
                        CompareAtPrice = null, // Default
                        CostPrice = null, // Default
                        StockQty = 0, // Default stock is 0
                        TrackInventory = true, // Default from your table
                        AllowBackorder = false, // Default from your table
                        WeightGrams = null // Default
                    };

                    // Call the new DA class to insert
                    _variantPriceStockDataAccess.Insert(vps);
                }

                // 4️⃣ INSERT VARIANT ATTRIBUTE VALUES
                if (variant.AttributeValueIds != null)
                {
                    int displayOrder = 1;

                    foreach (int valueId in variant.AttributeValueIds)
                    {
                        _ProductVariantDataAccess.InsertVariantAttributeValue(
                            variantId,
                            valueId,
                            displayOrder++
                        );
                    }
                }
            }

            return productId;
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
            // You already have this method in ProductVariantDataAccess
            return _ProductVariantDataAccess.GetProductVariantsByProductId(productId);
        }

        public bool? ToggleProductStatus(int productId)
        {
            // Simply pass the call down to the DA layer
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
        // ... inside the ProductFacade class

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

        // This method is unchanged and still correct
        public long UpdateProduct(Product product, string username)
        {
            product.UpdatedBy = username;
            product.UpdatedAt = DateTime.Now;
            return _ProductDataAccess.Update(product);
        }
        public long UpdateVariantPrice(int variantId, decimal newPrice, string newSku)
        {
            // Pass only the ID and the new Price
            return _variantPriceStockDataAccess.UpdatePrice(variantId, newPrice,  newSku);
        }
        public long DeleteVariant(int variantId)
        {
            // ✅ Use the existing .Delete() method
            // Since your database has "ON DELETE CASCADE", 
            // this simple delete is all you need.
            return _ProductVariantDataAccess.Delete(variantId);
        }

        public List<AttributeName> GetAttributesForProduct(int productId)
        {
            return _attributeNameDataAccess.GetByProductId(productId);
        }

        // Ensure you have this method to handle the saving
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

        // 1. Get Missing Attributes
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
            discount.CreatedAt = DateTime.Now;
            discount.UpdatedAt = DateTime.Now;
            return _ProductDiscountDataAccess.Insert(discount);
        }

        public long UpdateDiscount(ProductDiscount discount)
        {
            discount.UpdatedAt = DateTime.Now;
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
        public ProductDiscount GetBestDiscount(int productId, decimal basePrice)
        {
            // 1. Get ALL discounts for this product
            var allDiscounts = _ProductDiscountDataAccess.GetByProductId(productId);

            // 2. Filter for currently active (Date range + IsActive flag)
            var now = DateTime.Now;
            var validDiscounts = allDiscounts
                .Where(d => d.IsActive
                         && d.EffectiveFrom <= now
                         && (d.EffectiveTo == null || d.EffectiveTo >= now))
                .ToList();

            if (!validDiscounts.Any()) return null;

            // 3. Calculate which one gives the LOWEST price
            ProductDiscount bestDiscount = null;
            decimal lowestPriceFound = basePrice;

            foreach (var d in validDiscounts)
            {
                decimal calculatedPrice = basePrice;

                if (d.DiscountType == "Flat")
                {
                    calculatedPrice -= d.DiscountValue;
                }
                else if (d.DiscountType == "Percentage")
                {
                    calculatedPrice -= (basePrice * (d.DiscountValue / 100));
                }

                // If this discount results in a lower price, it's the new winner
                if (calculatedPrice < lowestPriceFound)
                {
                    lowestPriceFound = calculatedPrice;
                    bestDiscount = d;
                }
            }

            return bestDiscount;
        }
        // In ProductFacade.cs

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
                CreatedAt = DateTime.Now
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
            return _ProductDataAccess.SearchProducts(searchTerm);
        }
        #endregion

        // ... inside ProductFacade class ...

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

        public long AddProductVideo(ProductVideo video, string username)
        {
            // 1. Convert URL
            video.VideoUrl = ConvertToEmbedUrl(video.VideoUrl);

            // 2. Audit Fields
            video.CreatedBy = username;
            video.CreatedAt = DateTime.Now;

            // ✅ FIX: Set UpdatedAt to prevent "SqlDateTime overflow" error (0001-01-01)
            // Your Stored Procedure inserts this column, so it must be valid.
            video.UpdatedBy = username;
            video.UpdatedAt = DateTime.Now;

            // ✅ FIX: Ensure ThumbnailUrl isn't null if DB expects a string
            if (string.IsNullOrEmpty(video.ThumbnailUrl)) video.ThumbnailUrl = "";

            // 3. Fetch Existing Videos to check state
            var existingVideos = _productVideoDataAccess.GetByProductId(video.ProductId);

            // 4. Logic: Auto-Calculate SortOrder (Max + 1)
            if (existingVideos != null && existingVideos.Count > 0)
            {
                video.SortOrder = existingVideos.Max(v => v.SortOrder) + 1;
            }
            else
            {
                video.SortOrder = 1; // First video starts at 1
            }

            // 5. Logic: Handle "IsPrimary"
            bool isFirstVideo = (existingVideos == null || existingVideos.Count == 0);

            if (isFirstVideo)
            {
                // Force Primary if it's the first video ever
                video.IsPrimary = true;
            }
            else if (video.IsPrimary)
            {
                // If user manually checked "Set as Primary" for a NEW video,
                // we must UNSET the existing primary video first.
                var currentPrimary = existingVideos.FirstOrDefault(v => v.IsPrimary);
                if (currentPrimary != null)
                {
                    currentPrimary.IsPrimary = false;
                    currentPrimary.UpdatedBy = username;
                    currentPrimary.UpdatedAt = DateTime.Now;
                    _productVideoDataAccess.Update(currentPrimary);
                }
            }

            return _productVideoDataAccess.Insert(video);
        }

        // ... inside the class ...

        public List<LowStockItem> GetLowStockVariants(int topN)
        {
            // _variantPriceStockDataAccess is already injected in your constructor
            return _variantPriceStockDataAccess.GetLowStockVariants(topN);
        }

        // ...
        public string ConvertToEmbedUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return "";

            // 1. If already correct, return it
            if (url.Contains("/embed/")) return url;

            string videoId = "";

            try
            {
                // Case A: Short Link (youtu.be/ID?si=...)
                if (url.Contains("youtu.be/"))
                {
                    var parts = url.Split(new[] { "youtu.be/" }, StringSplitOptions.None);
                    if (parts.Length > 1)
                    {
                        // Grab everything after slash, then split by '?' or '&' to remove params
                        videoId = parts[1].Split('?')[0].Split('&')[0];
                    }
                }
                // Case B: Standard Link (youtube.com/watch?v=ID)
                else if (url.Contains("v="))
                {
                    var parts = url.Split(new[] { "v=" }, StringSplitOptions.None);
                    if (parts.Length > 1)
                    {
                        videoId = parts[1].Split('&')[0].Split('?')[0];
                    }
                }
            }
            catch { return url; }

            if (!string.IsNullOrEmpty(videoId))
            {
                return $"https://www.youtube.com/embed/{videoId}";
            }

            return url;
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
                    nextPrimary.UpdatedAt = DateTime.Now;
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
                    v.UpdatedAt = DateTime.Now;

                    _productVideoDataAccess.Update(v);
                }
            }
        }
    }
}
