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
    /// Business logic processing for UserLogin.
    /// </summary>    
	public partial class UserLoginManager
	{
	
		/// <summary>
        /// Update UserLogin Object.
        /// Data manipulation processing for: new, deleted, updated UserLogin
        /// </summary>
        /// <param name="userLoginObject"></param>
        /// <returns></returns>
        public bool Update(UserLogin userLoginObject)
        {
			bool success = false;
			
			success = UpdateBase(userLoginObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of UserLogin Object.
        /// </summary>
        /// <param name="userLoginObject"></param>
        /// <returns></returns>
        public void FillChilds(UserLogin userLoginObject)
        {
			///Fill external information of Childs of UserLoginObject
        }
	}	
}