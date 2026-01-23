using MDUA.Facade.Interface;
using MDUA.Web.UI.Services.Interface;
using Microsoft.Extensions.Caching.Memory;

namespace MDUA.Web.UI.Services
{
    public class TenantResolver : ITenantResolver
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly ICompanyFacade _companyFacade;
        private readonly IMemoryCache _cache;

        public TenantResolver(
            IHttpContextAccessor httpContext,
            ICompanyFacade companyFacade,
            IMemoryCache cache)
        {
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            _companyFacade = companyFacade ?? throw new ArgumentNullException(nameof(companyFacade));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public int GetCompanyId()
        {
            // 1. Priority: Logged-in users get their own company
            var httpContext = _httpContext.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var companyIdClaim = httpContext.User.FindFirst("CompanyId");
                if (companyIdClaim != null && int.TryParse(companyIdClaim.Value, out int companyId))
                {
                    return companyId;
                }
            }

            // 2. Anonymous users: Resolve from domain
            string host = httpContext?.Request.Host.Host?.ToLower() ?? "localhost";

            // 3. Normalize domain (remove www.)
            if (host.StartsWith("www."))
            {
                host = host.Substring(4);
            }

            // 4. Check cache first (performance optimization)
            string cacheKey = $"Tenant_Domain_{host}";

            return _cache.GetOrCreate(cacheKey, entry =>
            {
                // Cache for 60 minutes
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

                int dbCompanyId = _companyFacade.GetCompanyIdByDomain(host);

                // 6. Log resolution for debugging
                Console.WriteLine($"[TenantResolver] Domain '{host}' → CompanyId: {dbCompanyId}");

                return dbCompanyId > 0 ? dbCompanyId : 1;
            });
        }
    }
}