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
    /// Business logic processing for PaymentMethod.
    /// </summary>    
	public partial class PaymentMethodManager : BaseManager
	{
	
		#region Constructors
		public PaymentMethodManager(ClientContext context) : base(context) { }
		public PaymentMethodManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new paymentMethod.
        /// data manipulation for insertion of PaymentMethod
        /// </summary>
        /// <param name="paymentMethodObject"></param>
        /// <returns></returns>
        private bool Insert(PaymentMethod paymentMethodObject)
        {
            // new paymentMethod
            using (PaymentMethodDataAccess data = new PaymentMethodDataAccess(ClientContext))
            {
                // insert to paymentMethodObject
                Int32 _Id = data.Insert(paymentMethodObject);
                // if successful, process
                if (_Id > 0)
                {
                    paymentMethodObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of PaymentMethod Object.
        /// Data manipulation processing for: new, deleted, updated PaymentMethod
        /// </summary>
        /// <param name="paymentMethodObject"></param>
        /// <returns></returns>
        public bool UpdateBase(PaymentMethod paymentMethodObject)
        {
            // use of switch for different types of DML
            switch (paymentMethodObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(paymentMethodObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(paymentMethodObject.Id);
            }
            // update rows
            using (PaymentMethodDataAccess data = new PaymentMethodDataAccess(ClientContext))
            {
                return (data.Update(paymentMethodObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for PaymentMethod
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (PaymentMethodDataAccess data = new PaymentMethodDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve PaymentMethod data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>PaymentMethod Object</returns>
        public PaymentMethod Get(Int32 _Id)
        {
            using (PaymentMethodDataAccess data = new PaymentMethodDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a PaymentMethod .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public PaymentMethod Get(Int32 _Id, bool fillChild)
        {
            PaymentMethod paymentMethodObject;
            paymentMethodObject = Get(_Id);
            
            if (paymentMethodObject != null && fillChild)
            {
                // populate child data for a paymentMethodObject
                FillPaymentMethodWithChilds(paymentMethodObject, fillChild);
            }

            return paymentMethodObject;
        }
		
		/// <summary>
        /// populates a PaymentMethod with its child entities
        /// </summary>
        /// <param name="paymentMethod"></param>
		/// <param name="fillChilds"></param>
        private void FillPaymentMethodWithChilds(PaymentMethod paymentMethodObject, bool fillChilds)
        {
            // populate child data for a paymentMethodObject
            if (paymentMethodObject != null)
            {
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of PaymentMethod.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of PaymentMethod</returns>
        public PaymentMethodList GetAll()
        {
            using (PaymentMethodDataAccess data = new PaymentMethodDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of PaymentMethod.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of PaymentMethod</returns>
        public PaymentMethodList GetAll(bool fillChild)
        {
			PaymentMethodList paymentMethodList = new PaymentMethodList();
            using (PaymentMethodDataAccess data = new PaymentMethodDataAccess(ClientContext))
            {
                paymentMethodList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (PaymentMethod paymentMethodObject in paymentMethodList)
                {
					FillPaymentMethodWithChilds(paymentMethodObject, fillChild);
				}
			}
			return paymentMethodList;
        }
		
		/// <summary>
        /// Retrieve list of PaymentMethod  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of PaymentMethod</returns>
        public PaymentMethodList GetPaged(PagedRequest request)
        {
            using (PaymentMethodDataAccess data = new PaymentMethodDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of PaymentMethod  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of PaymentMethod</returns>
        public PaymentMethodList GetByQuery(String query)
        {
            using (PaymentMethodDataAccess data = new PaymentMethodDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get PaymentMethod Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of PaymentMethod
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (PaymentMethodDataAccess data = new PaymentMethodDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get PaymentMethod Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of PaymentMethod
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (PaymentMethodDataAccess data = new PaymentMethodDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		
		#endregion
	}	
}