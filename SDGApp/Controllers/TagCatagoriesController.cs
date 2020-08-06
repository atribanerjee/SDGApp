using SDGApp.Helpers;
using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.Controllers
{
    public class TagCatagoriesController : Controller
    {
        MailHelper MH;
        BaseModel BM;
        TagModel TM;
        UserModel UM;
        TagCatagoriesModel TCM;

        public TagCatagoriesController()
        {
            TM = new TagModel();
            MH = new MailHelper();
            BM = new BaseModel();
            UM = new UserModel();
            TCM = new TagCatagoriesModel();
        }
        // GET: TagCatagories
        public ActionResult Index()
        {

            return View();
        }
        [HttpGet]
        public ActionResult Add()
        {

            Int32 LoggedInUserID = UM.GetLoggedInUserInfo().UserID;
            List<TagsViewModel> TVM = new List<TagsViewModel>();
               TVM = TCM.GetAllTagCatagories(LoggedInUserID);
                return View(TVM);
        }
        [HttpPost]
        public JsonResult Add(int TagID, String[] Fields)
        {
            Int32 UserID = UM.GetLoggedInUserInfo().UserID;

            if (UserID > 0 && TCM.SaveTagCatagories(UserID, TagID, Fields))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
       
    }


}