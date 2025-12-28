using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDUA.Facade.Interface
{
    public interface IAiChatService
    {
        Task<string> GetResponseAsync(string userMessage, List<string> history, int? contextProductId = null);
    }
}