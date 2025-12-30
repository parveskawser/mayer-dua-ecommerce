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
    /// Business logic processing for VendorPayment.
    /// </summary>    
	public partial class VendorPaymentManager : BaseManager
	{
	
		#region Constructors
		public VendorPaymentManager(ClientContext context) : base(context) { }
		public VendorPaymentManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new vendorPayment.
        /// data manipulation for insertion of VendorPayment
        /// </summary>
        /// <param name="vendorPaymentObject"></param>
        /// <returns></returns>
        private bool Insert(VendorPayment vendorPaymentObject)
        {
            // new vendorPayment
            using (VendorPaymentDataAccess data = new VendorPaymentDataAccess(ClientContext))
            {
                // insert to vendorPaymentObject
                Int32 _Id = data.Insert(vendorPaymentObject);
                // if successful, process
                if (_Id > 0)
                {
                    vendorPaymentObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of VendorPayment Object.
        /// Data manipulation processing for: new, deleted, updated VendorPayment
        /// </summary>
        /// <param name="vendorPaymentObject"></param>
        /// <returns></returns>
        public bool UpdateBase(VendorPayment vendorPaymentObject)
        {
            // use of switch for different types of DML
            switch (vendorPaymentObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(vendorPaymentObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(vendorPaymentObject.Id);
            }
            // update rows
            using (VendorPaymentDataAccess data = new VendorPaymentDataAccess(ClientContext))
            {
                return (data.Update(vendorPaymentObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for VendorPayment
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (VendorPaymentDataAccess data = new VendorPaymentDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve VendorPayment data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>VendorPayment Object</returns>
        public VendorPayment Get(Int32 _Id)
        {
            using (VendorPaymentDataAccess data = new VendorPaymentDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a VendorPayment .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public VendorPayment Get(Int32 _Id, bool fillChild)
        {
            VendorPayment vendorPaymentObject;
            vendorPaymentObject = Get(_Id);
            
            if (vendorPaymentObject != null && fillChild)
            {
                // populate child data for a vendorPaymentObject
                FillVendorPaymentWithChilds(vendorPaymentObject, fillChild);
            }

            return vendorPaymentObject;
        }
		
		/// <summary>
        /// populates a VendorPayment with its child entities
        /// </summary>
        /// <param name="vendorPayment"></param>
		/// <param name="fillChilds"></param>
        private void FillVendorPaymentWithChilds(VendorPayment vendorPaymentObject, bool fillChilds)
        {
            // populate child data for a vendorPaymentObject
            if (vendorPaymentObject != null)
            {
				// Retrieve InventoryTransactionIdObject as InventoryTransaction type for the VendorPayment using InventoryTransactionId
				using(InventoryTransactionManager inventoryTransactionManager = new InventoryTransactionManager(ClientContext))
				{
					vendorPaymentObject.InventoryTransactionIdObject = inventoryTransactionManager.Get(vendorPaymentObject.InventoryTransactionId, fillChilds);
				}
				// Retrieve PaymentMethodIdObject as PaymentMethod type for the VendorPayment using PaymentMethodId
				using(PaymentMethodManager paymentMethodManager = new PaymentMethodManager(ClientContext))
				{
					vendorPaymentObject.PaymentMethodIdObject = paymentMethodManager.Get(vendorPaymentObject.PaymentMethodId, fillChilds);
				}
				// Retrieve VendorIdObject as CompanyVendor type for the VendorPayment using VendorId
				using(CompanyVendorManager companyVendorManager = new CompanyVendorManager(ClientContext))
				{
					vendorPaymentObject.VendorIdObject = companyVendorManager.Get(vendorPaymentObject.VendorId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of VendorPayment.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of VendorPayment</returns>
        public VendorPaymentList GetAll()
        {
            using (VendorPaymentDataAccess data = new VendorPaymentDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of VendorPayment.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of VendorPayment</returns>
        public VendorPaymentList GetAll(bool fillChild)
        {
			VendorPaymentList vendorPaymentList = new VendorPaymentList();
            using (VendorPaymentDataAccess data = new VendorPaymentDataAccess(ClientContext))
            {
                vendorPaymentList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (VendorPayment vendorPaymentObject in vendorPaymentList)
                {
					FillVendorPaymentWithChilds(vendorPaymentObject, fillChild);
				}
			}
			return vendorPaymentList;
        }
		
		/// <summary>
        /// Retrieve list of VendorPayment  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of VendorPayment</returns>
        public VendorPaymentList GetPaged(PagedRequest request)
        {
            using (VendorPaymentDataAccess data = new VendorPaymentDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of VendorPayment  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of VendorPayment</returns>
        public VendorPaymentList GetByQuery(String query)
        {
            using (VendorPaymentDataAccess data = new VendorPaymentDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get VendorPayment Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of VendorPayment
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (VendorPaymentDataAccess data = new VendorPaymentDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get VendorPayment Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of VendorPayment
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (VendorPaymentDataAccess data = new VendorPaymentDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of VendorPayment By VendorId        
		/// <param name="_VendorId"></param>
        /// </summary>
        /// <returns>List of VendorPayment</returns>
        public VendorPaymentList GetByVendorId(Int32 _VendorId)
        {
            using (VendorPaymentDataAccess data = new VendorPaymentDataAccess(ClientContext))
            {
                return data.GetByVendorId(_VendorId);
            }
        }
		
		/// <summary>
        /// Retrieve list of VendorPayment By PaymentMethodId        
		/// <param name="_PaymentMethodId"></param>
        /// </summary>
        /// <returns>List of VendorPayment</returns>
        public VendorPaymentList GetByPaymentMethodId(Int32 _PaymentMethodId)
        {
            using (VendorPaymentDataAccess data = new VendorPaymentDataAccess(ClientContext))
            {
                return data.GetByPaymentMethodId(_PaymentMethodId);
            }
        }
		
		/// <summary>
        /// Retrieve list of VendorPayment By InventoryTransactionId        
		/// <param name="_InventoryTransactionId"></param>
        /// </summary>
        /// <returns>List of VendorPayment</returns>
        public VendorPaymentList GetByInventoryTransactionId(Nullable<Int32> _InventoryTransactionId)
        {
            using (VendorPaymentDataAccess data = new VendorPaymentDataAccess(ClientContext))
            {
                return data.GetByInventoryTransactionId(_InventoryTransactionId);
            }
        }
		
		
		#endregion
	}	
}