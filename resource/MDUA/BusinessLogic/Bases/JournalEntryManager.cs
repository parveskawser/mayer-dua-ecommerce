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
	public partial class JournalEntryManager : BaseManager
	{
	
		#region Constructors
		public JournalEntryManager(ClientContext context) : base(context) { }
		public JournalEntryManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new journalEntry.
        /// data manipulation for insertion of JournalEntry
        /// </summary>
        /// <param name="journalEntryObject"></param>
        /// <returns></returns>
        private bool Insert(JournalEntry journalEntryObject)
        {
            // new journalEntry
            using (JournalEntryDataAccess data = new JournalEntryDataAccess(ClientContext))
            {
                // insert to journalEntryObject
                Int32 _Id = data.Insert(journalEntryObject);
                // if successful, process
                if (_Id > 0)
                {
                    journalEntryObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of JournalEntry Object.
        /// Data manipulation processing for: new, deleted, updated JournalEntry
        /// </summary>
        /// <param name="journalEntryObject"></param>
        /// <returns></returns>
        public bool UpdateBase(JournalEntry journalEntryObject)
        {
            // use of switch for different types of DML
            switch (journalEntryObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(journalEntryObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(journalEntryObject.Id);
            }
            // update rows
            using (JournalEntryDataAccess data = new JournalEntryDataAccess(ClientContext))
            {
                return (data.Update(journalEntryObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for JournalEntry
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (JournalEntryDataAccess data = new JournalEntryDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve JournalEntry data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>JournalEntry Object</returns>
        public JournalEntry Get(Int32 _Id)
        {
            using (JournalEntryDataAccess data = new JournalEntryDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a JournalEntry .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public JournalEntry Get(Int32 _Id, bool fillChild)
        {
            JournalEntry journalEntryObject;
            journalEntryObject = Get(_Id);
            
            if (journalEntryObject != null && fillChild)
            {
                // populate child data for a journalEntryObject
                FillJournalEntryWithChilds(journalEntryObject, fillChild);
            }

            return journalEntryObject;
        }
		
		/// <summary>
        /// populates a JournalEntry with its child entities
        /// </summary>
        /// <param name="journalEntry"></param>
		/// <param name="fillChilds"></param>
        private void FillJournalEntryWithChilds(JournalEntry journalEntryObject, bool fillChilds)
        {
            // populate child data for a journalEntryObject
            if (journalEntryObject != null)
            {
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of JournalEntry.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of JournalEntry</returns>
        public JournalEntryList GetAll()
        {
            using (JournalEntryDataAccess data = new JournalEntryDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of JournalEntry.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of JournalEntry</returns>
        public JournalEntryList GetAll(bool fillChild)
        {
			JournalEntryList journalEntryList = new JournalEntryList();
            using (JournalEntryDataAccess data = new JournalEntryDataAccess(ClientContext))
            {
                journalEntryList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (JournalEntry journalEntryObject in journalEntryList)
                {
					FillJournalEntryWithChilds(journalEntryObject, fillChild);
				}
			}
			return journalEntryList;
        }
		
		/// <summary>
        /// Retrieve list of JournalEntry  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of JournalEntry</returns>
        public JournalEntryList GetPaged(PagedRequest request)
        {
            using (JournalEntryDataAccess data = new JournalEntryDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of JournalEntry  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of JournalEntry</returns>
        public JournalEntryList GetByQuery(String query)
        {
            using (JournalEntryDataAccess data = new JournalEntryDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get JournalEntry Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of JournalEntry
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (JournalEntryDataAccess data = new JournalEntryDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get JournalEntry Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of JournalEntry
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (JournalEntryDataAccess data = new JournalEntryDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		
		#endregion
	}	
}