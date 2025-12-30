using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDUA.DataAccess.Interface
{
    public interface IEmailHistoryDataAccess : ICommonDataAccess<Entities.EmailHistory, Entities.List.EmailHistoryList, Entities.Bases.EmailHistoryBase>
    {
    }
}
