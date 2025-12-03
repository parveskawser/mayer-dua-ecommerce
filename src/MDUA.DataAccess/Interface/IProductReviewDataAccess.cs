using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess.Interface
{
    public interface IProductReviewDataAccess : ICommonDataAccess<ProductReview, ProductReviewList, ProductReviewBase>
    {
        /// <summary>
        /// Gets all approved reviews for a given product.
        /// </summary>
        /// <param name="productId">The product ID.</param>
        /// <returns>List of ProductReview.</returns>
        ProductReviewList GetByProductId(int productId);
    }
}
