using MDUA.Entities;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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
                var user = loginResult.UserLogin;
                Console.WriteLine($"2FA: {user.IsTwoFactorEnabled}, secret null? {string.IsNullOrEmpty(user.TwoFactorSecret)}");

                if (user.IsTwoFactorEnabled)
                {
                    TempData["PreAuthUserId"] = user.Id;
                    TempData["RememberMe"] = rememberMe; // Carry over this setting
                    TempData["ReturnUrl"] = returnUrl;   // Carry over this setting
                    return RedirectToAction("VerifyTwoFactor");
                }

                // If No 2FA, proceed with your existing robust login logic
                await CompleteSignInAsync(loginResult, rememberMe);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Dashboard", "Home");
            }

            ViewBag.Error = loginResult.ErrorMessage ?? "Invalid login attempt.";
            return View(loginResult);
        }

        [HttpGet]
        public IActionResult VerifyTwoFactor()
        {
            if (TempData.Peek("PreAuthUserId") == null) return RedirectToAction("LogIn");

            // ✅ Defaults for normal Login flow
            ViewData["FormAction"] = "VerifyTwoFactor";
            ViewData["BackAction"] = "LogIn";
            ViewData["BackText"] = "Back to Login";

            return View();
        }

        public class VerifyTwoFactorVm
        {
            public string Code { get; set; }
            public int TargetUserId { get; set; }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]   
        public IActionResult DisableTwoFactor()
        {
            try
            {
                // 1. Get Logged in User ID
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Json(new { success = false, message = "User session invalid." });
                }

                // 2. Call Facade to update DB
                _userLoginFacade.DisableTwoFactor(userId);

                // 3. Return Success
                return Json(new { success = true, message = "Two-Factor Authentication has been disabled." });
            }
            catch (Exception ex)
            {
                // Log error here
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyTwoFactor(VerifyTwoFactorVm model)
        {
            if (TempData["PreAuthUserId"] is not int userId) return RedirectToAction("LogIn");

            var result = _userLoginFacade.GetUserLoginById(userId);
            if (!result.IsSuccess) return RedirectToAction("LogIn");

            Console.WriteLine($"[2FA-POST] Code null? {model?.Code == null}, len={(model?.Code?.Length ?? 0)}, raw='{model?.Code}'");

            bool isValid = _userLoginFacade.VerifyTwoFactorByUserId(userId, model.Code);

            if (isValid)
            {
                bool rememberMe = (bool)(TempData["RememberMe"] ?? false);
                string returnUrl = TempData["ReturnUrl"] as string;

                await CompleteSignInAsync(result, rememberMe);

                TempData.Remove("PreAuthUserId");
                TempData.Remove("RememberMe");
                TempData.Remove("ReturnUrl");

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
                return RedirectToAction("Dashboard", "Home");
            }

            ModelState.AddModelError("", "Invalid authenticator code.");
            TempData.Keep("PreAuthUserId");
            TempData.Keep("RememberMe");
            TempData.Keep("ReturnUrl");
            return View(model);
        }




        private async Task CompleteSignInAsync(UserLoginResult loginResult, bool rememberMe)
        {
            // 3. DB AUTH: Create User Session in SQL
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

                // Add the SessionKey to the cookie claims.
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
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // 1. DB AUTH: Invalidate Session in SQL
            var sessionClaim = User.FindFirst("SessionKey");
            if (sessionClaim != null && Guid.TryParse(sessionClaim.Value, out Guid key))
            {
                _userLoginFacade.InvalidateSession(key);
            }

            // 2. Remove Cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("LogIn", "Account");
        }

        [HttpGet]
        public IActionResult AccessDenied(string missingPermission = null)
        {
            ViewBag.MissingPermission = missingPermission;
            ViewBag.UserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "No Role";
            return View();
        }



        // ... inside AccountController ...

        #region Forgot Password via 2FA

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                ViewBag.Error = "Please enter your username.";
                return View();
            }

            // 1. Find User
            var user = _userLoginFacade.GetUserByUsername(username);

            if (user == null)
            {
                // User doesn't exist
                ViewBag.Error = "Account not found.";
                return View();
            }

            // 2. CHECK: Is 2FA Enabled?
            if (!user.IsTwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecret))
            {
                // ❌ NO 2FA -> Show "Contact Admin" UI
                ViewBag.ManualResetRequired = true;
                return View();
            }

            // ✅ YES 2FA -> Proceed to Verification

            // Clear any old login data to prevent conflicts
            TempData.Remove("PreAuthUserId");
            TempData.Remove("RememberMe");
            TempData.Remove("ReturnUrl");

            // Set Reset Data
            TempData["ResetUserId"] = user.Id;
            TempData["ResetUsername"] = user.UserName;

            return RedirectToAction("VerifyReset2FA");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult VerifyReset2FA()
        {
            if (TempData.Peek("ResetUserId") == null) return RedirectToAction("ForgotPassword");

            // ✅ FIX: Pass the ID into the View Model
            var model = new VerifyTwoFactorVm
            {
                TargetUserId = (int)TempData.Peek("ResetUserId")
            };

            ViewData["FormAction"] = "VerifyReset2FA";
            ViewData["BackAction"] = "ForgotPassword";
            ViewData["BackText"] = "Try different user";
            ViewData["Title"] = "Verify Reset";

            return View("VerifyTwoFactor", model); // Pass model here
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyReset2FA(VerifyTwoFactorVm model)
        {
            // ✅ FIX: Read ID from the form (Model), NOT TempData
            // This prevents the "Redirect to Login" issue if TempData is lost
            if (model.TargetUserId == 0)
            {
                return RedirectToAction("ForgotPassword");
            }

            bool isValid = _userLoginFacade.VerifyTwoFactorByUserId(model.TargetUserId, model.Code);

            if (isValid)
            {
                TempData["CanResetPassword"] = true;
                TempData["ResetUserId"] = model.TargetUserId; // Store for the final step
                return RedirectToAction("ResetPassword");
            }

            ModelState.AddModelError("", "Invalid authenticator code.");

            // Restore View Settings
            ViewData["FormAction"] = "VerifyReset2FA";
            ViewData["BackAction"] = "ForgotPassword";
            ViewData["BackText"] = "Try different user";
            ViewData["Title"] = "Verify Reset";

            return View("VerifyTwoFactor", model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword()
        {
            // Ensure the user actually passed the 2FA check
            if (TempData["CanResetPassword"] == null || TempData.Peek("ResetUserId") == null)
            {
                return RedirectToAction("ForgotPassword");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            // 1. Security Check
            if (TempData["ResetUserId"] is not int userId) return RedirectToAction("ForgotPassword");

            // 2. Validation
            if (string.IsNullOrEmpty(newPassword) || newPassword != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match or are empty.";

                // ✅ CRITICAL FIX: Keep the flags alive so the page doesn't expire
                TempData.Keep("ResetUserId");
                TempData["CanResetPassword"] = true; // Ensure GET check passes

                return View();
            }

            try
            {
                // 3. Update & Cleanup
                _userLoginFacade.UpdatePassword(userId, newPassword);
                TempData.Clear(); // Clear all flags
                return View("ResetSuccess");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred.";
                TempData.Keep("ResetUserId");
                TempData["CanResetPassword"] = true;
                return View();
            }
        }



        #endregion
    }
}