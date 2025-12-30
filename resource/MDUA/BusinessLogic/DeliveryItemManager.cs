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
    /// Business logic processing for DeliveryItem.
    /// </summary>    
	public partial class DeliveryItemManager
	{
	
		/// <summary>
        /// Update DeliveryItem Object.
        /// Data manipulation processing for: new, deleted, updated DeliveryItem
        /// </summary>
        /// <param name="deliveryItemObject"></param>
        /// <returns></returns>
        public bool Update(DeliveryItem deliveryItemObject)
        {
			bool success = false;
			
			success = UpdateBase(deliveryItemObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of DeliveryItem Object.
        /// </summary>
        /// <param name="deliveryItemObject"></param>
        /// <returns></returns>
        public void FillChilds(DeliveryItem deliveryItemObject)
        {
			///Fill external information of Childs of DeliveryItemObject
        }
	}	
}