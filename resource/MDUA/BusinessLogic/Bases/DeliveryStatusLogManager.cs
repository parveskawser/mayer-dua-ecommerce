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
    /// Business logic processing for DeliveryStatusLog.
    /// </summary>    
	public partial class DeliveryStatusLogManager : BaseManager
	{
	
		#region Constructors
		public DeliveryStatusLogManager(ClientContext context) : base(context) { }
		public DeliveryStatusLogManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new deliveryStatusLog.
        /// data manipulation for insertion of DeliveryStatusLog
        /// </summary>
        /// <param name="deliveryStatusLogObject"></param>
        /// <returns></returns>
        private bool Insert(DeliveryStatusLog deliveryStatusLogObject)
        {
            // new deliveryStatusLog
            using (DeliveryStatusLogDataAccess data = new DeliveryStatusLogDataAccess(ClientContext))
            {
                // insert to deliveryStatusLogObject
                Int32 _Id = data.Insert(deliveryStatusLogObject);
                // if successful, process
                if (_Id > 0)
                {
                    deliveryStatusLogObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of DeliveryStatusLog Object.
        /// Data manipulation processing for: new, deleted, updated DeliveryStatusLog
        /// </summary>
        /// <param name="deliveryStatusLogObject"></param>
        /// <returns></returns>
        public bool UpdateBase(DeliveryStatusLog deliveryStatusLogObject)
        {
            // use of switch for different types of DML
            switch (deliveryStatusLogObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(deliveryStatusLogObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(deliveryStatusLogObject.Id);
            }
            // update rows
            using (DeliveryStatusLogDataAccess data = new DeliveryStatusLogDataAccess(ClientContext))
            {
                return (data.Update(deliveryStatusLogObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for DeliveryStatusLog
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (DeliveryStatusLogDataAccess data = new DeliveryStatusLogDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve DeliveryStatusLog data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>DeliveryStatusLog Object</returns>
        public DeliveryStatusLog Get(Int32 _Id)
        {
            using (DeliveryStatusLogDataAccess data = new DeliveryStatusLogDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a DeliveryStatusLog .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public DeliveryStatusLog Get(Int32 _Id, bool fillChild)
        {
            DeliveryStatusLog deliveryStatusLogObject;
            deliveryStatusLogObject = Get(_Id);
            
            if (deliveryStatusLogObject != null && fillChild)
            {
                // populate child data for a deliveryStatusLogObject
                FillDeliveryStatusLogWithChilds(deliveryStatusLogObject, fillChild);
            }

            return deliveryStatusLogObject;
        }
		
		/// <summary>
        /// populates a DeliveryStatusLog with its child entities
        /// </summary>
        /// <param name="deliveryStatusLog"></param>
		/// <param name="fillChilds"></param>
        private void FillDeliveryStatusLogWithChilds(DeliveryStatusLog deliveryStatusLogObject, bool fillChilds)
        {
            // populate child data for a deliveryStatusLogObject
            if (deliveryStatusLogObject != null)
            {
				// Retrieve SalesOrderIdObject as SalesOrderHeader type for the DeliveryStatusLog using SalesOrderId
				using(SalesOrderHeaderManager salesOrderHeaderManager = new SalesOrderHeaderManager(ClientContext))
				{
					deliveryStatusLogObject.SalesOrderIdObject = salesOrderHeaderManager.Get(deliveryStatusLogObject.SalesOrderId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of DeliveryStatusLog.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of DeliveryStatusLog</returns>
        public DeliveryStatusLogList GetAll()
        {
            using (DeliveryStatusLogDataAccess data = new DeliveryStatusLogDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of DeliveryStatusLog.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of DeliveryStatusLog</returns>
        public DeliveryStatusLogList GetAll(bool fillChild)
        {
			DeliveryStatusLogList deliveryStatusLogList = new DeliveryStatusLogList();
            using (DeliveryStatusLogDataAccess data = new DeliveryStatusLogDataAccess(ClientContext))
            {
                deliveryStatusLogList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (DeliveryStatusLog deliveryStatusLogObject in deliveryStatusLogList)
                {
					FillDeliveryStatusLogWithChilds(deliveryStatusLogObject, fillChild);
				}
			}
			return deliveryStatusLogList;
        }
		
		/// <summary>
        /// Retrieve list of DeliveryStatusLog  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of DeliveryStatusLog</returns>
        public DeliveryStatusLogList GetPaged(PagedRequest request)
        {
            using (DeliveryStatusLogDataAccess data = new DeliveryStatusLogDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of DeliveryStatusLog  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of DeliveryStatusLog</returns>
        public DeliveryStatusLogList GetByQuery(String query)
        {
            using (DeliveryStatusLogDataAccess data = new DeliveryStatusLogDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get DeliveryStatusLog Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of DeliveryStatusLog
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (DeliveryStatusLogDataAccess data = new DeliveryStatusLogDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get DeliveryStatusLog Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of DeliveryStatusLog
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (DeliveryStatusLogDataAccess data = new DeliveryStatusLogDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of DeliveryStatusLog By SalesOrderId        
		/// <param name="_SalesOrderId"></param>
        /// </summary>
        /// <returns>List of DeliveryStatusLog</returns>
        public DeliveryStatusLogList GetBySalesOrderId(Nullable<Int32> _SalesOrderId)
        {
            using (DeliveryStatusLogDataAccess data = new DeliveryStatusLogDataAccess(ClientContext))
            {
                return data.GetBySalesOrderId(_SalesOrderId);
            }
        }
		
		
		#endregion
	}	
}