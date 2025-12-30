using MDUA.Entities;
using MDUA.Entities.List;
using System;
using System.Collections.Generic;

namespace MDUA.Facade.Interface
{
    public interface IDeliveryStatusLogFacade
    {
        long Insert(DeliveryStatusLog obj);
        long Update(DeliveryStatusLog obj);
        long Delete(int id);
        DeliveryStatusLog Get(int id);
        DeliveryStatusLogList GetAll();

        // Reporting Logic
        List<DeliveryStatusLog> GetLogsForReport(DateTime? from, DateTime? to, string search, string entityType);

        // Helper to Log easily from other Facades
        void LogStatusChange(int entityId, string entityType, string oldStatus, string newStatus, string changedBy, int? orderId = null, string reason = null, bool isSystem = false);
    }
}