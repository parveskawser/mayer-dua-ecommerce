using MDUA.Framework;
using MDUA.Framework.DataAccess;
using System;
using System.Data;
using System.Data.SqlClient;

namespace MDUA.DataAccess
{
    public partial class PoReceivedDataAccess
    {

        public int Insert(int poReqId, int qty, decimal price, string invoice, string remarks, SqlTransaction transaction)
        {
            string spName = "InsertPoReceived";

            using (SqlCommand cmd = new SqlCommand(spName, transaction.Connection, transaction))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter outputId = new SqlParameter("@Id", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outputId);

                // Match params to table columns/SP
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

                cmd.ExecuteNonQuery();

                return (int)outputId.Value;
            }
        }
    }
}