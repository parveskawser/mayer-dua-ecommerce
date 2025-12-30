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
    /// Business logic processing for LanguageResource.
    /// </summary>    
	public partial class LanguageResourceManager
	{
	
		/// <summary>
        /// Update LanguageResource Object.
        /// Data manipulation processing for: new, deleted, updated LanguageResource
        /// </summary>
        /// <param name="languageResourceObject"></param>
        /// <returns></returns>
        public bool Update(LanguageResource languageResourceObject)
        {
			bool success = false;
			
			success = UpdateBase(languageResourceObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of LanguageResource Object.
        /// </summary>
        /// <param name="languageResourceObject"></param>
        /// <returns></returns>
        public void FillChilds(LanguageResource languageResourceObject)
        {
			///Fill external information of Childs of LanguageResourceObject
        }
	}	
}