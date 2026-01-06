using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using MDUA.Entities;
using MDUA.DataAccess.Interface;

namespace MDUA.DataAccess
{
    public partial class DeliveryStatusLogDataAccess : IDeliveryStatusLogDataAccess

    {

        public List<DeliveryStatusLog> GetLogsForReport(int companyId, DateTime? from, DateTime? to, string search, string entityType)
        {
            var list = new List<DeliveryStatusLog>();

            // ✅ FIX: Use EXISTS to check ownership via the Product table
            // We join Order -> OrderDetail -> Variant -> Product to find the CompanyId
            string sql = @"
        SELECT TOP 500 log.* FROM [dbo].[DeliveryStatusLog] log
        INNER JOIN [dbo].[SalesOrderHeader] soh ON log.SalesOrderId = soh.Id
        WHERE EXISTS (
            SELECT 1 
            FROM [dbo].[SalesOrderDetail] sod
            INNER JOIN [dbo].[ProductVariant] pv ON sod.ProductId = pv.Id
            INNER JOIN [dbo].[Product] p ON pv.ProductId = p.Id
            WHERE sod.SalesOrderId = soh.Id 
              AND p.CompanyId = @CompanyId
        ) ";

            if (from.HasValue)
                sql += " AND log.ChangedAt >= @FromDate";

            if (to.HasValue)
                sql += " AND log.ChangedAt <= @ToDate";

            if (!string.IsNullOrEmpty(entityType) && entityType != "All")
                sql += " AND log.EntityType = @EntityType";

            if (!string.IsNullOrWhiteSpace(search))
            {
                sql += " AND (CAST(log.SalesOrderId AS NVARCHAR) = @SearchTerm OR log.ChangedBy LIKE @SearchLike)";
            }

            sql += " ORDER BY log.ChangedAt DESC";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));

                if (from.HasValue) AddParameter(cmd, pDateTime("FromDate", from.Value));
                if (to.HasValue) AddParameter(cmd, pDateTime("ToDate", to.Value.AddDays(1).AddTicks(-1)));
                if (!string.IsNullOrEmpty(entityType) && entityType != "All") AddParameter(cmd, pNVarChar("EntityType", 50, entityType));

                if (!string.IsNullOrWhiteSpace(search))
                {
                    AddParameter(cmd, pNVarChar("SearchTerm", 50, search.Trim()));
                    AddParameter(cmd, pNVarChar("SearchLike", 100, "%" + search.Trim() + "%"));
                }

                SqlDataReader reader;
                SelectRecords(cmd, out reader);

                using (reader)
                {
                    while (reader.Read())
                    {
                        var log = new DeliveryStatusLog();
                        FillObject(log, reader);
                        list.Add(log);
                    }
                    reader.Close();
                }
            }
            return list;
        }
    }
}