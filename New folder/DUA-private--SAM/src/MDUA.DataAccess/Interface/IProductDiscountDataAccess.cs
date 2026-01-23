using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess.Interface
{
    public interface IProductDiscountDataAccess : ICommonDataAccess<ProductDiscount, ProductDiscountList, ProductDiscountBase>
    {
        /// <summary>
        /// Gets the currently active discount for a given product.
        /// </summary>
        /// <param name="productId">The product ID.</param>
        /// <returns>Active ProductDiscount or null.</returns>
        ProductDiscount GetActiveDiscount(int productId);
        ProductDiscountList GetByProductId(int productId);
    }
}
