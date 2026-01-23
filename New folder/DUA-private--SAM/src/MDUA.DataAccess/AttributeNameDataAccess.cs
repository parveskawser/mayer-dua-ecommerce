using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MDUA.Entities;
using MDUA.Framework.DataAccess;

namespace MDUA.DataAccess
{
    public partial class AttributeNameDataAccess
    {
        // ==========================================================================

        // ==========================================================================

        public List<AttributeValue> GetValuesByAttributeId(int attributeId)
        {
            string query = $"AttributeId = {attributeId}";
            using (SqlCommand cmd = GetSPCommand("GetAttributeValueByQuery"))
            {
                AddParameter(cmd, pNVarChar("Query", 4000, query));
                SqlDataReader reader;
                SelectRecords(cmd, out reader);
                List<AttributeValue> list = new List<AttributeValue>();
                using (reader)
                {
                    while (reader.Read())
                    {
                        list.Add(new AttributeValue
                        {
                            Id = reader.GetInt32(0),
                            AttributeId = reader.GetInt32(1),
                            Value = reader.GetString(2)
                        });
                    }
                }
                return list;
            }
        }

        public List<AttributeName> GetAttributeNamesByProductId(int productId)
        {
            string SQLQuery = @"
        SELECT a.*
        FROM AttributeName a
        JOIN ProductAttribute pa ON a.Id = pa.AttributeId
        WHERE pa.ProductId = @ProductId
        ORDER BY pa.DisplayOrder";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("ProductId", productId));

                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }
        public List<AttributeName> GetByProductId(int productId)
        {
            string SQLQuery = @"
                SELECT DISTINCT a.Id, a.Name
                FROM AttributeName a
                JOIN ProductAttribute pa ON a.Id = pa.AttributeId
                WHERE pa.ProductId = @ProductId
                ORDER BY a.Name";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("ProductId", productId));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }

        public List<AttributeName> GetMissingAttributesForVariant(int productId, int variantId)
        {
            var list = new List<AttributeName>();
            string SQLQuery = @"
                SELECT Id, Name 
                FROM AttributeName 
                WHERE Id NOT IN (
                    SELECT AttributeId 
                    FROM VariantAttributeValue 
                    WHERE VariantId = @VariantId
                )
                ORDER BY Name";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("VariantId", variantId));
                SqlDataReader reader;
                SelectRecords(cmd, out reader);
                using (reader)
                {
                    while (reader.Read())
                    {
                        list.Add(new AttributeName
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            return list;
        }

        public string GetValueName(int valueId)
        {
            string SQLQuery = "SELECT Value FROM AttributeValue WHERE Id = @Id";
            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("Id", valueId));
                SqlDataReader reader;
                SelectRecords(cmd, out reader);
                string result = "";
                using (reader)
                {
                    if (reader.Read() && !reader.IsDBNull(0))
                    {
                        result = reader.GetString(0);
                    }
                }
                return result;
            }
        }

        public Dictionary<string, List<string>> GetSpecificationsByProductId(int productId)
        {
            string SQLQuery = @"
                SELECT DISTINCT 
                    an.Name AS AttributeName, 
                    av.Value AS AttributeValue
                FROM ProductVariant pv
                JOIN VariantAttributeValue vav ON pv.Id = vav.VariantId
                JOIN AttributeValue av ON vav.AttributeValueId = av.Id
                JOIN AttributeName an ON av.AttributeId = an.Id
                WHERE pv.ProductId = @ProductId 
                  AND pv.IsActive = 1 
                ORDER BY an.Name, av.Value";

            var specs = new Dictionary<string, List<string>>();

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("ProductId", productId));
                SqlDataReader reader;
                SelectRecords(cmd, out reader);

                using (reader)
                {
                    while (reader.Read())
                    {
                        string attrName = reader.GetString(0);
                        string attrValue = reader.GetString(1);

                        if (!specs.ContainsKey(attrName))
                        {
                            specs[attrName] = new List<string>();
                        }
                        if (!specs[attrName].Contains(attrValue))
                        {
                            specs[attrName].Add(attrValue);
                        }
                    }
                }
            }
            return specs;
        }

        // ==========================================================================
        // 
        // ==========================================================================

        /// <summary>
        /// Gets all Global Attributes (NULL) AND Private Attributes for the specific company.
        /// </summary>
        public List<AttributeName> GetAvailableAttributes(int companyId)
        {
            string SQL = @"
            -- 1. My ACTIVE Private Attributes
            SELECT * FROM AttributeName 
            WHERE IsActive = 1 
            AND CompanyId = @CompanyId

            UNION ALL

            -- 2. Global ACTIVE Attributes (Only if NOT overridden by ANY Private attribute)
            SELECT * FROM AttributeName 
            WHERE IsActive = 1 
            AND CompanyId IS NULL
            AND LTRIM(RTRIM(Name)) NOT IN (
                -- If I have a private version (Active OR Inactive), 
                -- it overrides the Global one, so hide the Global one.
                SELECT LTRIM(RTRIM(Name)) 
                FROM AttributeName 
                WHERE CompanyId = @CompanyId
            )
            ORDER BY DisplayOrder, Name";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }

        public List<AttributeName> GetAllAttributesForManagement(int companyId)
        {
            string SQL = @"
            -- 1. My Private Attributes (Active AND Inactive)
            SELECT * FROM AttributeName 
            WHERE CompanyId = @CompanyId

            UNION ALL

            -- 2. Global Attributes (Active Only - usually we don't manage inactive system globals)
            -- But still hide them if I have a private version.
            SELECT * FROM AttributeName 
            WHERE IsActive = 1 
            AND CompanyId IS NULL
            AND LTRIM(RTRIM(Name)) NOT IN (
                SELECT LTRIM(RTRIM(Name)) 
                FROM AttributeName 
                WHERE CompanyId = @CompanyId
            )
            ORDER BY DisplayOrder, Name";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }
        /// Checks if a company already has a private version of an attribute name.
        /// </summary>
        public AttributeName GetPrivateAttribute(string name, int companyId)
        {
            string SQL = @"
                SELECT TOP 1 * FROM AttributeName 
                WHERE Name = @Name 
                AND CompanyId = @CompanyId";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pNVarChar("Name", 100, name));
                AddParameter(cmd, pInt32("CompanyId", companyId));
                return GetObject(cmd);
            }
        }

        /// <summary>
        /// Atomically clones a Global Attribute and all its Values
        /// into a new Private Attribute for the target company.
        /// </summary>
        public int CloneGlobalToPrivate(int globalAttributeId, int targetCompanyId)
        {
            string SQL = @"
                DECLARE @NewId int;

                -- 1. Insert Copy of Name (Private)
                INSERT INTO AttributeName (Name, DisplayOrder, IsVariantAffecting, IsActive, CompanyId)
                SELECT Name, DisplayOrder, IsVariantAffecting, IsActive, @TargetCompanyId
                FROM AttributeName 
                WHERE Id = @GlobalId AND CompanyId IS NULL; 

                SET @NewId = SCOPE_IDENTITY();

                -- 2. Copy Values
                IF @NewId IS NOT NULL
                BEGIN
                    INSERT INTO AttributeValue (AttributeId, Value, DisplayOrder, IsActive)
                    SELECT @NewId, Value, DisplayOrder, IsActive
                    FROM AttributeValue 
                    WHERE AttributeId = @GlobalId;
                END

                -- 3. Return New ID
                SELECT ISNULL(@NewId, 0);";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("GlobalId", globalAttributeId));
                AddParameter(cmd, pInt32("TargetCompanyId", targetCompanyId));

                // Transaction support handled automatically by base
                object result = SelectScaler(cmd);
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }
        /// <summary>
        /// Efficiently toggles the Active status without updating the whole object.
        /// </summary>
        public void UpdateStatus(int id, bool isActive)
        {
            string SQL = "UPDATE AttributeName SET IsActive = @IsActive WHERE Id = @Id";
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