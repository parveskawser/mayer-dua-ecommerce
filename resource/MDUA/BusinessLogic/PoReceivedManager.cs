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
    /// Business logic processing for PoReceived.
    /// </summary>    
	public partial class PoReceivedManager
	{
	
		/// <summary>
        /// Update PoReceived Object.
        /// Data manipulation processing for: new, deleted, updated PoReceived
        /// </summary>
        /// <param name="poReceivedObject"></param>
        /// <returns></returns>
        public bool Update(PoReceived poReceivedObject)
        {
			bool success = false;
			
			success = UpdateBase(poReceivedObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of PoReceived Object.
        /// </summary>
        /// <param name="poReceivedObject"></param>
        /// <returns></returns>
        public void FillChilds(PoReceived poReceivedObject)
        {
			///Fill external information of Childs of PoReceivedObject
        }
	}	
}