using System;
using System.Data;
using System.Data.SqlClient;

using MDUA.Framework;
using MDUA.Framework.Exceptions;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess
{
	public partial class ProductCategoryDataAccess
	{
        // MDUA.DataAccess/ProductCategoryDataAccess.cs

        public List<ProductCategory> GetByCompany(int companyId)
        {
            // ✅ SOFT FILTER: Matches specific Company ID OR NULL (Shared Data)
            string SQL = @"
        SELECT * FROM ProductCategory 
        WHERE (CompanyId = @CompanyId OR CompanyId IS NULL) 
        ORDER BY Name";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }
        public List<ProductCategory> GetAvailableCategories(int companyId)
        {
            // This SQL logic mirrors your Attribute example:
            // 1. Fetch my Active Private categories.
            // 2. Fetch Active Global categories, BUT hide them if I have a Private version (Active or Inactive).

            string SQL = @"
    -- 1. My ACTIVE Private Categories
    SELECT * FROM ProductCategory 
    WHERE IsActive = 1 
    AND CompanyId = @CompanyId

    UNION ALL

    -- 2. Global ACTIVE Categories (Only if NOT overridden by ANY Private category)
    SELECT * FROM ProductCategory 
    WHERE IsActive = 1 
    AND CompanyId IS NULL
    AND LTRIM(RTRIM(Name)) NOT IN (
        -- If I have a private version (Active OR Inactive), 
        -- it overrides the Global one, so do not show the Global one.
        SELECT LTRIM(RTRIM(Name)) 
        FROM ProductCategory 
        WHERE CompanyId = @CompanyId
    )
    ORDER BY Name";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }
        /// <summary>
        /// Checks if a company already has a private version of a category name.
        /// </summary>
        ///
        ///
        // MDUA.DataAccess/ProductCategoryDataAccess.cs

        /// <summary>
        /// Gets ALL categories (Active & Inactive) for the Management Screen.
        /// </summary>
        public List<ProductCategory> GetAllCategoriesForManagement(int companyId)
        {
            string SQL = @"
    -- 1. My Private Categories (Shows Active AND Inactive)
    SELECT * FROM ProductCategory 
    WHERE CompanyId = @CompanyId

    UNION ALL

    -- 2. Global Categories (NOW Showing Active AND Inactive)
    SELECT * FROM ProductCategory 
    WHERE CompanyId IS NULL
    -- REMOVED: AND IsActive = 1  <-- This was hiding the inactive global items
    AND LTRIM(RTRIM(Name)) NOT IN (
        SELECT LTRIM(RTRIM(Name)) 
        FROM ProductCategory 
        WHERE CompanyId = @CompanyId
    )
    ORDER BY Name";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }
        public ProductCategory GetPrivateCategoryByName(string name, int companyId)
        {
            string SQL = @"
                SELECT TOP 1 * FROM ProductCategory 
                WHERE Name = @Name 
                AND CompanyId = @CompanyId";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pNVarChar("Name", 50, name)); // Assuming 50 based on SQL schema
                AddParameter(cmd, pInt32("CompanyId", companyId));
                return GetObject(cmd);
            }
        }

        // ====================================================================
        // WRITE OPERATIONS (Cloning & Status)
        // ====================================================================

        /// <summary>
        /// Atomically clones a Global Category into a new Private Category for the target company.
        /// Returns the ID of the new Private Category.
        /// </summary>
        ///
        /// 
        public int CloneGlobalToPrivate(int globalCategoryId, int targetCompanyId)
        {
            string SQL = @"
        DECLARE @NewId int;

        -- Insert Copy of Global Category (Private)
        -- ADDED: CreatedAt, UpdatedAt (and CreatedBy if you have the username available, otherwise hardcode 'System' or similar)
        INSERT INTO ProductCategory (
            Name, 
            CompanyId, 
            IsActive, 
            CreatedAt, 
            UpdatedAt, 
            CreatedBy, 
            UpdatedBy
        )
        SELECT 
            Name, 
            @TargetCompanyId, 
            IsActive, 
            GETUTCDATE(),  -- Current Time
            GETUTCDATE(),  -- Current Time
            'System',      -- Default User (or pass username as param)
            'System'
        FROM ProductCategory 
        WHERE Id = @GlobalId AND CompanyId IS NULL; 

        SET @NewId = SCOPE_IDENTITY();

        -- Return New ID
        SELECT ISNULL(@NewId, 0);";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("GlobalId", globalCategoryId));
                AddParameter(cmd, pInt32("TargetCompanyId", targetCompanyId));

                object result = SelectScaler(cmd);
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }
        /// <summary>
        /// Efficiently toggles the Active status without updating the whole object.
        /// </summary>
        public void UpdateStatus(int id, bool isActive)
        {
            string SQL = "UPDATE ProductCategory SET IsActive = @IsActive WHERE Id = @Id";
            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("Id", id));
                AddParameter(cmd, pBool("IsActive", isActive));

                if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

    }
}
