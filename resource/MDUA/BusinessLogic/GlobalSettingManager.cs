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
    /// Business logic processing for GlobalSetting.
    /// </summary>    
	public partial class GlobalSettingManager
	{
	
		/// <summary>
        /// Update GlobalSetting Object.
        /// Data manipulation processing for: new, deleted, updated GlobalSetting
        /// </summary>
        /// <param name="globalSettingObject"></param>
        /// <returns></returns>
        public bool Update(GlobalSetting globalSettingObject)
        {
			bool success = false;
			
			success = UpdateBase(globalSettingObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of GlobalSetting Object.
        /// </summary>
        /// <param name="globalSettingObject"></param>
        /// <returns></returns>
        public void FillChilds(GlobalSetting globalSettingObject)
        {
			///Fill external information of Childs of GlobalSettingObject
        }
	}	
}