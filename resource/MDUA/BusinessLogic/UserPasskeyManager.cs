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
    /// Business logic processing for UserPasskey.
    /// </summary>    
	public partial class UserPasskeyManager
	{
	
		/// <summary>
        /// Update UserPasskey Object.
        /// Data manipulation processing for: new, deleted, updated UserPasskey
        /// </summary>
        /// <param name="userPasskeyObject"></param>
        /// <returns></returns>
        public bool Update(UserPasskey userPasskeyObject)
        {
			bool success = false;
			
			success = UpdateBase(userPasskeyObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of UserPasskey Object.
        /// </summary>
        /// <param name="userPasskeyObject"></param>
        /// <returns></returns>
        public void FillChilds(UserPasskey userPasskeyObject)
        {
			///Fill external information of Childs of UserPasskeyObject
        }
	}	
}