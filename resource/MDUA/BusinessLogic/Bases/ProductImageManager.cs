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
    /// Business logic processing for ProductImage.
    /// </summary>    
	public partial class ProductImageManager : BaseManager
	{
	
		#region Constructors
		public ProductImageManager(ClientContext context) : base(context) { }
		public ProductImageManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new productImage.
        /// data manipulation for insertion of ProductImage
        /// </summary>
        /// <param name="productImageObject"></param>
        /// <returns></returns>
        private bool Insert(ProductImage productImageObject)
        {
            // new productImage
            using (ProductImageDataAccess data = new ProductImageDataAccess(ClientContext))
            {
                // insert to productImageObject
                Int32 _Id = data.Insert(productImageObject);
                // if successful, process
                if (_Id > 0)
                {
                    productImageObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of ProductImage Object.
        /// Data manipulation processing for: new, deleted, updated ProductImage
        /// </summary>
        /// <param name="productImageObject"></param>
        /// <returns></returns>
        public bool UpdateBase(ProductImage productImageObject)
        {
            // use of switch for different types of DML
            switch (productImageObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(productImageObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(productImageObject.Id);
            }
            // update rows
            using (ProductImageDataAccess data = new ProductImageDataAccess(ClientContext))
            {
                return (data.Update(productImageObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for ProductImage
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (ProductImageDataAccess data = new ProductImageDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve ProductImage data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>ProductImage Object</returns>
        public ProductImage Get(Int32 _Id)
        {
            using (ProductImageDataAccess data = new ProductImageDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a ProductImage .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public ProductImage Get(Int32 _Id, bool fillChild)
        {
            ProductImage productImageObject;
            productImageObject = Get(_Id);
            
            if (productImageObject != null && fillChild)
            {
                // populate child data for a productImageObject
                FillProductImageWithChilds(productImageObject, fillChild);
            }

            return productImageObject;
        }
		
		/// <summary>
        /// populates a ProductImage with its child entities
        /// </summary>
        /// <param name="productImage"></param>
		/// <param name="fillChilds"></param>
        private void FillProductImageWithChilds(ProductImage productImageObject, bool fillChilds)
        {
            // populate child data for a productImageObject
            if (productImageObject != null)
            {
				// Retrieve ProductIdObject as Product type for the ProductImage using ProductId
				using(ProductManager productManager = new ProductManager(ClientContext))
				{
					productImageObject.ProductIdObject = productManager.Get(productImageObject.ProductId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of ProductImage.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of ProductImage</returns>
        public ProductImageList GetAll()
        {
            using (ProductImageDataAccess data = new ProductImageDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of ProductImage.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of ProductImage</returns>
        public ProductImageList GetAll(bool fillChild)
        {
			ProductImageList productImageList = new ProductImageList();
            using (ProductImageDataAccess data = new ProductImageDataAccess(ClientContext))
            {
                productImageList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (ProductImage productImageObject in productImageList)
                {
					FillProductImageWithChilds(productImageObject, fillChild);
				}
			}
			return productImageList;
        }
		
		/// <summary>
        /// Retrieve list of ProductImage  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of ProductImage</returns>
        public ProductImageList GetPaged(PagedRequest request)
        {
            using (ProductImageDataAccess data = new ProductImageDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of ProductImage  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of ProductImage</returns>
        public ProductImageList GetByQuery(String query)
        {
            using (ProductImageDataAccess data = new ProductImageDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get ProductImage Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ProductImage
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (ProductImageDataAccess data = new ProductImageDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get ProductImage Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ProductImage
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (ProductImageDataAccess data = new ProductImageDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of ProductImage By ProductId        
		/// <param name="_ProductId"></param>
        /// </summary>
        /// <returns>List of ProductImage</returns>
        public ProductImageList GetByProductId(Int32 _ProductId)
        {
            using (ProductImageDataAccess data = new ProductImageDataAccess(ClientContext))
            {
                return data.GetByProductId(_ProductId);
            }
        }
		
		
		#endregion
	}	
}