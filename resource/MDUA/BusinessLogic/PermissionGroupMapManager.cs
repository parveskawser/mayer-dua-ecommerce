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
	public partial class PermissionGroupMapManager
	{
	
		/// <summary>
        /// Update PermissionGroupMap Object.
        /// Data manipulation processing for: new, deleted, updated PermissionGroupMap
        /// </summary>
        /// <param name="permissionGroupMapObject"></param>
        /// <returns></returns>
        public bool Update(PermissionGroupMap permissionGroupMapObject)
        {
			bool success = false;
			
			success = UpdateBase(permissionGroupMapObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of PermissionGroupMap Object.
        /// </summary>
        /// <param name="permissionGroupMapObject"></param>
        /// <returns></returns>
        public void FillChilds(PermissionGroupMap permissionGroupMapObject)
        {
			///Fill external information of Childs of PermissionGroupMapObject
        }
	}	
}