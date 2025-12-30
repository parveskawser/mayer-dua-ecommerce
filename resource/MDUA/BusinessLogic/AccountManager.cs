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
    /// Business logic processing for Account.
    /// </summary>    
	public partial class AccountManager
	{
	
		/// <summary>
        /// Update Account Object.
        /// Data manipulation processing for: new, deleted, updated Account
        /// </summary>
        /// <param name="accountObject"></param>
        /// <returns></returns>
        public bool Update(Account accountObject)
        {
			bool success = false;
			
			success = UpdateBase(accountObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of Account Object.
        /// </summary>
        /// <param name="accountObject"></param>
        /// <returns></returns>
        public void FillChilds(Account accountObject)
        {
			///Fill external information of Childs of AccountObject
        }
	}	
}