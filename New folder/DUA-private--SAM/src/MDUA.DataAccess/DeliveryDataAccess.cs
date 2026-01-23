using MDUA.DataAccess.Security;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace MDUA.DataAccess
{
    public partial class DeliveryDataAccess
    {
        private readonly ISecretProtector _secretProtector;

        public DeliveryDataAccess(IConfiguration config, IDataProtectionProvider provider)
    : base(config)
        {
            _secretProtector = new AesSecretProtector(provider);
        }
        // Custom Stored Procedure Names
        private const string SP_INSERT_EXT = "[dbo].[InsertDelivery]";
        private const string SP_UPDATE_EXT = "[dbo].[UpdateDelivery]";
        private const string SP_GET_BY_ORDER_EXT = "[dbo].[GetDeliveryBySalesOrderId]";
        private const string SP_GET_BY_ID_EXT = "[dbo].[GetDeliveryById]";
        private const string SP_GET_ACTIVE_COMPANY_CARRIERS = "[dbo].[GetActiveCompanyCarriersForCompany]";
        private const string SP_GET_COMPANY_CARRIER_CREDENTIALS = "[dbo].[GetCompanyCarrierCredentialsById]";

        #region Extended Methods

        public long InsertExtended(Delivery delivery)
        {
            using (SqlCommand cmd = GetSQLCommand(SP_INSERT_EXT))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Output Parameter
                SqlParameter outParam = new SqlParameter("@Id", SqlDbType.Int);
                outParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outParam);

                AddExtendedParams(cmd, delivery);

                ExecuteCommand(cmd);

                int newId = (int)outParam.Value;
                delivery.Id = newId;
                return newId;
            }
        }
        public List<CompanyCarrierOptionDto> GetActiveCompanyCarrierOptions(int companyId)
        {
            var list = new List<CompanyCarrierOptionDto>();

            using (SqlCommand cmd = GetSQLCommand(SP_GET_ACTIVE_COMPANY_CARRIERS))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                AddParameter(cmd, pInt32("CompanyId", companyId));

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    int ordId = r.GetOrdinal("CompanyCarrierId");
                    int ordName = r.GetOrdinal("CarrierName");
                    int ordEndpoint = r.GetOrdinal("ApiEndpoint");
                    int ordReq = r.GetOrdinal("RequiresApi");

                    while (r.Read())
                    {
                        list.Add(new CompanyCarrierOptionDto
                        {
                            CompanyCarrierId = r.GetInt32(ordId),
                            CarrierName = r.IsDBNull(ordName) ? "" : r.GetString(ordName),
                            ApiEndpoint = r.IsDBNull(ordEndpoint) ? "" : r.GetString(ordEndpoint),
                            RequiresApi = !r.IsDBNull(ordReq) && Convert.ToBoolean(r.GetValue(ordReq))
                        });
                    }
                }
            }

            return list;
        }
        private static string MaskForLog(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "NULL";
            s = s.Trim();
            if (s.Length <= 4) return "****";
            return new string('*', s.Length - 4) + s.Substring(s.Length - 4);
        }

        private static string SqlCmdDebug(SqlCommand cmd)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("=== SQL COMMAND DEBUG ===");
                sb.AppendLine($"Type: {cmd.CommandType}");
                sb.AppendLine($"Text: {cmd.CommandText}");

                if (cmd.Parameters != null && cmd.Parameters.Count > 0)
                {
                    sb.AppendLine("Params:");
                    foreach (SqlParameter p in cmd.Parameters)
                    {
                        string val;
                        try
                        {
                            val = (p.Value == null || p.Value == DBNull.Value) ? "NULL" : p.Value.ToString();
                        }
                        catch
                        {
                            val = "(unreadable)";
                        }
                        sb.AppendLine($"  @{p.ParameterName} = {val} ({p.SqlDbType})");
                    }
                }

                return sb.ToString();
            }
            catch
            {
                return "=== SQL COMMAND DEBUG FAILED ===";
            }
        }

        private static void LogSqlFail(string where, SqlCommand cmd, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("#############################################");
            System.Diagnostics.Debug.WriteLine($"SQL FAILED AT: {where}");
            System.Diagnostics.Debug.WriteLine(ex.ToString());
            if (cmd != null)
                System.Diagnostics.Debug.WriteLine(SqlCmdDebug(cmd));
            System.Diagnostics.Debug.WriteLine("#############################################");
        }

        public CompanyCarrierCredentialDto GetCompanyCarrierCredentials(int companyCarrierId, int companyId)
        {
            using (SqlCommand cmd = GetSQLCommand(SP_GET_COMPANY_CARRIER_CREDENTIALS))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                AddParameter(cmd, pInt32("CompanyCarrierId", companyCarrierId));
                AddParameter(cmd, pInt32("CompanyId", companyId));

                try
                {
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (!r.Read()) return null;

                        string encKey = r.IsDBNull(r.GetOrdinal("ApiKeyEncrypted"))
                            ? null
                            : r.GetString(r.GetOrdinal("ApiKeyEncrypted"));

                        string encSecret = r.IsDBNull(r.GetOrdinal("ApiSecretEncrypted"))
                            ? null
                            : r.GetString(r.GetOrdinal("ApiSecretEncrypted"));

                        string encUsername = null;
                        string encPassword = null;
                        int? storeId = null;

                        int ordUsername = r.GetOrdinal("ApiUsernameEncrypted");
                        if (!r.IsDBNull(ordUsername)) encUsername = r.GetString(ordUsername);

                        int ordPassword = r.GetOrdinal("ApiPasswordEncrypted");
                        if (!r.IsDBNull(ordPassword)) encPassword = r.GetString(ordPassword);

                        int ordStore = r.GetOrdinal("StoreId");
                        if (!r.IsDBNull(ordStore)) storeId = r.GetInt32(ordStore);

                        var dto = new CompanyCarrierCredentialDto
                        {
                            CompanyCarrierId = r.GetInt32(r.GetOrdinal("CompanyCarrierId")),
                            CompanyId = r.GetInt32(r.GetOrdinal("CompanyId")),
                            CarrierId = r.GetInt32(r.GetOrdinal("CarrierId")),
                            CarrierName = r.IsDBNull(r.GetOrdinal("CarrierName")) ? "" : r.GetString(r.GetOrdinal("CarrierName")),
                            ApiEndpoint = r.IsDBNull(r.GetOrdinal("ApiEndpoint")) ? "" : r.GetString(r.GetOrdinal("ApiEndpoint")),
                            RequiresApi = !r.IsDBNull(r.GetOrdinal("RequiresApi")) && Convert.ToBoolean(r.GetValue(r.GetOrdinal("RequiresApi"))),

                            ApiKey = _secretProtector.Decrypt(encKey),
                            ApiSecret = _secretProtector.Decrypt(encSecret),

                            ApiUsername = _secretProtector.Decrypt(encUsername),
                            ApiPassword = _secretProtector.Decrypt(encPassword),
                            StoreId = storeId
                        };

                        // ✅ safe log (mask secrets)
                        System.Diagnostics.Debug.WriteLine(
                            $"[CourierCred] Carrier={dto.CarrierName}, Endpoint={dto.ApiEndpoint}, " +
                            $"Key={MaskForLog(dto.ApiKey)}, Secret={MaskForLog(dto.ApiSecret)}, " +
                            $"User={(string.IsNullOrWhiteSpace(dto.ApiUsername) ? "NULL" : "SET")}, " +
                            $"Pass={(string.IsNullOrWhiteSpace(dto.ApiPassword) ? "NULL" : "SET")}, StoreId={(dto.StoreId.HasValue ? dto.StoreId.Value.ToString() : "NULL")}"
                        );

                        return dto;
                    }
                }
                catch (SqlException ex)
                {
                    LogSqlFail(nameof(GetCompanyCarrierCredentials), cmd, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Builds the exact payload your "Steadfast Order" modal needs.
        /// Tenant-safe by CompanyId.
        /// </summary>
        public ShipmentModalDto GetShipmentModalData(int deliveryId, int companyId)
        {
            const string sql = @"
SELECT
    d.Id                AS DeliveryId,
    d.SalesOrderId,
    d.Status            AS DeliveryStatus,
    d.PackageWeightGrams,
    d.CarrierId         AS SelectedCompanyCarrierId,

    soh.SalesChannelId,
    soh.Id              AS OrderId,
    ISNULL(cus.CustomerName,'') AS RecipientName,
    ISNULL(cus.Phone,'')        AS RecipientPhone,

    LTRIM(RTRIM(
        ISNULL(a.Street,'')
        + CASE WHEN ISNULL(a.Thana,'') <> '' THEN ', ' + a.Thana ELSE '' END
        + CASE WHEN ISNULL(a.Divison,'') <> '' THEN ', ' + a.Divison ELSE '' END
        + CASE WHEN ISNULL(a.PostalCode,'') <> '' THEN ' - ' + a.PostalCode ELSE '' END
    )) AS RecipientAddress,

    ISNULL((
        SELECT SUM(cp.Amount)
        FROM dbo.CustomerPayment cp
        WHERE cp.TransactionReference = soh.SalesOrderId
    ), 0) AS PaidAmount,

    (soh.NetAmount - ISNULL((
        SELECT SUM(cp.Amount)
        FROM dbo.CustomerPayment cp
        WHERE cp.TransactionReference = soh.SalesOrderId
    ), 0)) AS DueAmount

FROM dbo.Delivery d
INNER JOIN dbo.SalesOrderHeader soh ON d.SalesOrderId = soh.Id
INNER JOIN dbo.CompanyCustomer cc ON soh.CompanyCustomerId = cc.Id
INNER JOIN dbo.Customer cus ON cc.CustomerId = cus.Id
LEFT JOIN dbo.Address a ON soh.AddressId = a.Id
WHERE d.Id = @DeliveryId
  AND cc.CompanyId = @CompanyId;

SELECT
    di.Id          AS DeliveryItemId,
    di.Quantity,
    ISNULL(p.ProductName,'') AS ProductName,
    ISNULL(pv.VariantName,'') AS VariantName,
    ISNULL(pv.Sku,'') AS Sku
FROM dbo.DeliveryItem di
INNER JOIN dbo.SalesOrderDetail sod ON di.SalesOrderDetailId = sod.Id
INNER JOIN dbo.ProductVariant pv ON sod.ProductId = pv.Id
INNER JOIN dbo.Product p ON pv.ProductId = p.Id
WHERE di.DeliveryId = @DeliveryId
ORDER BY di.Id;
";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                AddParameter(cmd, pInt32("DeliveryId", deliveryId));
                AddParameter(cmd, pInt32("CompanyId", companyId));

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                var dto = new ShipmentModalDto();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                        return null;

                    dto.DeliveryId = r.GetInt32(r.GetOrdinal("DeliveryId"));
                    dto.SalesOrderId = r.GetInt32(r.GetOrdinal("SalesOrderId"));

                    int ordPkg = r.GetOrdinal("PackageWeightGrams");
                    dto.PackageWeightGrams = r.IsDBNull(ordPkg) ? (int?)null : r.GetInt32(ordPkg);

                    int ordSel = r.GetOrdinal("SelectedCompanyCarrierId");
                    dto.SelectedCompanyCarrierId = r.IsDBNull(ordSel) ? (int?)null : r.GetInt32(ordSel);

                    int salesChannelId = r.GetInt32(r.GetOrdinal("SalesChannelId"));
                    int orderId = r.GetInt32(r.GetOrdinal("OrderId"));
                    dto.OrderNumber = (salesChannelId == 1 ? "ON" : "DO") + orderId.ToString().PadLeft(8, '0');

                    dto.RecipientName = r.GetString(r.GetOrdinal("RecipientName"));
                    dto.RecipientPhone = r.GetString(r.GetOrdinal("RecipientPhone"));
                    dto.RecipientAddress = r.GetString(r.GetOrdinal("RecipientAddress"));

                    decimal due = Convert.ToDecimal(r.GetValue(r.GetOrdinal("DueAmount")));
                    dto.AmountToCollect = due < 0 ? 0 : due;

                    if (r.NextResult())
                    {
                        while (r.Read())
                        {
                            dto.Items.Add(new ShipmentModalItemDto
                            {
                                DeliveryItemId = r.GetInt32(r.GetOrdinal("DeliveryItemId")),
                                Quantity = r.GetInt32(r.GetOrdinal("Quantity")),
                                ProductName = r.GetString(r.GetOrdinal("ProductName")),
                                VariantName = r.GetString(r.GetOrdinal("VariantName")),
                                Sku = r.GetString(r.GetOrdinal("Sku"))
                            });
                        }
                    }
                }

                int qty = 0;
                foreach (var it in dto.Items) qty += it.Quantity;
                dto.ItemQuantity = qty;

                return dto;
            }
        }
        public List<CourierCredentialRowDto> GetCourierCredentialSettings(int companyId)
        {
            var list = new List<CourierCredentialRowDto>();

            const string sql = @"
SELECT
    c.Id            AS CarrierId,
    c.CarrierName,
    c.ApiEndpoint,
    c.RequiresApi,

    ccx.Id          AS CompanyCarrierId,
    ISNULL(ccx.IsActive, 0) AS CompanyCarrierIsActive,

    -- ✅ NEW: these are UI-safe
    ccx.ApiKeyLast4,
    ccx.SecretUpdatedAt,

    -- ✅ existence checks (do NOT render these values)
    ccx.ApiKeyEncrypted,
    ccx.ApiSecretEncrypted,
    ccx.ApiUsernameEncrypted,
    ccx.ApiPasswordEncrypted,
    ccx.StoreId

FROM dbo.Carrier c
LEFT JOIN dbo.CompanyCarrier ccx
    ON ccx.CarrierId = c.Id
   AND ccx.CompanyId = @CompanyId
WHERE c.IsActive = 1
ORDER BY c.Id;
";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                AddParameter(cmd, pInt32("CompanyId", companyId));

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    int ordCarrierId = r.GetOrdinal("CarrierId");
                    int ordCarrierName = r.GetOrdinal("CarrierName");
                    int ordEndpoint = r.GetOrdinal("ApiEndpoint");
                    int ordReq = r.GetOrdinal("RequiresApi");

                    int ordCompanyCarrierId = r.GetOrdinal("CompanyCarrierId");
                    int ordIsActive = r.GetOrdinal("CompanyCarrierIsActive");

                    int ordLast4 = r.GetOrdinal("ApiKeyLast4");
                    int ordUpdatedAt = r.GetOrdinal("SecretUpdatedAt");

                    int ordEncKey = r.GetOrdinal("ApiKeyEncrypted");
                    int ordEncSecret = r.GetOrdinal("ApiSecretEncrypted");
                    int ordEncUser = r.GetOrdinal("ApiUsernameEncrypted");
                    int ordEncPass = r.GetOrdinal("ApiPasswordEncrypted");
                    int ordStoreId = r.GetOrdinal("StoreId");

                    while (r.Read())
                    {
                        bool hasEncKey = !r.IsDBNull(ordEncKey) && !string.IsNullOrWhiteSpace(r.GetString(ordEncKey));
                        bool hasEncSecret = !r.IsDBNull(ordEncSecret) && !string.IsNullOrWhiteSpace(r.GetString(ordEncSecret));
                        bool hasEncUser = !r.IsDBNull(ordEncUser) && !string.IsNullOrWhiteSpace(r.GetString(ordEncUser));
                        bool hasEncPass = !r.IsDBNull(ordEncPass) && !string.IsNullOrWhiteSpace(r.GetString(ordEncPass));

                        string last4 = r.IsDBNull(ordLast4) ? null : r.GetString(ordLast4);
                        DateTime? updatedAt = r.IsDBNull(ordUpdatedAt) ? (DateTime?)null : r.GetDateTime(ordUpdatedAt);
                        int? storeId = r.IsDBNull(ordStoreId) ? (int?)null : r.GetInt32(ordStoreId);

                        // Fixed-length display + optional last4
                        string keyMasked = hasEncKey
                            ? (string.IsNullOrWhiteSpace(last4) ? "••••••••••••" : $"•••••••••••• {last4}")
                            : "—";

                        // You don't have SecretLast4 column; keep constant mask
                        string secretMasked = hasEncSecret ? "••••••••••••" : "—";
                        string usernameMasked = hasEncUser ? "••••••••••••" : "—";
                        string passwordMasked = hasEncPass ? "••••••••••••" : "—";

                        list.Add(new CourierCredentialRowDto
                        {
                            CarrierId = r.GetInt32(ordCarrierId),
                            CarrierName = r.IsDBNull(ordCarrierName) ? "" : r.GetString(ordCarrierName),
                            ApiEndpoint = r.IsDBNull(ordEndpoint) ? "" : r.GetString(ordEndpoint),
                            RequiresApi = !r.IsDBNull(ordReq) && Convert.ToBoolean(r.GetValue(ordReq)),

                            CompanyCarrierId = r.IsDBNull(ordCompanyCarrierId) ? (int?)null : r.GetInt32(ordCompanyCarrierId),
                            IsActive = !r.IsDBNull(ordIsActive) && Convert.ToBoolean(r.GetValue(ordIsActive)),

                            HasApiKey = hasEncKey,
                            HasApiSecret = hasEncSecret,

                            ApiKeyMasked = keyMasked,
                            ApiSecretMasked = secretMasked,
                            ApiUsernameMasked = usernameMasked,
                            ApiPasswordMasked = passwordMasked,
                            HasApiUsername = hasEncUser,
                            HasApiPassword = hasEncPass,
                            StoreId = storeId,

                            // OPTIONAL: if your DTO has these fields; if not, remove these two lines
                            SecretUpdatedAt = updatedAt
                        });
                    }
                }
            }

            return list;
        }
        public void SaveCourierCredential(int companyId, SaveCourierCredentialRequest req, string user)
        {
            if (req == null) throw new Exception("Invalid request.");
            if (req.CarrierId <= 0) throw new Exception("Invalid CarrierId.");

            const string sqlSelect = @"
SELECT TOP 1 Id
FROM dbo.CompanyCarrier
WHERE CompanyId = @CompanyId AND CarrierId = @CarrierId;
";

            int? existingId = null;

            using (SqlCommand cmd = GetSQLCommand(sqlSelect))
            {
                cmd.CommandType = CommandType.Text;
                AddParameter(cmd, pInt32("CompanyId", companyId));
                AddParameter(cmd, pInt32("CarrierId", req.CarrierId));

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                        existingId = r.GetInt32(r.GetOrdinal("Id"));
                }
            }

            bool hasKey = !string.IsNullOrWhiteSpace(req.ApiKey);
            bool hasSecret = !string.IsNullOrWhiteSpace(req.ApiSecret);

            bool hasUser = !string.IsNullOrWhiteSpace(req.ApiUsername);
            bool hasPass = !string.IsNullOrWhiteSpace(req.ApiPassword);
            bool hasStore = req.StoreId.HasValue;

            string keyPlain = hasKey ? req.ApiKey.Trim() : null;
            string secPlain = hasSecret ? req.ApiSecret.Trim() : null;
            string userPlain = hasUser ? req.ApiUsername.Trim() : null;
            string passPlain = hasPass ? req.ApiPassword.Trim() : null;

            string encKey = hasKey ? _secretProtector.Encrypt(keyPlain) : null;
            string encSecret = hasSecret ? _secretProtector.Encrypt(secPlain) : null;
            string encUser = hasUser ? _secretProtector.Encrypt(userPlain) : null;
            string encPass = hasPass ? _secretProtector.Encrypt(passPlain) : null;

            string keyLast4 = null;
            if (hasKey)
                keyLast4 = (keyPlain.Length >= 4) ? keyPlain.Substring(keyPlain.Length - 4) : keyPlain;

            bool willUpdateSecrets = hasKey || hasSecret || hasUser || hasPass || hasStore;

            if (existingId.HasValue)
            {
                const string sqlUpdate = @"
UPDATE dbo.CompanyCarrier
SET
    IsActive = @IsActive,

    ApiKeyEncrypted        = CASE WHEN @HasKey = 1 THEN @ApiKeyEncrypted ELSE ApiKeyEncrypted END,
    ApiSecretEncrypted     = CASE WHEN @HasSecret = 1 THEN @ApiSecretEncrypted ELSE ApiSecretEncrypted END,

    ApiUsernameEncrypted   = CASE WHEN @HasUser = 1 THEN @ApiUsernameEncrypted ELSE ApiUsernameEncrypted END,
    ApiPasswordEncrypted   = CASE WHEN @HasPass = 1 THEN @ApiPasswordEncrypted ELSE ApiPasswordEncrypted END,
    StoreId                = CASE WHEN @HasStore = 1 THEN @StoreId ELSE StoreId END,

    ApiKeyLast4            = CASE WHEN @HasKey = 1 THEN @ApiKeyLast4 ELSE ApiKeyLast4 END,
    SecretUpdatedAt        = CASE WHEN @WillUpdateSecrets = 1 THEN @SecretUpdatedAt ELSE SecretUpdatedAt END
WHERE Id = @Id AND CompanyId = @CompanyId;
";

                using (SqlCommand cmd = GetSQLCommand(sqlUpdate))
                {
                    cmd.CommandType = CommandType.Text;

                    AddParameter(cmd, pInt32("Id", existingId.Value));
                    AddParameter(cmd, pInt32("CompanyId", companyId));

                    AddParameter(cmd, pBit("IsActive", req.IsActive));

                    AddParameter(cmd, pBit("HasKey", hasKey));
                    AddParameter(cmd, pBit("HasSecret", hasSecret));
                    AddParameter(cmd, pBit("HasUser", hasUser));
                    AddParameter(cmd, pBit("HasPass", hasPass));
                    AddParameter(cmd, pBit("HasStore", hasStore));
                    AddParameter(cmd, pBit("WillUpdateSecrets", willUpdateSecrets));

                    AddParameter(cmd, pNVarChar("ApiKeyEncrypted", 2000, encKey));
                    AddParameter(cmd, pNVarChar("ApiSecretEncrypted", 2000, encSecret));
                    AddParameter(cmd, pNVarChar("ApiUsernameEncrypted", 2000, encUser));
                    AddParameter(cmd, pNVarChar("ApiPasswordEncrypted", 2000, encPass));

                    AddParameter(cmd, pInt32("StoreId", hasStore ? req.StoreId.Value : 0));
                    AddParameter(cmd, pNVarChar("ApiKeyLast4", 20, keyLast4));
                    AddParameter(cmd, pDateTime("SecretUpdatedAt", willUpdateSecrets ? DateTime.UtcNow : (DateTime?)null));

                    ExecuteCommand(cmd);
                }
            }
            else
            {
                const string sqlInsert = @"
INSERT INTO dbo.CompanyCarrier
(
    CompanyId, CarrierId, IsActive,
    ApiKeyEncrypted, ApiSecretEncrypted,
    ApiUsernameEncrypted, ApiPasswordEncrypted, StoreId,
    ApiKeyLast4, SecretUpdatedAt
)
VALUES
(
    @CompanyId, @CarrierId, @IsActive,
    @ApiKeyEncrypted, @ApiSecretEncrypted,
    @ApiUsernameEncrypted, @ApiPasswordEncrypted, @StoreId,
    @ApiKeyLast4, @SecretUpdatedAt
);
";

                using (SqlCommand cmd = GetSQLCommand(sqlInsert))
                {
                    cmd.CommandType = CommandType.Text;

                    AddParameter(cmd, pInt32("CompanyId", companyId));
                    AddParameter(cmd, pInt32("CarrierId", req.CarrierId));
                    AddParameter(cmd, pBit("IsActive", req.IsActive));

                    AddParameter(cmd, pNVarChar("ApiKeyEncrypted", 2000, encKey));
                    AddParameter(cmd, pNVarChar("ApiSecretEncrypted", 2000, encSecret));
                    AddParameter(cmd, pNVarChar("ApiUsernameEncrypted", 2000, encUser));
                    AddParameter(cmd, pNVarChar("ApiPasswordEncrypted", 2000, encPass));

                    AddParameter(cmd, pInt32("StoreId", hasStore ? req.StoreId.Value : (int?)null));
                    AddParameter(cmd, pNVarChar("ApiKeyLast4", 20, keyLast4));
                    AddParameter(cmd, pDateTime("SecretUpdatedAt", willUpdateSecrets ? DateTime.UtcNow : (DateTime?)null));

                    ExecuteCommand(cmd);
                }
            }
        }


        private string MaskSecret(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "—";
            s = s.Trim();
            if (s.Length <= 4) return "****";
            return new string('*', s.Length - 4) + s.Substring(s.Length - 4);
        }

        private static bool TryGetOrdinal(IDataRecord record, string columnName, out int ordinal)
        {
            for (int i = 0; i < record.FieldCount; i++)
            {
                if (string.Equals(record.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    ordinal = i;
                    return true;
                }
            }

            ordinal = -1;
            return false;
        }

        public void UpdateExtended(Delivery delivery)
        {
            using (SqlCommand cmd = GetSQLCommand(SP_UPDATE_EXT))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = delivery.Id });

                AddExtendedParams(cmd, delivery);

                ExecuteCommand(cmd);
            }
        }

        public Delivery GetBySalesOrderIdExtended(int salesOrderId)
        {
            // DEBUG: Uncomment this line to prove the new code is loaded
            // throw new Exception("DEBUG: I am using the EXTENDED method!");

            using (SqlCommand cmd = GetSQLCommand(SP_GET_BY_ORDER_EXT))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@SalesOrderId", SqlDbType.Int) { Value = salesOrderId });

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Verify we are NOT calling the base FillObject
                        return FillObjectExtended(reader);
                    }
                }
            }
            return null;
        }
        // Inside MDUA.DataAccess/DeliveryDataAccess.cs

        public Delivery GetExtended(int id)
        {
            // Use the SP that does SELECT *
            using (SqlCommand cmd = GetSQLCommand("[dbo].[GetDeliveryById]"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // This calls the method that uses reader.GetOrdinal("ColumnName")
                        return FillObjectExtended(reader);
                    }
                }
            }
            return null;
        }

        // Inside DeliveryDataAccess.cs

        public long InsertDeliveryItem(int deliveryId, int salesOrderDetailId, int quantity)
        {
            // Use the SP name you provided
            using (SqlCommand cmd = GetSPCommand("[dbo].[InsertDeliveryItem]"))
            {
                // 1. Output Parameter (@Id)
                AddParameter(cmd, pInt32Out("Id"));

                // 2. Input Parameters
                AddParameter(cmd, pInt32("DeliveryId", deliveryId));
                AddParameter(cmd, pInt32("SalesOrderDetailId", salesOrderDetailId));
                AddParameter(cmd, pInt32("Quantity", quantity));

                // 3. Execute
                // InsertRecord handles the execution and connection logic in your framework
                long result = InsertRecord(cmd);

                // 4. Return the new ID (if successful)
                if (result > 0)
                {
                    return (int)GetOutParameter(cmd, "Id");
                }
                return -1;
            }
        }



        // --- 1. Fix for "Does not implement Update" ---
        public int Update(Delivery delivery)
        {
            // Simply call your existing extended logic
            UpdateExtended(delivery);
            return delivery.Id;
        }

        public System.Collections.Generic.IList<Delivery> LoadAllWithDetails(int companyId)
        {
            var result = new System.Collections.Generic.List<Delivery>();

            string sql = @"
    SELECT 
        d.Id, d.SalesOrderId, d.TrackingNumber, d.Status, 
        carr.CarrierName, -- ✅ FETCH FROM JOINED TABLE
        d.ShipDate, d.EstimatedArrival, d.ActualDeliveryDate, d.ShippingCost,
        
        soh.Id AS OrderId, 
        soh.SalesChannelId,
        soh.Status AS OrderStatus,      
        soh.Confirmed AS OrderConfirmed, 
        
        c.CustomerName,
        
        di.Id AS ItemId, di.Quantity,
        
        p.ProductName, 
        pv.VariantName, 
        pv.Sku
    FROM Delivery d
    INNER JOIN SalesOrderHeader soh ON d.SalesOrderId = soh.Id
    INNER JOIN CompanyCustomer cc ON soh.CompanyCustomerId = cc.Id
    INNER JOIN Customer c ON cc.CustomerId = c.Id -- (Alias 'c' is Customer)
    
    -- ✅ NEW JOINS FOR CARRIER
    LEFT JOIN CompanyCarrier compCarr ON d.CarrierId = compCarr.Id
    LEFT JOIN Carrier carr ON compCarr.CarrierId = carr.Id

    LEFT JOIN DeliveryItem di ON d.Id = di.DeliveryId
    LEFT JOIN SalesOrderDetail sod ON di.SalesOrderDetailId = sod.Id
    LEFT JOIN ProductVariant pv ON sod.ProductId = pv.Id 
    LEFT JOIN Product p ON pv.ProductId = p.Id
    WHERE cc.CompanyId = @CompanyId
    ORDER BY d.Id DESC";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                // ✅ Add the Security Parameter
                AddParameter(cmd, pInt32("CompanyId", companyId));

                cmd.CommandType = CommandType.Text;

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    var lookup = new System.Collections.Generic.Dictionary<int, Delivery>();

                    while (reader.Read())
                    {
                        int deliveryId = reader.GetInt32(reader.GetOrdinal("Id"));

                        if (!lookup.TryGetValue(deliveryId, out Delivery delivery))
                        {
                            delivery = new Delivery
                            {
                                Id = deliveryId,
                                SalesOrderId = reader.GetInt32(reader.GetOrdinal("SalesOrderId")),
                                TrackingNumber = reader.IsDBNull(reader.GetOrdinal("TrackingNumber")) ? null : reader.GetString(reader.GetOrdinal("TrackingNumber")),
                                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? "Pending" : reader.GetString(reader.GetOrdinal("Status")),
                              //  CarrierName = reader.IsDBNull(reader.GetOrdinal("CarrierName")) ? null : reader.GetString(reader.GetOrdinal("CarrierName")),
                                ShipDate = reader.IsDBNull(reader.GetOrdinal("ShipDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ShipDate")),
                                EstimatedArrival = reader.IsDBNull(reader.GetOrdinal("EstimatedArrival")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EstimatedArrival")),
                                ActualDeliveryDate = reader.IsDBNull(reader.GetOrdinal("ActualDeliveryDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ActualDeliveryDate")),
                                ShippingCost = reader.IsDBNull(reader.GetOrdinal("ShippingCost")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("ShippingCost")),

                                DeliveryItems = new System.Collections.Generic.List<DeliveryItem>(),

                                SalesOrderHeader = new SalesOrderHeader
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("OrderId")),
                                    Status = reader.IsDBNull(reader.GetOrdinal("OrderStatus")) ? "" : reader.GetString(reader.GetOrdinal("OrderStatus")),
                                    Confirmed = !reader.IsDBNull(reader.GetOrdinal("OrderConfirmed")) && reader.GetBoolean(reader.GetOrdinal("OrderConfirmed")),

                                    CompanyCustomer = new CompanyCustomer
                                    {
                                        Customer = new Customer
                                        {
                                            CustomerName = reader.GetString(reader.GetOrdinal("CustomerName"))
                                        }
                                    }
                                }
                            };

                            int channelId = reader.GetInt32(reader.GetOrdinal("SalesChannelId"));
                            if (channelId == 1)
                                delivery.SalesOrderHeader.OnlineOrderId = "ON" + delivery.SalesOrderHeader.Id.ToString().PadLeft(8, '0');
                            else
                                delivery.SalesOrderHeader.DirectOrderId = "DO" + delivery.SalesOrderHeader.Id.ToString().PadLeft(8, '0');

                            lookup.Add(deliveryId, delivery);
                            result.Add(delivery);
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("ItemId")))
                        {
                            var item = new DeliveryItem
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ItemId")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                SalesOrderDetail = new SalesOrderDetail
                                {
                                    ProductVariant = new ProductVariant
                                    {
                                        VariantName = reader.IsDBNull(reader.GetOrdinal("VariantName")) ? "" : reader.GetString(reader.GetOrdinal("VariantName")),
                                        SKU = reader.IsDBNull(reader.GetOrdinal("Sku")) ? "" : reader.GetString(reader.GetOrdinal("Sku")),
                                        Product = new Product
                                        {
                                            ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? "Unknown" : reader.GetString(reader.GetOrdinal("ProductName"))
                                        }
                                    }
                                }
                            };
                            delivery.DeliveryItems.Add(item);
                        }
                    }
                }
            }
            return result;
        }

        #endregion

        #region Courier Service Integration Methods
        public async Task<CourierShipmentResult> CreateCarrierShipmentAsync(
           int deliveryId,
           int companyId,
           int companyCarrierId,
           string updatedBy)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[ShipFlow] START deliveryId={deliveryId}, companyId={companyId}, companyCarrierId={companyCarrierId}");

                var shipmentData = GetShipmentModalData(deliveryId, companyId);
                if (shipmentData == null)
                    throw new Exception("Shipment data not found");

                var carrier = GetCompanyCarrierCredentials(companyCarrierId, companyId);
                if (carrier == null)
                    throw new Exception("Carrier credentials not found");

                var invoice = $"{shipmentData.SalesOrderId}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                var request = new CourierShipmentRequest
                {
                    Invoice = invoice,
                    RecipientName = shipmentData.RecipientName,
                    RecipientPhone = shipmentData.RecipientPhone,
                    RecipientAddress = shipmentData.RecipientAddress,
                    CodAmount = shipmentData.AmountToCollect,
                    TotalItem = shipmentData.ItemQuantity,
                    Note = shipmentData.SpecialInstruction,
                    ItemDescription = "Order items",

                    ApiBaseUrl = carrier.ApiEndpoint,
                    ApiKey = carrier.ApiKey,
                    ApiSecret = carrier.ApiSecret,

                    ApiUsername = carrier.ApiUsername,
                    ApiPassword = carrier.ApiPassword,
                    StoreId = carrier.StoreId,

                    PackageWeightGrams = shipmentData.PackageWeightGrams
                };

                // ✅ log request (mask secrets)
                System.Diagnostics.Debug.WriteLine(
                    $"[ShipFlow] Carrier={carrier.CarrierName}, BaseUrl={request.ApiBaseUrl}, Invoice={request.Invoice}, " +
                    $"Key={MaskForLog(request.ApiKey)}, Secret={MaskForLog(request.ApiSecret)}, " +
                    $"User={(string.IsNullOrWhiteSpace(request.ApiUsername) ? "NULL" : "SET")}, " +
                    $"Pass={(string.IsNullOrWhiteSpace(request.ApiPassword) ? "NULL" : "SET")}, StoreId={(request.StoreId.HasValue ? request.StoreId.Value.ToString() : "NULL")}"
                );

                var client = CourierClientFactory.Resolve(carrier.CarrierName);

                var result = await client.CreateShipmentAsync(request);

                System.Diagnostics.Debug.WriteLine($"[ShipFlow] ClientResult Success={result.Success}, Err={result.ErrorMessage}");

                if (!result.Success)
                    throw new Exception(result.ErrorMessage);

                var delivery = GetExtended(deliveryId);

                delivery.Status = "Shipped";
                delivery.ShipDate = DateTime.UtcNow;
                delivery.UpdatedAt = DateTime.UtcNow;
                delivery.UpdatedBy = updatedBy;

                delivery.CarrierId = companyCarrierId;
                delivery.TrackingNumber = result.TrackingNumber;
                delivery.ConsignmentId = result.ConsignmentId;
                delivery.CarrierResponse = result.RawResponse;

                UpdateExtended(delivery);

                System.Diagnostics.Debug.WriteLine($"[ShipFlow] DONE deliveryId={deliveryId}");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("#############################################");
                System.Diagnostics.Debug.WriteLine("[ShipFlow] FAILED");
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                System.Diagnostics.Debug.WriteLine("#############################################");
                throw;
            }
        }
        // Inside MDUA.DataAccess/DeliveryDataAccess.cs

        public MDUA.Entities.CompanyCarrierCredentialDto GetCompanyCarrierCredential(int companyId, int companyCarrierId)
        {
            // ✅ Corrected SQL: Joins correctly and selects Encrypted columns
            string sql = @"
        SELECT 
            cc.Id AS CompanyCarrierId,
            cc.CompanyId,
            c.Id AS CarrierId,
            c.CarrierName,
            c.ApiEndpoint,
            c.RequiresApi,
            cc.ApiKeyEncrypted,
            cc.ApiSecretEncrypted,
            cc.ApiUsernameEncrypted,
            cc.ApiPasswordEncrypted,
            cc.StoreId
        FROM dbo.CompanyCarrier cc
        INNER JOIN dbo.Carrier c ON cc.CarrierId = c.Id
        WHERE cc.CompanyId = @CompanyId 
          AND cc.Id = @CompanyCarrierId";

            // ✅ Fix: Use 'GetSQLCommand' instead of 'CreateCommand'
            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                cmd.CommandType = CommandType.Text;

                // ✅ Fix: Use your framework's parameter helpers
                AddParameter(cmd, pInt32("CompanyId", companyId));
                AddParameter(cmd, pInt32("CompanyCarrierId", companyCarrierId));

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        // Helper to safely read strings
                        string GetStr(string col) => r.IsDBNull(r.GetOrdinal(col)) ? null : r.GetString(r.GetOrdinal(col));

                        return new MDUA.Entities.CompanyCarrierCredentialDto
                        {
                            CompanyCarrierId = r.GetInt32(r.GetOrdinal("CompanyCarrierId")),
                            CompanyId = r.GetInt32(r.GetOrdinal("CompanyId")),
                            CarrierId = r.GetInt32(r.GetOrdinal("CarrierId")),
                            CarrierName = GetStr("CarrierName"),
                            ApiEndpoint = GetStr("ApiEndpoint"),
                            RequiresApi = !r.IsDBNull(r.GetOrdinal("RequiresApi")) && Convert.ToBoolean(r.GetValue(r.GetOrdinal("RequiresApi"))),

                            // ✅ Fix: Decrypt the credentials so the Facade gets real keys
                            ApiKey = _secretProtector.Decrypt(GetStr("ApiKeyEncrypted")),
                            ApiSecret = _secretProtector.Decrypt(GetStr("ApiSecretEncrypted")),
                            ApiUsername = _secretProtector.Decrypt(GetStr("ApiUsernameEncrypted")),
                            ApiPassword = _secretProtector.Decrypt(GetStr("ApiPasswordEncrypted")),
                            StoreId = r.IsDBNull(r.GetOrdinal("StoreId")) ? (int?)null : r.GetInt32(r.GetOrdinal("StoreId"))
                        };
                    }
                }
            }
            return null;
        }
        #endregion

        #region Private Helpers

        private void AddExtendedParams(SqlCommand cmd, Delivery obj)
        {
            // 1. Standard Fields
            cmd.Parameters.Add(new SqlParameter("@SalesOrderId", SqlDbType.Int) { Value = obj.SalesOrderId });
            cmd.Parameters.Add(new SqlParameter("@TrackingNumber", SqlDbType.NVarChar, 100) { Value = (object)obj.TrackingNumber ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.NVarChar, 50) { Value = (object)obj.Status ?? "Pending" });

            // ❌ REMOVED: @CarrierName (It is no longer in the SP)
            // cmd.Parameters.Add(new SqlParameter("@CarrierName", ...)); 

            // ✅ ADDED: New Foreign Key and Data Columns
            cmd.Parameters.Add(new SqlParameter("@CarrierId", SqlDbType.Int) { Value = (object)obj.CarrierId ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CarrierCharge", SqlDbType.Decimal) { Value = (object)obj.CarrierCharge ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PackageWeightGrams", SqlDbType.Int) { Value = (object)obj.PackageWeightGrams ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ConsignmentId", SqlDbType.NVarChar, 100) { Value = (object)obj.ConsignmentId ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CarrierResponse", SqlDbType.NVarChar, -1) { Value = (object)obj.CarrierResponse ?? DBNull.Value }); // -1 for MAX

            // Dates
            cmd.Parameters.Add(new SqlParameter("@ShipDate", SqlDbType.DateTime) { Value = obj.ShipDate.HasValue ? (object)obj.ShipDate.Value : DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@EstimatedArrival", SqlDbType.DateTime) { Value = obj.EstimatedArrival.HasValue ? (object)obj.EstimatedArrival.Value : DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ActualDeliveryDate", SqlDbType.DateTime) { Value = obj.ActualDeliveryDate.HasValue ? (object)obj.ActualDeliveryDate.Value : DBNull.Value });

            // Customer Shipping Cost (Revenue)
            cmd.Parameters.Add(new SqlParameter("@ShippingCost", SqlDbType.Decimal) { Value = obj.ShippingCost.HasValue ? (object)obj.ShippingCost.Value : DBNull.Value });

            // Audit Fields
            cmd.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100) { Value = (object)obj.CreatedBy ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CreatedAt", SqlDbType.DateTime) { Value = obj.CreatedAt == DateTime.MinValue ? DateTime.UtcNow : obj.CreatedAt });
            cmd.Parameters.Add(new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = (object)obj.UpdatedBy ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@UpdatedAt", SqlDbType.DateTime) { Value = obj.UpdatedAt.HasValue ? (object)obj.UpdatedAt.Value : DBNull.Value });
        }
        private Delivery FillObjectExtended(SqlDataReader reader)
        {
            Delivery obj = new Delivery();

            obj.Id = reader.GetInt32(reader.GetOrdinal("Id"));
            obj.SalesOrderId = reader.GetInt32(reader.GetOrdinal("SalesOrderId"));

            // 1. Map Existing Strings
            int idxTracking = reader.GetOrdinal("TrackingNumber");
            if (!reader.IsDBNull(idxTracking)) obj.TrackingNumber = reader.GetString(idxTracking);

            int idxStatus = reader.GetOrdinal("Status");
            if (!reader.IsDBNull(idxStatus)) obj.Status = reader.GetString(idxStatus);

            // 2. Map Carrier Name (Coming from the JOIN in your SP)
            // Note: Use a try/catch or check column existence if you use this method for other queries that don't join
            try
            {
                int idxCarrier = reader.GetOrdinal("CarrierName");
                if (!reader.IsDBNull(idxCarrier)) obj.CarrierName = reader.GetString(idxCarrier);
            }
            catch { /* Column might not exist in simple queries */ }

            // 3. ✅ Map NEW Columns
            int idxCarrierId = reader.GetOrdinal("CarrierId");
            if (!reader.IsDBNull(idxCarrierId)) obj.CarrierId = reader.GetInt32(idxCarrierId);

            int idxCarrierCharge = reader.GetOrdinal("CarrierCharge");
            if (!reader.IsDBNull(idxCarrierCharge)) obj.CarrierCharge = reader.GetDecimal(idxCarrierCharge);

            int idxWeight = reader.GetOrdinal("PackageWeightGrams");
            if (!reader.IsDBNull(idxWeight)) obj.PackageWeightGrams = reader.GetInt32(idxWeight);

            int idxConsignment = reader.GetOrdinal("ConsignmentId");
            if (!reader.IsDBNull(idxConsignment)) obj.ConsignmentId = reader.GetString(idxConsignment);

            int idxResponse = reader.GetOrdinal("CarrierResponse");
            if (!reader.IsDBNull(idxResponse)) obj.CarrierResponse = reader.GetString(idxResponse);

            // 4. Map Dates
            int idxShipDate = reader.GetOrdinal("ShipDate");
            if (!reader.IsDBNull(idxShipDate)) obj.ShipDate = reader.GetDateTime(idxShipDate);

            int idxEstArrival = reader.GetOrdinal("EstimatedArrival");
            if (!reader.IsDBNull(idxEstArrival)) obj.EstimatedArrival = reader.GetDateTime(idxEstArrival);

            int idxActualDelivery = reader.GetOrdinal("ActualDeliveryDate");
            if (!reader.IsDBNull(idxActualDelivery)) obj.ActualDeliveryDate = reader.GetDateTime(idxActualDelivery);

            // 5. Map Cost & Audit
            int idxCost = reader.GetOrdinal("ShippingCost");
            if (!reader.IsDBNull(idxCost)) obj.ShippingCost = reader.GetDecimal(idxCost);
            else obj.ShippingCost = 0;

            int idxCreatedBy = reader.GetOrdinal("CreatedBy");
            if (!reader.IsDBNull(idxCreatedBy)) obj.CreatedBy = reader.GetString(idxCreatedBy);

            obj.CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"));

            int idxUpdatedBy = reader.GetOrdinal("UpdatedBy");
            if (!reader.IsDBNull(idxUpdatedBy)) obj.UpdatedBy = reader.GetString(idxUpdatedBy);

            int idxUpdatedAt = reader.GetOrdinal("UpdatedAt");
            if (!reader.IsDBNull(idxUpdatedAt)) obj.UpdatedAt = reader.GetDateTime(idxUpdatedAt);

            obj.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
            return obj;
        }


        public void UpdateCourierStatus(int companyId, int companyCarrierId, bool isActive)
        {
            string sql = @"
        UPDATE dbo.CompanyCarrier
        SET IsActive = @IsActive
        WHERE Id = @CompanyCarrierId 
          AND CompanyId = @CompanyId";

            using (var cmd = GetSQLCommand(sql))
            {
                cmd.CommandType = CommandType.Text;

                AddParameter(cmd, pInt32("CompanyCarrierId", companyCarrierId));
                AddParameter(cmd, pInt32("CompanyId", companyId));
                AddParameter(cmd, pBit("IsActive", isActive));

                ExecuteCommand(cmd);
            }
        }
        #endregion
    }
}
