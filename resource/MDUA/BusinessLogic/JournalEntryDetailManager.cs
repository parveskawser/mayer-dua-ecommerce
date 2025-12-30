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
    /// Business logic processing for JournalEntryDetail.
    /// </summary>    
	public partial class JournalEntryDetailManager
	{
	
		/// <summary>
        /// Update JournalEntryDetail Object.
        /// Data manipulation processing for: new, deleted, updated JournalEntryDetail
        /// </summary>
        /// <param name="journalEntryDetailObject"></param>
        /// <returns></returns>
        public bool Update(JournalEntryDetail journalEntryDetailObject)
        {
			bool success = false;
			
			success = UpdateBase(journalEntryDetailObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of JournalEntryDetail Object.
        /// </summary>
        /// <param name="journalEntryDetailObject"></param>
        /// <returns></returns>
        public void FillChilds(JournalEntryDetail journalEntryDetailObject)
        {
			///Fill external information of Childs of JournalEntryDetailObject
        }
	}	
}