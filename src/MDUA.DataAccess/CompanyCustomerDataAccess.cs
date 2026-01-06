using System;
using System.Data;
using System.Data.SqlClient;
using MDUA.Framework.DataAccess; // Required for BaseDataAccess methods

namespace MDUA.DataAccess //
{
    public partial class CompanyCustomerDataAccess
    {
        public int EnsureLinkAndGetId(int companyId, int customerId)
        {
            // 1. Try to get existing ID
            string selectSql = "SELECT Id FROM CompanyCustomer WHERE CompanyId = @CompanyId AND CustomerId = @CustomerId";

            using (SqlCommand cmd = GetSQLCommand(selectSql))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));
                AddParameter(cmd, pInt32("CustomerId", customerId));

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }

            // 2. If not found, Insert and return new ID
            string insertSql = @"
            INSERT INTO CompanyCustomer (CompanyId, CustomerId) 
            VALUES (@CompanyId, @CustomerId);
            SELECT SCOPE_IDENTITY();";

            using (SqlCommand cmd = GetSQLCommand(insertSql))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));
                AddParameter(cmd, pInt32("CustomerId", customerId));

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                object newId = cmd.ExecuteScalar();
                return Convert.ToInt32(newId);
            }
        }

        public bool IsLinked(int companyId, int customerId)
        {
            string SQLQuery = "SELECT COUNT(1) FROM CompanyCustomer WHERE CompanyId = @CompanyId AND CustomerId = @CustomerId";
            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));
                AddParameter(cmd, pInt32("CustomerId", customerId));
                object result = SelectScaler(cmd);
                return (result != null && Convert.ToInt32(result) > 0);
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