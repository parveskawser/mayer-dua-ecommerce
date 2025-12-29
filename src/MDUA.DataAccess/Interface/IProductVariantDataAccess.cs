using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess.Interface
{
    public interface IProductVariantDataAccess : ICommonDataAccess<ProductVariant, ProductVariantList, ProductVariantBase>
    {
        /// <summary>
        /// Gets all variants for a specific product.
        /// </summary>
        /// <param name="productId">The product ID.</param>
        /// <returns>List of ProductVariant.</returns>
        ProductVariantList GetByProductId(int productId);
        int Insert(ProductVariant variant);
        void InsertVariantAttributeValue(int variantId, int attributeValueId, int displayOrder);
        ProductVariantList GetProductVariantsByProductId(int productId);
        void UpdateVariantName(int variantId, string newName);
        ProductVariant GetWithStock(int id);
        List<VariantAttributeDto> GetVariantAttributesByProductId(int productId);

    }
}
