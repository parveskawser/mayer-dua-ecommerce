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
    /// Business logic processing for Variant.
    /// </summary>    
	public partial class VariantManager
	{
	
		/// <summary>
        /// Update Variant Object.
        /// Data manipulation processing for: new, deleted, updated Variant
        /// </summary>
        /// <param name="variantObject"></param>
        /// <returns></returns>
        public bool Update(Variant variantObject)
        {
			bool success = false;
			
			success = UpdateBase(variantObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of Variant Object.
        /// </summary>
        /// <param name="variantObject"></param>
        /// <returns></returns>
        public void FillChilds(Variant variantObject)
        {
			///Fill external information of Childs of VariantObject
        }
	}	
}