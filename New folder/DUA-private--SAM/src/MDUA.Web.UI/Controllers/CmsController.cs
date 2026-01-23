using MDUA.Entities;
using MDUA.Facade;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting; // For IWebHostEnvironment
using Microsoft.AspNetCore.Http; // For IFormFile
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq; // Added for Linq in Render method
using System; // Added for Exception

namespace MDUA.Web.UI.Controllers
{
    public class CmsController : BaseController
    {
        private readonly ICmsFacade _cmsFacade;
        private readonly IWebHostEnvironment _env;

        public CmsController(ICmsFacade cmsFacade, IWebHostEnvironment env)
        {
            _cmsFacade = cmsFacade;
            _env = env;
        }

        // --- 1. ALL PAGES ---
        // GET: /cms/all-pages
        [Route("cms/all-pages")]
        public IActionResult Index()
        {
            var pages = _cmsFacade.GetAllPages(CurrentCompanyId);
            return View(pages);
        }

        // --- 2. EDIT PAGE ---
        // GET: /cms/edit-page/{id}
        [Route("cms/edit-page/{id}")]
        public IActionResult Edit(int id)
        {
            var page = _cmsFacade.GetPageById(id, CurrentCompanyId);
            if (page == null) return NotFound();
            return View("Create", page); // Reuse Create view
        }

        [HttpPost]
        [Route("cms/edit-page/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CmsPage model)
        {
            // 1. CLEAR VALIDATION
            ModelState.Remove("CompanyId");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("UpdatedBy");
            ModelState.Remove("UpdatedAt");
            ModelState.Remove("Version");
            ModelState.Remove("PublishedAt");
            ModelState.Remove("CustomJs");
            ModelState.Remove("CustomCss");
            ModelState.Remove("MetaTitle");
            ModelState.Remove("MetaDescription");
            ModelState.Remove("SidebarContentHtml");
            ModelState.Remove("CustomProperties");
            ModelState.Remove("CustomHeaderTags");
            if (string.IsNullOrEmpty(model.Slug)) ModelState.Remove("Slug");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Validation Failed", errors = errors });
            }

            try
            {
                model.Id = id;
                int safeCompanyId = CurrentCompanyId > 0 ? CurrentCompanyId : 1;
                string safeUserName = !string.IsNullOrEmpty(CurrentUserName) ? CurrentUserName : "Admin";

                // Save and capture ID
                long savedId = _cmsFacade.SavePage(model, safeCompanyId, safeUserName);

                // ✅ UPDATED: Return ID so frontend can upload files
                return Json(new { success = true, id = savedId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server Error: " + ex.Message });
            }
        }

        // --- 3. CREATE PAGE ---
        // GET: /cms/add-page
        [Route("cms/add-page")]
        public IActionResult Create()
        {
            var model = new CmsPage();
            // Default layout selection
            model.LayoutView = "_CmsBlank";
            return View(model);
        }
        [HttpPost]
        [Route("cms/add-page")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CmsPage model)
        {
            // 1. IGNORE FIELDS
            ModelState.Remove("CompanyId");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("UpdatedBy");
            ModelState.Remove("UpdatedAt");
            ModelState.Remove("Version");
            ModelState.Remove("PublishedAt");
            ModelState.Remove("CustomJs");
            ModelState.Remove("CustomCss");
            ModelState.Remove("MetaTitle");
            ModelState.Remove("MetaDescription");
            ModelState.Remove("SidebarContentHtml");
            ModelState.Remove("CustomProperties");
            ModelState.Remove("CustomHeaderTags");

            if (string.IsNullOrEmpty(model.Slug)) ModelState.Remove("Slug");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Validation Failed", errors = errors });
            }

            try
            {
                model.CustomCss = model.CustomCss ?? "";
                model.CustomJs = model.CustomJs ?? "";
                model.MetaTitle = model.MetaTitle ?? "";
                model.MetaDescription = model.MetaDescription ?? "";
                model.ContentHtml = model.ContentHtml ?? "";

                int safeCompanyId = CurrentCompanyId > 0 ? CurrentCompanyId : 1;
                string safeUserName = !string.IsNullOrEmpty(CurrentUserName) ? CurrentUserName : "Admin";

                // Save and capture ID
                long savedId = _cmsFacade.SavePage(model, safeCompanyId, safeUserName);

                // ✅ UPDATED: Return ID so frontend can upload files
                return Json(new { success = true, id = savedId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server Error: " + ex.Message });
            }
        }

        // --- 4. OTHER ACTIONS (Keep standard routes for JS calls) ---

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _cmsFacade.DeletePage(id, CurrentCompanyId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UploadAsset(int pageId, string fileType, List<IFormFile> files)
        {
            if (files != null && files.Count > 0)
            {
                try
                {
                    foreach (var file in files)
                    {
                        // ✅ Allow 'IMG' type for OG Images
                        string type = fileType.ToUpper();
                        if (type != "CSS" && type != "JS" && type != "IMG")
                            return Json(new { success = false, message = "Invalid file type." });

                        _cmsFacade.UploadPageAsset(
                            pageId,
                            type,
                            file.FileName,
                            file.OpenReadStream(),
                            _env.WebRootPath,
                            CurrentCompanyId
                        );
                    }
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return Json(new { success = false, message = "No files received" });
        }

        // --- 5. RENDER PUBLIC PAGE ---
        [Route("page/{slug}")]
        [AllowAnonymous]
        public IActionResult Render(string slug)
        {
            // 1. Try standard fetch
            var page = _cmsFacade.GetPageForRender(slug, CurrentCompanyId);

            // 2. IF NULL: It might be a Draft that the database hid. Let's find it manually.
            if (page == null)
            {
                // Get list of ALL pages (Drafts & Active)
                var allPages = _cmsFacade.GetAllPages(CurrentCompanyId);
                // Find match by slug
                var match = allPages.FirstOrDefault(p => p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                {
                    // Found the draft! Get full details by ID
                    page = _cmsFacade.GetPageById(match.Id, CurrentCompanyId);
                }
            }

            // 3. If still null, it really doesn't exist.
            if (page == null) return NotFound();

            // 4. CHECK PERMISSIONS (Draft Logic)
            if (!page.IsActive)
            {
                // Only allow Admin to see drafts
                if (!User.Identity.IsAuthenticated) return NotFound();

                ViewBag.IsDraftPreview = true;
            }

            // 5. Setup Layout
            string layoutName = !string.IsNullOrEmpty(page.LayoutView) ? page.LayoutView : "_CmsBlank";

            // ✅ FIX: Ensure layout is found in Shared/Layout subfolder
            ViewData["CmsLayout"] = $"~/Views/Shared/Layout/{layoutName}.cshtml";

            ViewBag.CssAssets = page.CssAssets;
            ViewBag.JsAssets = page.JsAssets;
            ViewBag.ImageAssets = page.ImageAssets;
            return View("~/Views/Cms/DynamicPage.cshtml", page);
        }

        [HttpPost]
        [Route("cms/delete-asset")] // Explicit route ensures the AJAX URL matches
        public IActionResult DeleteAsset(int id)
        {
            try
            {
                _cmsFacade.DeleteAsset(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}