using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDUA.Facade.Interface
{
    public interface ICustomerFacade : ICommonFacade<Customer, CustomerList, CustomerBase>
    {
        // 1. Get List for Admin View (Needs Name, Phone, Email, Status)
        List<Customer> GetAllCustomersForAdmin();
        // 2. Get Customer Details (Required for the Details button)
        Customer GetCustomerDetails(int customerId);

        // 3. Get All Addresses for a specific customer (Required for Addresses button)
        List<Address> GetCustomerAddresses(int customerId);

        // 4. Get Orders (Required for Orders button)
        SalesOrderHeaderList GetCustomerOrders(int customerId, int companyId);

        Customer GetCustomerDetailsById(int id);

    }
}
