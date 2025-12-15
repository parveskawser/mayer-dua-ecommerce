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
        private  readonly IPaymentFacade _paymentFacade;
        public SettingsController(ISettingsFacade settingsFacade, IPaymentFacade paymentFacade)
        {
            _settingsFacade = settingsFacade;
            _paymentFacade = paymentFacade;
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
    }
}