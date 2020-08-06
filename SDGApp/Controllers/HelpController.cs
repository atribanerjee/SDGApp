using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using static SDGApp.ViewModel.HelpViewModel;

namespace SDGApp.Controllers
{
    [UserAuthorization]
    public class HelpController : Controller
    {
        #region :: GlobalVariables

        Int32 PageSize;
        BaseModel BM;
        UserModel UM;
        HelpModel HM;

        #endregion

        #region :: Constructor

        public HelpController()
        {
            BM = new BaseModel();
            HM = new HelpModel();
            PageSize = GlobalConstants.PageSize;
            UM = new UserModel();
        }

        #endregion



        // GET: Help
        public ActionResult Index()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Help");
            ViewBag.lstbdcomb = lstBreadcrumb;

            HM.SetHelpSessionValue("Help List");
            return View();
        }

        [HttpGet]
        public ActionResult HelpList(int pageSize, int pageNumber, string SearchValue = "")
        {
            HelpViewModel HVM = new HelpViewModel();
            var helpList = HM.GetHelpList(HVM, pageSize, pageNumber, SearchValue);

            if (!String.IsNullOrEmpty(SearchValue))
            {
                ViewBag.SearchValue = SearchValue;
            }
            return PartialView("_List", helpList);
        }

        public ActionResult Add()
        {
            HelpViewModel model = new HelpViewModel();
            model.DDLTopic = HM.GetTopicDDL();
            return View(model);
        }

        //[HttpPost]
        //public ActionResult Add(HelpViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (!HM.CheckHelpTopic(model.FKTopicID))
        //        {
        //            HM.SaveHelp(model);
        //        }
        //        else
        //        {
        //            ViewBag.DuplicateMessage = "Record Exist";
        //        }
        //    }
        //    return RedirectToAction("Index", "Help", new { });
        //}

        [HttpGet]
        public ActionResult Edit(int ID)
        {
            HM.SetHelpSessionValue("Edit Help");
            //BREADCRUMB
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Help/Index");
            lstBreadcrumb.Add("HelpList");
            lstBreadcrumb.Add("Edit");
            ViewBag.lstbdcomb = lstBreadcrumb;

            HelpViewModel model = new HelpViewModel();
            if (ID > 0)
            {
                model = HM.GetHelpByID(BM.GetIntegerValue(ID));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(HelpViewModel model)
        {
            RemoveModelStateItem("Topic");
            if (ModelState.IsValid)
            {
                HM.UpdateHelp(model);
            }
            return RedirectToAction("Index", "Help", new { });
        }

        [HttpPost]
        public ActionResult InfinateScroll(Int32 blockNumber)
        {
            HelpViewModel HVM = new HelpViewModel();
            HVM.PageSize = PageSize;
            HVM.PageNumber = blockNumber;
            HVM.FromRow = ((blockNumber - 1) * PageSize) + 1;
            HVM.ToRow = PageSize * blockNumber;
            var datalist = HM.GetHelpList(HVM, PageSize, blockNumber);
            if (datalist == null)
            {
                return HttpNotFound();
            }
            JsonModel jsonModel = new JsonModel();
            jsonModel.NoMoreData = datalist.Count == 0;
            jsonModel.HTMLString = RenderPartialViewToString("_List", datalist);
            jsonModel.BlockNo = blockNumber;
            return Json(jsonModel);
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public JsonResult GetHelpText()        {            string TopicText = BM.GetStringValue(HM.GetHelpSessionValue());            if (TopicText.Length > 0)            {                HelpViewModel HVM = HM.GetRelatedHelp(TopicText);                if (HVM.Topic != null)                {                    return Json(new { Topic = HVM.Topic, HelpText = HVM.HelpText }, JsonRequestBehavior.AllowGet);                }                else                {                    return Json(new { Topic = "", HelpText = "" }, JsonRequestBehavior.AllowGet);                }            }            else            {                return Json(new { Topic = "", HelpText = "" }, JsonRequestBehavior.AllowGet);            }        }

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