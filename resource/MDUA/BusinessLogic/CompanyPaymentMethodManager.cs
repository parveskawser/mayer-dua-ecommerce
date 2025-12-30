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
    /// Business logic processing for CompanyPaymentMethod.
    /// </summary>    
	public partial class CompanyPaymentMethodManager
	{
	
		/// <summary>
        /// Update CompanyPaymentMethod Object.
        /// Data manipulation processing for: new, deleted, updated CompanyPaymentMethod
        /// </summary>
        /// <param name="companyPaymentMethodObject"></param>
        /// <returns></returns>
        public bool Update(CompanyPaymentMethod companyPaymentMethodObject)
        {
			bool success = false;
			
			success = UpdateBase(companyPaymentMethodObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of CompanyPaymentMethod Object.
        /// </summary>
        /// <param name="companyPaymentMethodObject"></param>
        /// <returns></returns>
        public void FillChilds(CompanyPaymentMethod companyPaymentMethodObject)
        {
			///Fill external information of Childs of CompanyPaymentMethodObject
        }
	}	
}