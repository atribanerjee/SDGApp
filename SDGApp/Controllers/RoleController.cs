using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.Controllers
{
    public class RoleController : Controller
    {
        #region :: GlobalVariables

        BaseModel BM;
        RoleModel RM;
        UserModel UM;

        #endregion

        #region :: Constructor

        public RoleController()
        {
            BM = new BaseModel();
            RM = new RoleModel();
            UM = new UserModel();
         
        }

        #endregion
        // GET: Role
        public ActionResult Index()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Role");
            ViewBag.lstbdcomb = lstBreadcrumb;
            return View();
        }

        [HttpGet]
        public ActionResult Add()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Role/Index");
            lstBreadcrumb.Add("Role");
            lstBreadcrumb.Add("Add");
            ViewBag.lstbdcomb = lstBreadcrumb;

            RoleViewModel model = new RoleViewModel();
            return View(model);
        }

        [HttpPost]
         public ActionResult Add(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (RM.AddNewRole(model))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int ID)
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Role/Index");
            lstBreadcrumb.Add("Role");
            lstBreadcrumb.Add("Edit");
            ViewBag.lstbdcomb = lstBreadcrumb;

            RoleViewModel model = new RoleViewModel();
            model = RM.GetRoleDetailByID(ID);

            return View(model);
        }

        [HttpPost]
        
        public ActionResult Edit(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (RM.UpdateRole(model))
                {
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult RoleList(int PageSize, int pageNumber)
        {
            List<RoleViewModel> lstRole = RM.FetchRoleList(PageSize, pageNumber);
            return PartialView("_RoleList", lstRole);
        }

        #region:: 4.  Delete
        [HttpPost]
        public JsonResult Delete(int ID)
        {
            if (RM.DeleteRolebyID(ID))
            {
                return Json(new { Result = true, Message = "Role deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Message = "Role delete failed." }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

    }
}