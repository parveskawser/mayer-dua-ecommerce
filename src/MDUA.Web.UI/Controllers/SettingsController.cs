using Fido2NetLib;
using Fido2NetLib.Objects;
using MDUA.Facade;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace MDUA.Web.UI.Controllers
{
    [Authorize]
    public class SettingsController : BaseController
    {
        private readonly ISettingsFacade _settingsFacade;
        private readonly IPaymentFacade _paymentFacade;
        private readonly IUserLoginFacade _userLoginFacade;
        private readonly IFido2 _fido2;
        private readonly ICompanyFacade _companyFacade;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SettingsController(
            ISettingsFacade settingsFacade,
            IPaymentFacade paymentFacade,
            IUserLoginFacade userLoginFacade,
            IFido2 fido2,
            ICompanyFacade companyFacade, IWebHostEnvironment webHostEnvironment // Injected

            )
        {
            _settingsFacade = settingsFacade;
            _paymentFacade = paymentFacade;
            _userLoginFacade = userLoginFacade;
            _fido2 = fido2;
            _companyFacade = companyFacade;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [Route("settings/payment-method")]
        public IActionResult PaymentSettings()
        {
            var model = _settingsFacade.GetCompanyPaymentSettings(CurrentCompanyId);

            // Pass delivery charges via ViewBag or extend your ViewModel
            var delivery = _settingsFacade.GetDeliverySettings(CurrentCompanyId);
            ViewBag.DeliveryDhaka = delivery["dhaka"];
            ViewBag.DeliveryOutside = delivery["outside"];

            return View(model);
        }

        [HttpPost]
        [Route("settings/payment-method/save")]
        [ValidateAntiForgeryToken]
        public IActionResult SavePaymentConfig(int methodId, bool isEnabled, bool isManual, bool isGateway, string instruction)
        {
            try
            {
                _settingsFacade.SavePaymentConfig(
                    CurrentCompanyId,
                    methodId,
                    isEnabled,
                    isManual,
                    isGateway,
                    instruction,
                    CurrentUserName
                );
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        [HttpPost]
        [Route("settings/payment-method/save-delivery")]
        [ValidateAntiForgeryToken]
        public IActionResult SaveDeliverySettings(int dhakaCharge, int outsideCharge)
        {
            try
            {
                _settingsFacade.SaveDeliverySettings(CurrentCompanyId, dhakaCharge, outsideCharge);
                return Json(new { success = true, message = "Delivery charges updated!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        //  Security Settings Page

        [HttpGet]
        [Route("settings/security")]
        public IActionResult Security()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("LogIn", "Account");

            int userId = int.Parse(userIdClaim.Value);

            var userResult = _userLoginFacade.GetUserLoginById(userId);
            ViewBag.IsTwoFactorEnabled = userResult.UserLogin.IsTwoFactorEnabled;

            if (!userResult.UserLogin.IsTwoFactorEnabled)
            {
                var setupInfo = _userLoginFacade.SetupTwoFactor(userResult.UserLogin.UserName);
                ViewBag.ManualEntryKey = setupInfo.secretKey;
                ViewBag.QrCodeImage =
                    $"https://api.qrserver.com/v1/create-qr-code/?size=200x200&data={Uri.EscapeDataString(setupInfo.qrCodeUri)}";
            }

            var passkeys = _userLoginFacade.GetPasskeysWithDeviceNames(userId);
            ViewBag.PasskeyList = passkeys;
            ViewBag.HasPasskeys = passkeys != null && passkeys.Any();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnableTwoFactor(string entryKey, string code)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                bool success = _userLoginFacade.EnableTwoFactor(userId, entryKey, code);
                if (success) return Json(new { success = true });
                return Json(new { success = false, message = "Invalid authentication code." });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword, bool LogoutAllDevices)
        {
            try
            {
                // 1. Get User ID
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Json(new { success = false, message = "Session expired." });
                }

                // 2. Validate
                if (string.IsNullOrEmpty(CurrentPassword) || string.IsNullOrEmpty(NewPassword))
                    return Json(new { success = false, message = "All fields are required." });

                if (NewPassword != ConfirmPassword)
                    return Json(new { success = false, message = "New passwords do not match." });

                // 3. Change Password
                _settingsFacade.ChangePassword(userId, CurrentPassword, NewPassword);

                // 4. Handle Logout Logic
                if (LogoutAllDevices)
                {
                    // A. Invalidate ALL sessions in DB
                    // (You need to add this method to your Facade, see below)
                    _userLoginFacade.InvalidateAllUserSessions(userId);

                    // B. Sign out the current cookie immediately
                    await HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);

                    return Json(new { success = true, redirect = true }); // Signal frontend to redirect
                }

                return Json(new { success = true, redirect = false });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult StartResetVia2FA()
        {
            // 1. Get ID
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToAction("LogIn", "Account");
            }

            // 2. Get User (This line will now work)
            var user = _settingsFacade.GetUserById(userId);

            if (user == null || !user.IsTwoFactorEnabled)
            {
                // Use TempData to show an alert on the settings page if 2FA is missing
                TempData["ErrorMessage"] = "You must enable 2FA first to use this feature.";
                return RedirectToAction("Index");
            }

            // 3. Log Out
            HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);

            // 4. Setup Reset Flow
            TempData["ResetUserId"] = userId;
            TempData["ResetUsername"] = user.UserName;

            // 5. Redirect to the 2FA Verify Screen
            return RedirectToAction("VerifyReset2FA", "Account");
        }

        [HttpPost]
[Route("Settings/MakeCredentialOptions")]
        [ValidateAntiForgeryToken]
        public IActionResult MakeCredentialOptions()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                // 1. Business Logic: Enforce 2-device limit
                var existingKeys = _userLoginFacade.GetPasskeysByUserId(userId);
                if (existingKeys != null && existingKeys.Count >= 2)
                {
                    return BadRequest(new { message = "Maximum of 2 devices allowed. Please remove one to add another." });
                }

                var user = _userLoginFacade.Get(userId);
                var fidoUser = new Fido2User
                {
                    Id = Encoding.UTF8.GetBytes(user.Id.ToString()),
                    Name = user.Email,
                    DisplayName = user.UserName
                };

                var excludeCredentials = existingKeys.Select(k => new PublicKeyCredentialDescriptor(k.CredentialId)).ToList();

                var options = _fido2.RequestNewCredential(new RequestNewCredentialParams
                {
                    User = fidoUser,
                    ExcludeCredentials = excludeCredentials,
                    AuthenticatorSelection = new AuthenticatorSelection
                    {
                        ResidentKey = ResidentKeyRequirement.Preferred,
                        UserVerification = UserVerificationRequirement.Preferred
                    },
                    AttestationPreference = AttestationConveyancePreference.None
                });

                HttpContext.Session.SetString("fido2.attestationOptions", options.ToJson());
                return Content(options.ToJson(), "application/json");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost]
        [Route("Settings/MakeCredential")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeCredential([FromBody] PasskeyRegistrationRequest request)
        {
            try
            {
                var jsonOptions = HttpContext.Session.GetString("fido2.attestationOptions");
                if (string.IsNullOrEmpty(jsonOptions)) return BadRequest(new { message = "Session expired" });

                var options = CredentialCreateOptions.FromJson(jsonOptions);

                // 1. Verify the Attestation
                var result = await _fido2.MakeNewCredentialAsync(new MakeNewCredentialParams
                {
                    AttestationResponse = request.AttestationResponse,
                    OriginalOptions = options,
                    IsCredentialIdUniqueToUserCallback = (args, cancellationToken) =>
                    {
                        var exists = _userLoginFacade.GetPasskeyByCredentialId(args.CredentialId) != null;
                        return Task.FromResult(!exists);
                    }
                });

                // 2. Business Logic: Device Detection
                string userAgent = Request.Headers["User-Agent"].ToString();
                string detectedDevice;

                if (request.AuthenticatorAttachment == "platform")
                {
                    // Registration happened on THIS device - trust User-Agent
                    detectedDevice = ParseDeviceFromUserAgent(userAgent);
                }
                else
                {
                    // Phone/security key was used remotely
                    detectedDevice = "Android Phone (Remote)";
                }

                // 3. Save to DB with Metadata
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _userLoginFacade.AddUserPasskey(new MDUA.Entities.UserPasskey
                {
                    UserId = userId,
                    CredentialId = result.Id,
                    PublicKey = result.PublicKey,
                    SignatureCounter = (int)result.SignCount,
                    CredType = "public-key",
                    RegDate = DateTime.UtcNow,
                    AaGuid = result.AaGuid,
                    FriendlyName = string.IsNullOrWhiteSpace(request.FriendlyName) ? null : request.FriendlyName,
                    DeviceType = detectedDevice
                });

                HttpContext.Session.Remove("fido2.attestationOptions");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class PasskeyRegistrationRequest
        {
            public AuthenticatorAttestationRawResponse AttestationResponse { get; set; }
            public string FriendlyName { get; set; }
            public string AuthenticatorAttachment { get; set; } 

        }
        private string ParseDeviceFromUserAgent(string ua)
        {
            if (ua.Contains("iPhone")) return "iPhone";
            if (ua.Contains("Android")) return "Android Device";
            if (ua.Contains("Windows")) return "Windows PC";
            if (ua.Contains("Macintosh")) return "MacBook";
            if (ua.Contains("Linux")) return "Linux Device";
            return "Unknown Device";
        }
       

        // ✅ 1. GET: Show Company Profile (Updated to fetch Favicon)


        [HttpGet]
        [Route("settings/company-profile")]
        public IActionResult CompanyProfile()
        {
            var company = _companyFacade.Get(CurrentCompanyId);

            // Fetch Favicon from Global Settings
            string faviconUrl = _settingsFacade.GetGlobalSetting(CurrentCompanyId, "FaviconUrl");
            ViewBag.FaviconUrl = faviconUrl;

            return View(company);
        }

        // ✅ 2. POST: Update Profile & Upload Images (Logo + Favicon)
        [HttpPost]
        [Route("settings/company-profile")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(100 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)]
        public IActionResult UpdateCompanyProfile(string CompanyName, IFormFile LogoFile, IFormFile FaviconFile)
        {
            try
            {
                // 1. Get existing data
                var company = _companyFacade.Get(CurrentCompanyId);
                if (company == null) return Json(new { success = false, message = "Company not found." });

                // 2. Update Name
                if (!string.IsNullOrEmpty(CompanyName))
                {
                    company.CompanyName = CompanyName;
                }

                // 3. Update Audit Fields
                company.UpdatedBy = CurrentUserName;
                company.UpdatedAt = DateTime.UtcNow;

                // 4. Call Facade to handle Files & Database Updates
                // This handles saving Logo to [Company] and Favicon to [GlobalSetting]
                _companyFacade.UpdateCompanyProfile(company, LogoFile, FaviconFile, _webHostEnvironment.WebRootPath);

                // 5. Return Success (Frontend will reload or update visuals)
                return Json(new
                {
                    success = true,
                    message = "Profile updated successfully!",
                    newLogoUrl = company.LogoImg,
                    newName = company.CompanyName
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteSinglePasskey(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                // Securely delete only if the key belongs to the logged-in user
                _userLoginFacade.DeleteSpecificUserPasskey(id, userId);
                return Json(new { success = true, message = "Device removed successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]

        [ValidateAntiForgeryToken]

        public IActionResult DisablePasskeys()

        {

            try

            {

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var keys = _userLoginFacade.GetPasskeysByUserId(userId);

                foreach (var key in keys)

                {

                    _userLoginFacade.DeleteUserPasskey(key.Id);

                }

                return Json(new { success = true, message = "All passkeys removed." });

            }

            catch (Exception ex)

            {

                return Json(new { success = false, message = ex.Message });

            }

        }

    }
}