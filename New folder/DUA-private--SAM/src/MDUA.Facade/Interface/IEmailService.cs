using MDUA.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MDUA.Facade.Interface
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
        Task<bool> SendEmail(Hashtable templateValue, string templateKey, List<Attachment> attachments = null);
        Task<EmailResult> SendEmailWithResultAsync(string toEmail, string subject, string body, bool isHtml = true);
    }
}
