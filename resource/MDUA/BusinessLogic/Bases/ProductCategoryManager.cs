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
    /// Business logic processing for ProductCategory.
    /// </summary>    
	public partial class ProductCategoryManager : BaseManager
	{
	
		#region Constructors
		public ProductCategoryManager(ClientContext context) : base(context) { }
		public ProductCategoryManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new productCategory.
        /// data manipulation for insertion of ProductCategory
        /// </summary>
        /// <param name="productCategoryObject"></param>
        /// <returns></returns>
        private bool Insert(ProductCategory productCategoryObject)
        {
            // new productCategory
            using (ProductCategoryDataAccess data = new ProductCategoryDataAccess(ClientContext))
            {
                // insert to productCategoryObject
                Int32 _Id = data.Insert(productCategoryObject);
                // if successful, process
                if (_Id > 0)
                {
                    productCategoryObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of ProductCategory Object.
        /// Data manipulation processing for: new, deleted, updated ProductCategory
        /// </summary>
        /// <param name="productCategoryObject"></param>
        /// <returns></returns>
        public bool UpdateBase(ProductCategory productCategoryObject)
        {
            // use of switch for different types of DML
            switch (productCategoryObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(productCategoryObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(productCategoryObject.Id);
            }
            // update rows
            using (ProductCategoryDataAccess data = new ProductCategoryDataAccess(ClientContext))
            {
                return (data.Update(productCategoryObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for ProductCategory
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (ProductCategoryDataAccess data = new ProductCategoryDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve ProductCategory data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>ProductCategory Object</returns>
        public ProductCategory Get(Int32 _Id)
        {
            using (ProductCategoryDataAccess data = new ProductCategoryDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a ProductCategory .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public ProductCategory Get(Int32 _Id, bool fillChild)
        {
            ProductCategory productCategoryObject;
            productCategoryObject = Get(_Id);
            
            if (productCategoryObject != null && fillChild)
            {
                // populate child data for a productCategoryObject
                FillProductCategoryWithChilds(productCategoryObject, fillChild);
            }

            return productCategoryObject;
        }
		
		/// <summary>
        /// populates a ProductCategory with its child entities
        /// </summary>
        /// <param name="productCategory"></param>
		/// <param name="fillChilds"></param>
        private void FillProductCategoryWithChilds(ProductCategory productCategoryObject, bool fillChilds)
        {
            // populate child data for a productCategoryObject
            if (productCategoryObject != null)
            {
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of ProductCategory.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of ProductCategory</returns>
        public ProductCategoryList GetAll()
        {
            using (ProductCategoryDataAccess data = new ProductCategoryDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of ProductCategory.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of ProductCategory</returns>
        public ProductCategoryList GetAll(bool fillChild)
        {
			ProductCategoryList productCategoryList = new ProductCategoryList();
            using (ProductCategoryDataAccess data = new ProductCategoryDataAccess(ClientContext))
            {
                productCategoryList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (ProductCategory productCategoryObject in productCategoryList)
                {
					FillProductCategoryWithChilds(productCategoryObject, fillChild);
				}
			}
			return productCategoryList;
        }
		
		/// <summary>
        /// Retrieve list of ProductCategory  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of ProductCategory</returns>
        public ProductCategoryList GetPaged(PagedRequest request)
        {
            using (ProductCategoryDataAccess data = new ProductCategoryDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of ProductCategory  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of ProductCategory</returns>
        public ProductCategoryList GetByQuery(String query)
        {
            using (ProductCategoryDataAccess data = new ProductCategoryDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get ProductCategory Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ProductCategory
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (ProductCategoryDataAccess data = new ProductCategoryDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get ProductCategory Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ProductCategory
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (ProductCategoryDataAccess data = new ProductCategoryDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		
		#endregion
	}	
}