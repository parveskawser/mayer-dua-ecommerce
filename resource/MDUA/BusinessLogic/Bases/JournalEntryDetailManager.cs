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
	public partial class JournalEntryDetailManager : BaseManager
	{
	
		#region Constructors
		public JournalEntryDetailManager(ClientContext context) : base(context) { }
		public JournalEntryDetailManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new journalEntryDetail.
        /// data manipulation for insertion of JournalEntryDetail
        /// </summary>
        /// <param name="journalEntryDetailObject"></param>
        /// <returns></returns>
        private bool Insert(JournalEntryDetail journalEntryDetailObject)
        {
            // new journalEntryDetail
            using (JournalEntryDetailDataAccess data = new JournalEntryDetailDataAccess(ClientContext))
            {
                // insert to journalEntryDetailObject
                Int32 _Id = data.Insert(journalEntryDetailObject);
                // if successful, process
                if (_Id > 0)
                {
                    journalEntryDetailObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of JournalEntryDetail Object.
        /// Data manipulation processing for: new, deleted, updated JournalEntryDetail
        /// </summary>
        /// <param name="journalEntryDetailObject"></param>
        /// <returns></returns>
        public bool UpdateBase(JournalEntryDetail journalEntryDetailObject)
        {
            // use of switch for different types of DML
            switch (journalEntryDetailObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(journalEntryDetailObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(journalEntryDetailObject.Id);
            }
            // update rows
            using (JournalEntryDetailDataAccess data = new JournalEntryDetailDataAccess(ClientContext))
            {
                return (data.Update(journalEntryDetailObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for JournalEntryDetail
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (JournalEntryDetailDataAccess data = new JournalEntryDetailDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve JournalEntryDetail data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>JournalEntryDetail Object</returns>
        public JournalEntryDetail Get(Int32 _Id)
        {
            using (JournalEntryDetailDataAccess data = new JournalEntryDetailDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a JournalEntryDetail .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public JournalEntryDetail Get(Int32 _Id, bool fillChild)
        {
            JournalEntryDetail journalEntryDetailObject;
            journalEntryDetailObject = Get(_Id);
            
            if (journalEntryDetailObject != null && fillChild)
            {
                // populate child data for a journalEntryDetailObject
                FillJournalEntryDetailWithChilds(journalEntryDetailObject, fillChild);
            }

            return journalEntryDetailObject;
        }
		
		/// <summary>
        /// populates a JournalEntryDetail with its child entities
        /// </summary>
        /// <param name="journalEntryDetail"></param>
		/// <param name="fillChilds"></param>
        private void FillJournalEntryDetailWithChilds(JournalEntryDetail journalEntryDetailObject, bool fillChilds)
        {
            // populate child data for a journalEntryDetailObject
            if (journalEntryDetailObject != null)
            {
				// Retrieve AccountIdObject as Account type for the JournalEntryDetail using AccountId
				using(AccountManager accountManager = new AccountManager(ClientContext))
				{
					journalEntryDetailObject.AccountIdObject = accountManager.Get(journalEntryDetailObject.AccountId, fillChilds);
				}
				// Retrieve JournalEntryIdObject as JournalEntry type for the JournalEntryDetail using JournalEntryId
				using(JournalEntryManager journalEntryManager = new JournalEntryManager(ClientContext))
				{
					journalEntryDetailObject.JournalEntryIdObject = journalEntryManager.Get(journalEntryDetailObject.JournalEntryId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of JournalEntryDetail.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of JournalEntryDetail</returns>
        public JournalEntryDetailList GetAll()
        {
            using (JournalEntryDetailDataAccess data = new JournalEntryDetailDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of JournalEntryDetail.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of JournalEntryDetail</returns>
        public JournalEntryDetailList GetAll(bool fillChild)
        {
			JournalEntryDetailList journalEntryDetailList = new JournalEntryDetailList();
            using (JournalEntryDetailDataAccess data = new JournalEntryDetailDataAccess(ClientContext))
            {
                journalEntryDetailList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (JournalEntryDetail journalEntryDetailObject in journalEntryDetailList)
                {
					FillJournalEntryDetailWithChilds(journalEntryDetailObject, fillChild);
				}
			}
			return journalEntryDetailList;
        }
		
		/// <summary>
        /// Retrieve list of JournalEntryDetail  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of JournalEntryDetail</returns>
        public JournalEntryDetailList GetPaged(PagedRequest request)
        {
            using (JournalEntryDetailDataAccess data = new JournalEntryDetailDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of JournalEntryDetail  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of JournalEntryDetail</returns>
        public JournalEntryDetailList GetByQuery(String query)
        {
            using (JournalEntryDetailDataAccess data = new JournalEntryDetailDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get JournalEntryDetail Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of JournalEntryDetail
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (JournalEntryDetailDataAccess data = new JournalEntryDetailDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get JournalEntryDetail Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of JournalEntryDetail
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (JournalEntryDetailDataAccess data = new JournalEntryDetailDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of JournalEntryDetail By JournalEntryId        
		/// <param name="_JournalEntryId"></param>
        /// </summary>
        /// <returns>List of JournalEntryDetail</returns>
        public JournalEntryDetailList GetByJournalEntryId(Int32 _JournalEntryId)
        {
            using (JournalEntryDetailDataAccess data = new JournalEntryDetailDataAccess(ClientContext))
            {
                return data.GetByJournalEntryId(_JournalEntryId);
            }
        }
		
		/// <summary>
        /// Retrieve list of JournalEntryDetail By AccountId        
		/// <param name="_AccountId"></param>
        /// </summary>
        /// <returns>List of JournalEntryDetail</returns>
        public JournalEntryDetailList GetByAccountId(Int32 _AccountId)
        {
            using (JournalEntryDetailDataAccess data = new JournalEntryDetailDataAccess(ClientContext))
            {
                return data.GetByAccountId(_AccountId);
            }
        }
		
		
		#endregion
	}	
}