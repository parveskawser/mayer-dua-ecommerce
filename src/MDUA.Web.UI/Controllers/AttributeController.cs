using Microsoft.AspNetCore.Mvc;
using MDUA.Facade;
using MDUA.Entities;
using System;
using System.Linq;

namespace MDUA.Web.Controllers
{
    public class AttributeController : Controller
    {
        private readonly IAttributeFacade _attributeFacade;

        public AttributeController(IAttributeFacade attributeFacade)
        {
            _attributeFacade = attributeFacade;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Attribute Name Actions

        [HttpGet]
        public IActionResult GetAllAttributes()
        {
            var list = _attributeFacade.GetAllAttributes();
            return Json(new { data = list });
        }

        [HttpGet]
        public IActionResult GetAttribute(int id)
        {
            var attr = _attributeFacade.GetAttribute(id);
            return Json(attr);
        }

        [HttpPost]
        public IActionResult SaveAttribute([FromBody] AttributeName model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Name))
                    return Json(new { success = false, message = "Attribute Name is required." });

                _attributeFacade.SaveAttribute(model);
                return Json(new { success = true, message = "Attribute saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteAttribute(int id)
        {
            try
            {
                _attributeFacade.DeleteAttribute(id);
                return Json(new { success = true, message = "Attribute deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting attribute. It may be in use." });
            }
        }

        #endregion

        #region Attribute Value Actions

        [HttpGet]
        public IActionResult GetValuesByAttribute(int attributeId)
        {
            var list = _attributeFacade.GetValuesByAttributeId(attributeId);
            // Sort by DisplayOrder for better UI
            var sortedList = list.OrderBy(x => x.DisplayOrder).ToList();
            return Json(new { data = sortedList });
        }

        [HttpPost]
        public IActionResult SaveValue([FromBody] AttributeValue model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Value))
                    return Json(new { success = false, message = "Value is required." });

                _attributeFacade.SaveAttributeValue(model);
                return Json(new { success = true, message = "Value saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteValue(int id)
        {
            try
            {
                _attributeFacade.DeleteAttributeValue(id);
                return Json(new { success = true, message = "Value deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion
    }
}