using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Facade.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDUA.Facade
{
    public class DeliveryFacade : IDeliveryFacade
    {
        private readonly IDeliveryDataAccess _deliveryDataAccess;
       

        public DeliveryFacade(IDeliveryDataAccess deliveryDataAccess)
        {
            _deliveryDataAccess = deliveryDataAccess;
        }
        // Inside MDUA.Facade/DeliveryFacade.cs

        public Delivery Get(int id)
        {
         
            if (_deliveryDataAccess is MDUA.DataAccess.DeliveryDataAccess concreteDA)
            {
                return concreteDA.GetExtended(id);
            }

            return _deliveryDataAccess.Get(id);
        }
        public IList<Delivery> GetAllDeliveries(int companyId)
        {
            return _deliveryDataAccess.LoadAllWithDetails(companyId);
        }

        public ShipmentModalDto GetShipmentModalData(int deliveryId, int companyId)
        {
            if (_deliveryDataAccess is MDUA.DataAccess.DeliveryDataAccess concreteDA)
                return concreteDA.GetShipmentModalData(deliveryId, companyId);

            throw new System.Exception("DeliveryDataAccess concrete implementation is required for shipment modal.");
        }

        public List<CompanyCarrierOptionDto> GetActiveCompanyCarrierOptions(int companyId)
        {
            if (_deliveryDataAccess is MDUA.DataAccess.DeliveryDataAccess concreteDA)
                return concreteDA.GetActiveCompanyCarrierOptions(companyId);

            throw new System.Exception("DeliveryDataAccess concrete implementation is required for carrier dropdown.");
        }

        public async Task<CourierShipmentResult> CreateCarrierShipmentAsync(
               int deliveryId,
               int companyId,
               int companyCarrierId,
               string updatedBy)
        {
            if (_deliveryDataAccess is MDUA.DataAccess.DeliveryDataAccess da)
            {
                return await da.CreateCarrierShipmentAsync(deliveryId, companyId, companyCarrierId, updatedBy);
            }

            throw new System.Exception("Concrete DeliveryDataAccess required for shipment creation.");
        }
        public async Task<CourierShipmentResult> ShipWithCarrierAsync(
           int deliveryId,
           int companyId,
           int companyCarrierId,
           string user)
        {
            if (_deliveryDataAccess is MDUA.DataAccess.DeliveryDataAccess da)
            {
                return await da.CreateCarrierShipmentAsync(deliveryId, companyId, companyCarrierId, user);
            }

            throw new System.Exception("Concrete DeliveryDataAccess required for shipment creation.");
        }
        public List<CourierCredentialRowDto> GetCourierCredentialSettings(int companyId)
        {
            if (_deliveryDataAccess is MDUA.DataAccess.DeliveryDataAccess concreteDA)
            {
                return concreteDA.GetCourierCredentialSettings(companyId);
            }

            throw new System.Exception(
                "DeliveryDataAccess concrete implementation is required for courier credential settings."
            );
        }

        public void SaveCourierCredential(int companyId, SaveCourierCredentialRequest req, string user)
        {
            if (_deliveryDataAccess is MDUA.DataAccess.DeliveryDataAccess concreteDA)
            {
                concreteDA.SaveCourierCredential(companyId, req, user);
                return;
            }

            throw new System.Exception(
                "DeliveryDataAccess concrete implementation is required for saving courier credentials."
            );
        }
        // Inside MDUA.Facade/DeliveryFacade.cs

        public async Task<CourierConnectionResult> TestCourierConnectionAsync(int companyId, int companyCarrierId)
        {
            // 1. Get the UNMASKED credentials from DB using the new DA method
            CompanyCarrierCredentialDto creds = null;

            if (_deliveryDataAccess is MDUA.DataAccess.DeliveryDataAccess concreteDA)
            {
                creds = concreteDA.GetCompanyCarrierCredential(companyId, companyCarrierId);
            }
            else
            {
                throw new Exception("Concrete DataAccess required.");
            }

            if (creds == null)
                return new CourierConnectionResult { Success = false, Message = "Configuration not found." };

            string carrierName = (creds.CarrierName ?? "").Trim().ToLower();

            // 2. PATHAO IMPLEMENTATION
            if (carrierName.Contains("pathao"))
            {
                return await TestPathaoConnection(creds);
            }

            // 3. STEADFAST IMPLEMENTATION
            if (carrierName.Contains("steadfast"))
            {
                return await TestSteadfastConnection(creds);
            }

            return new CourierConnectionResult { Success = false, Message = $"No test logic implemented for {creds.CarrierName}" };
        }

        // Helper methods use 'CompanyCarrierCredentialDto' which has the real keys
        private async Task<CourierConnectionResult> TestPathaoConnection(CompanyCarrierCredentialDto creds)
        {
            if (string.IsNullOrWhiteSpace(creds.ApiKey) ||
                string.IsNullOrWhiteSpace(creds.ApiSecret) ||
                string.IsNullOrWhiteSpace(creds.ApiUsername) ||
                string.IsNullOrWhiteSpace(creds.ApiPassword))
            {
                return new CourierConnectionResult { Success = false, Message = "Pathao requires Client ID, Secret, Username, and Password." };
            }

            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    string baseUrl = string.IsNullOrWhiteSpace(creds.ApiEndpoint)
                        ? "https://api-hermes.pathao.com"
                        : creds.ApiEndpoint.TrimEnd('/');

                    var payload = new
                    {
                        client_id = creds.ApiKey,
                        client_secret = creds.ApiSecret,
                        username = creds.ApiUsername,
                        password = creds.ApiPassword,
                        grant_type = "password"
                    };

                    // Requires Newtonsoft.Json or System.Text.Json
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                    var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    var response = await client.PostAsync($"{baseUrl}/aladdin/api/v1/issue-token", content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(responseString);
                        if (data.access_token != null)
                            return new CourierConnectionResult { Success = true, Message = "Pathao Connection Successful!" };
                    }

                    return new CourierConnectionResult { Success = false, Message = $"Pathao Error: {responseString}" };
                }
            }
            catch (Exception ex)
            {
                return new CourierConnectionResult { Success = false, Message = "Connection Exception: " + ex.Message };
            }
        }

        private async Task<CourierConnectionResult> TestSteadfastConnection(CompanyCarrierCredentialDto creds)

        {

            if (string.IsNullOrWhiteSpace(creds.ApiKey) || string.IsNullOrWhiteSpace(creds.ApiSecret))

                return new CourierConnectionResult { Success = false, Message = "Steadfast requires API Key and Secret." };



            try

            {

                using (var client = new System.Net.Http.HttpClient())

                {

                    string baseUrl = "https://portal.steadfast.com.bd/api/v1";

                    client.DefaultRequestHeaders.Add("Api-Key", creds.ApiKey);

                    client.DefaultRequestHeaders.Add("Secret-Key", creds.ApiSecret);



                    var response = await client.GetAsync($"{baseUrl}/check_balance");



                    if (response.IsSuccessStatusCode)

                        return new CourierConnectionResult { Success = true, Message = "Steadfast Connection Successful!" };



                    return new CourierConnectionResult { Success = false, Message = $"Steadfast Error: {response.StatusCode}" };

                }

            }

            catch (Exception ex)

            {

                return new CourierConnectionResult { Success = false, Message = ex.Message };

            }

        }
        public void ToggleCourierActive(int companyId, int companyCarrierId, bool isActive)
{
    if (_deliveryDataAccess is MDUA.DataAccess.DeliveryDataAccess concreteDA)
    {
        concreteDA.UpdateCourierStatus(companyId, companyCarrierId, isActive);
    }
    else
    {
        throw new Exception("Concrete DataAccess required for toggling status.");
    }
}

    }

}
