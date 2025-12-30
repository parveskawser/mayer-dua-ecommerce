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
    /// Business logic processing for PoRequested.
    /// </summary>    
	public partial class PoRequestedManager : BaseManager
	{
	
		#region Constructors
		public PoRequestedManager(ClientContext context) : base(context) { }
		public PoRequestedManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new poRequested.
        /// data manipulation for insertion of PoRequested
        /// </summary>
        /// <param name="poRequestedObject"></param>
        /// <returns></returns>
        private bool Insert(PoRequested poRequestedObject)
        {
            // new poRequested
            using (PoRequestedDataAccess data = new PoRequestedDataAccess(ClientContext))
            {
                // insert to poRequestedObject
                Int32 _Id = data.Insert(poRequestedObject);
                // if successful, process
                if (_Id > 0)
                {
                    poRequestedObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of PoRequested Object.
        /// Data manipulation processing for: new, deleted, updated PoRequested
        /// </summary>
        /// <param name="poRequestedObject"></param>
        /// <returns></returns>
        public bool UpdateBase(PoRequested poRequestedObject)
        {
            // use of switch for different types of DML
            switch (poRequestedObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(poRequestedObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(poRequestedObject.Id);
            }
            // update rows
            using (PoRequestedDataAccess data = new PoRequestedDataAccess(ClientContext))
            {
                return (data.Update(poRequestedObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for PoRequested
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (PoRequestedDataAccess data = new PoRequestedDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve PoRequested data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>PoRequested Object</returns>
        public PoRequested Get(Int32 _Id)
        {
            using (PoRequestedDataAccess data = new PoRequestedDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a PoRequested .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public PoRequested Get(Int32 _Id, bool fillChild)
        {
            PoRequested poRequestedObject;
            poRequestedObject = Get(_Id);
            
            if (poRequestedObject != null && fillChild)
            {
                // populate child data for a poRequestedObject
                FillPoRequestedWithChilds(poRequestedObject, fillChild);
            }

            return poRequestedObject;
        }
		
		/// <summary>
        /// populates a PoRequested with its child entities
        /// </summary>
        /// <param name="poRequested"></param>
		/// <param name="fillChilds"></param>
        private void FillPoRequestedWithChilds(PoRequested poRequestedObject, bool fillChilds)
        {
            // populate child data for a poRequestedObject
            if (poRequestedObject != null)
            {
				// Retrieve BulkPurchaseOrderIdObject as BulkPurchaseOrder type for the PoRequested using BulkPurchaseOrderId
				using(BulkPurchaseOrderManager bulkPurchaseOrderManager = new BulkPurchaseOrderManager(ClientContext))
				{
					poRequestedObject.BulkPurchaseOrderIdObject = bulkPurchaseOrderManager.Get(poRequestedObject.BulkPurchaseOrderId, fillChilds);
				}
				// Retrieve ProductVariantIdObject as ProductVariant type for the PoRequested using ProductVariantId
				using(ProductVariantManager productVariantManager = new ProductVariantManager(ClientContext))
				{
					poRequestedObject.ProductVariantIdObject = productVariantManager.Get(poRequestedObject.ProductVariantId, fillChilds);
				}
				// Retrieve VendorIdObject as Vendor type for the PoRequested using VendorId
				using(VendorManager vendorManager = new VendorManager(ClientContext))
				{
					poRequestedObject.VendorIdObject = vendorManager.Get(poRequestedObject.VendorId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of PoRequested.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of PoRequested</returns>
        public PoRequestedList GetAll()
        {
            using (PoRequestedDataAccess data = new PoRequestedDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of PoRequested.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of PoRequested</returns>
        public PoRequestedList GetAll(bool fillChild)
        {
			PoRequestedList poRequestedList = new PoRequestedList();
            using (PoRequestedDataAccess data = new PoRequestedDataAccess(ClientContext))
            {
                poRequestedList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (PoRequested poRequestedObject in poRequestedList)
                {
					FillPoRequestedWithChilds(poRequestedObject, fillChild);
				}
			}
			return poRequestedList;
        }
		
		/// <summary>
        /// Retrieve list of PoRequested  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of PoRequested</returns>
        public PoRequestedList GetPaged(PagedRequest request)
        {
            using (PoRequestedDataAccess data = new PoRequestedDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of PoRequested  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of PoRequested</returns>
        public PoRequestedList GetByQuery(String query)
        {
            using (PoRequestedDataAccess data = new PoRequestedDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get PoRequested Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of PoRequested
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (PoRequestedDataAccess data = new PoRequestedDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get PoRequested Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of PoRequested
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (PoRequestedDataAccess data = new PoRequestedDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of PoRequested By VendorId        
		/// <param name="_VendorId"></param>
        /// </summary>
        /// <returns>List of PoRequested</returns>
        public PoRequestedList GetByVendorId(Int32 _VendorId)
        {
            using (PoRequestedDataAccess data = new PoRequestedDataAccess(ClientContext))
            {
                return data.GetByVendorId(_VendorId);
            }
        }
		
		/// <summary>
        /// Retrieve list of PoRequested By ProductVariantId        
		/// <param name="_ProductVariantId"></param>
        /// </summary>
        /// <returns>List of PoRequested</returns>
        public PoRequestedList GetByProductVariantId(Int32 _ProductVariantId)
        {
            using (PoRequestedDataAccess data = new PoRequestedDataAccess(ClientContext))
            {
                return data.GetByProductVariantId(_ProductVariantId);
            }
        }
		
		/// <summary>
        /// Retrieve list of PoRequested By BulkPurchaseOrderId        
		/// <param name="_BulkPurchaseOrderId"></param>
        /// </summary>
        /// <returns>List of PoRequested</returns>
        public PoRequestedList GetByBulkPurchaseOrderId(Nullable<Int32> _BulkPurchaseOrderId)
        {
            using (PoRequestedDataAccess data = new PoRequestedDataAccess(ClientContext))
            {
                return data.GetByBulkPurchaseOrderId(_BulkPurchaseOrderId);
            }
        }
		
		
		#endregion
	}	
}