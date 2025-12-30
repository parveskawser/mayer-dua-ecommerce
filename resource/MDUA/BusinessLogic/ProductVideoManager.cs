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
	public partial class ProductVideoManager
	{
	
		/// <summary>
        /// Update ProductVideo Object.
        /// Data manipulation processing for: new, deleted, updated ProductVideo
        /// </summary>
        /// <param name="productVideoObject"></param>
        /// <returns></returns>
        public bool Update(ProductVideo productVideoObject)
        {
			bool success = false;
			
			success = UpdateBase(productVideoObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of ProductVideo Object.
        /// </summary>
        /// <param name="productVideoObject"></param>
        /// <returns></returns>
        public void FillChilds(ProductVideo productVideoObject)
        {
			///Fill external information of Childs of ProductVideoObject
        }
	}	
}