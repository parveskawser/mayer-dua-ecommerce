﻿using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text.Json; // ✅ Adds JsonSerializer support

namespace MDUA.Facade
{
    public class CompanyFacade : ICompanyFacade
    {
        private readonly ICompanyDataAccess _companyDataAccess;
        private readonly IGlobalSettingDataAccess _globalSettingDataAccess;

        public CompanyFacade(ICompanyDataAccess companyDataAccess, IGlobalSettingDataAccess globalSettingDataAccess)
        {
            _companyDataAccess = companyDataAccess;
            _globalSettingDataAccess = globalSettingDataAccess;
        }

        #region Common Implementation
        public Company Get(int _Id) => _companyDataAccess.Get(_Id);
        public long Update(Company company) => _companyDataAccess.Update(company);
        #endregion

        public CompanyList GetAll()
        {
            return _companyDataAccess.GetAll();
        }

        #region Extended Implementation

        public void UpdateCompanyProfile(Company company, IFormFile logoFile, IFormFile faviconFile, string webRootPath, string footerDescription)
        {
            // 1. Fetch existing company
            var existingCompany = _companyDataAccess.Get(company.Id);

            if (existingCompany != null)
            {
                // ✅ NEW: Update these fields from the incoming object
                existingCompany.CompanyName = company.CompanyName;
                existingCompany.Address = company.Address;
                existingCompany.Email = company.Email;
                existingCompany.Phone = company.Phone;

                // Update audit fields
                existingCompany.UpdatedBy = "System";
                existingCompany.UpdatedAt = DateTime.UtcNow;
            }

            // [KEEPING YOUR EXISTING LOGIC BELOW UNTOUCHED]

            // Define Company-Specific Folder Path
            string relativeFolder = $"/images/company/{company.Id}";
            string physicalFolder = Path.Combine(webRootPath, "images", "company", company.Id.ToString());

            if (!Directory.Exists(physicalFolder))
                Directory.CreateDirectory(physicalFolder);

            // LOGO UPLOAD
            if (logoFile != null && logoFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingCompany.LogoImg)) // Use existingCompany here to check old path
                {
                    DeleteOldFile(webRootPath, existingCompany.LogoImg);
                }

                string fileName = $"logo_{DateTime.UtcNow.Ticks}{Path.GetExtension(logoFile.FileName)}";
                string fullPath = Path.Combine(physicalFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    logoFile.CopyTo(stream);
                }

                existingCompany.LogoImg = $"{relativeFolder}/{fileName}";
            }

            // FAVICON UPLOAD
            if (faviconFile != null && faviconFile.Length > 0)
            {
                string oldFaviconUrl = _globalSettingDataAccess.GetSetting(company.Id, "FaviconUrl");
                if (!string.IsNullOrEmpty(oldFaviconUrl))
                {
                    DeleteOldFile(webRootPath, oldFaviconUrl);
                }

                string fileName = $"favicon_{DateTime.UtcNow.Ticks}{Path.GetExtension(faviconFile.FileName)}";
                string fullPath = Path.Combine(physicalFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    faviconFile.CopyTo(stream);
                }

                string dbPath = $"{relativeFolder}/{fileName}";
                _globalSettingDataAccess.SaveSetting(company.Id, "FaviconUrl", dbPath);
            }

            // ✅ SAVE to Database
            _companyDataAccess.Update(existingCompany);

            // ✅ SAVE Footer Description to Global Settings
            if (footerDescription != null)
            {
                // Note: Using "SaveValue" or "SaveSetting" depending on your interface name
                _globalSettingDataAccess.SaveValue(company.Id, "Footer_Description", footerDescription);
            }
        }
        // --- Helper Method to Delete Files ---
        private void DeleteOldFile(string webRootPath, string relativeUrl)
        {
            try
            {
                // Clean up URL to get physical path
                // Remove leading slash if present
                string cleanPath = relativeUrl.TrimStart('/', '\\');
                string physicalPath = Path.Combine(webRootPath, cleanPath);

                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                }
            }
            catch
            {
                // Suppress errors if file doesn't exist or is locked
                // We don't want to crash the profile update just because a cleanup failed
            }
        }

        // Ensure you have: using System.Text.Json;

        public HomepageConfig GetHomepageConfig(int companyId)
        {
            // 1. Try to fetch from DB
            // Assuming GetValue returns the string content from GlobalSetting
            string json = _globalSettingDataAccess.GetValue(companyId, "Homepage_Layout");

            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    return JsonSerializer.Deserialize<HomepageConfig>(json);
                }
                catch
                {
                    // If JSON is corrupt, ignore it
                }
            }

            // 2. Fallback: Return Default Layout (The "Clay" you built earlier)
            var defaultLayout = new HomepageConfig();

            // Default Hero
            defaultLayout.Sections.Add(new HomepageSection
            {
                Type = "Hero",
                Settings = new Dictionary<string, string>
        {
            { "Title", "Big Sale Is On!" },
            { "Subtitle", "Explore our latest arrivals." },
            { "BtnText", "Shop Now" }
        }
            });

            // Default Product Grid
            defaultLayout.Sections.Add(new HomepageSection
            {
                Type = "ProductGrid",
                Settings = new Dictionary<string, string>
        {
            { "Title", "New Arrivals" },
            { "Count", "8" }
        }
            });

            return defaultLayout;
        }

        public void SaveHomepageConfig(int companyId, HomepageConfig config)
        {
            string json = JsonSerializer.Serialize(config);
            // You likely need an InsertOrUpdate method in your DataAccess
            _globalSettingDataAccess.SaveValue(companyId, "Homepage_Layout", json);
        }

        // Add inside CompanyFacade class

        // ✅ Fix 1: Implement GetHomepageDraftConfig
        public HomepageConfig GetHomepageDraftConfig(int companyId)
        {
            // 1. Try fetch from "Homepage_Draft" key
            string json = _globalSettingDataAccess.GetValue(companyId, "Homepage_Draft");

            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    return JsonSerializer.Deserialize<HomepageConfig>(json);
                }
                catch
                {
                    // If draft is corrupt, fall back to LIVE config
                }
            }

            // 2. Fallback: If no draft exists, return the LIVE config
            return GetHomepageConfig(companyId);
        }

        // ✅ Fix 2: Implement SaveGlobalSetting
        public void SaveGlobalSetting(int companyId, string key, string value)
        {
            // Simple wrapper around DataAccess to allow Controller to save "Homepage_Draft" or other keys
            _globalSettingDataAccess.SaveValue(companyId, key, value);
        }


        public int GetCompanyIdByDomain(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
                return 1; 

            // Clean domain name
            domain = domain.Replace("https://", "")
                           .Replace("http://", "")
                           .Replace("www.", "")
                           .TrimEnd('/');

            return _companyDataAccess.GetIdByWebsite(domain);
        }


        #endregion
    }
}