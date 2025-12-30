using MDUA.Entities;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MDUA.Web.UI.Controllers;

namespace MDUA.Web.UI.Controllers
{
    public class ShipmentController : BaseController
    {
        private readonly IDeliveryFacade _deliveryFacade;
        private readonly IOrderFacade _orderFacade; // ✅ Inject OrderFacade for status updates
        private readonly IDeliveryStatusLogFacade _logFacade;
        public ShipmentController(IDeliveryFacade deliveryFacade, IOrderFacade orderFacade, IDeliveryStatusLogFacade logFacade)
        {
            _deliveryFacade = deliveryFacade;
            _orderFacade = orderFacade;
            _logFacade = logFacade;

        }
        [Route("delivery/all")]

        [HttpGet]
        public IActionResult DeliveryList()
        {
            IList<Delivery> list = _deliveryFacade.GetAllDeliveries();
            return View(list);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int deliveryId, string status)
        {
            try
            {
                // 1. Get Old Info
                var delivery = _deliveryFacade.Get(deliveryId);
                string oldStatus = delivery?.Status ?? "Unknown";
                int? salesOrderId = delivery?.SalesOrderId;

                // 2. Perform Update (This also handles order sync inside Facade)
                _orderFacade.UpdateDeliveryStatus(deliveryId, status);

                // 3. LOG DELIVERY CHANGE
                if (oldStatus != status)
                {
                    _logFacade.LogStatusChange(
                        entityId: deliveryId,
                        entityType: "Delivery",
                        oldStatus: oldStatus,
                        newStatus: status,
                        changedBy: User.Identity.Name ?? "Admin",
                        orderId: salesOrderId, 
                        reason: "Manual Delivery Update via Shipment Manager"
                    );
                }

                // Note: The Order might have been auto-updated by _orderFacade.UpdateDeliveryStatus.
                // Ideally, the logic inside _orderFacade.UpdateDeliveryStatus should call _logFacade internally 
                // to log the "System Auto-Sync" event for the Order itself. 
                // Since we are avoiding Facade changes right now, logging the Delivery change here is the priority.

                return Json(new { success = true });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    

}
}