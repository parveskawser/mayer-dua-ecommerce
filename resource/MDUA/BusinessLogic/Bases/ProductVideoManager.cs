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
    /// Business logic processing for ProductVideo.
    /// </summary>    
	public partial class ProductVideoManager : BaseManager
	{
	
		#region Constructors
		public ProductVideoManager(ClientContext context) : base(context) { }
		public ProductVideoManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new productVideo.
        /// data manipulation for insertion of ProductVideo
        /// </summary>
        /// <param name="productVideoObject"></param>
        /// <returns></returns>
        private bool Insert(ProductVideo productVideoObject)
        {
            // new productVideo
            using (ProductVideoDataAccess data = new ProductVideoDataAccess(ClientContext))
            {
                // insert to productVideoObject
                Int32 _Id = data.Insert(productVideoObject);
                // if successful, process
                if (_Id > 0)
                {
                    productVideoObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of ProductVideo Object.
        /// Data manipulation processing for: new, deleted, updated ProductVideo
        /// </summary>
        /// <param name="productVideoObject"></param>
        /// <returns></returns>
        public bool UpdateBase(ProductVideo productVideoObject)
        {
            // use of switch for different types of DML
            switch (productVideoObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(productVideoObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(productVideoObject.Id);
            }
            // update rows
            using (ProductVideoDataAccess data = new ProductVideoDataAccess(ClientContext))
            {
                return (data.Update(productVideoObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for ProductVideo
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (ProductVideoDataAccess data = new ProductVideoDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve ProductVideo data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>ProductVideo Object</returns>
        public ProductVideo Get(Int32 _Id)
        {
            using (ProductVideoDataAccess data = new ProductVideoDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a ProductVideo .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public ProductVideo Get(Int32 _Id, bool fillChild)
        {
            ProductVideo productVideoObject;
            productVideoObject = Get(_Id);
            
            if (productVideoObject != null && fillChild)
            {
                // populate child data for a productVideoObject
                FillProductVideoWithChilds(productVideoObject, fillChild);
            }

            return productVideoObject;
        }
		
		/// <summary>
        /// populates a ProductVideo with its child entities
        /// </summary>
        /// <param name="productVideo"></param>
		/// <param name="fillChilds"></param>
        private void FillProductVideoWithChilds(ProductVideo productVideoObject, bool fillChilds)
        {
            // populate child data for a productVideoObject
            if (productVideoObject != null)
            {
				// Retrieve ProductIdObject as Product type for the ProductVideo using ProductId
				using(ProductManager productManager = new ProductManager(ClientContext))
				{
					productVideoObject.ProductIdObject = productManager.Get(productVideoObject.ProductId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of ProductVideo.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of ProductVideo</returns>
        public ProductVideoList GetAll()
        {
            using (ProductVideoDataAccess data = new ProductVideoDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of ProductVideo.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of ProductVideo</returns>
        public ProductVideoList GetAll(bool fillChild)
        {
			ProductVideoList productVideoList = new ProductVideoList();
            using (ProductVideoDataAccess data = new ProductVideoDataAccess(ClientContext))
            {
                productVideoList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (ProductVideo productVideoObject in productVideoList)
                {
					FillProductVideoWithChilds(productVideoObject, fillChild);
				}
			}
			return productVideoList;
        }
		
		/// <summary>
        /// Retrieve list of ProductVideo  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of ProductVideo</returns>
        public ProductVideoList GetPaged(PagedRequest request)
        {
            using (ProductVideoDataAccess data = new ProductVideoDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of ProductVideo  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of ProductVideo</returns>
        public ProductVideoList GetByQuery(String query)
        {
            using (ProductVideoDataAccess data = new ProductVideoDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get ProductVideo Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ProductVideo
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (ProductVideoDataAccess data = new ProductVideoDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get ProductVideo Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ProductVideo
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (ProductVideoDataAccess data = new ProductVideoDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of ProductVideo By ProductId        
		/// <param name="_ProductId"></param>
        /// </summary>
        /// <returns>List of ProductVideo</returns>
        public ProductVideoList GetByProductId(Int32 _ProductId)
        {
            using (ProductVideoDataAccess data = new ProductVideoDataAccess(ClientContext))
            {
                return data.GetByProductId(_ProductId);
            }
        }
		
		
		#endregion
	}	
}