using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.Controllers
{
    [UserAuthorization]
    public class TagHistoryController : Controller
    {

        UserModel UM;
        BaseModel BM;
        ServiceModel SM;
        AttributeRuleModel ARM;
        TagHistoryModel TagHistoryModel;

        public TagHistoryController()
        {
            UM = new UserModel();
            BM = new BaseModel();
            SM = new ServiceModel();
            ARM = new AttributeRuleModel();
            TagHistoryModel = new TagHistoryModel();
        }


        // GET: TagHistory
        public ActionResult Index()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Tag History");
            ViewBag.lstbdcomb = lstBreadcrumb;

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetHistory(string type, String currentdate)
        {
            List<TagsHistoryViewModel> list = new List<TagsHistoryViewModel>();

            DateTime currentdatetime = DateTime.ParseExact(currentdate.ToString(), "MM-dd-yyyy", CultureInfo.InvariantCulture);

            if (type != "" && type == "day")
            {

                list = TagHistoryModel.GetAllTagHistoryList(currentdatetime, type);

                return Json(new { Taglist = list }, JsonRequestBehavior.AllowGet);

            }
            else if (type != "" && type == "month")
            {
                list = TagHistoryModel.GetAllTagHistoryList(currentdatetime, type);
                return Json(new { Taglist = list }, JsonRequestBehavior.AllowGet);


            }
            else if (type != "" && type == "week")
            {
                list = TagHistoryModel.GetAllTagHistoryList(currentdatetime, type);
                return Json(new { Taglist = list }, JsonRequestBehavior.AllowGet);

            }

            return Json(new { Taglist = string.Empty }, JsonRequestBehavior.AllowGet);
        }


        #region [ :: ADD ]


        [HttpGet]
        public ActionResult Add()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("TagHistory/Index");
            lstBreadcrumb.Add("Tag History");
            lstBreadcrumb.Add("Add");
            ViewBag.lstbdcomb = lstBreadcrumb;

            int UserID = UM.GetLoggedInUserInfo().UserID;

            TagsHistoryViewModel model = new TagsHistoryViewModel();
            model.DDLTagLabelType = TagHistoryModel.GetDDLTagLabelType(UserID);
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(TagsHistoryViewModel model)
        {
            int UserID = UM.GetLoggedInUserInfo().UserID;

            if (ModelState.IsValid)
            {
                if (TagHistoryModel.AddNewTag(model))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                   
                    model.DDLTagLabelType = TagHistoryModel.GetDDLTagLabelType(UserID);
                    return View(model);
                }
            }
            else
            {
                List<string> lstBreadcrumb = new List<string>();
                lstBreadcrumb.Add("TagHistory/Index");
                lstBreadcrumb.Add("Tag History");
                lstBreadcrumb.Add("Add");
                ViewBag.lstbdcomb = lstBreadcrumb;

                model.DDLTagLabelType = TagHistoryModel.GetDDLTagLabelType(UserID);
                return View(model);
            }

        }

        #endregion

        #region [ :: EDIT ]

        [HttpGet]
        public ActionResult Edit(int ID)
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("TagHistory/Index");
            lstBreadcrumb.Add("Tag History");
            lstBreadcrumb.Add("Edit");
            ViewBag.lstbdcomb = lstBreadcrumb;

            // AttributeRuleModel ATM = new AttributeRuleModel();
            int UserID = UM.GetLoggedInUserInfo().UserID;
            TagsHistoryViewModel model = new TagsHistoryViewModel();

            model = TagHistoryModel.GetTagsHistoryDetailById(ID);

            model.DDLTagLabelType = TagHistoryModel.GetDDLTagLabelType(UserID);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(TagsHistoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (TagHistoryModel.UpdateTagByID(model))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return PartialView("_TagLabelsList", model);
                }
            }
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("TagHistory/Index");
            lstBreadcrumb.Add("Tag History");
            lstBreadcrumb.Add("Edit");
            ViewBag.lstbdcomb = lstBreadcrumb;
            int UserID = UM.GetLoggedInUserInfo().UserID;
            model.DDLTagLabelType = TagHistoryModel.GetDDLTagLabelType(UserID);
            return View(model);
        }

        #endregion
    }
}