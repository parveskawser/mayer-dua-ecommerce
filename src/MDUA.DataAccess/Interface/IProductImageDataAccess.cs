using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess.Interface
{
    public interface IProductImageDataAccess : ICommonDataAccess<ProductImage, ProductImageList, ProductImageBase>
    {
        /// <summary>
        /// Gets all images for a given product.
        /// </summary>
        /// <param name="productId">The product ID.</param>
        /// <returns>List of ProductImage.</returns>
        ProductImageList GetByProductId(int productId);
    }
}
