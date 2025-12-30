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
    /// Business logic processing for AttributeValue.
    /// </summary>    
	public partial class AttributeValueManager : BaseManager
	{
	
		#region Constructors
		public AttributeValueManager(ClientContext context) : base(context) { }
		public AttributeValueManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new attributeValue.
        /// data manipulation for insertion of AttributeValue
        /// </summary>
        /// <param name="attributeValueObject"></param>
        /// <returns></returns>
        private bool Insert(AttributeValue attributeValueObject)
        {
            // new attributeValue
            using (AttributeValueDataAccess data = new AttributeValueDataAccess(ClientContext))
            {
                // insert to attributeValueObject
                Int32 _Id = data.Insert(attributeValueObject);
                // if successful, process
                if (_Id > 0)
                {
                    attributeValueObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of AttributeValue Object.
        /// Data manipulation processing for: new, deleted, updated AttributeValue
        /// </summary>
        /// <param name="attributeValueObject"></param>
        /// <returns></returns>
        public bool UpdateBase(AttributeValue attributeValueObject)
        {
            // use of switch for different types of DML
            switch (attributeValueObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(attributeValueObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(attributeValueObject.Id);
            }
            // update rows
            using (AttributeValueDataAccess data = new AttributeValueDataAccess(ClientContext))
            {
                return (data.Update(attributeValueObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for AttributeValue
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (AttributeValueDataAccess data = new AttributeValueDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve AttributeValue data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>AttributeValue Object</returns>
        public AttributeValue Get(Int32 _Id)
        {
            using (AttributeValueDataAccess data = new AttributeValueDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a AttributeValue .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public AttributeValue Get(Int32 _Id, bool fillChild)
        {
            AttributeValue attributeValueObject;
            attributeValueObject = Get(_Id);
            
            if (attributeValueObject != null && fillChild)
            {
                // populate child data for a attributeValueObject
                FillAttributeValueWithChilds(attributeValueObject, fillChild);
            }

            return attributeValueObject;
        }
		
		/// <summary>
        /// populates a AttributeValue with its child entities
        /// </summary>
        /// <param name="attributeValue"></param>
		/// <param name="fillChilds"></param>
        private void FillAttributeValueWithChilds(AttributeValue attributeValueObject, bool fillChilds)
        {
            // populate child data for a attributeValueObject
            if (attributeValueObject != null)
            {
				// Retrieve AttributeIdObject as AttributeName type for the AttributeValue using AttributeId
				using(AttributeNameManager attributeNameManager = new AttributeNameManager(ClientContext))
				{
					attributeValueObject.AttributeIdObject = attributeNameManager.Get(attributeValueObject.AttributeId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of AttributeValue.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of AttributeValue</returns>
        public AttributeValueList GetAll()
        {
            using (AttributeValueDataAccess data = new AttributeValueDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of AttributeValue.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of AttributeValue</returns>
        public AttributeValueList GetAll(bool fillChild)
        {
			AttributeValueList attributeValueList = new AttributeValueList();
            using (AttributeValueDataAccess data = new AttributeValueDataAccess(ClientContext))
            {
                attributeValueList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (AttributeValue attributeValueObject in attributeValueList)
                {
					FillAttributeValueWithChilds(attributeValueObject, fillChild);
				}
			}
			return attributeValueList;
        }
		
		/// <summary>
        /// Retrieve list of AttributeValue  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of AttributeValue</returns>
        public AttributeValueList GetPaged(PagedRequest request)
        {
            using (AttributeValueDataAccess data = new AttributeValueDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of AttributeValue  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of AttributeValue</returns>
        public AttributeValueList GetByQuery(String query)
        {
            using (AttributeValueDataAccess data = new AttributeValueDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get AttributeValue Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of AttributeValue
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (AttributeValueDataAccess data = new AttributeValueDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get AttributeValue Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of AttributeValue
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (AttributeValueDataAccess data = new AttributeValueDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of AttributeValue By AttributeId        
		/// <param name="_AttributeId"></param>
        /// </summary>
        /// <returns>List of AttributeValue</returns>
        public AttributeValueList GetByAttributeId(Int32 _AttributeId)
        {
            using (AttributeValueDataAccess data = new AttributeValueDataAccess(ClientContext))
            {
                return data.GetByAttributeId(_AttributeId);
            }
        }
		
		
		#endregion
	}	
}