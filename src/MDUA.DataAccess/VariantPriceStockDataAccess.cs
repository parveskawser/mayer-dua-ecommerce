using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework;
using MDUA.Framework.Exceptions;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;

namespace MDUA.DataAccess
{
	public partial class VariantPriceStockDataAccess
	{
        public int Insert(VariantPriceStock vps)
        {
            using SqlCommand cmd = GetSPCommand("InsertVariantPriceStock");

            // Add all parameters from the entity
            // Note: The @Id is an INPUT, not an OUTPUT
            AddParameter(cmd, pInt32("Id", vps.Id));
            AddParameter(cmd, pDecimal("Price", vps.Price));
            AddParameter(cmd, pDecimal("CompareAtPrice", vps.CompareAtPrice));
            AddParameter(cmd, pDecimal("CostPrice", vps.CostPrice));
            AddParameter(cmd, pInt32("StockQty", vps.StockQty));
            AddParameter(cmd, pBool("TrackInventory", vps.TrackInventory));
            AddParameter(cmd, pBool("AllowBackorder", vps.AllowBackorder));
            AddParameter(cmd, pInt32("WeightGrams", vps.WeightGrams));

            long result = InsertRecord(cmd);

            return (int)result; 
        }

        public long UpdatePrice(int variantId, decimal price, string sku)
        {
            // We update SKU in ProductVariant and Price in BOTH tables
            string SQLQuery = @"
        UPDATE ProductVariant 
        SET VariantPrice = @Price, SKU = @SKU, UpdatedAt = GETDATE() 
        WHERE Id = @Id;

        UPDATE VariantPriceStock 
        SET Price = @Price 
        WHERE Id = @Id;
        
        SELECT 1;";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("Id", variantId));
                AddParameter(cmd, pDecimal("Price", price));
                AddParameter(cmd, pNVarChar("SKU", 50, sku)); // Add SKU Parameter

                SqlDataReader reader;
                long result = SelectRecords(cmd, out reader);

                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }

                return 1;
            }
        }

        public void AddStock(int variantId, int qty, SqlTransaction transaction)
        {
            // Simply adds to existing stock
            string SQL = @"UPDATE VariantPriceStock SET StockQty = StockQty + @Qty WHERE Id = @Id";
            using (SqlCommand cmd = new SqlCommand(SQL, transaction.Connection, transaction))
            {
                cmd.Parameters.AddWithValue("@Id", variantId);
                cmd.Parameters.AddWithValue("@Qty", qty);
                cmd.ExecuteNonQuery();
            }
        }

        // Helper to find ProductId from Variant (for InventoryTransaction)
        public int GetProductIdByVariant(int variantId)
        {
            string SQL = "SELECT ProductId FROM ProductVariant WHERE Id = @Id";
            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("Id", variantId));
                if (cmd.Connection.State != System.Data.ConnectionState.Open) cmd.Connection.Open();
                var result = cmd.ExecuteScalar();
                cmd.Connection.Close();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }
    }	
}
