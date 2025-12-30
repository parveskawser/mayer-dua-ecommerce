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
    /// Business logic processing for SalesReturn.
    /// </summary>    
	public partial class SalesReturnManager : BaseManager
	{
	
		#region Constructors
		public SalesReturnManager(ClientContext context) : base(context) { }
		public SalesReturnManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new salesReturn.
        /// data manipulation for insertion of SalesReturn
        /// </summary>
        /// <param name="salesReturnObject"></param>
        /// <returns></returns>
        private bool Insert(SalesReturn salesReturnObject)
        {
            // new salesReturn
            using (SalesReturnDataAccess data = new SalesReturnDataAccess(ClientContext))
            {
                // insert to salesReturnObject
                Int32 _Id = data.Insert(salesReturnObject);
                // if successful, process
                if (_Id > 0)
                {
                    salesReturnObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of SalesReturn Object.
        /// Data manipulation processing for: new, deleted, updated SalesReturn
        /// </summary>
        /// <param name="salesReturnObject"></param>
        /// <returns></returns>
        public bool UpdateBase(SalesReturn salesReturnObject)
        {
            // use of switch for different types of DML
            switch (salesReturnObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(salesReturnObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(salesReturnObject.Id);
            }
            // update rows
            using (SalesReturnDataAccess data = new SalesReturnDataAccess(ClientContext))
            {
                return (data.Update(salesReturnObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for SalesReturn
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (SalesReturnDataAccess data = new SalesReturnDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve SalesReturn data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>SalesReturn Object</returns>
        public SalesReturn Get(Int32 _Id)
        {
            using (SalesReturnDataAccess data = new SalesReturnDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a SalesReturn .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public SalesReturn Get(Int32 _Id, bool fillChild)
        {
            SalesReturn salesReturnObject;
            salesReturnObject = Get(_Id);
            
            if (salesReturnObject != null && fillChild)
            {
                // populate child data for a salesReturnObject
                FillSalesReturnWithChilds(salesReturnObject, fillChild);
            }

            return salesReturnObject;
        }
		
		/// <summary>
        /// populates a SalesReturn with its child entities
        /// </summary>
        /// <param name="salesReturn"></param>
		/// <param name="fillChilds"></param>
        private void FillSalesReturnWithChilds(SalesReturn salesReturnObject, bool fillChilds)
        {
            // populate child data for a salesReturnObject
            if (salesReturnObject != null)
            {
				// Retrieve SalesOrderDetailIdObject as SalesOrderDetail type for the SalesReturn using SalesOrderDetailId
				using(SalesOrderDetailManager salesOrderDetailManager = new SalesOrderDetailManager(ClientContext))
				{
					salesReturnObject.SalesOrderDetailIdObject = salesOrderDetailManager.Get(salesReturnObject.SalesOrderDetailId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of SalesReturn.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of SalesReturn</returns>
        public SalesReturnList GetAll()
        {
            using (SalesReturnDataAccess data = new SalesReturnDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of SalesReturn.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of SalesReturn</returns>
        public SalesReturnList GetAll(bool fillChild)
        {
			SalesReturnList salesReturnList = new SalesReturnList();
            using (SalesReturnDataAccess data = new SalesReturnDataAccess(ClientContext))
            {
                salesReturnList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (SalesReturn salesReturnObject in salesReturnList)
                {
					FillSalesReturnWithChilds(salesReturnObject, fillChild);
				}
			}
			return salesReturnList;
        }
		
		/// <summary>
        /// Retrieve list of SalesReturn  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of SalesReturn</returns>
        public SalesReturnList GetPaged(PagedRequest request)
        {
            using (SalesReturnDataAccess data = new SalesReturnDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of SalesReturn  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of SalesReturn</returns>
        public SalesReturnList GetByQuery(String query)
        {
            using (SalesReturnDataAccess data = new SalesReturnDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get SalesReturn Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of SalesReturn
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (SalesReturnDataAccess data = new SalesReturnDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get SalesReturn Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of SalesReturn
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (SalesReturnDataAccess data = new SalesReturnDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of SalesReturn By SalesOrderDetailId        
		/// <param name="_SalesOrderDetailId"></param>
        /// </summary>
        /// <returns>List of SalesReturn</returns>
        public SalesReturnList GetBySalesOrderDetailId(Int32 _SalesOrderDetailId)
        {
            using (SalesReturnDataAccess data = new SalesReturnDataAccess(ClientContext))
            {
                return data.GetBySalesOrderDetailId(_SalesOrderDetailId);
            }
        }
		
		
		#endregion
	}	
}