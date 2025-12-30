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
    /// Business logic processing for Company.
    /// </summary>    
	public partial class CompanyManager
	{
	
		/// <summary>
        /// Update Company Object.
        /// Data manipulation processing for: new, deleted, updated Company
        /// </summary>
        /// <param name="companyObject"></param>
        /// <returns></returns>
        public bool Update(Company companyObject)
        {
			bool success = false;
			
			success = UpdateBase(companyObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of Company Object.
        /// </summary>
        /// <param name="companyObject"></param>
        /// <returns></returns>
        public void FillChilds(Company companyObject)
        {
			///Fill external information of Childs of CompanyObject
        }
	}	
}