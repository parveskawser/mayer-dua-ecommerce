using Microsoft.AspNetCore.Mvc;
using MDUA.Facade.Interface;
using MDUA.Entities;
using System;

namespace MDUA.Web.Controllers
{
    public class VendorController : Controller
    {
        private readonly IVendorFacade _vendorFacade;

        public VendorController(IVendorFacade vendorFacade)
        {
            _vendorFacade = vendorFacade;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // For simplicity, fetching all. Implement PagedRequest here if list is large.
            var list = _vendorFacade.GetAll();
            return View(list);
        }

        [HttpGet]
        public IActionResult Add(int? id)
        {
            Vendor model = new Vendor();
            if (id.HasValue && id > 0)
            {
                model = _vendorFacade.Get(id.Value);
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(Vendor vendor)
        {
            try
            {
                // Basic Server Validation
                if (string.IsNullOrEmpty(vendor.VendorName))
                {
                    TempData["Error"] = "Vendor Name is required.";
                    return View("Add", vendor);
                }

                if (vendor.Id > 0)
                {
                    // Update
                    vendor.UpdatedBy = User.Identity.Name ?? "Admin"; // Adjust based on your auth
                    _vendorFacade.Update(vendor);
                    TempData["Success"] = "Vendor updated successfully.";
                }
                else
                {
                    // Insert
                    vendor.CreatedBy = User.Identity.Name ?? "Admin";
                    _vendorFacade.Insert(vendor);
                    TempData["Success"] = "Vendor added successfully.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error saving vendor: " + ex.Message;
                return View("Add", vendor);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _vendorFacade.Delete(id);
                return Json(new { success = true, message = "Vendor deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting vendor." });
            }
        }
        
        [HttpGet]
        public IActionResult GetHistory(int id)
        {
            try
            {
                var history = _vendorFacade.GetVendorOrderHistory(id);
                return Json(new { success = true, data = history });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}