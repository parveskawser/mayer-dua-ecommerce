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

        // Inside SalesOrderDetailDataAccess.cs

        // Inside SalesOrderDetailDataAccess.cs

        public List<SalesOrderDetail> GetOrderDetailsSafe(int salesOrderId)
        {
            var list = new List<SalesOrderDetail>();

            // ✅ FIX: Removed '[ProductVariantId]' from the query.
            // We only select the columns strictly required for the Delivery logic.
            string sql = @"
        SELECT 
            [Id], 
            [SalesOrderId], 
            [Quantity]
        FROM [SalesOrderDetail] 
        WHERE [SalesOrderId] = @SalesOrderId";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@SalesOrderId", System.Data.SqlDbType.Int) { Value = salesOrderId });

                if (cmd.Connection.State != System.Data.ConnectionState.Open)
                    cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var obj = new SalesOrderDetail();

                        // 2. Map only what we selected
                        obj.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                        obj.SalesOrderId = reader.GetInt32(reader.GetOrdinal("SalesOrderId"));
                        obj.Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"));

                        // Note: We removed the line reading ProductVariantId because it caused the crash
                        // and your Delivery logic doesn't use it anyway.

                        list.Add(obj);
                    }
                }
            }
            return list;
        }
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

                        // ✅ FIX: Map SalesOrderId correctly
                        if (!reader.IsDBNull(reader.GetOrdinal("SalesOrderId")))
                        {
                            detail.SalesOrderId = reader.GetInt32(reader.GetOrdinal("SalesOrderId"));
                        }

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
