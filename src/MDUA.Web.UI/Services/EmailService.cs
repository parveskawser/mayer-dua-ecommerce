 #region SMTP gmail Service using MailKit
//
// using MailKit.Net.Smtp;
// using MailKit.Security;
// using MDUA.Entities;
// using MDUA.Facade.Interface;
// using Microsoft.Extensions.Configuration;
// using MimeKit;
// using System;
// using System.Net;
// using System.Net.Mail;
// using System.Threading.Tasks;
// using SmtpClient = MailKit.Net.Smtp.SmtpClient;
//
// namespace MDUA.Web.UI.Services
// {
//     /// <summary>
//     /// Safe Email Service using MailKit
//     /// - Never throws in constructor
//     /// - Fails gracefully if config is missing
//     /// - Designed for background / fire-and-forget usage
//     /// </summary>
//     public class EmailService : IEmailService
//     {
//         private readonly string _smtpHost;
//         private readonly int _smtpPort;
//         private readonly string _smtpUser;
//         private readonly string _smtpPassword;
//         private readonly string _fromEmail;
//         private readonly string _fromName;
//         private readonly bool _isConfigured;
//
//         public EmailService(IConfiguration configuration)
//         {
//             if (configuration == null)
//                 return; // Service stays disabled, app continues
//
//             _smtpHost = configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
//
//             _smtpPort = int.TryParse(configuration["Email:SmtpPort"], out var port)
//                 ? port
//                 : 587;
//
//             _smtpUser = configuration["Email:SmtpUser"];
//             _smtpPassword = configuration["Email:SmtpPassword"];
//             _fromEmail = configuration["Email:FromEmail"] ?? _smtpUser;
//             _fromName = configuration["Email:FromName"] ?? "MDUA System";
//
//             _isConfigured =
//                 !string.IsNullOrWhiteSpace(_smtpUser) &&
//                 !string.IsNullOrWhiteSpace(_smtpPassword);
//         }
//
//         public async Task<bool> SendEmailAsync(
//             string toEmail,
//             string subject,
//             string body,
//             bool isHtml = true)
//         {
//             var result = await SendEmailWithResultAsync(toEmail, subject, body, isHtml);
//             return result.Success;
//         }
//
//         public async Task<EmailResult> SendEmailWithResultAsync(
//             string toEmail,
//             string subject,
//             string body,
//             bool isHtml = true)
//         {
//             // ✅ Hard stop – no exception
//             if (!_isConfigured)
//             {
//                 return new EmailResult
//                 {
//                     Success = false,
//                     Message = "Email service is not configured"
//                 };
//             }
//
//             if (string.IsNullOrWhiteSpace(toEmail))
//             {
//                 return new EmailResult
//                 {
//                     Success = false,
//                     Message = "Recipient email address is required"
//                 };
//             }
//
//             try
//             {
//                 var message = new MimeMessage();
//
//                 message.From.Add(new MailboxAddress(_fromName, _fromEmail));
//                 message.To.Add(MailboxAddress.Parse(toEmail));
//                 message.Subject = subject ?? string.Empty;
//
//                 var bodyBuilder = new BodyBuilder();
//
//                 if (isHtml)
//                     bodyBuilder.HtmlBody = body;
//                 else
//                     bodyBuilder.TextBody = body;
//
//                 message.Body = bodyBuilder.ToMessageBody();
//
//                 using (var client = new SmtpClient())
//                 {
//                     await client.ConnectAsync(
//                         _smtpHost,
//                         _smtpPort,
//                         SecureSocketOptions.StartTls
//                     );
//
//                     await client.AuthenticateAsync(_smtpUser, _smtpPassword);
//
//                     var messageId = await client.SendAsync(message);
//
//                     await client.DisconnectAsync(true);
//
//                     return new EmailResult
//                     {
//                         Success = true,
//                         Message = "Email sent successfully",
//                         MessageId = messageId
//                     };
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"[EmailService] {ex.Message}");
//
//                 return new EmailResult
//                 {
//                     Success = false,
//                     Message = ex.Message
//                 };
//             }
//         }
//     }
// }
 #endregion






using MDUA.Entities;
using MDUA.Facade.Interface;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MDUA.Web.UI.Services
{
    /// <summary>
    /// High-Performance Email Service using SendGrid Web API
    /// - Uses HTTP instead of SMTP (Faster, firewall-friendly)
    /// - Requires 'SendGrid' NuGet package
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly bool _isConfigured;

        public EmailService(IConfiguration configuration)
        {
            // 1. Load Configuration
            _apiKey = configuration["SendGrid:ApiKey"];
            // --- ADD THESE DEBUG LINES ---
            if (string.IsNullOrEmpty(_apiKey))
            {
                Console.WriteLine("❌ ERROR: SendGrid API Key was NULL! Check .env spelling.");
            }
            else
            {
                Console.WriteLine($"✅ SUCCESS: Loaded SendGrid Key (Starts with: {_apiKey.Substring(0, 4)}...)");
            }
            //

            // Fallback to a default name if not provided
            _fromEmail = configuration["Email:FromEmail"] ?? "sabbirboi@gmail.com";
            _fromName = configuration["Email:FromName"] ?? "MDUA Shop";

            // 2. Safety Check
            _isConfigured = !string.IsNullOrWhiteSpace(_apiKey);
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            var result = await SendEmailWithResultAsync(toEmail, subject, body, isHtml);
            return result.Success;
        }

        public async Task<EmailResult> SendEmailWithResultAsync(
            string toEmail,
            string subject,
            string body,
            bool isHtml = true)
        {
            if (!_isConfigured)
            {
                Console.WriteLine("[SendGrid] ⚠️ Skipped: API Key missing.");
                return new EmailResult { Success = false, Message = "SendGrid API Key is missing" };
            }

            if (string.IsNullOrWhiteSpace(toEmail))
            {
                return new EmailResult { Success = false, Message = "Recipient email required" };
            }

            try
            {
                // 1. Create the Client
                var client = new SendGridClient(_apiKey);

                // 2. Construct the Message
                var from = new EmailAddress(_fromEmail, _fromName);
                var to = new EmailAddress(toEmail);

                var msg = MailHelper.CreateSingleEmail(
                    from,
                    to,
                    subject,
                    isHtml ? null : body,
                    isHtml ? body : null
                );

                // 3. Fire the Request (HTTP POST)
                var response = await client.SendEmailAsync(msg);

                // 4. Check Result (SendGrid returns 202 Accepted when successful)
                bool isSuccess = response.StatusCode == HttpStatusCode.OK ||
                                 response.StatusCode == HttpStatusCode.Accepted;

                if (isSuccess)
                {
                    return new EmailResult
                    {
                        Success = true,
                        Message = "Queued via SendGrid API",
                        MessageId = response.Headers.GetValues("X-Message-Id")?.ToString()
                    };
                }
                else
                {
                    var errorBody = await response.Body.ReadAsStringAsync();
                    Console.WriteLine($"[SendGrid Error] Status: {response.StatusCode}, Body: {errorBody}");

                    return new EmailResult
                    {
                        Success = false,
                        Message = $"SendGrid Failed: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailService Exception] {ex.Message}");
                return new EmailResult { Success = false, Message = ex.Message };
            }
        }
    }
}