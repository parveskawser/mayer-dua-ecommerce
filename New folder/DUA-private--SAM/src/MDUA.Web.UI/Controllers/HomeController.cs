using MDUA.Entities;
using MDUA.Facade;
using MDUA.Facade.Interface;
using MDUA.DataAccess.Interface;
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
        private readonly ISettingsFacade _settingsFacade; // ✅ 1. Add Field
        private readonly IBackInStockRequestDataAccess _backInStockRequestDataAccess;

        public HomeController(
            IUserLoginFacade userLoginFacade,
            IProductFacade productFacade,
            ILogger<HomeController> logger,
            IOrderFacade orderFacade,
            ICompanyFacade companyFacade,
            ISettingsFacade settingsFacade,
            IBackInStockRequestDataAccess backInStockRequestDataAccess)
        {
            _userLoginFacade = userLoginFacade;
            _productFacade = productFacade;
            _logger = logger;
            _orderFacade = orderFacade;
            _companyFacade = companyFacade;
            _settingsFacade = settingsFacade;
            _backInStockRequestDataAccess = backInStockRequestDataAccess;
        }

        public IActionResult Index(bool preview = false)
        {
            int companyId = CurrentCompanyId;
            ViewBag.CurrentCompanyId = companyId;
            ViewBag.FaviconUrl = _settingsFacade.GetFavicon(companyId);
            ViewBag.HomepageSeo = LoadHomepageSeo(companyId);
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
                var productData = _productFacade.GetAddProductData(companyId);
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

        private HomepageSeo LoadHomepageSeo(int companyId)
        {
            var json = _settingsFacade.GetGlobalSetting(companyId, "Homepage_SEO");
            if (!string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    return System.Text.Json.JsonSerializer.Deserialize<HomepageSeo>(json);
                }
                catch
                {
                    // ignore malformed JSON and fallback to defaults
                }
            }

            return new HomepageSeo();
        }

        [Route("dashboard")]
        //change
        [Authorize]
        [HttpGet]
        public IActionResult Dashboard()
        {
            int userId = CurrentUserId;
            // 1. Get Company ID
            int companyId = Convert.ToInt32(User.FindFirst("CompanyId")?.Value ?? "1");

            var loginResult = _userLoginFacade.GetUserLoginById(userId);
            loginResult.AuthorizedActions = _userLoginFacade.GetAllUserPermissionNames(userId);
            loginResult.CanViewProducts = loginResult.AuthorizedActions.Contains("Product.View");
            bool canAddProduct = loginResult.AuthorizedActions.Contains("Product.Add");

            if (loginResult.CanViewProducts)
                loginResult.LastFiveProducts = _productFacade.GetLastFiveProducts(companyId); // Update this too if needed

            if (canAddProduct)
            {
                var add = _productFacade.GetAddProductData(userId);
                loginResult.Categories = add.Categories;
                loginResult.Attributes = add.Attributes;
            }

            // ✅ LOAD DASHBOARD DATA (Stats, Orders, Charts)
            try
            {
                // Pass companyId to all methods
                loginResult.Stats = _orderFacade.GetDashboardMetrics(companyId);
                loginResult.RecentOrders = _orderFacade.GetRecentOrders(companyId);
                loginResult.SalesTrend = _orderFacade.GetSalesTrend(companyId);
                loginResult.OrderStatusCounts = _orderFacade.GetOrderStatusCounts(companyId);
                loginResult.LowStockItems = _productFacade.GetLowStockVariants(companyId, 5);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load dashboard data");
                loginResult.Stats = new DashboardStats();
                loginResult.RecentOrders = new List<SalesOrderHeader>();
                loginResult.SalesTrend = new List<ChartDataPoint>();
                loginResult.OrderStatusCounts = new List<ChartDataPoint>();
                loginResult.LowStockItems = new List<LowStockItem>();
            }

            try
            {
                loginResult.BackInStockRequestCount = _backInStockRequestDataAccess.GetPendingRequestCount(companyId);
                loginResult.BackInStockRequests = _backInStockRequestDataAccess.GetPendingRequestSummaries(companyId, 5);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load back-in-stock request summary");
                loginResult.BackInStockRequestCount = 0;
                loginResult.BackInStockRequests = new List<BackInStockRequestSummary>();
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
                model.Categories = _productFacade.GetAddProductData(companyId).Categories.ToList();
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
