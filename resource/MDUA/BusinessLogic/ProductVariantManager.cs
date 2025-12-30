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
    /// Business logic processing for ProductVariant.
    /// </summary>    
	public partial class ProductVariantManager
	{
	
		/// <summary>
        /// Update ProductVariant Object.
        /// Data manipulation processing for: new, deleted, updated ProductVariant
        /// </summary>
        /// <param name="productVariantObject"></param>
        /// <returns></returns>
        public bool Update(ProductVariant productVariantObject)
        {
			bool success = false;
			
			success = UpdateBase(productVariantObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of ProductVariant Object.
        /// </summary>
        /// <param name="productVariantObject"></param>
        /// <returns></returns>
        public void FillChilds(ProductVariant productVariantObject)
        {
			///Fill external information of Childs of ProductVariantObject
        }
	}	
}