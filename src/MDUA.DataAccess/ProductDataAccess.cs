
using MDUA.Entities;
using MDUA.Entities.List;
using System;
using System.Data.SqlClient;

namespace MDUA.DataAccess
{
    public partial class ProductDataAccess
    {

        public Product GetBySlug(string _Slug)
        {
            string SQLQuery = @"
        SELECT * 
        FROM Product 
        WHERE Slug = @Slug 
          AND IsActive = 1";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            AddParameter(cmd, pNVarChar("Slug", 400, _Slug));

            return GetObject(cmd);
        }


        public Product GetProductById(Int32 _Id)
        {
            string SQLQuery = @"
        SELECT 
            p.Id,
            p.CompanyId,
            p.ProductName,
            p.ReorderLevel,
            p.Barcode,
            p.CategoryId,
            p.Description,
            p.Slug,
            p.BasePrice,
            p.IsVariantBased,
            p.IsActive,
            p.CreatedBy,
            p.CreatedAt,
            p.UpdatedBy,
            p.UpdatedAt,
            c.CompanyName,
            ISNULL(SUM(vps.StockQty), 0) AS TotalStockQuantity
        FROM 
            Product p
        LEFT JOIN 
            Company c ON p.CompanyId = c.Id
        LEFT JOIN 
            ProductVariant pv ON pv.ProductId = p.Id
        LEFT JOIN 
            VariantPriceStock vps ON vps.Id = pv.Id
        WHERE 
            p.Id = @Id
        GROUP BY 
            p.Id, p.CompanyId, p.ProductName, p.ReorderLevel, p.Barcode, 
            p.CategoryId, p.Description, p.Slug, p.BasePrice, p.IsVariantBased, 
            p.IsActive, p.CreatedBy, p.CreatedAt, p.UpdatedBy, p.UpdatedAt, c.CompanyName;
    ";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            AddParameter(cmd, pInt32("Id", _Id));

            // 1. Get the object as usual (Dates will come as 'Unspecified')
            Product product = GetObject(cmd);

            // 2. ✅ FIX: Manually force the DateTimeKind to UTC
            // This tells .NET: "These dates are definitely UTC, not local."
            if (product != null)
            {
                if (product.CreatedAt.HasValue)
                    product.CreatedAt = DateTime.SpecifyKind(product.CreatedAt.Value, DateTimeKind.Utc);

                if (product.UpdatedAt.HasValue)
                    product.UpdatedAt = DateTime.SpecifyKind(product.UpdatedAt.Value, DateTimeKind.Utc);
            }

            return product;
        }
        public ProductList GetLastFiveProducts()
        {
            string SQLQuery = @"
        SELECT TOP 5
            Id,
            CompanyId,
            ProductName,
            ReorderLevel,
            Barcode,
            CategoryId,
            Description,
            Slug,
            BasePrice,
            IsVariantBased,
            IsActive,
            CreatedBy,
            CreatedAt,
            UpdatedBy,
            UpdatedAt
        FROM Product
        WHERE IsActive = 1
        ORDER BY CreatedAt DESC";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);

            return GetList(cmd, 5);
        }

        public bool? ToggleStatus(int productId)
        {
            // --- QUERY 1: UPDATE the product ---
            string SQLQueryUpdate = @"
        UPDATE Product
        SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END
        WHERE Id = @Id;";

            using (SqlCommand cmdUpdate = GetSQLCommand(SQLQueryUpdate))
            {
                AddParameter(cmdUpdate, pInt32("Id", productId));

                // ✅ Use the SelectRecords pattern to execute the UPDATE
                // This matches the pattern in your InsertVariantAttributeValue method
                SqlDataReader readerUpdate;
                SelectRecords(cmdUpdate, out readerUpdate);

                // We must close this reader immediately
                readerUpdate.Close();
                readerUpdate.Dispose();
            }

            // --- QUERY 2: SELECT the new status ---
            string SQLQuerySelect = @"
        SELECT IsActive 
        FROM Product
        WHERE Id = @Id;";

            bool? newStatus = null;
            using (SqlCommand cmdSelect = GetSQLCommand(SQLQuerySelect))
            {
                AddParameter(cmdSelect, pInt32("Id", productId));

                // Now we use the same SelectRecords pattern for the SELECT
                SqlDataReader readerSelect;
                SelectRecords(cmdSelect, out readerSelect);

                using (readerSelect)
                {
                    if (readerSelect.Read())
                    {
                        if (readerSelect[0] != null && readerSelect[0] != DBNull.Value)
                        {
                            newStatus = (bool)readerSelect[0];
                        }
                    }
                    readerSelect.Close();
                }
            }

            // This will return the new status, or null if the product wasn't found
            return newStatus;
        }

        // 1. GET FULL LIST
        public ProductList GetAllProductsWithCategory()
        {
            string SQLQuery = @"
                SELECT TOP 100 
                    p.Id, p.CompanyId, p.ProductName, p.ReorderLevel, p.Barcode,
                    p.CategoryId, p.Description, p.Slug, p.BasePrice, p.IsVariantBased,
                    p.IsActive, p.CreatedBy, p.CreatedAt, p.UpdatedBy, p.UpdatedAt,
                    NULL as DummyForBase, -- Index 15: Buffer for FillBaseObject
                    ISNULL(c.Name, '') as CategoryName -- Index 16: Actual Data
                FROM Product p
                LEFT JOIN ProductCategory c ON p.CategoryId = c.Id
                ORDER BY p.CreatedAt DESC";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            return GetListWithCategory(cmd);
        }
        private ProductList GetListWithCategory(SqlCommand cmd)
        {
            ProductList list = new ProductList();
            SqlDataReader reader;

            SelectRecords(cmd, out reader);

            using (reader)
            {
                while (reader.Read())
                {
                    Product product = new Product();

                    // 1. Fill standard properties (Indices 0-14, and potentially 15 via Base)
                    FillObject(product, reader);

                    // 2. Explicitly read Index 16 for CategoryName
                    if (reader.FieldCount > 16 && !reader.IsDBNull(16))
                    {
                        product.CategoryName = reader.GetString(16);
                    }
                    else
                    {
                        // Use empty string so Facade can set "N/A" if needed
                        // or leave as is if you want it blank
                        product.CategoryName = "";
                    }

                    list.Add(product);
                }
                reader.Close();
            }
            return list;
        }
        // 2. SEARCH PRODUCTS
        public ProductList SearchProducts(string searchTerm)
        {
            string SQLQuery = @"
                SELECT TOP 50
                    p.Id, p.CompanyId, p.ProductName, p.ReorderLevel, p.Barcode,
                    p.CategoryId, p.Description, p.Slug, p.BasePrice, p.IsVariantBased,
                    p.IsActive, p.CreatedBy, p.CreatedAt, p.UpdatedBy, p.UpdatedAt,
                    NULL as DummyForBase, -- Index 15: Buffer
                    ISNULL(c.Name, '') as CategoryName -- Index 16: Data
                FROM Product p
                LEFT JOIN ProductCategory c ON p.CategoryId = c.Id
                WHERE p.IsActive = 1 
                  AND p.ProductName LIKE @Search
                ORDER BY p.ProductName ASC";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            AddParameter(cmd, pNVarChar("Search", 400, $"%{searchTerm}%"));

            return GetListWithCategory(cmd);
        }
    }
}