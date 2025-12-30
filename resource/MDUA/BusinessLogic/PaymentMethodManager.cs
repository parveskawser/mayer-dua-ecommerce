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
    /// Business logic processing for PaymentMethod.
    /// </summary>    
	public partial class PaymentMethodManager
	{
	
		/// <summary>
        /// Update PaymentMethod Object.
        /// Data manipulation processing for: new, deleted, updated PaymentMethod
        /// </summary>
        /// <param name="paymentMethodObject"></param>
        /// <returns></returns>
        public bool Update(PaymentMethod paymentMethodObject)
        {
			bool success = false;
			
			success = UpdateBase(paymentMethodObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of PaymentMethod Object.
        /// </summary>
        /// <param name="paymentMethodObject"></param>
        /// <returns></returns>
        public void FillChilds(PaymentMethod paymentMethodObject)
        {
			///Fill external information of Childs of PaymentMethodObject
        }
	}	
}