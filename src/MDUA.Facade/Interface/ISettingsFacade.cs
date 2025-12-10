using System.Collections.Generic;
using MDUA.Entities;

namespace MDUA.Facade.Interface
{
    public interface ISettingsFacade
    {
        List<CompanyPaymentMethodResult> GetCompanyPaymentSettings(int companyId);
        void SavePaymentConfig(int companyId, int methodId, bool isActive, bool isManual, bool isGateway, string instruction, string username);
    }
}