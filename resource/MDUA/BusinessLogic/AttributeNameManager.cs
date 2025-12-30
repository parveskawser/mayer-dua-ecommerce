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
    /// Business logic processing for AttributeName.
    /// </summary>    
	public partial class AttributeNameManager
	{
	
		/// <summary>
        /// Update AttributeName Object.
        /// Data manipulation processing for: new, deleted, updated AttributeName
        /// </summary>
        /// <param name="attributeNameObject"></param>
        /// <returns></returns>
        public bool Update(AttributeName attributeNameObject)
        {
			bool success = false;
			
			success = UpdateBase(attributeNameObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of AttributeName Object.
        /// </summary>
        /// <param name="attributeNameObject"></param>
        /// <returns></returns>
        public void FillChilds(AttributeName attributeNameObject)
        {
			///Fill external information of Childs of AttributeNameObject
        }
	}	
}