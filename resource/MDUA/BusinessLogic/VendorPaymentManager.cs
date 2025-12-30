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
    /// Business logic processing for VendorPayment.
    /// </summary>    
	public partial class VendorPaymentManager
	{
	
		/// <summary>
        /// Update VendorPayment Object.
        /// Data manipulation processing for: new, deleted, updated VendorPayment
        /// </summary>
        /// <param name="vendorPaymentObject"></param>
        /// <returns></returns>
        public bool Update(VendorPayment vendorPaymentObject)
        {
			bool success = false;
			
			success = UpdateBase(vendorPaymentObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of VendorPayment Object.
        /// </summary>
        /// <param name="vendorPaymentObject"></param>
        /// <returns></returns>
        public void FillChilds(VendorPayment vendorPaymentObject)
        {
			///Fill external information of Childs of VendorPaymentObject
        }
	}	
}