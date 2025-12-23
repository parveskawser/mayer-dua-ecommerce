using System;
using System.Data.SqlClient;
using MDUA.Entities;
using MDUA.Entities.List;

namespace MDUA.DataAccess
{
    public partial class ProductReviewDataAccess
    {
        public ProductReviewList GetProductReviewsByProductId(int productId)
        {
            string SQLQuery = @"
                SELECT 
                    Id,
                    ProductId,
                    CustomerName,
                    Rating,
                    ReviewText,
                    IsApproved,
                    CreatedBy,
                    CreatedAt,
                    UpdatedBy,
                    UpdatedAt
                FROM ProductReview
                WHERE ProductId = @ProductId
                  AND IsApproved = 1
                ORDER BY CreatedAt DESC";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            AddParameter(cmd, pInt32("ProductId", productId));

            return GetList(cmd, ALL_AVAILABLE_RECORDS);
        }
    }
}
