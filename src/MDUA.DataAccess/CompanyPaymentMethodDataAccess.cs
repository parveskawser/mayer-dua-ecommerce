using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using MDUA.Framework;
using MDUA.Entities; // Access to CompanyPaymentMethodResult
using MDUA.Entities.Bases;

namespace MDUA.DataAccess
{
    public partial class CompanyPaymentMethodDataAccess
    {
        // 1. GET: Custom Query using your Framework's GetSQLCommand & SelectRecords

        public List<CompanyPaymentMethod> GetActiveByCompany(int companyId)
        {
            // ✅ FIX: Change 'MethodName' to the actual column name in your DB (likely 'Name')
            string sql = @"
        SELECT 
            cpm.Id, 
            cpm.CompanyId, 
            cpm.PaymentMethodId, 
            pm.Name AS MethodName,  -- <--- ALIAS IT AS 'MethodName'
            cpm.IsActive,
            cpm.IsManualEnabled,
            cpm.IsGatewayEnabled
        FROM [dbo].[CompanyPaymentMethod] cpm
        INNER JOIN [dbo].[PaymentMethod] pm ON cpm.PaymentMethodId = pm.Id
        WHERE cpm.CompanyId = @CompanyId AND cpm.IsActive = 1";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                AddParameter(cmd, pInt32("CompanyId", companyId));

                // Note: Standard 'GetList' might fail if it doesn't know how to map 'MethodName'.
                // If your Entity 'CompanyPaymentMethod' does not have 'MethodName' property, 
                // you need a Custom Fill here.

                return GetList_Custom(cmd);
            }
        }

        private List<CompanyPaymentMethod> GetList_Custom(SqlCommand cmd)
        {
            var list = new List<CompanyPaymentMethod>();
            SqlDataReader reader;

            // ✅ FIX: Use framework helper to handle Connection.Open() automatically
            SelectRecords(cmd, out reader);

            using (reader)
            {
                while (reader.Read())
                {
                    var item = new CompanyPaymentMethod();

                    // Map columns by index (matching your SQL query order)
                    item.Id = reader.GetInt32(0);
                    item.CompanyId = reader.GetInt32(1);
                    item.PaymentMethodId = reader.GetInt32(2);
                    item.MethodName = reader.GetString(3); // The joined name
                    item.IsActive = reader.GetBoolean(4);
                    item.IsManualEnabled = reader.GetBoolean(5);
                    item.IsGatewayEnabled = reader.GetBoolean(6);

                    // Required for BaseBusinessEntity
                    item.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;

                    list.Add(item);
                }
            }
            return list;
        }
        public List<CompanyPaymentMethodResult> GetMergedSettings(int companyId)
        {
            var list = new List<CompanyPaymentMethodResult>();

            string sql = @"
                SELECT 
                    pm.Id AS PaymentMethodId,
                    pm.Name AS MethodName,
                    pm.LogoUrl,
                    pm.SystemCode,
                    pm.SupportsManual AS GlobalSupportsManual,
                    pm.SupportsGateway AS GlobalSupportsGateway,
                    pm.ManualInstruction AS DefaultInstruction,
                    
                    ISNULL(cpm.IsActive, 0) AS IsEnabled,
                    ISNULL(cpm.IsManualEnabled, pm.SupportsManual) AS IsManualEnabled, 
                    ISNULL(cpm.IsGatewayEnabled, pm.SupportsGateway) AS IsGatewayEnabled,
                    ISNULL(cpm.CustomInstruction, pm.ManualInstruction) AS CustomInstruction

                FROM [dbo].[PaymentMethod] pm
                LEFT JOIN [dbo].[CompanyPaymentMethod] cpm 
                    ON pm.Id = cpm.PaymentMethodId AND cpm.CompanyId = @CompanyId
                WHERE pm.IsActive = 1
                ORDER BY pm.DisplayOrder";

            // ✅ FIX 1: Use GetSQLCommand (Handles Connection internally)
            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                // ✅ FIX 2: Use pInt32 helper for parameter
                AddParameter(cmd, pInt32("CompanyId", companyId));

                // ✅ FIX 3: Use SelectRecords to execute reader
                SqlDataReader reader;
                SelectRecords(cmd, out reader);

                using (reader)
                {
                    while (reader.Read())
                    {
                        list.Add(new CompanyPaymentMethodResult
                        {
                            PaymentMethodId = (int)reader["PaymentMethodId"],
                            MethodName = reader["MethodName"].ToString(),
                            LogoUrl = reader["LogoUrl"] != DBNull.Value ? reader["LogoUrl"].ToString() : null,
                            SystemCode = reader["SystemCode"] != DBNull.Value ? reader["SystemCode"].ToString() : null,

                            GlobalSupportsManual = (bool)reader["GlobalSupportsManual"],
                            GlobalSupportsGateway = (bool)reader["GlobalSupportsGateway"],
                            DefaultInstruction = reader["DefaultInstruction"] != DBNull.Value ? reader["DefaultInstruction"].ToString() : null,

                            // Safe casting for bit/boolean
                            IsEnabled = reader["IsEnabled"] != DBNull.Value && Convert.ToBoolean(reader["IsEnabled"]),
                            IsManualEnabled = reader["IsManualEnabled"] != DBNull.Value && Convert.ToBoolean(reader["IsManualEnabled"]),
                            IsGatewayEnabled = reader["IsGatewayEnabled"] != DBNull.Value && Convert.ToBoolean(reader["IsGatewayEnabled"]),
                            CustomInstruction = reader["CustomInstruction"] != DBNull.Value ? reader["CustomInstruction"].ToString() : null
                        });
                    }
                    reader.Close();
                }
            }
            return list;
        }

        // 2. SAVE: Logic stays the same (Uses Base Methods Insert/Update which are already correct)
        public void SaveConfiguration(int companyId, int methodId, bool isActive, bool isManual, bool isGateway, string instruction, string username)
        {
            // Build query for GetByQuery
            string query = $"CompanyId = {companyId} AND PaymentMethodId = {methodId}";

            // Call Base method
            var existingList = this.GetByQuery(query);
            var existingRecord = (existingList != null && existingList.Count > 0) ? existingList[0] : null;

            if (existingRecord != null)
            {
                // --- UPDATE EXISTING ---
                existingRecord.IsActive = isActive;
                existingRecord.IsManualEnabled = isManual;
                existingRecord.IsGatewayEnabled = isGateway;
                existingRecord.CustomInstruction = instruction;
                existingRecord.UpdatedBy = username;
                existingRecord.UpdatedAt = DateTime.UtcNow;

                this.Update(existingRecord);
            }
            else
            {
                // --- INSERT NEW ---
                var newRecord = new CompanyPaymentMethod
                {
                    CompanyId = companyId,
                    PaymentMethodId = methodId,
                    IsActive = isActive,
                    IsManualEnabled = isManual,
                    IsGatewayEnabled = isGateway,
                    CustomInstruction = instruction,
                    CreatedBy = username,
                    CreatedAt = DateTime.UtcNow
                };

                this.Insert(newRecord);
            }
        }
    }
}