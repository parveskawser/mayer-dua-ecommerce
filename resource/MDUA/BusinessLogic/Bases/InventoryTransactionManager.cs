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
    /// Business logic processing for InventoryTransaction.
    /// </summary>    
	public partial class InventoryTransactionManager : BaseManager
	{
	
		#region Constructors
		public InventoryTransactionManager(ClientContext context) : base(context) { }
		public InventoryTransactionManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new inventoryTransaction.
        /// data manipulation for insertion of InventoryTransaction
        /// </summary>
        /// <param name="inventoryTransactionObject"></param>
        /// <returns></returns>
        private bool Insert(InventoryTransaction inventoryTransactionObject)
        {
            // new inventoryTransaction
            using (InventoryTransactionDataAccess data = new InventoryTransactionDataAccess(ClientContext))
            {
                // insert to inventoryTransactionObject
                Int32 _Id = data.Insert(inventoryTransactionObject);
                // if successful, process
                if (_Id > 0)
                {
                    inventoryTransactionObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of InventoryTransaction Object.
        /// Data manipulation processing for: new, deleted, updated InventoryTransaction
        /// </summary>
        /// <param name="inventoryTransactionObject"></param>
        /// <returns></returns>
        public bool UpdateBase(InventoryTransaction inventoryTransactionObject)
        {
            // use of switch for different types of DML
            switch (inventoryTransactionObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(inventoryTransactionObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(inventoryTransactionObject.Id);
            }
            // update rows
            using (InventoryTransactionDataAccess data = new InventoryTransactionDataAccess(ClientContext))
            {
                return (data.Update(inventoryTransactionObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for InventoryTransaction
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (InventoryTransactionDataAccess data = new InventoryTransactionDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve InventoryTransaction data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>InventoryTransaction Object</returns>
        public InventoryTransaction Get(Int32 _Id)
        {
            using (InventoryTransactionDataAccess data = new InventoryTransactionDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a InventoryTransaction .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public InventoryTransaction Get(Int32 _Id, bool fillChild)
        {
            InventoryTransaction inventoryTransactionObject;
            inventoryTransactionObject = Get(_Id);
            
            if (inventoryTransactionObject != null && fillChild)
            {
                // populate child data for a inventoryTransactionObject
                FillInventoryTransactionWithChilds(inventoryTransactionObject, fillChild);
            }

            return inventoryTransactionObject;
        }
		
		/// <summary>
        /// populates a InventoryTransaction with its child entities
        /// </summary>
        /// <param name="inventoryTransaction"></param>
		/// <param name="fillChilds"></param>
        private void FillInventoryTransactionWithChilds(InventoryTransaction inventoryTransactionObject, bool fillChilds)
        {
            // populate child data for a inventoryTransactionObject
            if (inventoryTransactionObject != null)
            {
				// Retrieve PoReceivedIdObject as PoReceived type for the InventoryTransaction using PoReceivedId
				using(PoReceivedManager poReceivedManager = new PoReceivedManager(ClientContext))
				{
					inventoryTransactionObject.PoReceivedIdObject = poReceivedManager.Get(inventoryTransactionObject.PoReceivedId, fillChilds);
				}
				// Retrieve ProductVariantIdObject as ProductVariant type for the InventoryTransaction using ProductVariantId
				using(ProductVariantManager productVariantManager = new ProductVariantManager(ClientContext))
				{
					inventoryTransactionObject.ProductVariantIdObject = productVariantManager.Get(inventoryTransactionObject.ProductVariantId, fillChilds);
				}
				// Retrieve SalesOrderDetailIdObject as SalesOrderDetail type for the InventoryTransaction using SalesOrderDetailId
				using(SalesOrderDetailManager salesOrderDetailManager = new SalesOrderDetailManager(ClientContext))
				{
					inventoryTransactionObject.SalesOrderDetailIdObject = salesOrderDetailManager.Get(inventoryTransactionObject.SalesOrderDetailId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of InventoryTransaction.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of InventoryTransaction</returns>
        public InventoryTransactionList GetAll()
        {
            using (InventoryTransactionDataAccess data = new InventoryTransactionDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of InventoryTransaction.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of InventoryTransaction</returns>
        public InventoryTransactionList GetAll(bool fillChild)
        {
			InventoryTransactionList inventoryTransactionList = new InventoryTransactionList();
            using (InventoryTransactionDataAccess data = new InventoryTransactionDataAccess(ClientContext))
            {
                inventoryTransactionList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (InventoryTransaction inventoryTransactionObject in inventoryTransactionList)
                {
					FillInventoryTransactionWithChilds(inventoryTransactionObject, fillChild);
				}
			}
			return inventoryTransactionList;
        }
		
		/// <summary>
        /// Retrieve list of InventoryTransaction  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of InventoryTransaction</returns>
        public InventoryTransactionList GetPaged(PagedRequest request)
        {
            using (InventoryTransactionDataAccess data = new InventoryTransactionDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of InventoryTransaction  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of InventoryTransaction</returns>
        public InventoryTransactionList GetByQuery(String query)
        {
            using (InventoryTransactionDataAccess data = new InventoryTransactionDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get InventoryTransaction Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of InventoryTransaction
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (InventoryTransactionDataAccess data = new InventoryTransactionDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get InventoryTransaction Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of InventoryTransaction
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (InventoryTransactionDataAccess data = new InventoryTransactionDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of InventoryTransaction By SalesOrderDetailId        
		/// <param name="_SalesOrderDetailId"></param>
        /// </summary>
        /// <returns>List of InventoryTransaction</returns>
        public InventoryTransactionList GetBySalesOrderDetailId(Nullable<Int32> _SalesOrderDetailId)
        {
            using (InventoryTransactionDataAccess data = new InventoryTransactionDataAccess(ClientContext))
            {
                return data.GetBySalesOrderDetailId(_SalesOrderDetailId);
            }
        }
		
		/// <summary>
        /// Retrieve list of InventoryTransaction By PoReceivedId        
		/// <param name="_PoReceivedId"></param>
        /// </summary>
        /// <returns>List of InventoryTransaction</returns>
        public InventoryTransactionList GetByPoReceivedId(Nullable<Int32> _PoReceivedId)
        {
            using (InventoryTransactionDataAccess data = new InventoryTransactionDataAccess(ClientContext))
            {
                return data.GetByPoReceivedId(_PoReceivedId);
            }
        }
		
		/// <summary>
        /// Retrieve list of InventoryTransaction By ProductVariantId        
		/// <param name="_ProductVariantId"></param>
        /// </summary>
        /// <returns>List of InventoryTransaction</returns>
        public InventoryTransactionList GetByProductVariantId(Int32 _ProductVariantId)
        {
            using (InventoryTransactionDataAccess data = new InventoryTransactionDataAccess(ClientContext))
            {
                return data.GetByProductVariantId(_ProductVariantId);
            }
        }
		
		
		#endregion
	}	
}