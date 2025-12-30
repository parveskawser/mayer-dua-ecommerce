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
    /// Business logic processing for ProductDiscount.
    /// </summary>    
	public partial class ProductDiscountManager
	{
	
		/// <summary>
        /// Update ProductDiscount Object.
        /// Data manipulation processing for: new, deleted, updated ProductDiscount
        /// </summary>
        /// <param name="productDiscountObject"></param>
        /// <returns></returns>
        public bool Update(ProductDiscount productDiscountObject)
        {
			bool success = false;
			
			success = UpdateBase(productDiscountObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of ProductDiscount Object.
        /// </summary>
        /// <param name="productDiscountObject"></param>
        /// <returns></returns>
        public void FillChilds(ProductDiscount productDiscountObject)
        {
			///Fill external information of Childs of ProductDiscountObject
        }
	}	
}