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
    /// Business logic processing for SalesChannel.
    /// </summary>    
	public partial class SalesChannelManager : BaseManager
	{
	
		#region Constructors
		public SalesChannelManager(ClientContext context) : base(context) { }
		public SalesChannelManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new salesChannel.
        /// data manipulation for insertion of SalesChannel
        /// </summary>
        /// <param name="salesChannelObject"></param>
        /// <returns></returns>
        private bool Insert(SalesChannel salesChannelObject)
        {
            // new salesChannel
            using (SalesChannelDataAccess data = new SalesChannelDataAccess(ClientContext))
            {
                // insert to salesChannelObject
                Int32 _Id = data.Insert(salesChannelObject);
                // if successful, process
                if (_Id > 0)
                {
                    salesChannelObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of SalesChannel Object.
        /// Data manipulation processing for: new, deleted, updated SalesChannel
        /// </summary>
        /// <param name="salesChannelObject"></param>
        /// <returns></returns>
        public bool UpdateBase(SalesChannel salesChannelObject)
        {
            // use of switch for different types of DML
            switch (salesChannelObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(salesChannelObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(salesChannelObject.Id);
            }
            // update rows
            using (SalesChannelDataAccess data = new SalesChannelDataAccess(ClientContext))
            {
                return (data.Update(salesChannelObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for SalesChannel
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (SalesChannelDataAccess data = new SalesChannelDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve SalesChannel data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>SalesChannel Object</returns>
        public SalesChannel Get(Int32 _Id)
        {
            using (SalesChannelDataAccess data = new SalesChannelDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a SalesChannel .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public SalesChannel Get(Int32 _Id, bool fillChild)
        {
            SalesChannel salesChannelObject;
            salesChannelObject = Get(_Id);
            
            if (salesChannelObject != null && fillChild)
            {
                // populate child data for a salesChannelObject
                FillSalesChannelWithChilds(salesChannelObject, fillChild);
            }

            return salesChannelObject;
        }
		
		/// <summary>
        /// populates a SalesChannel with its child entities
        /// </summary>
        /// <param name="salesChannel"></param>
		/// <param name="fillChilds"></param>
        private void FillSalesChannelWithChilds(SalesChannel salesChannelObject, bool fillChilds)
        {
            // populate child data for a salesChannelObject
            if (salesChannelObject != null)
            {
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of SalesChannel.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of SalesChannel</returns>
        public SalesChannelList GetAll()
        {
            using (SalesChannelDataAccess data = new SalesChannelDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of SalesChannel.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of SalesChannel</returns>
        public SalesChannelList GetAll(bool fillChild)
        {
			SalesChannelList salesChannelList = new SalesChannelList();
            using (SalesChannelDataAccess data = new SalesChannelDataAccess(ClientContext))
            {
                salesChannelList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (SalesChannel salesChannelObject in salesChannelList)
                {
					FillSalesChannelWithChilds(salesChannelObject, fillChild);
				}
			}
			return salesChannelList;
        }
		
		/// <summary>
        /// Retrieve list of SalesChannel  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of SalesChannel</returns>
        public SalesChannelList GetPaged(PagedRequest request)
        {
            using (SalesChannelDataAccess data = new SalesChannelDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of SalesChannel  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of SalesChannel</returns>
        public SalesChannelList GetByQuery(String query)
        {
            using (SalesChannelDataAccess data = new SalesChannelDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get SalesChannel Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of SalesChannel
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (SalesChannelDataAccess data = new SalesChannelDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get SalesChannel Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of SalesChannel
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (SalesChannelDataAccess data = new SalesChannelDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		
		#endregion
	}	
}