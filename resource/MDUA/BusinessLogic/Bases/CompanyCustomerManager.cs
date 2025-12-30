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
    /// Business logic processing for CompanyCustomer.
    /// </summary>    
	public partial class CompanyCustomerManager : BaseManager
	{
	
		#region Constructors
		public CompanyCustomerManager(ClientContext context) : base(context) { }
		public CompanyCustomerManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new companyCustomer.
        /// data manipulation for insertion of CompanyCustomer
        /// </summary>
        /// <param name="companyCustomerObject"></param>
        /// <returns></returns>
        private bool Insert(CompanyCustomer companyCustomerObject)
        {
            // new companyCustomer
            using (CompanyCustomerDataAccess data = new CompanyCustomerDataAccess(ClientContext))
            {
                // insert to companyCustomerObject
                Int32 _Id = data.Insert(companyCustomerObject);
                // if successful, process
                if (_Id > 0)
                {
                    companyCustomerObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of CompanyCustomer Object.
        /// Data manipulation processing for: new, deleted, updated CompanyCustomer
        /// </summary>
        /// <param name="companyCustomerObject"></param>
        /// <returns></returns>
        public bool UpdateBase(CompanyCustomer companyCustomerObject)
        {
            // use of switch for different types of DML
            switch (companyCustomerObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(companyCustomerObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(companyCustomerObject.Id);
            }
            // update rows
            using (CompanyCustomerDataAccess data = new CompanyCustomerDataAccess(ClientContext))
            {
                return (data.Update(companyCustomerObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for CompanyCustomer
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (CompanyCustomerDataAccess data = new CompanyCustomerDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve CompanyCustomer data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>CompanyCustomer Object</returns>
        public CompanyCustomer Get(Int32 _Id)
        {
            using (CompanyCustomerDataAccess data = new CompanyCustomerDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a CompanyCustomer .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public CompanyCustomer Get(Int32 _Id, bool fillChild)
        {
            CompanyCustomer companyCustomerObject;
            companyCustomerObject = Get(_Id);
            
            if (companyCustomerObject != null && fillChild)
            {
                // populate child data for a companyCustomerObject
                FillCompanyCustomerWithChilds(companyCustomerObject, fillChild);
            }

            return companyCustomerObject;
        }
		
		/// <summary>
        /// populates a CompanyCustomer with its child entities
        /// </summary>
        /// <param name="companyCustomer"></param>
		/// <param name="fillChilds"></param>
        private void FillCompanyCustomerWithChilds(CompanyCustomer companyCustomerObject, bool fillChilds)
        {
            // populate child data for a companyCustomerObject
            if (companyCustomerObject != null)
            {
				// Retrieve CompanyIdObject as Company type for the CompanyCustomer using CompanyId
				using(CompanyManager companyManager = new CompanyManager(ClientContext))
				{
					companyCustomerObject.CompanyIdObject = companyManager.Get(companyCustomerObject.CompanyId, fillChilds);
				}
				// Retrieve CustomerIdObject as Customer type for the CompanyCustomer using CustomerId
				using(CustomerManager customerManager = new CustomerManager(ClientContext))
				{
					companyCustomerObject.CustomerIdObject = customerManager.Get(companyCustomerObject.CustomerId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of CompanyCustomer.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of CompanyCustomer</returns>
        public CompanyCustomerList GetAll()
        {
            using (CompanyCustomerDataAccess data = new CompanyCustomerDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of CompanyCustomer.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of CompanyCustomer</returns>
        public CompanyCustomerList GetAll(bool fillChild)
        {
			CompanyCustomerList companyCustomerList = new CompanyCustomerList();
            using (CompanyCustomerDataAccess data = new CompanyCustomerDataAccess(ClientContext))
            {
                companyCustomerList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (CompanyCustomer companyCustomerObject in companyCustomerList)
                {
					FillCompanyCustomerWithChilds(companyCustomerObject, fillChild);
				}
			}
			return companyCustomerList;
        }
		
		/// <summary>
        /// Retrieve list of CompanyCustomer  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of CompanyCustomer</returns>
        public CompanyCustomerList GetPaged(PagedRequest request)
        {
            using (CompanyCustomerDataAccess data = new CompanyCustomerDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of CompanyCustomer  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of CompanyCustomer</returns>
        public CompanyCustomerList GetByQuery(String query)
        {
            using (CompanyCustomerDataAccess data = new CompanyCustomerDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get CompanyCustomer Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of CompanyCustomer
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (CompanyCustomerDataAccess data = new CompanyCustomerDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get CompanyCustomer Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of CompanyCustomer
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (CompanyCustomerDataAccess data = new CompanyCustomerDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of CompanyCustomer By CompanyId        
		/// <param name="_CompanyId"></param>
        /// </summary>
        /// <returns>List of CompanyCustomer</returns>
        public CompanyCustomerList GetByCompanyId(Int32 _CompanyId)
        {
            using (CompanyCustomerDataAccess data = new CompanyCustomerDataAccess(ClientContext))
            {
                return data.GetByCompanyId(_CompanyId);
            }
        }
		
		/// <summary>
        /// Retrieve list of CompanyCustomer By CustomerId        
		/// <param name="_CustomerId"></param>
        /// </summary>
        /// <returns>List of CompanyCustomer</returns>
        public CompanyCustomerList GetByCustomerId(Int32 _CustomerId)
        {
            using (CompanyCustomerDataAccess data = new CompanyCustomerDataAccess(ClientContext))
            {
                return data.GetByCustomerId(_CustomerId);
            }
        }
		
		
		#endregion
	}	
}