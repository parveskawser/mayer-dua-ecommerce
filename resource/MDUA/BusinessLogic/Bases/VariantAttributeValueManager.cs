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
    /// Business logic processing for VariantAttributeValue.
    /// </summary>    
	public partial class VariantAttributeValueManager : BaseManager
	{
	
		#region Constructors
		public VariantAttributeValueManager(ClientContext context) : base(context) { }
		public VariantAttributeValueManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new variantAttributeValue.
        /// data manipulation for insertion of VariantAttributeValue
        /// </summary>
        /// <param name="variantAttributeValueObject"></param>
        /// <returns></returns>
        private bool Insert(VariantAttributeValue variantAttributeValueObject)
        {
            // new variantAttributeValue
            using (VariantAttributeValueDataAccess data = new VariantAttributeValueDataAccess(ClientContext))
            {
                // insert to variantAttributeValueObject
                Int32 _Id = data.Insert(variantAttributeValueObject);
                // if successful, process
                if (_Id > 0)
                {
                    variantAttributeValueObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of VariantAttributeValue Object.
        /// Data manipulation processing for: new, deleted, updated VariantAttributeValue
        /// </summary>
        /// <param name="variantAttributeValueObject"></param>
        /// <returns></returns>
        public bool UpdateBase(VariantAttributeValue variantAttributeValueObject)
        {
            // use of switch for different types of DML
            switch (variantAttributeValueObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(variantAttributeValueObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(variantAttributeValueObject.Id);
            }
            // update rows
            using (VariantAttributeValueDataAccess data = new VariantAttributeValueDataAccess(ClientContext))
            {
                return (data.Update(variantAttributeValueObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for VariantAttributeValue
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (VariantAttributeValueDataAccess data = new VariantAttributeValueDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve VariantAttributeValue data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>VariantAttributeValue Object</returns>
        public VariantAttributeValue Get(Int32 _Id)
        {
            using (VariantAttributeValueDataAccess data = new VariantAttributeValueDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a VariantAttributeValue .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public VariantAttributeValue Get(Int32 _Id, bool fillChild)
        {
            VariantAttributeValue variantAttributeValueObject;
            variantAttributeValueObject = Get(_Id);
            
            if (variantAttributeValueObject != null && fillChild)
            {
                // populate child data for a variantAttributeValueObject
                FillVariantAttributeValueWithChilds(variantAttributeValueObject, fillChild);
            }

            return variantAttributeValueObject;
        }
		
		/// <summary>
        /// populates a VariantAttributeValue with its child entities
        /// </summary>
        /// <param name="variantAttributeValue"></param>
		/// <param name="fillChilds"></param>
        private void FillVariantAttributeValueWithChilds(VariantAttributeValue variantAttributeValueObject, bool fillChilds)
        {
            // populate child data for a variantAttributeValueObject
            if (variantAttributeValueObject != null)
            {
				// Retrieve AttributeIdObject as AttributeName type for the VariantAttributeValue using AttributeId
				using(AttributeNameManager attributeNameManager = new AttributeNameManager(ClientContext))
				{
					variantAttributeValueObject.AttributeIdObject = attributeNameManager.Get(variantAttributeValueObject.AttributeId, fillChilds);
				}
				// Retrieve AttributeValueIdObject as AttributeValue type for the VariantAttributeValue using AttributeValueId
				using(AttributeValueManager attributeValueManager = new AttributeValueManager(ClientContext))
				{
					variantAttributeValueObject.AttributeValueIdObject = attributeValueManager.Get(variantAttributeValueObject.AttributeValueId, fillChilds);
				}
				// Retrieve VariantIdObject as ProductVariant type for the VariantAttributeValue using VariantId
				using(ProductVariantManager productVariantManager = new ProductVariantManager(ClientContext))
				{
					variantAttributeValueObject.VariantIdObject = productVariantManager.Get(variantAttributeValueObject.VariantId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of VariantAttributeValue.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of VariantAttributeValue</returns>
        public VariantAttributeValueList GetAll()
        {
            using (VariantAttributeValueDataAccess data = new VariantAttributeValueDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of VariantAttributeValue.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of VariantAttributeValue</returns>
        public VariantAttributeValueList GetAll(bool fillChild)
        {
			VariantAttributeValueList variantAttributeValueList = new VariantAttributeValueList();
            using (VariantAttributeValueDataAccess data = new VariantAttributeValueDataAccess(ClientContext))
            {
                variantAttributeValueList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (VariantAttributeValue variantAttributeValueObject in variantAttributeValueList)
                {
					FillVariantAttributeValueWithChilds(variantAttributeValueObject, fillChild);
				}
			}
			return variantAttributeValueList;
        }
		
		/// <summary>
        /// Retrieve list of VariantAttributeValue  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of VariantAttributeValue</returns>
        public VariantAttributeValueList GetPaged(PagedRequest request)
        {
            using (VariantAttributeValueDataAccess data = new VariantAttributeValueDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of VariantAttributeValue  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of VariantAttributeValue</returns>
        public VariantAttributeValueList GetByQuery(String query)
        {
            using (VariantAttributeValueDataAccess data = new VariantAttributeValueDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get VariantAttributeValue Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of VariantAttributeValue
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (VariantAttributeValueDataAccess data = new VariantAttributeValueDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get VariantAttributeValue Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of VariantAttributeValue
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (VariantAttributeValueDataAccess data = new VariantAttributeValueDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of VariantAttributeValue By VariantId        
		/// <param name="_VariantId"></param>
        /// </summary>
        /// <returns>List of VariantAttributeValue</returns>
        public VariantAttributeValueList GetByVariantId(Int32 _VariantId)
        {
            using (VariantAttributeValueDataAccess data = new VariantAttributeValueDataAccess(ClientContext))
            {
                return data.GetByVariantId(_VariantId);
            }
        }
		
		/// <summary>
        /// Retrieve list of VariantAttributeValue By AttributeId        
		/// <param name="_AttributeId"></param>
        /// </summary>
        /// <returns>List of VariantAttributeValue</returns>
        public VariantAttributeValueList GetByAttributeId(Int32 _AttributeId)
        {
            using (VariantAttributeValueDataAccess data = new VariantAttributeValueDataAccess(ClientContext))
            {
                return data.GetByAttributeId(_AttributeId);
            }
        }
		
		/// <summary>
        /// Retrieve list of VariantAttributeValue By AttributeValueId        
		/// <param name="_AttributeValueId"></param>
        /// </summary>
        /// <returns>List of VariantAttributeValue</returns>
        public VariantAttributeValueList GetByAttributeValueId(Int32 _AttributeValueId)
        {
            using (VariantAttributeValueDataAccess data = new VariantAttributeValueDataAccess(ClientContext))
            {
                return data.GetByAttributeValueId(_AttributeValueId);
            }
        }
		
		
		#endregion
	}	
}