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
    /// Business logic processing for Product.
    /// </summary>    
	public partial class ProductManager : BaseManager
	{
	
		#region Constructors
		public ProductManager(ClientContext context) : base(context) { }
		public ProductManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new product.
        /// data manipulation for insertion of Product
        /// </summary>
        /// <param name="productObject"></param>
        /// <returns></returns>
        private bool Insert(Product productObject)
        {
            // new product
            using (ProductDataAccess data = new ProductDataAccess(ClientContext))
            {
                // insert to productObject
                Int32 _Id = data.Insert(productObject);
                // if successful, process
                if (_Id > 0)
                {
                    productObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of Product Object.
        /// Data manipulation processing for: new, deleted, updated Product
        /// </summary>
        /// <param name="productObject"></param>
        /// <returns></returns>
        public bool UpdateBase(Product productObject)
        {
            // use of switch for different types of DML
            switch (productObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(productObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(productObject.Id);
            }
            // update rows
            using (ProductDataAccess data = new ProductDataAccess(ClientContext))
            {
                return (data.Update(productObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for Product
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (ProductDataAccess data = new ProductDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve Product data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>Product Object</returns>
        public Product Get(Int32 _Id)
        {
            using (ProductDataAccess data = new ProductDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a Product .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public Product Get(Int32 _Id, bool fillChild)
        {
            Product productObject;
            productObject = Get(_Id);
            
            if (productObject != null && fillChild)
            {
                // populate child data for a productObject
                FillProductWithChilds(productObject, fillChild);
            }

            return productObject;
        }
		
		/// <summary>
        /// populates a Product with its child entities
        /// </summary>
        /// <param name="product"></param>
		/// <param name="fillChilds"></param>
        private void FillProductWithChilds(Product productObject, bool fillChilds)
        {
            // populate child data for a productObject
            if (productObject != null)
            {
				// Retrieve CompanyIdObject as Company type for the Product using CompanyId
				using(CompanyManager companyManager = new CompanyManager(ClientContext))
				{
					productObject.CompanyIdObject = companyManager.Get(productObject.CompanyId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of Product.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of Product</returns>
        public ProductList GetAll()
        {
            using (ProductDataAccess data = new ProductDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of Product.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of Product</returns>
        public ProductList GetAll(bool fillChild)
        {
			ProductList productList = new ProductList();
            using (ProductDataAccess data = new ProductDataAccess(ClientContext))
            {
                productList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (Product productObject in productList)
                {
					FillProductWithChilds(productObject, fillChild);
				}
			}
			return productList;
        }
		
		/// <summary>
        /// Retrieve list of Product  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of Product</returns>
        public ProductList GetPaged(PagedRequest request)
        {
            using (ProductDataAccess data = new ProductDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of Product  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of Product</returns>
        public ProductList GetByQuery(String query)
        {
            using (ProductDataAccess data = new ProductDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get Product Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Product
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (ProductDataAccess data = new ProductDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get Product Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Product
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (ProductDataAccess data = new ProductDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of Product By CompanyId        
		/// <param name="_CompanyId"></param>
        /// </summary>
        /// <returns>List of Product</returns>
        public ProductList GetByCompanyId(Int32 _CompanyId)
        {
            using (ProductDataAccess data = new ProductDataAccess(ClientContext))
            {
                return data.GetByCompanyId(_CompanyId);
            }
        }
		
		
		#endregion
	}	
}