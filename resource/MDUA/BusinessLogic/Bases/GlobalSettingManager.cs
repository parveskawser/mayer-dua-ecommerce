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
    /// Business logic processing for GlobalSetting.
    /// </summary>    
	public partial class GlobalSettingManager : BaseManager
	{
	
		#region Constructors
		public GlobalSettingManager(ClientContext context) : base(context) { }
		public GlobalSettingManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new globalSetting.
        /// data manipulation for insertion of GlobalSetting
        /// </summary>
        /// <param name="globalSettingObject"></param>
        /// <returns></returns>
        private bool Insert(GlobalSetting globalSettingObject)
        {
            // new globalSetting
            using (GlobalSettingDataAccess data = new GlobalSettingDataAccess(ClientContext))
            {
                // insert to globalSettingObject
                Int32 _Id = data.Insert(globalSettingObject);
                // if successful, process
                if (_Id > 0)
                {
                    globalSettingObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of GlobalSetting Object.
        /// Data manipulation processing for: new, deleted, updated GlobalSetting
        /// </summary>
        /// <param name="globalSettingObject"></param>
        /// <returns></returns>
        public bool UpdateBase(GlobalSetting globalSettingObject)
        {
            // use of switch for different types of DML
            switch (globalSettingObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(globalSettingObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(globalSettingObject.Id);
            }
            // update rows
            using (GlobalSettingDataAccess data = new GlobalSettingDataAccess(ClientContext))
            {
                return (data.Update(globalSettingObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for GlobalSetting
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (GlobalSettingDataAccess data = new GlobalSettingDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve GlobalSetting data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>GlobalSetting Object</returns>
        public GlobalSetting Get(Int32 _Id)
        {
            using (GlobalSettingDataAccess data = new GlobalSettingDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a GlobalSetting .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public GlobalSetting Get(Int32 _Id, bool fillChild)
        {
            GlobalSetting globalSettingObject;
            globalSettingObject = Get(_Id);
            
            if (globalSettingObject != null && fillChild)
            {
                // populate child data for a globalSettingObject
                FillGlobalSettingWithChilds(globalSettingObject, fillChild);
            }

            return globalSettingObject;
        }
		
		/// <summary>
        /// populates a GlobalSetting with its child entities
        /// </summary>
        /// <param name="globalSetting"></param>
		/// <param name="fillChilds"></param>
        private void FillGlobalSettingWithChilds(GlobalSetting globalSettingObject, bool fillChilds)
        {
            // populate child data for a globalSettingObject
            if (globalSettingObject != null)
            {
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of GlobalSetting.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of GlobalSetting</returns>
        public GlobalSettingList GetAll()
        {
            using (GlobalSettingDataAccess data = new GlobalSettingDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of GlobalSetting.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of GlobalSetting</returns>
        public GlobalSettingList GetAll(bool fillChild)
        {
			GlobalSettingList globalSettingList = new GlobalSettingList();
            using (GlobalSettingDataAccess data = new GlobalSettingDataAccess(ClientContext))
            {
                globalSettingList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (GlobalSetting globalSettingObject in globalSettingList)
                {
					FillGlobalSettingWithChilds(globalSettingObject, fillChild);
				}
			}
			return globalSettingList;
        }
		
		/// <summary>
        /// Retrieve list of GlobalSetting  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of GlobalSetting</returns>
        public GlobalSettingList GetPaged(PagedRequest request)
        {
            using (GlobalSettingDataAccess data = new GlobalSettingDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of GlobalSetting  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of GlobalSetting</returns>
        public GlobalSettingList GetByQuery(String query)
        {
            using (GlobalSettingDataAccess data = new GlobalSettingDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get GlobalSetting Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of GlobalSetting
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (GlobalSettingDataAccess data = new GlobalSettingDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get GlobalSetting Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of GlobalSetting
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (GlobalSettingDataAccess data = new GlobalSettingDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		
		#endregion
	}	
}