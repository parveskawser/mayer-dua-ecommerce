using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace MDUA.DataAccess
{
    public partial class ProductVariantDataAccess : BaseDataAccess, IProductVariantDataAccess
        {
        /// <summary>
        /// Inline SQL fallback for GetProductVariantsByProductId
        /// Joins VariantPriceStock (vps) on v.Id = vps.Id to get stock (StockQty) and price if needed.
        /// </summary>
        public ProductVariantList GetProductVariantsByProductId(int productId)
        {
            string SQLQuery = @"
				SELECT 
					v.Id,
					v.ProductId,
					v.VariantName,
					v.SKU,
					v.Barcode,
					v.VariantPrice,
					v.IsActive,
					v.CreatedBy,
					v.CreatedAt,
					v.UpdatedBy,
					v.UpdatedAt,
					ISNULL(vps.StockQty, 0) AS StockQty,
					ISNULL(vps.Price, 0) AS VPS_Price
				FROM ProductVariant v
				LEFT JOIN VariantPriceStock vps ON v.Id = vps.Id
				WHERE v.ProductId = @ProductId
				ORDER BY v.Id";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            AddParameter(cmd, pInt32("ProductId", productId));

            SqlDataReader reader;
            long result = SelectRecords(cmd, out reader);
            ProductVariantList list = new ProductVariantList();

            using (reader)
            {
                while (reader.Read())
                {
                    var variant = new ProductVariant
                    {
                        Id = reader.GetInt32(0),
                        ProductId = reader.GetInt32(1),
                        VariantName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        SKU = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        Barcode = reader.IsDBNull(4) ? "" : reader.GetString(4),
                        VariantPrice = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5),
                        IsActive = reader.IsDBNull(6) ? true : reader.GetBoolean(6),
                        CreatedBy = reader.IsDBNull(7) ? "" : reader.GetString(7),
                        CreatedAt = reader.IsDBNull(8) ? DateTime.Now : reader.GetDateTime(8),
                        UpdatedBy = reader.IsDBNull(9) ? "" : reader.GetString(9),
                        UpdatedAt = reader.IsDBNull(10) ? (DateTime?)null : reader.GetDateTime(10),
                        // stock and optional price from VariantPriceStock
                        StockQty = reader.IsDBNull(11) ? 0 : reader.GetInt32(11)
                        // If you want to map VPS_Price, add a property to ProductVariant or ignore it here
                    };

                    list.Add(variant);
                }
                reader.Close();
            }

            return list;
        }
        public int Insert(ProductVariant variant)
        {
            using SqlCommand cmd = GetSPCommand("InsertProductVariant");

            // OUTPUT
            AddParameter(cmd, pInt32Out(ProductVariantBase.Property_Id));

            // COMMON PARAMS
            AddParameter(cmd, pInt32(ProductVariantBase.Property_ProductId, variant.ProductId));
            AddParameter(cmd, pNVarChar(ProductVariantBase.Property_VariantName, 150, variant.VariantName));
            AddParameter(cmd, pNVarChar(ProductVariantBase.Property_SKU, 50, variant.SKU));
            AddParameter(cmd, pNVarChar(ProductVariantBase.Property_Barcode, 100, variant.Barcode));
            AddParameter(cmd, pDecimal(ProductVariantBase.Property_VariantPrice, variant.VariantPrice));
            AddParameter(cmd, pBool(ProductVariantBase.Property_IsActive, true));
            AddParameter(cmd, pNVarChar(ProductVariantBase.Property_CreatedBy, 100, variant.CreatedBy));
            AddParameter(cmd, pDateTime(ProductVariantBase.Property_CreatedAt, variant.CreatedAt));
            AddParameter(cmd, pNVarChar(ProductVariantBase.Property_UpdatedBy, 100, null));
            AddParameter(cmd, pDateTime(ProductVariantBase.Property_UpdatedAt, null));

            // IMPORTANT: MDUA uses InsertRecord()
            long result = InsertRecord(cmd);

            if (result > 0)
                return (int)GetOutParameter(cmd, ProductVariantBase.Property_Id);

            return 0;
        }


        public void InsertVariantAttributeValue(int variantId, int attributeValueId, int displayOrder)
        {
            string SQLQuery = @"
        INSERT INTO VariantAttributeValue (VariantId, AttributeId, AttributeValueId, DisplayOrder)
        SELECT @VariantId, AttributeId, Id, @DisplayOrder
        FROM AttributeValue
        WHERE Id = @AttributeValueId";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);

            AddParameter(cmd, pInt32("VariantId", variantId));
            AddParameter(cmd, pInt32("AttributeValueId", attributeValueId));
            AddParameter(cmd, pInt32("DisplayOrder", displayOrder));

            SqlDataReader reader;
            // This executes the INSERT
            SelectRecords(cmd, out reader);

            reader.Close();
            reader.Dispose();
        }

        public void UpdateVariantName(int variantId, string newName)
        {
            string SQLQuery = "UPDATE ProductVariant SET VariantName = @Name WHERE Id = @Id";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("Id", variantId));
                AddParameter(cmd, pNVarChar("Name", 150, newName));

                UpdateRecord(cmd);
            }
        }


        public ProductVariant GetWithStock(int id)
        {
            // We use inline SQL here to join the tables and get the stock
            string SQLQuery = @"
        SELECT 
            v.Id,
            v.ProductId,
            v.VariantName,
            v.SKU,
            v.Barcode,
            v.VariantPrice,
            v.IsActive,
            v.CreatedBy,
            v.CreatedAt,
            v.UpdatedBy,
            v.UpdatedAt,
            ISNULL(vps.StockQty, 0) AS StockQty,  -- Get Stock
            ISNULL(vps.Price, 0) AS VPS_Price
        FROM ProductVariant v
        LEFT JOIN VariantPriceStock vps ON v.Id = vps.Id
        WHERE v.Id = @Id";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            AddParameter(cmd, pInt32("Id", id));

            SqlDataReader reader;
            SelectRecords(cmd, out reader);

            ProductVariant variant = null;

            using (reader)
            {
                if (reader.Read())
                {
                    // Manual mapping is required because the default FillObject() 
                    // doesn't know about the joined columns
                    variant = new ProductVariant
                    {
                        Id = reader.GetInt32(0),
                        ProductId = reader.GetInt32(1),
                        VariantName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        SKU = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        Barcode = reader.IsDBNull(4) ? "" : reader.GetString(4),
                        VariantPrice = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5),
                        IsActive = reader.IsDBNull(6) ? true : reader.GetBoolean(6),
                        CreatedBy = reader.IsDBNull(7) ? "" : reader.GetString(7),
                        CreatedAt = reader.IsDBNull(8) ? DateTime.Now : reader.GetDateTime(8),
                        UpdatedBy = reader.IsDBNull(9) ? "" : reader.GetString(9),
                        UpdatedAt = reader.IsDBNull(10) ? (DateTime?)null : reader.GetDateTime(10),

                        // ✅ THIS IS THE FIX: Read the stock column
                        StockQty = reader.IsDBNull(11) ? 0 : reader.GetInt32(11)
                    };
                }
                reader.Close();
            }

            return variant;
        }
    }
}

