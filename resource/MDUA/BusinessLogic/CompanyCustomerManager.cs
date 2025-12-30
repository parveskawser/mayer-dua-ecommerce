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
    /// Business logic processing for CompanyCustomer.
    /// </summary>    
	public partial class CompanyCustomerManager
	{
	
		/// <summary>
        /// Update CompanyCustomer Object.
        /// Data manipulation processing for: new, deleted, updated CompanyCustomer
        /// </summary>
        /// <param name="companyCustomerObject"></param>
        /// <returns></returns>
        public bool Update(CompanyCustomer companyCustomerObject)
        {
			bool success = false;
			
			success = UpdateBase(companyCustomerObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of CompanyCustomer Object.
        /// </summary>
        /// <param name="companyCustomerObject"></param>
        /// <returns></returns>
        public void FillChilds(CompanyCustomer companyCustomerObject)
        {
			///Fill external information of Childs of CompanyCustomerObject
        }
	}	
}