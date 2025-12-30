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
    /// Business logic processing for CompanyVendor.
    /// </summary>    
	public partial class CompanyVendorManager
	{
	
		/// <summary>
        /// Update CompanyVendor Object.
        /// Data manipulation processing for: new, deleted, updated CompanyVendor
        /// </summary>
        /// <param name="companyVendorObject"></param>
        /// <returns></returns>
        public bool Update(CompanyVendor companyVendorObject)
        {
			bool success = false;
			
			success = UpdateBase(companyVendorObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of CompanyVendor Object.
        /// </summary>
        /// <param name="companyVendorObject"></param>
        /// <returns></returns>
        public void FillChilds(CompanyVendor companyVendorObject)
        {
			///Fill external information of Childs of CompanyVendorObject
        }
	}	
}