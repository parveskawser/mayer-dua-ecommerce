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
    /// Business logic processing for ProductAttribute.
    /// </summary>    
	public partial class ProductAttributeManager
	{
	
		/// <summary>
        /// Update ProductAttribute Object.
        /// Data manipulation processing for: new, deleted, updated ProductAttribute
        /// </summary>
        /// <param name="productAttributeObject"></param>
        /// <returns></returns>
        public bool Update(ProductAttribute productAttributeObject)
        {
			bool success = false;
			
			success = UpdateBase(productAttributeObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of ProductAttribute Object.
        /// </summary>
        /// <param name="productAttributeObject"></param>
        /// <returns></returns>
        public void FillChilds(ProductAttribute productAttributeObject)
        {
			///Fill external information of Childs of ProductAttributeObject
        }
	}	
}