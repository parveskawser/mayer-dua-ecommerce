using MDUA.Entities;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MDUA.Web.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserLoginFacade _userLoginFacade;

        public AccountController(IUserLoginFacade userLoginFacade)
        {
            _userLoginFacade = userLoginFacade;
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(string username, string password, bool rememberMe, string returnUrl = null)
        {
            // 1. Input Validation
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please enter both username and password.";
                return View();
            }

            // 2. Authenticate User via Facade
            var loginResult = _userLoginFacade.GetUserLoginBy(username, password);

            if (loginResult.IsSuccess)
            {
                // 3. Build Claims List
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, loginResult.UserLogin.Id.ToString()),
                    new Claim(ClaimTypes.Name, loginResult.UserLogin.UserName),
                    new Claim("CompanyId", loginResult.UserLogin.CompanyId.ToString()),
                    
                    // ✅ FIXED: Use the actual RoleName from DB. Default to "User" only if null.
                    new Claim(ClaimTypes.Role, !string.IsNullOrEmpty(loginResult.RoleName) ? loginResult.RoleName : "User")
                };

                // 4. Add Permissions to Claims
                if (loginResult.AuthorizedActions != null)
                {
                    foreach (var permission in loginResult.AuthorizedActions)
                    {
                        claims.Add(new Claim("Permission", permission));
                    }
                }

                // 5. Create Identity
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 6. Configure Cookie Properties
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = rememberMe,
                    ExpiresUtc = rememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddMinutes(60),
                    AllowRefresh = true
                };

                // 7. Sign In
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Dashboard", "Home");
            }

            ViewBag.Error = loginResult.ErrorMessage ?? "Invalid login attempt.";
            return View(loginResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult AccessDenied(string missingPermission = null)
        {
            ViewBag.MissingPermission = missingPermission;
            ViewBag.UserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "No Role";
            return View();
        }
    }
}