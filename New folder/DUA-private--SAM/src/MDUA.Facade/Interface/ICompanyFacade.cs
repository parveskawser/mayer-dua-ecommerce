using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using Microsoft.AspNetCore.Http;

namespace MDUA.Facade.Interface
{
    public interface ICompanyFacade
    {
        int GetCompanyIdByDomain(string domain);

        Company Get(int _Id); //new

        CompanyList GetAll();
        long Update(Company company); // ✅ Add this
        void UpdateCompanyProfile(Company company, IFormFile logoFile, IFormFile faviconFile, string webRootPath, string footerDescription); HomepageConfig GetHomepageConfig(int companyId);
        void SaveHomepageConfig(int companyId, HomepageConfig config);
        // ✅ NEW: Draft & Generic Settings (Fixes your errors)
        HomepageConfig GetHomepageDraftConfig(int companyId);
        void SaveGlobalSetting(int companyId, string key, string value);
    }
}