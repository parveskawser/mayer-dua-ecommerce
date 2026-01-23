using Fido2NetLib;
using Fido2NetLib.Objects;
using MDUA.Entities;
using MDUA.Facade;
using MDUA.Facade.Interface;
using MDUA.Web.UI.Services.Interface;
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
using Microsoft.Extensions.Configuration; // ADD THIS
using System.Net.Http; // ADD THIS
using System.Text.Json; // ADD THIS
using System.Threading.Tasks; // ADD THIS
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
        private readonly IProductFacade _productFacade;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public SettingsController(
            ISettingsFacade settingsFacade,
            IPaymentFacade paymentFacade,
            IUserLoginFacade userLoginFacade,
            IFido2 fido2,
            ICompanyFacade companyFacade, IWebHostEnvironment webHostEnvironment,
            IProductFacade productFacade,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory

            )
        {
            _settingsFacade = settingsFacade;
            _paymentFacade = paymentFacade;
            _userLoginFacade = userLoginFacade;
            _fido2 = fido2;
            _companyFacade = companyFacade;
            _webHostEnvironment = webHostEnvironment;
            _productFacade = productFacade;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;

        }

        #region setting payment methods
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
        #endregion

        #region Security Settings
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
        // ✅ 1. GET: Show Company Profile (Updated to fetch Favicon)

        #endregion

        #region Company Profile Settings
        [HttpGet]
        [Route("settings/company-profile")]
        public IActionResult CompanyProfile()
        {
            var company = _companyFacade.Get(CurrentCompanyId);

            // Fetch Favicon from Global Settings
            string faviconUrl = _settingsFacade.GetGlobalSetting(CurrentCompanyId, "FaviconUrl");
            ViewBag.FaviconUrl = faviconUrl;
            // ✅ Fetch Footer Description
            string footerDesc = _settingsFacade.GetGlobalSetting(CurrentCompanyId, "Footer_Description");
            // Set default if empty
            if (string.IsNullOrEmpty(footerDesc))
            {
                footerDesc = "Your one-stop shop for the best quality products. We ensure authentic items, fast delivery, and excellent customer support.";
            }

            ViewBag.FooterDescription = footerDesc;
            return View(company);
        }

        // ✅ 2. POST: Update Profile & Upload Images (Logo + Favicon)
        [HttpPost]
        [Route("settings/company-profile")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(100 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)]
        public IActionResult UpdateCompanyProfile(
     string CompanyName,
     string Address,
     string Email,
     string Phone,
     string FooterDescription,
     IFormFile LogoFile,
     IFormFile FaviconFile)
        {
            try
            {
                // 1. Get existing data
                var company = _companyFacade.Get(CurrentCompanyId);
                if (company == null) return Json(new { success = false, message = "Company not found." });

                // 2. Update properties
                // We set these on the object we pass to the Facade
                company.CompanyName = CompanyName;
                company.Address = Address; // ✅ Pass Address
                company.Email = Email;     // ✅ Pass Email
                company.Phone = Phone;     // ✅ Pass Phone

                // 3. Call Facade
                _companyFacade.UpdateCompanyProfile(company, LogoFile, FaviconFile, _webHostEnvironment.WebRootPath, FooterDescription);

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



        #endregion


        #region homepage settings

        [HttpGet]
        public IActionResult Homepage()
        {
            // 1. Determine Company ID
            int companyId = CurrentCompanyId;
            // 2. Get Homepage Config
            var config = _companyFacade.GetHomepageConfig(companyId);

            // 3. Get Categories
            var productData = _productFacade.GetAddProductData(0);
            ViewBag.Categories = productData.Categories
                .Select(c => new { Id = c.Id, Name = c.Name })
                .ToList();

            // 4. Load Company Info with FALLBACK Defaults
            var company = _companyFacade.Get(companyId);

            if (company != null)
            {
                if (string.IsNullOrEmpty(company.CompanyName)) company.CompanyName = "MDUA Store";
                if (string.IsNullOrEmpty(company.Address)) company.Address = "Dhaka, Bangladesh";
                if (string.IsNullOrEmpty(company.Email)) company.Email = "support@mduastore.com";
                if (string.IsNullOrEmpty(company.Phone)) company.Phone = "+880 1234 567 890";
            }
            ViewBag.CompanyInfo = company;

            // 5. Load Footer Description
            string footerDesc = _settingsFacade.GetGlobalSetting(companyId, "Footer_Description");

            if (string.IsNullOrEmpty(footerDesc))
            {
                footerDesc = "Your one-stop shop for the best quality products. We ensure authentic items, fast delivery, and excellent customer support.";
            }
            ViewBag.FooterDescription = footerDesc;
            ViewBag.HomepageSeo = LoadHomepageSeo(companyId);

            return View(config);
        }

        [HttpPost]
        [Route("settings/save-homepage-seo")]
        public IActionResult SaveHomepageSeo([FromBody] HomepageSeo seo)
        {
            try
            {
                string json = System.Text.Json.JsonSerializer.Serialize(seo ?? new HomepageSeo());
                _settingsFacade.SaveGlobalSetting(CurrentCompanyId, "Homepage_SEO", json);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("settings/upload-homepage-seo-image")]
        public async Task<IActionResult> UploadHomepageSeoImage(IFormFile file, string previousUrl)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Json(new { success = false, message = "No file selected" });

                string folderName = Path.Combine("images", "seo", "homepage");
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderName);

                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                if (!string.IsNullOrEmpty(previousUrl) && previousUrl.Contains("/images/seo/homepage/"))
                {
                    try
                    {
                        string fileName = Path.GetFileName(previousUrl);
                        string oldPath = Path.Combine(uploadsFolder, fileName);
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }
                    catch
                    {
                        // ignore delete errors
                    }
                }

                string uniqueFileName = $"home_og_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return Json(new { success = true, url = $"/images/seo/homepage/{uniqueFileName}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private HomepageSeo LoadHomepageSeo(int companyId)
        {
            string json = _settingsFacade.GetGlobalSetting(companyId, "Homepage_SEO");
            if (!string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    return System.Text.Json.JsonSerializer.Deserialize<HomepageSeo>(json);
                }
                catch
                {
                    // ignore malformed JSON
                }
            }

            return new HomepageSeo();
        }
        [HttpPost]
        public IActionResult SaveHomepage([FromBody] HomepageConfig config)
        {
            try
            {
                int companyId = CurrentCompanyId;
                _companyFacade.SaveHomepageConfig(companyId, config);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult SaveDraft([FromBody] HomepageConfig config)
        {
            try
            {
                // Save to a DIFFERENT key (Homepage_Draft) so we don't break the live site
                string json = System.Text.Json.JsonSerializer.Serialize(config);
                _companyFacade.SaveGlobalSetting(CurrentCompanyId, "Homepage_Draft", json);
                return Json(new { success = true });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPost]
        public IActionResult ResetToDefault()
        {
            try
            {
                // Implement DeleteSetting in your Facade/DataAccess
                // Or just save an empty string/null to "Homepage_Layout"
                _companyFacade.SaveGlobalSetting(CurrentCompanyId, "Homepage_Layout", "");
                return Json(new { success = true });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }


        [HttpPost]
        [Route("Settings/UploadBanner")]
        public async Task<IActionResult> UploadBanner(IFormFile file, string previousUrl)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Json(new { success = false, message = "No file selected" });

                // CHANGE: Use a neutral folder name like "sliders" to bypass AdBlockers
                string folderName = "sliders";
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", folderName);

                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                // 1. DELETE OLD (Safe Check)
                if (!string.IsNullOrEmpty(previousUrl)
                    && !previousUrl.Contains("placehold.co")
                    // CHANGE: Check if the previous URL matches our new folder structure
                    && previousUrl.Contains($"/images/{folderName}/"))
                {
                    try
                    {
                        string fileName = Path.GetFileName(previousUrl);
                        string oldPath = Path.Combine(uploadsFolder, fileName);
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }
                    catch { /* Ignore delete errors */ }
                }

                // 2. SAVE NEW
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // CHANGE: Return the new path
                return Json(new { success = true, url = $"/images/{folderName}/" + uniqueFileName });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server Error: " + ex.Message });
            }
        }

        #endregion

        //
        #region analytics settings
        [HttpGet]
        [Route("settings/analytics")]
        public IActionResult Analytics()
        {
            // Fetch existing settings
            ViewBag.GoogleAnalyticsId = _settingsFacade.GetGlobalSetting(CurrentCompanyId, "GoogleAnalyticsId");
            ViewBag.FacebookPixelId = _settingsFacade.GetGlobalSetting(CurrentCompanyId, "FacebookPixelId");

            return View();
        }

        [HttpPost]
        [Route("settings/save-analytics")]
        [ValidateAntiForgeryToken]
        public IActionResult SaveAnalytics(string googleAnalyticsId, string facebookPixelId)
        {
            try
            {
                // Save Google Analytics ID
                _settingsFacade.SaveGlobalSetting(CurrentCompanyId, "GoogleAnalyticsId", googleAnalyticsId?.Trim());

                // Save Facebook Pixel ID
                _settingsFacade.SaveGlobalSetting(CurrentCompanyId, "FacebookPixelId", facebookPixelId?.Trim());

                return Json(new { success = true, message = "Analytics settings updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        [Route("settings/verify-recaptcha")]
        [AllowAnonymous] // Important: Allow this even if user is not logged in yet (for Login page)
        public async Task<IActionResult> VerifyRecaptcha([FromBody] RecaptchaRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return Json(new { success = false, message = "Token is missing" });
            }

            try
            {
                // 1. Get Secret Key
                string secretKey = _configuration["RecaptchaSettings:SecretKey"];
                if (string.IsNullOrEmpty(secretKey))
                {
                    // Fallback for debugging if config is missing
                    return Json(new { success = false, message = "Server config missing" });
                }

                // 2. Prepare Request to Google
                var client = _httpClientFactory.CreateClient();
                var googleUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={request.Token}";

                // 3. Send Request
                var response = await client.PostAsync(googleUrl, null);
                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = "Google service error" });
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<RecaptchaResponse>(jsonString);

                // 4. Evaluate Score
                // Score 0.0 (Bot) -> 1.0 (Human)
                if (result.Success && result.Score >= 0.5)
                {
                    return Json(new { success = true, score = result.Score });
                }
                else
                {
                    return Json(new { success = false, score = result.Score, message = "Bot detected" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Helper Classes
        public class RecaptchaRequest
        {
            public string Token { get; set; }
        }

        public class RecaptchaResponse
        {
            [System.Text.Json.Serialization.JsonPropertyName("success")]
            public bool Success { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("score")]
            public double Score { get; set; }
        }

        #endregion



        [HttpGet]
        [Route("Settings/GetFooterConfig")]
        public IActionResult GetFooterConfig()
        {
            // Fetch JSON config or return a Default Layout if empty
            var json = _settingsFacade.GetGlobalSetting(CurrentCompanyId, "Footer_Config");
            if (string.IsNullOrEmpty(json))
            {
                return Json(new { success = false, message = "No config found" });
            }
            return Content(json, "application/json");
        }

        [HttpPost]
        [Route("Settings/SaveFooterConfig")]
        public IActionResult SaveFooterConfig([FromBody] object config)
        {
            try
            {
                string json = System.Text.Json.JsonSerializer.Serialize(config);
                _settingsFacade.SaveGlobalSetting(CurrentCompanyId, "Footer_Config", json);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Settings/SavePageContent")]
        public IActionResult SavePageContent(string slug, string content)
        {
            try
            {
                if (string.IsNullOrEmpty(slug)) return Json(new { success = false, message = "Slug is required" });

                // Save to GlobalSetting with "Page_" prefix
                string key = "Page_" + slug.ToLower();
                _settingsFacade.SaveGlobalSetting(CurrentCompanyId, key, content);

                return Json(new { success = true, url = $"/page/{slug}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("Settings/GetPageContent")]
        public IActionResult GetPageContent(string slug)
        {
            string key = "Page_" + slug.ToLower();
            string content = _settingsFacade.GetGlobalSetting(CurrentCompanyId, key);
            return Json(new { success = true, content = content });
        }

    }
}
