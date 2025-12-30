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
    /// Business logic processing for CustomerPayment.
    /// </summary>    
	public partial class CustomerPaymentManager : BaseManager
	{
	
		#region Constructors
		public CustomerPaymentManager(ClientContext context) : base(context) { }
		public CustomerPaymentManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new customerPayment.
        /// data manipulation for insertion of CustomerPayment
        /// </summary>
        /// <param name="customerPaymentObject"></param>
        /// <returns></returns>
        private bool Insert(CustomerPayment customerPaymentObject)
        {
            // new customerPayment
            using (CustomerPaymentDataAccess data = new CustomerPaymentDataAccess(ClientContext))
            {
                // insert to customerPaymentObject
                Int32 _Id = data.Insert(customerPaymentObject);
                // if successful, process
                if (_Id > 0)
                {
                    customerPaymentObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of CustomerPayment Object.
        /// Data manipulation processing for: new, deleted, updated CustomerPayment
        /// </summary>
        /// <param name="customerPaymentObject"></param>
        /// <returns></returns>
        public bool UpdateBase(CustomerPayment customerPaymentObject)
        {
            // use of switch for different types of DML
            switch (customerPaymentObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(customerPaymentObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(customerPaymentObject.Id);
            }
            // update rows
            using (CustomerPaymentDataAccess data = new CustomerPaymentDataAccess(ClientContext))
            {
                return (data.Update(customerPaymentObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for CustomerPayment
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (CustomerPaymentDataAccess data = new CustomerPaymentDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve CustomerPayment data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>CustomerPayment Object</returns>
        public CustomerPayment Get(Int32 _Id)
        {
            using (CustomerPaymentDataAccess data = new CustomerPaymentDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a CustomerPayment .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public CustomerPayment Get(Int32 _Id, bool fillChild)
        {
            CustomerPayment customerPaymentObject;
            customerPaymentObject = Get(_Id);
            
            if (customerPaymentObject != null && fillChild)
            {
                // populate child data for a customerPaymentObject
                FillCustomerPaymentWithChilds(customerPaymentObject, fillChild);
            }

            return customerPaymentObject;
        }
		
		/// <summary>
        /// populates a CustomerPayment with its child entities
        /// </summary>
        /// <param name="customerPayment"></param>
		/// <param name="fillChilds"></param>
        private void FillCustomerPaymentWithChilds(CustomerPayment customerPaymentObject, bool fillChilds)
        {
            // populate child data for a customerPaymentObject
            if (customerPaymentObject != null)
            {
				// Retrieve CustomerIdObject as Customer type for the CustomerPayment using CustomerId
				using(CustomerManager customerManager = new CustomerManager(ClientContext))
				{
					customerPaymentObject.CustomerIdObject = customerManager.Get(customerPaymentObject.CustomerId, fillChilds);
				}
				// Retrieve InventoryTransactionIdObject as InventoryTransaction type for the CustomerPayment using InventoryTransactionId
				using(InventoryTransactionManager inventoryTransactionManager = new InventoryTransactionManager(ClientContext))
				{
					customerPaymentObject.InventoryTransactionIdObject = inventoryTransactionManager.Get(customerPaymentObject.InventoryTransactionId, fillChilds);
				}
				// Retrieve PaymentMethodIdObject as PaymentMethod type for the CustomerPayment using PaymentMethodId
				using(PaymentMethodManager paymentMethodManager = new PaymentMethodManager(ClientContext))
				{
					customerPaymentObject.PaymentMethodIdObject = paymentMethodManager.Get(customerPaymentObject.PaymentMethodId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of CustomerPayment.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of CustomerPayment</returns>
        public CustomerPaymentList GetAll()
        {
            using (CustomerPaymentDataAccess data = new CustomerPaymentDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of CustomerPayment.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of CustomerPayment</returns>
        public CustomerPaymentList GetAll(bool fillChild)
        {
			CustomerPaymentList customerPaymentList = new CustomerPaymentList();
            using (CustomerPaymentDataAccess data = new CustomerPaymentDataAccess(ClientContext))
            {
                customerPaymentList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (CustomerPayment customerPaymentObject in customerPaymentList)
                {
					FillCustomerPaymentWithChilds(customerPaymentObject, fillChild);
				}
			}
			return customerPaymentList;
        }
		
		/// <summary>
        /// Retrieve list of CustomerPayment  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of CustomerPayment</returns>
        public CustomerPaymentList GetPaged(PagedRequest request)
        {
            using (CustomerPaymentDataAccess data = new CustomerPaymentDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of CustomerPayment  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of CustomerPayment</returns>
        public CustomerPaymentList GetByQuery(String query)
        {
            using (CustomerPaymentDataAccess data = new CustomerPaymentDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get CustomerPayment Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of CustomerPayment
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (CustomerPaymentDataAccess data = new CustomerPaymentDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get CustomerPayment Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of CustomerPayment
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (CustomerPaymentDataAccess data = new CustomerPaymentDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of CustomerPayment By CustomerId        
		/// <param name="_CustomerId"></param>
        /// </summary>
        /// <returns>List of CustomerPayment</returns>
        public CustomerPaymentList GetByCustomerId(Int32 _CustomerId)
        {
            using (CustomerPaymentDataAccess data = new CustomerPaymentDataAccess(ClientContext))
            {
                return data.GetByCustomerId(_CustomerId);
            }
        }
		
		/// <summary>
        /// Retrieve list of CustomerPayment By PaymentMethodId        
		/// <param name="_PaymentMethodId"></param>
        /// </summary>
        /// <returns>List of CustomerPayment</returns>
        public CustomerPaymentList GetByPaymentMethodId(Int32 _PaymentMethodId)
        {
            using (CustomerPaymentDataAccess data = new CustomerPaymentDataAccess(ClientContext))
            {
                return data.GetByPaymentMethodId(_PaymentMethodId);
            }
        }
		
		/// <summary>
        /// Retrieve list of CustomerPayment By InventoryTransactionId        
		/// <param name="_InventoryTransactionId"></param>
        /// </summary>
        /// <returns>List of CustomerPayment</returns>
        public CustomerPaymentList GetByInventoryTransactionId(Nullable<Int32> _InventoryTransactionId)
        {
            using (CustomerPaymentDataAccess data = new CustomerPaymentDataAccess(ClientContext))
            {
                return data.GetByInventoryTransactionId(_InventoryTransactionId);
            }
        }
		
		
		#endregion
	}	
}