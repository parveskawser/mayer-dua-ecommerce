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
    /// Business logic processing for AccountType.
    /// </summary>    
	public partial class AccountTypeManager
	{
	
		/// <summary>
        /// Update AccountType Object.
        /// Data manipulation processing for: new, deleted, updated AccountType
        /// </summary>
        /// <param name="accountTypeObject"></param>
        /// <returns></returns>
        public bool Update(AccountType accountTypeObject)
        {
			bool success = false;
			
			success = UpdateBase(accountTypeObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of AccountType Object.
        /// </summary>
        /// <param name="accountTypeObject"></param>
        /// <returns></returns>
        public void FillChilds(AccountType accountTypeObject)
        {
			///Fill external information of Childs of AccountTypeObject
        }
	}	
}