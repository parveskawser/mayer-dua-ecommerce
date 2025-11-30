using System;
using System.Data;
using System.Data.SqlClient;

using MDUA.Framework;
using MDUA.Framework.Exceptions;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess
{
	public partial class CompanyCustomerDataAccess
	{
        public bool IsLinked(int companyId, int customerId)
        {
            string SQLQuery = "SELECT COUNT(1) FROM CompanyCustomer WHERE CompanyId = @CompanyId AND CustomerId = @CustomerId";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));
                AddParameter(cmd, pInt32("CustomerId", customerId));

                SqlDataReader reader;
                SelectRecords(cmd, out reader);

                int count = 0;
                using (reader)
                {
                    if (reader.Read() && !reader.IsDBNull(0))
                    {
                        count = reader.GetInt32(0);
                    }
                    reader.Close();
                }
                return count > 0;
            }
        }

        public int GetId(int companyId, int customerId)
        {
            string query = "SELECT Id FROM CompanyCustomer WHERE CompanyId = @CompanyId AND CustomerId = @CustomerId";
            using (SqlCommand cmd = GetSQLCommand(query))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));
                AddParameter(cmd, pInt32("CustomerId", customerId));
                object result = SelectScaler(cmd);
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

    }
}
