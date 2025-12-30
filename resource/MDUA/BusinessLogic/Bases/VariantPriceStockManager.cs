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
    /// Business logic processing for VariantPriceStock.
    /// </summary>    
	public partial class VariantPriceStockManager : BaseManager
	{
	
		#region Constructors
		public VariantPriceStockManager(ClientContext context) : base(context) { }
		public VariantPriceStockManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new variantPriceStock.
        /// data manipulation for insertion of VariantPriceStock
        /// </summary>
        /// <param name="variantPriceStockObject"></param>
        /// <returns></returns>
        private bool Insert(VariantPriceStock variantPriceStockObject)
        {
            // new variantPriceStock
            using (VariantPriceStockDataAccess data = new VariantPriceStockDataAccess(ClientContext))
            {
                // insert to variantPriceStockObject
                Int32 _Id = data.Insert(variantPriceStockObject);
                // if successful, process
                if (_Id > 0)
                {
                    variantPriceStockObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of VariantPriceStock Object.
        /// Data manipulation processing for: new, deleted, updated VariantPriceStock
        /// </summary>
        /// <param name="variantPriceStockObject"></param>
        /// <returns></returns>
        public bool UpdateBase(VariantPriceStock variantPriceStockObject)
        {
            // use of switch for different types of DML
            switch (variantPriceStockObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(variantPriceStockObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(variantPriceStockObject.Id);
            }
            // update rows
            using (VariantPriceStockDataAccess data = new VariantPriceStockDataAccess(ClientContext))
            {
                return (data.Update(variantPriceStockObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for VariantPriceStock
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (VariantPriceStockDataAccess data = new VariantPriceStockDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve VariantPriceStock data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>VariantPriceStock Object</returns>
        public VariantPriceStock Get(Int32 _Id)
        {
            using (VariantPriceStockDataAccess data = new VariantPriceStockDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a VariantPriceStock .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public VariantPriceStock Get(Int32 _Id, bool fillChild)
        {
            VariantPriceStock variantPriceStockObject;
            variantPriceStockObject = Get(_Id);
            
            if (variantPriceStockObject != null && fillChild)
            {
                // populate child data for a variantPriceStockObject
                FillVariantPriceStockWithChilds(variantPriceStockObject, fillChild);
            }

            return variantPriceStockObject;
        }
		
		/// <summary>
        /// populates a VariantPriceStock with its child entities
        /// </summary>
        /// <param name="variantPriceStock"></param>
		/// <param name="fillChilds"></param>
        private void FillVariantPriceStockWithChilds(VariantPriceStock variantPriceStockObject, bool fillChilds)
        {
            // populate child data for a variantPriceStockObject
            if (variantPriceStockObject != null)
            {
				// Retrieve IdObject as ProductVariant type for the VariantPriceStock using Id
				using(ProductVariantManager productVariantManager = new ProductVariantManager(ClientContext))
				{
					variantPriceStockObject.IdObject = productVariantManager.Get(variantPriceStockObject.Id, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of VariantPriceStock.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of VariantPriceStock</returns>
        public VariantPriceStockList GetAll()
        {
            using (VariantPriceStockDataAccess data = new VariantPriceStockDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of VariantPriceStock.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of VariantPriceStock</returns>
        public VariantPriceStockList GetAll(bool fillChild)
        {
			VariantPriceStockList variantPriceStockList = new VariantPriceStockList();
            using (VariantPriceStockDataAccess data = new VariantPriceStockDataAccess(ClientContext))
            {
                variantPriceStockList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (VariantPriceStock variantPriceStockObject in variantPriceStockList)
                {
					FillVariantPriceStockWithChilds(variantPriceStockObject, fillChild);
				}
			}
			return variantPriceStockList;
        }
		
		/// <summary>
        /// Retrieve list of VariantPriceStock  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of VariantPriceStock</returns>
        public VariantPriceStockList GetPaged(PagedRequest request)
        {
            using (VariantPriceStockDataAccess data = new VariantPriceStockDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of VariantPriceStock  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of VariantPriceStock</returns>
        public VariantPriceStockList GetByQuery(String query)
        {
            using (VariantPriceStockDataAccess data = new VariantPriceStockDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get VariantPriceStock Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of VariantPriceStock
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (VariantPriceStockDataAccess data = new VariantPriceStockDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get VariantPriceStock Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of VariantPriceStock
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (VariantPriceStockDataAccess data = new VariantPriceStockDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of VariantPriceStock By Id        
		/// <param name="_Id"></param>
        /// </summary>
        /// <returns>List of VariantPriceStock</returns>
        public VariantPriceStockList GetById(Int32 _Id)
        {
            using (VariantPriceStockDataAccess data = new VariantPriceStockDataAccess(ClientContext))
            {
                return data.GetById(_Id);
            }
        }
		
		
		#endregion
	}	
}