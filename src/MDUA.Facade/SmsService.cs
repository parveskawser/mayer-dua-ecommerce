using MDUA.Facade.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MDUA.Facade
{
    public class SmsService : ISmsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiToken;
        private const string BaseUrl = "http://api.greenweb.com.bd/api.php";

        public SmsService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            // Check if the config exists, and if not, set _apiToken to null or a default value
            _apiToken = config["GreenWebSms_ApiToken"]; // No exception thrown

            // If you want to log a message instead of throwing an exception, you can log it here:
            if (string.IsNullOrEmpty(_apiToken))
            {
                Console.WriteLine("Warning: GreenWeb API token not configured. SMS will not be sent.");
            }
        }
        public async Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                phoneNumber = FormatBangladeshiNumber(phoneNumber);

                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("token", _apiToken),
                    new KeyValuePair<string, string>("to", phoneNumber),
                    new KeyValuePair<string, string>("message", message)
                });

                var response = await _httpClient.PostAsync(BaseUrl, formData);
                var result = await response.Content.ReadAsStringAsync();

                return response.IsSuccessStatusCode &&
                       result.Contains("ok:", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greenweb SMS failed: {ex.Message}");
                return false;
            }
        }

        public async Task<SmsResult> SendSmsWithResultAsync(string phoneNumber, string message)
        {
            try
            {
                phoneNumber = FormatBangladeshiNumber(phoneNumber);

                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("token", _apiToken),
                    new KeyValuePair<string, string>("to", phoneNumber),
                    new KeyValuePair<string, string>("message", message),
                    new KeyValuePair<string, string>("json", "1") // request JSON response
                });

                var response = await _httpClient.PostAsync(BaseUrl, formData);
                var jsonResult = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode &&
                    jsonResult.Contains("\"status\":\"sent\"", StringComparison.OrdinalIgnoreCase))
                {
                    return new SmsResult { Success = true, Message = "SMS sent successfully" };
                }

                return new SmsResult
                {
                    Success = false,
                    Message = jsonResult.Contains("invalid number") ? "Invalid phone number" : "Failed to send SMS"
                };
            }
            catch (Exception ex)
            {
                return new SmsResult
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        private string FormatBangladeshiNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber.Trim().Replace(" ", "").Replace("-", "");

            if (phoneNumber.StartsWith("+88"))
                phoneNumber = phoneNumber.Substring(3);
            else if (phoneNumber.StartsWith("88"))
                phoneNumber = phoneNumber.Substring(2);

            if (phoneNumber.StartsWith("0"))
                phoneNumber = phoneNumber.Substring(1);

            return "+88" + phoneNumber;
        }
    }

    public class SmsResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
