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
    /// Business logic processing for PermissionGroup.
    /// </summary>    
	public partial class PermissionGroupManager : BaseManager
	{
	
		#region Constructors
		public PermissionGroupManager(ClientContext context) : base(context) { }
		public PermissionGroupManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new permissionGroup.
        /// data manipulation for insertion of PermissionGroup
        /// </summary>
        /// <param name="permissionGroupObject"></param>
        /// <returns></returns>
        private bool Insert(PermissionGroup permissionGroupObject)
        {
            // new permissionGroup
            using (PermissionGroupDataAccess data = new PermissionGroupDataAccess(ClientContext))
            {
                // insert to permissionGroupObject
                Int32 _Id = data.Insert(permissionGroupObject);
                // if successful, process
                if (_Id > 0)
                {
                    permissionGroupObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of PermissionGroup Object.
        /// Data manipulation processing for: new, deleted, updated PermissionGroup
        /// </summary>
        /// <param name="permissionGroupObject"></param>
        /// <returns></returns>
        public bool UpdateBase(PermissionGroup permissionGroupObject)
        {
            // use of switch for different types of DML
            switch (permissionGroupObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(permissionGroupObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(permissionGroupObject.Id);
            }
            // update rows
            using (PermissionGroupDataAccess data = new PermissionGroupDataAccess(ClientContext))
            {
                return (data.Update(permissionGroupObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for PermissionGroup
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (PermissionGroupDataAccess data = new PermissionGroupDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve PermissionGroup data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>PermissionGroup Object</returns>
        public PermissionGroup Get(Int32 _Id)
        {
            using (PermissionGroupDataAccess data = new PermissionGroupDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a PermissionGroup .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public PermissionGroup Get(Int32 _Id, bool fillChild)
        {
            PermissionGroup permissionGroupObject;
            permissionGroupObject = Get(_Id);
            
            if (permissionGroupObject != null && fillChild)
            {
                // populate child data for a permissionGroupObject
                FillPermissionGroupWithChilds(permissionGroupObject, fillChild);
            }

            return permissionGroupObject;
        }
		
		/// <summary>
        /// populates a PermissionGroup with its child entities
        /// </summary>
        /// <param name="permissionGroup"></param>
		/// <param name="fillChilds"></param>
        private void FillPermissionGroupWithChilds(PermissionGroup permissionGroupObject, bool fillChilds)
        {
            // populate child data for a permissionGroupObject
            if (permissionGroupObject != null)
            {
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of PermissionGroup.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of PermissionGroup</returns>
        public PermissionGroupList GetAll()
        {
            using (PermissionGroupDataAccess data = new PermissionGroupDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of PermissionGroup.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of PermissionGroup</returns>
        public PermissionGroupList GetAll(bool fillChild)
        {
			PermissionGroupList permissionGroupList = new PermissionGroupList();
            using (PermissionGroupDataAccess data = new PermissionGroupDataAccess(ClientContext))
            {
                permissionGroupList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (PermissionGroup permissionGroupObject in permissionGroupList)
                {
					FillPermissionGroupWithChilds(permissionGroupObject, fillChild);
				}
			}
			return permissionGroupList;
        }
		
		/// <summary>
        /// Retrieve list of PermissionGroup  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of PermissionGroup</returns>
        public PermissionGroupList GetPaged(PagedRequest request)
        {
            using (PermissionGroupDataAccess data = new PermissionGroupDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of PermissionGroup  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of PermissionGroup</returns>
        public PermissionGroupList GetByQuery(String query)
        {
            using (PermissionGroupDataAccess data = new PermissionGroupDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get PermissionGroup Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of PermissionGroup
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (PermissionGroupDataAccess data = new PermissionGroupDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get PermissionGroup Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of PermissionGroup
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (PermissionGroupDataAccess data = new PermissionGroupDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		
		#endregion
	}	
}