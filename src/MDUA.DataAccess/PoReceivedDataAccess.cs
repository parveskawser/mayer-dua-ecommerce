using MDUA.Framework;
using MDUA.Framework.DataAccess;
using System;
using System.Data;
using System.Data.SqlClient;

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
    }
}