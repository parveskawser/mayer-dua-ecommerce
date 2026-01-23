using MDUA.Entities;
using MDUA.Entities.List;

namespace MDUA.Facade
{
    public interface ICmsFacade
    {
        // Added companyId to ensure we fetch the correct tenant's page
        CmsPage GetPageForRender(string slug, int companyId);

        // Added companyId and userName for tracking
        long SavePage(CmsPage page, int companyId, string userName);

        // Added companyId for folder structure isolation
        void UploadPageAsset(int pageId, string fileType, string fileName, Stream fileStream, string webRootPath, int companyId);
        CmsPageList GetAllPages(int companyId);
        CmsPage GetPageById(int id, int companyId);
        void DeletePage(int id, int companyId);
        void DeleteAsset(int id);
    }
}