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
    /// Business logic processing for Vendor.
    /// </summary>    
	public partial class VendorManager
	{
	
		/// <summary>
        /// Update Vendor Object.
        /// Data manipulation processing for: new, deleted, updated Vendor
        /// </summary>
        /// <param name="vendorObject"></param>
        /// <returns></returns>
        public bool Update(Vendor vendorObject)
        {
			bool success = false;
			
			success = UpdateBase(vendorObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of Vendor Object.
        /// </summary>
        /// <param name="vendorObject"></param>
        /// <returns></returns>
        public void FillChilds(Vendor vendorObject)
        {
			///Fill external information of Childs of VendorObject
        }
	}	
}