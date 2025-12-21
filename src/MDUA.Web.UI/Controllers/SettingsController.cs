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

    }
}