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
        private readonly IUserLoginDataAccess _userDataAccess;
        public SettingsFacade(ICompanyPaymentMethodDataAccess dataAccess, IGlobalSettingDataAccess globalSettingDataAccess, IUserLoginDataAccess userDataAccess)
        {
            _dataAccess = dataAccess;
            _globalSettingDataAccess = globalSettingDataAccess;
            _userDataAccess = userDataAccess;

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

        public void ChangePassword(int userId, string oldPassword, string newPassword)
        {
            // A. Get the user
            var user = _userDataAccess.Get(userId);
            if (user == null) throw new Exception("User not found.");

            // B. Check Old Password (Direct String Comparison)
            // Note: This matches exact text (Case-Sensitive usually, depending on DB collation)
            if (user.Password != oldPassword)
            {
                throw new Exception("Current password is incorrect.");
            }

            // C. Set New Password (Plain Text)
            user.Password = newPassword;

            // D. Save to Database
            _userDataAccess.Update(user);
        }

        public UserLogin GetUserById(int userId)
        {
            // We use the existing Get method from your DataAccess layer
            return _userDataAccess.Get(userId);
        }

        // Inject IGlobalSettingDataAccess in constructor if not already present
        // private readonly IGlobalSettingDataAccess _globalSettingDataAccess;

        public string GetGlobalSetting(int companyId, string key)
        {
            // Assuming you have implemented GetSetting in GlobalSettingDataAccess as discussed
            return _globalSettingDataAccess.GetSetting(companyId, key);
        }

    }
}