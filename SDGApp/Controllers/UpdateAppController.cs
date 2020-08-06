using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.Controllers
{
    [UserAuthorization]
    public class UpdateAppController : Controller
    {
        UserModel UM;
        UpdateAppModel updateAppModel;

        public UpdateAppController()
        {

            UM = new UserModel();
            updateAppModel = new UpdateAppModel();

        }

        // GET: UpdateApp
        public ActionResult Index()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Upload App");
            ViewBag.lstbdcomb = lstBreadcrumb;
            return View();
        }
        

        [HttpPost]
        public ActionResult Index(UpdateAppViewModel model, string apptype)
        {
            if (!String.IsNullOrEmpty(apptype))
            {
                int userid = UM.GetLoggedInUserInfo().UserID;

                if (apptype.Trim().ToLower().Contains("ios"))
                {
                    if (!String.IsNullOrEmpty(model.AppiosURL) && !String.IsNullOrEmpty(model.AppiosVersion))
                    {
                        if (updateAppModel.SaveUpdateApp(userid, model, apptype))
                        {
                            TempData["SuccessMessage"] = "Data saved successfully.";
                            return View();
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Data saved failed";
                            return View();
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Please input url and version into text box";
                        return View();
                    }
                }
                else if (apptype.Trim().ToLower().Contains("android"))
                {
                    if (!String.IsNullOrEmpty(model.AppAndroidURL) && !String.IsNullOrEmpty(model.AppAndroidVersion))
                    {
                        if (updateAppModel.SaveUpdateApp(userid, model, apptype))
                        {
                            TempData["SuccessMessage"] = "Data saved successfully.";
                            return View();
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Data saved failed";
                            return View();
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Please input url and version into text box";
                        return View();
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Please input url and version into text box";
                    return View();
                }
            }
            else
            {
                //TempData["ErrorMessage"] = "Please input url into text box";
                return View();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult UpdateAppAndroidList(int AppType, int PageNumber, int PageSize)
        {
            List<UpdateAppViewModel> lst = updateAppModel.FetchAppListByType(AppType, PageNumber, PageSize);
            return PartialView("_UpdateAppAndroidList", lst);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult UpdateAppIOSList(int AppType, int PageNumber, int PageSize)
        {
            
            List<UpdateAppViewModel> lst = updateAppModel.FetchAppListByType(AppType, PageNumber, PageSize);
            return PartialView("_UpdateAppIOSList", lst);
        }

        [HttpPost]
        public JsonResult Delete(int ID)
        {
            if (updateAppModel.DeleteAppbyID(ID))
            {
                return Json(new { Result = true, Message = "App URL deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Message = "App URL delete failed." }, JsonRequestBehavior.AllowGet);
            }

        }

        
    }
}