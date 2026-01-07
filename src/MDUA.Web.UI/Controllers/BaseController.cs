using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MDUA.Web.UI.Services;
using MDUA.Web.UI.Services.Interface;

namespace MDUA.Web.UI.Controllers
{
    public class BaseController : Controller
    {
        protected int CurrentUserId
        {
            get
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
                return 0;
            }
        }

        // Helper to get the logged-in user's Name
        protected string CurrentUserName
        {
            get
            {
                return User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            }
        }

        // ✅ CORRECTED: Smart Company ID Resolution
        protected int CurrentCompanyId
        {
            get
            {
                // 1. Priority: Trust the Logged-in User's Claim (Admin/User Context)
                var companyIdClaim = User.FindFirst("CompanyId");
                if (companyIdClaim != null && int.TryParse(companyIdClaim.Value, out int companyId))
                {
                    return companyId;
                }

                // 2. Fallback: Ask the TenantResolver (Anonymous / Localhost / Landing Page)
                var tenantResolver = HttpContext.RequestServices.GetService<ITenantResolver>();
                if (tenantResolver != null)
                {
                    return tenantResolver.GetCompanyId();
                }

                // 3. Ultimate Fallback (Should rarely happen if Resolver works)
                return 1;
            }
        }

        public new IActionResult HandleAccessDenied()
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                return Json(new
                {
                    success = false,
                    message = "Access Denied",
                    redirectUrl = Url.Action("AccessDenied", "Account")
                });
            }

            return RedirectToAction("AccessDenied", "Account");
        }

        protected bool HasPermission(string permissionName)
        {
            return User.HasClaim(c => c.Type == "Permission" && c.Value.Equals(permissionName, StringComparison.OrdinalIgnoreCase));
        }

        protected bool IsLoggedIn => User.Identity != null && User.Identity.IsAuthenticated;
    }
}