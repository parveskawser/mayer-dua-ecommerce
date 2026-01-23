using MDUA.Facade.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web; 
namespace MDUA.Facade
{
    public class SmsService : ISmsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _senderId;

        // ✅ CORRECT URL for BulkSMSBD
        private const string BaseUrl = "http://bulksmsbd.net/api/smsapi";

        public SmsService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;

            // Load new keys
            _apiKey = config["BulkSms_ApiKey"];
            _senderId = config["BulkSms_SenderId"];

            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_senderId))
            {
                Console.WriteLine("⚠️ Warning: BulkSMSBD Config missing (ApiKey or SenderId).");
            }
        }

        public async Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                // 1. Format Number (88017...)
                string formattedNumber = FormatBangladeshiNumber(phoneNumber);

             
                string url = $"{BaseUrl}?api_key={_apiKey}&type=text&number={formattedNumber}&senderid={_senderId}&message={Uri.EscapeDataString(message)}";

                Console.WriteLine($"[SMS Request] Sending to: {formattedNumber}");

                // 4. Send Request
                var response = await _httpClient.GetAsync(url);
                string responseBody = await response.Content.ReadAsStringAsync();

          
                Console.WriteLine($"[SMS Response] Code: {(int)response.StatusCode} | Body: {responseBody}");

                bool isSuccess = response.IsSuccessStatusCode &&
                                 (responseBody.Contains("success", StringComparison.OrdinalIgnoreCase) ||
                                  responseBody.Contains("1005"));

                return isSuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SMS Exception] {ex.Message}");
                return false;
            }
        }
        public async Task<SmsResult> SendSmsWithResultAsync(string phoneNumber, string message)
        {
            try
            {
                if (string.IsNullOrEmpty(_apiKey))
                {
                    return new SmsResult { Success = false, Message = "API Key missing" };
                }

                string formattedNumber = FormatBangladeshiNumber(phoneNumber);

                // Build URL
                var builder = new UriBuilder(BaseUrl);
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["api_key"] = _apiKey;
                query["type"] = "text";
                query["number"] = formattedNumber;
                query["senderid"] = _senderId;
                query["message"] = message;
                builder.Query = query.ToString();

                // Send
                var response = await _httpClient.GetAsync(builder.ToString());
                string responseBody = await response.Content.ReadAsStringAsync();

                // BulkSMSBD Response Logic:

                bool isSuccess = response.IsSuccessStatusCode &&
                                 (responseBody.Contains("Success", StringComparison.OrdinalIgnoreCase) ||
                                  responseBody.Contains("1005") || //  success code
                                  responseBody.Contains("202"));

                return new SmsResult
                {
                    Success = isSuccess,
                    Message = isSuccess ? "SMS Sent Successfully" : $"Provider Error: {responseBody}"
                };
            }
            catch (Exception ex)
            {
                return new SmsResult
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }
        public class SmsResult

        {
            public bool Success { get; set; }
            public string Message
            {
                get; set;
            }
        }
            // Helper to ensure format is 88017XXXXXXXX (No +)
            private string FormatBangladeshiNumber(string phoneNumber)
            {
                // Remove common chars
                string clean = phoneNumber.Replace(" ", "").Replace("-", "").Replace("+", "");

                // Ensure it starts with 88
                if (clean.StartsWith("01"))
                {
                    return "88" + clean;
                }
                if (clean.StartsWith("8801"))
                {
                    return clean;
                }

                return clean; // Fallback
            }
        }
    }
