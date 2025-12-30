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
    /// Business logic processing for AttributeValue.
    /// </summary>    
	public partial class AttributeValueManager
	{
	
		/// <summary>
        /// Update AttributeValue Object.
        /// Data manipulation processing for: new, deleted, updated AttributeValue
        /// </summary>
        /// <param name="attributeValueObject"></param>
        /// <returns></returns>
        public bool Update(AttributeValue attributeValueObject)
        {
			bool success = false;
			
			success = UpdateBase(attributeValueObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of AttributeValue Object.
        /// </summary>
        /// <param name="attributeValueObject"></param>
        /// <returns></returns>
        public void FillChilds(AttributeValue attributeValueObject)
        {
			///Fill external information of Childs of AttributeValueObject
        }
	}	
}