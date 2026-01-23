using MDUA.Framework;
using MDUA.Framework.DataAccess;
using MDUA.Framework.Exceptions;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;

namespace MDUA.DataAccess
{
    public partial class PoReceivedDataAccess
    {
        /// <summary>
        /// Inserts a PO Receipt. 
        /// Updated to support Payment Tracking (TotalPaid / PaymentStatus).
        /// </summary>
        public int Insert(int poReqId, int qty, decimal price, string invoice, string remarks, SqlTransaction transaction, decimal totalPaid = 0, string paymentStatus = "Unpaid")
        {
            string spName = "InsertPoReceived";

            // Use the connection associated with the transaction
            using (SqlCommand cmd = new SqlCommand(spName, transaction.Connection, transaction))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter outputId = new SqlParameter("@Id", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outputId);

                // Standard Fields
                cmd.Parameters.AddWithValue("@PoRequestedId", poReqId);
                cmd.Parameters.AddWithValue("@ReceivedQuantity", qty);
                cmd.Parameters.AddWithValue("@BuyingPrice", price);
                cmd.Parameters.AddWithValue("@ReceivedDate", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@CreatedBy", "System"); 
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@UpdatedBy", DBNull.Value);
                cmd.Parameters.AddWithValue("@UpdatedAt", DBNull.Value); 
                cmd.Parameters.AddWithValue("@Remarks", (object)remarks ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@InvoiceNo", (object)invoice ?? DBNull.Value);

               
                // Note: TotalPaymentDue is NOT passed; SQL calculates it automatically.
                cmd.Parameters.AddWithValue("@TotalPaid", totalPaid);
                cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
                // REASON: The SQL Stored Procedure now automatically looks up the VendorId 
                // from the PoRequested table based on @PoRequestedId. 
                // We do not need to pass it from C#.
                // cmd.Parameters.AddWithValue("@VendorId", DBNull.Value); 
                cmd.ExecuteNonQuery();

                return (int)outputId.Value;
            }
        }

        public void ReceiveBulkStock(List<dynamic> items, string invoice, string remarks, decimal totalPaid, int? paymentMethodId, int vendorId, string username)
        {
            // Serialize items to JSON
            string jsonItems = JsonSerializer.Serialize(items);

            try
            {
                using (SqlCommand cmd = GetSPCommand("ReceiveBulkOrderItems"))
                {
                    AddParameter(cmd, pNVarChar("JsonItems", -1, jsonItems)); // -1 = MAX
                    AddParameter(cmd, pNVarChar("InvoiceNo", 100, invoice));
                    AddParameter(cmd, pNVarChar("Remarks", 500, remarks));

                    AddParameter(cmd, pDecimal("TotalPaidAmount", 18, totalPaid));

                    if (paymentMethodId.HasValue)
                        AddParameter(cmd, pInt32("PaymentMethodId", paymentMethodId.Value));
                    else
                        AddParameter(cmd, new SqlParameter("@PaymentMethodId", DBNull.Value));

                    AddParameter(cmd, pInt32("VendorId", vendorId));
                    AddParameter(cmd, pNVarChar("CreatedBy", 100, username));

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                // Guardrail: Catch the "Double Receive" error from SQL (Error 50001)
                if (ex.Number == 50001)
                {
                    throw new WorkflowException(ex.Message);
                }
                throw;
            }
        }

        // Inside PoReceivedDataAccess class

        public int? GetBulkOrderIdByInvoice(string invoice, int vendorId)
        {
            // Returns:
            // NULL = Invoice doesn't exist for this vendor (Safe to use)
            // 0 = Invoice exists on a Standard Order (No Bulk ID)
            // >0 = Invoice exists on a specific Bulk Order

            string sql = @"
        SELECT TOP 1 pr.BulkPurchaseOrderId 
        FROM PoReceived r
        JOIN PoRequested pr ON r.PoRequestedId = pr.Id
        WHERE r.InvoiceNo = @Invoice AND r.VendorId = @VendorId";

            using (var cmd = GetSQLCommand(sql))
            {
                AddParameter(cmd, pNVarChar("Invoice", 100, invoice));
                AddParameter(cmd, pInt32("VendorId", vendorId));

                if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();

                var result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value) return null; // Invoice new/unused

                return Convert.ToInt32(result); // Returns BulkId (or 0 if standard)
            }
        }
    }
}