using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MDUA.Framework;
using MDUA.Entities;
using MDUA.DataAccess.Interface; // ✅ Ensure Interface namespace is included

namespace MDUA.DataAccess
{
    public partial class InventoryTransactionDataAccess : IInventoryTransactionDataAccess
    {
        public void InsertInTransaction(int poReceivedId, int variantId, int qty, decimal price, string remarks, SqlTransaction transaction)
        {
            string spName = "InsertInventoryTransaction";
            using (SqlCommand cmd = new SqlCommand(spName, transaction.Connection, transaction))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter outputId = new SqlParameter("@Id", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outputId);

                cmd.Parameters.Add("@SalesOrderDetailId", SqlDbType.Int).Value = DBNull.Value;

                // the FK that was crashing. With SCOPE_IDENTITY() fixed, this will now work.
                cmd.Parameters.AddWithValue("@PoReceivedId", poReceivedId);

                cmd.Parameters.AddWithValue("@ProductVariantId", variantId);
                cmd.Parameters.AddWithValue("@InOut", "IN");
                cmd.Parameters.AddWithValue("@Date", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@Price", price);
                cmd.Parameters.AddWithValue("@Quantity", qty);
                cmd.Parameters.AddWithValue("@CreatedBy", "System");
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@UpdatedBy", DBNull.Value);
                cmd.Parameters.AddWithValue("@UpdatedAt", DBNull.Value);
                cmd.Parameters.AddWithValue("@Remarks", (object)remarks ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }
    }
}