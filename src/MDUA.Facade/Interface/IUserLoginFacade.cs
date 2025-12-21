using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Entities;
using System;
namespace MDUA.Facade.Interface
{
    public interface IUserLoginFacade : ICommonFacade<UserLogin, UserLoginList, UserLoginBase>
    {
        UserLoginResult GetUserLoginBy(string email, string password);

        // New method to fetch user by Id
        UserLoginResult GetUserLoginById(int userId);

        bool IsEmailExists(string email);

        List<string> GetUserPermissionNamesByUserId(int userId);
        bool IsUserAuthorized(int userId, string actionName);
        List<string> GetAllUserPermissionNames(int userId);

        Guid CreateUserSession(int userId, string ipAddress, string deviceInfo);
        bool IsSessionValid(Guid sessionKey);
        void InvalidateSession(Guid sessionKey);

        (string secretKey, string qrCodeUri) SetupTwoFactor(string username);
        bool EnableTwoFactor(int userId, string secret, string codeInput);
        bool VerifyTwoFactor(string dbSecret, string codeInput);

        void ForceLogoutAllSessions(int userId);

        bool VerifyTwoFactorByUserId(int userId, string codeInput);

        void DisableTwoFactor(int userId);
    }
}