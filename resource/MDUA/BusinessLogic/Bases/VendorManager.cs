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
    /// Business logic processing for Vendor.
    /// </summary>    
	public partial class VendorManager : BaseManager
	{
	
		#region Constructors
		public VendorManager(ClientContext context) : base(context) { }
		public VendorManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new vendor.
        /// data manipulation for insertion of Vendor
        /// </summary>
        /// <param name="vendorObject"></param>
        /// <returns></returns>
        private bool Insert(Vendor vendorObject)
        {
            // new vendor
            using (VendorDataAccess data = new VendorDataAccess(ClientContext))
            {
                // insert to vendorObject
                Int32 _Id = data.Insert(vendorObject);
                // if successful, process
                if (_Id > 0)
                {
                    vendorObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of Vendor Object.
        /// Data manipulation processing for: new, deleted, updated Vendor
        /// </summary>
        /// <param name="vendorObject"></param>
        /// <returns></returns>
        public bool UpdateBase(Vendor vendorObject)
        {
            // use of switch for different types of DML
            switch (vendorObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(vendorObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(vendorObject.Id);
            }
            // update rows
            using (VendorDataAccess data = new VendorDataAccess(ClientContext))
            {
                return (data.Update(vendorObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for Vendor
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (VendorDataAccess data = new VendorDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve Vendor data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>Vendor Object</returns>
        public Vendor Get(Int32 _Id)
        {
            using (VendorDataAccess data = new VendorDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a Vendor .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public Vendor Get(Int32 _Id, bool fillChild)
        {
            Vendor vendorObject;
            vendorObject = Get(_Id);
            
            if (vendorObject != null && fillChild)
            {
                // populate child data for a vendorObject
                FillVendorWithChilds(vendorObject, fillChild);
            }

            return vendorObject;
        }
		
		/// <summary>
        /// populates a Vendor with its child entities
        /// </summary>
        /// <param name="vendor"></param>
		/// <param name="fillChilds"></param>
        private void FillVendorWithChilds(Vendor vendorObject, bool fillChilds)
        {
            // populate child data for a vendorObject
            if (vendorObject != null)
            {
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of Vendor.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of Vendor</returns>
        public VendorList GetAll()
        {
            using (VendorDataAccess data = new VendorDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of Vendor.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of Vendor</returns>
        public VendorList GetAll(bool fillChild)
        {
			VendorList vendorList = new VendorList();
            using (VendorDataAccess data = new VendorDataAccess(ClientContext))
            {
                vendorList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (Vendor vendorObject in vendorList)
                {
					FillVendorWithChilds(vendorObject, fillChild);
				}
			}
			return vendorList;
        }
		
		/// <summary>
        /// Retrieve list of Vendor  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of Vendor</returns>
        public VendorList GetPaged(PagedRequest request)
        {
            using (VendorDataAccess data = new VendorDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of Vendor  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of Vendor</returns>
        public VendorList GetByQuery(String query)
        {
            using (VendorDataAccess data = new VendorDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get Vendor Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Vendor
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (VendorDataAccess data = new VendorDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get Vendor Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Vendor
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (VendorDataAccess data = new VendorDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		
		#endregion
	}	
}