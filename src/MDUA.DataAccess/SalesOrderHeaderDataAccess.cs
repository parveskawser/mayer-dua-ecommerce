using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq; // Needed for FirstOrDefault

using MDUA.Framework;
using MDUA.Framework.Exceptions;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess
{
    public partial class SalesOrderHeaderDataAccess
    {
        public long InsertSalesOrderHeaderSafe(SalesOrderHeader order)
        {
            // ✅ We insert TotalAmount (Products + Delivery). 
            // DB automatically calculates NetAmount = TotalAmount - DiscountAmount.
            string SQLQuery = @"
                INSERT INTO [dbo].[SalesOrderHeader]
                ([CompanyCustomerId], [AddressId], [SalesChannelId], [OrderDate], 
                 [TotalAmount], [DiscountAmount], [Status], [IsActive], [Confirmed], 
                 [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt], [SessionId], [IPAddress])
                VALUES
                (@CompanyCustomerId, @AddressId, @SalesChannelId, @OrderDate, 
                 @TotalAmount, @DiscountAmount, @Status, @IsActive, @Confirmed, 
                 @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @SessionId, @IPAddress);
                 
                SELECT CONVERT(INT, SCOPE_IDENTITY());";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("CompanyCustomerId", order.CompanyCustomerId));
                AddParameter(cmd, pInt32("AddressId", order.AddressId));
                AddParameter(cmd, pInt32("SalesChannelId", order.SalesChannelId));
                AddParameter(cmd, pDateTime("OrderDate", order.OrderDate));

                // ✅ TotalAmount includes Delivery Charge
                AddParameter(cmd, pDecimal("TotalAmount", order.TotalAmount));
                AddParameter(cmd, pDecimal("DiscountAmount", order.DiscountAmount));

                // ❌ NetAmount is computed in DB, so we don't insert it.

                AddParameter(cmd, pNVarChar("Status", 30, order.Status));
                AddParameter(cmd, pBool("IsActive", order.IsActive));
                AddParameter(cmd, pBool("Confirmed", order.Confirmed));
                AddParameter(cmd, pNVarChar("CreatedBy", 100, order.CreatedBy));
                AddParameter(cmd, pDateTime("CreatedAt", order.CreatedAt));
                AddParameter(cmd, pNVarChar("UpdatedBy", 100, null));
                AddParameter(cmd, pDateTime("UpdatedAt", null));
                AddParameter(cmd, pNVarChar("SessionId", 100, order.SessionId ?? ""));
                AddParameter(cmd, pVarChar("IPAddress", 45, order.IPAddress ?? ""));

                SqlDataReader reader;
                SelectRecords(cmd, out reader);
                int newId = 0;
                using (reader)
                {
                    if (reader.Read() && !reader.IsDBNull(0)) newId = reader.GetInt32(0);
                    reader.Close();
                }
                return newId;
            }
        }

        // ✅ NEW CRITICAL FIX: Update TotalAmount correctly when Delivery changes
        // Logic: TotalAmount = (Sum of Product Details) + DiscountAmount + NewDelivery
        // This ensures the DB computed column NetAmount (Total - Discount) works correctly.
        public void UpdateOrderDeliveryCharge(int orderId, decimal newDeliveryCharge)
        {
            string SQLQuery = @"
                UPDATE [dbo].[SalesOrderHeader]
                SET [TotalAmount] = (
                    ISNULL((SELECT SUM(UnitPrice * Quantity) FROM SalesOrderDetail WHERE SalesOrderId = @Id), 0)
                    + [DiscountAmount]
                    + @Delivery
                ),
                [UpdatedAt] = GETUTCDATE()
                WHERE [Id] = @Id";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("Id", orderId));
                AddParameter(cmd, pDecimal("Delivery", newDeliveryCharge));

                ExecuteCommand(cmd);
            }
        }

        public void UpdateTotalAmountSafe(int orderId, decimal newTotalAmount)
        {
            string SQLQuery = @"
                UPDATE [dbo].[SalesOrderHeader] 
                SET [TotalAmount] = @Total, 
                    [UpdatedAt] = GETUTCDATE() 
                WHERE [Id] = @Id";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("Id", orderId));
                AddParameter(cmd, pDecimal("Total", newTotalAmount));

                // ✅ FIX: Use the safe helper method 'ExecuteCommand' from BaseDataAccess.
                ExecuteCommand(cmd);
            }
        }

        public decimal GetProductTotalFromDetails(int orderId)
        {
            // Sums (UnitPrice * Quantity) for all items in this order
            string SQLQuery = "SELECT ISNULL(SUM(UnitPrice * Quantity), 0) FROM SalesOrderDetail WHERE SalesOrderId = @Id";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("Id", orderId));
                object result = SelectScaler(cmd);
                return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0m;
            }
        }

        public SalesOrderHeaderList GetOrdersByCompanyCustomer(int customerId)
        {
            // ✅ Explicitly select ONLY non-computed columns and critical FKs.
            string SQLQuery = @"
                SELECT soh.Id, soh.CompanyCustomerId, soh.AddressId, soh.SalesChannelId, 
                       soh.OrderDate, soh.TotalAmount, soh.DiscountAmount, 
                       soh.SessionId, soh.IPAddress, soh.Status, soh.IsActive, soh.Confirmed, 
                       soh.CreatedBy, soh.CreatedAt, soh.UpdatedBy, soh.UpdatedAt
                       
                FROM SalesOrderHeader soh
                JOIN CompanyCustomer cc ON soh.CompanyCustomerId = cc.Id
                WHERE cc.CustomerId = @CustomerId
                ORDER BY soh.OrderDate DESC";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("CustomerId", customerId));
                return GetList(cmd, 0);
            }
        }

        public SalesOrderHeaderList GetOrdersByCustomerId(int customerId)
        {
            string SQLQuery = @"
            SELECT 
                soh.Id, 
                soh.CompanyCustomerId, 
                soh.AddressId, 
                soh.SalesChannelId, 
                soh.SalesOrderId,      -- Index 4
                soh.OnlineOrderId,     -- Index 5
                soh.DirectOrderId,     -- Index 6
                soh.OrderDate,         -- Index 7
                soh.TotalAmount,       -- Index 8
                soh.DiscountAmount,    -- Index 9
                soh.NetAmount,         -- Index 10
                soh.SessionId, 
                soh.IPAddress, 
                soh.Status, 
                soh.IsActive, 
                soh.Confirmed, 
                soh.CreatedBy, 
                soh.CreatedAt, 
                soh.UpdatedBy, 
                soh.UpdatedAt
            FROM SalesOrderHeader soh
            JOIN CompanyCustomer cc ON soh.CompanyCustomerId = cc.Id
            WHERE cc.CustomerId = @CustomerId
            ORDER BY soh.OrderDate DESC";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("CustomerId", customerId));
                return GetList(cmd, -1);
            }
        }

        public List<object> GetOrderReceiptByOnlineId(string onlineOrderId)
        {
            const string spName = "[dbo].[GetSalesOrderReceiptByOnlineOrderId]";
            var receiptData = new List<object>();
            SqlDataReader reader = null;

            using (SqlCommand cmd = GetSQLCommand(spName))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                AddParameter(cmd, pNVarChar("OnlineOrderId", 10, onlineOrderId));

                try
                {
                    SelectRecords(cmd, out reader);
                    if (reader != null && reader.HasRows)
                    {
                        var columnNames = new List<string>(reader.FieldCount);
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            columnNames.Add(reader.GetName(i));
                        }

                        while (reader.Read())
                        {
                            var rowData = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                rowData[columnNames[i]] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }
                            receiptData.Add(rowData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching order receipt by online ID.", ex);
                }
                finally
                {
                    if (reader != null) reader.Close();
                }
            }
            return receiptData;
        }

        public SalesOrderHeaderList GetAllSalesOrderHeaders()
        {
            // ✅ Joins Customer & Address to fetch full details efficiently
            const string SQLQuery = @"
                SELECT 
                    soh.[Id], 
                    soh.[CompanyCustomerId], 
                    soh.[AddressId], 
                    soh.[SalesChannelId], 
                    soh.[OnlineOrderId], 
                    soh.[DirectOrderId], 
                    soh.[OrderDate], 
                    soh.[TotalAmount], 
                    soh.[DiscountAmount], 
                    soh.[NetAmount], 
                    soh.[SessionId], 
                    soh.[IPAddress], 
                    soh.[Status], 
                    soh.[IsActive], 
                    soh.[Confirmed], 
                    soh.[CreatedBy], 
                    soh.[CreatedAt], 
                    soh.[UpdatedBy], 
                    soh.[UpdatedAt], 
                    soh.[SalesOrderId],
                    
                    -- ✅ FETCH ADDRESS DETAILS
                    ISNULL(a.[Street], '') AS Street,
                    ISNULL(a.[City], '') AS City,
                    ISNULL(a.[Divison], '') AS Divison,
                    ISNULL(a.[Thana], '') AS Thana,
                    ISNULL(a.[SubOffice], '') AS SubOffice,
                    ISNULL(a.[PostalCode], '') AS PostalCode,
                    ISNULL(a.[Country], 'Bangladesh') AS Country,

                    -- ✅ FETCH CUSTOMER NAME
                    ISNULL(c.[CustomerName], 'Guest') AS CustomerName,
                    ISNULL(c.[Phone], '') AS CustomerPhone,
                    ISNULL(c.[Email], '') AS CustomerEmail,

                    -- Payment Stats
                    ISNULL((
                        SELECT SUM(cp.Amount) 
                        FROM CustomerPayment cp 
                        WHERE cp.TransactionReference = soh.SalesOrderId
                    ), 0) AS PaidAmount,

                    (soh.[NetAmount] - ISNULL((
                        SELECT SUM(cp.Amount) 
                        FROM CustomerPayment cp 
                        WHERE cp.TransactionReference = soh.SalesOrderId
                    ), 0)) AS DueAmount

                FROM [dbo].[SalesOrderHeader] soh
                LEFT JOIN [dbo].[Address] a ON soh.AddressId = a.Id
                LEFT JOIN [dbo].[CompanyCustomer] cc ON soh.CompanyCustomerId = cc.Id
                LEFT JOIN [dbo].[Customer] c ON cc.CustomerId = c.Id
                ORDER BY soh.[OrderDate] DESC";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                SqlDataReader reader;
                SelectRecords(cmd, out reader);
                SalesOrderHeaderList list = new SalesOrderHeaderList();

                using (reader)
                {
                    while (reader.Read())
                    {
                        SalesOrderHeader order = new SalesOrderHeader();
                        int i = 0;

                        // 1. Base Fields (Indices 0-19)
                        order.Id = reader.GetInt32(i++);
                        order.CompanyCustomerId = reader.GetInt32(i++);
                        order.AddressId = reader.GetInt32(i++);
                        order.SalesChannelId = reader.GetInt32(i++);
                        order.OnlineOrderId = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                        order.DirectOrderId = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                        order.OrderDate = reader.GetDateTime(i++);
                        order.TotalAmount = reader.GetDecimal(i++);
                        order.DiscountAmount = reader.GetDecimal(i++);
                        order.NetAmount = reader.GetDecimal(i++);
                        order.SessionId = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                        order.IPAddress = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                        order.Status = reader.GetString(i++);
                        order.IsActive = reader.GetBoolean(i++);
                        order.Confirmed = reader.GetBoolean(i++);
                        order.CreatedBy = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                        order.CreatedAt = reader.GetDateTime(i++);
                        order.UpdatedBy = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                        order.UpdatedAt = reader.IsDBNull(i) ? (DateTime?)null : reader.GetDateTime(i); i++;
                        order.SalesOrderId = reader.IsDBNull(i) ? null : reader.GetString(i); i++;

                        // 2. ✅ Address Mapping
                        order.Street = reader.GetString(i++);
                        order.City = reader.GetString(i++);
                        order.Divison = reader.GetString(i++);
                        order.Thana = reader.GetString(i++);
                        order.SubOffice = reader.GetString(i++);
                        order.PostalCode = reader.GetString(i++);
                        order.Country = reader.GetString(i++);

                        // 3. ✅ Customer Mapping
                        order.CustomerName = reader.GetString(i++);
                        order.CustomerPhone = reader.GetString(i++);
                        order.CustomerEmail = reader.GetString(i++);

                        // 4. Financials
                        order.PaidAmount = reader.GetDecimal(i++);
                        order.DueAmount = reader.GetDecimal(i++);

                        order.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
                        list.Add(order);
                    }
                    reader.Close();
                }
                return list;
            }
        }


        public void UpdateStatusSafe(int orderId, string status, bool confirmed)
        {
            {
                string SQLQuery = @"
            UPDATE [dbo].[SalesOrderHeader]
            SET 
                [Status] = @Status,
                [Confirmed] = @Confirmed,
                [UpdatedAt] = GETUTCDATE()
            WHERE [Id] = @Id";

                using (SqlCommand cmd = GetSQLCommand(SQLQuery))
                {
                    AddParameter(cmd, pInt32("Id", orderId));
                    AddParameter(cmd, pNVarChar("Status", 30, status));
                    AddParameter(cmd, pBool("Confirmed", confirmed));

                    if (cmd.Connection.State != System.Data.ConnectionState.Open)
                        cmd.Connection.Open();

                    int rowsAffected = cmd.ExecuteNonQuery();

                    cmd.Connection.Close();

                    if (rowsAffected == 0)
                    {
                        throw new Exception($"CRITICAL FAILURE: Tried to update Order #{orderId}, but database found 0 matching rows. The Order ID might be wrong or the Order doesn't exist.");
                    }
                }
            }
        } 

        public List<Dictionary<string, object>> GetVariantsForDropdown()
        {
            var list = new List<Dictionary<string, object>>();

            string SQLQuery = @"
                SELECT 
                    p.Id as ProductId,
                    ISNULL(p.ProductName, 'Unknown Product') as ProductName,
                    v.Id as VariantId, 
                    ISNULL(v.VariantName, 'Standard') as VariantName,
                    ISNULL(vps.StockQty, 0) as StockQty,
                    ISNULL(vps.Price, ISNULL(p.BasePrice, 0.00)) as Price
                FROM ProductVariant v
                JOIN Product p ON v.ProductId = p.Id
                LEFT JOIN VariantPriceStock vps ON v.Id = vps.Id
                WHERE v.IsActive = 1 AND p.IsActive = 1
                ORDER BY p.ProductName, v.VariantName";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    int ordPid = reader.GetOrdinal("ProductId");
                    int ordPName = reader.GetOrdinal("ProductName");
                    int ordVid = reader.GetOrdinal("VariantId");
                    int ordVName = reader.GetOrdinal("VariantName");
                    int ordStock = reader.GetOrdinal("StockQty");
                    int ordPrice = reader.GetOrdinal("Price");

                    while (reader.Read())
                    {
                        var item = new Dictionary<string, object>
                        {
                            { "ProductId", reader.GetInt32(ordPid) },
                            { "ProductName", reader.GetString(ordPName) },
                            { "VariantId", reader.GetInt32(ordVid) },
                            { "VariantName", reader.GetString(ordVName) },
                            { "Stock", reader.GetInt32(ordStock) },
                            { "Price", Convert.ToDecimal(reader.GetValue(ordPrice)) }
                        };
                        list.Add(item);
                    }
                    reader.Close();
                }
                cmd.Connection.Close();
            }
            return list;
        }

        public (int StockQty, decimal Price)? GetVariantStockAndPrice(int variantId)
        {
            string SQLQuery = @"
                SELECT 
                    ISNULL(StockQty, 0) as StockQty, 
                    ISNULL(Price, 0.00) as Price 
                FROM VariantPriceStock 
                WHERE Id = @Id";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("Id", variantId));

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int stock = reader.GetInt32(0);
                        decimal price = Convert.ToDecimal(reader.GetValue(1));
                        return (stock, price);
                    }
                }
                cmd.Connection.Close();
            }
            return null;
        }

        public DashboardStats GetDashboardStats()
        {
            var stats = new DashboardStats();

            string SQLQuery = @"
                SELECT 
                    (SELECT ISNULL(SUM(TotalAmount - DiscountAmount), 0) FROM SalesOrderHeader WHERE Status = 'Confirmed') as TotalRevenue,
                    (SELECT COUNT(*) FROM SalesOrderHeader) as TotalOrders,
                    (SELECT COUNT(*) FROM SalesOrderHeader WHERE Status IN ('Draft', 'Pending')) as PendingOrders,
                    (SELECT COUNT(*) FROM SalesOrderHeader WHERE CAST(OrderDate AS DATE) = CAST(GETUTCDATE() AS DATE)) as TodayOrders,
                    (SELECT COUNT(*) FROM Customer WHERE IsActive = 1) as TotalCustomers";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        stats.TotalRevenue = reader.GetDecimal(0);
                        stats.TotalOrders = reader.GetInt32(1);
                        stats.PendingOrders = reader.GetInt32(2);
                        stats.TodayOrders = reader.GetInt32(3);
                        stats.TotalCustomers = reader.GetInt32(4);
                    }
                }
                cmd.Connection.Close();
            }
            return stats;
        }

        public List<SalesOrderHeader> GetRecentOrders(int count = 5)
        {
            string SQLQuery = $@"
                SELECT TOP ({count}) 
                    [Id], [CompanyCustomerId], [AddressId], [SalesChannelId], [OnlineOrderId], [DirectOrderId], 
                    [OrderDate], [TotalAmount], [DiscountAmount], [NetAmount], [SessionId], [IPAddress], 
                    [Status], [IsActive], [Confirmed], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt], [SalesOrderId]
                FROM [dbo].[SalesOrderHeader]
                ORDER BY [OrderDate] DESC";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                SqlDataReader reader;
                SelectRecords(cmd, out reader);
                SalesOrderHeaderList list = new SalesOrderHeaderList();

                using (reader)
                {
                    while (reader.Read())
                    {
                        SalesOrderHeader order = new SalesOrderHeader();
                        int i = 0;
                        order.Id = reader.GetInt32(i++);
                        order.CompanyCustomerId = reader.GetInt32(i++);
                        order.AddressId = reader.GetInt32(i++);
                        order.SalesChannelId = reader.GetInt32(i++);
                        order.OnlineOrderId = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                        order.DirectOrderId = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                        order.OrderDate = reader.GetDateTime(i++);
                        order.TotalAmount = reader.GetDecimal(i++);
                        order.DiscountAmount = reader.GetDecimal(i++);
                        order.NetAmount = reader.GetDecimal(i++);
                        order.SessionId = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                        order.IPAddress = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                        order.Status = reader.GetString(i++);
                        order.IsActive = reader.GetBoolean(i++);
                        order.Confirmed = reader.GetBoolean(i++);
                        order.CreatedBy = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                        order.CreatedAt = reader.GetDateTime(i++);
                        order.UpdatedBy = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                        order.UpdatedAt = reader.IsDBNull(i) ? (DateTime?)null : reader.GetDateTime(i); i++;
                        order.SalesOrderId = reader.IsDBNull(i) ? null : reader.GetString(i); i++;

                        order.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
                        list.Add(order);
                    }
                    reader.Close();
                }
                return list;
            }
        }

        public List<ChartDataPoint> GetSalesTrend(int months = 6)
        {
            var list = new List<ChartDataPoint>();
            string SQLQuery = @"
                SELECT 
                    DATENAME(month, OrderDate) + ' ' + CAST(YEAR(OrderDate) AS VARCHAR(4)) as Label,
                    SUM(TotalAmount - DiscountAmount) as Value,
                    MIN(OrderDate) as SortDate
                FROM SalesOrderHeader
                WHERE OrderDate >= DATEADD(month, -@Months, GETUTCDATE())
                  AND Status != 'Cancelled'
                GROUP BY YEAR(OrderDate), MONTH(OrderDate), DATENAME(month, OrderDate)
                ORDER BY MIN(OrderDate)";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("Months", months));
                if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ChartDataPoint
                        {
                            Label = reader.GetString(0),
                            Value = Convert.ToDecimal(reader.GetValue(1))
                        });
                    }
                }
                cmd.Connection.Close();
            }
            return list;
        }

        public List<ChartDataPoint> GetOrderStatusCounts()
        {
            var list = new List<ChartDataPoint>();
            string SQLQuery = @"SELECT Status, COUNT(*) FROM SalesOrderHeader GROUP BY Status";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ChartDataPoint
                        {
                            Label = reader.GetString(0),
                            Value = Convert.ToDecimal(reader.GetValue(1))
                        });
                    }
                }
                cmd.Connection.Close();
            }
            return list;
        }

        public void UpdateNetAmountSafe(int orderId, decimal newNetAmount)
        {
            string SQLQuery = @"UPDATE [dbo].[SalesOrderHeader] SET [NetAmount] = @Net, [UpdatedAt] = GETUTCDATE() WHERE [Id] = @Id";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("Id", orderId));
                AddParameter(cmd, pDecimal("Net", newNetAmount));

                if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
        }

        public SalesOrderHeader GetBySalesOrderRef(string salesOrderRef)
        {
            string sql = @"
            SELECT TOP 1 *
            FROM SalesOrderHeader
            WHERE SalesOrderId = @SalesOrderId";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                AddParameter(cmd, pNVarChar("SalesOrderId", 50, salesOrderRef));

                SqlDataReader reader;
                SelectRecords(cmd, out reader);

                using (reader)
                {
                    if (!reader.Read()) return null;

                    SalesOrderHeader order = new SalesOrderHeader();
                    int i = 0;

                    order.Id = reader.GetInt32(i++);
                    order.CompanyCustomerId = reader.GetInt32(i++);
                    order.AddressId = reader.GetInt32(i++);
                    order.SalesChannelId = reader.GetInt32(i++);
                    order.OnlineOrderId = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                    order.DirectOrderId = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                    order.OrderDate = reader.GetDateTime(i++);
                    order.TotalAmount = reader.GetDecimal(i++);
                    order.DiscountAmount = reader.GetDecimal(i++);
                    order.NetAmount = reader.GetDecimal(i++);
                    order.SessionId = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                    order.IPAddress = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                    order.Status = reader.GetString(i++);
                    order.IsActive = reader.GetBoolean(i++);
                    order.Confirmed = reader.GetBoolean(i++);
                    order.CreatedBy = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                    order.CreatedAt = reader.GetDateTime(i++);
                    order.UpdatedBy = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                    order.UpdatedAt = reader.IsDBNull(i) ? (DateTime?)null : reader.GetDateTime(i); i++;
                    order.SalesOrderId = reader.IsDBNull(i) ? null : reader.GetString(i);

                    return order;
                }
            }
        }

        public SalesOrderHeader GetOrderTotalsSafe(int orderId)
        {
            var order = new SalesOrderHeader();

            // We only select the columns we calculate with. 
            // We SKIP date columns to avoid the crash.
            string sql = @"
            SELECT 
                [TotalAmount], 
                [DiscountAmount] 
            FROM [SalesOrderHeader] 
            WHERE [Id] = @Id";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", System.Data.SqlDbType.Int) { Value = orderId });

                if (cmd.Connection.State != System.Data.ConnectionState.Open)
                    cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Manual Mapping - Safe and Fast

                        // 1. TotalAmount
                        int idxTotal = reader.GetOrdinal("TotalAmount");
                        if (!reader.IsDBNull(idxTotal))
                            order.TotalAmount = reader.GetDecimal(idxTotal);
                        else
                            order.TotalAmount = 0;

                        // 2. DiscountAmount
                        int idxDisc = reader.GetOrdinal("DiscountAmount");
                        if (!reader.IsDBNull(idxDisc))
                            order.DiscountAmount = reader.GetDecimal(idxDisc);
                        else
                            order.DiscountAmount = 0;
                    }
                }
            }
            return order;
        }



        public bool GetConfirmedFlag(int orderId)
        {
            string sql = "SELECT Confirmed FROM dbo.SalesOrderHeader WHERE Id = @Id";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                AddParameter(cmd, pInt32("Id", orderId));

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                object val = cmd.ExecuteScalar();
                cmd.Connection.Close();

                if (val == null || val == DBNull.Value) return false;
                return Convert.ToBoolean(val);
            }
        }


        //Changed
        public void UpdateStatusSafeLogged(int orderId, string status, bool confirmed)
        {
            string sql = @"
UPDATE dbo.SalesOrderHeader
SET Status = @Status,
    Confirmed = @Confirmed,
UpdatedAt = @UpdatedAt 
WHERE Id = @Id;  
SELECT @@ROWCOUNT;";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                AddParameter(cmd, pInt32("Id", orderId));
                AddParameter(cmd, pNVarChar("Status", 30, status));
                AddParameter(cmd, pBool("Confirmed", confirmed));
                AddParameter(cmd, new SqlParameter("@UpdatedAt", SqlDbType.DateTime) { Value = DateTime.UtcNow });
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                var dbName = cmd.Connection.Database;
                int rows = Convert.ToInt32(cmd.ExecuteScalar());


                cmd.CommandText = "SELECT Status, Confirmed, UpdatedAt FROM dbo.SalesOrderHeader WHERE Id = @Id";
                cmd.Parameters.Clear();
                AddParameter(cmd, pInt32("Id", orderId));



                cmd.Connection.Close();

                if (rows == 0)
                    throw new Exception($"SOH update affected 0 rows. Wrong DB or invalid orderId={orderId}.");
            }
        }

        public int GetOrderPageNumber(int orderId, int pageSize)
        {
            // Logic: Find the rank of this order when sorted by OrderDate DESC
            // Then calculate: Page = Ceiling(Rank / PageSize)

            string sql = @"
        WITH OrderedOrders AS (
            SELECT Id, ROW_NUMBER() OVER (ORDER BY OrderDate DESC) as RowNum 
            FROM SalesOrderHeader
        )
        SELECT RowNum FROM OrderedOrders WHERE Id = @OrderId";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                AddParameter(cmd, pInt32("OrderId", orderId));

                if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();

                object result = cmd.ExecuteScalar();
                cmd.Connection.Close();

                if (result != null && int.TryParse(result.ToString(), out int rowNum))
                {
                    // Calculate Page Number (1-based)
                    return (int)Math.Ceiling((double)rowNum / pageSize);
                }
            }
            return 1; // Default to page 1 if not found
        }


        #region it is used in admin order list with pagination,Advance Payment modal and  order details modal in allorders.cshtml

        public SalesOrderHeaderList GetPagedOrdersExtended(int pageIndex, int pageSize, string whereClause, out int totalRows)
        {
            SalesOrderHeaderList list = new SalesOrderHeaderList();
            totalRows = 0;

            string sql = $@"
    SELECT 
        soh.Id, 
        soh.CompanyCustomerId, 
        soh.AddressId, 
        soh.SalesChannelId, 
        soh.OrderDate, 
        soh.TotalAmount, 
        soh.DiscountAmount, 
        soh.NetAmount,      -- Ensure this is selected
        soh.Status, 
        soh.IsActive, 
        soh.Confirmed, 
        soh.CreatedBy, 
        soh.CreatedAt, 
        soh.UpdatedBy, 
        soh.UpdatedAt,
        soh.SalesOrderId, 
        soh.OnlineOrderId,
        soh.DirectOrderId,
        soh.IPAddress,      
        soh.SessionId,     
        
        c.Id AS RealCustomerId,
        ISNULL(c.CustomerName, 'Guest') AS CustomerName,
        c.Phone as CustomerPhone,
        c.Email as CustomerEmail,
        
        -- ✅ Address Data for the Modal
        ISNULL(a.Street, '') AS Street,
        ISNULL(a.City, '') AS City,
        ISNULL(a.Divison, '') AS Divison,
        ISNULL(a.Thana, '') AS Thana,
        ISNULL(a.SubOffice, '') AS SubOffice,
        ISNULL(a.PostalCode, '') AS PostalCode,
        ISNULL(a.Country, 'Bangladesh') AS Country,

ISNULL((SELECT SUM(sod.UnitPrice * sod.Quantity) 
                FROM SalesOrderDetail sod 
                WHERE sod.SalesOrderId = soh.Id), 0) AS ProductTotal,

        ISNULL((SELECT SUM(cp.Amount) FROM CustomerPayment cp 
        WHERE cp.TransactionReference = soh.SalesOrderId), 0) AS PaidAmount,
        
        COUNT(*) OVER() AS TotalCount
    FROM SalesOrderHeader soh
    LEFT JOIN CompanyCustomer cc ON soh.CompanyCustomerId = cc.Id
    LEFT JOIN Customer c ON cc.CustomerId = c.Id
    LEFT JOIN Address a ON soh.AddressId = a.Id  -- ✅ Join Address
    WHERE {whereClause}
    ORDER BY soh.OrderDate DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                AddParameter(cmd, pInt32("Offset", (pageIndex - 1) * pageSize));
                AddParameter(cmd, pInt32("PageSize", pageSize));

                if (cmd.Connection.State != System.Data.ConnectionState.Open) cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (totalRows == 0) totalRows = (int)reader["TotalCount"];
                        var order = new SalesOrderHeader();

                        // --- IDs & Basic Info ---
                        order.Id = (int)reader["Id"];
                        order.SalesOrderId = reader["SalesOrderId"].ToString();
                        order.CompanyCustomerId = (int)reader["CompanyCustomerId"]; // ✅ Fixes "0" issue (if using CCID)
                        order.CustomerId = reader["RealCustomerId"] != DBNull.Value ? (int)reader["RealCustomerId"] : 0; // ✅ Fixes "0" issue (Real ID)
                        order.AddressId = (int)reader["AddressId"]; // ✅ Fixes Address ID N/A
                        order.SalesChannelId = (int)reader["SalesChannelId"];

                        order.OrderDate = (DateTime)reader["OrderDate"];
                        order.CreatedAt = (DateTime)reader["CreatedAt"]; // ✅ Now Mapped
                                                                         // Data comes as 'Unspecified' (Local). .ToUniversalTime() converts it to UTC format.
                        order.UpdatedAt = reader["UpdatedAt"] != DBNull.Value
                            ? ((DateTime)reader["UpdatedAt"]).ToUniversalTime()
                            : (DateTime?)null;

                        order.Status = reader["Status"].ToString();
                        order.Confirmed = (bool)reader["Confirmed"];

                        // --- Customer & Technical ---
                        order.CustomerName = reader["CustomerName"].ToString();
                        order.CustomerPhone = reader["CustomerPhone"] != DBNull.Value ? reader["CustomerPhone"].ToString() : "";
                        order.CustomerEmail = reader["CustomerEmail"] != DBNull.Value ? reader["CustomerEmail"].ToString() : "";
                        order.IPAddress = reader["IPAddress"] != DBNull.Value ? reader["IPAddress"].ToString() : "N/A"; // ✅ Fixes Technical Data
                        order.SessionId = reader["SessionId"] != DBNull.Value ? reader["SessionId"].ToString() : "N/A";

                        order.Street = reader["Street"].ToString();
                        order.City = reader["City"].ToString();
                        order.Divison = reader["Divison"].ToString();
                        order.Thana = reader["Thana"].ToString();
                        order.SubOffice = reader["SubOffice"].ToString();
                        order.PostalCode = reader["PostalCode"].ToString();
                        order.Country = reader["Country"].ToString();

                        // --- Financials ---
                        order.TotalAmount = reader["TotalAmount"] != DBNull.Value ? (decimal)reader["TotalAmount"] : 0m;
                        order.DiscountAmount = reader["DiscountAmount"] != DBNull.Value ? (decimal)reader["DiscountAmount"] : 0m;
                        order.NetAmount = reader["NetAmount"] != DBNull.Value ? (decimal)reader["NetAmount"] : 0m;
                        order.PaidAmount = reader["PaidAmount"] != DBNull.Value ? (decimal)reader["PaidAmount"] : 0m;
                        order.DueAmount = (order.NetAmount ?? 0m) - order.PaidAmount;
                        decimal productTotal = reader["ProductTotal"] != DBNull.Value ? (decimal)reader["ProductTotal"] : 0m;

                        if (order.TotalAmount > 0)
                        {
                            order.DeliveryCharge = order.TotalAmount - productTotal - order.DiscountAmount;

                            if (order.DeliveryCharge < 0) order.DeliveryCharge = 0;
                        }
                        else
                        {
                            order.DeliveryCharge = 0;
                        }
                        order.CreatedBy = reader["CreatedBy"] != DBNull.Value ? reader["CreatedBy"].ToString() : "";

                        list.Add(order);
                    }
                }
                cmd.Connection.Close();
            }
            return list;
        }
        #endregion

        public List<Dictionary<string, object>> GetExportDataDynamic(string whereClause, List<string> columns)
        {
            var results = new List<Dictionary<string, object>>();
            var selectParts = new List<string>();

            // Define the subquery for "Paid Amount" string to reuse it safely
            string paidSql = "ISNULL((SELECT SUM(cp.Amount) FROM CustomerPayment cp WHERE cp.TransactionReference = soh.SalesOrderId), 0)";

            foreach (var col in columns)
            {
                switch (col)
                {
                    case "Id": selectParts.Add("soh.SalesOrderId AS Id"); break;
                    case "OrderDate": selectParts.Add("FORMAT(soh.OrderDate, 'yyyy-MM-dd HH:mm') AS OrderDate"); break;
                    case "TotalAmount": selectParts.Add("soh.TotalAmount"); break;
                    case "Status": selectParts.Add("soh.Status"); break;
                    case "CustomerName": selectParts.Add("ISNULL(c.CustomerName, 'Guest') AS CustomerName"); break;
                    case "CustomerPhone": selectParts.Add("ISNULL(c.Phone, 'N/A') AS CustomerPhone"); break;
                    case "ShippingAddress": selectParts.Add("CONCAT(a.Street, ', ', a.City, '-', a.PostalCode) AS ShippingAddress"); break;
                    case "Paid":
                        selectParts.Add($"{paidSql} AS Paid");
                        break;

                    case "Due":
                        selectParts.Add($"(soh.NetAmount - {paidSql}) AS Due");
                        break;
                    case "PaymentStatus":
                        selectParts.Add($@"CASE 
                    WHEN (soh.NetAmount - {paidSql}) <= 0 THEN 'Paid' 
                    WHEN {paidSql} > 0 THEN 'Partial' 
                    ELSE 'Unpaid' END AS PaymentStatus");
                        break;
                    default: break;
                }
            }

            if (selectParts.Count == 0) selectParts.Add("soh.SalesOrderId");

            string selectClause = string.Join(", ", selectParts);

            string sql = $@"
        SELECT {selectClause}
        FROM SalesOrderHeader soh
        LEFT JOIN CompanyCustomer cc ON soh.CompanyCustomerId = cc.Id
        LEFT JOIN Customer c ON cc.CustomerId = c.Id
        LEFT JOIN Address a ON soh.AddressId = a.Id
        WHERE {whereClause}
        ORDER BY soh.OrderDate DESC";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                if (cmd.Connection.State != System.Data.ConnectionState.Open) cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string key = reader.GetName(i);
                            object val = reader.IsDBNull(i) ? "" : reader.GetValue(i);

                        

                            row[key] = val;
                        }
                        results.Add(row);
                    }
                }
                cmd.Connection.Close();
            }

            return results;
        }
    }


}