using System;
using System.Collections.Generic;
using MDUA.DataAccess.Interface; // Ensure you use the Interface namespace
using MDUA.Entities;
using MDUA.Facade.Interface; // Ensure this exists

namespace MDUA.Facade
{
    // 1. Ensure it implements the Interface
    public class ProductCategoryFacade : IProductCategoryFacade
    {
        // 2. Change field type to Interface
        private readonly IProductCategoryDataAccess _categoryDA;

        // 3. Change Constructor parameter to Interface
        public ProductCategoryFacade(IProductCategoryDataAccess categoryDA)
        {
            _categoryDA = categoryDA;
        }

        // ... Keep your existing methods ...

        public List<ProductCategory> GetAllCategoriesForManagement(int companyId)
        {
            return _categoryDA.GetAllCategoriesForManagement(companyId);        }

        public ProductCategory GetCategoryById(int id)
        {
            return _categoryDA.Get(id);
        }

        public int SaveProductCategory(ProductCategory category, int companyId)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new Exception("Category Name is required.");

            if (category.Id <= 0)
            {
                category.CompanyId = companyId;
                category.IsActive = true; 

                var existing = _categoryDA.GetPrivateCategoryByName(category.Name, companyId);
                if (existing != null)
                    throw new Exception($"You already have a category named '{category.Name}'.");

                _categoryDA.Insert(category);
                return category.Id;
            }
            else
            {
                var existing = _categoryDA.Get(category.Id);
                if (existing == null) throw new Exception("Category not found.");

                if (existing.CompanyId == null)
                {
                    int newPrivateId = _categoryDA.CloneGlobalToPrivate(existing.Id, companyId);
                    if (newPrivateId <= 0) throw new Exception("Failed to clone global category.");

                    category.Id = newPrivateId;
                    category.CompanyId = companyId;
                }
                else if (existing.CompanyId == companyId)
                {
                    category.CompanyId = companyId; 
                }
                else
                {
                    throw new Exception("Access Denied: You do not own this category.");
                }

                _categoryDA.Update(category);
                return category.Id;
            }
        }

        public void UpdateCategoryStatus(int id, bool isActive, int companyId)
        {
            var existing = _categoryDA.Get(id);
            if (existing == null) throw new Exception("Category not found.");

            if (existing.CompanyId == companyId)
            {
                _categoryDA.UpdateStatus(id, isActive);
            }
            else if (existing.CompanyId == null)
            {
                if (isActive) return; 

                int newPrivateId = _categoryDA.CloneGlobalToPrivate(existing.Id, companyId);
                if (newPrivateId > 0)
                {
                    _categoryDA.UpdateStatus(newPrivateId, false);
                }
                else
                {
                    throw new Exception("Failed to clone global category for deactivation.");
                }
            }
            else
            {
                throw new Exception("Access Denied.");
            }
        }
        
        

        // USE THIS for the "Add Product" Dropdown
        public List<ProductCategory> GetAvailableCategories(int companyId)
        {
            // Calls the method that shows ONLY Active items
            return _categoryDA.GetAvailableCategories(companyId);
        }
    }
}