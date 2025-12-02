using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDUA.Facade.Interface
{
    public interface IPurchaseFacade : ICommonFacade<PoRequested,   PoRequestedList, PoRequestedBase>
    {
        List<dynamic> GetInventoryStatus();

        long CreatePurchaseOrder(PoRequested po);
        List<Vendor> GetAllVendors();
        dynamic GetPendingRequestInfo(int variantId);
        void ReceiveStock(int variantId, int qty, decimal price, string invoice, string remarks);
    }
}
