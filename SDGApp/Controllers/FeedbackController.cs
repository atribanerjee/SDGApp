using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.Controllers
{
    public class FeedbackController : Controller
    {
        UserModel UM;
        HelpModel HM;
        FeedbackModel FM;
        BaseModel BM;
        public FeedbackController()
        {
            UM = new UserModel();
            HM = new HelpModel();
            FM=new FeedbackModel();
            BM = new BaseModel();
        }
        // GET: Feedback
        public ActionResult Index()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Feedback");
            ViewBag.lstbdcomb = lstBreadcrumb;
            return View();
        }

        [HttpGet]
        public ActionResult FeedbackList(int pageSize, int pageNumber, string SearchValue = "")
        {
            int UserID = UM.GetLoggedInUserInfo().UserID;
            int CompanyID = UM.GetLoggedInUserInfo().CompanyID;
            List<FeedbackViewModel> lstuvm = FM.GetFeedbackList(CompanyID, UserID, pageSize, pageNumber, SearchValue);

            if (!String.IsNullOrEmpty(SearchValue))
            {
                ViewBag.SearchValue = SearchValue;
            }
            return PartialView("_FeedbackList", lstuvm);
        }


        [HttpGet]
        public ActionResult Add()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Feedback/Index");
            lstBreadcrumb.Add("Feedback");
            lstBreadcrumb.Add("Add");
            ViewBag.lstbdcomb = lstBreadcrumb;

            //HM.SetHelpSessionValue("Add Feedback");
            FeedbackViewModel model = new FeedbackViewModel();
         //   model.FKHelpModuleID = HM.GetHelpModuleID();
            model.UserFullName = UM.GetLoggedInUserInfo().FirstName + " " + UM.GetLoggedInUserInfo().LastName;
            model.DDLTopic = HM.GetTopicDDL();
           return PartialView("_Add", model);
      //   return View(model);
        }

        [HttpPost]
        public ActionResult Add(FeedbackViewModel model)
        {
            if (ModelState.IsValid)
            {
                FM.AddNewFeedback(model);
            }
            return RedirectToAction("Index", "Feedback", new { });
           // return Redirect(Request.UrlReferrer.PathAndQuery);
        }
        [HttpGet]
        public ActionResult Edit(int ID)
        {

            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Feedback/Index");
            lstBreadcrumb.Add("Feedback");
            lstBreadcrumb.Add("Edit");
            ViewBag.lstbdcomb = lstBreadcrumb;

            //  HM.SetHelpSessionValue("Edit Feedback");
            FeedbackViewModel model = new FeedbackViewModel();
            if (ID > 0)
            {
                model = FM.GetFeedbackDetailByID(BM.GetIntegerValue(ID));
            }
            model.DDLTopic = HM.GetTopicDDL();
            // model.UserFullName = UM.GetLoggedInUserInfo().FirstName + " " + UM.GetLoggedInUserInfo().LastName;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(FeedbackViewModel model)
        {
            if (ModelState.IsValid)
            {
                FM.UpdateFeedback(model);
            }
            return RedirectToAction("Index", "Feedback", new { });
        }

        public JsonResult Delete(int FeedbackID)
        {
            if (FM.DeleteFeedbackByID(FeedbackID))
            {
                return Json(new { Result = true, Message = "Feedback deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Message = "Feedback delete failed." }, JsonRequestBehavior.AllowGet);
            }

        }

    }
}