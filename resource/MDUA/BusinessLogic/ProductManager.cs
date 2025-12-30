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
    /// Business logic processing for Product.
    /// </summary>    
	public partial class ProductManager
	{
	
		/// <summary>
        /// Update Product Object.
        /// Data manipulation processing for: new, deleted, updated Product
        /// </summary>
        /// <param name="productObject"></param>
        /// <returns></returns>
        public bool Update(Product productObject)
        {
			bool success = false;
			
			success = UpdateBase(productObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of Product Object.
        /// </summary>
        /// <param name="productObject"></param>
        /// <returns></returns>
        public void FillChilds(Product productObject)
        {
			///Fill external information of Childs of ProductObject
        }
	}	
}