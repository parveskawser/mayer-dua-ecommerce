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
    /// Business logic processing for BulkPurchaseOrder.
    /// </summary>    
	public partial class BulkPurchaseOrderManager : BaseManager
	{
	
		#region Constructors
		public BulkPurchaseOrderManager(ClientContext context) : base(context) { }
		public BulkPurchaseOrderManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new bulkPurchaseOrder.
        /// data manipulation for insertion of BulkPurchaseOrder
        /// </summary>
        /// <param name="bulkPurchaseOrderObject"></param>
        /// <returns></returns>
        private bool Insert(BulkPurchaseOrder bulkPurchaseOrderObject)
        {
            // new bulkPurchaseOrder
            using (BulkPurchaseOrderDataAccess data = new BulkPurchaseOrderDataAccess(ClientContext))
            {
                // insert to bulkPurchaseOrderObject
                Int32 _Id = data.Insert(bulkPurchaseOrderObject);
                // if successful, process
                if (_Id > 0)
                {
                    bulkPurchaseOrderObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of BulkPurchaseOrder Object.
        /// Data manipulation processing for: new, deleted, updated BulkPurchaseOrder
        /// </summary>
        /// <param name="bulkPurchaseOrderObject"></param>
        /// <returns></returns>
        public bool UpdateBase(BulkPurchaseOrder bulkPurchaseOrderObject)
        {
            // use of switch for different types of DML
            switch (bulkPurchaseOrderObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(bulkPurchaseOrderObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(bulkPurchaseOrderObject.Id);
            }
            // update rows
            using (BulkPurchaseOrderDataAccess data = new BulkPurchaseOrderDataAccess(ClientContext))
            {
                return (data.Update(bulkPurchaseOrderObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for BulkPurchaseOrder
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (BulkPurchaseOrderDataAccess data = new BulkPurchaseOrderDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve BulkPurchaseOrder data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>BulkPurchaseOrder Object</returns>
        public BulkPurchaseOrder Get(Int32 _Id)
        {
            using (BulkPurchaseOrderDataAccess data = new BulkPurchaseOrderDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a BulkPurchaseOrder .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public BulkPurchaseOrder Get(Int32 _Id, bool fillChild)
        {
            BulkPurchaseOrder bulkPurchaseOrderObject;
            bulkPurchaseOrderObject = Get(_Id);
            
            if (bulkPurchaseOrderObject != null && fillChild)
            {
                // populate child data for a bulkPurchaseOrderObject
                FillBulkPurchaseOrderWithChilds(bulkPurchaseOrderObject, fillChild);
            }

            return bulkPurchaseOrderObject;
        }
		
		/// <summary>
        /// populates a BulkPurchaseOrder with its child entities
        /// </summary>
        /// <param name="bulkPurchaseOrder"></param>
		/// <param name="fillChilds"></param>
        private void FillBulkPurchaseOrderWithChilds(BulkPurchaseOrder bulkPurchaseOrderObject, bool fillChilds)
        {
            // populate child data for a bulkPurchaseOrderObject
            if (bulkPurchaseOrderObject != null)
            {
				// Retrieve VendorIdObject as Vendor type for the BulkPurchaseOrder using VendorId
				using(VendorManager vendorManager = new VendorManager(ClientContext))
				{
					bulkPurchaseOrderObject.VendorIdObject = vendorManager.Get(bulkPurchaseOrderObject.VendorId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of BulkPurchaseOrder.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of BulkPurchaseOrder</returns>
        public BulkPurchaseOrderList GetAll()
        {
            using (BulkPurchaseOrderDataAccess data = new BulkPurchaseOrderDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of BulkPurchaseOrder.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of BulkPurchaseOrder</returns>
        public BulkPurchaseOrderList GetAll(bool fillChild)
        {
			BulkPurchaseOrderList bulkPurchaseOrderList = new BulkPurchaseOrderList();
            using (BulkPurchaseOrderDataAccess data = new BulkPurchaseOrderDataAccess(ClientContext))
            {
                bulkPurchaseOrderList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (BulkPurchaseOrder bulkPurchaseOrderObject in bulkPurchaseOrderList)
                {
					FillBulkPurchaseOrderWithChilds(bulkPurchaseOrderObject, fillChild);
				}
			}
			return bulkPurchaseOrderList;
        }
		
		/// <summary>
        /// Retrieve list of BulkPurchaseOrder  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of BulkPurchaseOrder</returns>
        public BulkPurchaseOrderList GetPaged(PagedRequest request)
        {
            using (BulkPurchaseOrderDataAccess data = new BulkPurchaseOrderDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of BulkPurchaseOrder  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of BulkPurchaseOrder</returns>
        public BulkPurchaseOrderList GetByQuery(String query)
        {
            using (BulkPurchaseOrderDataAccess data = new BulkPurchaseOrderDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get BulkPurchaseOrder Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of BulkPurchaseOrder
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (BulkPurchaseOrderDataAccess data = new BulkPurchaseOrderDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get BulkPurchaseOrder Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of BulkPurchaseOrder
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (BulkPurchaseOrderDataAccess data = new BulkPurchaseOrderDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of BulkPurchaseOrder By VendorId        
		/// <param name="_VendorId"></param>
        /// </summary>
        /// <returns>List of BulkPurchaseOrder</returns>
        public BulkPurchaseOrderList GetByVendorId(Int32 _VendorId)
        {
            using (BulkPurchaseOrderDataAccess data = new BulkPurchaseOrderDataAccess(ClientContext))
            {
                return data.GetByVendorId(_VendorId);
            }
        }
		
		
		#endregion
	}	
}