using System.Collections.Generic;
using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Facade.Interface;

namespace MDUA.Facade
{
    public class SettingsFacade : ISettingsFacade
    {
        private readonly ICompanyPaymentMethodDataAccess _dataAccess;

        public SettingsFacade(ICompanyPaymentMethodDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public List<CompanyPaymentMethodResult> GetCompanyPaymentSettings(int companyId)
        {
            // Cast to concrete class to access partial method 'GetMergedSettings'
            return ((CompanyPaymentMethodDataAccess)_dataAccess).GetMergedSettings(companyId);
        }

        public void SavePaymentConfig(int companyId, int methodId, bool isActive, bool isManual, bool isGateway, string instruction, string username)
        {
            // Cast to concrete class to access partial method 'SaveConfiguration'
            ((CompanyPaymentMethodDataAccess)_dataAccess).SaveConfiguration(companyId, methodId, isActive, isManual, isGateway, instruction, username);
        }
    }
}