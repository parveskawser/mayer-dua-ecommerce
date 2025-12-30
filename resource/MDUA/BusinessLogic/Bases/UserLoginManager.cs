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
    /// Business logic processing for UserLogin.
    /// </summary>    
	public partial class UserLoginManager : BaseManager
	{
	
		#region Constructors
		public UserLoginManager(ClientContext context) : base(context) { }
		public UserLoginManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new userLogin.
        /// data manipulation for insertion of UserLogin
        /// </summary>
        /// <param name="userLoginObject"></param>
        /// <returns></returns>
        private bool Insert(UserLogin userLoginObject)
        {
            // new userLogin
            using (UserLoginDataAccess data = new UserLoginDataAccess(ClientContext))
            {
                // insert to userLoginObject
                Int32 _Id = data.Insert(userLoginObject);
                // if successful, process
                if (_Id > 0)
                {
                    userLoginObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of UserLogin Object.
        /// Data manipulation processing for: new, deleted, updated UserLogin
        /// </summary>
        /// <param name="userLoginObject"></param>
        /// <returns></returns>
        public bool UpdateBase(UserLogin userLoginObject)
        {
            // use of switch for different types of DML
            switch (userLoginObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(userLoginObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(userLoginObject.Id);
            }
            // update rows
            using (UserLoginDataAccess data = new UserLoginDataAccess(ClientContext))
            {
                return (data.Update(userLoginObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for UserLogin
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (UserLoginDataAccess data = new UserLoginDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve UserLogin data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>UserLogin Object</returns>
        public UserLogin Get(Int32 _Id)
        {
            using (UserLoginDataAccess data = new UserLoginDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a UserLogin .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public UserLogin Get(Int32 _Id, bool fillChild)
        {
            UserLogin userLoginObject;
            userLoginObject = Get(_Id);
            
            if (userLoginObject != null && fillChild)
            {
                // populate child data for a userLoginObject
                FillUserLoginWithChilds(userLoginObject, fillChild);
            }

            return userLoginObject;
        }
		
		/// <summary>
        /// populates a UserLogin with its child entities
        /// </summary>
        /// <param name="userLogin"></param>
		/// <param name="fillChilds"></param>
        private void FillUserLoginWithChilds(UserLogin userLoginObject, bool fillChilds)
        {
            // populate child data for a userLoginObject
            if (userLoginObject != null)
            {
				// Retrieve CompanyIdObject as Company type for the UserLogin using CompanyId
				using(CompanyManager companyManager = new CompanyManager(ClientContext))
				{
					userLoginObject.CompanyIdObject = companyManager.Get(userLoginObject.CompanyId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of UserLogin.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of UserLogin</returns>
        public UserLoginList GetAll()
        {
            using (UserLoginDataAccess data = new UserLoginDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of UserLogin.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of UserLogin</returns>
        public UserLoginList GetAll(bool fillChild)
        {
			UserLoginList userLoginList = new UserLoginList();
            using (UserLoginDataAccess data = new UserLoginDataAccess(ClientContext))
            {
                userLoginList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (UserLogin userLoginObject in userLoginList)
                {
					FillUserLoginWithChilds(userLoginObject, fillChild);
				}
			}
			return userLoginList;
        }
		
		/// <summary>
        /// Retrieve list of UserLogin  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of UserLogin</returns>
        public UserLoginList GetPaged(PagedRequest request)
        {
            using (UserLoginDataAccess data = new UserLoginDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of UserLogin  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of UserLogin</returns>
        public UserLoginList GetByQuery(String query)
        {
            using (UserLoginDataAccess data = new UserLoginDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get UserLogin Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of UserLogin
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (UserLoginDataAccess data = new UserLoginDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get UserLogin Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of UserLogin
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (UserLoginDataAccess data = new UserLoginDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of UserLogin By CompanyId        
		/// <param name="_CompanyId"></param>
        /// </summary>
        /// <returns>List of UserLogin</returns>
        public UserLoginList GetByCompanyId(Int32 _CompanyId)
        {
            using (UserLoginDataAccess data = new UserLoginDataAccess(ClientContext))
            {
                return data.GetByCompanyId(_CompanyId);
            }
        }
		
		
		#endregion
	}	
}