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
    /// Business logic processing for AuditLog.
    /// </summary>    
	public partial class AuditLogManager : BaseManager
	{
	
		#region Constructors
		public AuditLogManager(ClientContext context) : base(context) { }
		public AuditLogManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new auditLog.
        /// data manipulation for insertion of AuditLog
        /// </summary>
        /// <param name="auditLogObject"></param>
        /// <returns></returns>
        private bool Insert(AuditLog auditLogObject)
        {
            // new auditLog
            using (AuditLogDataAccess data = new AuditLogDataAccess(ClientContext))
            {
                // insert to auditLogObject
                Int32 _Id = data.Insert(auditLogObject);
                // if successful, process
                if (_Id > 0)
                {
                    auditLogObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of AuditLog Object.
        /// Data manipulation processing for: new, deleted, updated AuditLog
        /// </summary>
        /// <param name="auditLogObject"></param>
        /// <returns></returns>
        public bool UpdateBase(AuditLog auditLogObject)
        {
            // use of switch for different types of DML
            switch (auditLogObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(auditLogObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(auditLogObject.Id);
            }
            // update rows
            using (AuditLogDataAccess data = new AuditLogDataAccess(ClientContext))
            {
                return (data.Update(auditLogObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for AuditLog
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (AuditLogDataAccess data = new AuditLogDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve AuditLog data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>AuditLog Object</returns>
        public AuditLog Get(Int32 _Id)
        {
            using (AuditLogDataAccess data = new AuditLogDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a AuditLog .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public AuditLog Get(Int32 _Id, bool fillChild)
        {
            AuditLog auditLogObject;
            auditLogObject = Get(_Id);
            
            if (auditLogObject != null && fillChild)
            {
                // populate child data for a auditLogObject
                FillAuditLogWithChilds(auditLogObject, fillChild);
            }

            return auditLogObject;
        }
		
		/// <summary>
        /// populates a AuditLog with its child entities
        /// </summary>
        /// <param name="auditLog"></param>
		/// <param name="fillChilds"></param>
        private void FillAuditLogWithChilds(AuditLog auditLogObject, bool fillChilds)
        {
            // populate child data for a auditLogObject
            if (auditLogObject != null)
            {
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of AuditLog.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of AuditLog</returns>
        public AuditLogList GetAll()
        {
            using (AuditLogDataAccess data = new AuditLogDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of AuditLog.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of AuditLog</returns>
        public AuditLogList GetAll(bool fillChild)
        {
			AuditLogList auditLogList = new AuditLogList();
            using (AuditLogDataAccess data = new AuditLogDataAccess(ClientContext))
            {
                auditLogList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (AuditLog auditLogObject in auditLogList)
                {
					FillAuditLogWithChilds(auditLogObject, fillChild);
				}
			}
			return auditLogList;
        }
		
		/// <summary>
        /// Retrieve list of AuditLog  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of AuditLog</returns>
        public AuditLogList GetPaged(PagedRequest request)
        {
            using (AuditLogDataAccess data = new AuditLogDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of AuditLog  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of AuditLog</returns>
        public AuditLogList GetByQuery(String query)
        {
            using (AuditLogDataAccess data = new AuditLogDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get AuditLog Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of AuditLog
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (AuditLogDataAccess data = new AuditLogDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get AuditLog Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of AuditLog
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (AuditLogDataAccess data = new AuditLogDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		
		#endregion
	}	
}