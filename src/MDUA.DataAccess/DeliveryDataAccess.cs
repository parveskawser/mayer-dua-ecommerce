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