using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using MDUA.Entities;
using MDUA.Entities.List;

namespace MDUA.DataAccess
{
    public partial class BackInStockRequestDataAccess
    {
        public BackInStockRequestList GetPendingByProductVariantId(int productVariantId)
        {
            const string SQLQuery = @"
SELECT *
FROM BackInStockRequest
WHERE ProductVariantId = @ProductVariantId
  AND IsNotified = 0
ORDER BY RequestDate ASC";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            AddParameter(cmd, pInt32("ProductVariantId", productVariantId));

            return GetList(cmd, ALL_AVAILABLE_RECORDS);
        }

        public bool HasPendingRequest(int productVariantId, string contactNumber)
        {
            const string SQLQuery = @"
SELECT TOP 1 1
FROM BackInStockRequest
WHERE ProductVariantId = @ProductVariantId
  AND ContactNumber = @ContactNumber
  AND IsNotified = 0";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            AddParameter(cmd, pInt32("ProductVariantId", productVariantId));
            AddParameter(cmd, pNVarChar("ContactNumber", 50, contactNumber));

            SqlDataReader reader;
            SelectRecords(cmd, out reader);

            using (reader)
            {
                return reader.Read();
            }
        }

        public int MarkNotified(IEnumerable<int> requestIds, string updatedBy)
        {
            var idList = requestIds?.Where(id => id > 0).Distinct().ToList() ?? new List<int>();
            if (idList.Count == 0)
            {
                return 0;
            }

            var paramNames = new List<string>();
            using SqlCommand cmd = GetSQLCommand(string.Empty);

            for (int i = 0; i < idList.Count; i++)
            {
                string paramName = $"@Id{i}";
                paramNames.Add(paramName);
                AddParameter(cmd, pInt32(paramName.TrimStart('@'), idList[i]));
            }

            AddParameter(cmd, pNVarChar("UpdatedBy", 100, updatedBy));

            cmd.CommandText = $@"
UPDATE BackInStockRequest
SET IsNotified = 1,
    NotifiedDate = GETUTCDATE(),
    UpdatedBy = @UpdatedBy,
    UpdatedAt = GETUTCDATE()
WHERE Id IN ({string.Join(",", paramNames)})";

            return (int)ExecuteCommand(cmd);
        }

        public int GetPendingRequestCount(int companyId)
        {
            const string SQLQuery = @"
SELECT COUNT(1)
FROM BackInStockRequest req
INNER JOIN ProductVariant pv ON pv.Id = req.ProductVariantId
INNER JOIN Product p ON p.Id = pv.ProductId
WHERE req.IsNotified = 0
  AND p.CompanyId = @CompanyId";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            AddParameter(cmd, pInt32("CompanyId", companyId));

            SqlDataReader reader;
            SelectRecords(cmd, out reader);

            using (reader)
            {
                if (reader.Read() && !reader.IsDBNull(0))
                {
                    return reader.GetInt32(0);
                }
            }

            return 0;
        }

        public List<BackInStockRequestSummary> GetPendingRequestSummaries(int companyId, int top = 20)
        {
            const string SQLQuery = @"
SELECT TOP (@Top)
    p.Id AS ProductId,
    p.ProductName,
    pv.Id AS VariantId,
    pv.VariantName,
    COUNT(1) AS RequestCount
FROM BackInStockRequest req
INNER JOIN ProductVariant pv ON pv.Id = req.ProductVariantId
INNER JOIN Product p ON p.Id = pv.ProductId
WHERE req.IsNotified = 0
  AND p.CompanyId = @CompanyId
GROUP BY p.Id, p.ProductName, pv.Id, pv.VariantName
ORDER BY COUNT(1) DESC, p.ProductName, pv.VariantName";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            AddParameter(cmd, pInt32("CompanyId", companyId));
            AddParameter(cmd, pInt32("Top", top));

            SqlDataReader reader;
            SelectRecords(cmd, out reader);

            var results = new List<BackInStockRequestSummary>();

            using (reader)
            {
                while (reader.Read())
                {
                    results.Add(new BackInStockRequestSummary
                    {
                        ProductId = reader.GetInt32(0),
                        ProductName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        VariantId = reader.GetInt32(2),
                        VariantName = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        RequestCount = reader.GetInt32(4)
                    });
                }
            }

            return results;
        }

        public List<BackInStockRequestDetail> GetPendingRequestDetails(int companyId)
        {
            const string SQLQuery = @"
SELECT
    p.Id AS ProductId,
    p.ProductName,
    pv.Id AS VariantId,
    pv.VariantName,
    COUNT(1) AS RequestCount,
    MIN(req.RequestDate) AS FirstRequestDate,
    MAX(req.RequestDate) AS LatestRequestDate
FROM BackInStockRequest req
INNER JOIN ProductVariant pv ON pv.Id = req.ProductVariantId
INNER JOIN Product p ON p.Id = pv.ProductId
WHERE req.IsNotified = 0
  AND p.CompanyId = @CompanyId
GROUP BY p.Id, p.ProductName, pv.Id, pv.VariantName
ORDER BY COUNT(1) DESC, LatestRequestDate DESC";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            AddParameter(cmd, pInt32("CompanyId", companyId));

            SqlDataReader reader;
            SelectRecords(cmd, out reader);

            var results = new List<BackInStockRequestDetail>();

            using (reader)
            {
                while (reader.Read())
                {
                    results.Add(new BackInStockRequestDetail
                    {
                        ProductId = reader.GetInt32(0),
                        ProductName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        VariantId = reader.GetInt32(2),
                        VariantName = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        RequestCount = reader.GetInt32(4),
                        FirstRequestDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                        LatestRequestDate = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6)
                    });
                }
            }

            return results;
        }
    }
}
