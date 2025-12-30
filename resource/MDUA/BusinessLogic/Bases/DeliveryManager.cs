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
    /// Business logic processing for Delivery.
    /// </summary>    
	public partial class DeliveryManager : BaseManager
	{
	
		#region Constructors
		public DeliveryManager(ClientContext context) : base(context) { }
		public DeliveryManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new delivery.
        /// data manipulation for insertion of Delivery
        /// </summary>
        /// <param name="deliveryObject"></param>
        /// <returns></returns>
        private bool Insert(Delivery deliveryObject)
        {
            // new delivery
            using (DeliveryDataAccess data = new DeliveryDataAccess(ClientContext))
            {
                // insert to deliveryObject
                Int32 _Id = data.Insert(deliveryObject);
                // if successful, process
                if (_Id > 0)
                {
                    deliveryObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of Delivery Object.
        /// Data manipulation processing for: new, deleted, updated Delivery
        /// </summary>
        /// <param name="deliveryObject"></param>
        /// <returns></returns>
        public bool UpdateBase(Delivery deliveryObject)
        {
            // use of switch for different types of DML
            switch (deliveryObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(deliveryObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(deliveryObject.Id);
            }
            // update rows
            using (DeliveryDataAccess data = new DeliveryDataAccess(ClientContext))
            {
                return (data.Update(deliveryObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for Delivery
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (DeliveryDataAccess data = new DeliveryDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve Delivery data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>Delivery Object</returns>
        public Delivery Get(Int32 _Id)
        {
            using (DeliveryDataAccess data = new DeliveryDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a Delivery .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public Delivery Get(Int32 _Id, bool fillChild)
        {
            Delivery deliveryObject;
            deliveryObject = Get(_Id);
            
            if (deliveryObject != null && fillChild)
            {
                // populate child data for a deliveryObject
                FillDeliveryWithChilds(deliveryObject, fillChild);
            }

            return deliveryObject;
        }
		
		/// <summary>
        /// populates a Delivery with its child entities
        /// </summary>
        /// <param name="delivery"></param>
		/// <param name="fillChilds"></param>
        private void FillDeliveryWithChilds(Delivery deliveryObject, bool fillChilds)
        {
            // populate child data for a deliveryObject
            if (deliveryObject != null)
            {
				// Retrieve SalesOrderIdObject as SalesOrderHeader type for the Delivery using SalesOrderId
				using(SalesOrderHeaderManager salesOrderHeaderManager = new SalesOrderHeaderManager(ClientContext))
				{
					deliveryObject.SalesOrderIdObject = salesOrderHeaderManager.Get(deliveryObject.SalesOrderId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of Delivery.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of Delivery</returns>
        public DeliveryList GetAll()
        {
            using (DeliveryDataAccess data = new DeliveryDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of Delivery.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of Delivery</returns>
        public DeliveryList GetAll(bool fillChild)
        {
			DeliveryList deliveryList = new DeliveryList();
            using (DeliveryDataAccess data = new DeliveryDataAccess(ClientContext))
            {
                deliveryList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (Delivery deliveryObject in deliveryList)
                {
					FillDeliveryWithChilds(deliveryObject, fillChild);
				}
			}
			return deliveryList;
        }
		
		/// <summary>
        /// Retrieve list of Delivery  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of Delivery</returns>
        public DeliveryList GetPaged(PagedRequest request)
        {
            using (DeliveryDataAccess data = new DeliveryDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of Delivery  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of Delivery</returns>
        public DeliveryList GetByQuery(String query)
        {
            using (DeliveryDataAccess data = new DeliveryDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get Delivery Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Delivery
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (DeliveryDataAccess data = new DeliveryDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get Delivery Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Delivery
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (DeliveryDataAccess data = new DeliveryDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of Delivery By SalesOrderId        
		/// <param name="_SalesOrderId"></param>
        /// </summary>
        /// <returns>List of Delivery</returns>
        public DeliveryList GetBySalesOrderId(Int32 _SalesOrderId)
        {
            using (DeliveryDataAccess data = new DeliveryDataAccess(ClientContext))
            {
                return data.GetBySalesOrderId(_SalesOrderId);
            }
        }
		
		
		#endregion
	}	
}