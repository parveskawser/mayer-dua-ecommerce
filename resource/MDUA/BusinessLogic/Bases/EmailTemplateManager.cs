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
    /// Business logic processing for EmailTemplate.
    /// </summary>    
	public partial class EmailTemplateManager : BaseManager
	{
	
		#region Constructors
		public EmailTemplateManager(ClientContext context) : base(context) { }
		public EmailTemplateManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new emailTemplate.
        /// data manipulation for insertion of EmailTemplate
        /// </summary>
        /// <param name="emailTemplateObject"></param>
        /// <returns></returns>
        private bool Insert(EmailTemplate emailTemplateObject)
        {
            // new emailTemplate
            using (EmailTemplateDataAccess data = new EmailTemplateDataAccess(ClientContext))
            {
                // insert to emailTemplateObject
                Int32 _Id = data.Insert(emailTemplateObject);
                // if successful, process
                if (_Id > 0)
                {
                    emailTemplateObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of EmailTemplate Object.
        /// Data manipulation processing for: new, deleted, updated EmailTemplate
        /// </summary>
        /// <param name="emailTemplateObject"></param>
        /// <returns></returns>
        public bool UpdateBase(EmailTemplate emailTemplateObject)
        {
            // use of switch for different types of DML
            switch (emailTemplateObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(emailTemplateObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(emailTemplateObject.Id);
            }
            // update rows
            using (EmailTemplateDataAccess data = new EmailTemplateDataAccess(ClientContext))
            {
                return (data.Update(emailTemplateObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for EmailTemplate
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (EmailTemplateDataAccess data = new EmailTemplateDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve EmailTemplate data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>EmailTemplate Object</returns>
        public EmailTemplate Get(Int32 _Id)
        {
            using (EmailTemplateDataAccess data = new EmailTemplateDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a EmailTemplate .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public EmailTemplate Get(Int32 _Id, bool fillChild)
        {
            EmailTemplate emailTemplateObject;
            emailTemplateObject = Get(_Id);
            
            if (emailTemplateObject != null && fillChild)
            {
                // populate child data for a emailTemplateObject
                FillEmailTemplateWithChilds(emailTemplateObject, fillChild);
            }

            return emailTemplateObject;
        }
		
		/// <summary>
        /// populates a EmailTemplate with its child entities
        /// </summary>
        /// <param name="emailTemplate"></param>
		/// <param name="fillChilds"></param>
        private void FillEmailTemplateWithChilds(EmailTemplate emailTemplateObject, bool fillChilds)
        {
            // populate child data for a emailTemplateObject
            if (emailTemplateObject != null)
            {
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of EmailTemplate.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of EmailTemplate</returns>
        public EmailTemplateList GetAll()
        {
            using (EmailTemplateDataAccess data = new EmailTemplateDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of EmailTemplate.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of EmailTemplate</returns>
        public EmailTemplateList GetAll(bool fillChild)
        {
			EmailTemplateList emailTemplateList = new EmailTemplateList();
            using (EmailTemplateDataAccess data = new EmailTemplateDataAccess(ClientContext))
            {
                emailTemplateList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (EmailTemplate emailTemplateObject in emailTemplateList)
                {
					FillEmailTemplateWithChilds(emailTemplateObject, fillChild);
				}
			}
			return emailTemplateList;
        }
		
		/// <summary>
        /// Retrieve list of EmailTemplate  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of EmailTemplate</returns>
        public EmailTemplateList GetPaged(PagedRequest request)
        {
            using (EmailTemplateDataAccess data = new EmailTemplateDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of EmailTemplate  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of EmailTemplate</returns>
        public EmailTemplateList GetByQuery(String query)
        {
            using (EmailTemplateDataAccess data = new EmailTemplateDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get EmailTemplate Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of EmailTemplate
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (EmailTemplateDataAccess data = new EmailTemplateDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get EmailTemplate Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of EmailTemplate
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (EmailTemplateDataAccess data = new EmailTemplateDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		
		#endregion
	}	
}