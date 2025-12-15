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
        private readonly IGlobalSettingDataAccess _globalSettingDataAccess;
        public SettingsFacade(ICompanyPaymentMethodDataAccess dataAccess, IGlobalSettingDataAccess globalSettingDataAccess)
        {
            _dataAccess = dataAccess;
            _globalSettingDataAccess = globalSettingDataAccess;
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

        #region Delivery Settings
        public Dictionary<string, int> GetDeliverySettings(int companyId)
        {
            var dCharge = _globalSettingDataAccess.GetValue(companyId, "DeliveryCharge_Dhaka");
            var oCharge = _globalSettingDataAccess.GetValue(companyId, "DeliveryCharge_Outside");

            int.TryParse(dCharge, out int d);
            int.TryParse(oCharge, out int o);

            return new Dictionary<string, int> { { "dhaka", d }, { "outside", o } };
        }

        public void SaveDeliverySettings(int companyId, int dhaka, int outside)
        {
            _globalSettingDataAccess.SaveValue(companyId, "DeliveryCharge_Dhaka", dhaka.ToString());
            _globalSettingDataAccess.SaveValue(companyId, "DeliveryCharge_Outside", outside.ToString());
        }
        #endregion

    }
}