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
    /// Business logic processing for CompanyPaymentMethod.
    /// </summary>    
	public partial class CompanyPaymentMethodManager : BaseManager
	{
	
		#region Constructors
		public CompanyPaymentMethodManager(ClientContext context) : base(context) { }
		public CompanyPaymentMethodManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new companyPaymentMethod.
        /// data manipulation for insertion of CompanyPaymentMethod
        /// </summary>
        /// <param name="companyPaymentMethodObject"></param>
        /// <returns></returns>
        private bool Insert(CompanyPaymentMethod companyPaymentMethodObject)
        {
            // new companyPaymentMethod
            using (CompanyPaymentMethodDataAccess data = new CompanyPaymentMethodDataAccess(ClientContext))
            {
                // insert to companyPaymentMethodObject
                Int32 _Id = data.Insert(companyPaymentMethodObject);
                // if successful, process
                if (_Id > 0)
                {
                    companyPaymentMethodObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of CompanyPaymentMethod Object.
        /// Data manipulation processing for: new, deleted, updated CompanyPaymentMethod
        /// </summary>
        /// <param name="companyPaymentMethodObject"></param>
        /// <returns></returns>
        public bool UpdateBase(CompanyPaymentMethod companyPaymentMethodObject)
        {
            // use of switch for different types of DML
            switch (companyPaymentMethodObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(companyPaymentMethodObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(companyPaymentMethodObject.Id);
            }
            // update rows
            using (CompanyPaymentMethodDataAccess data = new CompanyPaymentMethodDataAccess(ClientContext))
            {
                return (data.Update(companyPaymentMethodObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for CompanyPaymentMethod
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (CompanyPaymentMethodDataAccess data = new CompanyPaymentMethodDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve CompanyPaymentMethod data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>CompanyPaymentMethod Object</returns>
        public CompanyPaymentMethod Get(Int32 _Id)
        {
            using (CompanyPaymentMethodDataAccess data = new CompanyPaymentMethodDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a CompanyPaymentMethod .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public CompanyPaymentMethod Get(Int32 _Id, bool fillChild)
        {
            CompanyPaymentMethod companyPaymentMethodObject;
            companyPaymentMethodObject = Get(_Id);
            
            if (companyPaymentMethodObject != null && fillChild)
            {
                // populate child data for a companyPaymentMethodObject
                FillCompanyPaymentMethodWithChilds(companyPaymentMethodObject, fillChild);
            }

            return companyPaymentMethodObject;
        }
		
		/// <summary>
        /// populates a CompanyPaymentMethod with its child entities
        /// </summary>
        /// <param name="companyPaymentMethod"></param>
		/// <param name="fillChilds"></param>
        private void FillCompanyPaymentMethodWithChilds(CompanyPaymentMethod companyPaymentMethodObject, bool fillChilds)
        {
            // populate child data for a companyPaymentMethodObject
            if (companyPaymentMethodObject != null)
            {
				// Retrieve CompanyIdObject as Company type for the CompanyPaymentMethod using CompanyId
				using(CompanyManager companyManager = new CompanyManager(ClientContext))
				{
					companyPaymentMethodObject.CompanyIdObject = companyManager.Get(companyPaymentMethodObject.CompanyId, fillChilds);
				}
				// Retrieve PaymentMethodIdObject as PaymentMethod type for the CompanyPaymentMethod using PaymentMethodId
				using(PaymentMethodManager paymentMethodManager = new PaymentMethodManager(ClientContext))
				{
					companyPaymentMethodObject.PaymentMethodIdObject = paymentMethodManager.Get(companyPaymentMethodObject.PaymentMethodId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of CompanyPaymentMethod.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of CompanyPaymentMethod</returns>
        public CompanyPaymentMethodList GetAll()
        {
            using (CompanyPaymentMethodDataAccess data = new CompanyPaymentMethodDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of CompanyPaymentMethod.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of CompanyPaymentMethod</returns>
        public CompanyPaymentMethodList GetAll(bool fillChild)
        {
			CompanyPaymentMethodList companyPaymentMethodList = new CompanyPaymentMethodList();
            using (CompanyPaymentMethodDataAccess data = new CompanyPaymentMethodDataAccess(ClientContext))
            {
                companyPaymentMethodList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (CompanyPaymentMethod companyPaymentMethodObject in companyPaymentMethodList)
                {
					FillCompanyPaymentMethodWithChilds(companyPaymentMethodObject, fillChild);
				}
			}
			return companyPaymentMethodList;
        }
		
		/// <summary>
        /// Retrieve list of CompanyPaymentMethod  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of CompanyPaymentMethod</returns>
        public CompanyPaymentMethodList GetPaged(PagedRequest request)
        {
            using (CompanyPaymentMethodDataAccess data = new CompanyPaymentMethodDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of CompanyPaymentMethod  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of CompanyPaymentMethod</returns>
        public CompanyPaymentMethodList GetByQuery(String query)
        {
            using (CompanyPaymentMethodDataAccess data = new CompanyPaymentMethodDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get CompanyPaymentMethod Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of CompanyPaymentMethod
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (CompanyPaymentMethodDataAccess data = new CompanyPaymentMethodDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get CompanyPaymentMethod Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of CompanyPaymentMethod
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (CompanyPaymentMethodDataAccess data = new CompanyPaymentMethodDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of CompanyPaymentMethod By CompanyId        
		/// <param name="_CompanyId"></param>
        /// </summary>
        /// <returns>List of CompanyPaymentMethod</returns>
        public CompanyPaymentMethodList GetByCompanyId(Int32 _CompanyId)
        {
            using (CompanyPaymentMethodDataAccess data = new CompanyPaymentMethodDataAccess(ClientContext))
            {
                return data.GetByCompanyId(_CompanyId);
            }
        }
		
		/// <summary>
        /// Retrieve list of CompanyPaymentMethod By PaymentMethodId        
		/// <param name="_PaymentMethodId"></param>
        /// </summary>
        /// <returns>List of CompanyPaymentMethod</returns>
        public CompanyPaymentMethodList GetByPaymentMethodId(Int32 _PaymentMethodId)
        {
            using (CompanyPaymentMethodDataAccess data = new CompanyPaymentMethodDataAccess(ClientContext))
            {
                return data.GetByPaymentMethodId(_PaymentMethodId);
            }
        }
		
		
		#endregion
	}	
}