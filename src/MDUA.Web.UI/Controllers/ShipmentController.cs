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
                // 1. Get Old Info (Wrapped in Try/Catch to prevent crash if read fails)
                string oldStatus = "Unknown";
                int? salesOrderId = null;

                try
                {
                    var delivery = _deliveryFacade.Get(deliveryId);
                    if (delivery != null)
                    {
                        oldStatus = delivery.Status;
                        salesOrderId = delivery.SalesOrderId;
                    }
                }
                catch
                {
                    // If Get(id) crashes, we log a warning but CONTINUE to update the status.
                    Console.WriteLine($"[Warning] Failed to fetch old Delivery info for ID {deliveryId}");
                }

                // 2. Perform the Update (The Critical Action)
                _orderFacade.UpdateDeliveryStatus(deliveryId, status);

                // 3. Log the Change (Wrapped in Try/Catch)
                try
                {
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
                }
                catch { /* Ignore logging errors */ }

                return Json(new { success = true });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }
    }
}