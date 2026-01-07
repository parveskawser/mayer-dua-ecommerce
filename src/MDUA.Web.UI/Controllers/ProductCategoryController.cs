using MDUA.Entities;
using MDUA.Facade.Interface; // Ensure this namespace matches where your Facade is located
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace MDUA.Web.UI.Controllers
{
    [Authorize]
    public class ProductCategoryController : BaseController
    {
        private readonly IProductCategoryFacade _categoryFacade;

        public ProductCategoryController(IProductCategoryFacade categoryFacade)
        {
            _categoryFacade = categoryFacade;
        }

        #region Category Management

        // GET: ProductCategory/Index
        [Route("product-category/index")]
        public IActionResult Index()
        {
            // 1. Permission Check
           // if (!HasPermission("Category.View")) return HandleAccessDenied();

            try
            {
                // 2. Fetch Categories
                // We call 'GetAllCategoriesForManagement' because it returns BOTH Active and Inactive 
                // private categories, preventing them from disappearing from the grid.
                List<ProductCategory> list = _categoryFacade.GetAllCategoriesForManagement(CurrentCompanyId);
        
                return View(list);
            }
            catch (Exception ex)
            {
                // Log the error if needed
                ViewBag.Error = "Error loading categories: " + ex.Message;
                return View(new List<ProductCategory>());
            }
        }

        // GET: ProductCategory/AddEdit/5
        [HttpGet]
        [Route("product-category/add-edit/{id?}")]
        public IActionResult AddEdit(int? id)
        {
            // Permission Check Logic
            if (id.HasValue && id.Value > 0)
            {
                // Editing existing
               // if (!HasPermission("Category.Edit")) return HandleAccessDenied();
            }
            else
            {
                // Adding new
              //  if (!HasPermission("Category.Add")) return HandleAccessDenied();
            }

            ProductCategory model = new ProductCategory();

            if (id.HasValue && id.Value > 0)
            {
                model = _categoryFacade.GetCategoryById(id.Value);
                if (model == null) return NotFound();
            }
            else
            {
                // Default settings for new item
                model.IsActive = true;
            }

            return PartialView("_AddEdit", model);
        }

        // POST: ProductCategory/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product-category/save")]
        public IActionResult Save(ProductCategory model)
        {
            // Permission Check Logic
            if (model.Id > 0)
            {
               // if (!HasPermission("Category.Edit")) return HandleAccessDenied();
            }
            else
            {
              //  if (!HasPermission("Category.Add")) return HandleAccessDenied();
            }

            try
            {
                // The Facade handles the logic:
                // 1. If ID=0 -> Insert Private
                // 2. If ID>0 and Owner=Me -> Update
                // 3. If ID>0 and Owner=Global -> Clone to Private -> Update
                _categoryFacade.SaveProductCategory(model, CurrentCompanyId);

                return Json(new { success = true, message = "Category saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: ProductCategory/UpdateStatus
        [HttpPost]
        [Route("product-category/update-status")]
        public IActionResult UpdateStatus(int id, bool isActive)
        {
            // Permission Check
            // Changing status is considered an Edit operation
           // if (!HasPermission("Category.Edit")) return HandleAccessDenied();

            try
            {
                // Facade Logic:
                // 1. If Private: Simple Update.
                // 2. If Global & Deactivating: Clone to Private -> Set Inactive.
                _categoryFacade.UpdateCategoryStatus(id, isActive, CurrentCompanyId);

                return Json(new { success = true, message = "Status updated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion
    }
}