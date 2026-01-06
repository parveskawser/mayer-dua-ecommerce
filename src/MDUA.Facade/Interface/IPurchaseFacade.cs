using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MDUA.Entities.BulkPurchaseOrder;

namespace MDUA.Facade.Interface
{
    public interface IPurchaseFacade : ICommonFacade<PoRequested,   PoRequestedList, PoRequestedBase>
    {
        List<dynamic> GetInventoryStatus(int companyId);

        long CreatePurchaseOrder(PoRequested po);
        List<Vendor> GetAllVendors(int companyId);
        dynamic GetPendingRequestInfo(int variantId);
        void ReceiveStock(int poReqId, int qty, decimal price, string invoice, string remarks,decimal paidAmount = 0, int? paymentMethodId = null, string paymentRef = null); object GetVariantStatus(int variantId);
        
        List<dynamic> GetInventorySortedByStockAsc(int companyId);
        void CreateBulkOrder(BulkPurchaseOrder bulkOrder, List<PoRequested> items);
        List<BulkPurchaseOrder> GetBulkOrdersReceivedList(int companyId);
        PoReceivedList GetAllReceived();




        // Return the specific type, NOT dynamic
        List<BulkOrderItemViewModel> GetBulkOrderItems(int bulkOrderId);
        void RejectPurchaseOrder(int poRequestId);
    }
}
