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
    /// Business logic processing for VariantPriceStock.
    /// </summary>    
	public partial class VariantPriceStockManager
	{
	
		/// <summary>
        /// Update VariantPriceStock Object.
        /// Data manipulation processing for: new, deleted, updated VariantPriceStock
        /// </summary>
        /// <param name="variantPriceStockObject"></param>
        /// <returns></returns>
        public bool Update(VariantPriceStock variantPriceStockObject)
        {
			bool success = false;
			
			success = UpdateBase(variantPriceStockObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of VariantPriceStock Object.
        /// </summary>
        /// <param name="variantPriceStockObject"></param>
        /// <returns></returns>
        public void FillChilds(VariantPriceStock variantPriceStockObject)
        {
			///Fill external information of Childs of VariantPriceStockObject
        }
	}	
}