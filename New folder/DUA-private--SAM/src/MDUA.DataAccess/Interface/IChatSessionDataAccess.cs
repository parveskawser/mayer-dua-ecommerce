using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess.Interface
{
    public interface IChatSessionDataAccess : ICommonDataAccess<ChatSession, ChatSessionList, ChatSessionBase>
    {
    }
}