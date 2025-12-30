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
    /// Business logic processing for SalesOrderHeader.
    /// </summary>    
	public partial class SalesOrderHeaderManager
	{
	
		/// <summary>
        /// Update SalesOrderHeader Object.
        /// Data manipulation processing for: new, deleted, updated SalesOrderHeader
        /// </summary>
        /// <param name="salesOrderHeaderObject"></param>
        /// <returns></returns>
        public bool Update(SalesOrderHeader salesOrderHeaderObject)
        {
			bool success = false;
			
			success = UpdateBase(salesOrderHeaderObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of SalesOrderHeader Object.
        /// </summary>
        /// <param name="salesOrderHeaderObject"></param>
        /// <returns></returns>
        public void FillChilds(SalesOrderHeader salesOrderHeaderObject)
        {
			///Fill external information of Childs of SalesOrderHeaderObject
        }
	}	
}