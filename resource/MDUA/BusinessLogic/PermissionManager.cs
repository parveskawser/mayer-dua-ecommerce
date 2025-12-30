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
	public partial class PermissionManager
	{
	
		/// <summary>
        /// Update Permission Object.
        /// Data manipulation processing for: new, deleted, updated Permission
        /// </summary>
        /// <param name="permissionObject"></param>
        /// <returns></returns>
        public bool Update(Permission permissionObject)
        {
			bool success = false;
			
			success = UpdateBase(permissionObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of Permission Object.
        /// </summary>
        /// <param name="permissionObject"></param>
        /// <returns></returns>
        public void FillChilds(Permission permissionObject)
        {
			///Fill external information of Childs of PermissionObject
        }
	}	
}