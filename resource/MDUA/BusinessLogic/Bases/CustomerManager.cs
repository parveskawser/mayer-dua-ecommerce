using System;
using System.Data.SqlClient;

using MDUA.Framework;
using MDUA.Framework.Exceptions;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.DataAccess;

namespace MDUA.BusinessLogic
{
	/// <summary>
    /// Business logic processing for Customer.
    /// </summary>    
	public partial class CustomerManager : BaseManager
	{
	
		#region Constructors
		public CustomerManager(ClientContext context) : base(context) { }
		public CustomerManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new customer.
        /// data manipulation for insertion of Customer
        /// </summary>
        /// <param name="customerObject"></param>
        /// <returns></returns>
        private bool Insert(Customer customerObject)
        {
            // new customer
            using (CustomerDataAccess data = new CustomerDataAccess(ClientContext))
            {
                // insert to customerObject
                Int32 _Id = data.Insert(customerObject);
                // if successful, process
                if (_Id > 0)
                {
                    customerObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of Customer Object.
        /// Data manipulation processing for: new, deleted, updated Customer
        /// </summary>
        /// <param name="customerObject"></param>
        /// <returns></returns>
        public bool UpdateBase(Customer customerObject)
        {
            // use of switch for different types of DML
            switch (customerObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(customerObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(customerObject.Id);
            }
            // update rows
            using (CustomerDataAccess data = new CustomerDataAccess(ClientContext))
            {
                return (data.Update(customerObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for Customer
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (CustomerDataAccess data = new CustomerDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve Customer data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>Customer Object</returns>
        public Customer Get(Int32 _Id)
        {
            using (CustomerDataAccess data = new CustomerDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a Customer .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public Customer Get(Int32 _Id, bool fillChild)
        {
            Customer customerObject;
            customerObject = Get(_Id);
            
            if (customerObject != null && fillChild)
            {
                // populate child data for a customerObject
                FillCustomerWithChilds(customerObject, fillChild);
            }

            return customerObject;
        }
		
		/// <summary>
        /// populates a Customer with its child entities
        /// </summary>
        /// <param name="customer"></param>
		/// <param name="fillChilds"></param>
        private void FillCustomerWithChilds(Customer customerObject, bool fillChilds)
        {
            // populate child data for a customerObject
            if (customerObject != null)
            {
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of Customer.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of Customer</returns>
        public CustomerList GetAll()
        {
            using (CustomerDataAccess data = new CustomerDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of Customer.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of Customer</returns>
        public CustomerList GetAll(bool fillChild)
        {
			CustomerList customerList = new CustomerList();
            using (CustomerDataAccess data = new CustomerDataAccess(ClientContext))
            {
                customerList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (Customer customerObject in customerList)
                {
					FillCustomerWithChilds(customerObject, fillChild);
				}
			}
			return customerList;
        }
		
		/// <summary>
        /// Retrieve list of Customer  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of Customer</returns>
        public CustomerList GetPaged(PagedRequest request)
        {
            using (CustomerDataAccess data = new CustomerDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of Customer  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of Customer</returns>
        public CustomerList GetByQuery(String query)
        {
            using (CustomerDataAccess data = new CustomerDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get Customer Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Customer
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (CustomerDataAccess data = new CustomerDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get Customer Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Customer
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (CustomerDataAccess data = new CustomerDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		
		#endregion
	}	
}