using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MDUA.Framework;
using MDUA.Entities;
using MDUA.DataAccess.Interface; // ✅ Ensure Interface namespace is included

namespace MDUA.DataAccess
{
    // ✅ FIX: Explicitly add ': IInventoryTransactionDataAccess' here.
    // Since this is a partial class, it merges with your base class and satisfies the Dependency Injection.
    public partial class InventoryTransactionDataAccess : IInventoryTransactionDataAccess
    {
        // ✅ Custom Method for Transactional Insert
        public void InsertInTransaction(int poReceivedId, int variantId, int qty, decimal price, string remarks, SqlTransaction transaction)
        {
            // Use the existing SP
            string spName = "InsertInventoryTransaction";

            using (SqlCommand cmd = new SqlCommand(spName, transaction.Connection, transaction))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Output Param
                SqlParameter outputId = new SqlParameter("@Id", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outputId);

                cmd.Parameters.AddWithValue("@SalesOrderDetailId", DBNull.Value);
                cmd.Parameters.AddWithValue("@PoReceivedId", poReceivedId);

                // ✅ FIXED: Updated parameter name to match your corrected Stored Procedure
                cmd.Parameters.AddWithValue("@ProductVariantId", variantId);

                cmd.Parameters.AddWithValue("@InOut", "IN");
                cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                cmd.Parameters.AddWithValue("@Price", price);
                cmd.Parameters.AddWithValue("@Quantity", qty);
                cmd.Parameters.AddWithValue("@CreatedBy", "System");
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                cmd.Parameters.AddWithValue("@UpdatedBy", DBNull.Value);
                cmd.Parameters.AddWithValue("@UpdatedAt", DBNull.Value);
                cmd.Parameters.AddWithValue("@Remarks", (object)remarks ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }
    }
}