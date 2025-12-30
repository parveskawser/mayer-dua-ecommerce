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
    /// Business logic processing for JournalEntry.
    /// </summary>    
	public partial class JournalEntryManager
	{
	
		/// <summary>
        /// Update JournalEntry Object.
        /// Data manipulation processing for: new, deleted, updated JournalEntry
        /// </summary>
        /// <param name="journalEntryObject"></param>
        /// <returns></returns>
        public bool Update(JournalEntry journalEntryObject)
        {
			bool success = false;
			
			success = UpdateBase(journalEntryObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of JournalEntry Object.
        /// </summary>
        /// <param name="journalEntryObject"></param>
        /// <returns></returns>
        public void FillChilds(JournalEntry journalEntryObject)
        {
			///Fill external information of Childs of JournalEntryObject
        }
	}	
}