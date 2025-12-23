using DotNetEnv;
using MDUA.Facade;
using MDUA.Facade.Interface;

using MDUA.Web.UI.Hubs;
using MDUA.Web.UI.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
var builder = WebApplication.CreateBuilder(args);
Env.Load();

// 2. Load nvironment Variables into Configuration
builder.Configuration.AddEnvironmentVariables();
System.Transactions.TransactionManager.ImplicitDistributedTransactions = true;
// 1. Register Services and Facades
builder.Services.AddService();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<IAiChatService, SmartGeminiChatService>();

// ✅ NEW: Add SignalR Service
builder.Services.AddSignalR();

// 2. Configure Authentication with "Real World" Validation
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/LogIn";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;

        // ✅ THE MAGIC EVENT: Runs on every single request
        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = async context =>
            {
                // A. Check for SessionKey
                var sessionClaim = context.Principal.FindFirst("SessionKey");
                if (sessionClaim == null)
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return;
                }

                // B. Resolve Services (Cannot use Constructor Injection here)
                var userFacade = context.HttpContext.RequestServices.GetRequiredService<IUserLoginFacade>();

                // C. Parse Session Key
                if (Guid.TryParse(sessionClaim.Value, out Guid sessionKey))
                {
                    // D. Check if Session is still Active in DB
                    bool isValid = userFacade.IsSessionValid(sessionKey);

                    if (!isValid)
                    {
                        // ❌ BANNED: Kill the request immediately
                        context.RejectPrincipal();
                        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    }
                    else
                    {
                        // ✅ VALID: Now force-refresh Permissions (Real-Time Authorization)
                        var userIdClaim = context.Principal.FindFirst(ClaimTypes.NameIdentifier);
                        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                        {
                            var identity = (ClaimsIdentity)context.Principal.Identity;

                            // 1. Remove OLD permissions (stale data from cookie)
                            var oldPermissionClaims = identity.FindAll("Permission").ToList();
                            foreach (var oldClaim in oldPermissionClaims)
                            {
                                identity.RemoveClaim(oldClaim);
                            }

                            // 2. Fetch NEW permissions from DB
                            var freshPermissions = userFacade.GetAllUserPermissionNames(userId);

                            // 3. Add NEW permissions to the current request
                            foreach (var permissionName in freshPermissions)
                            {
                                identity.AddClaim(new Claim("Permission", permissionName));
                            }
                        }
                    }
                }
                else
                {
                    // Invalid Guid format
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            }
        };
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache(); // Stores session in memory
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

// 3. Middlewares
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // 👈 THIS MUST BE HERE
app.UseAuthentication();
app.UseAuthorization();

// ✅ NEW: Map the SignalR Hub
// This creates the endpoint "/supportHub" that combo.js connects to
app.MapHub<SupportHub>("/supportHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();