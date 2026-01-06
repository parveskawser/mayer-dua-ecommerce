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
	public partial class ProductCategoryDataAccess
	{
        // MDUA.DataAccess/ProductCategoryDataAccess.cs

        public List<ProductCategory> GetByCompany(int companyId)
        {
            // ✅ SOFT FILTER: Matches specific Company ID OR NULL (Shared Data)
            string SQL = @"
        SELECT * FROM ProductCategory 
        WHERE (CompanyId = @CompanyId OR CompanyId IS NULL) 
        ORDER BY Name";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }
    }	
}
