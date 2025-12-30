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
    /// Business logic processing for SalesOrderDetail.
    /// </summary>    
	public partial class SalesOrderDetailManager
	{
	
		/// <summary>
        /// Update SalesOrderDetail Object.
        /// Data manipulation processing for: new, deleted, updated SalesOrderDetail
        /// </summary>
        /// <param name="salesOrderDetailObject"></param>
        /// <returns></returns>
        public bool Update(SalesOrderDetail salesOrderDetailObject)
        {
			bool success = false;
			
			success = UpdateBase(salesOrderDetailObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of SalesOrderDetail Object.
        /// </summary>
        /// <param name="salesOrderDetailObject"></param>
        /// <returns></returns>
        public void FillChilds(SalesOrderDetail salesOrderDetailObject)
        {
			///Fill external information of Childs of SalesOrderDetailObject
        }
	}	
}