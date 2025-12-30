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
    /// Business logic processing for VariantAttributeValue.
    /// </summary>    
	public partial class VariantAttributeValueManager
	{
	
		/// <summary>
        /// Update VariantAttributeValue Object.
        /// Data manipulation processing for: new, deleted, updated VariantAttributeValue
        /// </summary>
        /// <param name="variantAttributeValueObject"></param>
        /// <returns></returns>
        public bool Update(VariantAttributeValue variantAttributeValueObject)
        {
			bool success = false;
			
			success = UpdateBase(variantAttributeValueObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of VariantAttributeValue Object.
        /// </summary>
        /// <param name="variantAttributeValueObject"></param>
        /// <returns></returns>
        public void FillChilds(VariantAttributeValue variantAttributeValueObject)
        {
			///Fill external information of Childs of VariantAttributeValueObject
        }
	}	
}