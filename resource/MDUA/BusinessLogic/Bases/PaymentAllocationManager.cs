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
    /// Business logic processing for PaymentAllocation.
    /// </summary>    
	public partial class PaymentAllocationManager : BaseManager
	{
	
		#region Constructors
		public PaymentAllocationManager(ClientContext context) : base(context) { }
		public PaymentAllocationManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new paymentAllocation.
        /// data manipulation for insertion of PaymentAllocation
        /// </summary>
        /// <param name="paymentAllocationObject"></param>
        /// <returns></returns>
        private bool Insert(PaymentAllocation paymentAllocationObject)
        {
            // new paymentAllocation
            using (PaymentAllocationDataAccess data = new PaymentAllocationDataAccess(ClientContext))
            {
                // insert to paymentAllocationObject
                Int32 _Id = data.Insert(paymentAllocationObject);
                // if successful, process
                if (_Id > 0)
                {
                    paymentAllocationObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of PaymentAllocation Object.
        /// Data manipulation processing for: new, deleted, updated PaymentAllocation
        /// </summary>
        /// <param name="paymentAllocationObject"></param>
        /// <returns></returns>
        public bool UpdateBase(PaymentAllocation paymentAllocationObject)
        {
            // use of switch for different types of DML
            switch (paymentAllocationObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(paymentAllocationObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(paymentAllocationObject.Id);
            }
            // update rows
            using (PaymentAllocationDataAccess data = new PaymentAllocationDataAccess(ClientContext))
            {
                return (data.Update(paymentAllocationObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for PaymentAllocation
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (PaymentAllocationDataAccess data = new PaymentAllocationDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve PaymentAllocation data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>PaymentAllocation Object</returns>
        public PaymentAllocation Get(Int32 _Id)
        {
            using (PaymentAllocationDataAccess data = new PaymentAllocationDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a PaymentAllocation .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public PaymentAllocation Get(Int32 _Id, bool fillChild)
        {
            PaymentAllocation paymentAllocationObject;
            paymentAllocationObject = Get(_Id);
            
            if (paymentAllocationObject != null && fillChild)
            {
                // populate child data for a paymentAllocationObject
                FillPaymentAllocationWithChilds(paymentAllocationObject, fillChild);
            }

            return paymentAllocationObject;
        }
		
		/// <summary>
        /// populates a PaymentAllocation with its child entities
        /// </summary>
        /// <param name="paymentAllocation"></param>
		/// <param name="fillChilds"></param>
        private void FillPaymentAllocationWithChilds(PaymentAllocation paymentAllocationObject, bool fillChilds)
        {
            // populate child data for a paymentAllocationObject
            if (paymentAllocationObject != null)
            {
				// Retrieve CustomerPaymentIdObject as CustomerPayment type for the PaymentAllocation using CustomerPaymentId
				using(CustomerPaymentManager customerPaymentManager = new CustomerPaymentManager(ClientContext))
				{
					paymentAllocationObject.CustomerPaymentIdObject = customerPaymentManager.Get(paymentAllocationObject.CustomerPaymentId, fillChilds);
				}
				// Retrieve SalesOrderIdObject as SalesOrderHeader type for the PaymentAllocation using SalesOrderId
				using(SalesOrderHeaderManager salesOrderHeaderManager = new SalesOrderHeaderManager(ClientContext))
				{
					paymentAllocationObject.SalesOrderIdObject = salesOrderHeaderManager.Get(paymentAllocationObject.SalesOrderId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of PaymentAllocation.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of PaymentAllocation</returns>
        public PaymentAllocationList GetAll()
        {
            using (PaymentAllocationDataAccess data = new PaymentAllocationDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of PaymentAllocation.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of PaymentAllocation</returns>
        public PaymentAllocationList GetAll(bool fillChild)
        {
			PaymentAllocationList paymentAllocationList = new PaymentAllocationList();
            using (PaymentAllocationDataAccess data = new PaymentAllocationDataAccess(ClientContext))
            {
                paymentAllocationList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (PaymentAllocation paymentAllocationObject in paymentAllocationList)
                {
					FillPaymentAllocationWithChilds(paymentAllocationObject, fillChild);
				}
			}
			return paymentAllocationList;
        }
		
		/// <summary>
        /// Retrieve list of PaymentAllocation  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of PaymentAllocation</returns>
        public PaymentAllocationList GetPaged(PagedRequest request)
        {
            using (PaymentAllocationDataAccess data = new PaymentAllocationDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of PaymentAllocation  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of PaymentAllocation</returns>
        public PaymentAllocationList GetByQuery(String query)
        {
            using (PaymentAllocationDataAccess data = new PaymentAllocationDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get PaymentAllocation Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of PaymentAllocation
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (PaymentAllocationDataAccess data = new PaymentAllocationDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get PaymentAllocation Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of PaymentAllocation
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (PaymentAllocationDataAccess data = new PaymentAllocationDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of PaymentAllocation By CustomerPaymentId        
		/// <param name="_CustomerPaymentId"></param>
        /// </summary>
        /// <returns>List of PaymentAllocation</returns>
        public PaymentAllocationList GetByCustomerPaymentId(Int32 _CustomerPaymentId)
        {
            using (PaymentAllocationDataAccess data = new PaymentAllocationDataAccess(ClientContext))
            {
                return data.GetByCustomerPaymentId(_CustomerPaymentId);
            }
        }
		
		/// <summary>
        /// Retrieve list of PaymentAllocation By SalesOrderId        
		/// <param name="_SalesOrderId"></param>
        /// </summary>
        /// <returns>List of PaymentAllocation</returns>
        public PaymentAllocationList GetBySalesOrderId(Nullable<Int32> _SalesOrderId)
        {
            using (PaymentAllocationDataAccess data = new PaymentAllocationDataAccess(ClientContext))
            {
                return data.GetBySalesOrderId(_SalesOrderId);
            }
        }
		
		
		#endregion
	}	
}