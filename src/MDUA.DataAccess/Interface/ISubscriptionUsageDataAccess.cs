using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MDUA.Entities.SubscriptionPlan;

namespace MDUA.DataAccess.Interface
{
    public interface ISubscriptionUsageDataAccess : ICommonDataAccess<SubscriptionUsage, SubscriptionUsageList, SubscriptionUsageBase>
    {
        QuotaCheckResult TryConsumeOrderQuota(int companyId);
    }
}
