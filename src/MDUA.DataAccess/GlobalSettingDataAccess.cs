using MDUA.Entities;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace MDUA.DataAccess
{
    public partial class GlobalSettingDataAccess
    {
        public string GetValue(int companyId, string key)
        {
            string sql = "SELECT GContent FROM GlobalSetting WHERE CompanyId = @CompanyId AND GKey = @Key";

            // 1. Use GetSQLCommand (Manages Connection/Transaction automatically)
            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                // 2. Use the Framework's parameter helpers
                AddParameter(cmd, pInt32("CompanyId", companyId));
                AddParameter(cmd, pNVarChar("Key", 50, key));

                // 3. Use SelectScaler instead of ExecuteScalar
                object result = SelectScaler(cmd);

                return result != null && result != DBNull.Value ? result.ToString() : null;
            }
        }
        public void SaveValue(int companyId, string key, string value)
        {
            // UPSERT Query: Updates if exists, Inserts if new
            string sql = @"
                MERGE GlobalSetting AS target
                USING (SELECT @CompanyId AS CompanyId, @Key AS GKey) AS source
                ON (target.CompanyId = source.CompanyId AND target.GKey = source.GKey)
                WHEN MATCHED THEN
                    UPDATE SET GContent = @Value
                WHEN NOT MATCHED THEN
                    INSERT (Id, CompanyId, GKey, GContent)
                    VALUES ((SELECT ISNULL(MAX(Id),0)+1 FROM GlobalSetting), @CompanyId, @Key, @Value);";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));
                AddParameter(cmd, pNVarChar("Key", 50, key));
                AddParameter(cmd, pNVarChar("Value", value ?? ""));

                // 4. Use ExecuteCommand instead of ExecuteNonQuery
                ExecuteCommand(cmd);
            }
        }


    }
}