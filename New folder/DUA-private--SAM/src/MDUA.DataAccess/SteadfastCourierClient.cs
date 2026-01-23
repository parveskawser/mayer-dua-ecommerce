using MDUA.DataAccess.Interface;
using MDUA.Entities;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MDUA.DataAccess
{
    public class SteadfastCourierClient : ICourierClient
    {
        public string CarrierName => "Steadfast";

        private static string MaskLast4(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "NULL";
            s = s.Trim();
            if (s.Length <= 4) return "****";
            return new string('*', s.Length - 4) + s.Substring(s.Length - 4);
        }

        private static string Shorten(string s, int max = 1200)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.Length <= max ? s : s.Substring(0, max) + "...(truncated)";
        }

        private static string CleanBaseUrl(string s)
        {
            // removes \r\n and trims and strips trailing /
            var baseUrl = (s ?? "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Trim()
                .TrimEnd('/');

            return baseUrl;
        }

        public async Task<CourierShipmentResult> CreateShipmentAsync(CourierShipmentRequest r)
        {
            // 1) Clean URL
            var baseUrl = CleanBaseUrl(r.ApiBaseUrl);
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return new CourierShipmentResult
                {
                    Success = false,
                    RawResponse = "",
                    ErrorMessage = "Courier API endpoint is missing."
                };
            }

            // IMPORTANT:
            // If baseUrl is already ".../api/v1", create_order is usually ".../create_order"
            // so final becomes ".../api/v1/create_order"
            var url = baseUrl + "/create_order";

            System.Diagnostics.Debug.WriteLine($"[STEADFAST] BaseUrlRaw=[{r.ApiBaseUrl ?? ""}]");
            System.Diagnostics.Debug.WriteLine($"[STEADFAST] BaseUrlClean=[{baseUrl}]");
            System.Diagnostics.Debug.WriteLine($"[STEADFAST] FinalUrl=[{url}]");
            System.Diagnostics.Debug.WriteLine($"[STEADFAST] ApiKey={MaskLast4(r.ApiKey)} Secret={MaskLast4(r.ApiSecret)}");

            // 2) Sanitize phone
            var phoneRaw = r.RecipientPhone ?? "";
            var digits = System.Text.RegularExpressions.Regex.Replace(phoneRaw, @"\D", "");

            if (digits.Length == 13 && digits.StartsWith("880"))
                digits = "0" + digits.Substring(3);

            if (digits.Length == 13 && digits.StartsWith("88"))
                digits = "0" + digits.Substring(2);

            if (digits.Length != 11 || !digits.StartsWith("01"))
            {
                return new CourierShipmentResult
                {
                    Success = false,
                    RawResponse = "",
                    ErrorMessage = "Recipient phone must be BD 11-digit (01XXXXXXXXX)."
                };
            }

            // 3) Payload
            var payload = new
            {
                invoice = r.Invoice,
                recipient_name = r.RecipientName,
                recipient_phone = digits,
                recipient_address = r.RecipientAddress,
                cod_amount = r.CodAmount,
                total_lot = r.TotalItem,
                note = r.Note,
                item_description = r.ItemDescription
            };

            var json = JsonSerializer.Serialize(payload);
            System.Diagnostics.Debug.WriteLine($"[STEADFAST] Payload={Shorten(json)}");

            // 4) HTTP
            using var http = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            // ✅ Steadfast header variants in the wild:
            // Some accept "Api-Key"/"Secret-Key"
            // Some accept "api_key"/"secret_key"
            // Some accept "ApiKey"/"SecretKey"
            // We'll set all (server will use what it understands).
            if (!string.IsNullOrWhiteSpace(r.ApiKey))
            {
                http.DefaultRequestHeaders.TryAddWithoutValidation("Api-Key", r.ApiKey);
                http.DefaultRequestHeaders.TryAddWithoutValidation("api_key", r.ApiKey);
                http.DefaultRequestHeaders.TryAddWithoutValidation("ApiKey", r.ApiKey);
            }

            if (!string.IsNullOrWhiteSpace(r.ApiSecret))
            {
                http.DefaultRequestHeaders.TryAddWithoutValidation("Secret-Key", r.ApiSecret);
                http.DefaultRequestHeaders.TryAddWithoutValidation("secret_key", r.ApiSecret);
                http.DefaultRequestHeaders.TryAddWithoutValidation("SecretKey", r.ApiSecret);
            }

            // Also log which headers are present (names only)
            foreach (var h in http.DefaultRequestHeaders)
                System.Diagnostics.Debug.WriteLine($"[STEADFAST] HeaderSet: {h.Key}");

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            string body = "";

            try
            {
                response = await http.PostAsync(url, content);
                body = await response.Content.ReadAsStringAsync();
            }
            catch (TaskCanceledException)
            {
                return new CourierShipmentResult
                {
                    Success = false,
                    RawResponse = "",
                    ErrorMessage = "Courier request timed out."
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[STEADFAST] HTTP SEND FAILED: " + ex);
                return new CourierShipmentResult
                {
                    Success = false,
                    RawResponse = "",
                    ErrorMessage = "Courier request failed to send."
                };
            }

            System.Diagnostics.Debug.WriteLine($"[STEADFAST] Status={(int)response.StatusCode} {response.ReasonPhrase}");
            System.Diagnostics.Debug.WriteLine($"[STEADFAST] Body={Shorten(body)}");

            if (!response.IsSuccessStatusCode)
            {
                var msg = (body ?? "").Trim();

                // Steadfast sometimes returns plain text like: "Account is not active!"
                // If empty, fallback to status text.
                if (string.IsNullOrWhiteSpace(msg))
                    msg = $"Courier API returned {(int)response.StatusCode} {response.ReasonPhrase}.";

                // Optional: normalize message so your UI is consistent
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized &&
                    msg.IndexOf("not active", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    msg = "Steadfast API account is not active. Please activate API access from Steadfast support/portal.";
                }

                return new CourierShipmentResult
                {
                    Success = false,
                    RawResponse = body,
                    ErrorMessage = msg
                };
            }

            // 6) Parse JSON
            JsonDocument doc;
            try
            {
                doc = JsonDocument.Parse(body);
            }
            catch
            {
                return new CourierShipmentResult
                {
                    Success = false,
                    RawResponse = body,
                    ErrorMessage = "Courier returned invalid JSON."
                };
            }

            if (!doc.RootElement.TryGetProperty("consignment", out var consignment))
            {
                return new CourierShipmentResult
                {
                    Success = false,
                    RawResponse = body,
                    ErrorMessage = "Courier response format is invalid."
                };
            }

            string tracking = null;
            string consignmentId = null;

            if (consignment.TryGetProperty("tracking_code", out var trackingEl))
                tracking = trackingEl.GetString();

            if (consignment.TryGetProperty("consignment_id", out var consignmentEl))
                consignmentId = consignmentEl.ToString();

            if (string.IsNullOrWhiteSpace(tracking))
            {
                return new CourierShipmentResult
                {
                    Success = false,
                    RawResponse = body,
                    ErrorMessage = "Courier did not return a tracking number."
                };
            }

            return new CourierShipmentResult
            {
                Success = true,
                TrackingNumber = tracking,
                ConsignmentId = consignmentId ?? "",
                RawResponse = body
            };
        }
    }
}
