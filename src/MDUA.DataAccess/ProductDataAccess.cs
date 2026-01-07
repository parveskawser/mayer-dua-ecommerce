
using MDUA.Entities;
using MDUA.Entities.List;
using System;
using System.Data;
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
            p.Id, p.CompanyId, p.ProductName, p.ReorderLevel, p.Barcode,
            p.CategoryId, p.Description, p.Slug, p.BasePrice, p.IsVariantBased,
            p.IsActive, p.CreatedBy, p.CreatedAt, p.UpdatedBy, p.UpdatedAt,
            c.CompanyName,
            ISNULL(SUM(vps.StockQty), 0) AS TotalStockQuantity
        FROM Product p
        LEFT JOIN Company c ON p.CompanyId = c.Id
        LEFT JOIN ProductVariant pv ON pv.ProductId = p.Id
        LEFT JOIN VariantPriceStock vps ON vps.Id = pv.Id
        WHERE p.Id = @Id
        GROUP BY 
            p.Id, p.CompanyId, p.ProductName, p.ReorderLevel, p.Barcode, 
            p.CategoryId, p.Description, p.Slug, p.BasePrice, p.IsVariantBased, 
            p.IsActive, p.CreatedBy, p.CreatedAt, p.UpdatedBy, p.UpdatedAt, c.CompanyName;";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            AddParameter(cmd, pInt32("Id", _Id));

            // ✅ FIX: Ensure the connection is open for async/AI service calls
            if (cmd.Connection.State != System.Data.ConnectionState.Open)
                cmd.Connection.Open();

            // 1. Get the object
            Product product = GetObject(cmd);

            // 2. Force the DateTimeKind to UTC
            if (product != null)
            {
                if (product.CreatedAt.HasValue)
                    product.CreatedAt = DateTime.SpecifyKind(product.CreatedAt.Value, DateTimeKind.Utc);

                if (product.UpdatedAt.HasValue)
                    product.UpdatedAt = DateTime.SpecifyKind(product.UpdatedAt.Value, DateTimeKind.Utc);
            }

            return product;
        }
        // MDUA.DataAccess/ProductDataAccess.cs

        public ProductList GetLastFiveProducts(int companyId) // ✅ Added Parameter
        {
            // ✅ Added "AND CompanyId = @CompanyId"
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
            UpdatedAt,
            ExtraInfo 
        FROM Product
        WHERE IsActive = 1 
          AND CompanyId = @CompanyId -- ✅ FILTER
        ORDER BY CreatedAt DESC";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                // ✅ Add Parameter
                AddParameter(cmd, pInt32("CompanyId", companyId));

                return GetList(cmd, 5);
            }
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
        // Add int companyId parameter
        public ProductList GetAllProductsWithCategory(int companyId)
        {
            // ✅ ADDED: WHERE p.CompanyId = @CompanyId
            string SQLQuery = @"
        SELECT TOP 100 
            p.Id, p.CompanyId, p.ProductName, p.ReorderLevel, p.Barcode,
            p.CategoryId, p.Description, p.Slug, p.BasePrice, p.IsVariantBased,
            p.IsActive, p.CreatedBy, p.CreatedAt, p.UpdatedBy, p.UpdatedAt,
            NULL as DummyForBase, 
            ISNULL(c.Name, '') as CategoryName 
        FROM Product p
        LEFT JOIN ProductCategory c ON p.CategoryId = c.Id
        WHERE p.CompanyId = @CompanyId  -- <--- CRITICAL FIX
        ORDER BY p.CreatedAt DESC";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);

            // ✅ Add the parameter
            AddParameter(cmd, pInt32("CompanyId", companyId));

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
        // Add int companyId parameter
        public ProductList SearchProducts(string searchTerm, int companyId)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return new ProductList();

            var keywords = searchTerm.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (keywords.Length == 0) return new ProductList();

            var sqlBuilder = new System.Text.StringBuilder();

            // ✅ ADDED: p.CompanyId = @CompanyId inside the WHERE clause
            sqlBuilder.Append(@"
    SELECT TOP 50
        p.Id, p.CompanyId, p.ProductName, p.ReorderLevel, p.Barcode,
        p.CategoryId, p.Description, p.Slug, p.BasePrice, p.IsVariantBased,
        p.IsActive, p.CreatedBy, p.CreatedAt, p.UpdatedBy, p.UpdatedAt,
        NULL as DummyForBase,
        ISNULL(c.Name, '') as CategoryName
    FROM Product p
    LEFT JOIN ProductCategory c ON p.CategoryId = c.Id
    WHERE p.CompanyId = @CompanyId  -- <--- CRITICAL FIX
      AND p.IsActive = 1 
      AND (
        (");

            // Loop for ProductName logic
            for (int i = 0; i < keywords.Length; i++)
            {
                if (i > 0) sqlBuilder.Append(" AND ");
                sqlBuilder.Append($"p.ProductName LIKE @Word{i}");
            }

            sqlBuilder.Append(@") 
        OR p.Id IN (
            SELECT ProductId 
            FROM ProductVariant pv 
            WHERE ");

            // Loop for Variant logic
            for (int i = 0; i < keywords.Length; i++)
            {
                if (i > 0) sqlBuilder.Append(" AND ");
                sqlBuilder.Append($"(pv.VariantName LIKE @Word{i} OR pv.SKU LIKE @Word{i})");
            }

            sqlBuilder.Append(@")
      )
    ORDER BY p.ProductName ASC");

            using SqlCommand cmd = GetSQLCommand(sqlBuilder.ToString());

            // ✅ Add CompanyId Parameter
            AddParameter(cmd, pInt32("CompanyId", companyId));

            // Add Search Parameters
            for (int i = 0; i < keywords.Length; i++)
            {
                AddParameter(cmd, pNVarChar($"Word{i}", 100, $"%{keywords[i]}%"));
            }

            return GetListWithCategory(cmd);
        }

        public ProductList GetRecentProductsWithImages(int count)
        {
            // Fetches the latest active products
            string SQLQuery = $@"
        SELECT TOP ({count}) 
            p.Id, p.CompanyId, p.ProductName, p.ReorderLevel, p.Barcode,
            p.CategoryId, p.Description, p.Slug, p.BasePrice, p.IsVariantBased,
            p.IsActive, p.CreatedBy, p.CreatedAt, p.UpdatedBy, p.UpdatedAt,
            NULL as DummyForBase,
            ISNULL(c.Name, '') as CategoryName
        FROM Product p
        LEFT JOIN ProductCategory c ON p.CategoryId = c.Id
        WHERE p.IsActive = 1
        ORDER BY p.CreatedAt DESC";

            using SqlCommand cmd = GetSQLCommand(SQLQuery);
            return GetListWithCategory(cmd); // Reuse your existing private helper
        }

        private const string SP_GET_PRODUCT_COUNT = "sp_GetProductCountByCompany";

        public int GetProductCount(int companyId)
        {
            using (SqlCommand cmd = GetSPCommand(SP_GET_PRODUCT_COUNT))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));

                // Manually manage connection if base helper doesn't auto-open for Scalars
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                object result = cmd.ExecuteScalar();

                return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
        }
    }
}