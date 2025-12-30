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
    /// Business logic processing for EmailHistory.
    /// </summary>    
	public partial class EmailHistoryManager
	{
	
		/// <summary>
        /// Update EmailHistory Object.
        /// Data manipulation processing for: new, deleted, updated EmailHistory
        /// </summary>
        /// <param name="emailHistoryObject"></param>
        /// <returns></returns>
        public bool Update(EmailHistory emailHistoryObject)
        {
			bool success = false;
			
			success = UpdateBase(emailHistoryObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of EmailHistory Object.
        /// </summary>
        /// <param name="emailHistoryObject"></param>
        /// <returns></returns>
        public void FillChilds(EmailHistory emailHistoryObject)
        {
			///Fill external information of Childs of EmailHistoryObject
        }
	}	
}