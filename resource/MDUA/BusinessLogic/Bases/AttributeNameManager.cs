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
    /// Business logic processing for AttributeName.
    /// </summary>    
	public partial class AttributeNameManager : BaseManager
	{
	
		#region Constructors
		public AttributeNameManager(ClientContext context) : base(context) { }
		public AttributeNameManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new attributeName.
        /// data manipulation for insertion of AttributeName
        /// </summary>
        /// <param name="attributeNameObject"></param>
        /// <returns></returns>
        private bool Insert(AttributeName attributeNameObject)
        {
            // new attributeName
            using (AttributeNameDataAccess data = new AttributeNameDataAccess(ClientContext))
            {
                // insert to attributeNameObject
                Int32 _Id = data.Insert(attributeNameObject);
                // if successful, process
                if (_Id > 0)
                {
                    attributeNameObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of AttributeName Object.
        /// Data manipulation processing for: new, deleted, updated AttributeName
        /// </summary>
        /// <param name="attributeNameObject"></param>
        /// <returns></returns>
        public bool UpdateBase(AttributeName attributeNameObject)
        {
            // use of switch for different types of DML
            switch (attributeNameObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(attributeNameObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(attributeNameObject.Id);
            }
            // update rows
            using (AttributeNameDataAccess data = new AttributeNameDataAccess(ClientContext))
            {
                return (data.Update(attributeNameObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for AttributeName
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (AttributeNameDataAccess data = new AttributeNameDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve AttributeName data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>AttributeName Object</returns>
        public AttributeName Get(Int32 _Id)
        {
            using (AttributeNameDataAccess data = new AttributeNameDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a AttributeName .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public AttributeName Get(Int32 _Id, bool fillChild)
        {
            AttributeName attributeNameObject;
            attributeNameObject = Get(_Id);
            
            if (attributeNameObject != null && fillChild)
            {
                // populate child data for a attributeNameObject
                FillAttributeNameWithChilds(attributeNameObject, fillChild);
            }

            return attributeNameObject;
        }
		
		/// <summary>
        /// populates a AttributeName with its child entities
        /// </summary>
        /// <param name="attributeName"></param>
		/// <param name="fillChilds"></param>
        private void FillAttributeNameWithChilds(AttributeName attributeNameObject, bool fillChilds)
        {
            // populate child data for a attributeNameObject
            if (attributeNameObject != null)
            {
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of AttributeName.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of AttributeName</returns>
        public AttributeNameList GetAll()
        {
            using (AttributeNameDataAccess data = new AttributeNameDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of AttributeName.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of AttributeName</returns>
        public AttributeNameList GetAll(bool fillChild)
        {
			AttributeNameList attributeNameList = new AttributeNameList();
            using (AttributeNameDataAccess data = new AttributeNameDataAccess(ClientContext))
            {
                attributeNameList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (AttributeName attributeNameObject in attributeNameList)
                {
					FillAttributeNameWithChilds(attributeNameObject, fillChild);
				}
			}
			return attributeNameList;
        }
		
		/// <summary>
        /// Retrieve list of AttributeName  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of AttributeName</returns>
        public AttributeNameList GetPaged(PagedRequest request)
        {
            using (AttributeNameDataAccess data = new AttributeNameDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of AttributeName  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of AttributeName</returns>
        public AttributeNameList GetByQuery(String query)
        {
            using (AttributeNameDataAccess data = new AttributeNameDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get AttributeName Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of AttributeName
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (AttributeNameDataAccess data = new AttributeNameDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get AttributeName Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of AttributeName
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (AttributeNameDataAccess data = new AttributeNameDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		
		#endregion
	}	
}