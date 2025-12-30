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
    /// Business logic processing for RecruitmentEligible.
    /// </summary>    
	public partial class RecruitmentEligibleManager : BaseManager
	{
	
		#region Constructors
		public RecruitmentEligibleManager(ClientContext context) : base(context) { }
		public RecruitmentEligibleManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new recruitmentEligible.
        /// data manipulation for insertion of RecruitmentEligible
        /// </summary>
        /// <param name="recruitmentEligibleObject"></param>
        /// <returns></returns>
        private bool Insert(RecruitmentEligible recruitmentEligibleObject)
        {
            // new recruitmentEligible
            using (RecruitmentEligibleDataAccess data = new RecruitmentEligibleDataAccess(ClientContext))
            {
                // insert to recruitmentEligibleObject
                Int32 _Id = data.Insert(recruitmentEligibleObject);
                // if successful, process
                if (_Id > 0)
                {
                    recruitmentEligibleObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of RecruitmentEligible Object.
        /// Data manipulation processing for: new, deleted, updated RecruitmentEligible
        /// </summary>
        /// <param name="recruitmentEligibleObject"></param>
        /// <returns></returns>
        public bool UpdateBase(RecruitmentEligible recruitmentEligibleObject)
        {
            // use of switch for different types of DML
            switch (recruitmentEligibleObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(recruitmentEligibleObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(recruitmentEligibleObject.Id);
            }
            // update rows
            using (RecruitmentEligibleDataAccess data = new RecruitmentEligibleDataAccess(ClientContext))
            {
                return (data.Update(recruitmentEligibleObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for RecruitmentEligible
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (RecruitmentEligibleDataAccess data = new RecruitmentEligibleDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve RecruitmentEligible data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>RecruitmentEligible Object</returns>
        public RecruitmentEligible Get(Int32 _Id)
        {
            using (RecruitmentEligibleDataAccess data = new RecruitmentEligibleDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a RecruitmentEligible .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public RecruitmentEligible Get(Int32 _Id, bool fillChild)
        {
            RecruitmentEligible recruitmentEligibleObject;
            recruitmentEligibleObject = Get(_Id);
            
            if (recruitmentEligibleObject != null && fillChild)
            {
                // populate child data for a recruitmentEligibleObject
                FillRecruitmentEligibleWithChilds(recruitmentEligibleObject, fillChild);
            }

            return recruitmentEligibleObject;
        }
		
		/// <summary>
        /// populates a RecruitmentEligible with its child entities
        /// </summary>
        /// <param name="recruitmentEligible"></param>
		/// <param name="fillChilds"></param>
        private void FillRecruitmentEligibleWithChilds(RecruitmentEligible recruitmentEligibleObject, bool fillChilds)
        {
            // populate child data for a recruitmentEligibleObject
            if (recruitmentEligibleObject != null)
            {
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of RecruitmentEligible.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of RecruitmentEligible</returns>
        public RecruitmentEligibleList GetAll()
        {
            using (RecruitmentEligibleDataAccess data = new RecruitmentEligibleDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of RecruitmentEligible.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of RecruitmentEligible</returns>
        public RecruitmentEligibleList GetAll(bool fillChild)
        {
			RecruitmentEligibleList recruitmentEligibleList = new RecruitmentEligibleList();
            using (RecruitmentEligibleDataAccess data = new RecruitmentEligibleDataAccess(ClientContext))
            {
                recruitmentEligibleList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (RecruitmentEligible recruitmentEligibleObject in recruitmentEligibleList)
                {
					FillRecruitmentEligibleWithChilds(recruitmentEligibleObject, fillChild);
				}
			}
			return recruitmentEligibleList;
        }
		
		/// <summary>
        /// Retrieve list of RecruitmentEligible  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of RecruitmentEligible</returns>
        public RecruitmentEligibleList GetPaged(PagedRequest request)
        {
            using (RecruitmentEligibleDataAccess data = new RecruitmentEligibleDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of RecruitmentEligible  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of RecruitmentEligible</returns>
        public RecruitmentEligibleList GetByQuery(String query)
        {
            using (RecruitmentEligibleDataAccess data = new RecruitmentEligibleDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get RecruitmentEligible Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of RecruitmentEligible
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (RecruitmentEligibleDataAccess data = new RecruitmentEligibleDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get RecruitmentEligible Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of RecruitmentEligible
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (RecruitmentEligibleDataAccess data = new RecruitmentEligibleDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		
		#endregion
	}	
}