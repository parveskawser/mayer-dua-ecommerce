using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MDUA.Web.UI.Controllers
{
    public class BaseController : Controller
    {
        // Helper to get the logged-in user's ID
        protected int CurrentUserId
        {
            get
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
                return 0; // Or throw exception based on your preference
            }
        }

        // Helper to get the logged-in user's Name
        protected string CurrentUserName
        {
            get
            {
                Console.WriteLine("CurrentUserName is null or ",ClaimTypes.Name);

                return User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
               
            }
        }
        // Helper to get the logged-in user's CompanyId
        protected int CurrentCompanyId
        {
            get
            {
                var companyIdClaim = User.FindFirst("CompanyId");
                if (companyIdClaim != null && int.TryParse(companyIdClaim.Value, out int companyId))
                {
                    return companyId;
                }
                return 0;
            }
        }
        public new IActionResult HandleAccessDenied()
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                // Returns JSON telling the JS Global Handler to redirect
                return Json(new
                {
                    success = false,
                    message = "Access Denied",
                    redirectUrl = Url.Action("AccessDenied", "Account")
                });
            }

            // Normal Redirect
            return RedirectToAction("AccessDenied", "Account");
        }
       


        protected bool HasPermission(string permissionName)
        {
            return User.HasClaim(c => c.Type == "Permission" && c.Value.Equals(permissionName, StringComparison.OrdinalIgnoreCase));
        }
        // Helper to check if user is logged in
        protected bool IsLoggedIn => User.Identity != null && User.Identity.IsAuthenticated;
    }
}