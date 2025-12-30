using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using Microsoft.AspNetCore.Http;

namespace MDUA.Facade.Interface
{
    public interface ICompanyFacade
    {
        Company Get(int _Id); //new
        long Update(Company company); // ✅ Add this
        void UpdateCompanyProfile(Company company, IFormFile logoFile, IFormFile faviconFile, string webRootPath);
    }
}