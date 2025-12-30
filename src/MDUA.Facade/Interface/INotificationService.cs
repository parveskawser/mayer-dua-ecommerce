using MDUA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDUA.Facade.Interface
{
    public interface INotificationService
    {
        Task<NotificationResult> SendOrderConfirmationAsync(
            string customerName,
            string customerPhone,
            string customerEmail,
            string orderNumber,
            int quantity,
            decimal totalAmount
        );
        Task<bool> SendSmsOnlyAsync(string phone, string message);
    }
}