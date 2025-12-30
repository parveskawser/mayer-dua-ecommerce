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
    /// Business logic processing for ChatSession.
    /// </summary>    
	public partial class ChatSessionManager
	{
	
		/// <summary>
        /// Update ChatSession Object.
        /// Data manipulation processing for: new, deleted, updated ChatSession
        /// </summary>
        /// <param name="chatSessionObject"></param>
        /// <returns></returns>
        public bool Update(ChatSession chatSessionObject)
        {
			bool success = false;
			
			success = UpdateBase(chatSessionObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of ChatSession Object.
        /// </summary>
        /// <param name="chatSessionObject"></param>
        /// <returns></returns>
        public void FillChilds(ChatSession chatSessionObject)
        {
			///Fill external information of Childs of ChatSessionObject
        }
	}	
}