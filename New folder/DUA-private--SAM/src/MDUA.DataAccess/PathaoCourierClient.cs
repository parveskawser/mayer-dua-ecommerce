using MDUA.DataAccess.Interface;
using MDUA.Entities;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MDUA.DataAccess
{
    public class PathaoCourierClient : ICourierClient
    {
        public string CarrierName => "Pathao";

        private static string CleanBaseUrl(string s)
        {
            var raw = (s ?? "").Trim();
            raw = Regex.Replace(raw, @"\s+", ""); // remove \r\n spaces etc anywhere
            return raw.TrimEnd('/');
        }

        private static string CleanBdPhone11(string input)
        {
            var digits = Regex.Replace(input ?? "", @"\D", "");

            // +8801XXXXXXXXX => 01XXXXXXXXX
            if (digits.Length == 13 && digits.StartsWith("880"))
                digits = "0" + digits.Substring(3);

            // 8801XXXXXXXXX (still 13) => 01XXXXXXXXX
            if (digits.Length == 13 && digits.StartsWith("88"))
                digits = "0" + digits.Substring(2);

            if (digits.Length != 11 || !digits.StartsWith("01"))
                return null;

            return digits;
        }
        private static string Shorten(string s, int max = 1200)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return (s.Length <= max) ? s : s.Substring(0, max) + "...(truncated)";
        }

        private static string MaskForLog(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "NULL";
            s = s.Trim();
            if (s.Length <= 4) return "****";
            return new string('*', s.Length - 4) + s.Substring(s.Length - 4);
        }

        private sealed class TokenResp
        {
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string access_token { get; set; }
            public string refresh_token { get; set; }
        }

        private async Task<string> IssueTokenAsync(string baseUrl, string clientId, string clientSecret, string username, string password)
        {
            var b = CleanBaseUrl(baseUrl);
            var tokenUrl = $"{b}/aladdin/api/v1/issue-token";

            System.Diagnostics.Debug.WriteLine(
                $"[Pathao] IssueToken URL={tokenUrl}, client_id={MaskForLog(clientId)}, client_secret={MaskForLog(clientSecret)}, " +
                $"username={(string.IsNullOrWhiteSpace(username) ? "NULL" : "SET")}, password={(string.IsNullOrWhiteSpace(password) ? "NULL" : "SET")}"
            );

            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

            var payload = new
            {
                client_id = clientId,
                client_secret = clientSecret,
                grant_type = "password",
                username = username,
                password = password
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await http.PostAsync(tokenUrl, content);
            var body = await resp.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[Pathao] IssueToken Status={(int)resp.StatusCode} {resp.ReasonPhrase} Body={Shorten(body)}");

            if (!resp.IsSuccessStatusCode)
                throw new Exception($"Pathao token failed: {(int)resp.StatusCode} {resp.ReasonPhrase} :: {body}");

            var token = JsonSerializer.Deserialize<TokenResp>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (token == null || string.IsNullOrWhiteSpace(token.access_token))
                throw new Exception($"Pathao token missing access_token :: {body}");

            return token.access_token;
        }

        public async Task<CourierShipmentResult> CreateShipmentAsync(CourierShipmentRequest r)
        {
            try
            {
                // r.ApiKey     = client_id
                // r.ApiSecret  = client_secret
                // For Pathao we also NEED username/password + store_id.
                // ✅ You must pass those via r.Note?? NO. Add them properly or extend request.
                // Since you are fetching CompanyCarrierCredentialDto, do this cleanly:
                // Put username/password/store_id in the request before calling this client.
                // Below we assume you extended CourierShipmentRequest with:
                //   string ApiUsername, string ApiPassword, int? StoreId, int? PackageWeightGrams
                // If you didn't extend, you MUST.

                if (string.IsNullOrWhiteSpace(r.ApiBaseUrl))
                    return new CourierShipmentResult { Success = false, ErrorMessage = "Pathao base URL missing." };

                string apiUsername = r.ApiUsername;
                string apiPassword = r.ApiPassword;
                int? storeId = r.StoreId;
                int? pkgGrams = r.PackageWeightGrams;

                if (string.IsNullOrWhiteSpace(r.ApiKey))
                    return new CourierShipmentResult { Success = false, ErrorMessage = "Pathao client_id missing." };
                if (string.IsNullOrWhiteSpace(r.ApiSecret))
                    return new CourierShipmentResult { Success = false, ErrorMessage = "Pathao client_secret missing." };
                if (string.IsNullOrWhiteSpace(apiUsername))
                    return new CourierShipmentResult { Success = false, ErrorMessage = "Pathao username missing." };
                if (string.IsNullOrWhiteSpace(apiPassword))
                    return new CourierShipmentResult { Success = false, ErrorMessage = "Pathao password missing." };
                if (!storeId.HasValue || storeId.Value <= 0)
                    return new CourierShipmentResult { Success = false, ErrorMessage = "Pathao store_id missing." };

                var phone11 = CleanBdPhone11(r.RecipientPhone);
                if (phone11 == null)
                {
                    return new CourierShipmentResult
                    {
                        Success = false,
                        ErrorMessage = $"Recipient phone must be BD 11-digit (01XXXXXXXXX). Got: '{r.RecipientPhone}'"
                    };
                }

                // Weight
                decimal weightKg = 0.5m;
                if (pkgGrams.HasValue && pkgGrams.Value > 0)
                    weightKg = Math.Round(pkgGrams.Value / 1000m, 3);

                // 1) Issue token
                var accessToken = await IssueTokenAsync(r.ApiBaseUrl, r.ApiKey, r.ApiSecret, apiUsername, apiPassword);

                // 2) Create order
                var baseUrl = CleanBaseUrl(r.ApiBaseUrl);
                var url = $"{baseUrl}/aladdin/api/v1/orders"; // ✅ from doc
                System.Diagnostics.Debug.WriteLine($"[Pathao] CreateOrder URL={url}, store_id={storeId}, invoice={r.Invoice}, phone={phone11}, cod={r.CodAmount}, qty={r.TotalItem}, weightKg={weightKg}");

                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // ✅ fields from doc example
                var payload = new
                {
                    store_id = storeId.Value,
                    merchant_order_id = r.Invoice,
                    recipient_name = r.RecipientName,
                    recipient_phone = phone11,
                    recipient_address = r.RecipientAddress,

                    // Reasonable defaults (you can later make these configurable in UI)
                    delivery_type = 48,
                    item_type = 2,

                    special_instruction = r.Note ?? "",
                    item_quantity = r.TotalItem,
                    item_weight = weightKg.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    item_description = r.ItemDescription ?? "Order items",
                    amount_to_collect = r.CodAmount
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine($"[Pathao] CreateOrder POST {url} invoice={r.Invoice}");

                var resp = await http.PostAsync(url, content);
                var body = await resp.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"[Pathao] CreateOrder Status={(int)resp.StatusCode} {resp.ReasonPhrase} Body={Shorten(body)}");



                if (!resp.IsSuccessStatusCode)
                {
                    return new CourierShipmentResult
                    {
                        Success = false,
                        RawResponse = body,
                        ErrorMessage = $"Pathao order failed: URL={url} :: {(int)resp.StatusCode} {resp.ReasonPhrase} :: {body}"
                    };
                }

                // Pathao response format differs by version; safest: return body and let you map tracking fields after you see it.
                // If your doc shows tracking/consignment fields later, we’ll parse exactly.
                return new CourierShipmentResult
                {
                    Success = true,
                    TrackingNumber = "",
                    ConsignmentId = "",
                    RawResponse = body
                };
            }
            catch (Exception ex)
            {
                return new CourierShipmentResult
                {
                    Success = false,
                    RawResponse = "",
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
