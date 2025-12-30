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
	public partial class UserSessionManager
	{
	
		/// <summary>
        /// Update UserSession Object.
        /// Data manipulation processing for: new, deleted, updated UserSession
        /// </summary>
        /// <param name="userSessionObject"></param>
        /// <returns></returns>
        public bool Update(UserSession userSessionObject)
        {
			bool success = false;
			
			success = UpdateBase(userSessionObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of UserSession Object.
        /// </summary>
        /// <param name="userSessionObject"></param>
        /// <returns></returns>
        public void FillChilds(UserSession userSessionObject)
        {
			///Fill external information of Childs of UserSessionObject
        }
	}	
}