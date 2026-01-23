using MDUA.DataAccess.Interface;
using MDUA.Entities;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MDUA.DataAccess
{
    public class RedxCourierClient : ICourierClient
    {
        public string CarrierName => "RedX";

        public async Task<CourierShipmentResult> CreateShipmentAsync(CourierShipmentRequest r)
        {
            // API base from DB, typically something like https://api.redx.com.bd/v1/parcel
            var baseUrl = (r.ApiBaseUrl ?? "").Trim().TrimEnd('/');
            var url = $"{baseUrl}/order/create"; 

            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            http.DefaultRequestHeaders.Add("Authorization", $"Bearer {r.ApiKey}"); 

            var payload = new
            {
                merchant_invoice_id = r.Invoice,
                customer_name = r.RecipientName,
                customer_phone = r.RecipientPhone,
                customer_address = r.RecipientAddress,
                cash_collection_amount = r.CodAmount,
                parcel_weight = r.TotalItem,
                instruction = r.Note
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await http.PostAsync(url, content);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new CourierShipmentResult
                {
                    Success = false,
                    RawResponse = body,
                    ErrorMessage = $"RedX API failed ({(int)response.StatusCode}): {body}"
                };
            }

            var doc = JsonDocument.Parse(body);
            return new CourierShipmentResult
            {
                Success = true,
                TrackingNumber = doc.RootElement.GetProperty("tracking_id").GetString(),
                ConsignmentId = doc.RootElement.GetProperty("order_id").GetString(),
                RawResponse = body
            };
        }
    }
}
