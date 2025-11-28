using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using System.Collections.Generic;
using System.Linq;

namespace MDUA.Facade
{
    public class CustomerFacade : ICustomerFacade
    {
        // Inject all necessary Data Access classes
        private readonly ICustomerDataAccess _customerDataAccess;
        private readonly IAddressDataAccess _addressDataAccess;
        private readonly ISalesOrderHeaderDataAccess _salesOrderHeaderDataAccess;
        private readonly ICompanyCustomerDataAccess _companyCustomerDataAccess;

        public CustomerFacade(
            ICustomerDataAccess customerDataAccess,
            IAddressDataAccess addressDataAccess,
            ISalesOrderHeaderDataAccess salesOrderHeaderDataAccess,
            ICompanyCustomerDataAccess companyCustomerDataAccess)
        {
            _customerDataAccess = customerDataAccess;
            _addressDataAccess = addressDataAccess;
            _salesOrderHeaderDataAccess = salesOrderHeaderDataAccess;
            _companyCustomerDataAccess = companyCustomerDataAccess;
        }

        #region Common Implementation (Delegate to base DA)
        public long Delete(int id) => _customerDataAccess.Delete(id);
        public Customer Get(int id) => _customerDataAccess.Get(id);
        public CustomerList GetAll() => _customerDataAccess.GetAll();
        public CustomerList GetByQuery(string query) => _customerDataAccess.GetByQuery(query);
        public long Insert(CustomerBase obj) => _customerDataAccess.Insert(obj);
        public long Update(CustomerBase obj) => _customerDataAccess.Update(obj);
        #endregion

        #region Extended Implementation

        public List<Customer> GetAllCustomersForAdmin()
        {
            // Assuming GetAll() returns CustomerList which is List<Customer>
            return _customerDataAccess.GetAll().ToList();
        }

        public Customer GetCustomerDetails(int customerId)
        {
            return _customerDataAccess.Get(customerId);
        }

        public List<Address> GetCustomerAddresses(int customerId)
        {
            // Assuming you have a GetByCustomerId method exposed in IAddressDataAccess
            return _addressDataAccess.GetByCustomerId(customerId).ToList();
        }

        // Inside CustomerFacade.cs

        // In MDUA.Facade/CustomerFacade.cs

        public SalesOrderHeaderList GetCustomerOrders(int customerId, int companyId)
        {
            int companyCustomerId = _companyCustomerDataAccess.GetId(companyId, customerId);
            Console.WriteLine($"DEBUG: GetCustomerOrders -> companyId={companyId}, customerId={customerId}, companyCustomerId={companyCustomerId}");

            if (companyCustomerId <= 0)
            {
                Console.WriteLine("DEBUG: No companyCustomerId found, returning empty list.");

                return new SalesOrderHeaderList();
            }

            var list = _salesOrderHeaderDataAccess.GetOrdersByCustomerId(customerId);
            Console.WriteLine($"DEBUG: Orders returned from DA = {list?.Count ?? -1}");

            return list;
        }

        public Customer GetCustomerDetailsById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid Customer ID.", nameof(id));
            }

            // Reuse the existing public Get method
            return Get(id);
        }



        #endregion
    }
}