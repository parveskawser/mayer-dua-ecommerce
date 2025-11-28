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
    /// Business logic processing for ProductInventory.
    /// </summary>    
	public partial class ProductInventoryManager : BaseManager
	{
	
		#region Constructors
		public ProductInventoryManager(ClientContext context) : base(context) { }
		public ProductInventoryManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new productInventory.
        /// data manipulation for insertion of ProductInventory
        /// </summary>
        /// <param name="productInventoryObject"></param>
        /// <returns></returns>
        private bool Insert(ProductInventory productInventoryObject)
        {
            // new productInventory
            using (ProductInventoryDataAccess data = new ProductInventoryDataAccess(ClientContext))
            {
                // insert to productInventoryObject
                Int32 _Id = data.Insert(productInventoryObject);
                // if successful, process
                if (_Id > 0)
                {
                    productInventoryObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of ProductInventory Object.
        /// Data manipulation processing for: new, deleted, updated ProductInventory
        /// </summary>
        /// <param name="productInventoryObject"></param>
        /// <returns></returns>
        public bool UpdateBase(ProductInventory productInventoryObject)
        {
            // use of switch for different types of DML
            switch (productInventoryObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(productInventoryObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(productInventoryObject.Id);
            }
            // update rows
            using (ProductInventoryDataAccess data = new ProductInventoryDataAccess(ClientContext))
            {
                return (data.Update(productInventoryObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for ProductInventory
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (ProductInventoryDataAccess data = new ProductInventoryDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve ProductInventory data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>ProductInventory Object</returns>
        public ProductInventory Get(Int32 _Id)
        {
            using (ProductInventoryDataAccess data = new ProductInventoryDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a ProductInventory .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public ProductInventory Get(Int32 _Id, bool fillChild)
        {
            ProductInventory productInventoryObject;
            productInventoryObject = Get(_Id);
            
            if (productInventoryObject != null && fillChild)
            {
                // populate child data for a productInventoryObject
                FillProductInventoryWithChilds(productInventoryObject, fillChild);
            }

            return productInventoryObject;
        }
		
		/// <summary>
        /// populates a ProductInventory with its child entities
        /// </summary>
        /// <param name="productInventory"></param>
		/// <param name="fillChilds"></param>
        private void FillProductInventoryWithChilds(ProductInventory productInventoryObject, bool fillChilds)
        {
            // populate child data for a productInventoryObject
            if (productInventoryObject != null)
            {
				// Retrieve ProductIdObject as Product type for the ProductInventory using ProductId
				using(ProductManager productManager = new ProductManager(ClientContext))
				{
					productInventoryObject.ProductIdObject = productManager.Get(productInventoryObject.ProductId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of ProductInventory.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of ProductInventory</returns>
        public ProductInventoryList GetAll()
        {
            using (ProductInventoryDataAccess data = new ProductInventoryDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of ProductInventory.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of ProductInventory</returns>
        public ProductInventoryList GetAll(bool fillChild)
        {
			ProductInventoryList productInventoryList = new ProductInventoryList();
            using (ProductInventoryDataAccess data = new ProductInventoryDataAccess(ClientContext))
            {
                productInventoryList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (ProductInventory productInventoryObject in productInventoryList)
                {
					FillProductInventoryWithChilds(productInventoryObject, fillChild);
				}
			}
			return productInventoryList;
        }
		
		/// <summary>
        /// Retrieve list of ProductInventory  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of ProductInventory</returns>
        public ProductInventoryList GetPaged(PagedRequest request)
        {
            using (ProductInventoryDataAccess data = new ProductInventoryDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of ProductInventory  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of ProductInventory</returns>
        public ProductInventoryList GetByQuery(String query)
        {
            using (ProductInventoryDataAccess data = new ProductInventoryDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get ProductInventory Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ProductInventory
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (ProductInventoryDataAccess data = new ProductInventoryDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get ProductInventory Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ProductInventory
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (ProductInventoryDataAccess data = new ProductInventoryDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of ProductInventory By ProductId        
		/// <param name="_ProductId"></param>
        /// </summary>
        /// <returns>List of ProductInventory</returns>
        public ProductInventoryList GetByProductId(Int32 _ProductId)
        {
            using (ProductInventoryDataAccess data = new ProductInventoryDataAccess(ClientContext))
            {
                return data.GetByProductId(_ProductId);
            }
        }
		
		
		#endregion
	}	
}
