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
    /// Business logic processing for Delivery.
    /// </summary>    
	public partial class DeliveryManager
	{
	
		/// <summary>
        /// Update Delivery Object.
        /// Data manipulation processing for: new, deleted, updated Delivery
        /// </summary>
        /// <param name="deliveryObject"></param>
        /// <returns></returns>
        public bool Update(Delivery deliveryObject)
        {
			bool success = false;
			
			success = UpdateBase(deliveryObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of Delivery Object.
        /// </summary>
        /// <param name="deliveryObject"></param>
        /// <returns></returns>
        public void FillChilds(Delivery deliveryObject)
        {
			///Fill external information of Childs of DeliveryObject
        }
	}	
}