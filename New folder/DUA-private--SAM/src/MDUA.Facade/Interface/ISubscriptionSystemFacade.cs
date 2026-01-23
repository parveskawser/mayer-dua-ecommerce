using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MDUA.Entities.SubscriptionPlan;

namespace MDUA.Facade.Interface
{
    public interface ISubscriptionSystemFacade
    {
        int CreatePlan(SubscriptionPlan plan);
        SubscriptionPlan GetPlan(int id);
        List<SubscriptionPlan> GetAllPlans();
        int SubscribeCompanyToPlan(CompanySubscription subscription);
        CompanySubscription GetCompanySubscription(int companyId);
        QuotaCheckResult CheckProductCapability(int companyId);
        QuotaCheckResult ProcessOrderUsage(int companyId);

        bool UpdatePlan(SubscriptionPlan plan);

        SubscriptionUsage GetCurrentUsage(int companyId);
        bool IsSubscriptionLocked(int companyId, out int currentUsage, out int limit);
        bool IsProductLocked(int companyId, out int currentCount, out int limit);
        List<CompanySubscription> GetAllCompanySubscriptions();
        int GetCurrentProductCount(int companyId);
        bool UpdateCompanySubscription(CompanySubscription subscription);
    }
}
