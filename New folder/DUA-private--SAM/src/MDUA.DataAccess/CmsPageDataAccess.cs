using System;
using System.Data;
using System.Data.SqlClient;
using MDUA.Entities;

namespace MDUA.DataAccess
{
    public partial class CmsPageDataAccess
    {
        // Custom Stored Procedure Constants
        private const string GET_BY_SLUG = "sp_CmsPage_GetBySlug";
        private const string CHECK_SLUG = "sp_CmsPage_CheckSlugExists";

        public CmsPage GetBySlug(string slug, int companyId)
        {
            CmsPage page = null;
            using (var cmd = GetSPCommand(GET_BY_SLUG))
            {
                AddParameter(cmd, pNVarChar("Slug", 200, slug));
                AddParameter(cmd, pInt32("CompanyId", companyId));

                using (var reader = GetDataReader(cmd))
                {
                    if (reader.Read())
                    {
                        page = MapCmsPage(reader);
                    }
                }
            }
            return page;
        }

        public bool CheckSlugExists(string slug, int companyId, int excludeId = 0)
        {
            using (var cmd = GetSPCommand(CHECK_SLUG))
            {
                AddParameter(cmd, pNVarChar("Slug", 200, slug));
                AddParameter(cmd, pInt32("CompanyId", companyId));
                AddParameter(cmd, pInt32("ExcludeId", excludeId));

                var result = SelectScaler(cmd);

                return result != null && Convert.ToBoolean(result);
            }
        }

        private CmsPage MapCmsPage(SqlDataReader reader)
        {
            return new CmsPage
            {
                Id = Convert.ToInt32(reader["Id"]),
                CompanyId = Convert.ToInt32(reader["CompanyId"]),
                Title = reader["Title"].ToString(),
                Slug = reader["Slug"].ToString(),
                ContentHtml = reader["ContentHtml"] != DBNull.Value ? reader["ContentHtml"].ToString() : null,
                SidebarContentHtml = reader["SidebarContentHtml"] != DBNull.Value ? reader["SidebarContentHtml"].ToString() : null,
                LayoutView = reader["LayoutView"].ToString(),
                MetaTitle = reader["MetaTitle"] != DBNull.Value ? reader["MetaTitle"].ToString() : null,
                MetaDescription = reader["MetaDescription"] != DBNull.Value ? reader["MetaDescription"].ToString() : null,
                CustomCss = reader["CustomCss"] != DBNull.Value ? reader["CustomCss"].ToString() : null,
                CustomJs = reader["CustomJs"] != DBNull.Value ? reader["CustomJs"].ToString() : null,
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                Version = Convert.ToInt32(reader["Version"]),
                CustomHeaderTags = reader["CustomHeaderTags"] != DBNull.Value ? reader["CustomHeaderTags"].ToString() : null
            };
        }
    }
}