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
            if (User.Identity != null && User.Identity.IsAuthenticated)
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
                // 3.  DB AUTH: Create User Session in SQL
                // Get IP and User Agent for security tracking
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                string deviceInfo = Request.Headers["User-Agent"].ToString();

                // This writes to the UserSession table and returns a unique SessionKey (Guid)
                Guid sessionKey = _userLoginFacade.CreateUserSession(loginResult.UserLogin.Id, ipAddress, deviceInfo);

                // 4. Build Claims List
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, loginResult.UserLogin.Id.ToString()),
                    new Claim(ClaimTypes.Name, loginResult.UserLogin.UserName),
                    new Claim("CompanyId", loginResult.UserLogin.CompanyId.ToString()),
                    
                    // actual RoleName from DB
                    new Claim(ClaimTypes.Role, !string.IsNullOrEmpty(loginResult.RoleName) ? loginResult.RoleName : "User"),

                    //   Add the SessionKey to the cookie claims. 
                    // This allows Program.cs to validate against the DB on every request.
                    new Claim("SessionKey", sessionKey.ToString())
                };

                // 5. Add Permissions to Claims
                if (loginResult.AuthorizedActions != null)
                {
                    foreach (var permission in loginResult.AuthorizedActions)
                    {
                        claims.Add(new Claim("Permission", permission));
                    }
                }

                // 6. Create Identity
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 7. Configure Cookie Properties
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = rememberMe,
                    ExpiresUtc = rememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddMinutes(60),
                    AllowRefresh = true
                };

                // 8. Sign In
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
            // 1.  DB AUTH: Invalidate Session in SQL
            // Before deleting the cookie, we grab the SessionKey and tell the DB this session is over.
            var sessionClaim = User.FindFirst("SessionKey");
            if (sessionClaim != null && Guid.TryParse(sessionClaim.Value, out Guid key))
            {
                _userLoginFacade.InvalidateSession(key);
            }

            // 2. Remove Cookie
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