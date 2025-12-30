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
    /// Business logic processing for UserPermission.
    /// </summary>    
	public partial class UserPermissionManager : BaseManager
	{
	
		#region Constructors
		public UserPermissionManager(ClientContext context) : base(context) { }
		public UserPermissionManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new userPermission.
        /// data manipulation for insertion of UserPermission
        /// </summary>
        /// <param name="userPermissionObject"></param>
        /// <returns></returns>
        private bool Insert(UserPermission userPermissionObject)
        {
            // new userPermission
            using (UserPermissionDataAccess data = new UserPermissionDataAccess(ClientContext))
            {
                // insert to userPermissionObject
                Int32 _Id = data.Insert(userPermissionObject);
                // if successful, process
                if (_Id > 0)
                {
                    userPermissionObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of UserPermission Object.
        /// Data manipulation processing for: new, deleted, updated UserPermission
        /// </summary>
        /// <param name="userPermissionObject"></param>
        /// <returns></returns>
        public bool UpdateBase(UserPermission userPermissionObject)
        {
            // use of switch for different types of DML
            switch (userPermissionObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(userPermissionObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(userPermissionObject.Id);
            }
            // update rows
            using (UserPermissionDataAccess data = new UserPermissionDataAccess(ClientContext))
            {
                return (data.Update(userPermissionObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for UserPermission
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (UserPermissionDataAccess data = new UserPermissionDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve UserPermission data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>UserPermission Object</returns>
        public UserPermission Get(Int32 _Id)
        {
            using (UserPermissionDataAccess data = new UserPermissionDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a UserPermission .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public UserPermission Get(Int32 _Id, bool fillChild)
        {
            UserPermission userPermissionObject;
            userPermissionObject = Get(_Id);
            
            if (userPermissionObject != null && fillChild)
            {
                // populate child data for a userPermissionObject
                FillUserPermissionWithChilds(userPermissionObject, fillChild);
            }

            return userPermissionObject;
        }
		
		/// <summary>
        /// populates a UserPermission with its child entities
        /// </summary>
        /// <param name="userPermission"></param>
		/// <param name="fillChilds"></param>
        private void FillUserPermissionWithChilds(UserPermission userPermissionObject, bool fillChilds)
        {
            // populate child data for a userPermissionObject
            if (userPermissionObject != null)
            {
				// Retrieve PermissionIdObject as Permission type for the UserPermission using PermissionId
				using(PermissionManager permissionManager = new PermissionManager(ClientContext))
				{
					userPermissionObject.PermissionIdObject = permissionManager.Get(userPermissionObject.PermissionId, fillChilds);
				}
				// Retrieve PermissionGroupIdObject as PermissionGroup type for the UserPermission using PermissionGroupId
				using(PermissionGroupManager permissionGroupManager = new PermissionGroupManager(ClientContext))
				{
					userPermissionObject.PermissionGroupIdObject = permissionGroupManager.Get(userPermissionObject.PermissionGroupId, fillChilds);
				}
				// Retrieve UserIdObject as UserLogin type for the UserPermission using UserId
				using(UserLoginManager userLoginManager = new UserLoginManager(ClientContext))
				{
					userPermissionObject.UserIdObject = userLoginManager.Get(userPermissionObject.UserId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of UserPermission.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of UserPermission</returns>
        public UserPermissionList GetAll()
        {
            using (UserPermissionDataAccess data = new UserPermissionDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of UserPermission.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of UserPermission</returns>
        public UserPermissionList GetAll(bool fillChild)
        {
			UserPermissionList userPermissionList = new UserPermissionList();
            using (UserPermissionDataAccess data = new UserPermissionDataAccess(ClientContext))
            {
                userPermissionList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (UserPermission userPermissionObject in userPermissionList)
                {
					FillUserPermissionWithChilds(userPermissionObject, fillChild);
				}
			}
			return userPermissionList;
        }
		
		/// <summary>
        /// Retrieve list of UserPermission  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of UserPermission</returns>
        public UserPermissionList GetPaged(PagedRequest request)
        {
            using (UserPermissionDataAccess data = new UserPermissionDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of UserPermission  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of UserPermission</returns>
        public UserPermissionList GetByQuery(String query)
        {
            using (UserPermissionDataAccess data = new UserPermissionDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get UserPermission Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of UserPermission
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (UserPermissionDataAccess data = new UserPermissionDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get UserPermission Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of UserPermission
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (UserPermissionDataAccess data = new UserPermissionDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of UserPermission By UserId        
		/// <param name="_UserId"></param>
        /// </summary>
        /// <returns>List of UserPermission</returns>
        public UserPermissionList GetByUserId(Int32 _UserId)
        {
            using (UserPermissionDataAccess data = new UserPermissionDataAccess(ClientContext))
            {
                return data.GetByUserId(_UserId);
            }
        }
		
		/// <summary>
        /// Retrieve list of UserPermission By PermissionId        
		/// <param name="_PermissionId"></param>
        /// </summary>
        /// <returns>List of UserPermission</returns>
        public UserPermissionList GetByPermissionId(Nullable<Int32> _PermissionId)
        {
            using (UserPermissionDataAccess data = new UserPermissionDataAccess(ClientContext))
            {
                return data.GetByPermissionId(_PermissionId);
            }
        }
		
		/// <summary>
        /// Retrieve list of UserPermission By PermissionGroupId        
		/// <param name="_PermissionGroupId"></param>
        /// </summary>
        /// <returns>List of UserPermission</returns>
        public UserPermissionList GetByPermissionGroupId(Nullable<Int32> _PermissionGroupId)
        {
            using (UserPermissionDataAccess data = new UserPermissionDataAccess(ClientContext))
            {
                return data.GetByPermissionGroupId(_PermissionGroupId);
            }
        }
		
		
		#endregion
	}	
}