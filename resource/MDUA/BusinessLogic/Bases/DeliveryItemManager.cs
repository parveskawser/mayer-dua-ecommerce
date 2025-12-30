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
    /// Business logic processing for DeliveryItem.
    /// </summary>    
	public partial class DeliveryItemManager : BaseManager
	{
	
		#region Constructors
		public DeliveryItemManager(ClientContext context) : base(context) { }
		public DeliveryItemManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new deliveryItem.
        /// data manipulation for insertion of DeliveryItem
        /// </summary>
        /// <param name="deliveryItemObject"></param>
        /// <returns></returns>
        private bool Insert(DeliveryItem deliveryItemObject)
        {
            // new deliveryItem
            using (DeliveryItemDataAccess data = new DeliveryItemDataAccess(ClientContext))
            {
                // insert to deliveryItemObject
                Int32 _Id = data.Insert(deliveryItemObject);
                // if successful, process
                if (_Id > 0)
                {
                    deliveryItemObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of DeliveryItem Object.
        /// Data manipulation processing for: new, deleted, updated DeliveryItem
        /// </summary>
        /// <param name="deliveryItemObject"></param>
        /// <returns></returns>
        public bool UpdateBase(DeliveryItem deliveryItemObject)
        {
            // use of switch for different types of DML
            switch (deliveryItemObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(deliveryItemObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(deliveryItemObject.Id);
            }
            // update rows
            using (DeliveryItemDataAccess data = new DeliveryItemDataAccess(ClientContext))
            {
                return (data.Update(deliveryItemObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for DeliveryItem
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (DeliveryItemDataAccess data = new DeliveryItemDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve DeliveryItem data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>DeliveryItem Object</returns>
        public DeliveryItem Get(Int32 _Id)
        {
            using (DeliveryItemDataAccess data = new DeliveryItemDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a DeliveryItem .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public DeliveryItem Get(Int32 _Id, bool fillChild)
        {
            DeliveryItem deliveryItemObject;
            deliveryItemObject = Get(_Id);
            
            if (deliveryItemObject != null && fillChild)
            {
                // populate child data for a deliveryItemObject
                FillDeliveryItemWithChilds(deliveryItemObject, fillChild);
            }

            return deliveryItemObject;
        }
		
		/// <summary>
        /// populates a DeliveryItem with its child entities
        /// </summary>
        /// <param name="deliveryItem"></param>
		/// <param name="fillChilds"></param>
        private void FillDeliveryItemWithChilds(DeliveryItem deliveryItemObject, bool fillChilds)
        {
            // populate child data for a deliveryItemObject
            if (deliveryItemObject != null)
            {
				// Retrieve DeliveryIdObject as Delivery type for the DeliveryItem using DeliveryId
				using(DeliveryManager deliveryManager = new DeliveryManager(ClientContext))
				{
					deliveryItemObject.DeliveryIdObject = deliveryManager.Get(deliveryItemObject.DeliveryId, fillChilds);
				}
				// Retrieve SalesOrderDetailIdObject as SalesOrderDetail type for the DeliveryItem using SalesOrderDetailId
				using(SalesOrderDetailManager salesOrderDetailManager = new SalesOrderDetailManager(ClientContext))
				{
					deliveryItemObject.SalesOrderDetailIdObject = salesOrderDetailManager.Get(deliveryItemObject.SalesOrderDetailId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of DeliveryItem.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of DeliveryItem</returns>
        public DeliveryItemList GetAll()
        {
            using (DeliveryItemDataAccess data = new DeliveryItemDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of DeliveryItem.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of DeliveryItem</returns>
        public DeliveryItemList GetAll(bool fillChild)
        {
			DeliveryItemList deliveryItemList = new DeliveryItemList();
            using (DeliveryItemDataAccess data = new DeliveryItemDataAccess(ClientContext))
            {
                deliveryItemList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (DeliveryItem deliveryItemObject in deliveryItemList)
                {
					FillDeliveryItemWithChilds(deliveryItemObject, fillChild);
				}
			}
			return deliveryItemList;
        }
		
		/// <summary>
        /// Retrieve list of DeliveryItem  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of DeliveryItem</returns>
        public DeliveryItemList GetPaged(PagedRequest request)
        {
            using (DeliveryItemDataAccess data = new DeliveryItemDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of DeliveryItem  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of DeliveryItem</returns>
        public DeliveryItemList GetByQuery(String query)
        {
            using (DeliveryItemDataAccess data = new DeliveryItemDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get DeliveryItem Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of DeliveryItem
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (DeliveryItemDataAccess data = new DeliveryItemDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get DeliveryItem Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of DeliveryItem
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (DeliveryItemDataAccess data = new DeliveryItemDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of DeliveryItem By DeliveryId        
		/// <param name="_DeliveryId"></param>
        /// </summary>
        /// <returns>List of DeliveryItem</returns>
        public DeliveryItemList GetByDeliveryId(Int32 _DeliveryId)
        {
            using (DeliveryItemDataAccess data = new DeliveryItemDataAccess(ClientContext))
            {
                return data.GetByDeliveryId(_DeliveryId);
            }
        }
		
		/// <summary>
        /// Retrieve list of DeliveryItem By SalesOrderDetailId        
		/// <param name="_SalesOrderDetailId"></param>
        /// </summary>
        /// <returns>List of DeliveryItem</returns>
        public DeliveryItemList GetBySalesOrderDetailId(Int32 _SalesOrderDetailId)
        {
            using (DeliveryItemDataAccess data = new DeliveryItemDataAccess(ClientContext))
            {
                return data.GetBySalesOrderDetailId(_SalesOrderDetailId);
            }
        }
		
		
		#endregion
	}	
}