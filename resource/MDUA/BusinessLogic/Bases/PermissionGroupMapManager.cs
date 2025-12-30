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
    /// Business logic processing for PermissionGroupMap.
    /// </summary>    
	public partial class PermissionGroupMapManager : BaseManager
	{
	
		#region Constructors
		public PermissionGroupMapManager(ClientContext context) : base(context) { }
		public PermissionGroupMapManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new permissionGroupMap.
        /// data manipulation for insertion of PermissionGroupMap
        /// </summary>
        /// <param name="permissionGroupMapObject"></param>
        /// <returns></returns>
        private bool Insert(PermissionGroupMap permissionGroupMapObject)
        {
            // new permissionGroupMap
            using (PermissionGroupMapDataAccess data = new PermissionGroupMapDataAccess(ClientContext))
            {
                // insert to permissionGroupMapObject
                Int32 _Id = data.Insert(permissionGroupMapObject);
                // if successful, process
                if (_Id > 0)
                {
                    permissionGroupMapObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of PermissionGroupMap Object.
        /// Data manipulation processing for: new, deleted, updated PermissionGroupMap
        /// </summary>
        /// <param name="permissionGroupMapObject"></param>
        /// <returns></returns>
        public bool UpdateBase(PermissionGroupMap permissionGroupMapObject)
        {
            // use of switch for different types of DML
            switch (permissionGroupMapObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(permissionGroupMapObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(permissionGroupMapObject.Id);
            }
            // update rows
            using (PermissionGroupMapDataAccess data = new PermissionGroupMapDataAccess(ClientContext))
            {
                return (data.Update(permissionGroupMapObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for PermissionGroupMap
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (PermissionGroupMapDataAccess data = new PermissionGroupMapDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve PermissionGroupMap data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>PermissionGroupMap Object</returns>
        public PermissionGroupMap Get(Int32 _Id)
        {
            using (PermissionGroupMapDataAccess data = new PermissionGroupMapDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a PermissionGroupMap .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public PermissionGroupMap Get(Int32 _Id, bool fillChild)
        {
            PermissionGroupMap permissionGroupMapObject;
            permissionGroupMapObject = Get(_Id);
            
            if (permissionGroupMapObject != null && fillChild)
            {
                // populate child data for a permissionGroupMapObject
                FillPermissionGroupMapWithChilds(permissionGroupMapObject, fillChild);
            }

            return permissionGroupMapObject;
        }
		
		/// <summary>
        /// populates a PermissionGroupMap with its child entities
        /// </summary>
        /// <param name="permissionGroupMap"></param>
		/// <param name="fillChilds"></param>
        private void FillPermissionGroupMapWithChilds(PermissionGroupMap permissionGroupMapObject, bool fillChilds)
        {
            // populate child data for a permissionGroupMapObject
            if (permissionGroupMapObject != null)
            {
				// Retrieve PermissionIdObject as Permission type for the PermissionGroupMap using PermissionId
				using(PermissionManager permissionManager = new PermissionManager(ClientContext))
				{
					permissionGroupMapObject.PermissionIdObject = permissionManager.Get(permissionGroupMapObject.PermissionId, fillChilds);
				}
				// Retrieve PermissionGroupIdObject as PermissionGroup type for the PermissionGroupMap using PermissionGroupId
				using(PermissionGroupManager permissionGroupManager = new PermissionGroupManager(ClientContext))
				{
					permissionGroupMapObject.PermissionGroupIdObject = permissionGroupManager.Get(permissionGroupMapObject.PermissionGroupId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of PermissionGroupMap.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of PermissionGroupMap</returns>
        public PermissionGroupMapList GetAll()
        {
            using (PermissionGroupMapDataAccess data = new PermissionGroupMapDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of PermissionGroupMap.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of PermissionGroupMap</returns>
        public PermissionGroupMapList GetAll(bool fillChild)
        {
			PermissionGroupMapList permissionGroupMapList = new PermissionGroupMapList();
            using (PermissionGroupMapDataAccess data = new PermissionGroupMapDataAccess(ClientContext))
            {
                permissionGroupMapList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (PermissionGroupMap permissionGroupMapObject in permissionGroupMapList)
                {
					FillPermissionGroupMapWithChilds(permissionGroupMapObject, fillChild);
				}
			}
			return permissionGroupMapList;
        }
		
		/// <summary>
        /// Retrieve list of PermissionGroupMap  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of PermissionGroupMap</returns>
        public PermissionGroupMapList GetPaged(PagedRequest request)
        {
            using (PermissionGroupMapDataAccess data = new PermissionGroupMapDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of PermissionGroupMap  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of PermissionGroupMap</returns>
        public PermissionGroupMapList GetByQuery(String query)
        {
            using (PermissionGroupMapDataAccess data = new PermissionGroupMapDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get PermissionGroupMap Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of PermissionGroupMap
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (PermissionGroupMapDataAccess data = new PermissionGroupMapDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get PermissionGroupMap Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of PermissionGroupMap
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (PermissionGroupMapDataAccess data = new PermissionGroupMapDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of PermissionGroupMap By PermissionId        
		/// <param name="_PermissionId"></param>
        /// </summary>
        /// <returns>List of PermissionGroupMap</returns>
        public PermissionGroupMapList GetByPermissionId(Int32 _PermissionId)
        {
            using (PermissionGroupMapDataAccess data = new PermissionGroupMapDataAccess(ClientContext))
            {
                return data.GetByPermissionId(_PermissionId);
            }
        }
		
		/// <summary>
        /// Retrieve list of PermissionGroupMap By PermissionGroupId        
		/// <param name="_PermissionGroupId"></param>
        /// </summary>
        /// <returns>List of PermissionGroupMap</returns>
        public PermissionGroupMapList GetByPermissionGroupId(Int32 _PermissionGroupId)
        {
            using (PermissionGroupMapDataAccess data = new PermissionGroupMapDataAccess(ClientContext))
            {
                return data.GetByPermissionGroupId(_PermissionGroupId);
            }
        }
		
		
		#endregion
	}	
}