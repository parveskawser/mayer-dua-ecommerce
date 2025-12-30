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
    /// Business logic processing for SalesOrderHeader.
    /// </summary>    
	public partial class SalesOrderHeaderManager : BaseManager
	{
	
		#region Constructors
		public SalesOrderHeaderManager(ClientContext context) : base(context) { }
		public SalesOrderHeaderManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new salesOrderHeader.
        /// data manipulation for insertion of SalesOrderHeader
        /// </summary>
        /// <param name="salesOrderHeaderObject"></param>
        /// <returns></returns>
        private bool Insert(SalesOrderHeader salesOrderHeaderObject)
        {
            // new salesOrderHeader
            using (SalesOrderHeaderDataAccess data = new SalesOrderHeaderDataAccess(ClientContext))
            {
                // insert to salesOrderHeaderObject
                Int32 _Id = data.Insert(salesOrderHeaderObject);
                // if successful, process
                if (_Id > 0)
                {
                    salesOrderHeaderObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of SalesOrderHeader Object.
        /// Data manipulation processing for: new, deleted, updated SalesOrderHeader
        /// </summary>
        /// <param name="salesOrderHeaderObject"></param>
        /// <returns></returns>
        public bool UpdateBase(SalesOrderHeader salesOrderHeaderObject)
        {
            // use of switch for different types of DML
            switch (salesOrderHeaderObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(salesOrderHeaderObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(salesOrderHeaderObject.Id);
            }
            // update rows
            using (SalesOrderHeaderDataAccess data = new SalesOrderHeaderDataAccess(ClientContext))
            {
                return (data.Update(salesOrderHeaderObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for SalesOrderHeader
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (SalesOrderHeaderDataAccess data = new SalesOrderHeaderDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve SalesOrderHeader data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>SalesOrderHeader Object</returns>
        public SalesOrderHeader Get(Int32 _Id)
        {
            using (SalesOrderHeaderDataAccess data = new SalesOrderHeaderDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a SalesOrderHeader .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public SalesOrderHeader Get(Int32 _Id, bool fillChild)
        {
            SalesOrderHeader salesOrderHeaderObject;
            salesOrderHeaderObject = Get(_Id);
            
            if (salesOrderHeaderObject != null && fillChild)
            {
                // populate child data for a salesOrderHeaderObject
                FillSalesOrderHeaderWithChilds(salesOrderHeaderObject, fillChild);
            }

            return salesOrderHeaderObject;
        }
		
		/// <summary>
        /// populates a SalesOrderHeader with its child entities
        /// </summary>
        /// <param name="salesOrderHeader"></param>
		/// <param name="fillChilds"></param>
        private void FillSalesOrderHeaderWithChilds(SalesOrderHeader salesOrderHeaderObject, bool fillChilds)
        {
            // populate child data for a salesOrderHeaderObject
            if (salesOrderHeaderObject != null)
            {
				// Retrieve AddressIdObject as Address type for the SalesOrderHeader using AddressId
				using(AddressManager addressManager = new AddressManager(ClientContext))
				{
					salesOrderHeaderObject.AddressIdObject = addressManager.Get(salesOrderHeaderObject.AddressId, fillChilds);
				}
				// Retrieve SalesChannelIdObject as SalesChannel type for the SalesOrderHeader using SalesChannelId
				using(SalesChannelManager salesChannelManager = new SalesChannelManager(ClientContext))
				{
					salesOrderHeaderObject.SalesChannelIdObject = salesChannelManager.Get(salesOrderHeaderObject.SalesChannelId, fillChilds);
				}
				// Retrieve CompanyCustomerIdObject as CompanyCustomer type for the SalesOrderHeader using CompanyCustomerId
				using(CompanyCustomerManager companyCustomerManager = new CompanyCustomerManager(ClientContext))
				{
					salesOrderHeaderObject.CompanyCustomerIdObject = companyCustomerManager.Get(salesOrderHeaderObject.CompanyCustomerId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of SalesOrderHeader.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of SalesOrderHeader</returns>
        public SalesOrderHeaderList GetAll()
        {
            using (SalesOrderHeaderDataAccess data = new SalesOrderHeaderDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of SalesOrderHeader.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of SalesOrderHeader</returns>
        public SalesOrderHeaderList GetAll(bool fillChild)
        {
			SalesOrderHeaderList salesOrderHeaderList = new SalesOrderHeaderList();
            using (SalesOrderHeaderDataAccess data = new SalesOrderHeaderDataAccess(ClientContext))
            {
                salesOrderHeaderList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (SalesOrderHeader salesOrderHeaderObject in salesOrderHeaderList)
                {
					FillSalesOrderHeaderWithChilds(salesOrderHeaderObject, fillChild);
				}
			}
			return salesOrderHeaderList;
        }
		
		/// <summary>
        /// Retrieve list of SalesOrderHeader  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of SalesOrderHeader</returns>
        public SalesOrderHeaderList GetPaged(PagedRequest request)
        {
            using (SalesOrderHeaderDataAccess data = new SalesOrderHeaderDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of SalesOrderHeader  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of SalesOrderHeader</returns>
        public SalesOrderHeaderList GetByQuery(String query)
        {
            using (SalesOrderHeaderDataAccess data = new SalesOrderHeaderDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get SalesOrderHeader Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of SalesOrderHeader
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (SalesOrderHeaderDataAccess data = new SalesOrderHeaderDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get SalesOrderHeader Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of SalesOrderHeader
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (SalesOrderHeaderDataAccess data = new SalesOrderHeaderDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of SalesOrderHeader By CompanyCustomerId        
		/// <param name="_CompanyCustomerId"></param>
        /// </summary>
        /// <returns>List of SalesOrderHeader</returns>
        public SalesOrderHeaderList GetByCompanyCustomerId(Int32 _CompanyCustomerId)
        {
            using (SalesOrderHeaderDataAccess data = new SalesOrderHeaderDataAccess(ClientContext))
            {
                return data.GetByCompanyCustomerId(_CompanyCustomerId);
            }
        }
		
		/// <summary>
        /// Retrieve list of SalesOrderHeader By AddressId        
		/// <param name="_AddressId"></param>
        /// </summary>
        /// <returns>List of SalesOrderHeader</returns>
        public SalesOrderHeaderList GetByAddressId(Int32 _AddressId)
        {
            using (SalesOrderHeaderDataAccess data = new SalesOrderHeaderDataAccess(ClientContext))
            {
                return data.GetByAddressId(_AddressId);
            }
        }
		
		/// <summary>
        /// Retrieve list of SalesOrderHeader By SalesChannelId        
		/// <param name="_SalesChannelId"></param>
        /// </summary>
        /// <returns>List of SalesOrderHeader</returns>
        public SalesOrderHeaderList GetBySalesChannelId(Int32 _SalesChannelId)
        {
            using (SalesOrderHeaderDataAccess data = new SalesOrderHeaderDataAccess(ClientContext))
            {
                return data.GetBySalesChannelId(_SalesChannelId);
            }
        }
		
		
		#endregion
	}	
}