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
    /// Business logic processing for ProductImage.
    /// </summary>    
	public partial class ProductImageManager
	{
	
		/// <summary>
        /// Update ProductImage Object.
        /// Data manipulation processing for: new, deleted, updated ProductImage
        /// </summary>
        /// <param name="productImageObject"></param>
        /// <returns></returns>
        public bool Update(ProductImage productImageObject)
        {
			bool success = false;
			
			success = UpdateBase(productImageObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of ProductImage Object.
        /// </summary>
        /// <param name="productImageObject"></param>
        /// <returns></returns>
        public void FillChilds(ProductImage productImageObject)
        {
			///Fill external information of Childs of ProductImageObject
        }
	}	
}