using System;
using System.Data;
using System.Data.SqlClient;
using MDUA.Framework.DataAccess;
using MDUA.Entities;
namespace MDUA.DataAccess
{
    public partial class AttributeValueDataAccess
    {
        /// <summary>
        /// Hard deletes all values for a specific attribute. 
        /// </summary>
        public void DeleteAllByAttributeId(int attributeId)
        {
            string SQL = "DELETE FROM AttributeValue WHERE AttributeId = @AttributeId";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("AttributeId", attributeId));
                ExecuteCommand(cmd);
            }
        }

        /// <summary>
        /// Validates if an Attribute belongs to the Company before inserting values.
        /// </summary>
        public bool IsAttributeOwnedByCompany(int attributeId, int companyId)
        {
            string SQL = @"
                SELECT COUNT(1) 
                FROM AttributeName 
                WHERE Id = @AttributeId AND CompanyId = @CompanyId";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("AttributeId", attributeId));
                AddParameter(cmd, pInt32("CompanyId", companyId));

                object result = SelectScaler(cmd);
                return (result != null && Convert.ToInt32(result) > 0);
            }
        }
        /// <summary>
        /// Efficiently toggles the Active status of a value.
        /// </summary>
        public void UpdateStatus(int id, bool isActive)
        {
            string SQL = "UPDATE AttributeValue SET IsActive = @IsActive WHERE Id = @Id";
            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("Id", id));
                AddParameter(cmd, pBool("IsActive", isActive));

                if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Gets ONLY active values for a specific attribute (for Dropdowns).
        /// </summary>
        public List<AttributeValue> GetActiveValues(int attributeId)
        {
            // This correctly shows only active values for the selected attribute ID
            // If we Cloned 'Fabric' (ID 4) to 'Fabric' (ID 5), the UI sends ID 5.
            // We fetch values for ID 5.
            string SQL = @"
        SELECT * FROM AttributeValue 
        WHERE AttributeId = @AttributeId AND IsActive = 1 
        ORDER BY DisplayOrder, Value";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("AttributeId", attributeId));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }
    }
}