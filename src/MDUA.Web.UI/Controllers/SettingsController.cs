using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MDUA.Web.UI.Controllers
{
    public class SettingsController : BaseController
    {
        private readonly IPaymentMethodFacade _paymentMethodFacade;
       // private readonly ICompanyPaymentMethodFacade _companyPaymentMethodFacade;

        public SettingsController(
            IPaymentMethodFacade paymentMethodFacade)
        {
            _paymentMethodFacade = paymentMethodFacade;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
