using MDUA.DataAccess;
using MDUA.Entities;
using MDUA.Entities.List;
using Microsoft.Extensions.Configuration; // Required for IConfiguration
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MDUA.Facade
{

    public class CmsFacade : ICmsFacade
    {
        private readonly CmsPageDataAccess _pageDa;
        private readonly CmsAssetDataAccess _assetDa;

        // Inject IConfiguration instead of ClientContext to avoid the compilation error
        // and use the standard BaseDataAccess constructor.
        public CmsFacade(IConfiguration configuration)
        {
            _pageDa = new CmsPageDataAccess(configuration);
            _assetDa = new CmsAssetDataAccess(configuration);
        }

        public CmsPage GetPageForRender(string slug, int companyId)
        {
            var page = _pageDa.GetBySlug(slug, companyId);

            if (page != null)
            {
                page.CssAssets = _assetDa.GetActiveAssets(page.Id, "CSS");
                page.JsAssets = _assetDa.GetActiveAssets(page.Id, "JS");
                page.ImageAssets = _assetDa.GetActiveAssets(page.Id, "IMG");
            }

            return page;
        }

        public long SavePage(CmsPage page, int companyId, string userName)
        {
            // 1. Slug Generation
            if (string.IsNullOrWhiteSpace(page.Slug))
                page.Slug = GenerateSlug(page.Title);
            else
                page.Slug = GenerateSlug(page.Slug);

            // 2. Check Duplicates
            bool exists = _pageDa.CheckSlugExists(page.Slug, companyId, page.Id);
            if (exists) throw new Exception($"The URL '{page.Slug}' is already taken.");
            // 1. Auto-Generate Meta Title if empty
            if (string.IsNullOrWhiteSpace(page.MetaTitle))
            {
                // Default: "Page Title"
                page.MetaTitle = page.Title;
            }

            // 2. Auto-Generate Meta Description if empty
            if (string.IsNullOrWhiteSpace(page.MetaDescription))
            {
                // Strip HTML tags to get raw text from the content
                string rawText = Regex.Replace(page.ContentHtml ?? "", "<.*?>", " ");

                // Take first 155 chars (standard SEO length)
                if (rawText.Length > 155)
                {
                    page.MetaDescription = rawText.Substring(0, 152).Trim() + "...";
                }
                else
                {
                    page.MetaDescription = rawText.Trim();
                }
            }
            // 3. Prepare Data
            page.CompanyId = companyId;

            // Ensure Meta/Custom fields are empty strings if null
            page.MetaTitle = page.MetaTitle ?? "";
            page.MetaDescription = page.MetaDescription ?? "";
            page.CustomCss = page.CustomCss ?? "";
            page.CustomJs = page.CustomJs ?? "";
            page.CustomHeaderTags = page.CustomHeaderTags ?? "";

            // 4. Handle PublishedAt (Date Fix)
            if (page.PublishedAt == null || page.PublishedAt == DateTime.MinValue)
            {
                // Only set to Now if it's completely missing (New Page)
                page.PublishedAt = DateTime.UtcNow;
            }
            // Note: If it's an Edit, the hidden field in the view will pass the OLD date back.
            // So we don't need to do anything else here.

            page.UpdatedAt = DateTime.UtcNow;
            page.UpdatedBy = userName;

            if (page.Id == 0)
            {
                page.CreatedBy = userName;
                page.CreatedAt = DateTime.UtcNow;
                page.Version = 1;
                return _pageDa.Insert(page);
            }
            else
            {
                // On Edit, use the hidden fields for CreatedBy/At
                if (string.IsNullOrEmpty(page.CreatedBy)) page.CreatedBy = userName;
                if (page.CreatedAt == DateTime.MinValue) page.CreatedAt = DateTime.UtcNow;

                page.Version++;
                _pageDa.Update(page);
                return page.Id;
            }
        }
        public void UploadPageAsset(int pageId, string fileType, string fileName, Stream fileStream, string webRootPath, int companyId)
        {
            // ✅ NEW LOGIC: If uploading an Image, delete ALL existing active images for this page first.
            if (fileType.ToUpper() == "IMG")
            {
                var existingImages = _assetDa.GetActiveAssets(pageId, "IMG");
                foreach (var img in existingImages)
                {
                    // Logic to Soft Delete (set Active=0) or Hard Delete
                    _assetDa.Delete(img.Id); // Uses your existing Delete stored procedure

                    // Optional: Delete physical file if you want to save space
                    // DeletePhysicalFile(webRootPath, img.FilePath); 
                }
            }


            // 1. Prepare Paths: uploads/cms/{CompanyId}/{css|js}/
            string relativeFolder = Path.Combine("uploads", "cms", companyId.ToString(), fileType.ToLower());
            string absoluteFolder = Path.Combine(webRootPath, relativeFolder);

            if (!Directory.Exists(absoluteFolder))
                Directory.CreateDirectory(absoluteFolder);

            // 2. Unique Filename
            string uniqueName = $"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.UtcNow.Ticks}{Path.GetExtension(fileName)}";
            string fullPath = Path.Combine(absoluteFolder, uniqueName);

            // 3. Save Physical File
            using (var fileStreamOutput = new FileStream(fullPath, FileMode.Create))
            {
                fileStream.CopyTo(fileStreamOutput);
            }

            // 4. Save to DB
            var asset = new CmsAsset
            {
                CompanyId = companyId,
                PageId = pageId,
                FileName = fileName,
                FilePath = "/" + relativeFolder.Replace("\\", "/") + "/" + uniqueName,
                FileType = fileType,
                IsActive = true,
                Version = 1,
                CreatedAt = DateTime.UtcNow
            };

            _assetDa.Insert(asset);
        }

        // New Helper for Delete Action
        public void DeleteAsset(int id)
        {
            _assetDa.Delete(id);
        }

        private string GenerateSlug(string phrase)
        {
            if (string.IsNullOrEmpty(phrase)) return "";
            string str = phrase.ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-");
            return str;
        }

        public CmsPageList GetAllPages(int companyId)
        {
            return _pageDa.GetByQuery($"CompanyId = {companyId}");
        }

        public CmsPage GetPageById(int id, int companyId)
        {
            var page = _pageDa.Get(id);
            if (page != null && page.CompanyId == companyId)
            {
                page.CssAssets = _assetDa.GetActiveAssets(page.Id, "CSS");
                page.JsAssets = _assetDa.GetActiveAssets(page.Id, "JS");
                page.ImageAssets = _assetDa.GetActiveAssets(page.Id, "IMG");
                return page;
            }
            return null;
        }

        public void DeletePage(int id, int companyId)
        {
            var page = _pageDa.Get(id);
            if (page != null && page.CompanyId == companyId)
            {
                _pageDa.Delete(id);
            }
        }
    }
}