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
        private readonly IOrderFacade _orderFacade; 


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


        [Route("dashboard")]
        //change
        [Authorize]
        [HttpGet]
        public IActionResult Dashboard()
        {
            int userId = CurrentUserId;
            var loginResult = _userLoginFacade.GetUserLoginById(userId);

            // ... (Permissions & Products loading) ...
            loginResult.AuthorizedActions = _userLoginFacade.GetAllUserPermissionNames(userId);
            loginResult.CanViewProducts = loginResult.AuthorizedActions.Contains("Product.View");
            bool canAddProduct = loginResult.AuthorizedActions.Contains("Product.Add");

            if (loginResult.CanViewProducts)
                loginResult.LastFiveProducts = _productFacade.GetLastFiveProducts();

            if (canAddProduct)
            {
                var add = _productFacade.GetAddProductData(userId);
                loginResult.Categories = add.Categories;
                loginResult.Attributes = add.Attributes;
            }

            // ✅ LOAD DASHBOARD DATA (Stats, Orders, Charts)
            try
            {
                loginResult.Stats = _orderFacade.GetDashboardMetrics();
                loginResult.RecentOrders = _orderFacade.GetRecentOrders();

                // Load Chart Data
                loginResult.SalesTrend = _orderFacade.GetSalesTrend();
                loginResult.OrderStatusCounts = _orderFacade.GetOrderStatusCounts();
                loginResult.LowStockItems = _productFacade.GetLowStockVariants(5);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load dashboard data");
                // Init empty to avoid nulls
                loginResult.Stats = new DashboardStats();
                loginResult.RecentOrders = new List<SalesOrderHeader>();
                loginResult.SalesTrend = new List<ChartDataPoint>();
                loginResult.OrderStatusCounts = new List<ChartDataPoint>();
                loginResult.LowStockItems = new List<LowStockItem>(); // ✅ Init empty list
            }

            return View(loginResult);
        }
    }
}