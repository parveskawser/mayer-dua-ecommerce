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
    public interface ICmsPageDataAccess : ICommonDataAccess<CmsPage, CmsPageList, CmsPageBase>
    {
        CmsPage GetBySlug(string slug, int companyId);
        bool CheckSlugExists(string slug, int companyId, int excludeId = 0);


    }
}
