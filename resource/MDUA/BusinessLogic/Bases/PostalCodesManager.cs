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
    /// Business logic processing for PostalCodes.
    /// </summary>    
	public partial class PostalCodesManager : BaseManager
	{
	
		#region Constructors
		public PostalCodesManager(ClientContext context) : base(context) { }
		public PostalCodesManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new postalCodes.
        /// data manipulation for insertion of PostalCodes
        /// </summary>
        /// <param name="postalCodesObject"></param>
        /// <returns></returns>
        private bool Insert(PostalCodes postalCodesObject)
        {
            // new postalCodes
            using (PostalCodesDataAccess data = new PostalCodesDataAccess(ClientContext))
            {
                // insert to postalCodesObject
                Int32 _Id = data.Insert(postalCodesObject);
                // if successful, process
                if (_Id > 0)
                {
                    postalCodesObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of PostalCodes Object.
        /// Data manipulation processing for: new, deleted, updated PostalCodes
        /// </summary>
        /// <param name="postalCodesObject"></param>
        /// <returns></returns>
        public bool UpdateBase(PostalCodes postalCodesObject)
        {
            // use of switch for different types of DML
            switch (postalCodesObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(postalCodesObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(postalCodesObject.Id);
            }
            // update rows
            using (PostalCodesDataAccess data = new PostalCodesDataAccess(ClientContext))
            {
                return (data.Update(postalCodesObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for PostalCodes
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (PostalCodesDataAccess data = new PostalCodesDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve PostalCodes data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>PostalCodes Object</returns>
        public PostalCodes Get(Int32 _Id)
        {
            using (PostalCodesDataAccess data = new PostalCodesDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a PostalCodes .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public PostalCodes Get(Int32 _Id, bool fillChild)
        {
            PostalCodes postalCodesObject;
            postalCodesObject = Get(_Id);
            
            if (postalCodesObject != null && fillChild)
            {
                // populate child data for a postalCodesObject
                FillPostalCodesWithChilds(postalCodesObject, fillChild);
            }

            return postalCodesObject;
        }
		
		/// <summary>
        /// populates a PostalCodes with its child entities
        /// </summary>
        /// <param name="postalCodes"></param>
		/// <param name="fillChilds"></param>
        private void FillPostalCodesWithChilds(PostalCodes postalCodesObject, bool fillChilds)
        {
            // populate child data for a postalCodesObject
            if (postalCodesObject != null)
            {
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of PostalCodes.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of PostalCodes</returns>
        public PostalCodesList GetAll()
        {
            using (PostalCodesDataAccess data = new PostalCodesDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of PostalCodes.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of PostalCodes</returns>
        public PostalCodesList GetAll(bool fillChild)
        {
			PostalCodesList postalCodesList = new PostalCodesList();
            using (PostalCodesDataAccess data = new PostalCodesDataAccess(ClientContext))
            {
                postalCodesList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (PostalCodes postalCodesObject in postalCodesList)
                {
					FillPostalCodesWithChilds(postalCodesObject, fillChild);
				}
			}
			return postalCodesList;
        }
		
		/// <summary>
        /// Retrieve list of PostalCodes  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of PostalCodes</returns>
        public PostalCodesList GetPaged(PagedRequest request)
        {
            using (PostalCodesDataAccess data = new PostalCodesDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of PostalCodes  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of PostalCodes</returns>
        public PostalCodesList GetByQuery(String query)
        {
            using (PostalCodesDataAccess data = new PostalCodesDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get PostalCodes Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of PostalCodes
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (PostalCodesDataAccess data = new PostalCodesDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get PostalCodes Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of PostalCodes
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (PostalCodesDataAccess data = new PostalCodesDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		
		#endregion
	}	
}