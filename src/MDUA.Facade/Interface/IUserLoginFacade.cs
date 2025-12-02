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
    }
}