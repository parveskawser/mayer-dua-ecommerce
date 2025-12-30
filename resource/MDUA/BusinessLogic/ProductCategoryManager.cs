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
    /// Business logic processing for ProductCategory.
    /// </summary>    
	public partial class ProductCategoryManager
	{
	
		/// <summary>
        /// Update ProductCategory Object.
        /// Data manipulation processing for: new, deleted, updated ProductCategory
        /// </summary>
        /// <param name="productCategoryObject"></param>
        /// <returns></returns>
        public bool Update(ProductCategory productCategoryObject)
        {
			bool success = false;
			
			success = UpdateBase(productCategoryObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of ProductCategory Object.
        /// </summary>
        /// <param name="productCategoryObject"></param>
        /// <returns></returns>
        public void FillChilds(ProductCategory productCategoryObject)
        {
			///Fill external information of Childs of ProductCategoryObject
        }
	}	
}