using MDUA.Entities;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;

namespace MDUA.Web.UI.Controllers
{
    
    public class HomeController : BaseController
    {
        private readonly IUserLoginFacade _userLoginFacade;
        private readonly IProductFacade _productFacade;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IUserLoginFacade userLoginFacade, IProductFacade productFacade, ILogger<HomeController> logger)
        {
            _userLoginFacade = userLoginFacade;
            _productFacade = productFacade;
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (IsLoggedIn) return RedirectToAction("Dashboard");

            return RedirectToAction("LogIn", "Account");
        }

        [Authorize] 
        [HttpGet]
        public IActionResult Dashboard()
        {
            int userId = CurrentUserId;

            var loginResult = _userLoginFacade.GetUserLoginById(userId);

            loginResult.AuthorizedActions = _userLoginFacade.GetAllUserPermissionNames(userId);

            loginResult.CanViewProducts = loginResult.AuthorizedActions.Contains("Product.View");
            bool canAddProduct = loginResult.AuthorizedActions.Contains("Product.Add");

            if (loginResult.CanViewProducts)
                loginResult.LastFiveProducts = _productFacade.GetLastFiveProducts();

            if (canAddProduct)
            {
                var addProductData = _productFacade.GetAddProductData(userId);
                loginResult.Categories = addProductData.Categories;
                loginResult.Attributes = addProductData.Attributes;
            }

            return View(loginResult);
        }
    }
}