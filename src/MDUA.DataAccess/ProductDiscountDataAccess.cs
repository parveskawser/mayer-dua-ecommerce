using System;
using System.Data.SqlClient;
using MDUA.Entities;
using MDUA.Entities.Bases;

using MDUA.Entities.List;

namespace MDUA.DataAccess
{
    public partial class ProductDiscountDataAccess
    {
        public ProductDiscountList GetProductDiscountsByProductId(int productId)
        {
            string SQLQuery = @"
                SELECT
                    Id,
                    ProductId,
                    DiscountType,
                    DiscountValue,
                    MinQuantity,
                    EffectiveFrom,
                    EffectiveTo,
                    IsActive,
                    CreatedBy,
                    CreatedAt,
                    UpdatedBy,
                    UpdatedAt
                FROM ProductDiscount
                WHERE ProductId = @ProductId
                ORDER BY EffectiveFrom DESC";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            AddParameter(cmd, pInt32("ProductId", productId));
            return GetList(cmd, ALL_AVAILABLE_RECORDS);
        }

        public ProductDiscount GetActiveDiscount(int productId)
        {
            string SQLQuery = @"
                SELECT TOP 1
                    Id,
                    ProductId,
                    DiscountType,
                    DiscountValue,
                    MinQuantity,
                    EffectiveFrom,
                    EffectiveTo,
                    IsActive,
                    CreatedBy,
                    CreatedAt,
                    UpdatedBy,
                    UpdatedAt
                FROM ProductDiscount
                WHERE ProductId = @ProductId
                  AND IsActive = 1
                  AND EffectiveFrom <= SYSUTCDATETIME()
                  AND (EffectiveTo IS NULL OR EffectiveTo >= SYSUTCDATETIME())
                ORDER BY EffectiveFrom DESC";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            AddParameter(cmd, pInt32("ProductId", productId));
            return GetObject(cmd);
        }
    }
}
