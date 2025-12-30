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
    /// Business logic processing for CustomerPayment.
    /// </summary>    
	public partial class CustomerPaymentManager
	{
	
		/// <summary>
        /// Update CustomerPayment Object.
        /// Data manipulation processing for: new, deleted, updated CustomerPayment
        /// </summary>
        /// <param name="customerPaymentObject"></param>
        /// <returns></returns>
        public bool Update(CustomerPayment customerPaymentObject)
        {
			bool success = false;
			
			success = UpdateBase(customerPaymentObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of CustomerPayment Object.
        /// </summary>
        /// <param name="customerPaymentObject"></param>
        /// <returns></returns>
        public void FillChilds(CustomerPayment customerPaymentObject)
        {
			///Fill external information of Childs of CustomerPaymentObject
        }
	}	
}