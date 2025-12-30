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
    /// Business logic processing for AccountType.
    /// </summary>    
	public partial class AccountTypeManager : BaseManager
	{
	
		#region Constructors
		public AccountTypeManager(ClientContext context) : base(context) { }
		public AccountTypeManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new accountType.
        /// data manipulation for insertion of AccountType
        /// </summary>
        /// <param name="accountTypeObject"></param>
        /// <returns></returns>
        private bool Insert(AccountType accountTypeObject)
        {
            // new accountType
            using (AccountTypeDataAccess data = new AccountTypeDataAccess(ClientContext))
            {
                // insert to accountTypeObject
                Int32 _Id = data.Insert(accountTypeObject);
                // if successful, process
                if (_Id > 0)
                {
                    accountTypeObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of AccountType Object.
        /// Data manipulation processing for: new, deleted, updated AccountType
        /// </summary>
        /// <param name="accountTypeObject"></param>
        /// <returns></returns>
        public bool UpdateBase(AccountType accountTypeObject)
        {
            // use of switch for different types of DML
            switch (accountTypeObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(accountTypeObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(accountTypeObject.Id);
            }
            // update rows
            using (AccountTypeDataAccess data = new AccountTypeDataAccess(ClientContext))
            {
                return (data.Update(accountTypeObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for AccountType
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (AccountTypeDataAccess data = new AccountTypeDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve AccountType data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>AccountType Object</returns>
        public AccountType Get(Int32 _Id)
        {
            using (AccountTypeDataAccess data = new AccountTypeDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a AccountType .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public AccountType Get(Int32 _Id, bool fillChild)
        {
            AccountType accountTypeObject;
            accountTypeObject = Get(_Id);
            
            if (accountTypeObject != null && fillChild)
            {
                // populate child data for a accountTypeObject
                FillAccountTypeWithChilds(accountTypeObject, fillChild);
            }

            return accountTypeObject;
        }
		
		/// <summary>
        /// populates a AccountType with its child entities
        /// </summary>
        /// <param name="accountType"></param>
		/// <param name="fillChilds"></param>
        private void FillAccountTypeWithChilds(AccountType accountTypeObject, bool fillChilds)
        {
            // populate child data for a accountTypeObject
            if (accountTypeObject != null)
            {
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of AccountType.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of AccountType</returns>
        public AccountTypeList GetAll()
        {
            using (AccountTypeDataAccess data = new AccountTypeDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of AccountType.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of AccountType</returns>
        public AccountTypeList GetAll(bool fillChild)
        {
			AccountTypeList accountTypeList = new AccountTypeList();
            using (AccountTypeDataAccess data = new AccountTypeDataAccess(ClientContext))
            {
                accountTypeList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (AccountType accountTypeObject in accountTypeList)
                {
					FillAccountTypeWithChilds(accountTypeObject, fillChild);
				}
			}
			return accountTypeList;
        }
		
		/// <summary>
        /// Retrieve list of AccountType  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of AccountType</returns>
        public AccountTypeList GetPaged(PagedRequest request)
        {
            using (AccountTypeDataAccess data = new AccountTypeDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of AccountType  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of AccountType</returns>
        public AccountTypeList GetByQuery(String query)
        {
            using (AccountTypeDataAccess data = new AccountTypeDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get AccountType Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of AccountType
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (AccountTypeDataAccess data = new AccountTypeDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get AccountType Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of AccountType
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (AccountTypeDataAccess data = new AccountTypeDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		
		#endregion
	}	
}