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
    /// Business logic processing for Permission.
    /// </summary>    
	public partial class PermissionManager : BaseManager
	{
	
		#region Constructors
		public PermissionManager(ClientContext context) : base(context) { }
		public PermissionManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new permission.
        /// data manipulation for insertion of Permission
        /// </summary>
        /// <param name="permissionObject"></param>
        /// <returns></returns>
        private bool Insert(Permission permissionObject)
        {
            // new permission
            using (PermissionDataAccess data = new PermissionDataAccess(ClientContext))
            {
                // insert to permissionObject
                Int32 _Id = data.Insert(permissionObject);
                // if successful, process
                if (_Id > 0)
                {
                    permissionObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of Permission Object.
        /// Data manipulation processing for: new, deleted, updated Permission
        /// </summary>
        /// <param name="permissionObject"></param>
        /// <returns></returns>
        public bool UpdateBase(Permission permissionObject)
        {
            // use of switch for different types of DML
            switch (permissionObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(permissionObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(permissionObject.Id);
            }
            // update rows
            using (PermissionDataAccess data = new PermissionDataAccess(ClientContext))
            {
                return (data.Update(permissionObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for Permission
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (PermissionDataAccess data = new PermissionDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve Permission data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>Permission Object</returns>
        public Permission Get(Int32 _Id)
        {
            using (PermissionDataAccess data = new PermissionDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a Permission .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public Permission Get(Int32 _Id, bool fillChild)
        {
            Permission permissionObject;
            permissionObject = Get(_Id);
            
            if (permissionObject != null && fillChild)
            {
                // populate child data for a permissionObject
                FillPermissionWithChilds(permissionObject, fillChild);
            }

            return permissionObject;
        }
		
		/// <summary>
        /// populates a Permission with its child entities
        /// </summary>
        /// <param name="permission"></param>
		/// <param name="fillChilds"></param>
        private void FillPermissionWithChilds(Permission permissionObject, bool fillChilds)
        {
            // populate child data for a permissionObject
            if (permissionObject != null)
            {
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of Permission.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of Permission</returns>
        public PermissionList GetAll()
        {
            using (PermissionDataAccess data = new PermissionDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of Permission.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of Permission</returns>
        public PermissionList GetAll(bool fillChild)
        {
			PermissionList permissionList = new PermissionList();
            using (PermissionDataAccess data = new PermissionDataAccess(ClientContext))
            {
                permissionList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (Permission permissionObject in permissionList)
                {
					FillPermissionWithChilds(permissionObject, fillChild);
				}
			}
			return permissionList;
        }
		
		/// <summary>
        /// Retrieve list of Permission  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of Permission</returns>
        public PermissionList GetPaged(PagedRequest request)
        {
            using (PermissionDataAccess data = new PermissionDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of Permission  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of Permission</returns>
        public PermissionList GetByQuery(String query)
        {
            using (PermissionDataAccess data = new PermissionDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get Permission Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Permission
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (PermissionDataAccess data = new PermissionDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get Permission Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Permission
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (PermissionDataAccess data = new PermissionDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		
		#endregion
	}	
}