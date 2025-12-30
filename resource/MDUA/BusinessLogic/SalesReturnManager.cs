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
    /// Business logic processing for SalesReturn.
    /// </summary>    
	public partial class SalesReturnManager
	{
	
		/// <summary>
        /// Update SalesReturn Object.
        /// Data manipulation processing for: new, deleted, updated SalesReturn
        /// </summary>
        /// <param name="salesReturnObject"></param>
        /// <returns></returns>
        public bool Update(SalesReturn salesReturnObject)
        {
			bool success = false;
			
			success = UpdateBase(salesReturnObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of SalesReturn Object.
        /// </summary>
        /// <param name="salesReturnObject"></param>
        /// <returns></returns>
        public void FillChilds(SalesReturn salesReturnObject)
        {
			///Fill external information of Childs of SalesReturnObject
        }
	}	
}