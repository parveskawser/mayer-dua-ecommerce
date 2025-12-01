using MDUA.Entities;
using MDUA.Entities.List;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MDUA.DataAccess
{
    public partial class ProductImageDataAccess
    {
        private ProductImageList GetProductImagesByProductId(int productId)
        {
            // Use the Stored Procedure you provided: GetProductImageByProductId
            using (SqlCommand cmd = GetSPCommand("GetProductImageByProductId"))
            {
                AddParameter(cmd, pInt32("ProductId", productId));
                // 0 means "All Rows" in your framework's GetList method
                return GetList(cmd, 10000);
            }
        }
    }
}
