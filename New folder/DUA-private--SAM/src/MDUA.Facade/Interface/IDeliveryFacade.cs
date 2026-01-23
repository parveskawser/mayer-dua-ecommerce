using MDUA.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MDUA.Facade.DeliveryFacade;

namespace MDUA.Facade.Interface
{
    public interface IDeliveryFacade
    {
        /// <summary>
        /// Retrieves all deliveries including their items and order details.
        /// </summary>
        IList<Delivery> GetAllDeliveries(int companyId);
        Delivery Get(int id);
        /// <summary>
        /// Updates the status and tracking number of a specific delivery.
        /// </summary>
        /// 
        List<CompanyCarrierOptionDto> GetActiveCompanyCarrierOptions(int companyId);
        ShipmentModalDto GetShipmentModalData(int deliveryId, int companyId);


        Task<CourierShipmentResult> ShipWithCarrierAsync(
            int deliveryId,
            int companyId,
            int companyCarrierId,
            string user);

        List<CourierCredentialRowDto> GetCourierCredentialSettings(int companyId);
        void SaveCourierCredential(int companyId, SaveCourierCredentialRequest req, string user);
        Task<CourierConnectionResult> TestCourierConnectionAsync(int companyId, int companyCarrierId);
            void ToggleCourierActive(int companyId, int companyCarrierId, bool isActive);
    }
}