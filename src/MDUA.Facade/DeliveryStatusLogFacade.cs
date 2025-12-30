using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using System;
using System.Collections.Generic;

namespace MDUA.Facade
{
    public class DeliveryStatusLogFacade : IDeliveryStatusLogFacade
    {
        private readonly IDeliveryStatusLogDataAccess _dataAccess;

        public DeliveryStatusLogFacade(IDeliveryStatusLogDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public long Delete(int id) => _dataAccess.Delete(id);
        public DeliveryStatusLog Get(int id) => _dataAccess.Get(id);
        public DeliveryStatusLogList GetAll() => _dataAccess.GetAll();
        public long Insert(DeliveryStatusLog obj) => _dataAccess.Insert(obj);
        public long Update(DeliveryStatusLog obj) => _dataAccess.Update(obj);

        public List<DeliveryStatusLog> GetLogsForReport(DateTime? from, DateTime? to, string search, string entityType)
        {
            return _dataAccess.GetLogsForReport(from, to, search, entityType);
        }

        // Helper Method used by other Facades (OrderFacade, etc.)
        public void LogStatusChange(int entityId, string entityType, string oldStatus, string newStatus, string changedBy, int? orderId = null, string reason = null, bool isSystem = false)
        {
            if (string.Equals(oldStatus, newStatus, StringComparison.OrdinalIgnoreCase))
                return; // Optimization: Don't log if status is identical

            var log = new DeliveryStatusLog
            {
                EntityId = entityId,
                EntityType = entityType,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                ChangedBy = changedBy,
                SalesOrderId = orderId ?? (entityType == "SalesOrderHeader" ? entityId : (int?)null),
                ChangeReason = reason,
                IsSystemGenerated = isSystem,
                ChangedAt = DateTime.UtcNow // C# time is used if DB default is overridden by Insert param
            };

            _dataAccess.Insert(log);
        }
    }
}