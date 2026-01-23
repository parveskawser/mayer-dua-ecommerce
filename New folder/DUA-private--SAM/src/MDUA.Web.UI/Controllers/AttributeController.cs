using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MDUA.Entities;
using MDUA.Facade; // Ensure this points to your interface namespace
using MDUA.Web.UI.Controllers; // Assuming BaseController is here

namespace MDUA.Web.UI.Controllers
{
    public class AttributeController : BaseController
    {
        private readonly IAttributeFacade _attributeFacade;

        public AttributeController(IAttributeFacade attributeFacade)
        {
            _attributeFacade = attributeFacade;
        }

        // Helper to get CompanyId safely
        private int GetCurrentCompanyId()
        {
            var claim = User.FindFirst("CompanyId");
            if (claim != null && int.TryParse(claim.Value, out int id))
            {
                return id;
            }
            return 0; 
        }

        #region Attribute Name (Parent) Operations

        [HttpGet]
        [Route("attribute/list")]
        public IActionResult Index()
        {
            if (!HasPermission("Attribute.View")) return HandleAccessDenied();

            int companyId = GetCurrentCompanyId();
            if (companyId <= 0) return RedirectToAction("Login", "Account");

            try
            {
                // ✅ CHANGE: Use the "Management" method to see Inactive attributes
                var list = _attributeFacade.GetAllAttributesForManagement(companyId);
                return View(list);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View(new List<AttributeName>());
            }
        }

        [HttpGet]
        [Route("attribute/get/{id}")]
        public IActionResult GetAttribute(int id)
        {
            if (!HasPermission("Attribute.View")) return Unauthorized();

            var attr = _attributeFacade.GetAttributeName(id);
            if (attr == null) return NotFound();

            return Ok(attr);
        }

        [HttpPost]
        [Route("attribute/save")]
        public IActionResult SaveAttribute([FromBody] AttributeName model)
        {
            if (model.Id > 0 && !HasPermission("Attribute.Edit")) return HandleAccessDenied();
            if (model.Id == 0 && !HasPermission("Attribute.Create")) return HandleAccessDenied();

            int companyId = GetCurrentCompanyId();
            if (companyId <= 0) return Unauthorized(new { success = false, message = "Invalid Company Context" });

            try
            {
                // Facade handles logic: if ID=0, inserts with CompanyId. If ID>0, checks ownership.
                int id = _attributeFacade.SaveAttributeName(model, companyId);
                return Json(new { success = true, message = "Attribute saved successfully.", id = id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("attribute/delete/{id}")]
        public IActionResult DeleteAttribute(int id)
        {
            if (!HasPermission("Attribute.Delete")) return HandleAccessDenied();

            int companyId = GetCurrentCompanyId();
            try
            {
                bool result = _attributeFacade.DeleteAttributeName(id, companyId);
                if (result)
                    return Json(new { success = true, message = "Attribute deleted successfully." });
                else
                    return Json(new { success = false, message = "Attribute not found or cannot be deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("attribute/toggle-status")]
        public IActionResult ToggleAttributeStatus(int id, bool isActive)
        {
            if (!HasPermission("Attribute.Edit")) return HandleAccessDenied();

            int companyId = GetCurrentCompanyId();
            try
            {
                _attributeFacade.UpdateAttributeNameStatus(id, isActive, companyId);
                return Json(new { success = true, message = "Status updated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Attribute Values (Child) Operations

        [HttpGet]
        [Route("attribute/values-partial/{attributeId}")]
        public IActionResult GetValuesPartial(int attributeId)
        {
            if (!HasPermission("Attribute.View")) return Content("Access Denied");

            try
            {
                // We want ALL values (Active & Inactive) for the Admin Grid
                var values = _attributeFacade.GetValuesByAttributeId(attributeId, onlyActive: false);
                ViewBag.AttributeId = attributeId;
                
                // Returns a PartialView that renders the <table> rows
                return PartialView("_AttributeValuesList", values);
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("attribute/value/save")]
        public IActionResult SaveValue([FromBody] AttributeValue model)
        {
            if (model.Id > 0 && !HasPermission("Attribute.Edit")) return HandleAccessDenied();
            if (model.Id == 0 && !HasPermission("Attribute.Edit")) return HandleAccessDenied(); // Adding values is considered Editing the attribute

            int companyId = GetCurrentCompanyId();
            if (companyId <= 0) return Unauthorized();

            try
            {
                // Facade Logic: 
                // If the parent attribute is Global, it will be CLONED to Private.
                // The method returns the ID of the attribute that was actually modified.
                int workingAttributeId = _attributeFacade.SaveAttributeValue(model, companyId);

                return Json(new 
                { 
                    success = true, 
                    message = "Value saved.", 
                    // Send back the ID used. Frontend check: if this != sent attributeId, 
                    // it means a Clone happened, and the UI should reload the main list.
                    attributeId = workingAttributeId 
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("attribute/value/delete/{id}")]
        public IActionResult DeleteValue(int id)
        {
            if (!HasPermission("Attribute.Edit")) return HandleAccessDenied();

            int companyId = GetCurrentCompanyId();
            try
            {
                bool success = _attributeFacade.DeleteAttributeValue(id, companyId);
                return Json(new { success = success, message = success ? "Value deleted." : "Failed." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("attribute/value/toggle-status")]
        public IActionResult ToggleValueStatus(int id, bool isActive)
        {
            if (!HasPermission("Attribute.Edit")) return HandleAccessDenied();

            int companyId = GetCurrentCompanyId();
            try
            {
                _attributeFacade.UpdateAttributeValueStatus(id, isActive, companyId);
                return Json(new { success = true, message = "Status updated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

    }
}