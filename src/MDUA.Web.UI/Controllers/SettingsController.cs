using MDUA.Facade;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Claims;

namespace MDUA.Web.UI.Controllers
{
    [Authorize]
    public class SettingsController : BaseController
    {
        private readonly ISettingsFacade _settingsFacade;
        private readonly IPaymentFacade _paymentFacade;
        private readonly IUserLoginFacade _userLoginFacade;
        public SettingsController(ISettingsFacade settingsFacade, IPaymentFacade paymentFacade, IUserLoginFacade userLoginFacade)
        {
            _settingsFacade = settingsFacade;
            _paymentFacade = paymentFacade;
            _userLoginFacade = userLoginFacade;

        }

        [HttpGet]
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

        // ✅ NEW: Security Settings Page
        [HttpGet]
        // No changes needed here, but ensure this is your code:
        [HttpGet]
        public IActionResult Security()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToAction("LogIn", "Account");

            int userId = int.Parse(userIdClaim.Value);

            // Now this returns the User with IsTwoFactorEnabled = true
            var userResult = _userLoginFacade.GetUserLoginById(userId);

            // This will now be TRUE
            ViewBag.IsTwoFactorEnabled = userResult.UserLogin.IsTwoFactorEnabled;

            // The QR code logic will be skipped if enabled
            if (!userResult.UserLogin.IsTwoFactorEnabled)
            {
                var setupInfo = _userLoginFacade.SetupTwoFactor(userResult.UserLogin.UserName);
                ViewBag.ManualEntryKey = setupInfo.secretKey;
                ViewBag.QrCodeImage = $"https://api.qrserver.com/v1/create-qr-code/?size=200x200&data={Uri.EscapeDataString(setupInfo.qrCodeUri)}";
            }

            return View();
        }
        // ✅ NEW: Enable 2FA Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableTwoFactor(string entryKey, string code)
        {
            bool success = _userLoginFacade.EnableTwoFactor(CurrentUserId, entryKey, code);

            if (success)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Json(new { success = true, message = "2FA Enabled. Please log in again." });
            }
            return Json(new { success = false, message = "Invalid Code. Please try again." });
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
    }
}