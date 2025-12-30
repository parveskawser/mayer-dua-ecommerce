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
    /// Business logic processing for ProductAttribute.
    /// </summary>    
	public partial class ProductAttributeManager : BaseManager
	{
	
		#region Constructors
		public ProductAttributeManager(ClientContext context) : base(context) { }
		public ProductAttributeManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new productAttribute.
        /// data manipulation for insertion of ProductAttribute
        /// </summary>
        /// <param name="productAttributeObject"></param>
        /// <returns></returns>
        private bool Insert(ProductAttribute productAttributeObject)
        {
            // new productAttribute
            using (ProductAttributeDataAccess data = new ProductAttributeDataAccess(ClientContext))
            {
                // insert to productAttributeObject
                Int32 _Id = data.Insert(productAttributeObject);
                // if successful, process
                if (_Id > 0)
                {
                    productAttributeObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of ProductAttribute Object.
        /// Data manipulation processing for: new, deleted, updated ProductAttribute
        /// </summary>
        /// <param name="productAttributeObject"></param>
        /// <returns></returns>
        public bool UpdateBase(ProductAttribute productAttributeObject)
        {
            // use of switch for different types of DML
            switch (productAttributeObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(productAttributeObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(productAttributeObject.Id);
            }
            // update rows
            using (ProductAttributeDataAccess data = new ProductAttributeDataAccess(ClientContext))
            {
                return (data.Update(productAttributeObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for ProductAttribute
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (ProductAttributeDataAccess data = new ProductAttributeDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve ProductAttribute data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>ProductAttribute Object</returns>
        public ProductAttribute Get(Int32 _Id)
        {
            using (ProductAttributeDataAccess data = new ProductAttributeDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a ProductAttribute .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public ProductAttribute Get(Int32 _Id, bool fillChild)
        {
            ProductAttribute productAttributeObject;
            productAttributeObject = Get(_Id);
            
            if (productAttributeObject != null && fillChild)
            {
                // populate child data for a productAttributeObject
                FillProductAttributeWithChilds(productAttributeObject, fillChild);
            }

            return productAttributeObject;
        }
		
		/// <summary>
        /// populates a ProductAttribute with its child entities
        /// </summary>
        /// <param name="productAttribute"></param>
		/// <param name="fillChilds"></param>
        private void FillProductAttributeWithChilds(ProductAttribute productAttributeObject, bool fillChilds)
        {
            // populate child data for a productAttributeObject
            if (productAttributeObject != null)
            {
				// Retrieve AttributeIdObject as AttributeName type for the ProductAttribute using AttributeId
				using(AttributeNameManager attributeNameManager = new AttributeNameManager(ClientContext))
				{
					productAttributeObject.AttributeIdObject = attributeNameManager.Get(productAttributeObject.AttributeId, fillChilds);
				}
				// Retrieve ProductIdObject as Product type for the ProductAttribute using ProductId
				using(ProductManager productManager = new ProductManager(ClientContext))
				{
					productAttributeObject.ProductIdObject = productManager.Get(productAttributeObject.ProductId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of ProductAttribute.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of ProductAttribute</returns>
        public ProductAttributeList GetAll()
        {
            using (ProductAttributeDataAccess data = new ProductAttributeDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of ProductAttribute.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of ProductAttribute</returns>
        public ProductAttributeList GetAll(bool fillChild)
        {
			ProductAttributeList productAttributeList = new ProductAttributeList();
            using (ProductAttributeDataAccess data = new ProductAttributeDataAccess(ClientContext))
            {
                productAttributeList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (ProductAttribute productAttributeObject in productAttributeList)
                {
					FillProductAttributeWithChilds(productAttributeObject, fillChild);
				}
			}
			return productAttributeList;
        }
		
		/// <summary>
        /// Retrieve list of ProductAttribute  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of ProductAttribute</returns>
        public ProductAttributeList GetPaged(PagedRequest request)
        {
            using (ProductAttributeDataAccess data = new ProductAttributeDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of ProductAttribute  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of ProductAttribute</returns>
        public ProductAttributeList GetByQuery(String query)
        {
            using (ProductAttributeDataAccess data = new ProductAttributeDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get ProductAttribute Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ProductAttribute
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (ProductAttributeDataAccess data = new ProductAttributeDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get ProductAttribute Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ProductAttribute
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (ProductAttributeDataAccess data = new ProductAttributeDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of ProductAttribute By ProductId        
		/// <param name="_ProductId"></param>
        /// </summary>
        /// <returns>List of ProductAttribute</returns>
        public ProductAttributeList GetByProductId(Int32 _ProductId)
        {
            using (ProductAttributeDataAccess data = new ProductAttributeDataAccess(ClientContext))
            {
                return data.GetByProductId(_ProductId);
            }
        }
		
		/// <summary>
        /// Retrieve list of ProductAttribute By AttributeId        
		/// <param name="_AttributeId"></param>
        /// </summary>
        /// <returns>List of ProductAttribute</returns>
        public ProductAttributeList GetByAttributeId(Int32 _AttributeId)
        {
            using (ProductAttributeDataAccess data = new ProductAttributeDataAccess(ClientContext))
            {
                return data.GetByAttributeId(_AttributeId);
            }
        }
		
		
		#endregion
	}	
}