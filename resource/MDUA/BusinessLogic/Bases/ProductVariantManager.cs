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
    /// Business logic processing for ProductVariant.
    /// </summary>    
	public partial class ProductVariantManager : BaseManager
	{
	
		#region Constructors
		public ProductVariantManager(ClientContext context) : base(context) { }
		public ProductVariantManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new productVariant.
        /// data manipulation for insertion of ProductVariant
        /// </summary>
        /// <param name="productVariantObject"></param>
        /// <returns></returns>
        private bool Insert(ProductVariant productVariantObject)
        {
            // new productVariant
            using (ProductVariantDataAccess data = new ProductVariantDataAccess(ClientContext))
            {
                // insert to productVariantObject
                Int32 _Id = data.Insert(productVariantObject);
                // if successful, process
                if (_Id > 0)
                {
                    productVariantObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of ProductVariant Object.
        /// Data manipulation processing for: new, deleted, updated ProductVariant
        /// </summary>
        /// <param name="productVariantObject"></param>
        /// <returns></returns>
        public bool UpdateBase(ProductVariant productVariantObject)
        {
            // use of switch for different types of DML
            switch (productVariantObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(productVariantObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(productVariantObject.Id);
            }
            // update rows
            using (ProductVariantDataAccess data = new ProductVariantDataAccess(ClientContext))
            {
                return (data.Update(productVariantObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for ProductVariant
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (ProductVariantDataAccess data = new ProductVariantDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve ProductVariant data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>ProductVariant Object</returns>
        public ProductVariant Get(Int32 _Id)
        {
            using (ProductVariantDataAccess data = new ProductVariantDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a ProductVariant .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public ProductVariant Get(Int32 _Id, bool fillChild)
        {
            ProductVariant productVariantObject;
            productVariantObject = Get(_Id);
            
            if (productVariantObject != null && fillChild)
            {
                // populate child data for a productVariantObject
                FillProductVariantWithChilds(productVariantObject, fillChild);
            }

            return productVariantObject;
        }
		
		/// <summary>
        /// populates a ProductVariant with its child entities
        /// </summary>
        /// <param name="productVariant"></param>
		/// <param name="fillChilds"></param>
        private void FillProductVariantWithChilds(ProductVariant productVariantObject, bool fillChilds)
        {
            // populate child data for a productVariantObject
            if (productVariantObject != null)
            {
				// Retrieve ProductIdObject as Product type for the ProductVariant using ProductId
				using(ProductManager productManager = new ProductManager(ClientContext))
				{
					productVariantObject.ProductIdObject = productManager.Get(productVariantObject.ProductId, fillChilds);
				}
				// Retrieve IdObject as ProductVariant type for the ProductVariant using Id
				using(ProductVariantManager productVariantManager = new ProductVariantManager(ClientContext))
				{
					productVariantObject.IdObject = productVariantManager.Get(productVariantObject.Id, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of ProductVariant.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of ProductVariant</returns>
        public ProductVariantList GetAll()
        {
            using (ProductVariantDataAccess data = new ProductVariantDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of ProductVariant.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of ProductVariant</returns>
        public ProductVariantList GetAll(bool fillChild)
        {
			ProductVariantList productVariantList = new ProductVariantList();
            using (ProductVariantDataAccess data = new ProductVariantDataAccess(ClientContext))
            {
                productVariantList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (ProductVariant productVariantObject in productVariantList)
                {
					FillProductVariantWithChilds(productVariantObject, fillChild);
				}
			}
			return productVariantList;
        }
		
		/// <summary>
        /// Retrieve list of ProductVariant  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of ProductVariant</returns>
        public ProductVariantList GetPaged(PagedRequest request)
        {
            using (ProductVariantDataAccess data = new ProductVariantDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of ProductVariant  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of ProductVariant</returns>
        public ProductVariantList GetByQuery(String query)
        {
            using (ProductVariantDataAccess data = new ProductVariantDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get ProductVariant Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ProductVariant
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (ProductVariantDataAccess data = new ProductVariantDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get ProductVariant Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ProductVariant
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (ProductVariantDataAccess data = new ProductVariantDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of ProductVariant By Id        
		/// <param name="_Id"></param>
        /// </summary>
        /// <returns>List of ProductVariant</returns>
        public ProductVariantList GetById(Int32 _Id)
        {
            using (ProductVariantDataAccess data = new ProductVariantDataAccess(ClientContext))
            {
                return data.GetById(_Id);
            }
        }
		
		/// <summary>
        /// Retrieve list of ProductVariant By ProductId        
		/// <param name="_ProductId"></param>
        /// </summary>
        /// <returns>List of ProductVariant</returns>
        public ProductVariantList GetByProductId(Int32 _ProductId)
        {
            using (ProductVariantDataAccess data = new ProductVariantDataAccess(ClientContext))
            {
                return data.GetByProductId(_ProductId);
            }
        }
		
		
		#endregion
	}	
}