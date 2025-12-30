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
    /// Business logic processing for PostalCodes.
    /// </summary>    
	public partial class PostalCodesManager
	{
	
		/// <summary>
        /// Update PostalCodes Object.
        /// Data manipulation processing for: new, deleted, updated PostalCodes
        /// </summary>
        /// <param name="postalCodesObject"></param>
        /// <returns></returns>
        public bool Update(PostalCodes postalCodesObject)
        {
			bool success = false;
			
			success = UpdateBase(postalCodesObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of PostalCodes Object.
        /// </summary>
        /// <param name="postalCodesObject"></param>
        /// <returns></returns>
        public void FillChilds(PostalCodes postalCodesObject)
        {
			///Fill external information of Childs of PostalCodesObject
        }
	}	
}