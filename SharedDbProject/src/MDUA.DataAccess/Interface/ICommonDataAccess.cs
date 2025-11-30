using MDUA.Framework;

namespace MDUA.DataAccess;

public interface ICommonDataAccess<T, L, B>
{
    long Delete(int _Id);
    T Get(int _Id);
    L GetAll();
    L GetByQuery(string query);
    int GetMaxId();
    L GetPaged(PagedRequest request);
    int GetRowCount();
    long Insert(B Object);
    long Update(B Object);
}