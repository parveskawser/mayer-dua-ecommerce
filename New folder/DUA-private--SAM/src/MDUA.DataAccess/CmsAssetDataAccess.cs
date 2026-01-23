using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MDUA.Entities;

namespace MDUA.DataAccess
{
    public partial class CmsAssetDataAccess
    {
        private const string GET_ACTIVE_ASSETS = "sp_CmsAsset_GetActiveByPageId";
        private const string DEACTIVATE_OLD = "sp_CmsAsset_DeactivateOldVersions";

        public List<CmsAsset> GetActiveAssets(int pageId, string fileType)
        {
            var list = new List<CmsAsset>();
            using (var cmd = GetSPCommand(GET_ACTIVE_ASSETS))
            {
                AddParameter(cmd, pInt32("PageId", pageId));
                AddParameter(cmd, pNVarChar("FileType", 20, fileType));

                using (var reader = GetDataReader(cmd))
                {
                    while (reader.Read())
                    {
                        list.Add(MapCmsAsset(reader));
                    }
                }
            }
            return list;
        }

        public void DeactivateOldVersions(int pageId, string fileType, int companyId)
        {
            using (var cmd = GetSPCommand(DEACTIVATE_OLD))
            {
                AddParameter(cmd, pInt32("PageId", pageId));
                AddParameter(cmd, pNVarChar("FileType", 20, fileType));
                AddParameter(cmd, pInt32("CompanyId", companyId));

                ExecuteCommand(cmd);
            }
        }

        private CmsAsset MapCmsAsset(SqlDataReader reader)
        {
            return new CmsAsset
            {
                Id = Convert.ToInt32(reader["Id"]),
                CompanyId = Convert.ToInt32(reader["CompanyId"]),
                PageId = reader["PageId"] != DBNull.Value ? (int?)reader["PageId"] : null,
                FileName = reader["FileName"].ToString(),
                FilePath = reader["FilePath"].ToString(),
                FileType = reader["FileType"].ToString(),
                Version = Convert.ToInt32(reader["Version"]),
                IsActive = Convert.ToBoolean(reader["IsActive"]),

                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
            };
        }
    }
}