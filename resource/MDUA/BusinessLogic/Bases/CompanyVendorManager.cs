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
    /// Business logic processing for CompanyVendor.
    /// </summary>    
	public partial class CompanyVendorManager : BaseManager
	{
	
		#region Constructors
		public CompanyVendorManager(ClientContext context) : base(context) { }
		public CompanyVendorManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new companyVendor.
        /// data manipulation for insertion of CompanyVendor
        /// </summary>
        /// <param name="companyVendorObject"></param>
        /// <returns></returns>
        private bool Insert(CompanyVendor companyVendorObject)
        {
            // new companyVendor
            using (CompanyVendorDataAccess data = new CompanyVendorDataAccess(ClientContext))
            {
                // insert to companyVendorObject
                Int32 _Id = data.Insert(companyVendorObject);
                // if successful, process
                if (_Id > 0)
                {
                    companyVendorObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of CompanyVendor Object.
        /// Data manipulation processing for: new, deleted, updated CompanyVendor
        /// </summary>
        /// <param name="companyVendorObject"></param>
        /// <returns></returns>
        public bool UpdateBase(CompanyVendor companyVendorObject)
        {
            // use of switch for different types of DML
            switch (companyVendorObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(companyVendorObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(companyVendorObject.Id);
            }
            // update rows
            using (CompanyVendorDataAccess data = new CompanyVendorDataAccess(ClientContext))
            {
                return (data.Update(companyVendorObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for CompanyVendor
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (CompanyVendorDataAccess data = new CompanyVendorDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve CompanyVendor data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>CompanyVendor Object</returns>
        public CompanyVendor Get(Int32 _Id)
        {
            using (CompanyVendorDataAccess data = new CompanyVendorDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a CompanyVendor .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public CompanyVendor Get(Int32 _Id, bool fillChild)
        {
            CompanyVendor companyVendorObject;
            companyVendorObject = Get(_Id);
            
            if (companyVendorObject != null && fillChild)
            {
                // populate child data for a companyVendorObject
                FillCompanyVendorWithChilds(companyVendorObject, fillChild);
            }

            return companyVendorObject;
        }
		
		/// <summary>
        /// populates a CompanyVendor with its child entities
        /// </summary>
        /// <param name="companyVendor"></param>
		/// <param name="fillChilds"></param>
        private void FillCompanyVendorWithChilds(CompanyVendor companyVendorObject, bool fillChilds)
        {
            // populate child data for a companyVendorObject
            if (companyVendorObject != null)
            {
				// Retrieve CompanyIdObject as Company type for the CompanyVendor using CompanyId
				using(CompanyManager companyManager = new CompanyManager(ClientContext))
				{
					companyVendorObject.CompanyIdObject = companyManager.Get(companyVendorObject.CompanyId, fillChilds);
				}
				// Retrieve VendorIdObject as Vendor type for the CompanyVendor using VendorId
				using(VendorManager vendorManager = new VendorManager(ClientContext))
				{
					companyVendorObject.VendorIdObject = vendorManager.Get(companyVendorObject.VendorId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of CompanyVendor.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of CompanyVendor</returns>
        public CompanyVendorList GetAll()
        {
            using (CompanyVendorDataAccess data = new CompanyVendorDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of CompanyVendor.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of CompanyVendor</returns>
        public CompanyVendorList GetAll(bool fillChild)
        {
			CompanyVendorList companyVendorList = new CompanyVendorList();
            using (CompanyVendorDataAccess data = new CompanyVendorDataAccess(ClientContext))
            {
                companyVendorList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (CompanyVendor companyVendorObject in companyVendorList)
                {
					FillCompanyVendorWithChilds(companyVendorObject, fillChild);
				}
			}
			return companyVendorList;
        }
		
		/// <summary>
        /// Retrieve list of CompanyVendor  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of CompanyVendor</returns>
        public CompanyVendorList GetPaged(PagedRequest request)
        {
            using (CompanyVendorDataAccess data = new CompanyVendorDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of CompanyVendor  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of CompanyVendor</returns>
        public CompanyVendorList GetByQuery(String query)
        {
            using (CompanyVendorDataAccess data = new CompanyVendorDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get CompanyVendor Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of CompanyVendor
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (CompanyVendorDataAccess data = new CompanyVendorDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get CompanyVendor Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of CompanyVendor
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (CompanyVendorDataAccess data = new CompanyVendorDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of CompanyVendor By CompanyId        
		/// <param name="_CompanyId"></param>
        /// </summary>
        /// <returns>List of CompanyVendor</returns>
        public CompanyVendorList GetByCompanyId(Int32 _CompanyId)
        {
            using (CompanyVendorDataAccess data = new CompanyVendorDataAccess(ClientContext))
            {
                return data.GetByCompanyId(_CompanyId);
            }
        }
		
		/// <summary>
        /// Retrieve list of CompanyVendor By VendorId        
		/// <param name="_VendorId"></param>
        /// </summary>
        /// <returns>List of CompanyVendor</returns>
        public CompanyVendorList GetByVendorId(Int32 _VendorId)
        {
            using (CompanyVendorDataAccess data = new CompanyVendorDataAccess(ClientContext))
            {
                return data.GetByVendorId(_VendorId);
            }
        }
		
		
		#endregion
	}	
}