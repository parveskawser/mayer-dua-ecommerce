using MDUA.Entities;
using MDUA.Facade;
using MDUA.Facade.Interface;
using MDUA.Web.UI.Models;
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
        private readonly ICompanyFacade _companyFacade; // ✅ 1. Add Field

        public HomeController(IUserLoginFacade userLoginFacade, IProductFacade productFacade, ILogger<HomeController> logger, IOrderFacade orderFacade, ICompanyFacade companyFacade)
        {
            _userLoginFacade = userLoginFacade;
            _productFacade = productFacade;
            _logger = logger;
            _orderFacade = orderFacade;
            _companyFacade = companyFacade;

        }

        public IActionResult Index(bool preview = false)
        {
            int companyId = (User.Identity.IsAuthenticated && CurrentCompanyId > 0) ? CurrentCompanyId : 1;

            HomepageConfig config = null;

            // 1. Load Config (Draft or Live)
            if (preview && User.Identity.IsAuthenticated)
            {
                config = _companyFacade.GetHomepageDraftConfig(companyId);
            }
            else
            {
                config = _companyFacade.GetHomepageConfig(companyId);
            }

            // ✅ CRITICAL FIX: If database returned null, create a new object so we don't crash
            if (config == null)
            {
                config = new HomepageConfig();
            }

            ViewBag.IsPreview = preview;

            // 2. Load Categories
            try
            {
                var productData = _productFacade.GetAddProductData(0);
                config.Categories = productData.Categories.ToList();
            }
            catch
            {
                config.Categories = new List<MDUA.Entities.ProductCategory>();
            }

            // 3. Ensure Sections list is never null
            if (config.Sections == null)
            {
                config.Sections = new List<MDUA.Entities.HomepageSection>();
            }

            return View(config);
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

        [Route("all-products")]
        public IActionResult Shop(int? category, string query) // ✅ Added 'query' parameter
        {
            int companyId = 1;
            var companyClaim = User.FindFirst("CompanyId");
            if (companyClaim != null && int.TryParse(companyClaim.Value, out int cid))
            {
                companyId = cid;
            }

            var model = new LandingPageViewModel();

            try
            {
                // ✅ Pass 'query' to the Facade
                model.NewArrivals = _productFacade.GetShopData(companyId, category, query);

                // Load Categories for Sidebar (Helper to handle login check safely)
                model.Categories = _productFacade.GetAddProductData(0).Categories.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading shop data");
            }

            // Keep state for View (Sidebar active state & Search UI)
            ViewBag.CurrentCategory = category;
            ViewBag.SearchQuery = query;

            return View(model);
        }
    }
}