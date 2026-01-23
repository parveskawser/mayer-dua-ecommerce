using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDUA.DataAccess.Interface
{
    public interface ICmsAssetDataAccess : ICommonDataAccess<CmsAsset, CmsAssetList, CmsAssetBase>
    {
        List<CmsAsset> GetActiveAssets(int pageId, string fileType);
        void DeactivateOldVersions(int pageId, string fileType, int companyId);

    }
}
