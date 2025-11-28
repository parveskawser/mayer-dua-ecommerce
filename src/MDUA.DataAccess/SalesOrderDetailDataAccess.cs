using System;
using System.Data;
using System.Data.SqlClient;

using MDUA.Framework;
using MDUA.Framework.Exceptions;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess
{
	public partial class SalesOrderDetailDataAccess
	{
        public long InsertSalesOrderDetailSafe(SalesOrderDetail detail)
        {
            // ✅ FIX: Using [ProductId] based on your database schema confirmation
            string SQLQuery = @"
                INSERT INTO [dbo].[SalesOrderDetail]
                ([SalesOrderId], [ProductId], [Quantity], [UnitPrice], 
                 [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt], [ProfitAmount])
                VALUES
                (@SalesOrderId, @ProductId, @Quantity, @UnitPrice, 
                 @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @ProfitAmount);
                
                SELECT CONVERT(INT, SCOPE_IDENTITY());";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("SalesOrderId", detail.SalesOrderId));

                // ✅ Map Entity property (ProductVariantId) to Database Parameter (@ProductId)
                AddParameter(cmd, pInt32("ProductId", detail.ProductVariantId));

                AddParameter(cmd, pInt32("Quantity", detail.Quantity));
                AddParameter(cmd, pDecimal("UnitPrice", detail.UnitPrice));

                AddParameter(cmd, pNVarChar("CreatedBy", 100, detail.CreatedBy));
                AddParameter(cmd, pDateTime("CreatedAt", detail.CreatedAt));
                AddParameter(cmd, pNVarChar("UpdatedBy", 100, null));
                AddParameter(cmd, pDateTime("UpdatedAt", null));

                AddParameter(cmd, pDecimal("ProfitAmount", detail.ProfitAmount ?? 0));

                SqlDataReader reader;
                SelectRecords(cmd, out reader);

                int newId = 0;
                using (reader)
                {
                    if (reader.Read() && !reader.IsDBNull(0))
                    {
                        newId = reader.GetInt32(0);
                    }
                    reader.Close();
                }

                return newId;
            }
        }
    }	
}
