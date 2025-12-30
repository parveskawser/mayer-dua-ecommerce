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
	public partial class UserPermissionManager
	{
	
		/// <summary>
        /// Update UserPermission Object.
        /// Data manipulation processing for: new, deleted, updated UserPermission
        /// </summary>
        /// <param name="userPermissionObject"></param>
        /// <returns></returns>
        public bool Update(UserPermission userPermissionObject)
        {
			bool success = false;
			
			success = UpdateBase(userPermissionObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of UserPermission Object.
        /// </summary>
        /// <param name="userPermissionObject"></param>
        /// <returns></returns>
        public void FillChilds(UserPermission userPermissionObject)
        {
			///Fill external information of Childs of UserPermissionObject
        }
	}	
}