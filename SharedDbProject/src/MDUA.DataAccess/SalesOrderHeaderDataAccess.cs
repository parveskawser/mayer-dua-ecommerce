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


        //new
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
    }	
}
