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
    

    public string GetPrimaryImage(int productId)
        {
            // Tries to find the image marked 'IsPrimary'. 
            // If none, falls back to the one with the lowest SortOrder.
            string SQLQuery = @"
        SELECT TOP 1 ImageUrl 
        FROM ProductImage 
        WHERE ProductId = @ProductId 
        ORDER BY IsPrimary DESC, SortOrder ASC";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("ProductId", productId));

                // Open connection safely
                if (cmd.Connection.State != System.Data.ConnectionState.Open)
                    cmd.Connection.Open();

                object result = cmd.ExecuteScalar();
                cmd.Connection.Close();

                if (result != null && result != DBNull.Value)
                {
                    return result.ToString();
                }
            }
            return null; // Return null if no image found
        }

    } }
