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
    /// Business logic processing for InventoryTransaction.
    /// </summary>    
	public partial class InventoryTransactionManager
	{
	
		/// <summary>
        /// Update InventoryTransaction Object.
        /// Data manipulation processing for: new, deleted, updated InventoryTransaction
        /// </summary>
        /// <param name="inventoryTransactionObject"></param>
        /// <returns></returns>
        public bool Update(InventoryTransaction inventoryTransactionObject)
        {
			bool success = false;
			
			success = UpdateBase(inventoryTransactionObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of InventoryTransaction Object.
        /// </summary>
        /// <param name="inventoryTransactionObject"></param>
        /// <returns></returns>
        public void FillChilds(InventoryTransaction inventoryTransactionObject)
        {
			///Fill external information of Childs of InventoryTransactionObject
        }
	}	
}