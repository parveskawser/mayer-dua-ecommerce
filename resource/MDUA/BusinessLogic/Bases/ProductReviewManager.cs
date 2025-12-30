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
    /// Business logic processing for ProductReview.
    /// </summary>    
	public partial class ProductReviewManager : BaseManager
	{
	
		#region Constructors
		public ProductReviewManager(ClientContext context) : base(context) { }
		public ProductReviewManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new productReview.
        /// data manipulation for insertion of ProductReview
        /// </summary>
        /// <param name="productReviewObject"></param>
        /// <returns></returns>
        private bool Insert(ProductReview productReviewObject)
        {
            // new productReview
            using (ProductReviewDataAccess data = new ProductReviewDataAccess(ClientContext))
            {
                // insert to productReviewObject
                Int32 _Id = data.Insert(productReviewObject);
                // if successful, process
                if (_Id > 0)
                {
                    productReviewObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of ProductReview Object.
        /// Data manipulation processing for: new, deleted, updated ProductReview
        /// </summary>
        /// <param name="productReviewObject"></param>
        /// <returns></returns>
        public bool UpdateBase(ProductReview productReviewObject)
        {
            // use of switch for different types of DML
            switch (productReviewObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(productReviewObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(productReviewObject.Id);
            }
            // update rows
            using (ProductReviewDataAccess data = new ProductReviewDataAccess(ClientContext))
            {
                return (data.Update(productReviewObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for ProductReview
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (ProductReviewDataAccess data = new ProductReviewDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve ProductReview data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>ProductReview Object</returns>
        public ProductReview Get(Int32 _Id)
        {
            using (ProductReviewDataAccess data = new ProductReviewDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a ProductReview .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public ProductReview Get(Int32 _Id, bool fillChild)
        {
            ProductReview productReviewObject;
            productReviewObject = Get(_Id);
            
            if (productReviewObject != null && fillChild)
            {
                // populate child data for a productReviewObject
                FillProductReviewWithChilds(productReviewObject, fillChild);
            }

            return productReviewObject;
        }
		
		/// <summary>
        /// populates a ProductReview with its child entities
        /// </summary>
        /// <param name="productReview"></param>
		/// <param name="fillChilds"></param>
        private void FillProductReviewWithChilds(ProductReview productReviewObject, bool fillChilds)
        {
            // populate child data for a productReviewObject
            if (productReviewObject != null)
            {
				// Retrieve ProductIdObject as Product type for the ProductReview using ProductId
				using(ProductManager productManager = new ProductManager(ClientContext))
				{
					productReviewObject.ProductIdObject = productManager.Get(productReviewObject.ProductId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of ProductReview.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of ProductReview</returns>
        public ProductReviewList GetAll()
        {
            using (ProductReviewDataAccess data = new ProductReviewDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of ProductReview.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of ProductReview</returns>
        public ProductReviewList GetAll(bool fillChild)
        {
			ProductReviewList productReviewList = new ProductReviewList();
            using (ProductReviewDataAccess data = new ProductReviewDataAccess(ClientContext))
            {
                productReviewList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (ProductReview productReviewObject in productReviewList)
                {
					FillProductReviewWithChilds(productReviewObject, fillChild);
				}
			}
			return productReviewList;
        }
		
		/// <summary>
        /// Retrieve list of ProductReview  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of ProductReview</returns>
        public ProductReviewList GetPaged(PagedRequest request)
        {
            using (ProductReviewDataAccess data = new ProductReviewDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of ProductReview  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of ProductReview</returns>
        public ProductReviewList GetByQuery(String query)
        {
            using (ProductReviewDataAccess data = new ProductReviewDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get ProductReview Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ProductReview
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (ProductReviewDataAccess data = new ProductReviewDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get ProductReview Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ProductReview
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (ProductReviewDataAccess data = new ProductReviewDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of ProductReview By ProductId        
		/// <param name="_ProductId"></param>
        /// </summary>
        /// <returns>List of ProductReview</returns>
        public ProductReviewList GetByProductId(Int32 _ProductId)
        {
            using (ProductReviewDataAccess data = new ProductReviewDataAccess(ClientContext))
            {
                return data.GetByProductId(_ProductId);
            }
        }
		
		
		#endregion
	}	
}