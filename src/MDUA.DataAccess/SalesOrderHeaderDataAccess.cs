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
	public partial class SalesOrderHeaderDataAccess
	{
        public long InsertSalesOrderHeaderSafe(SalesOrderHeader order)
        {
            // ✅ BULLETPROOF METHOD: Inline SQL
            // We insert and immediately select the new ID.
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
                AddParameter(cmd, pDecimal("TotalAmount", order.TotalAmount));
                AddParameter(cmd, pDecimal("DiscountAmount", order.DiscountAmount));

                AddParameter(cmd, pNVarChar("Status", 30, order.Status));
                AddParameter(cmd, pBool("IsActive", order.IsActive));
                AddParameter(cmd, pBool("Confirmed", order.Confirmed));

                AddParameter(cmd, pNVarChar("CreatedBy", 100, order.CreatedBy));
                AddParameter(cmd, pDateTime("CreatedAt", order.CreatedAt));
                AddParameter(cmd, pNVarChar("UpdatedBy", 100, null));
                AddParameter(cmd, pDateTime("UpdatedAt", null));

                // Optional fields (handle nulls)
                AddParameter(cmd, pNVarChar("SessionId", 100, order.SessionId ?? ""));
                AddParameter(cmd, pVarChar("IPAddress", 45, order.IPAddress ?? ""));

                // ✅ Execute and Get ID
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


        public SalesOrderHeaderList GetOrdersByCompanyCustomer(int customerId)
        {
            // ✅ FINAL FIX: Explicitly select ONLY non-computed columns 
            // and critical FKs. This ensures FillObject maps correctly.

            // We select columns likely mapped in the first ~12 indices of FillObject
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
                // We ensure the parameter is explicitly defined as an integer.
                AddParameter(cmd, pInt32("CustomerId", customerId));

                // Use existing GetList helper
                return GetList(cmd, 0);
            }
        }

        // Add this method to your partial class
        // Inside SalesOrderHeaderDataAccess.Custom.cs

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

            SqlDataReader reader = null; // Initialize reader outside try block

            using (SqlCommand cmd = GetSQLCommand(spName))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Pass the OnlineOrderId parameter to the stored procedure
                AddParameter(cmd, pNVarChar("OnlineOrderId", 10, onlineOrderId));

                try
                {
                    // Execute the stored procedure and get the reader
                    SelectRecords(cmd, out reader);

                    if (reader != null && reader.HasRows)
                    {
                        // Cache column names
                        var columnNames = new List<string>(reader.FieldCount);
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            columnNames.Add(reader.GetName(i));
                        }

                        while (reader.Read())
                        {
                            // Use Dictionary<string, object> for dynamic mapping
                            var rowData = new Dictionary<string, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                rowData[columnNames[i]] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }

                            receiptData.Add(rowData); // Add the dictionary as an object
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Use the base Exception class for consistency if DataAccessException is unavailable
                    throw new Exception("Error fetching order receipt by online ID.", ex);
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
            }

            return receiptData;
        }

        public SalesOrderHeaderList GetAllSalesOrderHeaders()
        {
            // 1. Define the query explicitly. We select columns by name to be 100% sure of the data types.
            const string SQLQuery = @"
                SELECT 
                    [Id], 
                    [CompanyCustomerId], 
                    [AddressId], 
                    [SalesChannelId], 
                    [OnlineOrderId], 
                    [DirectOrderId], 
                    [OrderDate], 
                    [TotalAmount], 
                    [DiscountAmount], 
                    [NetAmount], 
                    [SessionId], 
                    [IPAddress], 
                    [Status], 
                    [IsActive], 
                    [Confirmed], 
                    [CreatedBy], 
                    [CreatedAt], 
                    [UpdatedBy], 
                    [UpdatedAt], 
                    [SalesOrderId]
                FROM [dbo].[SalesOrderHeader]
                ORDER BY [OrderDate] DESC";

            // 2. Execute the Inline Query
            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                SqlDataReader reader;
                SelectRecords(cmd, out reader);

                SalesOrderHeaderList list = new SalesOrderHeaderList();

                using (reader)
                {
                    while (reader.Read())
                    {
                        // 3. Manually map the row. 
                        // Since we wrote the SELECT above, we know the EXACT index of every column.
                        SalesOrderHeader order = new SalesOrderHeader();

                        int i = 0; // Start index

                        order.Id = reader.GetInt32(i++); // Index 0
                        order.CompanyCustomerId = reader.GetInt32(i++);
                        order.AddressId = reader.GetInt32(i++);
                        order.SalesChannelId = reader.GetInt32(i++);

                        // Strings (Handle Nulls)
                        order.OnlineOrderId = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                        order.DirectOrderId = reader.IsDBNull(i) ? null : reader.GetString(i); i++;

                        // 🛑 CORE DATE & DECIMALS (Guaranteed correct by query order) 🛑
                        order.OrderDate = reader.GetDateTime(i++); // Index 6
                        order.TotalAmount = reader.GetDecimal(i++); // Index 7
                        order.DiscountAmount = reader.GetDecimal(i++); // Index 8
                        order.NetAmount = reader.GetDecimal(i++); // Index 9

                        // More Strings
                        order.SessionId = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                        order.IPAddress = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                        order.Status = reader.GetString(i++); // Index 12

                        // Booleans
                        order.IsActive = reader.GetBoolean(i++);
                        order.Confirmed = reader.GetBoolean(i++);

                        // Audit
                        order.CreatedBy = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                        order.CreatedAt = reader.GetDateTime(i++);
                        order.UpdatedBy = reader.IsDBNull(i) ? null : reader.GetString(i); i++;
                        order.UpdatedAt = reader.IsDBNull(i) ? (DateTime?)null : reader.GetDateTime(i); i++;

                        // Computed SalesOrderId
                        order.SalesOrderId = reader.IsDBNull(i) ? null : reader.GetString(i); i++;

                        order.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
                        list.Add(order);
                    }
                    reader.Close();
                }
                return list;
            }
        }


        //new
        public void UpdateStatusSafe(int orderId, string status, bool confirmed)
        {
            string SQLQuery = @"
                UPDATE [dbo].[SalesOrderHeader]
                SET 
                    [Status] = @Status,
                    [Confirmed] = @Confirmed,
                    [UpdatedAt] = GETDATE()
                WHERE [Id] = @Id";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("Id", orderId));
                AddParameter(cmd, pNVarChar("Status", 30, status));
                AddParameter(cmd, pBool("Confirmed", confirmed));

                // ✅ FIX: ExecuteNonQuery requires an open connection.
                // GetSQLCommand returns a command with a closed connection by default.
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.ExecuteNonQuery();

                // Close the connection explicitly after work is done
                cmd.Connection.Close();
            }
        }

        public List<Dictionary<string, object>> GetVariantsForDropdown()
        {
            var list = new List<Dictionary<string, object>>();

            // Note: vps.StockQty is now joined correctly
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

        // ✅ FIXED: Return Tuple (int, decimal)? for safe access in Facade
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

                        // Return Tuple
                        return (stock, price);
                    }
                }
                cmd.Connection.Close();
            }
            return null;
        }


        // ✅ NEW: Fetch all KPI stats in one go
        public DashboardStats GetDashboardStats()
        {
            var stats = new DashboardStats();

            string SQLQuery = @"
                SELECT 
                    -- 1. Total Revenue (Confirmed Only)
                    (SELECT ISNULL(SUM(TotalAmount - DiscountAmount), 0) 
                     FROM SalesOrderHeader 
                     WHERE Status = 'Confirmed') as TotalRevenue,

                    -- 2. Total Orders
                    (SELECT COUNT(*) FROM SalesOrderHeader) as TotalOrders,

                    -- 3. Pending Actions (Draft or Pending)
                    (SELECT COUNT(*) FROM SalesOrderHeader 
                     WHERE Status IN ('Draft', 'Pending')) as PendingOrders,

                    -- 4. Today's Orders
                    (SELECT COUNT(*) FROM SalesOrderHeader 
                     WHERE CAST(OrderDate AS DATE) = CAST(GETDATE() AS DATE)) as TodayOrders,

                    -- 5. Total Customers
                    (SELECT COUNT(*) FROM Customer WHERE IsActive = 1) as TotalCustomers
            ";

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

        // ✅ NEW: Fetch top 5 recent orders
        public List<SalesOrderHeader> GetRecentOrders(int count = 5)
        {
            // Explicitly selecting columns to match your entity structure
            string SQLQuery = $@"
                SELECT TOP ({count}) 
                    [Id], [CompanyCustomerId], [AddressId], [SalesChannelId], [OnlineOrderId], [DirectOrderId], 
                    [OrderDate], [TotalAmount], [DiscountAmount], [NetAmount], [SessionId], [IPAddress], 
                    [Status], [IsActive], [Confirmed], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt], [SalesOrderId]
                FROM [dbo].[SalesOrderHeader]
                ORDER BY [OrderDate] DESC";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                // Reusing your existing internal list mapping logic
                // If you have a generic GetList(cmd), use it. Otherwise, this mimics GetAll() logic:
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

        // ✅ NEW: Get Monthly Sales Trend
        public List<ChartDataPoint> GetSalesTrend(int months = 6)
        {
            var list = new List<ChartDataPoint>();

            // Universal SQL Date Grouping (Works on older SQL versions too)
            string SQLQuery = @"
                SELECT 
                    DATENAME(month, OrderDate) + ' ' + CAST(YEAR(OrderDate) AS VARCHAR(4)) as Label,
                    SUM(TotalAmount - DiscountAmount) as Value,
                    MIN(OrderDate) as SortDate
                FROM SalesOrderHeader
                WHERE OrderDate >= DATEADD(month, -@Months, GETDATE())
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
                            Value = Convert.ToDecimal(reader.GetValue(1)) // Safe Cast
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
            string SQLQuery = @"
                SELECT Status, COUNT(*) 
                FROM SalesOrderHeader 
                GROUP BY Status";

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
                            Value = Convert.ToDecimal(reader.GetValue(1)) // Count is int, need decimal for model
                        });
                    }
                }
                cmd.Connection.Close();
            }
            return list;
        }
    }
    }

