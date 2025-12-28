using MDUA.Entities;
using System.Collections.Generic; // Required for List<>
using System.Data;                 // Required for ConnectionState
using System.Data.SqlClient;

namespace MDUA.DataAccess
{
    public partial class PostalCodesDataAccess
    {
        public PostalCodes GetPostalCodeDetails(string postCode)
        {
            string sql = @"SELECT TOP 1 * FROM PostalCodes WHERE PostCode = @PostCode";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                AddParameter(cmd, pNVarChar("PostCode", 10, postCode));
                // GetObject likely handles opening the connection internally
                return GetObject(cmd);
            }
        }

        // 1. Get All Divisions
        public List<string> GetDivisions()
        {
            var list = new List<string>();
            string sql = "SELECT DISTINCT DivisionEn FROM PostalCodes ORDER BY DivisionEn";

            using (var cmd = GetSQLCommand(sql))
            {
                // ✅ FIX: Explicitly open the connection before reading
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Safe check for DBNull if necessary, though distinct usually implies values
                        if (reader["DivisionEn"] != DBNull.Value)
                            list.Add(reader["DivisionEn"].ToString());
                    }
                }
            }
            return list;
        }

        // 2. Get Districts by Division
        public List<string> GetDistricts(string division)
        {
            var list = new List<string>();

            // 1. Build SQL dynamically based on whether 'division' is provided
            string sql;
            if (string.IsNullOrWhiteSpace(division))
            {
                // NO Division provided -> Return ALL unique districts
                sql = "SELECT DISTINCT DistrictEn FROM PostalCodes ORDER BY DistrictEn";
            }
            else
            {
                // Division provided -> Filter by that division
                sql = "SELECT DISTINCT DistrictEn FROM PostalCodes WHERE DivisionEn = @Div ORDER BY DistrictEn";
            }

            using (var cmd = GetSQLCommand(sql))
            {
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                // 2. Only add the parameter if we are using the WHERE clause
                if (!string.IsNullOrWhiteSpace(division))
                {
                    AddParameter(cmd, pNVarChar("Div", 100, division));
                }

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["DistrictEn"] != DBNull.Value)
                        {
                            list.Add(reader["DistrictEn"].ToString());
                        }
                    }
                }
            }
            return list;
        }
        // 3. Get Thanas by District
        public List<string> GetThanas(string district)
        {
            var list = new List<string>();
            string sql = "SELECT DISTINCT ThanaEn FROM PostalCodes WHERE DistrictEn = @Dist ORDER BY ThanaEn";

            using (var cmd = GetSQLCommand(sql))
            {
                // ✅ FIX: Open Connection
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                AddParameter(cmd, pNVarChar("Dist", 100, district));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["ThanaEn"] != DBNull.Value)
                            list.Add(reader["ThanaEn"].ToString());
                    }
                }
            }
            return list;
        }

        // 4. Get SubOffices (and their Codes) by Thana
        public List<dynamic> GetSubOffices(string thana)
        {
            var list = new List<dynamic>();
            string sql = "SELECT SubOfficeEn, PostCode FROM PostalCodes WHERE ThanaEn = @Thana ORDER BY SubOfficeEn";

            using (var cmd = GetSQLCommand(sql))
            {
                // ✅ FIX: Open Connection
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                AddParameter(cmd, pNVarChar("Thana", 100, thana));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new
                        {
                            Name = reader["SubOfficeEn"].ToString(),
                            Code = reader["PostCode"].ToString()
                        });
                    }
                }
            }
            return list;
        }
    }
}