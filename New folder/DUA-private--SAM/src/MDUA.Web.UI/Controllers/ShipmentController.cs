using MDUA.Entities;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDUA.Web.UI.Controllers
{
    public class ShipmentController : BaseController
    {
        private readonly IDeliveryFacade _deliveryFacade;
        private readonly IOrderFacade _orderFacade;
        private readonly IDeliveryStatusLogFacade _logFacade;
        private readonly ISubscriptionSystemFacade _subscriptionFacade;

        public ShipmentController(
            IDeliveryFacade deliveryFacade,
            IOrderFacade orderFacade,
            IDeliveryStatusLogFacade logFacade,
            ISubscriptionSystemFacade subscriptionFacade)
        {
            _deliveryFacade = deliveryFacade;
            _orderFacade = orderFacade;
            _logFacade = logFacade;
            _subscriptionFacade = subscriptionFacade;
        }

        [Route("delivery/all")]
        [HttpGet]
        public IActionResult DeliveryList()
        {
            if (!HasPermission("Order.View")) return HandleAccessDenied();

            if (_subscriptionFacade.IsSubscriptionLocked(CurrentCompanyId, out int current, out int limit))
            {
                return RedirectToAction("LimitReached", "Subscription", new
                {
                    current = current,
                    limit = limit,
                    feature = "Order"
                });
            }

            int companyId = CurrentCompanyId;
            if (companyId <= 0)
            {
                var claim = User.FindFirst("CompanyId");
                if (claim != null && int.TryParse(claim.Value, out int cid))
                    companyId = cid;
                else
                    return RedirectToAction("Login", "Account");
            }

            IList<Delivery> list = _deliveryFacade.GetAllDeliveries(companyId);
            return View(list);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int deliveryId, string status)
        {
            try
            {
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
                    Console.WriteLine($"[Warning] Failed to fetch old Delivery info for ID {deliveryId}");
                }

                _orderFacade.UpdateDeliveryStatus(deliveryId, status);

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
                catch { }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // ✅ NEW: Load Modal Data + Carrier Options in one call
        // ============================================================
        [HttpGet]
        public IActionResult GetShipmentModalBootstrapData(int deliveryId)
        {
            try
            {
                int companyId = CurrentCompanyId;
                if (companyId <= 0) return Json(new { success = false, message = "Invalid company." });

                var modal = _deliveryFacade.GetShipmentModalData(deliveryId, companyId);
                if (modal == null)
                    return Json(new { success = false, message = "Delivery not found for this company." });

                var carriers = _deliveryFacade.GetActiveCompanyCarrierOptions(companyId);

                return Json(new
                {
                    success = true,
                    modal,
                    carriers
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // ✅ NEW: Create Shipment (Carrier API OR Own Delivery)
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> ShipWithCarrier(int deliveryId, int companyCarrierId)
        {
            try
            {
                int companyId = CurrentCompanyId;
                if (companyId <= 0) return Json(new { success = false, message = "Invalid company." });

                // Old info for log
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
                catch { }

                var result = await _deliveryFacade.ShipWithCarrierAsync(
                    deliveryId,
                    companyId,
                    companyCarrierId,
                    User?.Identity?.Name ?? "Admin"
                );

                if (result == null || !result.Success)
                    return Json(new { success = false, message = result?.ErrorMessage ?? "Shipment failed." });

                // Keep your existing status pipeline consistent
                _orderFacade.UpdateDeliveryStatus(deliveryId, "Shipped");

                try
                {
                    _logFacade.LogStatusChange(
                        entityId: deliveryId,
                        entityType: "Delivery",
                        oldStatus: oldStatus,
                        newStatus: "Shipped",
                        changedBy: User.Identity.Name ?? "Admin",
                        orderId: salesOrderId,
                        reason: "Shipment created via Shipment Modal"
                    );
                }
                catch { }

                return Json(new
                {
                    success = true,
                    trackingNumber = result.TrackingNumber ?? "",
                    consignmentId = result.ConsignmentId ?? "",
                    rawResponse = result.RawResponse ?? ""
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [Route("shipment/courier-credentials")]
        [HttpGet]
        public IActionResult CourierCredentials()
        {
            if (!HasPermission("Order.View")) return HandleAccessDenied();
            return View();
        }

        // ============================================================
        // Load courier + current company settings (AJAX)
        // ============================================================
        [HttpGet]
        public IActionResult GetCourierCredentialSettings()
        {
            try
            {
                int companyId = CurrentCompanyId;
                if (companyId <= 0) return Json(new { success = false, message = "Invalid company." });

                // You will implement this in facade later
                List<CourierCredentialRowDto> rows = _deliveryFacade.GetCourierCredentialSettings(companyId);

                return Json(new { success = true, rows });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // Save/update one courier credential (AJAX)
        // ============================================================
        [HttpPost]
        public IActionResult SaveCourierCredential([FromBody] SaveCourierCredentialRequest req)
        {
            try
            {
                int companyId = CurrentCompanyId;
                if (companyId <= 0) return Json(new { success = false, message = "Invalid company." });

                if (req == null) return Json(new { success = false, message = "Invalid request." });

                // You will implement this in facade later
                _deliveryFacade.SaveCourierCredential(companyId, req, User?.Identity?.Name ?? "Admin");

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> TestCourierConnection(int companyCarrierId)
        {
            try
            {
                int companyId = CurrentCompanyId;
                if (companyId <= 0) return Json(new { success = false, message = "Invalid company." });

                // Call Facade
                var result = await _deliveryFacade.TestCourierConnectionAsync(companyId, companyCarrierId);

                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Controller Error: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ToggleCourierActive(int companyCarrierId, bool isActive)
        {
            try
            {
                int companyId = CurrentCompanyId;
                if (companyId <= 0) return Json(new { success = false, message = "Invalid company." });

                _deliveryFacade.ToggleCourierActive(companyId, companyCarrierId, isActive);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
