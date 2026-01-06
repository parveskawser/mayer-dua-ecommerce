using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework;
using MDUA.Framework.Exceptions;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace MDUA.DataAccess
{
    public partial class CustomerDataAccess
    {
        public Customer GetByPhone(string phone)
        {
            string SQLQuery = "SELECT TOP 1 * FROM Customer WHERE Phone = @Phone";
            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pNVarChar("Phone", 20, phone));
                return GetObject(cmd); // Uses your existing GetObject helper
            }
        }

        // ✅ FIXED: Use the same pattern as GetByPhone
        public Customer GetByEmail(string email)
        {
            string SQLQuery = "SELECT TOP 1 * FROM Customer WHERE Email = @Email";
            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pNVarChar("Email", 255, email)); // Adjust size as needed
                return GetObject(cmd); // Uses your existing GetObject helper
            }
        }



             //new
     // Assuming this method signature is required by your ICustomerDataAccess interface:
     public Customer GetById(int id)
     {
         // The existing public method 'Get' already implements the full logic
         // of calling the GetCustomerById stored procedure and mapping the result.
         return Get(id);
     }

        public CustomerList GetCustomersByCompanyId(int companyId)
        {
            // ✅ SQL Logic:
            // Select Customers ONLY if they exist in CompanyCustomer for this specific CompanyId
            string SQLQuery = @"
        SELECT c.* FROM Customer c
        INNER JOIN CompanyCustomer cc ON c.Id = cc.CustomerId
        WHERE cc.CompanyId = @CompanyId
        ORDER BY c.CreatedAt DESC";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));

                // ✅ FIX: Pass a limit argument (e.g., 10000 or int.MaxValue)
                // because your GetList method signature is GetList(SqlCommand cmd, long rows)
                return GetList(cmd, 10000);
            }
        }
    }
}