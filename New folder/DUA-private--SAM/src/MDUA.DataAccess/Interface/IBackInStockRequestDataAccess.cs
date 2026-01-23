using System.Collections.Generic;

using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess.Interface
{
    public interface IBackInStockRequestDataAccess : ICommonDataAccess<BackInStockRequest, BackInStockRequestList, BackInStockRequestBase>
    {
        BackInStockRequestList GetByProductVariantId(int productVariantId);
        BackInStockRequestList GetPendingByProductVariantId(int productVariantId);
        bool HasPendingRequest(int productVariantId, string contactNumber);
        int MarkNotified(IEnumerable<int> requestIds, string updatedBy);
        int GetPendingRequestCount(int companyId);
        List<BackInStockRequestSummary> GetPendingRequestSummaries(int companyId, int top = 20);
        List<BackInStockRequestDetail> GetPendingRequestDetails(int companyId);
    }
}
