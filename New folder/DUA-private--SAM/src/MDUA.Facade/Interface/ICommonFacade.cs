using MDUA.Framework;

namespace MDUA.Facade.Interface
{
    public interface ICommonFacade<out T, out TL, in TB>
    {
        long Delete(int id);
        T Get(int id);
        TL GetAll();
        TL GetByQuery(string query);
         
        long Insert(TB dto);
        long Update(TB dto);
    }
}
