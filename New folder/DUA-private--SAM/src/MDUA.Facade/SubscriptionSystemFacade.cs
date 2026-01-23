using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Facade.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using static MDUA.Entities.SubscriptionPlan;


namespace MDUA.Facade
{
    public class SubscriptionSystemFacade : ISubscriptionSystemFacade
    {
        // Dependencies
        private readonly ISubscriptionPlanDataAccess _planDa;
        private readonly ICompanySubscriptionDataAccess _subDa;
        private readonly ISubscriptionUsageDataAccess _usageDa;
        private readonly IProductDataAccess _productDa;
        public SubscriptionSystemFacade(
            ISubscriptionPlanDataAccess planDa,
            ICompanySubscriptionDataAccess subDa,
            ISubscriptionUsageDataAccess usageDa,
            IProductDataAccess productDa)
        {
            _planDa = planDa;
            _subDa = subDa;
            _usageDa = usageDa;
            _productDa = productDa;

        }

        #region Plan Management (Catalog)

        public int CreatePlan(SubscriptionPlan plan)
        {
            // Validations 
            if (plan.BasePrice < 0) throw new ArgumentException("Price cannot be negative");

            return (int)((SubscriptionPlanDataAccess)_planDa).Insert(plan);
        }

        public bool UpdatePlan(SubscriptionPlan plan)
        {
            if (plan.BasePrice < 0) throw new ArgumentException("Price cannot be negative");

            // Returns true if rows affected > 0
            return ((SubscriptionPlanDataAccess)_planDa).Update(plan) > 0;
        }
        public SubscriptionPlan GetPlan(int id)
        {
            return ((SubscriptionPlanDataAccess)_planDa).Get(id);
        }

        public List<SubscriptionPlan> GetAllPlans()
        {
            return ((SubscriptionPlanDataAccess)_planDa).GetAll();
        }

        #endregion

        #region Company Subscription (Contract)

        public List<CompanySubscription> GetAllCompanySubscriptions()
        {
            return ((CompanySubscriptionDataAccess)_subDa).GetAll();
        }
        public int SubscribeCompanyToPlan(CompanySubscription subscription)
        {
            // Real-world logic: Calculate EndDate/BillingDate here if not set
            if (subscription.StartDate == default)
                subscription.StartDate = DateTime.UtcNow;

            return (int)((CompanySubscriptionDataAccess)_subDa).Insert(subscription);
        }

        public CompanySubscription GetCompanySubscription(int companyId)
        {
            // Note: Your Base DA has GetByCompanyId returning a list
            // But usually we want the single active one. 
            var list = ((CompanySubscriptionDataAccess)_subDa).GetByCompanyId(companyId);
            if (list == null || list.Count == 0)
            {
                return null;
            }

            var activeSub = list
                .Where(sub => string.Equals(sub.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(sub => sub.StartDate)
                .FirstOrDefault();

            if (activeSub != null)
            {
                return activeSub;
            }

            return list
                .OrderByDescending(sub => sub.StartDate)
                .FirstOrDefault();
        }

        public bool UpdateCompanySubscription(CompanySubscription subscription)
        {
            if (subscription == null) throw new ArgumentNullException(nameof(subscription));

            return ((CompanySubscriptionDataAccess)_subDa).Update(subscription) > 0;
        }

        #endregion

        #region Quota Enforcement (The "Gym Membership" Logic)

        /// <summary>
        /// Call this BEFORE allowing a user to see the "Add Product" screen or hitting Save.
        /// </summary>
        public QuotaCheckResult CheckProductCapability(int companyId)
        {
            // Calls our custom Partial implementation
            return ((CompanySubscriptionDataAccess)_subDa).CanAddProduct(companyId);
        }

        /// <summary>
        /// Call this during the Checkout process. 
        /// If this returns false, DO NOT process the payment or create the order.
        /// </summary>
        public QuotaCheckResult ProcessOrderUsage(int companyId)
        {
            // Calls our custom Partial implementation for atomic updates
            return ((SubscriptionUsageDataAccess)_usageDa).TryConsumeOrderQuota(companyId);
        }

        public SubscriptionUsage GetCurrentUsage(int companyId)
        {
            // 1. Get the active subscription
            var sub = GetCompanySubscription(companyId);
            if (sub == null) return null;

            var usages = ((SubscriptionUsageDataAccess)_usageDa).GetBySubscriptionId(sub.Id);

            var now = DateTime.UtcNow;

            var currentUsage = usages.FirstOrDefault(u => u.CycleStart <= now && u.CycleEnd > now);

            return currentUsage ?? usages.OrderByDescending(u => u.CycleStart).FirstOrDefault();
        }

        public int GetCurrentProductCount(int companyId)
        {
            if (companyId <= 0) return 0;
            // Reuse the ProductDataAccess we injected earlier
            return _productDa.GetProductCount(companyId);
        }

        public bool IsSubscriptionLocked(int companyId, out int currentUsage, out int limit)

        {

            currentUsage = 0;

            limit = 0;

            if (companyId <= 0) return false;

            // 1. Get Subscription (Limit)

            var subObj = GetCompanySubscription(companyId);

            if (subObj == null) return false; // No plan usually means no lock (or full lock depending on biz logic, assuming false here)

            limit = subObj.MaxOrders;

            // 2. Get Usage (Counter)

            // Reusing GetCurrentUsage to ensure we check the correct date window

            var usageObj = GetCurrentUsage(companyId);

            if (usageObj != null)

            {

                currentUsage = usageObj.OrdersProcessed;

            }

            else

            {

                // If usage is null, it means no orders in this cycle yet.

                currentUsage = 0;

            }

            // 3. The Lock Check

            // Return TRUE if they have exceeded their limit

            return currentUsage > limit;

        }


        #endregion

        #region Product Lock Logic

        public bool IsProductLocked(int companyId, out int currentCount, out int limit)
        {
            currentCount = 0;
            limit = 0;

            if (companyId <= 0) return true; // Safety

            // 1. Get the Subscription (The Limit)
            var sub = GetCompanySubscription(companyId);

            // If no subscription exists, lock it (or unlock based on your default/free tier policy)
            if (sub == null) return true;

            limit = sub.MaxProducts;

            // 2. Get the Current Count (Using the new SP)
            currentCount = _productDa.GetProductCount(companyId);

            // 3. Compare
            // Return TRUE if they have hit or exceeded the limit
            return currentCount >= limit;
        }

        #endregion
    }
}