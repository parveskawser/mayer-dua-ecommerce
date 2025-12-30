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
    /// Business logic processing for VariantImage.
    /// </summary>    
	public partial class VariantImageManager : BaseManager
	{
	
		#region Constructors
		public VariantImageManager(ClientContext context) : base(context) { }
		public VariantImageManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new variantImage.
        /// data manipulation for insertion of VariantImage
        /// </summary>
        /// <param name="variantImageObject"></param>
        /// <returns></returns>
        private bool Insert(VariantImage variantImageObject)
        {
            // new variantImage
            using (VariantImageDataAccess data = new VariantImageDataAccess(ClientContext))
            {
                // insert to variantImageObject
                Int32 _Id = data.Insert(variantImageObject);
                // if successful, process
                if (_Id > 0)
                {
                    variantImageObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of VariantImage Object.
        /// Data manipulation processing for: new, deleted, updated VariantImage
        /// </summary>
        /// <param name="variantImageObject"></param>
        /// <returns></returns>
        public bool UpdateBase(VariantImage variantImageObject)
        {
            // use of switch for different types of DML
            switch (variantImageObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(variantImageObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(variantImageObject.Id);
            }
            // update rows
            using (VariantImageDataAccess data = new VariantImageDataAccess(ClientContext))
            {
                return (data.Update(variantImageObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for VariantImage
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (VariantImageDataAccess data = new VariantImageDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve VariantImage data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>VariantImage Object</returns>
        public VariantImage Get(Int32 _Id)
        {
            using (VariantImageDataAccess data = new VariantImageDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a VariantImage .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public VariantImage Get(Int32 _Id, bool fillChild)
        {
            VariantImage variantImageObject;
            variantImageObject = Get(_Id);
            
            if (variantImageObject != null && fillChild)
            {
                // populate child data for a variantImageObject
                FillVariantImageWithChilds(variantImageObject, fillChild);
            }

            return variantImageObject;
        }
		
		/// <summary>
        /// populates a VariantImage with its child entities
        /// </summary>
        /// <param name="variantImage"></param>
		/// <param name="fillChilds"></param>
        private void FillVariantImageWithChilds(VariantImage variantImageObject, bool fillChilds)
        {
            // populate child data for a variantImageObject
            if (variantImageObject != null)
            {
				// Retrieve VariantIdObject as ProductVariant type for the VariantImage using VariantId
				using(ProductVariantManager productVariantManager = new ProductVariantManager(ClientContext))
				{
					variantImageObject.VariantIdObject = productVariantManager.Get(variantImageObject.VariantId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of VariantImage.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of VariantImage</returns>
        public VariantImageList GetAll()
        {
            using (VariantImageDataAccess data = new VariantImageDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of VariantImage.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of VariantImage</returns>
        public VariantImageList GetAll(bool fillChild)
        {
			VariantImageList variantImageList = new VariantImageList();
            using (VariantImageDataAccess data = new VariantImageDataAccess(ClientContext))
            {
                variantImageList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (VariantImage variantImageObject in variantImageList)
                {
					FillVariantImageWithChilds(variantImageObject, fillChild);
				}
			}
			return variantImageList;
        }
		
		/// <summary>
        /// Retrieve list of VariantImage  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of VariantImage</returns>
        public VariantImageList GetPaged(PagedRequest request)
        {
            using (VariantImageDataAccess data = new VariantImageDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of VariantImage  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of VariantImage</returns>
        public VariantImageList GetByQuery(String query)
        {
            using (VariantImageDataAccess data = new VariantImageDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get VariantImage Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of VariantImage
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (VariantImageDataAccess data = new VariantImageDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get VariantImage Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of VariantImage
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (VariantImageDataAccess data = new VariantImageDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of VariantImage By VariantId        
		/// <param name="_VariantId"></param>
        /// </summary>
        /// <returns>List of VariantImage</returns>
        public VariantImageList GetByVariantId(Int32 _VariantId)
        {
            using (VariantImageDataAccess data = new VariantImageDataAccess(ClientContext))
            {
                return data.GetByVariantId(_VariantId);
            }
        }
		
		
		#endregion
	}	
}