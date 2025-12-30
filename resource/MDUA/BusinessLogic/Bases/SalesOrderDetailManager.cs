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
    /// Business logic processing for SalesOrderDetail.
    /// </summary>    
	public partial class SalesOrderDetailManager : BaseManager
	{
	
		#region Constructors
		public SalesOrderDetailManager(ClientContext context) : base(context) { }
		public SalesOrderDetailManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new salesOrderDetail.
        /// data manipulation for insertion of SalesOrderDetail
        /// </summary>
        /// <param name="salesOrderDetailObject"></param>
        /// <returns></returns>
        private bool Insert(SalesOrderDetail salesOrderDetailObject)
        {
            // new salesOrderDetail
            using (SalesOrderDetailDataAccess data = new SalesOrderDetailDataAccess(ClientContext))
            {
                // insert to salesOrderDetailObject
                Int32 _Id = data.Insert(salesOrderDetailObject);
                // if successful, process
                if (_Id > 0)
                {
                    salesOrderDetailObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of SalesOrderDetail Object.
        /// Data manipulation processing for: new, deleted, updated SalesOrderDetail
        /// </summary>
        /// <param name="salesOrderDetailObject"></param>
        /// <returns></returns>
        public bool UpdateBase(SalesOrderDetail salesOrderDetailObject)
        {
            // use of switch for different types of DML
            switch (salesOrderDetailObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(salesOrderDetailObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(salesOrderDetailObject.Id);
            }
            // update rows
            using (SalesOrderDetailDataAccess data = new SalesOrderDetailDataAccess(ClientContext))
            {
                return (data.Update(salesOrderDetailObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for SalesOrderDetail
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (SalesOrderDetailDataAccess data = new SalesOrderDetailDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve SalesOrderDetail data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>SalesOrderDetail Object</returns>
        public SalesOrderDetail Get(Int32 _Id)
        {
            using (SalesOrderDetailDataAccess data = new SalesOrderDetailDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a SalesOrderDetail .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public SalesOrderDetail Get(Int32 _Id, bool fillChild)
        {
            SalesOrderDetail salesOrderDetailObject;
            salesOrderDetailObject = Get(_Id);
            
            if (salesOrderDetailObject != null && fillChild)
            {
                // populate child data for a salesOrderDetailObject
                FillSalesOrderDetailWithChilds(salesOrderDetailObject, fillChild);
            }

            return salesOrderDetailObject;
        }
		
		/// <summary>
        /// populates a SalesOrderDetail with its child entities
        /// </summary>
        /// <param name="salesOrderDetail"></param>
		/// <param name="fillChilds"></param>
        private void FillSalesOrderDetailWithChilds(SalesOrderDetail salesOrderDetailObject, bool fillChilds)
        {
            // populate child data for a salesOrderDetailObject
            if (salesOrderDetailObject != null)
            {
				// Retrieve SalesOrderIdObject as SalesOrderHeader type for the SalesOrderDetail using SalesOrderId
				using(SalesOrderHeaderManager salesOrderHeaderManager = new SalesOrderHeaderManager(ClientContext))
				{
					salesOrderDetailObject.SalesOrderIdObject = salesOrderHeaderManager.Get(salesOrderDetailObject.SalesOrderId, fillChilds);
				}
				// Retrieve ProductIdObject as ProductVariant type for the SalesOrderDetail using ProductId
				using(ProductVariantManager productVariantManager = new ProductVariantManager(ClientContext))
				{
					salesOrderDetailObject.ProductIdObject = productVariantManager.Get(salesOrderDetailObject.ProductId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of SalesOrderDetail.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of SalesOrderDetail</returns>
        public SalesOrderDetailList GetAll()
        {
            using (SalesOrderDetailDataAccess data = new SalesOrderDetailDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of SalesOrderDetail.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of SalesOrderDetail</returns>
        public SalesOrderDetailList GetAll(bool fillChild)
        {
			SalesOrderDetailList salesOrderDetailList = new SalesOrderDetailList();
            using (SalesOrderDetailDataAccess data = new SalesOrderDetailDataAccess(ClientContext))
            {
                salesOrderDetailList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (SalesOrderDetail salesOrderDetailObject in salesOrderDetailList)
                {
					FillSalesOrderDetailWithChilds(salesOrderDetailObject, fillChild);
				}
			}
			return salesOrderDetailList;
        }
		
		/// <summary>
        /// Retrieve list of SalesOrderDetail  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of SalesOrderDetail</returns>
        public SalesOrderDetailList GetPaged(PagedRequest request)
        {
            using (SalesOrderDetailDataAccess data = new SalesOrderDetailDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of SalesOrderDetail  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of SalesOrderDetail</returns>
        public SalesOrderDetailList GetByQuery(String query)
        {
            using (SalesOrderDetailDataAccess data = new SalesOrderDetailDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get SalesOrderDetail Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of SalesOrderDetail
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (SalesOrderDetailDataAccess data = new SalesOrderDetailDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get SalesOrderDetail Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of SalesOrderDetail
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (SalesOrderDetailDataAccess data = new SalesOrderDetailDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of SalesOrderDetail By SalesOrderId        
		/// <param name="_SalesOrderId"></param>
        /// </summary>
        /// <returns>List of SalesOrderDetail</returns>
        public SalesOrderDetailList GetBySalesOrderId(Int32 _SalesOrderId)
        {
            using (SalesOrderDetailDataAccess data = new SalesOrderDetailDataAccess(ClientContext))
            {
                return data.GetBySalesOrderId(_SalesOrderId);
            }
        }
		
		/// <summary>
        /// Retrieve list of SalesOrderDetail By ProductId        
		/// <param name="_ProductId"></param>
        /// </summary>
        /// <returns>List of SalesOrderDetail</returns>
        public SalesOrderDetailList GetByProductId(Int32 _ProductId)
        {
            using (SalesOrderDetailDataAccess data = new SalesOrderDetailDataAccess(ClientContext))
            {
                return data.GetByProductId(_ProductId);
            }
        }
		
		
		#endregion
	}	
}