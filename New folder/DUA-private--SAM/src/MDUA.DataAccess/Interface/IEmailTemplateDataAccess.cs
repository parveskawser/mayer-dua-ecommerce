using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDUA.DataAccess.Interface
{
    public interface IEmailTemplateDataAccess : ICommonDataAccess<Entities.EmailTemplate, Entities.List.EmailTemplateList, Entities.Bases.EmailTemplateBase>
    {
    }
}
