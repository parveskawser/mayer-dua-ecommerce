using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
 
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess.Interface
{
    public interface ICompanyCustomerDataAccess
    {
        long Insert(CompanyCustomerBase companyCustomerObject);
        // ... 

   
        int EnsureLinkAndGetId(int companyId, int customerId);

        bool IsLinked(int companyId, int customerId);
        int GetId(int companyId, int customerId);
    }
}