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
    [UserAuthorization]
    public class ContactsController : Controller
    {

        BaseModel BM;
        UserContactsModel UCM;
        HelpModel HM;
        MailHelper MH;
        UserModel UM;

        public ContactsController()
        {
            BM = new BaseModel();
            UCM = new UserContactsModel();
            HM = new HelpModel();
            MH = new MailHelper();
            UM = new UserModel();
        }


        public ActionResult Index()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Contacts");
            ViewBag.lstbdcomb = lstBreadcrumb;

            List<UserContactsViewModel> lstcontacts = new List<UserContactsViewModel>();

            int UserId = UM.GetLoggedInUserInfo().UserID;

            int PageNo = 1;
            int Pagesize = GlobalConstants.PageSize;

            // lstcontacts = UCM.GetUserContactList(UserId, Server.MapPath("~/Content/images"), PageNo, Pagesize, SearchValue);

            return View();
        }

        [HttpGet]
        public ActionResult UserContactList(string SearchValue = "", string sortbyvalue = "accepteddate")
        {
            List<UserContactsViewModel> lstcontacts = new List<UserContactsViewModel>();

            int UserId = UM.GetLoggedInUserInfo().UserID;

            int PageNo = 1;
            int Pagesize = GlobalConstants.PageSize;

            lstcontacts = UCM.GetUserContactList(UserId, Server.MapPath("~/Content/images"), PageNo, Pagesize, SearchValue, sortbyvalue);
            if (lstcontacts != null && lstcontacts.Count > 0)
            {
                TempData["Contactscount"] = lstcontacts.Count;
            }

            return PartialView("_UserContactList", lstcontacts);
        }

        [HttpPost]
        public JsonResult UserContactListCount(string SearchValue = "", string sortbyvalue = "accepteddate")
        {
            // List<UserContactsViewModel> lstcontacts = new List<UserContactsViewModel>();

            int UserId = UM.GetLoggedInUserInfo().UserID;

            int PageNo = 1;
            int Pagesize = GlobalConstants.PageSize;

            var lstcontacts = UCM.GetUserContactList(UserId, Server.MapPath("~/Content/images"), PageNo, Pagesize, SearchValue, sortbyvalue);

            return Json(lstcontacts.Count, JsonRequestBehavior.AllowGet);
        }
        

        [HttpGet]
        public ActionResult PendingContactList(string SearchValue = "")
        {
            List<UserContactsViewModel> lstinv = new List<UserContactsViewModel>();

            lstinv = UCM.GetPendingInvitationList(UM.GetLoggedInUserInfo().UserID, Server.MapPath("~/Content/images"), SearchValue);

            return PartialView("_PendingContactList", lstinv);
        }

        public ActionResult PendingInvitation()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Contacts/Index");
            lstBreadcrumb.Add("Contacts");
            lstBreadcrumb.Add("Pending Invitation");
            ViewBag.lstbdcomb = lstBreadcrumb;

            return View();


        }

        [HttpPost]
        public JsonResult SendInvitation(int UserID)
        {
            bool IsValidInvitee = true;
            string ReturnMessage = string.Empty;

            UCM.SendInvitation(UM.GetLoggedInUserInfo().UserID, UserID, ref IsValidInvitee, ref ReturnMessage);

            return Json(new { Result = true, Message = ReturnMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ContactAcceptReject(int ContactID, string mode)
        {
            UserContactsViewModel model = UCM.GetContactDetailsByContactID(ContactID);

            if (model != null && model.FKSenderUserID > 0 && !model.IsAccepted && !model.IsRejected && !model.IsDeleted)
            {

                if (mode.ToLower().Contains("a"))
                {
                    mode = "accepted";

                    UCM.SaveContactAccept(model.ContactID); // IF ACCEPTED THEN SAVE A REVERSE ENTRY IN USERCONTACTS TABLE

                }
                else if (mode.ToLower().Contains("r"))
                {
                    mode = "rejected";
                }

                if (UCM.UpdateContactsReplyType(model.ContactID, mode))
                {
                    TempData["SuccessMessage"] = "Invitation " + mode;
                }

                return RedirectToAction("Index", "Contacts");
            }
            else
            {
                TempData["ErrorMessage"] = "User Does not exist";
                return RedirectToAction("Index", "Contacts");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult GetInfiniteScrollContacts(int PageNo = 1, int Pagesize = 10, string SearchValue = "")
        {
            var lst = UCM.GetInfiniteScrollContacts(PageNo, Pagesize, SearchValue);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ContactsInvitation()

        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Contacts/Index");
            lstBreadcrumb.Add("Contacts");
            lstBreadcrumb.Add("Contact Invitation");
            ViewBag.lstbdcomb = lstBreadcrumb;

            return View();
        }

        public ActionResult ContactsInvitationList(string SearchValue)
        {
            var lstcontacts = UCM.SearchLeads(UM.GetLoggedInUserInfo().UserID, Server.MapPath("~/Content/images"), SearchValue);
            return PartialView("_ContactsInvitationList", lstcontacts);
        }

    }
}