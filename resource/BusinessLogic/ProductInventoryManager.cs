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
    /// Business logic processing for ProductInventory.
    /// </summary>    
	public partial class ProductInventoryManager
	{
	
		/// <summary>
        /// Update ProductInventory Object.
        /// Data manipulation processing for: new, deleted, updated ProductInventory
        /// </summary>
        /// <param name="productInventoryObject"></param>
        /// <returns></returns>
        public bool Update(ProductInventory productInventoryObject)
        {
			bool success = false;
			
			success = UpdateBase(productInventoryObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of ProductInventory Object.
        /// </summary>
        /// <param name="productInventoryObject"></param>
        /// <returns></returns>
        public void FillChilds(ProductInventory productInventoryObject)
        {
			///Fill external information of Childs of ProductInventoryObject
        }
	}	
}
