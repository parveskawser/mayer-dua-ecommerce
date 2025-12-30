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
    /// Business logic processing for ProductDiscount.
    /// </summary>    
	public partial class ProductDiscountManager : BaseManager
	{
	
		#region Constructors
		public ProductDiscountManager(ClientContext context) : base(context) { }
		public ProductDiscountManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new productDiscount.
        /// data manipulation for insertion of ProductDiscount
        /// </summary>
        /// <param name="productDiscountObject"></param>
        /// <returns></returns>
        private bool Insert(ProductDiscount productDiscountObject)
        {
            // new productDiscount
            using (ProductDiscountDataAccess data = new ProductDiscountDataAccess(ClientContext))
            {
                // insert to productDiscountObject
                Int32 _Id = data.Insert(productDiscountObject);
                // if successful, process
                if (_Id > 0)
                {
                    productDiscountObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of ProductDiscount Object.
        /// Data manipulation processing for: new, deleted, updated ProductDiscount
        /// </summary>
        /// <param name="productDiscountObject"></param>
        /// <returns></returns>
        public bool UpdateBase(ProductDiscount productDiscountObject)
        {
            // use of switch for different types of DML
            switch (productDiscountObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(productDiscountObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(productDiscountObject.Id);
            }
            // update rows
            using (ProductDiscountDataAccess data = new ProductDiscountDataAccess(ClientContext))
            {
                return (data.Update(productDiscountObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for ProductDiscount
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (ProductDiscountDataAccess data = new ProductDiscountDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve ProductDiscount data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>ProductDiscount Object</returns>
        public ProductDiscount Get(Int32 _Id)
        {
            using (ProductDiscountDataAccess data = new ProductDiscountDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a ProductDiscount .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public ProductDiscount Get(Int32 _Id, bool fillChild)
        {
            ProductDiscount productDiscountObject;
            productDiscountObject = Get(_Id);
            
            if (productDiscountObject != null && fillChild)
            {
                // populate child data for a productDiscountObject
                FillProductDiscountWithChilds(productDiscountObject, fillChild);
            }

            return productDiscountObject;
        }
		
		/// <summary>
        /// populates a ProductDiscount with its child entities
        /// </summary>
        /// <param name="productDiscount"></param>
		/// <param name="fillChilds"></param>
        private void FillProductDiscountWithChilds(ProductDiscount productDiscountObject, bool fillChilds)
        {
            // populate child data for a productDiscountObject
            if (productDiscountObject != null)
            {
				// Retrieve ProductIdObject as Product type for the ProductDiscount using ProductId
				using(ProductManager productManager = new ProductManager(ClientContext))
				{
					productDiscountObject.ProductIdObject = productManager.Get(productDiscountObject.ProductId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of ProductDiscount.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of ProductDiscount</returns>
        public ProductDiscountList GetAll()
        {
            using (ProductDiscountDataAccess data = new ProductDiscountDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of ProductDiscount.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of ProductDiscount</returns>
        public ProductDiscountList GetAll(bool fillChild)
        {
			ProductDiscountList productDiscountList = new ProductDiscountList();
            using (ProductDiscountDataAccess data = new ProductDiscountDataAccess(ClientContext))
            {
                productDiscountList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (ProductDiscount productDiscountObject in productDiscountList)
                {
					FillProductDiscountWithChilds(productDiscountObject, fillChild);
				}
			}
			return productDiscountList;
        }
		
		/// <summary>
        /// Retrieve list of ProductDiscount  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of ProductDiscount</returns>
        public ProductDiscountList GetPaged(PagedRequest request)
        {
            using (ProductDiscountDataAccess data = new ProductDiscountDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of ProductDiscount  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of ProductDiscount</returns>
        public ProductDiscountList GetByQuery(String query)
        {
            using (ProductDiscountDataAccess data = new ProductDiscountDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get ProductDiscount Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ProductDiscount
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (ProductDiscountDataAccess data = new ProductDiscountDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get ProductDiscount Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ProductDiscount
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (ProductDiscountDataAccess data = new ProductDiscountDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of ProductDiscount By ProductId        
		/// <param name="_ProductId"></param>
        /// </summary>
        /// <returns>List of ProductDiscount</returns>
        public ProductDiscountList GetByProductId(Int32 _ProductId)
        {
            using (ProductDiscountDataAccess data = new ProductDiscountDataAccess(ClientContext))
            {
                return data.GetByProductId(_ProductId);
            }
        }
		
		
		#endregion
	}	
}