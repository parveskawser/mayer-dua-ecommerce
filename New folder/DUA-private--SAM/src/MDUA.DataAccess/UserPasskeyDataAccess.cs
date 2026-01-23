using MDUA.DataAccess.Interface;
using MDUA.Framework.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDUA.DataAccess.Interface;
namespace MDUA.DataAccess
{
    public partial class UserPasskeyDataAccess : BaseDataAccess, IUserPasskeyDataAccess
    {
        public void DeleteUserPasskey(int id)
        {
            Delete(id);
        }
    }
}
