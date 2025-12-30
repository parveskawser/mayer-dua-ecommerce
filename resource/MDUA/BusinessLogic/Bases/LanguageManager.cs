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
    /// Business logic processing for Language.
    /// </summary>    
	public partial class LanguageManager : BaseManager
	{
	
		#region Constructors
		public LanguageManager(ClientContext context) : base(context) { }
		public LanguageManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new language.
        /// data manipulation for insertion of Language
        /// </summary>
        /// <param name="languageObject"></param>
        /// <returns></returns>
        private bool Insert(Language languageObject)
        {
            // new language
            using (LanguageDataAccess data = new LanguageDataAccess(ClientContext))
            {
                // insert to languageObject
                Int32 _Id = data.Insert(languageObject);
                // if successful, process
                if (_Id > 0)
                {
                    languageObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of Language Object.
        /// Data manipulation processing for: new, deleted, updated Language
        /// </summary>
        /// <param name="languageObject"></param>
        /// <returns></returns>
        public bool UpdateBase(Language languageObject)
        {
            // use of switch for different types of DML
            switch (languageObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(languageObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(languageObject.Id);
            }
            // update rows
            using (LanguageDataAccess data = new LanguageDataAccess(ClientContext))
            {
                return (data.Update(languageObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for Language
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (LanguageDataAccess data = new LanguageDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve Language data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>Language Object</returns>
        public Language Get(Int32 _Id)
        {
            using (LanguageDataAccess data = new LanguageDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a Language .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public Language Get(Int32 _Id, bool fillChild)
        {
            Language languageObject;
            languageObject = Get(_Id);
            
            if (languageObject != null && fillChild)
            {
                // populate child data for a languageObject
                FillLanguageWithChilds(languageObject, fillChild);
            }

            return languageObject;
        }
		
		/// <summary>
        /// populates a Language with its child entities
        /// </summary>
        /// <param name="language"></param>
		/// <param name="fillChilds"></param>
        private void FillLanguageWithChilds(Language languageObject, bool fillChilds)
        {
            // populate child data for a languageObject
            if (languageObject != null)
            {
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of Language.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of Language</returns>
        public LanguageList GetAll()
        {
            using (LanguageDataAccess data = new LanguageDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of Language.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of Language</returns>
        public LanguageList GetAll(bool fillChild)
        {
			LanguageList languageList = new LanguageList();
            using (LanguageDataAccess data = new LanguageDataAccess(ClientContext))
            {
                languageList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (Language languageObject in languageList)
                {
					FillLanguageWithChilds(languageObject, fillChild);
				}
			}
			return languageList;
        }
		
		/// <summary>
        /// Retrieve list of Language  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of Language</returns>
        public LanguageList GetPaged(PagedRequest request)
        {
            using (LanguageDataAccess data = new LanguageDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of Language  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of Language</returns>
        public LanguageList GetByQuery(String query)
        {
            using (LanguageDataAccess data = new LanguageDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get Language Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Language
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (LanguageDataAccess data = new LanguageDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get Language Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Language
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (LanguageDataAccess data = new LanguageDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		
		#endregion
	}	
}