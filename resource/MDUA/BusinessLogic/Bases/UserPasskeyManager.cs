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
    /// Business logic processing for UserPasskey.
    /// </summary>    
	public partial class UserPasskeyManager : BaseManager
	{
	
		#region Constructors
		public UserPasskeyManager(ClientContext context) : base(context) { }
		public UserPasskeyManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new userPasskey.
        /// data manipulation for insertion of UserPasskey
        /// </summary>
        /// <param name="userPasskeyObject"></param>
        /// <returns></returns>
        private bool Insert(UserPasskey userPasskeyObject)
        {
            // new userPasskey
            using (UserPasskeyDataAccess data = new UserPasskeyDataAccess(ClientContext))
            {
                // insert to userPasskeyObject
                Int32 _Id = data.Insert(userPasskeyObject);
                // if successful, process
                if (_Id > 0)
                {
                    userPasskeyObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of UserPasskey Object.
        /// Data manipulation processing for: new, deleted, updated UserPasskey
        /// </summary>
        /// <param name="userPasskeyObject"></param>
        /// <returns></returns>
        public bool UpdateBase(UserPasskey userPasskeyObject)
        {
            // use of switch for different types of DML
            switch (userPasskeyObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(userPasskeyObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(userPasskeyObject.Id);
            }
            // update rows
            using (UserPasskeyDataAccess data = new UserPasskeyDataAccess(ClientContext))
            {
                return (data.Update(userPasskeyObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for UserPasskey
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (UserPasskeyDataAccess data = new UserPasskeyDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve UserPasskey data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>UserPasskey Object</returns>
        public UserPasskey Get(Int32 _Id)
        {
            using (UserPasskeyDataAccess data = new UserPasskeyDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a UserPasskey .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public UserPasskey Get(Int32 _Id, bool fillChild)
        {
            UserPasskey userPasskeyObject;
            userPasskeyObject = Get(_Id);
            
            if (userPasskeyObject != null && fillChild)
            {
                // populate child data for a userPasskeyObject
                FillUserPasskeyWithChilds(userPasskeyObject, fillChild);
            }

            return userPasskeyObject;
        }
		
		/// <summary>
        /// populates a UserPasskey with its child entities
        /// </summary>
        /// <param name="userPasskey"></param>
		/// <param name="fillChilds"></param>
        private void FillUserPasskeyWithChilds(UserPasskey userPasskeyObject, bool fillChilds)
        {
            // populate child data for a userPasskeyObject
            if (userPasskeyObject != null)
            {
				// Retrieve UserIdObject as UserLogin type for the UserPasskey using UserId
				using(UserLoginManager userLoginManager = new UserLoginManager(ClientContext))
				{
					userPasskeyObject.UserIdObject = userLoginManager.Get(userPasskeyObject.UserId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of UserPasskey.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of UserPasskey</returns>
        public UserPasskeyList GetAll()
        {
            using (UserPasskeyDataAccess data = new UserPasskeyDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of UserPasskey.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of UserPasskey</returns>
        public UserPasskeyList GetAll(bool fillChild)
        {
			UserPasskeyList userPasskeyList = new UserPasskeyList();
            using (UserPasskeyDataAccess data = new UserPasskeyDataAccess(ClientContext))
            {
                userPasskeyList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (UserPasskey userPasskeyObject in userPasskeyList)
                {
					FillUserPasskeyWithChilds(userPasskeyObject, fillChild);
				}
			}
			return userPasskeyList;
        }
		
		/// <summary>
        /// Retrieve list of UserPasskey  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of UserPasskey</returns>
        public UserPasskeyList GetPaged(PagedRequest request)
        {
            using (UserPasskeyDataAccess data = new UserPasskeyDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of UserPasskey  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of UserPasskey</returns>
        public UserPasskeyList GetByQuery(String query)
        {
            using (UserPasskeyDataAccess data = new UserPasskeyDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get UserPasskey Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of UserPasskey
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (UserPasskeyDataAccess data = new UserPasskeyDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get UserPasskey Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of UserPasskey
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (UserPasskeyDataAccess data = new UserPasskeyDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of UserPasskey By UserId        
		/// <param name="_UserId"></param>
        /// </summary>
        /// <returns>List of UserPasskey</returns>
        public UserPasskeyList GetByUserId(Int32 _UserId)
        {
            using (UserPasskeyDataAccess data = new UserPasskeyDataAccess(ClientContext))
            {
                return data.GetByUserId(_UserId);
            }
        }
		
		
		#endregion
	}	
}