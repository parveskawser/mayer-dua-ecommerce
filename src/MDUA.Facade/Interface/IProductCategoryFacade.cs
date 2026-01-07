using MDUA.Entities;
using System.Collections.Generic;

namespace MDUA.Facade.Interface
{
    public interface IProductCategoryFacade
    {
        List<ProductCategory> GetAllCategoriesForManagement(int companyId);
        ProductCategory GetCategoryById(int id);
        int SaveProductCategory(ProductCategory category, int companyId);
        void UpdateCategoryStatus(int id, bool isActive, int companyId);
        List<ProductCategory> GetAvailableCategories(int companyId);

    }
}