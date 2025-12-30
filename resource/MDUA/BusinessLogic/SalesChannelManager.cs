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
    /// Business logic processing for SalesChannel.
    /// </summary>    
	public partial class SalesChannelManager
	{
	
		/// <summary>
        /// Update SalesChannel Object.
        /// Data manipulation processing for: new, deleted, updated SalesChannel
        /// </summary>
        /// <param name="salesChannelObject"></param>
        /// <returns></returns>
        public bool Update(SalesChannel salesChannelObject)
        {
			bool success = false;
			
			success = UpdateBase(salesChannelObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of SalesChannel Object.
        /// </summary>
        /// <param name="salesChannelObject"></param>
        /// <returns></returns>
        public void FillChilds(SalesChannel salesChannelObject)
        {
			///Fill external information of Childs of SalesChannelObject
        }
	}	
}