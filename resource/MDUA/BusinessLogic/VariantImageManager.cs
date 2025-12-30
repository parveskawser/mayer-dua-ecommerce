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
    /// Business logic processing for VariantImage.
    /// </summary>    
	public partial class VariantImageManager
	{
	
		/// <summary>
        /// Update VariantImage Object.
        /// Data manipulation processing for: new, deleted, updated VariantImage
        /// </summary>
        /// <param name="variantImageObject"></param>
        /// <returns></returns>
        public bool Update(VariantImage variantImageObject)
        {
			bool success = false;
			
			success = UpdateBase(variantImageObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of VariantImage Object.
        /// </summary>
        /// <param name="variantImageObject"></param>
        /// <returns></returns>
        public void FillChilds(VariantImage variantImageObject)
        {
			///Fill external information of Childs of VariantImageObject
        }
	}	
}