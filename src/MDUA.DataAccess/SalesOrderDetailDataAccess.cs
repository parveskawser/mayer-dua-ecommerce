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
        public SalesOrderDetail GetFirstDetailByOrderRef(string orderRef)
        {
            // 1. Explicitly select the DB Column Name 'ProductId'
            string sql = @"
        SELECT TOP 1 
            d.Id, 
            d.SalesOrderId,
            d.ProductId,  -- <--- DB Column Name
            d.Quantity
        FROM [dbo].[SalesOrderDetail] d
        INNER JOIN [dbo].[SalesOrderHeader] h ON d.SalesOrderId = h.Id
        WHERE h.SalesOrderId = @OrderRef 
           OR h.OnlineOrderId = @OrderRef 
           OR h.DirectOrderId = @OrderRef";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                AddParameter(cmd, pNVarChar("OrderRef", 50, orderRef));

                SqlDataReader reader;
                SelectRecords(cmd, out reader);

                using (reader)
                {
                    if (reader.Read())
                    {
                        var detail = new SalesOrderDetail();

                        // 2. Standard Maps
                        if (!reader.IsDBNull(reader.GetOrdinal("Id")))
                            detail.Id = reader.GetInt32(reader.GetOrdinal("Id"));

                        // ✅ 3. CRITICAL MAP: Read 'ProductId' -> Store in 'ProductVariantId'
                        // This ensures the value is passed to the property expected by Inventory Logic
                        int prodIdx = reader.GetOrdinal("ProductId");
                        if (!reader.IsDBNull(prodIdx))
                        {
                            detail.ProductVariantId = reader.GetInt32(prodIdx);
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("Quantity")))
                            detail.Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"));

                        return detail;
                    }
                }
                return null;
            }
        }        // Helper (Keep this in your class)
 
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
