using SDGApp.Helpers;
using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SDGApp.Controllers
{
    [UserAuthorization]
    public class TagsController : Controller
    {
        MailHelper MH;
        BaseModel BM;
        TagModel TM;
        UserModel UM;

        public TagsController()
        {
            TM = new TagModel();
            MH = new MailHelper();
            BM = new BaseModel();
            UM = new UserModel();
        }
        // GET: Tags
        public ActionResult Index()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("TagsList");
            ViewBag.lstbdcomb = lstBreadcrumb;
            return View();
        }

        [HttpGet]
        public ActionResult TagsList(int PageNumber, int PageSize)
        {
            TagsViewModel model = new TagsViewModel();
           

            model.PageNumber = PageNumber;
            model.PageSize = PageSize;

            return PartialView("_TagsList", TM.GetTagList(model));
        }

        [HttpGet]
        public ActionResult Add()
        {
            TagsViewModel TVM = new TagsViewModel();

            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Tags/Index");
            lstBreadcrumb.Add("TagList");
            lstBreadcrumb.Add("Add");
            ViewBag.lstbdcomb = lstBreadcrumb;

            TVM.TypeList = TM.GetTypeList();
            TVM.TagCategoryList = TM.GetTagCatagoriesListDDL();
            return View(TVM);
        }

        [HttpPost]
        public JsonResult Add(Int32 TagCategoryID, bool IsActivity, String TagName, String Desc, String[] Fields)
        {
            Int32 UserID = UM.GetLoggedInUserInfo().UserID;

            if (UserID > 0 && TM.AddNewTags(TagCategoryID, IsActivity, TagName, Desc, UserID, Fields))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(int ID)
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Tags/Index");
            lstBreadcrumb.Add("TagList");
            lstBreadcrumb.Add("Edit");
            ViewBag.lstbdcomb = lstBreadcrumb;

            TagDetailsViewModel TDVM = new TagDetailsViewModel();
            return View("Edit", TM.GetTagDetailByTagID(ID));
        }

        [HttpPost]
        public JsonResult Edit( Int32 TagID, String TagName, String Desc, String[] Fields)
        {
            if (TM.EditTags(TagID, TagName, Desc, Fields))
            {
                ViewBag.SuccessMessage = "Your profile has updated successfully by admin.";
               
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FetchTypeList()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            lst = TM.GetTypeList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteTags(int ID)
        {
            if (TM.DeleteTagsbyTagID(ID))
            {
                return Json(new { Result = true, Message = "Tag deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Message = "Tag deleted unsuccessful." }, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult FetchTAgCAtegoriesList()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            lst = TM.GetTagCatagoriesListDDL();
            // return Json(lst, JsonRequestBehavior.AllowGet);
            return View(lst);
        }

        public JsonResult GetTagListByTagCategories(int ID)
        {
            List<TagsViewModel> lst = new List<TagsViewModel>();
            lst = TM.GetTagListByTagCategoriesID(ID);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }


    }
}