using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess.Interface
{
    public interface IChatMessageDataAccess : ICommonDataAccess<ChatMessage, ChatMessageList, ChatMessageBase>
    {
    }
}