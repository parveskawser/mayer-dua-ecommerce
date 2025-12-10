using MDUA.Facade;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace MDUA.Web.UI.Controllers
{
    [Authorize]
    public class SettingsController : BaseController
    {
        private readonly ISettingsFacade _settingsFacade;

        public SettingsController(ISettingsFacade settingsFacade)
        {
            _settingsFacade = settingsFacade;
        }

        [HttpGet]
        public IActionResult PaymentSettings()
        {
            // Uses the custom result class
            var model = _settingsFacade.GetCompanyPaymentSettings(CurrentCompanyId);
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
    }
}