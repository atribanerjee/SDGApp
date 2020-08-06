using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.Controllers
{
    public class AttributeRuleController : Controller
    {
        #region GLOBAL VARIABLE
        AttributeRuleModel ARM;
        #endregion


        #region CONSTRUCTOR
        public AttributeRuleController()
        {
            ARM = new AttributeRuleModel();
        }
        #endregion
        // GET: AttributeRule

        #region :: 1. LIST
        public ActionResult Index()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("AttributeRule");
            ViewBag.lstbdcomb = lstBreadcrumb;

            return View();
        }

        [HttpGet]
        public ActionResult AttributeRuleList(int pageSize, int pageNumber)
        {
            List<AttributeRuleViewModel> lstAttr = ARM.GetAttributeRuleList(pageSize, pageNumber);
            return PartialView("_AttributeRuleList", lstAttr);
        }

        #endregion

        #region :: 2. ADD

        [HttpGet]
        public ActionResult AddRule()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("AttributeRule/Index");
            lstBreadcrumb.Add("AttributeRule");
            lstBreadcrumb.Add("Add");
            ViewBag.lstbdcomb = lstBreadcrumb;

            AttributeRuleViewModel model = new AttributeRuleViewModel();
            //model.DDLAttributeLabel = ARM.GetAttributeLabel();
            model.DDLAttributeRuleType = ARM.GetAttributeRuleType();
            model.DDLAttributeType = ARM.GetAttributeType();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddRule(AttributeRuleViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (ARM.SaveAttributeRule(model))
                {
                    return RedirectToAction("Index");
                }
            }
            AttributeRuleViewModel arvm = new AttributeRuleViewModel();
            arvm.DDLAttributeRuleType = ARM.GetAttributeRuleType();
            arvm.DDLAttributeType = ARM.GetAttributeType();
            return View(arvm);
        }

        #endregion

        #region  :: 3. EDIT



        [HttpGet]
        public ActionResult EditAttributeRule(int AttrID)
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("AttributeRule/Index");
            lstBreadcrumb.Add("AttributeRule");
            lstBreadcrumb.Add("Edit");
            ViewBag.lstbdcomb = lstBreadcrumb;



            AttributeRuleViewModel model = new AttributeRuleViewModel();
            model = ARM.GetforEditAttributeRulebyID(AttrID);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditAttributeRule(AttributeRuleViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (ARM.UpdateAttributeRule(model))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    model = ARM.GetforEditAttributeRulebyID(model.AttributeRuleID);
                    return View(model);
                }
            }
            else
            {
                model = ARM.GetforEditAttributeRulebyID(model.AttributeRuleID);
                return View(model);
            }


        }

        #endregion

        #region :: 4. DELETE

        [HttpPost]
        public JsonResult DeleteAttributeRule(int AttributeID)
        {
            if (ARM.DeleteAttributeRulebyID(AttributeID))
            {
                return Json(new { Result = true, Message = "AttributeRule deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Message = "AttributeRule delete filed." }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        public JsonResult GetAttributeLabelforDDlByTypeID(int TypeID)
        {
            return Json(ARM.GetAttributeLabelByAttrType(TypeID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAttributeTypeAndValue(int AttrLabelId)
        {
            return Json(ARM.GetAttrLabelValue(AttrLabelId), JsonRequestBehavior.AllowGet);
        }



        private void RemoveModelStateItem(String data)
        {
            try
            {
                String[] items = data.Split(',');
                foreach (String item in items)
                {
                    ModelState.Remove(item);
                }
            }
            catch { }
        }

    }
}