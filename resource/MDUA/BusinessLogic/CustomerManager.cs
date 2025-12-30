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
    /// Business logic processing for Customer.
    /// </summary>    
	public partial class CustomerManager
	{
	
		/// <summary>
        /// Update Customer Object.
        /// Data manipulation processing for: new, deleted, updated Customer
        /// </summary>
        /// <param name="customerObject"></param>
        /// <returns></returns>
        public bool Update(Customer customerObject)
        {
			bool success = false;
			
			success = UpdateBase(customerObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of Customer Object.
        /// </summary>
        /// <param name="customerObject"></param>
        /// <returns></returns>
        public void FillChilds(Customer customerObject)
        {
			///Fill external information of Childs of CustomerObject
        }
	}	
}