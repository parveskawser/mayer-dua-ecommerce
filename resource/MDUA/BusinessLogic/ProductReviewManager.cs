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
	public partial class ProductReviewManager
	{
	
		/// <summary>
        /// Update ProductReview Object.
        /// Data manipulation processing for: new, deleted, updated ProductReview
        /// </summary>
        /// <param name="productReviewObject"></param>
        /// <returns></returns>
        public bool Update(ProductReview productReviewObject)
        {
			bool success = false;
			
			success = UpdateBase(productReviewObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of ProductReview Object.
        /// </summary>
        /// <param name="productReviewObject"></param>
        /// <returns></returns>
        public void FillChilds(ProductReview productReviewObject)
        {
			///Fill external information of Childs of ProductReviewObject
        }
	}	
}