using System.Threading.Tasks;

namespace MDUA.Facade.Interface
{
    public interface ISmsService
    {
        Task<bool> SendSmsAsync(string phoneNumber, string message);
    }
}