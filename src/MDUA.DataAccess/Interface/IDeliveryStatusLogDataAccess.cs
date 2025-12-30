using MDUA.Entities;
using MDUA.Entities.Bases; // Needed for DeliveryStatusLogBase
using MDUA.Entities.List;  // Needed for DeliveryStatusLogList
using System;
using System.Collections.Generic;

namespace MDUA.DataAccess.Interface
{
    public interface IDeliveryStatusLogDataAccess
    {
        
        List<DeliveryStatusLog> GetLogsForReport(DateTime? from, DateTime? to, string search, string entityType);

     

        // Basic CRUD
        long Insert(DeliveryStatusLogBase deliveryStatusLogObject);
        long Update(DeliveryStatusLogBase deliveryStatusLogObject);
        long Delete(int id);
        DeliveryStatusLog Get(int id);
        DeliveryStatusLogList GetAll();

  
        DeliveryStatusLogList GetBySalesOrderId(int? salesOrderId);
        // DeliveryStatusLogList GetPaged(PagedRequest request);
        // DeliveryStatusLogList GetByQuery(string query);
        // int GetMaxId();
        // int GetRowCount();
    }
}