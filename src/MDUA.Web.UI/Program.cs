using MDUA.Facade;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// 1. Register Services and Facades
builder.Services.AddService();
builder.Services.AddControllersWithViews();

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
                        // This ensures that if you remove a permission in DB, it applies INSTANTLY,
                        // even if the user doesn't log out.

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
                            // This hits the DB again, but ensures 100% accuracy.
                            var freshPermissions = userFacade.GetAllUserPermissionNames(userId);

                            // 3. Add NEW permissions to the current request
                            foreach (var permissionName in freshPermissions)
                            {
                                identity.AddClaim(new Claim("Permission", permissionName));
                            }

                            // Note: We modify the 'identity' in memory for THIS request.
                            // We do not strictly need to rewrite the cookie here, because we
                            // will just fetch fresh data again on the next request.
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

var app = builder.Build();

// 3. Middlewares
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();