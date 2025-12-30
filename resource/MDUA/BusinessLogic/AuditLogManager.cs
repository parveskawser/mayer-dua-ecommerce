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
    /// Business logic processing for AuditLog.
    /// </summary>    
	public partial class AuditLogManager
	{
	
		/// <summary>
        /// Update AuditLog Object.
        /// Data manipulation processing for: new, deleted, updated AuditLog
        /// </summary>
        /// <param name="auditLogObject"></param>
        /// <returns></returns>
        public bool Update(AuditLog auditLogObject)
        {
			bool success = false;
			
			success = UpdateBase(auditLogObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of AuditLog Object.
        /// </summary>
        /// <param name="auditLogObject"></param>
        /// <returns></returns>
        public void FillChilds(AuditLog auditLogObject)
        {
			///Fill external information of Childs of AuditLogObject
        }
	}	
}