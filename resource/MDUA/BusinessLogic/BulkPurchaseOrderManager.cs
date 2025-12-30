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
    /// Business logic processing for BulkPurchaseOrder.
    /// </summary>    
	public partial class BulkPurchaseOrderManager
	{
	
		/// <summary>
        /// Update BulkPurchaseOrder Object.
        /// Data manipulation processing for: new, deleted, updated BulkPurchaseOrder
        /// </summary>
        /// <param name="bulkPurchaseOrderObject"></param>
        /// <returns></returns>
        public bool Update(BulkPurchaseOrder bulkPurchaseOrderObject)
        {
			bool success = false;
			
			success = UpdateBase(bulkPurchaseOrderObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of BulkPurchaseOrder Object.
        /// </summary>
        /// <param name="bulkPurchaseOrderObject"></param>
        /// <returns></returns>
        public void FillChilds(BulkPurchaseOrder bulkPurchaseOrderObject)
        {
			///Fill external information of Childs of BulkPurchaseOrderObject
        }
	}	
}