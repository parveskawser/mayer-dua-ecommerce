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
    /// Business logic processing for EmailHistory.
    /// </summary>    
	public partial class EmailHistoryManager : BaseManager
	{
	
		#region Constructors
		public EmailHistoryManager(ClientContext context) : base(context) { }
		public EmailHistoryManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new emailHistory.
        /// data manipulation for insertion of EmailHistory
        /// </summary>
        /// <param name="emailHistoryObject"></param>
        /// <returns></returns>
        private bool Insert(EmailHistory emailHistoryObject)
        {
            // new emailHistory
            using (EmailHistoryDataAccess data = new EmailHistoryDataAccess(ClientContext))
            {
                // insert to emailHistoryObject
                Int32 _Id = data.Insert(emailHistoryObject);
                // if successful, process
                if (_Id > 0)
                {
                    emailHistoryObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of EmailHistory Object.
        /// Data manipulation processing for: new, deleted, updated EmailHistory
        /// </summary>
        /// <param name="emailHistoryObject"></param>
        /// <returns></returns>
        public bool UpdateBase(EmailHistory emailHistoryObject)
        {
            // use of switch for different types of DML
            switch (emailHistoryObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(emailHistoryObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(emailHistoryObject.Id);
            }
            // update rows
            using (EmailHistoryDataAccess data = new EmailHistoryDataAccess(ClientContext))
            {
                return (data.Update(emailHistoryObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for EmailHistory
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (EmailHistoryDataAccess data = new EmailHistoryDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve EmailHistory data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>EmailHistory Object</returns>
        public EmailHistory Get(Int32 _Id)
        {
            using (EmailHistoryDataAccess data = new EmailHistoryDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a EmailHistory .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public EmailHistory Get(Int32 _Id, bool fillChild)
        {
            EmailHistory emailHistoryObject;
            emailHistoryObject = Get(_Id);
            
            if (emailHistoryObject != null && fillChild)
            {
                // populate child data for a emailHistoryObject
                FillEmailHistoryWithChilds(emailHistoryObject, fillChild);
            }

            return emailHistoryObject;
        }
		
		/// <summary>
        /// populates a EmailHistory with its child entities
        /// </summary>
        /// <param name="emailHistory"></param>
		/// <param name="fillChilds"></param>
        private void FillEmailHistoryWithChilds(EmailHistory emailHistoryObject, bool fillChilds)
        {
            // populate child data for a emailHistoryObject
            if (emailHistoryObject != null)
            {
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of EmailHistory.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of EmailHistory</returns>
        public EmailHistoryList GetAll()
        {
            using (EmailHistoryDataAccess data = new EmailHistoryDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of EmailHistory.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of EmailHistory</returns>
        public EmailHistoryList GetAll(bool fillChild)
        {
			EmailHistoryList emailHistoryList = new EmailHistoryList();
            using (EmailHistoryDataAccess data = new EmailHistoryDataAccess(ClientContext))
            {
                emailHistoryList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (EmailHistory emailHistoryObject in emailHistoryList)
                {
					FillEmailHistoryWithChilds(emailHistoryObject, fillChild);
				}
			}
			return emailHistoryList;
        }
		
		/// <summary>
        /// Retrieve list of EmailHistory  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of EmailHistory</returns>
        public EmailHistoryList GetPaged(PagedRequest request)
        {
            using (EmailHistoryDataAccess data = new EmailHistoryDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of EmailHistory  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of EmailHistory</returns>
        public EmailHistoryList GetByQuery(String query)
        {
            using (EmailHistoryDataAccess data = new EmailHistoryDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get EmailHistory Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of EmailHistory
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (EmailHistoryDataAccess data = new EmailHistoryDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get EmailHistory Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of EmailHistory
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (EmailHistoryDataAccess data = new EmailHistoryDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		
		#endregion
	}	
}