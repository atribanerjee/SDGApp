using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SDGApp.Controllers
{
    [UserAuthorization]
    public class TagCategoryController : Controller
    {
        UserModel UM;
        public TagModel TM;

        public TagCategoryController()
        {
            UM = new UserModel();
            TM = new TagModel();
        }

        [HttpGet]
        public ActionResult Index()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Tag Labels");
            ViewBag.lstbdcomb = lstBreadcrumb;

            return View();
        }

        [HttpGet]
        public ActionResult TagCategoryList(int PageNumber, int PageSize)
        {
            TagsCategoryViewModel model = new TagsCategoryViewModel();

            model.PageNumber = PageNumber;
            model.PageSize = PageSize;

            return PartialView("_TagCategoryList", TM.GetTagsCategoryList(model));
        }

        [HttpGet]
        public ActionResult TagLabelsList(int PageNumber, int PageSize)
        {
            TagsCategoryViewModel model = new TagsCategoryViewModel();

            model.PageNumber = PageNumber;
            model.PageSize = PageSize;

            return PartialView("_TagCategoryList", TM.GetTagsCategoryList(model));
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(TagsCategoryViewModel model)
        {
            if(ModelState.IsValid)
            {
                if(TM.InserNewTagCategories(model))
                {
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            return RedirectToAction("Index", "Dashboard");
        }
     

        [HttpGet]
        public ActionResult Detail(Int32 TagID)
        {
            return PartialView("_Detail", TM.GetTagDetailByTagID(TagID));
        }
        
        
        [HttpPost]
        public JsonResult DeleteTagCatagories(int ID)
        {
            if (TM.DeleteTagcatagory(ID))
            {
                return Json(new { Result = true, Message = "Tag Catagories deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Message = "Tag Catagories deleted unsuccessful." }, JsonRequestBehavior.AllowGet);
            }

        }

        //[HttpGet]
        //public ActionResult Edit(Int32 ID)
        //{
        //    TagsCategoryViewModel model = TM.GetTagCategoryDetails(ID);

        //    return View(model);
        //}

        //[HttpPost]
        //public JsonResult Edit(Int32 TagCategoryID, String TagCategoryName)
        //{
        //    if (TM.EditTagCategory(TagCategoryID, TagCategoryName))
        //    {
        //        return Json(true, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(false, JsonRequestBehavior.AllowGet);
        //}
    }
}