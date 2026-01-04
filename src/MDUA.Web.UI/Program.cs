using DotNetEnv;
using Fido2NetLib;
using MDUA.Facade;
using MDUA.Facade.Interface;
using MDUA.Web.UI.Hubs;
using MDUA.Web.UI.Services;
using MDUA.Web.UI.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
var builder = WebApplication.CreateBuilder(args);
Env.Load();

// 2. Load nvironment Variables into Configuration
builder.Configuration.AddEnvironmentVariables();
//Transactions.TransactionManager.ImplicitDistributedTransactions = true;
builder.Services.AddService();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<IAiChatService, SmartGeminiChatService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IExportService, ExportService>();
// ✅ Add SignalR Service
builder.Services.AddSignalR();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
        Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;

    // TRUST EVERYTHING (Only safe if the server is not directly exposed to the internet, 
    // but sits behind IIS/Nginx). exact setting depends on your setup.
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

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

                // B. Resolve Services
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
                        // ✅ VALID SESSION
                        var userIdClaim = context.Principal.FindFirst(ClaimTypes.NameIdentifier);
                        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                        {
                            var identity = (ClaimsIdentity)context.Principal.Identity;

                            // ---------------------------------------------------------
                            // E. REFRESH USER DATA (Username & CompanyId)
                            // ---------------------------------------------------------

                            // 1. Fetch the actual User Object from DB
                            var freshUser = userFacade.Get(userId);

                            if (freshUser != null)
                            {
                                // 2. Refresh Username (ClaimTypes.Name)
                                var oldNameClaim = identity.FindFirst(ClaimTypes.Name);
                                if (oldNameClaim != null)
                                {
                                    identity.RemoveClaim(oldNameClaim);
                                }
                                // Add fresh name
                                identity.AddClaim(new Claim(ClaimTypes.Name, freshUser.UserName));

                                var oldCompanyClaim = identity.FindFirst("CompanyId");
                                if (oldCompanyClaim != null)
                                {
                                    identity.RemoveClaim(oldCompanyClaim); // Remove stale company
                                }
                                identity.AddClaim(new Claim("CompanyId", freshUser.CompanyId.ToString()));
                            }

                            // ---------------------------------------------------------
                            // F. REFRESH PERMISSIONS
                            // ---------------------------------------------------------

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

builder.Services.AddSingleton<IFido2>(_ =>
{
    // 1. Load raw values
    var serverDomain = builder.Configuration["Fido2:ServerDomain"];
    var serverName = builder.Configuration["Fido2:ServerName"];
    var originsStr = builder.Configuration["Fido2:Origins"];
    var driftConfig = builder.Configuration["Fido2:TimestampDriftTolerance"];
    var driftTolerance = int.Parse(driftConfig ?? "300000");

    // 2. Parse Origins
    var originsSet = new HashSet<string>(originsStr?.Split(',') ?? Array.Empty<string>());

    // 3. Check for missing config (Graceful Failure)
    if (string.IsNullOrEmpty(serverDomain) || originsSet.Count == 0)
    {
        // LOG the error, but DO NOT CRASH the app.
        Console.WriteLine("****************************************************************");
        Console.WriteLine("WARNING: Fido2 configuration is missing! Passkeys will not work.");
        Console.WriteLine("Check your .env file or environment variables.");
        Console.WriteLine("****************************************************************");

        // Use fallbacks to prevent null reference crashes later, 
        // though Fido2 logic will likely reject requests.
        serverDomain = serverDomain ?? "localhost";
        serverName = serverName ?? "Unknown App";
    }

    return new Fido2(new Fido2Configuration
    {
        ServerDomain = serverDomain,
        ServerName = serverName,
        Origins = new HashSet<string>(originsStr?.Split(',') ?? Array.Empty<string>()),
        TimestampDriftTolerance = driftTolerance
    });
});
var app = builder.Build();

// 🔍 STARTUP CONFIGURATION DEEP DIVE
// =========================================================
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine($"\n[CONFIG] Environment: {app.Environment.EnvironmentName}");
Console.WriteLine("[CONFIG] Loaded Configuration Providers:");

// 👇 This loop prints EVERY source .NET is reading from (JSON, Secrets, Env, etc.)
foreach (var provider in ((IConfigurationRoot)app.Configuration).Providers)
{
    Console.WriteLine($"   - {provider}");
}
Console.ResetColor();

var config = app.Configuration;
string apiKey = config["TextBee:ApiKey"];
string deviceId = config["TextBee:DeviceId"];

if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(deviceId))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("\n/***************************************************/");
    Console.WriteLine(" ❌ CRITICAL ERROR: SMS Configuration is MISSING!");
    Console.WriteLine(" ---------------------------------------------------");
    Console.WriteLine($" Key 'TextBee:ApiKey'   : {(string.IsNullOrWhiteSpace(apiKey) ? "MISSING" : "FOUND")}");
    Console.WriteLine($" Key 'TextBee:DeviceId' : {(string.IsNullOrWhiteSpace(deviceId) ? "MISSING" : "FOUND")}");
    Console.WriteLine("/***************************************************/\n");
    Console.ResetColor();
}
else
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"\n✅ SUCCESS: SMS Config Loaded! (Device: {deviceId})\n");
    Console.ResetColor();
}
// =========================================================

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