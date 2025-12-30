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
    /// Business logic processing for LanguageResource.
    /// </summary>    
	public partial class LanguageResourceManager : BaseManager
	{
	
		#region Constructors
		public LanguageResourceManager(ClientContext context) : base(context) { }
		public LanguageResourceManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new languageResource.
        /// data manipulation for insertion of LanguageResource
        /// </summary>
        /// <param name="languageResourceObject"></param>
        /// <returns></returns>
        private bool Insert(LanguageResource languageResourceObject)
        {
            // new languageResource
            using (LanguageResourceDataAccess data = new LanguageResourceDataAccess(ClientContext))
            {
                // insert to languageResourceObject
                Int32 _Id = data.Insert(languageResourceObject);
                // if successful, process
                if (_Id > 0)
                {
                    languageResourceObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of LanguageResource Object.
        /// Data manipulation processing for: new, deleted, updated LanguageResource
        /// </summary>
        /// <param name="languageResourceObject"></param>
        /// <returns></returns>
        public bool UpdateBase(LanguageResource languageResourceObject)
        {
            // use of switch for different types of DML
            switch (languageResourceObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(languageResourceObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(languageResourceObject.Id);
            }
            // update rows
            using (LanguageResourceDataAccess data = new LanguageResourceDataAccess(ClientContext))
            {
                return (data.Update(languageResourceObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for LanguageResource
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (LanguageResourceDataAccess data = new LanguageResourceDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve LanguageResource data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>LanguageResource Object</returns>
        public LanguageResource Get(Int32 _Id)
        {
            using (LanguageResourceDataAccess data = new LanguageResourceDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a LanguageResource .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public LanguageResource Get(Int32 _Id, bool fillChild)
        {
            LanguageResource languageResourceObject;
            languageResourceObject = Get(_Id);
            
            if (languageResourceObject != null && fillChild)
            {
                // populate child data for a languageResourceObject
                FillLanguageResourceWithChilds(languageResourceObject, fillChild);
            }

            return languageResourceObject;
        }
		
		/// <summary>
        /// populates a LanguageResource with its child entities
        /// </summary>
        /// <param name="languageResource"></param>
		/// <param name="fillChilds"></param>
        private void FillLanguageResourceWithChilds(LanguageResource languageResourceObject, bool fillChilds)
        {
            // populate child data for a languageResourceObject
            if (languageResourceObject != null)
            {
				// Retrieve CompanyIdObject as Company type for the LanguageResource using CompanyId
				using(CompanyManager companyManager = new CompanyManager(ClientContext))
				{
					languageResourceObject.CompanyIdObject = companyManager.Get(languageResourceObject.CompanyId, fillChilds);
				}
				// Retrieve LanguageIdObject as Language type for the LanguageResource using LanguageId
				using(LanguageManager languageManager = new LanguageManager(ClientContext))
				{
					languageResourceObject.LanguageIdObject = languageManager.Get(languageResourceObject.LanguageId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of LanguageResource.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of LanguageResource</returns>
        public LanguageResourceList GetAll()
        {
            using (LanguageResourceDataAccess data = new LanguageResourceDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of LanguageResource.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of LanguageResource</returns>
        public LanguageResourceList GetAll(bool fillChild)
        {
			LanguageResourceList languageResourceList = new LanguageResourceList();
            using (LanguageResourceDataAccess data = new LanguageResourceDataAccess(ClientContext))
            {
                languageResourceList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (LanguageResource languageResourceObject in languageResourceList)
                {
					FillLanguageResourceWithChilds(languageResourceObject, fillChild);
				}
			}
			return languageResourceList;
        }
		
		/// <summary>
        /// Retrieve list of LanguageResource  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of LanguageResource</returns>
        public LanguageResourceList GetPaged(PagedRequest request)
        {
            using (LanguageResourceDataAccess data = new LanguageResourceDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of LanguageResource  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of LanguageResource</returns>
        public LanguageResourceList GetByQuery(String query)
        {
            using (LanguageResourceDataAccess data = new LanguageResourceDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get LanguageResource Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of LanguageResource
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (LanguageResourceDataAccess data = new LanguageResourceDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get LanguageResource Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of LanguageResource
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (LanguageResourceDataAccess data = new LanguageResourceDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of LanguageResource By CompanyId        
		/// <param name="_CompanyId"></param>
        /// </summary>
        /// <returns>List of LanguageResource</returns>
        public LanguageResourceList GetByCompanyId(Nullable<Int32> _CompanyId)
        {
            using (LanguageResourceDataAccess data = new LanguageResourceDataAccess(ClientContext))
            {
                return data.GetByCompanyId(_CompanyId);
            }
        }
		
		/// <summary>
        /// Retrieve list of LanguageResource By LanguageId        
		/// <param name="_LanguageId"></param>
        /// </summary>
        /// <returns>List of LanguageResource</returns>
        public LanguageResourceList GetByLanguageId(Int32 _LanguageId)
        {
            using (LanguageResourceDataAccess data = new LanguageResourceDataAccess(ClientContext))
            {
                return data.GetByLanguageId(_LanguageId);
            }
        }
		
		
		#endregion
	}	
}