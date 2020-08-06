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
    public class TagLabelsController : Controller
    {
        UserModel UM;
        BaseModel BM;
        AttributeRuleModel ARM;
        TagLabesModel TagLabesModel;

        public TagLabelsController()
        {
            UM = new UserModel();
            BM = new BaseModel();
            ARM = new AttributeRuleModel();
            TagLabesModel = new TagLabesModel();
        }

        #region [ :: LIST ]

        // GET: TagLabels
        public ActionResult Index()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Tag Editor");
            ViewBag.lstbdcomb = lstBreadcrumb;

            return View();
        }

        public ActionResult TagLabeslList(int PageNumber, int PageSize)
        {
            TagLabelViewModel TLVM = new TagLabelViewModel();
            TLVM.PageNumber = PageNumber;
            TLVM.PageSize = PageSize;
            int UserID = UM.GetLoggedInUserInfo().UserID;

            return PartialView("_TagLabelsList", TagLabesModel.GetAllTagLabelList(UserID,PageNumber, PageSize));
        }

        #endregion

        #region [ :: ADD ]

        [HttpGet]
        public ActionResult Add()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("TagLabels/Index");
            lstBreadcrumb.Add("Tag Editor");
            lstBreadcrumb.Add("Add");
            ViewBag.lstbdcomb = lstBreadcrumb;

            TagLabelViewModel TLVM = new TagLabelViewModel();

            //TLVM.DDLTagLabelTypeName = TagLabesModel.GetTagLabelTypeNameDDL();
            return View(TLVM);
        }

        [HttpPost]
        public ActionResult Add(TagLabelViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!TagLabesModel.DuplicateTagLabel(model.LabelName))
                {
                    if (UM.GetLoggedInUserInfo().UserRoleID == (int)SDGApp.Helpers.SDGUtilities.UserRoleType.Administrator)
                    {
                        model.UserID = 0;
                    }
                    else
                    {
                        model.UserID = UM.GetLoggedInUserInfo().UserID;
                    }

                    if (TagLabesModel.AddNewLabel(model))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View(model);
                    }
                }
                else
                {
                    List<string> lstBreadcrumb = new List<string>();
                    lstBreadcrumb.Add("TagLabels/Index");
                    lstBreadcrumb.Add("Tag Editor");
                    lstBreadcrumb.Add("Add");
                    ViewBag.lstbdcomb = lstBreadcrumb;
                    ViewBag.ErrorMessage = "Duplicate Tag Label Name.";
                    return View(model);
                }

            }
            else
            {
                List<string> lstBreadcrumb = new List<string>();
                lstBreadcrumb.Add("TagLabels/Index");
                lstBreadcrumb.Add("Tag Editor");
                lstBreadcrumb.Add("Add");
                ViewBag.lstbdcomb = lstBreadcrumb;
                return View(model);
            }
        }

        #endregion

        #region [ :: EDIT ]

        [HttpGet]
        public ActionResult Edit(int ID)
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("TagLabels/Index");
            lstBreadcrumb.Add("Tag Editor");
            lstBreadcrumb.Add("Edit");
            ViewBag.lstbdcomb = lstBreadcrumb;

            
            TagLabelViewModel TLVM = new TagLabelViewModel();

            TLVM = TagLabesModel.GetTagLabelDetailById(ID);
            //TLVM.DDLTagLabelTypeName = TagLabesModel.GetTagLabelTypeNameDDL();

            return View(TLVM);
        }

        [HttpPost]
        public ActionResult Edit(TagLabelViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (TagLabesModel.UpdateTagLabelByID(model))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return PartialView("_TagLabelsList", model);
                }

            }
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("TagLabels/Index");
            lstBreadcrumb.Add("Tag Editor");
            lstBreadcrumb.Add("Edit");
            ViewBag.lstbdcomb = lstBreadcrumb;
            return View(model);
        }

        #endregion

        #region [ :: DELETE ]

        [HttpPost]
        public JsonResult DeleteLabelByID(int ID)
        {
            if (TagLabesModel.DeleteLabelByID(ID))
            {
                return Json(new { Result = true, Message = "Tag label deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Message = "Tag label not deleted." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

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