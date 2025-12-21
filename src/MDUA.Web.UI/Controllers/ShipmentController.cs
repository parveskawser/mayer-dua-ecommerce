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

        public ShipmentController(IDeliveryFacade deliveryFacade, IOrderFacade orderFacade)
        {
            _deliveryFacade = deliveryFacade;
            _orderFacade = orderFacade;
        }

        [HttpGet]
        public IActionResult DeliveryList()
        {
            // Use DeliveryFacade for the complex list loading
            IList<Delivery> list = _deliveryFacade.GetAllDeliveries();
            return View(list);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int deliveryId, string status)
        {
            _orderFacade.UpdateDeliveryStatus(deliveryId, status);
            return Json(new { success = true });
        }

    }
}