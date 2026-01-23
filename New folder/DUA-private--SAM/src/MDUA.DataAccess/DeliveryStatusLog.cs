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

            // ✅ FIX: Replaced complex Product loop with direct CompanyCustomer check.
            // This matches the logic in DeliveryDataAccess and works even if an order has no items yet.
            string sql = @"
    SELECT TOP 500 log.* FROM [dbo].[DeliveryStatusLog] log
    
    -- 1. Helper: Get Delivery info if needed
    LEFT JOIN [dbo].[Delivery] d ON log.EntityType = 'Delivery' AND log.EntityId = d.Id

    -- 2. Link to Order Header (using Log's ID or Delivery's ID)
    INNER JOIN [dbo].[SalesOrderHeader] soh 
        ON soh.Id = COALESCE(log.SalesOrderId, d.SalesOrderId)

    -- 3. Security: Filter by Company via the Customer link
    INNER JOIN [dbo].[CompanyCustomer] cc ON soh.CompanyCustomerId = cc.Id
    
    WHERE cc.CompanyId = @CompanyId ";

            // --- Dynamic Filters ---
            if (from.HasValue)
                sql += " AND log.ChangedAt >= @FromDate";

            if (to.HasValue)
                sql += " AND log.ChangedAt <= @ToDate";

            if (!string.IsNullOrEmpty(entityType) && entityType != "All")
                sql += " AND log.EntityType = @EntityType";

            if (!string.IsNullOrWhiteSpace(search))
            {
                // Search by Order ID (soh.Id) or User Name
                sql += " AND (CAST(soh.Id AS NVARCHAR) = @SearchTerm OR log.ChangedBy LIKE @SearchLike)";
            }

            sql += " ORDER BY log.ChangedAt DESC";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));

                if (from.HasValue) AddParameter(cmd, pDateTime("FromDate", from.Value));
                // End of day adjustment
                if (to.HasValue) AddParameter(cmd, pDateTime("ToDate", to.Value.Date.AddDays(1).AddTicks(-1)));

                if (!string.IsNullOrEmpty(entityType) && entityType != "All")
                    AddParameter(cmd, pNVarChar("EntityType", 50, entityType));

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