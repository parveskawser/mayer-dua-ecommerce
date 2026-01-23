using Fido2NetLib;
using Fido2NetLib.Objects;
using MDUA.Facade.Interface;
using MDUA.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MDUA.Web.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasskeyController : BaseController
    {
        private readonly IFido2 _fido2;
        private readonly IUserLoginFacade _userLoginFacade;
        private readonly IConfiguration _config;

        public PasskeyController(
            IFido2 fido2,
            IUserLoginFacade userLoginFacade,
            IConfiguration config)
        {
            _fido2 = fido2;
            _userLoginFacade = userLoginFacade;
            _config = config;
        }

        // =========================================================
        // 1. GENERATE LOGIN OPTIONS
        // =========================================================
        [HttpPost("assertionOptions")]
        public ActionResult LoginOptions([FromBody] string username)
        {
            try
            {
                // You may later filter credentials by username
                var existingCredentials = new List<PublicKeyCredentialDescriptor>();

                // FIXED: In Fido2 v4, GetAssertionOptions takes GetAssertionOptionsParams
                var options = _fido2.GetAssertionOptions(
                    new GetAssertionOptionsParams
                    {
                        AllowedCredentials = existingCredentials,
                        UserVerification = UserVerificationRequirement.Preferred
                    }
                );

                HttpContext.Session.SetString(
                    "fido2.assertionOptions",
                    options.ToJson()
                );

                return Content(options.ToJson(), "application/json");
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    message = "Failed to generate options",
                    error = e.Message
                });
            }
        }

        // =========================================================
        // 2. VERIFY ASSERTION (LOGIN)
        // =========================================================
        [HttpPost("makeAssertion")]
        public async Task<IActionResult> LoginVerify(
            [FromBody] AuthenticatorAssertionRawResponse clientResponse)
        {
            try
            {
                var jsonOptions =
                    HttpContext.Session.GetString("fido2.assertionOptions");

                if (string.IsNullOrEmpty(jsonOptions))
                {
                    return BadRequest(new
                    {
                        message = "Session expired. Please try again."
                    });
                }

                var options = AssertionOptions.FromJson(jsonOptions);

                // =========================================================
                // 2.1 GET PASSKEY BY RAW BINARY ID
                // =========================================================
                var passkey =
                    _userLoginFacade.GetPasskeyByCredentialId(
                        clientResponse.RawId);

                if (passkey == null)
                {
                    return Unauthorized(new
                    {
                        message = "Passkey not found."
                    });
                }

                // =========================================================
                // 2.2 VERIFY ASSERTION (FIDO2 v4)
                // =========================================================
                // IsUserHandleOwnerOfCredentialIdAsync requires 2 parameters
                var result = await _fido2.MakeAssertionAsync(
                    new MakeAssertionParams
                    {
                        AssertionResponse = clientResponse,
                        OriginalOptions = options,
                        StoredPublicKey = passkey.PublicKey,
                        StoredSignatureCounter = (uint)passkey.SignatureCounter,

                        //  2 parameters (args, cancellationToken)
                        IsUserHandleOwnerOfCredentialIdCallback =
                            (args, cancellationToken) =>
                            {
                                return Task.FromResult(
                                    args.UserHandle.SequenceEqual(
                                        System.Text.Encoding.UTF8.GetBytes(
                                            passkey.UserId.ToString()
                                        )
                                    )
                                );
                            }
                    }
                );

                // =========================================================
                // 2.3 UPDATE SIGNATURE COUNTER (v4)
                // =========================================================
                _userLoginFacade.UpdatePasskeyCounter(
                    passkey.Id,
                    result.SignCount
                );

                HttpContext.Session.Remove("fido2.assertionOptions");

                // =========================================================
                // 3. CREATE APP SESSION (UNCHANGED)
                // =========================================================
                var userResult =
                    _userLoginFacade.GetUserLoginById(passkey.UserId);

                var user = userResult.UserLogin;

                Guid sessionKey =
                    _userLoginFacade.CreateUserSession(
                        user.Id,
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString(),
                        "Passkey"
                    );

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName ?? user.Email),
                    new Claim("SessionKey", sessionKey.ToString()),
                    new Claim("AuthMethod", "Passkey")
                };

                foreach (var p in _userLoginFacade.GetAllUserPermissionNames(user.Id))
                {
                    claims.Add(new Claim("Permission", p));
                }

                var claimsIdentity =
                    new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                    );

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
                    }
                );

                return Ok(new
                {
                    success = true,
                    redirectUrl = "/Dashboard"
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new
                {
                    message = "Verification failed",
                    error = ex.Message
                });
            }
        }
    }
}