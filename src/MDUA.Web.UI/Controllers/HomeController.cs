using MDUA.Entities;
using MDUA.Facade;
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
        private readonly IOrderFacade _orderFacade; // new


        public HomeController(IUserLoginFacade userLoginFacade, IProductFacade productFacade, ILogger<HomeController> logger, IOrderFacade orderFacade)
        {
            _userLoginFacade = userLoginFacade;
            _productFacade = productFacade;
            _logger = logger;
            _orderFacade = orderFacade;
        }

        public IActionResult Index()
        {
            if (IsLoggedIn) return RedirectToAction("Dashboard");

            return RedirectToAction("LogIn", "Account");
        }



        //change
        [Authorize]
        [HttpGet]
        public IActionResult Dashboard()
        {
            int userId = CurrentUserId;

            // 1. Get Basic Login Info
            var loginResult = _userLoginFacade.GetUserLoginById(userId);
            loginResult.AuthorizedActions = _userLoginFacade.GetAllUserPermissionNames(userId);

            loginResult.CanViewProducts = loginResult.AuthorizedActions.Contains("Product.View");
            bool canAddProduct = loginResult.AuthorizedActions.Contains("Product.Add");

            // 2. Load Products
            if (loginResult.CanViewProducts)
                loginResult.LastFiveProducts = _productFacade.GetLastFiveProducts();

            if (canAddProduct)
            {
                var addProductData = _productFacade.GetAddProductData(userId);
                loginResult.Categories = addProductData.Categories;
                loginResult.Attributes = addProductData.Attributes;
            }

            // 3. ✅ LOAD DASHBOARD STATS & ORDERS
            try
            {
                loginResult.Stats = _orderFacade.GetDashboardMetrics();
                loginResult.RecentOrders = _orderFacade.GetRecentOrders();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load dashboard stats");
                // Initialize empty to prevent null reference in view
                loginResult.Stats = new DashboardStats();
                loginResult.RecentOrders = new List<SalesOrderHeader>();
            }

            return View(loginResult);
        }
    }
}