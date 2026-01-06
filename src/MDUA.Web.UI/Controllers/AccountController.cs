using Google.Apis.Auth;
using MDUA.Entities;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using System.Security.Claims;

namespace MDUA.Web.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserLoginFacade _userLoginFacade;
        private readonly IConfiguration _configuration; 
        public AccountController(IUserLoginFacade userLoginFacade, IConfiguration configuration)
        {
            _userLoginFacade = userLoginFacade;
            _configuration = configuration;

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
            // 1. Session Check
            if (TempData["PreAuthUserId"] is not int userId) return RedirectToAction("LogIn");

            // 2. Validate User
            var result = _userLoginFacade.GetUserLoginById(userId);
            if (!result.IsSuccess) return RedirectToAction("LogIn");


            // 3. Verify Code
            bool isValid = _userLoginFacade.VerifyTwoFactorByUserId(userId, model.Code);

            if (isValid)
            {
                bool rememberMe = (bool)(TempData["RememberMe"] ?? false);
                string returnUrl = TempData["ReturnUrl"] as string;

                //  Retrieve LoginMethod (set by GoogleCallback or Password Login)
                // Default to "Password" if missing (e.g. legacy flows)
                string loginMethod = (TempData["LoginMethod"] as string) ?? "Password";

                //   Pass loginMethod to CompleteSignInAsync
                await CompleteSignInAsync(result, rememberMe, loginMethod);

                // 4. Cleanup TempData
                TempData.Remove("PreAuthUserId");
                TempData.Remove("RememberMe");
                TempData.Remove("ReturnUrl");
                TempData.Remove("LoginMethod"); 

                // 5. Redirect
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
                return RedirectToAction("Dashboard", "Home");
            }

            // 6. Handle Failure
            ModelState.AddModelError("", "Invalid authenticator code.");

            //  Keep ALL TempData keys so the user can try again
            TempData.Keep("PreAuthUserId");
            TempData.Keep("RememberMe");
            TempData.Keep("ReturnUrl");
            TempData.Keep("LoginMethod"); 

            return View(model);
        }





        private async Task CompleteSignInAsync(UserLoginResult loginResult, bool rememberMe, string loginMethod = "Password")
        {
            // 1. Get Environment Info
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            string deviceInfo = Request.Headers["User-Agent"].ToString();

            // 2. DB AUTH: Create User Session in SQL (Passing the new LoginMethod)
            Guid sessionKey = _userLoginFacade.CreateUserSession(
                loginResult.UserLogin.Id,
                ipAddress,
                deviceInfo,
                loginMethod 
            );

            // 3. Build Claims List 
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, loginResult.UserLogin.Id.ToString()),
        new Claim(ClaimTypes.Name, loginResult.UserLogin.UserName),
        new Claim("CompanyId", loginResult.UserLogin.CompanyId.ToString()),
        
        // Role Logic
        new Claim(ClaimTypes.Role, !string.IsNullOrEmpty(loginResult.RoleName) ? loginResult.RoleName : "User"),

        // Session Key Claim
        new Claim("SessionKey", sessionKey.ToString()),
        
        //  Helpful for UI to know how they logged in without querying DB
        new Claim("LoginMethod", loginMethod)
    };

            // 4. Add Permissions to Claims (EXISTING LOGIC PRESERVED)
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
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GoogleCallback(string credential)
        {
            try
            {
                var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");

                if (string.IsNullOrEmpty(clientId))
                {
                    return View("LogIn", new MDUA.Entities.UserLoginResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Server Config Error: Missing GOOGLE_CLIENT_ID in .env"
                    });
                }

                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);

                if (payload == null)
                {
                    return View("LogIn", new MDUA.Entities.UserLoginResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Google authentication failed (Invalid Token)."
                    });
                }

                // B. Find User
                var user = _userLoginFacade.GetUserByEmail(payload.Email);

                if (user == null)
                {
                    return View("LogIn", new MDUA.Entities.UserLoginResult
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Access Denied. No account found for {payload.Email}."
                    });
                }

                // C. Get Full Context
                var loginResult = _userLoginFacade.GetUserLoginById(user.Id);
                if (!loginResult.IsSuccess)
                {
                    return View("LogIn", new MDUA.Entities.UserLoginResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Account is disabled or invalid."
                    });
                }

                // D. 2FA Check
                if (user.IsTwoFactorEnabled)
                {
                    TempData["PreAuthUserId"] = user.Id;
                    TempData["RememberMe"] = true;
                    TempData["ReturnUrl"] = "/";
                    TempData["LoginMethod"] = "Google";

                    return RedirectToAction("VerifyTwoFactor");
                }

                // E. Complete Login
                // Ensure your DB has the 'LoginMethod' column or this will throw!
                await CompleteSignInAsync(loginResult, rememberMe: true, loginMethod: "Google");

                return RedirectToAction("Dashboard", "Home");

            }
            catch (Google.Apis.Auth.InvalidJwtException)
            {
                return View("LogIn", new MDUA.Entities.UserLoginResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Security Alert: Invalid Google Token received."
                });
            }
            catch (Exception ex)
            {
                // Check your server logs/console for the real error details
                Console.WriteLine($"Google Login Error: {ex.Message}");

                return View("LogIn", new MDUA.Entities.UserLoginResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"System Error: {ex.Message}"
                });
            }
        }
        [HttpGet]
        public IActionResult AccessDenied(string missingPermission = null)
        {
            ViewBag.MissingPermission = missingPermission;
            ViewBag.UserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "No Role";
            return View();
        }




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