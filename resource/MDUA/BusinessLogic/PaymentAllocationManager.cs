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
    /// Business logic processing for PaymentAllocation.
    /// </summary>    
	public partial class PaymentAllocationManager
	{
	
		/// <summary>
        /// Update PaymentAllocation Object.
        /// Data manipulation processing for: new, deleted, updated PaymentAllocation
        /// </summary>
        /// <param name="paymentAllocationObject"></param>
        /// <returns></returns>
        public bool Update(PaymentAllocation paymentAllocationObject)
        {
			bool success = false;
			
			success = UpdateBase(paymentAllocationObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of PaymentAllocation Object.
        /// </summary>
        /// <param name="paymentAllocationObject"></param>
        /// <returns></returns>
        public void FillChilds(PaymentAllocation paymentAllocationObject)
        {
			///Fill external information of Childs of PaymentAllocationObject
        }
	}	
}