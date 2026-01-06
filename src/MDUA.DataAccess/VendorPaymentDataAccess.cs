using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework.DataAccess;
using System;
using System.Data;
using System.Data.SqlClient;

namespace MDUA.DataAccess
{
    public partial class VendorPaymentDataAccess
    {
        public long InsertPayment(VendorPayment payment, SqlTransaction trans = null)
        {
            const string sql = "InsertVendorPayment";

            // 1. Use GetSPCommand but attach the Transaction if it exists
            using (var cmd = GetSPCommand(sql))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // ✅ CRITICAL: Attach to the existing transaction from ReceiveStock
                if (trans != null)
                {
                    cmd.Connection = trans.Connection;
                    cmd.Transaction = trans;
                }

                var idParam = new SqlParameter("@Id", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(idParam);

                cmd.Parameters.AddWithValue("@VendorId", payment.VendorId);
                cmd.Parameters.AddWithValue("@PaymentMethodId", payment.PaymentMethodId);

                // ✅ LOGIC: Default to 'Purchase' to satisfy DB Check Constraint
                cmd.Parameters.AddWithValue("@PaymentType", payment.PaymentType ?? "Purchase");

                cmd.Parameters.AddWithValue("@Amount", payment.Amount);
                cmd.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
                cmd.Parameters.AddWithValue("@Status", payment.Status ?? "Completed");
                cmd.Parameters.AddWithValue("@CreatedBy", payment.CreatedBy ?? "System");
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                // Optional / Nullable
                cmd.Parameters.AddWithValue("@InventoryTransactionId", (object)payment.InventoryTransactionId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ReferenceNumber", (object)payment.ReferenceNumber ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Notes", (object)payment.Notes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PoReceivedId", (object)payment.PoReceivedId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PoRequestedId", (object)payment.PoRequestedId ?? DBNull.Value);

                // Open connection only if it's not already open by the transaction
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                cmd.ExecuteNonQuery();

                return (int)idParam.Value;
            }
        }
        // ✅ NEW Method implementation
        public List<dynamic> GetPendingBills(int vendorId)
        {
            var list = new List<dynamic>();
            const string sql = "GetUnpaidPoReceivedByVendor";

            using (var cmd = GetSPCommand(sql))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VendorId", vendorId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new
                        {
                            Id = (int)reader["Id"],
                            InvoiceNo = reader["InvoiceNo"].ToString(),
                            BalanceDue = Convert.ToDecimal(reader["BalanceDue"]),
                            ReceivedDate = Convert.ToDateTime(reader["ReceivedDate"]).ToString("dd-MMM-yyyy")
                        });
                    }
                }
            }
            return list;
        }

    }
}