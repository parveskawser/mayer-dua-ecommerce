using MDUA.Entities;
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
                return GetObject(cmd);
            }
        }
    }
}