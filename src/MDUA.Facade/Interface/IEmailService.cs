using MDUA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDUA.Facade.Interface
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
        Task<EmailResult> SendEmailWithResultAsync(string toEmail, string subject, string body, bool isHtml = true);
    }
}
