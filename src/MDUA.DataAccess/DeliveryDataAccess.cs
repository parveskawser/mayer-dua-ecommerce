using System;
using System.Data;
using System.Data.SqlClient;
using MDUA.Framework;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.DataAccess.Interface;

namespace MDUA.DataAccess
{
    public partial class DeliveryDataAccess
    {
        // Custom Stored Procedure Names
        private const string SP_INSERT_EXT = "[dbo].[InsertDelivery]";
        private const string SP_UPDATE_EXT = "[dbo].[UpdateDelivery]";
        private const string SP_GET_BY_ORDER_EXT = "[dbo].[GetDeliveryBySalesOrderId]";
        private const string SP_GET_BY_ID_EXT = "[dbo].[GetDeliveryById]";

        #region Extended Methods

        public long InsertExtended(Delivery delivery)
        {
            using (SqlCommand cmd = GetSQLCommand(SP_INSERT_EXT))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Output Parameter
                SqlParameter outParam = new SqlParameter("@Id", SqlDbType.Int);
                outParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outParam);

                AddExtendedParams(cmd, delivery);

                ExecuteCommand(cmd);

                int newId = (int)outParam.Value;
                delivery.Id = newId;
                return newId;
            }
        }

        public void UpdateExtended(Delivery delivery)
        {
            using (SqlCommand cmd = GetSQLCommand(SP_UPDATE_EXT))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = delivery.Id });

                AddExtendedParams(cmd, delivery);

                ExecuteCommand(cmd);
            }
        }

        public Delivery GetBySalesOrderIdExtended(int salesOrderId)
        {
            // DEBUG: Uncomment this line to prove the new code is loaded
            // throw new Exception("DEBUG: I am using the EXTENDED method!");

            using (SqlCommand cmd = GetSQLCommand(SP_GET_BY_ORDER_EXT))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@SalesOrderId", SqlDbType.Int) { Value = salesOrderId });

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Verify we are NOT calling the base FillObject
                        return FillObjectExtended(reader);
                    }
                }
            }
            return null;
        }
        public Delivery GetExtended(int id)
        {
            using (SqlCommand cmd = GetSQLCommand(SP_GET_BY_ID_EXT))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return FillObjectExtended(reader);
                    }
                }
            }
            return null;
        }


        // Inside DeliveryDataAccess.cs

        public long InsertDeliveryItem(int deliveryId, int salesOrderDetailId, int quantity)
        {
            // Use the SP name you provided
            using (SqlCommand cmd = GetSPCommand("[dbo].[InsertDeliveryItem]"))
            {
                // 1. Output Parameter (@Id)
                AddParameter(cmd, pInt32Out("Id"));

                // 2. Input Parameters
                AddParameter(cmd, pInt32("DeliveryId", deliveryId));
                AddParameter(cmd, pInt32("SalesOrderDetailId", salesOrderDetailId));
                AddParameter(cmd, pInt32("Quantity", quantity));

                // 3. Execute
                // InsertRecord handles the execution and connection logic in your framework
                long result = InsertRecord(cmd);

                // 4. Return the new ID (if successful)
                if (result > 0)
                {
                    return (int)GetOutParameter(cmd, "Id");
                }
                return -1;
            }
        }



        // --- 1. Fix for "Does not implement Update" ---
        public int Update(Delivery delivery)
        {
            // Simply call your existing extended logic
            UpdateExtended(delivery);
            return delivery.Id;
        }

        // --- 2. Fix for "Does not implement LoadAllWithDetails" ---
        public System.Collections.Generic.IList<Delivery> LoadAllWithDetails()
        {
            var result = new System.Collections.Generic.List<Delivery>();

            // SQL to fetch Delivery + Order Info + Customer + Items + Product Info
            string sql = @"
        SELECT 
            d.Id, d.SalesOrderId, d.TrackingNumber, d.Status, d.CarrierName, 
            d.ShipDate, d.EstimatedArrival, d.ActualDeliveryDate, d.ShippingCost,
            
            soh.Id AS OrderId, 
            soh.SalesChannelId,
            soh.Status AS OrderStatus,      -- ✅ ADDED: Fetch Order Status
            soh.Confirmed AS OrderConfirmed, -- ✅ ADDED: Fetch Confirmed Bit
            
            c.CustomerName,
            
            di.Id AS ItemId, di.Quantity,
            
            p.ProductName, 
            pv.VariantName, 
            pv.Sku
        FROM Delivery d
        INNER JOIN SalesOrderHeader soh ON d.SalesOrderId = soh.Id
        INNER JOIN CompanyCustomer cc ON soh.CompanyCustomerId = cc.Id
        INNER JOIN Customer c ON cc.CustomerId = c.Id
        LEFT JOIN DeliveryItem di ON d.Id = di.DeliveryId
        LEFT JOIN SalesOrderDetail sod ON di.SalesOrderDetailId = sod.Id
        LEFT JOIN ProductVariant pv ON sod.ProductId = pv.Id 
        LEFT JOIN Product p ON pv.ProductId = p.Id
        ORDER BY d.Id DESC";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                cmd.CommandType = CommandType.Text;

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    var lookup = new System.Collections.Generic.Dictionary<int, Delivery>();

                    while (reader.Read())
                    {
                        int deliveryId = reader.GetInt32(reader.GetOrdinal("Id"));

                        if (!lookup.TryGetValue(deliveryId, out Delivery delivery))
                        {
                            delivery = new Delivery
                            {
                                Id = deliveryId,
                                SalesOrderId = reader.GetInt32(reader.GetOrdinal("SalesOrderId")), // Added this missing map
                                TrackingNumber = reader.IsDBNull(reader.GetOrdinal("TrackingNumber")) ? null : reader.GetString(reader.GetOrdinal("TrackingNumber")),
                                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? "Pending" : reader.GetString(reader.GetOrdinal("Status")),
                                CarrierName = reader.IsDBNull(reader.GetOrdinal("CarrierName")) ? null : reader.GetString(reader.GetOrdinal("CarrierName")),
                                ShipDate = reader.IsDBNull(reader.GetOrdinal("ShipDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ShipDate")),
                                EstimatedArrival = reader.IsDBNull(reader.GetOrdinal("EstimatedArrival")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EstimatedArrival")),
                                ActualDeliveryDate = reader.IsDBNull(reader.GetOrdinal("ActualDeliveryDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ActualDeliveryDate")),
                                ShippingCost = reader.IsDBNull(reader.GetOrdinal("ShippingCost")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("ShippingCost")),

                                DeliveryItems = new System.Collections.Generic.List<DeliveryItem>(),

                                SalesOrderHeader = new SalesOrderHeader
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("OrderId")),

                                    // ✅ MAPPING FIX: Map the new columns
                                    Status = reader.IsDBNull(reader.GetOrdinal("OrderStatus")) ? "" : reader.GetString(reader.GetOrdinal("OrderStatus")),
                                    Confirmed = !reader.IsDBNull(reader.GetOrdinal("OrderConfirmed")) && reader.GetBoolean(reader.GetOrdinal("OrderConfirmed")),

                                    CompanyCustomer = new CompanyCustomer
                                    {
                                        Customer = new Customer
                                        {
                                            CustomerName = reader.GetString(reader.GetOrdinal("CustomerName"))
                                        }
                                    }
                                }
                            };

                            int channelId = reader.GetInt32(reader.GetOrdinal("SalesChannelId"));
                            if (channelId == 1)
                                delivery.SalesOrderHeader.OnlineOrderId = "ON" + delivery.SalesOrderHeader.Id.ToString().PadLeft(8, '0');
                            else
                                delivery.SalesOrderHeader.DirectOrderId = "DO" + delivery.SalesOrderHeader.Id.ToString().PadLeft(8, '0');

                            lookup.Add(deliveryId, delivery);
                            result.Add(delivery);
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("ItemId")))
                        {
                            var item = new DeliveryItem
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ItemId")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                SalesOrderDetail = new SalesOrderDetail
                                {
                                    ProductVariant = new ProductVariant
                                    {
                                        VariantName = reader.IsDBNull(reader.GetOrdinal("VariantName")) ? "" : reader.GetString(reader.GetOrdinal("VariantName")),
                                        SKU = reader.IsDBNull(reader.GetOrdinal("Sku")) ? "" : reader.GetString(reader.GetOrdinal("Sku")),
                                        Product = new Product
                                        {
                                            ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? "Unknown" : reader.GetString(reader.GetOrdinal("ProductName"))
                                        }
                                    }
                                }
                            };
                            delivery.DeliveryItems.Add(item);
                        }
                    }
                }
            }
            return result;
        }


        #endregion

        #region Private Helpers

        private void AddExtendedParams(SqlCommand cmd, Delivery obj)
        {
            // Use explicit SqlDbType to handle DBNull correctly
            cmd.Parameters.Add(new SqlParameter("@SalesOrderId", SqlDbType.Int) { Value = obj.SalesOrderId });

            cmd.Parameters.Add(new SqlParameter("@TrackingNumber", SqlDbType.NVarChar, 100) { Value = (object)obj.TrackingNumber ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.NVarChar, 50) { Value = (object)obj.Status ?? "Pending" });
            cmd.Parameters.Add(new SqlParameter("@CarrierName", SqlDbType.NVarChar, 100) { Value = (object)obj.CarrierName ?? DBNull.Value });

            // Dates
            cmd.Parameters.Add(new SqlParameter("@ShipDate", SqlDbType.DateTime) { Value = obj.ShipDate.HasValue ? (object)obj.ShipDate.Value : DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@EstimatedArrival", SqlDbType.DateTime) { Value = obj.EstimatedArrival.HasValue ? (object)obj.EstimatedArrival.Value : DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ActualDeliveryDate", SqlDbType.DateTime) { Value = obj.ActualDeliveryDate.HasValue ? (object)obj.ActualDeliveryDate.Value : DBNull.Value });

            // Cost
            cmd.Parameters.Add(new SqlParameter("@ShippingCost", SqlDbType.Decimal) { Value = obj.ShippingCost.HasValue ? (object)obj.ShippingCost.Value : DBNull.Value });

            // Audit
            cmd.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100) { Value = (object)obj.CreatedBy ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CreatedAt", SqlDbType.DateTime) { Value = obj.CreatedAt == DateTime.MinValue ? DateTime.UtcNow : obj.CreatedAt });
            cmd.Parameters.Add(new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = (object)obj.UpdatedBy ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@UpdatedAt", SqlDbType.DateTime) { Value = obj.UpdatedAt.HasValue ? (object)obj.UpdatedAt.Value : DBNull.Value });
        }

        private Delivery FillObjectExtended(SqlDataReader reader)
        {
            Delivery obj = new Delivery();

            // ✅ SAFE MAPPING: Uses Column Name (GetOrdinal), NOT Index
            obj.Id = reader.GetInt32(reader.GetOrdinal("Id"));
            obj.SalesOrderId = reader.GetInt32(reader.GetOrdinal("SalesOrderId"));

            int idxTracking = reader.GetOrdinal("TrackingNumber");
            if (!reader.IsDBNull(idxTracking)) obj.TrackingNumber = reader.GetString(idxTracking);

            int idxStatus = reader.GetOrdinal("Status");
            if (!reader.IsDBNull(idxStatus)) obj.Status = reader.GetString(idxStatus);

            int idxCarrier = reader.GetOrdinal("CarrierName");
            if (!reader.IsDBNull(idxCarrier)) obj.CarrierName = reader.GetString(idxCarrier);

            int idxShipDate = reader.GetOrdinal("ShipDate");
            if (!reader.IsDBNull(idxShipDate)) obj.ShipDate = reader.GetDateTime(idxShipDate);

            int idxEstArrival = reader.GetOrdinal("EstimatedArrival");
            if (!reader.IsDBNull(idxEstArrival)) obj.EstimatedArrival = reader.GetDateTime(idxEstArrival);

            int idxActualDelivery = reader.GetOrdinal("ActualDeliveryDate");
            if (!reader.IsDBNull(idxActualDelivery)) obj.ActualDeliveryDate = reader.GetDateTime(idxActualDelivery);

            int idxCost = reader.GetOrdinal("ShippingCost");
            if (!reader.IsDBNull(idxCost)) obj.ShippingCost = reader.GetDecimal(idxCost);
            else obj.ShippingCost = 0;

            int idxCreatedBy = reader.GetOrdinal("CreatedBy");
            if (!reader.IsDBNull(idxCreatedBy)) obj.CreatedBy = reader.GetString(idxCreatedBy);

            obj.CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"));

            int idxUpdatedBy = reader.GetOrdinal("UpdatedBy");
            if (!reader.IsDBNull(idxUpdatedBy)) obj.UpdatedBy = reader.GetString(idxUpdatedBy);

            int idxUpdatedAt = reader.GetOrdinal("UpdatedAt");
            if (!reader.IsDBNull(idxUpdatedAt)) obj.UpdatedAt = reader.GetDateTime(idxUpdatedAt);

            obj.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
            return obj;
        }

        #endregion
    }
}