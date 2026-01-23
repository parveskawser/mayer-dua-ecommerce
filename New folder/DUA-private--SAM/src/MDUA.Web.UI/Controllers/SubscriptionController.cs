using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Facade;
using MDUA.Facade.Interface;
using MDUA.Framework.Exceptions;
using MDUA.Web.UI.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text.Json;

namespace MDUA.Web.Controllers
{
    public class SubscriptionController : BaseController
    {
        private readonly ISubscriptionSystemFacade _subscriptionFacade;
        private readonly IUserLoginFacade _userLoginFacade;
        private readonly ICompanyFacade _companyFacade;
        public SubscriptionController(ISubscriptionSystemFacade subscriptionFacade, IUserLoginFacade userLoginFacade, ICompanyFacade companyFacade)
        {
            _subscriptionFacade = subscriptionFacade;
            _userLoginFacade = userLoginFacade;
            _companyFacade = companyFacade;
        }

        #region 1. Subscription Plans (Catalog Management - SuperAdmin)

        [HttpGet]
        [Route("subscription/plans")]
        public IActionResult PlanList()
        {
            // 1. Permission Check
            if (!HasPermission("SubscriptionPlan.View")) return HandleAccessDenied();

            try
            {
                var plans = _subscriptionFacade.GetAllPlans();
                return View(plans);
            }
            catch (Exception ex)
            {
                // In real world, log this
                return StatusCode(500, "Error loading plans.");
            }
        }

        [HttpGet]
        [Route("subscription/plan/create")]
        public IActionResult CreatePlan()
        {
            if (!HasPermission("SubscriptionPlan.Create")) return HandleAccessDenied();
            return View();
        }

        [HttpPost]
        [Route("subscription/plan/create")]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePlan(SubscriptionPlan model)
        {
            // 1. Permission Check
            if (!HasPermission("SubscriptionPlan.Create")) return HandleAccessDenied();

            // 2. Setup Data
            model.IsActive = true;
            model.CreatedAt = DateTime.UtcNow;
            model.UpdatedAt = null;



            int id = _subscriptionFacade.CreatePlan(model);

            if (id > 0)
            {
                return RedirectToAction(nameof(PlanList));
            }

            ModelState.AddModelError("", "Database returned 0 ID, but no exception was thrown.");
            return View(model);
        }

        [HttpGet]
        [Route("subscription/plan/edit/{id}")]
        public IActionResult EditPlan(int id)
        {
            if (!HasPermission("SubscriptionPlan.Edit")) return HandleAccessDenied();

            var plan = _subscriptionFacade.GetPlan(id);
            if (plan == null) return NotFound();

            return View(plan);
        }

        [HttpPost]
        [Route("subscription/plan/edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult EditPlan(SubscriptionPlan model)
        {
            if (!HasPermission("SubscriptionPlan.Edit")) return HandleAccessDenied();

            if (ModelState.IsValid)
            {
                try
                {
                    // =========================================================
                    // ✅ CRITICAL FIX: DATE SANITIZATION
                    // =========================================================

                    // 1. Ensure CreatedAt is valid (If it came back as MinValue, reset it or keep original)
                    // Since we can't easily fetch original here without a db call, just ensure it's safe for SQL.
                    if (model.CreatedAt.Year < 1753)
                    {
                        model.CreatedAt = DateTime.UtcNow; // Fallback if lost
                    }

                    // 2. Set UpdatedAt to NOW (This is an Edit, so we update the timestamp)
                    model.UpdatedAt = DateTime.UtcNow;

                    // =========================================================

                    // The Facade/DA should handle the Update logic
                    bool success = _subscriptionFacade.UpdatePlan(model);

                    if (success)
                        return RedirectToAction(nameof(PlanList));
                    else
                        ModelState.AddModelError("", "Update failed (Database returned 0 rows affected).");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error: {ex.Message}");
                }
            }
            return View(model);
        }

        #endregion

        #region 2. Company Subscriptions (Contract Management - SuperAdmin)

        // View all companies and their current plans
        [HttpGet]
        [Route("subscription/company-list")]
        public IActionResult CompanySubscriptionList()
        {
            if (!HasPermission("CompanySubscription.View")) return HandleAccessDenied();

            // 1. Get ALL Companies
            var allCompanies = _companyFacade.GetAll();

            // 2. Get ALL Active Subscriptions
            var allSubs = _subscriptionFacade.GetAllCompanySubscriptions();

            // 3. Join them into a ViewModel for the View
            var model = from c in allCompanies
                        join s in allSubs on c.Id equals s.CompanyId into subGroup
                        from sub in subGroup.DefaultIfEmpty() // Left Join
                        select new CompanySubscriptionStatusViewModel
                        {
                            CompanyId = c.Id,
                            CompanyName = c.CompanyName,
                            CompanyCode = c.CompanyCode,
                            // If sub is null, they have no plan
                            HasPlan = sub != null,
                            PlanName = sub?.PlanNameSnapshot ?? "No Active Plan",
                            Status = sub?.Status ?? "None",
                            EndDate = sub?.EndDate
                        };

            return View(model);
        }

        // Helper Class (Put this at the bottom of Controller or in a separate file)
        public class CompanySubscriptionStatusViewModel
        {
            public int CompanyId { get; set; }
            public string CompanyName { get; set; }
            public string CompanyCode { get; set; }
            public bool HasPlan { get; set; }
            public string PlanName { get; set; }
            public string Status { get; set; }
            public DateTime? EndDate { get; set; }
        }

        [HttpGet]
        [Route("subscription/assign/{companyId}")]
        public IActionResult AssignPlan(int companyId)
        {
            if (!HasPermission("CompanySubscription.Manage")) return HandleAccessDenied();

            // Load data for the dropdown
            ViewBag.Plans = _subscriptionFacade.GetAllPlans();
            ViewBag.CompanyId = companyId;

            // Check if they already have one
            var currentSub = _subscriptionFacade.GetCompanySubscription(companyId);
            return View(currentSub ?? new CompanySubscription { CompanyId = companyId });
        }


        [HttpPost]
        [Route("subscription/assign/{companyId?}")] // ✅ FIX: Allows the ID in the URL
        [ValidateAntiForgeryToken]
        public IActionResult AssignPlan(CompanySubscription model)
        {
            // 1. Permission Check
            if (!HasPermission("CompanySubscription.Manage")) return HandleAccessDenied();

            try
            {
                var existingSub = _subscriptionFacade.GetCompanySubscription(model.CompanyId);

                // 2. Logic to Fill Plan Details (Snapshotting)
                if (model.SubscriptionPlanId > 0)
                {
                    var plan = _subscriptionFacade.GetPlan(model.SubscriptionPlanId.Value);
                    if (plan != null)
                    {
                        var defaults = ParsePlanDefaults(plan.DefaultsJSON);

                        model.PlanNameSnapshot = plan.PlanName;
                        if (model.MaxProducts == 0) model.MaxProducts = defaults.MaxProducts ?? 2000; // Default if empty
                        if (model.MaxOrders == 0) model.MaxOrders = defaults.MaxOrders ?? 1000;     // Default if empty
                        if (model.PriceCharged == 0) model.PriceCharged = plan.BasePrice;
                    }
                }

                // Fallback for Custom Plans
                if (string.IsNullOrEmpty(model.PlanNameSnapshot)) model.PlanNameSnapshot = "Custom Plan";
                if (string.IsNullOrEmpty(model.CurrencyCode)) model.CurrencyCode = "BDT";

                // 3. Audit Fields
                model.CreatedBy = CurrentUserName;
                model.Status = "ACTIVE";

                // =========================================================
                // ✅ CRITICAL FIX: DATE SANITIZATION (Prevents SQL Crash)
                // =========================================================

                // Required Dates: Set to NOW
                model.StartDate = DateTime.UtcNow;
                model.CreatedAt = existingSub?.CreatedAt ?? DateTime.UtcNow;

                // Nullable Dates: Explicitly set to NULL (prevent 0001-01-01)
                model.EndDate = null;
                model.UpdatedAt = DateTime.UtcNow;
                model.UpdatedBy = CurrentUserName;

                // Optional: Calculate Next Billing Date so it's not empty/invalid
                if (model.OrderCycle == "MONTHLY")
                    model.NextBillingDate = DateTime.UtcNow.AddMonths(1);
                else if (model.OrderCycle == "YEARLY")
                    model.NextBillingDate = DateTime.UtcNow.AddYears(1);
                else
                    model.NextBillingDate = null;
                // =========================================================

                // 4. Call Facade
                int id = 0;
                if (existingSub != null)
                {
                    model.Id = existingSub.Id;
                    model.CreatedBy = existingSub.CreatedBy;
                    model.Status = "ACTIVE";
                    id = _subscriptionFacade.UpdateCompanySubscription(model) ? model.Id : 0;
                }
                else
                {
                    id = _subscriptionFacade.SubscribeCompanyToPlan(model);
                }

                if (id > 0)
                {
                    return RedirectToAction(nameof(CompanySubscriptionList));
                }
                else
                {
                    ModelState.AddModelError("", "Database returned 0 ID (Insert failed silently).");
                }
            }
            catch (Exception ex)
            {
                // Reload dropdown on error so the page doesn't break
                ViewBag.Plans = _subscriptionFacade.GetAllPlans();
                ViewBag.CompanyId = model.CompanyId;

                ModelState.AddModelError("", $"System Error: {ex.Message}");
            }

            // If we got here, something failed. Return view with model.
            ViewBag.Plans = _subscriptionFacade.GetAllPlans(); // Ensure ViewBag is refilled
            return View(model);
        }

        #endregion

        #region 3. Tenant View (My Subscription)

        // This is for a logged-in Company Admin to see THEIR OWN limits
        [HttpGet]
        [Route("subscription/my-plan")]
        public IActionResult MySubscription()
        {
            // 1. Ensure user is logged in
            if (!IsLoggedIn) return RedirectToAction("LogIn", "Account");

            // 2. Get their CompanyID
            int companyId = CurrentCompanyId;
            if (companyId <= 0) return RedirectToAction("AccessDenied", "Account");

            // 3. Fetch their subscription
            var sub = _subscriptionFacade.GetCompanySubscription(companyId);

            // 4. Fetch their Usage stats (Flow check - Orders)
            var usage = _subscriptionFacade.GetCurrentUsage(companyId);

            // ✅ 5. Fetch Product Count (NEW ADDITION)
            // This calls the method we just added to the Facade to count rows in the Product table
            int productCount = _subscriptionFacade.GetCurrentProductCount(companyId);

            var viewModel = new MySubscriptionViewModel
            {
                Subscription = sub,
                CurrentUsage = usage,
                CurrentProductCount = productCount // ✅ Assign it here so the View doesn't crash
            };

            return View(viewModel);
        }
        #endregion

        #region 3.1 Tenant Plan Upgrade

        [HttpGet]
        [Route("subscription/update-plan")]
        public IActionResult UpdateMyPlan()
        {
            if (!IsLoggedIn) return RedirectToAction("LogIn", "Account");

            int companyId = CurrentCompanyId;
            if (companyId <= 0) return RedirectToAction("AccessDenied", "Account");

            var plans = _subscriptionFacade.GetAllPlans()
                .Where(plan => plan.IsActive)
                .ToList();

            var model = new PlanUpgradeViewModel
            {
                CurrentSubscription = _subscriptionFacade.GetCompanySubscription(companyId),
                AvailablePlans = plans.Select(plan =>
                {
                    var defaults = ParsePlanDefaults(plan.DefaultsJSON);
                    return new PlanUpgradeOption
                    {
                        Plan = plan,
                        MaxProducts = defaults.MaxProducts ?? 0,
                        MaxOrders = defaults.MaxOrders ?? 0,
                        OrderCycle = string.IsNullOrWhiteSpace(defaults.OrderCycle) ? "MONTHLY" : defaults.OrderCycle
                    };
                }).ToList()
            };

            return View("UpdatePlan", model);
        }

        [HttpPost]
        [Route("subscription/update-plan")]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateMyPlan(int planId)
        {
            if (!IsLoggedIn) return RedirectToAction("LogIn", "Account");

            int companyId = CurrentCompanyId;
            if (companyId <= 0) return RedirectToAction("AccessDenied", "Account");

            var plan = _subscriptionFacade.GetPlan(planId);
            if (plan == null || !plan.IsActive)
            {
                ModelState.AddModelError("", "Selected plan is not available.");
                return RedirectToAction(nameof(MySubscription));
            }

            var existingSub = _subscriptionFacade.GetCompanySubscription(companyId);
            var defaults = ParsePlanDefaults(plan.DefaultsJSON);
            var maxProducts = defaults.MaxProducts ?? existingSub?.MaxProducts ?? 0;
            var maxOrders = defaults.MaxOrders ?? existingSub?.MaxOrders ?? 0;
            var orderCycle = string.IsNullOrWhiteSpace(defaults.OrderCycle)
                ? existingSub?.OrderCycle ?? "MONTHLY"
                : defaults.OrderCycle;

            if (maxProducts <= 0) maxProducts = 2000;
            if (maxOrders <= 0) maxOrders = 1000;

            var now = DateTime.UtcNow;

            if (existingSub != null)
            {
                ApplyPlanToSubscription(existingSub, plan, maxProducts, maxOrders, orderCycle, now);
                _subscriptionFacade.UpdateCompanySubscription(existingSub);
            }
            else
            {
                var newSubscription = new CompanySubscription
                {
                    CompanyId = companyId,
                    SubscriptionPlanId = plan.Id,
                    PlanNameSnapshot = plan.PlanName,
                    MaxProducts = maxProducts,
                    MaxOrders = maxOrders,
                    OrderCycle = orderCycle,
                    PriceCharged = plan.BasePrice,
                    CurrencyCode = plan.CurrencyCode,
                    StartDate = now,
                    EndDate = null,
                    NextBillingDate = orderCycle == "YEARLY" ? now.AddYears(1) : now.AddMonths(1),
                    CycleAnchorDate = null,
                    Status = "ACTIVE",
                    CreatedBy = CurrentUserName,
                    CreatedAt = now,
                    UpdatedBy = null,
                    UpdatedAt = null
                };

                try
                {
                    _subscriptionFacade.SubscribeCompanyToPlan(newSubscription);
                }
                catch (ObjectInsertException)
                {
                    existingSub = _subscriptionFacade.GetCompanySubscription(companyId);
                    if (existingSub != null)
                    {
                        ApplyPlanToSubscription(existingSub, plan, maxProducts, maxOrders, orderCycle, now);
                        _subscriptionFacade.UpdateCompanySubscription(existingSub);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return RedirectToAction(nameof(MySubscription));
        }

        #endregion

        #region 4. Partial Views (AJAX Support)

        [HttpGet]
        [Route("subscription/get-limits-partial/{companyId}")]
        [Route("Subscription/GetLimitsPartial/{companyId}")]



        public IActionResult GetLimitsPartial(int companyId)
        {
            // Security check: only SuperAdmin or the Company Owner can see this
            bool isSuperAdmin = HasPermission("CompanySubscription.View");
            bool isOwner = (CurrentCompanyId == companyId);

            if (!isSuperAdmin && !isOwner) return HandleAccessDenied();

            try
            {
                var sub = _subscriptionFacade.GetCompanySubscription(companyId);
                return PartialView("_SubscriptionLimitsPartial", sub);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        [HttpPost]
        [Route("subscription/remove/{companyId}")]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveSubscription(int companyId)
        {
            if (!HasPermission("CompanySubscription.Manage")) return HandleAccessDenied();

            var existingSub = _subscriptionFacade.GetCompanySubscription(companyId);
            if (existingSub == null) return RedirectToAction(nameof(CompanySubscriptionList));

            existingSub.Status = "EXPIRED";
            existingSub.EndDate = DateTime.UtcNow;
            existingSub.NextBillingDate = null;
            existingSub.UpdatedAt = DateTime.UtcNow;
            existingSub.UpdatedBy = CurrentUserName;

            _subscriptionFacade.UpdateCompanySubscription(existingSub);

            return RedirectToAction(nameof(CompanySubscriptionList));
        }

        [HttpGet]
        [Route("subscription/limit-reached")]
        public IActionResult LimitReached(int current, int limit)
        {
            ViewBag.CurrentUsage = current;
            ViewBag.Limit = limit;
            ViewBag.PendingOrders = current - limit;

            return View();
        }

        private PlanDefaults ParsePlanDefaults(string defaultsJson)
        {
            if (string.IsNullOrWhiteSpace(defaultsJson))
            {
                return new PlanDefaults();
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<PlanDefaults>(defaultsJson, options) ?? new PlanDefaults();
            }
            catch (JsonException)
            {
                return new PlanDefaults();
            }
        }

        private void ApplyPlanToSubscription(
            CompanySubscription subscription,
            SubscriptionPlan plan,
            int maxProducts,
            int maxOrders,
            string orderCycle,
            DateTime timestamp)
        {
            subscription.SubscriptionPlanId = plan.Id;
            subscription.PlanNameSnapshot = plan.PlanName;
            subscription.MaxProducts = maxProducts;
            subscription.MaxOrders = maxOrders;
            subscription.OrderCycle = orderCycle;
            subscription.PriceCharged = plan.BasePrice;
            subscription.CurrencyCode = plan.CurrencyCode;
            subscription.StartDate = timestamp;
            subscription.EndDate = null;
            subscription.NextBillingDate = orderCycle == "YEARLY"
                ? timestamp.AddYears(1)
                : timestamp.AddMonths(1);
            subscription.CycleAnchorDate = null;
            subscription.Status = "ACTIVE";
            subscription.UpdatedBy = CurrentUserName;
            subscription.UpdatedAt = timestamp;
        }

        private class PlanDefaults
        {
            public int? MaxProducts { get; set; }
            public int? MaxOrders { get; set; }
            public string OrderCycle { get; set; }
        }
    }

    public class MySubscriptionViewModel
    {
        public CompanySubscription Subscription { get; set; }
        public SubscriptionUsage CurrentUsage { get; set; }
        public int CurrentProductCount { get; set; }
    }

    public class PlanUpgradeViewModel
    {
        public CompanySubscription CurrentSubscription { get; set; }
        public System.Collections.Generic.List<PlanUpgradeOption> AvailablePlans { get; set; }
    }

    public class PlanUpgradeOption
    {
        public SubscriptionPlan Plan { get; set; }
        public int MaxProducts { get; set; }
        public int MaxOrders { get; set; }
        public string OrderCycle { get; set; }
    }

}