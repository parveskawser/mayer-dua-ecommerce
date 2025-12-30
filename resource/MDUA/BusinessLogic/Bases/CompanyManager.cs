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
    /// Business logic processing for Company.
    /// </summary>    
	public partial class CompanyManager : BaseManager
	{
	
		#region Constructors
		public CompanyManager(ClientContext context) : base(context) { }
		public CompanyManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new company.
        /// data manipulation for insertion of Company
        /// </summary>
        /// <param name="companyObject"></param>
        /// <returns></returns>
        private bool Insert(Company companyObject)
        {
            // new company
            using (CompanyDataAccess data = new CompanyDataAccess(ClientContext))
            {
                // insert to companyObject
                Int32 _Id = data.Insert(companyObject);
                // if successful, process
                if (_Id > 0)
                {
                    companyObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of Company Object.
        /// Data manipulation processing for: new, deleted, updated Company
        /// </summary>
        /// <param name="companyObject"></param>
        /// <returns></returns>
        public bool UpdateBase(Company companyObject)
        {
            // use of switch for different types of DML
            switch (companyObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(companyObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(companyObject.Id);
            }
            // update rows
            using (CompanyDataAccess data = new CompanyDataAccess(ClientContext))
            {
                return (data.Update(companyObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for Company
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (CompanyDataAccess data = new CompanyDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve Company data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>Company Object</returns>
        public Company Get(Int32 _Id)
        {
            using (CompanyDataAccess data = new CompanyDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a Company .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public Company Get(Int32 _Id, bool fillChild)
        {
            Company companyObject;
            companyObject = Get(_Id);
            
            if (companyObject != null && fillChild)
            {
                // populate child data for a companyObject
                FillCompanyWithChilds(companyObject, fillChild);
            }

            return companyObject;
        }
		
		/// <summary>
        /// populates a Company with its child entities
        /// </summary>
        /// <param name="company"></param>
		/// <param name="fillChilds"></param>
        private void FillCompanyWithChilds(Company companyObject, bool fillChilds)
        {
            // populate child data for a companyObject
            if (companyObject != null)
            {
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of Company.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of Company</returns>
        public CompanyList GetAll()
        {
            using (CompanyDataAccess data = new CompanyDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of Company.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of Company</returns>
        public CompanyList GetAll(bool fillChild)
        {
			CompanyList companyList = new CompanyList();
            using (CompanyDataAccess data = new CompanyDataAccess(ClientContext))
            {
                companyList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (Company companyObject in companyList)
                {
					FillCompanyWithChilds(companyObject, fillChild);
				}
			}
			return companyList;
        }
		
		/// <summary>
        /// Retrieve list of Company  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of Company</returns>
        public CompanyList GetPaged(PagedRequest request)
        {
            using (CompanyDataAccess data = new CompanyDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of Company  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of Company</returns>
        public CompanyList GetByQuery(String query)
        {
            using (CompanyDataAccess data = new CompanyDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get Company Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Company
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (CompanyDataAccess data = new CompanyDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get Company Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Company
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (CompanyDataAccess data = new CompanyDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		
		#endregion
	}	
}