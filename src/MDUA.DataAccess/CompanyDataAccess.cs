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
        private void FillObjectSafe(Company companyObject, SqlDataReader reader)
        {
            // Safely map columns by Name instead of Index
            if (HasColumn(reader, "Id"))
                companyObject.Id = Convert.ToInt32(reader["Id"]);

            if (HasColumn(reader, "CompanyName"))
                companyObject.CompanyName = reader["CompanyName"].ToString();

            if (HasColumn(reader, "LogoImg") && reader["LogoImg"] != DBNull.Value)
                companyObject.LogoImg = reader["LogoImg"].ToString();

            // You can add other fields here if needed for the UI
            // e.g. Address, Phone, etc.
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

    } // End of Class
} // End of Namespace