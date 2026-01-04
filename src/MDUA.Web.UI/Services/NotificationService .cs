using MDUA.Facade.Interface;
using MDUA.Entities;
using System;
using System.Threading.Tasks;

namespace MDUA.Web.UI.Services
{
    /// <summary>
    /// Unified notification service with intelligent fallback
    /// Strategy:
    /// 1. Try EMAIL first (FREE) if email is provided
    /// 2. Fall back to SMS (COSTS MONEY) if email fails or not provided
    /// 3. Return detailed result showing what worked
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        public NotificationService(
            IEmailService emailService,
            ISmsService smsService)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
        }

        public async Task<NotificationResult> SendOrderConfirmationAsync(
            string customerName,
            string customerPhone,
            string customerEmail,
            string orderNumber,
            int quantity,
            decimal totalAmount)
        {
            var result = new NotificationResult
            {
                EmailSent = false,
                SmsSent = false
            };

            // ===================================================================
            // STRATEGY 1: Try Email First (FREE)
            // ===================================================================
            if (!string.IsNullOrWhiteSpace(customerEmail) && IsValidEmail(customerEmail))
            {
                try
                {
                    string emailSubject = $"Order Confirmation - {orderNumber}";
                    string emailBody = GenerateOrderEmailHtml(
                        customerName,
                        orderNumber,
                        quantity,
                        totalAmount
                    );

                    var emailResult = await _emailService.SendEmailWithResultAsync(
                        customerEmail,
                        emailSubject,
                        emailBody,
                        isHtml: true
                    );

                    result.EmailSent = emailResult.Success;
                    result.EmailMessage = emailResult.Message;

                    // If email succeeded, THEN done! No need for SMS
                    if (emailResult.Success)
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("========== Notification Error ==========");
                    Console.WriteLine(ex);                 // includes stack trace
                    Console.WriteLine(ex.InnerException);   // if any
                    Console.WriteLine("========================================");
                }

            }
            else
            {
                result.EmailMessage = "No valid email provided";
            }

            // ===================================================================
            // STRATEGY 2: Fallback to SMS 
            // ===================================================================
            if (!string.IsNullOrWhiteSpace(customerPhone))
            {
                try
                {
                    string smsMessage = GenerateOrderSms(orderNumber, quantity, totalAmount);

                    result.SmsSent = await _smsService.SendSmsAsync(customerPhone, smsMessage);
                    result.SmsMessage = result.SmsSent
                        ? "SMS sent successfully"
                        : "SMS sending failed";
                }
                catch (Exception ex)
                {
                    Console.WriteLine("========== Notification Error ==========");
                    Console.WriteLine(ex);                 // includes stack trace
                    Console.WriteLine(ex.InnerException);   // if any
                    Console.WriteLine("========================================");
                }

            }
            else
            {
                result.SmsMessage = "No phone number provided";
            }

            return result;
        }
        //FALLBACK TO EMAIL HTML TEMPLATE
        private string GenerateOrderEmailHtml(
            string customerName,
            string orderNumber,
            int quantity,
            decimal totalAmount)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .header {{
            background-color: #4CAF50;
            color: white;
            padding: 20px;
            text-align: center;
            border-radius: 5px 5px 0 0;
        }}
        .content {{
            background-color: #f9f9f9;
            padding: 20px;
            border: 1px solid #ddd;
            border-radius: 0 0 5px 5px;
        }}
        .order-details {{
            background-color: white;
            padding: 15px;
            margin: 15px 0;
            border-left: 4px solid #4CAF50;
        }}
        .footer {{
            text-align: center;
            margin-top: 20px;
            color: #777;
            font-size: 12px;
        }}
        .amount {{
            font-size: 24px;
            color: #4CAF50;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class=""header"">
        <h1>Order Confirmed! 🎉</h1>
    </div>
    <div class=""content"">
        <p>Dear {customerName},</p>
        <p>Thank you for your order! Your order has been successfully confirmed.</p>
        
        <div class=""order-details"">
            <h3>Order Details</h3>
            <p><strong>Order Number:</strong> {orderNumber}</p>
            <p><strong>Quantity:</strong> {quantity} item(s)</p>
            <p><strong>Total Amount:</strong> <span class=""amount"">{totalAmount:N2} Tk</span></p>
        </div>

        <p>We'll notify you once your order is shipped. You can track your order status using the order number above.</p>
        
        <p>If you have any questions, feel free to contact our support team.</p>

        <p>Best regards,<br><strong>MDUA Team</strong></p>
    </div>
    <div class=""footer"">
        <p>This is an automated email. Please do not reply to this message.</p>
        <p>&copy; {DateTime.Now.Year} MDUA. All rights reserved.</p>
    </div>
</body>
</html>";
        }
        public async Task<bool> SendSmsOnlyAsync(string phone, string message)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;

            try
            {
                // Delegates directly to the low-level SMS service
                return await _smsService.SendSmsAsync(phone, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SMS Error] {ex.Message}");
                return false;
            }
        }

        private string GenerateOrderSms(string orderNumber, int quantity, decimal totalAmount)
        {
            return $"Order {orderNumber} confirmed. Qty: {quantity}, Total: {totalAmount:N2} Tk. Thank you for shopping with MDUA!";
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}