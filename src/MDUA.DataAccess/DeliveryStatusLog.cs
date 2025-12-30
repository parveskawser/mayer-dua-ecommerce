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

        public List<DeliveryStatusLog> GetLogsForReport(DateTime? from, DateTime? to, string search, string entityType)
        {
            var list = new List<DeliveryStatusLog>();

            // Build Dynamic SQL safely (or use a dedicated SP if preferred)
            string sql = @"
                SELECT TOP 500 * FROM [dbo].[DeliveryStatusLog]
                WHERE 1=1 ";

            if (from.HasValue)
                sql += " AND ChangedAt >= @FromDate";

            if (to.HasValue)
                sql += " AND ChangedAt <= @ToDate";

            if (!string.IsNullOrEmpty(entityType) && entityType != "All")
                sql += " AND EntityType = @EntityType";

            if (!string.IsNullOrWhiteSpace(search))
            {
               
                sql += " AND (CAST(SalesOrderId AS NVARCHAR) = @SearchTerm OR ChangedBy LIKE @SearchLike)";
            }

            sql += " ORDER BY ChangedAt DESC";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                if (from.HasValue) AddParameter(cmd, pDateTime("FromDate", from.Value));
                if (to.HasValue) AddParameter(cmd, pDateTime("ToDate", to.Value.AddDays(1).AddTicks(-1))); // Include full end day
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