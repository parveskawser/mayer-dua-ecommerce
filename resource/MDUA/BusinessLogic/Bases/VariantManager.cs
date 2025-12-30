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
    /// Business logic processing for Variant.
    /// </summary>    
	public partial class VariantManager : BaseManager
	{
	
		#region Constructors
		public VariantManager(ClientContext context) : base(context) { }
		public VariantManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new variant.
        /// data manipulation for insertion of Variant
        /// </summary>
        /// <param name="variantObject"></param>
        /// <returns></returns>
        private bool Insert(Variant variantObject)
        {
            // new variant
            using (VariantDataAccess data = new VariantDataAccess(ClientContext))
            {
                // insert to variantObject
                Int32 _Id = data.Insert(variantObject);
                // if successful, process
                if (_Id > 0)
                {
                    variantObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of Variant Object.
        /// Data manipulation processing for: new, deleted, updated Variant
        /// </summary>
        /// <param name="variantObject"></param>
        /// <returns></returns>
        public bool UpdateBase(Variant variantObject)
        {
            // use of switch for different types of DML
            switch (variantObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(variantObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(variantObject.Id);
            }
            // update rows
            using (VariantDataAccess data = new VariantDataAccess(ClientContext))
            {
                return (data.Update(variantObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for Variant
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (VariantDataAccess data = new VariantDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve Variant data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>Variant Object</returns>
        public Variant Get(Int32 _Id)
        {
            using (VariantDataAccess data = new VariantDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a Variant .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public Variant Get(Int32 _Id, bool fillChild)
        {
            Variant variantObject;
            variantObject = Get(_Id);
            
            if (variantObject != null && fillChild)
            {
                // populate child data for a variantObject
                FillVariantWithChilds(variantObject, fillChild);
            }

            return variantObject;
        }
		
		/// <summary>
        /// populates a Variant with its child entities
        /// </summary>
        /// <param name="variant"></param>
		/// <param name="fillChilds"></param>
        private void FillVariantWithChilds(Variant variantObject, bool fillChilds)
        {
            // populate child data for a variantObject
            if (variantObject != null)
            {
				// Retrieve ProductIdObject as Product type for the Variant using ProductId
				using(ProductManager productManager = new ProductManager(ClientContext))
				{
					variantObject.ProductIdObject = productManager.Get(variantObject.ProductId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of Variant.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of Variant</returns>
        public VariantList GetAll()
        {
            using (VariantDataAccess data = new VariantDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of Variant.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of Variant</returns>
        public VariantList GetAll(bool fillChild)
        {
			VariantList variantList = new VariantList();
            using (VariantDataAccess data = new VariantDataAccess(ClientContext))
            {
                variantList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (Variant variantObject in variantList)
                {
					FillVariantWithChilds(variantObject, fillChild);
				}
			}
			return variantList;
        }
		
		/// <summary>
        /// Retrieve list of Variant  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of Variant</returns>
        public VariantList GetPaged(PagedRequest request)
        {
            using (VariantDataAccess data = new VariantDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of Variant  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of Variant</returns>
        public VariantList GetByQuery(String query)
        {
            using (VariantDataAccess data = new VariantDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get Variant Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Variant
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (VariantDataAccess data = new VariantDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get Variant Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Variant
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (VariantDataAccess data = new VariantDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of Variant By ProductId        
		/// <param name="_ProductId"></param>
        /// </summary>
        /// <returns>List of Variant</returns>
        public VariantList GetByProductId(Int32 _ProductId)
        {
            using (VariantDataAccess data = new VariantDataAccess(ClientContext))
            {
                return data.GetByProductId(_ProductId);
            }
        }
		
		
		#endregion
	}	
}