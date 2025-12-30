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
    /// Business logic processing for Account.
    /// </summary>    
	public partial class AccountManager : BaseManager
	{
	
		#region Constructors
		public AccountManager(ClientContext context) : base(context) { }
		public AccountManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new account.
        /// data manipulation for insertion of Account
        /// </summary>
        /// <param name="accountObject"></param>
        /// <returns></returns>
        private bool Insert(Account accountObject)
        {
            // new account
            using (AccountDataAccess data = new AccountDataAccess(ClientContext))
            {
                // insert to accountObject
                Int32 _Id = data.Insert(accountObject);
                // if successful, process
                if (_Id > 0)
                {
                    accountObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of Account Object.
        /// Data manipulation processing for: new, deleted, updated Account
        /// </summary>
        /// <param name="accountObject"></param>
        /// <returns></returns>
        public bool UpdateBase(Account accountObject)
        {
            // use of switch for different types of DML
            switch (accountObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(accountObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(accountObject.Id);
            }
            // update rows
            using (AccountDataAccess data = new AccountDataAccess(ClientContext))
            {
                return (data.Update(accountObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for Account
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (AccountDataAccess data = new AccountDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve Account data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>Account Object</returns>
        public Account Get(Int32 _Id)
        {
            using (AccountDataAccess data = new AccountDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a Account .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public Account Get(Int32 _Id, bool fillChild)
        {
            Account accountObject;
            accountObject = Get(_Id);
            
            if (accountObject != null && fillChild)
            {
                // populate child data for a accountObject
                FillAccountWithChilds(accountObject, fillChild);
            }

            return accountObject;
        }
		
		/// <summary>
        /// populates a Account with its child entities
        /// </summary>
        /// <param name="account"></param>
		/// <param name="fillChilds"></param>
        private void FillAccountWithChilds(Account accountObject, bool fillChilds)
        {
            // populate child data for a accountObject
            if (accountObject != null)
            {
				// Retrieve AccountTypeIdObject as AccountType type for the Account using AccountTypeId
				using(AccountTypeManager accountTypeManager = new AccountTypeManager(ClientContext))
				{
					accountObject.AccountTypeIdObject = accountTypeManager.Get(accountObject.AccountTypeId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of Account.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of Account</returns>
        public AccountList GetAll()
        {
            using (AccountDataAccess data = new AccountDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of Account.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of Account</returns>
        public AccountList GetAll(bool fillChild)
        {
			AccountList accountList = new AccountList();
            using (AccountDataAccess data = new AccountDataAccess(ClientContext))
            {
                accountList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (Account accountObject in accountList)
                {
					FillAccountWithChilds(accountObject, fillChild);
				}
			}
			return accountList;
        }
		
		/// <summary>
        /// Retrieve list of Account  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of Account</returns>
        public AccountList GetPaged(PagedRequest request)
        {
            using (AccountDataAccess data = new AccountDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of Account  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of Account</returns>
        public AccountList GetByQuery(String query)
        {
            using (AccountDataAccess data = new AccountDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get Account Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Account
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (AccountDataAccess data = new AccountDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get Account Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Account
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (AccountDataAccess data = new AccountDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of Account By AccountTypeId        
		/// <param name="_AccountTypeId"></param>
        /// </summary>
        /// <returns>List of Account</returns>
        public AccountList GetByAccountTypeId(Int32 _AccountTypeId)
        {
            using (AccountDataAccess data = new AccountDataAccess(ClientContext))
            {
                return data.GetByAccountTypeId(_AccountTypeId);
            }
        }
		
		
		#endregion
	}	
}