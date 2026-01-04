using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

// MDUA Dependencies
using MDUA.DataAccess; 
using MDUA.Entities;
using MDUA.Facade.Interface;
using MDUA.Framework.Utils; // For EmailParser
using MDUA.DataAccess.Interface; // For IEmailHistoryDataAccess

// SendGrid Dependencies
using SendGrid;
using SendGrid.Helpers.Mail;

//  Explicit Alias to fix "Ambiguous Reference"
using SysNet = System.Net.Mail;

namespace MDUA.Facade
{
    public class EmailService : IEmailService
    {
        // Dependencies
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;

        // Data Access Layers
        private readonly IEmailTemplateDataAccess _emailTemplateDataAccess;
        private readonly IEmailHistoryDataAccess _emailHistoryDataAccess;

        // Configuration State
        private readonly string _defaultFromEmail;
        private readonly string _defaultFromName;
        private readonly bool _isConfigured;

        public EmailService(
            IConfiguration configuration,
            IEmailTemplateDataAccess emailTemplateDataAccess,
            IEmailHistoryDataAccess emailHistoryDataAccess)
        {
            _configuration = configuration;
            _emailTemplateDataAccess = emailTemplateDataAccess;
            _emailHistoryDataAccess = emailHistoryDataAccess;

            // 1. Load SendGrid Config
            _apiKey = _configuration["SendGrid:ApiKey"];
            _defaultFromEmail = _configuration["Email:FromEmail"] ?? "noreply@mdua.com";
            _defaultFromName = _configuration["Email:FromName"] ?? "MDUA Shop";

            // 2. Safety Check
            _isConfigured = !string.IsNullOrWhiteSpace(_apiKey);

            if (!_isConfigured) Console.WriteLine("⚠️ [EmailService] SendGrid API Key Missing!");
            else Console.WriteLine($"✅ [EmailService] Ready (Key: {_apiKey.Substring(0, 4)}...)");
        }

        // =================================================================================
        // METHOD A: The "Raw" Sender (Used by  OrderFacade currently)
        // =================================================================================
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            if (!_isConfigured) return false;

            try
            {
                var client = new SendGridClient(_apiKey);
                var from = new EmailAddress(_defaultFromEmail, _defaultFromName);
                var to = new EmailAddress(toEmail);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, isHtml ? null : body, isHtml ? body : null);

                var response = await client.SendEmailAsync(msg);
                return response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email Error] {ex.Message}");
                return false;
            }
        }
        public async Task<EmailResult> SendEmailWithResultAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            if (!_isConfigured)
                return new EmailResult { Success = false, Message = "Service not configured" };

            try
            {
                var client = new SendGridClient(_apiKey);
                var from = new EmailAddress(_defaultFromEmail, _defaultFromName);
                var to = new EmailAddress(toEmail);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, isHtml ? null : body, isHtml ? body : null);

                var response = await client.SendEmailAsync(msg);
                bool success = response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted;

                if (success)
                {
                    return new EmailResult
                    {
                        Success = true,
                        Message = "Sent via SendGrid",
                        MessageId = response.Headers.GetValues("X-Message-Id")?.FirstOrDefault()
                    };
                }
                else
                {
                    var errorBody = await response.Body.ReadAsStringAsync();
                    return new EmailResult { Success = false, Message = $"SendGrid Error: {response.StatusCode} - {errorBody}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email Error] {ex.Message}");
                return new EmailResult { Success = false, Message = ex.Message };
            }
        }
        // =================================================================================
        // METHOD B: Hybrid Method (Database Templates + SendGrid + History)
        // =================================================================================
        // Use the Alias 'SysNet.Attachment' to explicitly tell C# we mean the Legacy Attachment
        public async Task<bool> SendEmail(Hashtable templateValue, string templateKey, List<SysNet.Attachment> attachments = null)
        {
            Console.WriteLine($"\n🔍 [Email Debug] Starting Send process for Template: '{templateKey}'");

            if (!_isConfigured)
            {
                Console.WriteLine("❌ [Email Debug] Error: Service not configured (API Key missing).");
                return false;
            }

            // 1. Get Template from DB
            var emailTemplate = _emailTemplateDataAccess.GetByQuery($"TemplateKey='{templateKey}'").FirstOrDefault();

            if (emailTemplate == null)
            {
                Console.WriteLine($"❌ [Email Debug] Template NOT FOUND in Database: {templateKey}");
                return false;
            }

            // 2. Parse Body & Subject
            EmailParser bodyParser;
            if (!string.IsNullOrWhiteSpace(emailTemplate.BodyContent))
            {
                bodyParser = new EmailParser(emailTemplate.BodyContent, templateValue, false);
            }
            else
            {
                string filePath = Path.Combine("wwwroot", emailTemplate.BodyFile ?? "");
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"❌ [Email Debug] Body file not found: {filePath}");
                    return false;
                }
                bodyParser = new EmailParser(filePath, templateValue, true);
            }

            string finalBody = bodyParser.Parse();
            string finalSubject = ParseString(emailTemplate.Subject, templateValue);

            Console.WriteLine($"   [Email Debug] Subject Resolved: '{finalSubject}'");

            // 3. Prepare SendGrid Message
            var msg = new SendGridMessage();
            msg.SetSubject(finalSubject);
            msg.HtmlContent = finalBody;

            // 4. Resolve "FROM" Address
            string fromAddressRaw = ParseString(emailTemplate.FromEmail, templateValue);
            string fromNameRaw = ParseString(emailTemplate.FromName, templateValue);

            // Debugging From Address
            Console.WriteLine($"   [Email Debug] From (DB): '{emailTemplate.FromEmail}' -> Resolved: '{fromAddressRaw}'");

            if (string.IsNullOrWhiteSpace(fromAddressRaw)) fromAddressRaw = _defaultFromEmail;
            if (string.IsNullOrWhiteSpace(fromNameRaw)) fromNameRaw = _defaultFromName;

            msg.SetFrom(new EmailAddress(fromAddressRaw, fromNameRaw));

            // 5. Resolve "TO" Address
            string toAddressRaw = ParseString(emailTemplate.ToEmail, templateValue);
            string finalToEmail = "";

            // Debugging To Address
            Console.WriteLine($"   [Email Debug] To (DB): '{emailTemplate.ToEmail}' -> Resolved: '{toAddressRaw}'");

            AddRecipients(msg, toAddressRaw, out finalToEmail);

            if (string.IsNullOrWhiteSpace(finalToEmail))
            {
                Console.WriteLine("❌ [Email Debug] CRITICAL ERROR: 'To' Address is EMPTY after parsing!");
                Console.WriteLine("   --> Check your Hashtable keys vs Database ##Placeholders##.");
                return false;
            }

            // 6. Handle CC / BCC
            string ccRaw = ParseString(emailTemplate.CcEmail, templateValue);
            if (!string.IsNullOrWhiteSpace(ccRaw)) AddCCs(msg, ccRaw);

            string bccRaw = ParseString(emailTemplate.BccEmail, templateValue);
            if (!string.IsNullOrWhiteSpace(bccRaw)) AddBCCs(msg, bccRaw);

            // 7. Handle Attachments
            if (attachments != null && attachments.Any())
            {
                Console.WriteLine($"   [Email Debug] Adding {attachments.Count} attachment(s).");
                foreach (var att in attachments)
                {
                    if (att.ContentStream != null)
                    {
                        using (var ms = new MemoryStream())
                        {
                            att.ContentStream.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            string base64 = Convert.ToBase64String(fileBytes);
                            msg.AddAttachment(att.Name, base64);
                        }
                    }
                }
            }

            // 8. SEND via SendGrid API
            try
            {
                Console.WriteLine("   [Email Debug] Sending to SendGrid API...");
                var client = new SendGridClient(_apiKey);
                var response = await client.SendEmailAsync(msg);

                bool success = response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted;

                if (success)
                {
                    Console.WriteLine($"✅ [Email Debug] SendGrid Success! Status: {response.StatusCode}");
                }
                else
                {
                    // 🚨 READ THE ERROR BODY FROM SENDGRID 🚨
                    // This is the most important part for debugging
                    var errorBody = await response.Body.ReadAsStringAsync();
                    Console.WriteLine($"❌ [Email Debug] SendGrid FAILED. Status: {response.StatusCode}");
                    Console.WriteLine($"   [Email Debug] ERROR DETAILS: {errorBody}");
                }

                // 9. Log to History
                LogHistory(templateKey, finalToEmail, fromAddressRaw, finalSubject, finalBody, success);

                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [Template Email Exception] {ex.Message}");
                if (ex.InnerException != null) Console.WriteLine($"   Inner: {ex.InnerException.Message}");

                LogHistory(templateKey, finalToEmail, fromAddressRaw, $"ERROR: {ex.Message}", finalBody, false);
                return false;
            }
        }
        // =================================================================================
        // HELPERS
        // =================================================================================

        private string ParseString(string input, Hashtable values)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";
            if (input.Contains("##"))
            {
                try { return new EmailParser(input, values, false).Parse(); }
                catch { return input; }
            }
            return input;
        }

        private void AddRecipients(SendGridMessage msg, string rawEmails, out string primaryEmail)
        {
            primaryEmail = "";
            if (string.IsNullOrWhiteSpace(rawEmails)) return;

            var emails = rawEmails.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var email in emails)
            {
                string cleanEmail = email.Trim();
                if (cleanEmail.Contains("@"))
                {
                    msg.AddTo(new EmailAddress(cleanEmail));
                    if (string.IsNullOrEmpty(primaryEmail)) primaryEmail = cleanEmail;
                }
            }
        }

        private void AddCCs(SendGridMessage msg, string rawEmails)
        {
            var emails = rawEmails.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var email in emails)
            {
                if (email.Contains("@")) msg.AddCc(new EmailAddress(email.Trim()));
            }
        }

        private void AddBCCs(SendGridMessage msg, string rawEmails)
        {
            var emails = rawEmails.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var email in emails)
            {
                if (email.Contains("@")) msg.AddBcc(new EmailAddress(email.Trim()));
            }
        }

        private void LogHistory(string key, string to, string from, string subject, string body, bool success)
        {
            try
            {
                var history = new EmailHistory
                {
                    TemplateKey = key,
                    ToEmail = to,
                    FromEmail = from,
                    EmailSubject = subject,
                    EmailBodyContent = body,
                    EmailSentDate = DateTime.UtcNow,
                    IsSystemAutoSent = true,
                    IsRead = false,
                    ReadCount = 0,
                    LastUpdatedDate = DateTime.UtcNow
                };

                if (!success) history.EmailSubject = "FAILED: " + subject;

                _emailHistoryDataAccess.Insert(history);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[History Log Failed] {ex.Message}");
            }
        }
    }
}