#region GREENWEB SMS Service Implementation 
//using MDUA.Facade.Interface;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Configuration;


//namespace MDUA.Web.UI.Services

//{
//    /// <summary>
//    /// GreenWeb BD Bulk SMS Service
//    /// Documentation: https://bdbulksms.net/bulk-sms-api-bd-english.php
//    /// </summary>
//    public class SmsService : ISmsService
//    {
//        private readonly HttpClient _httpClient;
//        private readonly string _apiToken;
//        private const string BASE_URL = "http://api.greenweb.com.bd/api.php";
//        // For JSON responses: http://api.greenweb.com.bd/api.php?json

//        public SmsService(HttpClient httpClient, IConfiguration config)
//        {
//            _httpClient = httpClient;
//            // ⚠️ Store token securely in appsettings.json or in env
//            // Generate token from: https://gwb.li/token
//            _apiToken = config["GreenWebSms:ApiToken"] ??
//                throw new InvalidOperationException("GreenWeb API token not configured");
//        }

//        public async Task<bool> SendSmsAsync(string phoneNumber, string message)
//        {
//            try
//            {
//                // Format phone number for Bangladesh
//                // GreenWeb accepts: +8801xxxxxxxxx or 01xxxxxxxxx
//                phoneNumber = FormatBangladeshiNumber(phoneNumber);

//                // Prepare form data (GreenWeb uses FormUrlEncoded for POST)
//                var formData = new FormUrlEncodedContent(new[]
//                {
//                    new KeyValuePair<string, string>("token", _apiToken),
//                    new KeyValuePair<string, string>("to", phoneNumber),
//                    new KeyValuePair<string, string>("message", message)
//                });

//                // Send request
//                var response = await _httpClient.PostAsync(BASE_URL, formData);
//                var result = await response.Content.ReadAsStringAsync();

//                // GreenWeb returns:
//                // Success: "Ok: SMS Sent Successfully To +8801xxxxxxxxx"
//                // Error: "Error: +8801xxxxxxxxx Invalid Number !"

//                return response.IsSuccessStatusCode &&
//                       result.Contains("Ok:", StringComparison.OrdinalIgnoreCase);
//            }
//            catch (Exception ex)
//            {
//                // Log error (use proper logging in production)
//                Console.WriteLine($"GreenWeb SMS Failed: {ex.Message}");
//                return false;
//            }
//        }

//        /// <summary>
//        /// Send SMS with JSON response for detailed error handling
//        /// </summary>
//        public async Task<SmsResult> SendSmsWithResultAsync(string phoneNumber, string message)
//        {
//            try
//            {
//                phoneNumber = FormatBangladeshiNumber(phoneNumber);

//                var formData = new FormUrlEncodedContent(new[]
//                {
//                    new KeyValuePair<string, string>("token", _apiToken),
//                    new KeyValuePair<string, string>("to", phoneNumber),
//                    new KeyValuePair<string, string>("message", message),
//                    new KeyValuePair<string, string>("json", "1") // Request JSON response
//                });

//                var response = await _httpClient.PostAsync(BASE_URL, formData);
//                var jsonResult = await response.Content.ReadAsStringAsync();

//                // Parse JSON response:
//                // [{"to":"+8801xxx","message":"test","status":"SENT","statusmsg":"SMS Sent Successfully"}]

//                if (response.IsSuccessStatusCode &&
//                    jsonResult.Contains("\"status\":\"SENT\"", StringComparison.OrdinalIgnoreCase))
//                {
//                    return new SmsResult { Success = true, Message = "SMS sent successfully" };
//                }

//                return new SmsResult
//                {
//                    Success = false,
//                    Message = jsonResult.Contains("Invalid Number")
//                        ? "Invalid phone number"
//                        : "Failed to send SMS"
//                };
//            }
//            catch (Exception ex)
//            {
//                return new SmsResult
//                {
//                    Success = false,
//                    Message = $"Error: {ex.Message}"
//                };
//            }
//        }

//        private string FormatBangladeshiNumber(string phoneNumber)
//        {
//            // Remove spaces and special characters
//            phoneNumber = phoneNumber.Trim().Replace(" ", "").Replace("-", "");

//            // GreenWeb accepts both formats:
//            // 1. +8801xxxxxxxxx (recommended)
//            // 2. 01xxxxxxxxx

//            // Remove +88 if present to normalize
//            if (phoneNumber.StartsWith("+88"))
//                phoneNumber = phoneNumber.Substring(3);
//            else if (phoneNumber.StartsWith("88"))
//                phoneNumber = phoneNumber.Substring(2);

//            // Remove leading 0 if present
//            if (phoneNumber.StartsWith("0"))
//                phoneNumber = phoneNumber.Substring(1);

//            // Add +88 prefix (GreenWeb standard)
//            return "+88" + phoneNumber;
//        }
//    }

//    public class SmsResult
//    {
//        public bool Success { get; set; }
//        public string Message { get; set; }
//    }
//}
#endregion

#region TEXTBEE SMS Service Implementation 

using MDUA.Facade.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MDUA.Web.UI.Services
{
    /// <summary>
    /// TextBee SMS Service (International SMS Gateway)
    /// for Bangladesh - use GreenWeb instead
    /// </summary>
    public class SmsService : ISmsService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        //should be in environment variable or secure storage
        // private const string API_KEY = "TEXTBEE_API_KEY";
        // private const string DEVICE_ID = "YOUR_DEVICE_ID";
        private const string BASE_URL = "https://api.textbee.dev/api/v1";
        public SmsService(HttpClient httpClient,IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                string apiKey = _configuration["TextBee:ApiKey"];
                string deviceId = _configuration["TextBee:DeviceId"];

                // Validation Check
                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(deviceId))
                {
                    return false;
                }

                // Phone Number Formatting
                if (phoneNumber.StartsWith("0"))
                    phoneNumber = "+88" + phoneNumber;
                if (!phoneNumber.StartsWith("+"))
                    phoneNumber = "+" + phoneNumber;

                var payload = new
                {
                    recipients = new[] { phoneNumber },
                    message = message
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);

                // Send Request
                var response = await _httpClient.PostAsync(
                    $"{BASE_URL}/gateway/devices/{deviceId}/send-sms",
                    content
                );

                return response.IsSuccessStatusCode;
            }
            catch
            {
                // Fail silently or handle via ILogger in future
                return false;
            }
        }
    }
}

#endregion