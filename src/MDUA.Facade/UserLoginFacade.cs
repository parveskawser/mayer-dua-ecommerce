using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Framework;
using Microsoft.AspNetCore.DataProtection;
using OtpNet;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MDUA.Facade
{
    public class UserLoginFacade : IUserLoginFacade
    {
        private readonly IUserLoginDataAccess _UserLoginDataAccess;
        private readonly IUserPermissionDataAccess _userPermissionDataAccess;
        private readonly IPermissionDataAccess _permissionDataAccess;
        private readonly IPermissionGroupMapDataAccess _IPermissionGroupMapDataAccess;
        private readonly IPermissionGroupDataAccess _permissionGroupDataAccess;
        private readonly IUserSessionDataAccess _userSessionDataAccess;


        public UserLoginFacade(
            IUserLoginDataAccess userLoginDataAccess,
            IUserPermissionDataAccess userPermissionDataAccess,
            IPermissionDataAccess permissionDataAccess,
            IPermissionGroupMapDataAccess permissionGroupMapDataAccess,
            IUserSessionDataAccess userSessionDataAccess,
                IPermissionGroupDataAccess permissionGroupDataAccess)

        {
            _UserLoginDataAccess = userLoginDataAccess;
            _userPermissionDataAccess = userPermissionDataAccess;
            _permissionDataAccess = permissionDataAccess;
            _IPermissionGroupMapDataAccess = permissionGroupMapDataAccess;
            _permissionGroupDataAccess = permissionGroupDataAccess;
            _userSessionDataAccess = userSessionDataAccess;

        }

        #region common implementation 

        public long Delete(int _Id)
        {
            return _UserLoginDataAccess.Delete(_Id);
        }

        public UserLogin Get(int _Id)
        {
            return _UserLoginDataAccess.Get(_Id);
        }

        public UserLoginList GetAll()
        {
            return _UserLoginDataAccess.GetAll();
        }

        public UserLoginList GetByQuery(string query)
        {
            return _UserLoginDataAccess.GetByQuery(query);
        }

        public long Insert(UserLoginBase Object)
        {
            return _UserLoginDataAccess.Insert(Object);
        }
        public long Update(UserLoginBase Object)
        {
            return _UserLoginDataAccess.Update(Object);
        }

        #endregion

        #region extented implementation
        public bool IsEmailExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            // Option 1: If your DataAccess supports a specific query string
            // This is usually more efficient than fetching GetAll()
            string query = $"Email = '{email.Replace("'", "''")}'";
            var users = _UserLoginDataAccess.GetByQuery(query);

            if (users != null && users.Count > 0)
            {
                return true;
            }

            // Option 2: Fallback (Load all and check in memory - use only if Option 1 fails)
            /*
            var allUsers = _UserLoginDataAccess.GetAll();
            return allUsers.Any(u => u.Email.Trim().Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));
            */

            return false;
        }

        public UserLoginResult GetUserLoginById(int userId)
        {
            UserLoginResult result = new UserLoginResult();
            UserLogin obUser = _UserLoginDataAccess.Get(userId);
            if (obUser != null)
            {
                result.IsSuccess = true;
                result.UserLogin = obUser;
                result.ids = GetUserPermissionByUserId(userId);
                result.RoleName = GetUserRoleNames(obUser.Id);

                // Optional: mark admin if any role is "Admin"
                result.IsAdmin = result.RoleName.Split(',').Any(r => r.Trim().Equals("Admin", StringComparison.OrdinalIgnoreCase));

            }
            else
            {
                result.IsSuccess = false;
                result.ErrorMessage = "User not found";
            }

            return result;
        }


        public List<int> GetUserPermissionByUserId(int userId)
        {
            var permissions = _userPermissionDataAccess.GetByUserIdForFacade(userId);
            return permissions
                   .Where(p => p.PermissionId.HasValue)
                   .Select(p => p.PermissionId.Value)
                   .ToList();
        }


        public List<string> GetUserPermissionNamesByUserId(int userId)
        {
            var permissionIds = GetUserPermissionByUserId(userId); // already List<int>
            return _permissionDataAccess.GetByIds(permissionIds)
                                        .Select(p => p.Name)
                                        .ToList();
        }
        public UserLoginResult GetUserLoginBy(string email, string password)
        {
            UserLoginResult result = new UserLoginResult();
            var obUser = _UserLoginDataAccess.GetUserLogin(email, password);

            if (obUser == null)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Wrong user/password";
                return result;
            }

            result.IsSuccess = true;
            result.UserLogin = obUser;

            // Direct permissions
            // Direct permissions (user-specific)
            var directPermissionIds = GetUserPermissionByUserId(obUser.Id); // List<int>, already non-null

            // Role-based permissions
            List<int> rolePermissionIds = new List<int>();
            var roleIds = _userPermissionDataAccess.GetByUserIdForFacade(obUser.Id)
                            .Where(p => p.PermissionGroupId.HasValue)   // Only this one is nullable
                            .Select(p => p.PermissionGroupId.Value)
                            .Distinct()
                            .ToList();

            foreach (var roleId in roleIds)
            {
                var perms = _IPermissionGroupMapDataAccess.GetPermissionsByGroupId(roleId)
                                .Select(p => p.Id)  // p.Id is int, not nullable
                                .ToList();

                rolePermissionIds.AddRange(perms);
            }

            // Merge all permission IDs
            var allPermissionIds = directPermissionIds.Concat(rolePermissionIds)
                                                     .Distinct()
                                                     .ToList();

            // Get actual permission details
            result.AuthorizedActions = _permissionDataAccess.GetByIds(allPermissionIds)
                                                            .Select(p => p.Name)
                                                            .ToList();

            // Roles
            result.RoleName = GetUserRoleNames(obUser.Id);
            result.IsAdmin = result.RoleName.Split(',')
                                           .Any(r => r.Trim().Equals("Admin", StringComparison.OrdinalIgnoreCase));

            return result;
        }

        public string GetUserRoleNames(int userId)
        {
            var permissions = _userPermissionDataAccess.GetByUserIdForFacade(userId);

            // Get all PermissionGroupIds assigned to the user
            var roleIds = permissions
                .Where(p => p.PermissionGroupId.HasValue)
                .Select(p => p.PermissionGroupId.Value)
                .Distinct()
                .ToList();

            if (!roleIds.Any()) return "User"; // default role

            // Lookup all roles by Id
            var roles = roleIds
                .Select(id => _permissionGroupDataAccess.GetById(id))
                .Where(r => r != null)
                .Select(r => r.Name)
                .ToList();

            return string.Join(", ", roles); // comma-separated roles
        }


        public bool IsUserAuthorized(int userId, string actionName)
        {
            // Get all permission IDs for this user
            var permissionIds = GetUserPermissionByUserId(userId);

            // Check if the user has a permission with this action
            bool hasPermission = _permissionDataAccess.GetByIds(permissionIds)
                                    .Any(p => string.Equals(p.ActionName, actionName, StringComparison.OrdinalIgnoreCase));

            if (hasPermission) return true;

            // Check role-based permission
            var roles = _userPermissionDataAccess.GetByUserIdForFacade(userId)
                            .Where(p => p.PermissionGroupId.HasValue)
                            .Select(p => p.PermissionGroupId.Value)
                            .ToList();

            foreach (var roleId in roles)
            {
                var rolePermissions = _IPermissionGroupMapDataAccess.GetPermissionsByGroupId(roleId);
                if (rolePermissions.Any(p => string.Equals(p.ActionName, actionName, StringComparison.OrdinalIgnoreCase)))
                    return true;
            }

            return false; // No permission found
        }

        public List<string> GetAllUserPermissionNames(int userId)
        {
            // 1. Get direct permission IDs
            var directPermissionIds = GetUserPermissionByUserId(userId); // This method is correct

            // 2. Get role-based permission IDs
            List<int> rolePermissionIds = new List<int>();
            var roleIds = _userPermissionDataAccess.GetByUserIdForFacade(userId)
                                .Where(p => p.PermissionGroupId.HasValue)
                                .Select(p => p.PermissionGroupId.Value)
                                .Distinct()
                                .ToList();

            foreach (var roleId in roleIds)
            {
                var perms = _IPermissionGroupMapDataAccess.GetPermissionsByGroupId(roleId)
                                .Select(p => p.Id)
                                .ToList();
                rolePermissionIds.AddRange(perms);
            }

            // 3. Combine all IDs
            var allPermissionIds = directPermissionIds.Concat(rolePermissionIds)
                                                .Distinct()
                                                .ToList();

            // 4. Get the *NAMES* from the IDs
            return _permissionDataAccess.GetByIds(allPermissionIds)
                                        .Select(p => p.Name) // Get the Permission Name
                                        .ToList();
        }



        //  Create a session in SQL when user logs in
        public Guid CreateUserSession(int userId, string ipAddress, string deviceInfo)
        {
            Guid sessionKey = Guid.NewGuid();

            var session = new UserSessionBase
            {
                SessionKey = sessionKey,
                UserId = userId,
                IPAddress = ipAddress,
                DeviceInfo = deviceInfo,
                CreatedAt = DateTime.UtcNow,
                LastActiveAt = DateTime.UtcNow,
                IsActive = true
            };

            _userSessionDataAccess.Insert(session);
            return sessionKey;
        }

        //  Check if session is valid (runs on every request)
        public bool IsSessionValid(Guid sessionKey)
        {

            // NOTE: In a high-traffic real app, you might want a specialized "GetBySessionKey" SP for speed.
            var list = _userSessionDataAccess.GetByQuery($"SessionKey = '{sessionKey}' AND IsActive = 1");

            if (list != null && list.Count > 0)
            {
                return true;
            }
            return false;
        }

        public void InvalidateSession(Guid sessionKey)
        {
            var list = _userSessionDataAccess.GetByQuery($"SessionKey = '{sessionKey}'");
            if (list != null && list.Count > 0)
            {
                var session = list[0];
                session.IsActive = false;
                _userSessionDataAccess.Update(session);
            }
        }

        #endregion

        #region Two Factor Authentication (OATH / TOTP)

        public (string secretKey, string qrCodeUri) SetupTwoFactor(string username)
        {
            var key = KeyGeneration.GenerateRandomKey(20);
            string base32Secret = Base32Encoding.ToString(key);

            string issuer = "MDUA Admin";
            string label = $"{issuer}:{username}";

            // ✅ FIX: Use Uri.EscapeDataString to handle spaces/symbols correctly
            string uri = $"otpauth://totp/{Uri.EscapeDataString(label)}?secret={base32Secret}&issuer={Uri.EscapeDataString(issuer)}";

            return (base32Secret, uri);
        }
        public bool EnableTwoFactor(int userId, string secret, string codeInput)
        {
            if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(codeInput))
                return false;

            var bytes = Base32Encoding.ToBytes(secret);
            var totp = new Totp(bytes);

            var window = new VerificationWindow(previous: 1, future: 1);
            if (totp.VerifyTotp(codeInput.Trim(), out long matchedStep, window))
            {
                _UserLoginDataAccess.EnableTwoFactor(userId, secret);
                _UserLoginDataAccess.InvalidateAllSessionsByUser(userId);
                return true;
            }

            return false;
        }
        public void ForceLogoutAllSessions(int userId)
        {
            _UserLoginDataAccess.InvalidateAllSessionsByUser(userId);
        }


        public bool VerifyTwoFactor(string dbSecret, string codeInput)

        {

            if (string.IsNullOrWhiteSpace(dbSecret) || string.IsNullOrWhiteSpace(codeInput))

            {

                Console.WriteLine("[2FA] FAIL: secret/code empty");

                return false;

            }

            string secret = dbSecret.Trim().Replace(" ", "");

            string code = codeInput.Trim().Replace(" ", "").Replace("-", "");

            if (code.Length != 6)

            {

                Console.WriteLine($"[2FA] FAIL: code length {code.Length}, raw='{codeInput}'");

                return false;

            }

            try

            {

                var bytes = Base32Encoding.ToBytes(secret);

                var totp = new Totp(bytes);

                var utcNow = DateTime.UtcNow;

                var serverCodeNow = totp.ComputeTotp(utcNow);

                Console.WriteLine($"[2FA] UTC Now: {utcNow:O}");


                Console.WriteLine($"[2FA] ServerNow: {serverCodeNow}");


                var window = new VerificationWindow(previous: 1, future: 1);

                bool ok = totp.VerifyTotp(code, out long matchedStep, window);


                return ok;

            }

            catch (Exception ex)

            {

                Console.WriteLine($"[2FA] EXCEPTION: {ex.GetType().Name}: {ex.Message}");

                return false;

            }

        }

        public void DisableTwoFactor(int userId)
        {
            _UserLoginDataAccess.DisableTwoFactor(userId);
        }
        public bool VerifyTwoFactorByUserId(int userId, string code)
        {
            string secret = _UserLoginDataAccess.GetTwoFactorSecretByUserId(userId);
            return VerifyTwoFactor(secret, code);
        }



        #endregion

        public void InvalidateAllUserSessions(int userId)
        {
            // Pass this call down to DataAccess
            _UserLoginDataAccess.InvalidateAllSessions(userId);
        }
        public UserLogin GetUserByUsername(string username)
        {
            // Add this method to your DataAccess layer too
            return _UserLoginDataAccess.GetByUsername(username);
        }
        public void UpdatePassword(int userId, string newPassword)
        {
            // 1. Get User
            var user = _UserLoginDataAccess.Get(userId);
            if (user != null)
            {
                // 2. Update Password (Plain text based on your previous request)
                user.Password = newPassword;
                _UserLoginDataAccess.Update(user);
            }
        }
    }
}
