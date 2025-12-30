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
    /// Business logic processing for DeliveryStatusLog.
    /// </summary>    
	public partial class DeliveryStatusLogManager
	{
	
		/// <summary>
        /// Update DeliveryStatusLog Object.
        /// Data manipulation processing for: new, deleted, updated DeliveryStatusLog
        /// </summary>
        /// <param name="deliveryStatusLogObject"></param>
        /// <returns></returns>
        public bool Update(DeliveryStatusLog deliveryStatusLogObject)
        {
			bool success = false;
			
			success = UpdateBase(deliveryStatusLogObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of DeliveryStatusLog Object.
        /// </summary>
        /// <param name="deliveryStatusLogObject"></param>
        /// <returns></returns>
        public void FillChilds(DeliveryStatusLog deliveryStatusLogObject)
        {
			///Fill external information of Childs of DeliveryStatusLogObject
        }
	}	
}