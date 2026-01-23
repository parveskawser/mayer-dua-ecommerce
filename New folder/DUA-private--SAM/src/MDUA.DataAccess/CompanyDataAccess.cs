using System;
using System.Data.SqlClient;
using MDUA.Entities;

namespace MDUA.DataAccess
{
    public partial class CompanyDataAccess
    {
        // ✅ 1. SAFE GET METHOD
        public Company GetCompanySafe(int id)
        {
            using (SqlCommand cmd = GetSPCommand("GetCompanyById"))
            {
                AddParameter(cmd, pInt32("Id", id));

                SqlDataReader reader;
                SelectRecords(cmd, out reader);

                using (reader)
                {
                    if (reader.Read())
                    {
                        Company obj = new Company();
                        FillObjectSafe(obj, reader); // Calls the method below
                        return obj;
                    }
                    return null;
                }
            }
        }

        // ✅ 2. THE MISSING METHOD (Must be inside the class)
        // MDUA.DataAccess/CompanyDataAccess.partial.cs

        private void FillObjectSafe(Company companyObject, SqlDataReader reader)
        {
            // 1. Existing Mappings
            if (HasColumn(reader, "Id"))
                companyObject.Id = Convert.ToInt32(reader["Id"]);

            if (HasColumn(reader, "CompanyName"))
                companyObject.CompanyName = reader["CompanyName"].ToString();

            if (HasColumn(reader, "LogoImg") && reader["LogoImg"] != DBNull.Value)
                companyObject.LogoImg = reader["LogoImg"].ToString();

            // ✅ 2. ADD THESE NEW MAPPINGS
            if (HasColumn(reader, "Address") && reader["Address"] != DBNull.Value)
                companyObject.Address = reader["Address"].ToString();

            if (HasColumn(reader, "Email") && reader["Email"] != DBNull.Value)
                companyObject.Email = reader["Email"].ToString();

            if (HasColumn(reader, "Phone") && reader["Phone"] != DBNull.Value)
                companyObject.Phone = reader["Phone"].ToString();

            if (HasColumn(reader, "Website") && reader["Website"] != DBNull.Value)
                companyObject.Website = reader["Website"].ToString();

            if (HasColumn(reader, "CompanyCode") && reader["CompanyCode"] != DBNull.Value)
                companyObject.CompanyCode = reader["CompanyCode"].ToString();

            // Don't forget IsActive if needed
            if (HasColumn(reader, "IsActive") && reader["IsActive"] != DBNull.Value)
                companyObject.IsActive = Convert.ToBoolean(reader["IsActive"]);
        }
        // ✅ 3. HELPER METHOD
        private bool HasColumn(SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        public int GetIdByWebsite(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
                return 0;

            string sql = @"
        SELECT TOP 1 Id 
        FROM Company 
        WHERE REPLACE(REPLACE(REPLACE(Website, 'https://', ''), 'http://', ''), 'www.', '') = @Domain
        AND IsActive = 1";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                AddParameter(cmd, pNVarChar("Domain", 255, domain));

                object result = SelectScaler(cmd);

                return result != null && result != DBNull.Value
                    ? Convert.ToInt32(result)
                    : 0;
            }
        }

    } // End of Class
} // End of Namespace