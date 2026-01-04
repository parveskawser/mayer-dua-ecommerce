using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Framework.Utils; // For EmailTemplateKey constants
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace MDUA.Web.UI.Controllers
{
    public class EmailTemplateController : Controller
    {
        private readonly IEmailTemplateDataAccess _emailTemplateDataAccess;

        public EmailTemplateController(IEmailTemplateDataAccess emailTemplateDataAccess)
        {
            _emailTemplateDataAccess = emailTemplateDataAccess;
        }

        // 1. LIST VIEW
        public IActionResult Index()
        {
            return View();
        }

        // 2. AJAX: Get All Templates for Grid/Table
        [HttpGet]
        public IActionResult GetAllTemplates()
        {
            try
            {
                var list = _emailTemplateDataAccess.GetAll();
                return Json(new { success = true, data = list });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                // 1. Check if valid
                if (id <= 0) return Json(new { success = false, message = "Invalid ID" });

                _emailTemplateDataAccess.Delete(id);

                return Json(new { success = true, message = "Deleted Successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 3. AJAX: Get Single Template for Edit
        [HttpGet]
        public IActionResult GetById(int id)
        {
            try
            {
                var template = _emailTemplateDataAccess.Get(id);
                if (template == null) return Json(new { success = false, message = "Not Found" });
                return Json(new { success = true, data = template });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 4. SAVE (Insert or Update)
        [HttpPost]
        public IActionResult Save(EmailTemplate model)
        {
            try
            {
                // Basic Validation
                if (string.IsNullOrWhiteSpace(model.TemplateKey) || string.IsNullOrWhiteSpace(model.Subject))
                {
                    return Json(new { success = false, message = "Template Key and Subject are required." });
                }

                model.LastUpdatedDate = DateTime.UtcNow;
                model.LastUpdatedBy = User.Identity?.Name ?? "Admin"; // Audit Trail

                if (model.Id > 0)
                {
                    // UPDATE
                    _emailTemplateDataAccess.Update(model);
                }
                else
                {
                    // INSERT
                    // Ensure IsActive is true by default for new ones
                    model.IsActive = true;
                    _emailTemplateDataAccess.Insert(model);
                }

                return Json(new { success = true, message = "Saved Successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}