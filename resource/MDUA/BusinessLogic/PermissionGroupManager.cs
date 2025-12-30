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
	public partial class PermissionGroupManager
	{
	
		/// <summary>
        /// Update PermissionGroup Object.
        /// Data manipulation processing for: new, deleted, updated PermissionGroup
        /// </summary>
        /// <param name="permissionGroupObject"></param>
        /// <returns></returns>
        public bool Update(PermissionGroup permissionGroupObject)
        {
			bool success = false;
			
			success = UpdateBase(permissionGroupObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of PermissionGroup Object.
        /// </summary>
        /// <param name="permissionGroupObject"></param>
        /// <returns></returns>
        public void FillChilds(PermissionGroup permissionGroupObject)
        {
			///Fill external information of Childs of PermissionGroupObject
        }
	}	
}