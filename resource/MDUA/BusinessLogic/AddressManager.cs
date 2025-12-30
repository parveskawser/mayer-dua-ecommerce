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
    /// Business logic processing for Address.
    /// </summary>    
	public partial class AddressManager
	{
	
		/// <summary>
        /// Update Address Object.
        /// Data manipulation processing for: new, deleted, updated Address
        /// </summary>
        /// <param name="addressObject"></param>
        /// <returns></returns>
        public bool Update(Address addressObject)
        {
			bool success = false;
			
			success = UpdateBase(addressObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of Address Object.
        /// </summary>
        /// <param name="addressObject"></param>
        /// <returns></returns>
        public void FillChilds(Address addressObject)
        {
			///Fill external information of Childs of AddressObject
        }
	}	
}