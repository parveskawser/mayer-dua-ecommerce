using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Entities;
using System;
namespace MDUA.Facade.Interface
{
    public interface IUserLoginFacade : ICommonFacade<UserLogin, UserLoginList, UserLoginBase>
    {
        UserLoginResult GetUserLoginBy(string email, string password);

        // fetch user by Id
        UserLoginResult GetUserLoginById(int userId);

        bool IsEmailExists(string email);
        UserLogin GetUserByEmail(string email);
        List<string> GetUserPermissionNamesByUserId(int userId);
        bool IsUserAuthorized(int userId, string actionName);
        List<string> GetAllUserPermissionNames(int userId);

        Guid CreateUserSession(int userId, string ipAddress, string deviceInfo, string loginMethod); bool IsSessionValid(Guid sessionKey);
        void InvalidateSession(Guid sessionKey);

        (string secretKey, string qrCodeUri) SetupTwoFactor(string username);
        bool EnableTwoFactor(int userId, string secret, string codeInput);
        bool VerifyTwoFactor(string dbSecret, string codeInput);

        void ForceLogoutAllSessions(int userId);

        bool VerifyTwoFactorByUserId(int userId, string codeInput);

        void DisableTwoFactor(int userId);
        void InvalidateAllUserSessions(int userId);
        UserLogin GetUserByUsername(string username);
        void UpdatePassword(int userId, string newPassword);


        UserPasskeyResult GetPasskeyByCredentialId(byte[] credentialId);
        void UpdatePasskeyCounter(int id, uint counter);

        void AddUserPasskey(UserPasskey passkey);
        List<UserPasskey> GetPasskeysByUserId(int userId);
        void DeleteUserPasskey(int id);
    }
}