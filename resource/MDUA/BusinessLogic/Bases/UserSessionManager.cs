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
    /// Business logic processing for UserSession.
    /// </summary>    
	public partial class UserSessionManager : BaseManager
	{
	
		#region Constructors
		public UserSessionManager(ClientContext context) : base(context) { }
		public UserSessionManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new userSession.
        /// data manipulation for insertion of UserSession
        /// </summary>
        /// <param name="userSessionObject"></param>
        /// <returns></returns>
        private bool Insert(UserSession userSessionObject)
        {
            // new userSession
            using (UserSessionDataAccess data = new UserSessionDataAccess(ClientContext))
            {
                // insert to userSessionObject
                Int32 _Id = data.Insert(userSessionObject);
                // if successful, process
                if (_Id > 0)
                {
                    userSessionObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of UserSession Object.
        /// Data manipulation processing for: new, deleted, updated UserSession
        /// </summary>
        /// <param name="userSessionObject"></param>
        /// <returns></returns>
        public bool UpdateBase(UserSession userSessionObject)
        {
            // use of switch for different types of DML
            switch (userSessionObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(userSessionObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(userSessionObject.Id);
            }
            // update rows
            using (UserSessionDataAccess data = new UserSessionDataAccess(ClientContext))
            {
                return (data.Update(userSessionObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for UserSession
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (UserSessionDataAccess data = new UserSessionDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve UserSession data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>UserSession Object</returns>
        public UserSession Get(Int32 _Id)
        {
            using (UserSessionDataAccess data = new UserSessionDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a UserSession .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public UserSession Get(Int32 _Id, bool fillChild)
        {
            UserSession userSessionObject;
            userSessionObject = Get(_Id);
            
            if (userSessionObject != null && fillChild)
            {
                // populate child data for a userSessionObject
                FillUserSessionWithChilds(userSessionObject, fillChild);
            }

            return userSessionObject;
        }
		
		/// <summary>
        /// populates a UserSession with its child entities
        /// </summary>
        /// <param name="userSession"></param>
		/// <param name="fillChilds"></param>
        private void FillUserSessionWithChilds(UserSession userSessionObject, bool fillChilds)
        {
            // populate child data for a userSessionObject
            if (userSessionObject != null)
            {
				// Retrieve UserIdObject as UserLogin type for the UserSession using UserId
				using(UserLoginManager userLoginManager = new UserLoginManager(ClientContext))
				{
					userSessionObject.UserIdObject = userLoginManager.Get(userSessionObject.UserId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of UserSession.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of UserSession</returns>
        public UserSessionList GetAll()
        {
            using (UserSessionDataAccess data = new UserSessionDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of UserSession.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of UserSession</returns>
        public UserSessionList GetAll(bool fillChild)
        {
			UserSessionList userSessionList = new UserSessionList();
            using (UserSessionDataAccess data = new UserSessionDataAccess(ClientContext))
            {
                userSessionList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (UserSession userSessionObject in userSessionList)
                {
					FillUserSessionWithChilds(userSessionObject, fillChild);
				}
			}
			return userSessionList;
        }
		
		/// <summary>
        /// Retrieve list of UserSession  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of UserSession</returns>
        public UserSessionList GetPaged(PagedRequest request)
        {
            using (UserSessionDataAccess data = new UserSessionDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of UserSession  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of UserSession</returns>
        public UserSessionList GetByQuery(String query)
        {
            using (UserSessionDataAccess data = new UserSessionDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get UserSession Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of UserSession
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (UserSessionDataAccess data = new UserSessionDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get UserSession Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of UserSession
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (UserSessionDataAccess data = new UserSessionDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of UserSession By UserId        
		/// <param name="_UserId"></param>
        /// </summary>
        /// <returns>List of UserSession</returns>
        public UserSessionList GetByUserId(Int32 _UserId)
        {
            using (UserSessionDataAccess data = new UserSessionDataAccess(ClientContext))
            {
                return data.GetByUserId(_UserId);
            }
        }
		
		
		#endregion
	}	
}