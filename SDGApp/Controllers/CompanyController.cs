using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.Controllers
{



    public class CompanyController : Controller
    {
        BaseModel BM;
        CompanyModel CM;
        HelpModel HM;
        ThirdPartyModel TPM;
        UserModel UM;

        public CompanyController()
        {

            BM = new BaseModel();
            CM = new CompanyModel();
            HM = new HelpModel();
            UM = new UserModel();
            TPM = new ThirdPartyModel();
        }
        // GET: Company
        public ActionResult Index()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Organisation");
            ViewBag.lstbdcomb = lstBreadcrumb;

            HM.SetHelpSessionValue("Company List");

            return View();
        }

        [HttpGet]
        public ActionResult CompanyList(int pageSize, int pageNumber, string SearchValue = "")
        {
            List<CompanyViewModel> lstuvm = CM.FetchCompanyList(pageSize, pageNumber, SearchValue);

            if (!String.IsNullOrEmpty(SearchValue))
            {
                ViewBag.SearchValue = SearchValue;
            }
            return PartialView("_CompanyList", lstuvm);
        }


        [HttpGet]
        public ActionResult Add()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Company/Index");
            lstBreadcrumb.Add("Organisation");
            lstBreadcrumb.Add("Add");
            ViewBag.lstbdcomb = lstBreadcrumb;

            CompanyViewModel model = new CompanyViewModel();
            //model.DDLAWSSubcription = CM.GetAWSCompanySubscriptionDDL();
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(CompanyViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (CM.AddNewCompany(model))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Organization name already exist.";
                    return View(model);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please put organization name and address.";
                return View(model);
            }
        }



        [HttpGet]
        public ActionResult Edit(int ID)
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Company/Index");
            lstBreadcrumb.Add("Organisation");
            lstBreadcrumb.Add("Edit");
            ViewBag.lstbdcomb = lstBreadcrumb;

            // AttributeRuleModel ATM = new AttributeRuleModel();
            CompanyViewModel CVM = new CompanyViewModel();
            CVM = CM.GetComanyDetailById(ID);
            // CVM.DDLAttributeLabel = ATM.GetAttributeLabelByAttrType(1);

            return View(CVM);
        }

        [HttpPost]
        public ActionResult Edit(CompanyViewModel model)
        {
            RemoveModelStateItem("CompanyName");
            if (ModelState.IsValid)
            {
                if (CM.UpdateCompanyProfile(model))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return PartialView("_CompanyList", model);
                }

            }
            return View(model);

        }

      

        [HttpPost]
        public JsonResult DeleteCompany(int CompanyID)
        {
            if (CM.DeleteCompanyByCompanyID(CompanyID))
            {
                return Json(new { Result = true, Message = "Organisation deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Message = "Organisation deleted unsuccessful." }, JsonRequestBehavior.AllowGet);
            }

        }

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
        

        [HttpPost]
        public JsonResult JoinOrganization(String CompanyCode)
        {

            if (!String.IsNullOrEmpty(CompanyCode))
            {
                int UserId = UM.GetLoggedInUserInfo().UserID;
                int UserRoleId = UM.GetLoggedInUserInfo().UserRoleID;

                if (CM.JoinOrganization(CompanyCode, UserId, UserRoleId))
                {
                    return Json(new { Result = true, Message = "Organisation join successfully." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false, Message = "Organisation joining failed." }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new { Result = false, Message = "Organisation code required." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetOrganizationListByUserId()
        {

            int UserId = UM.GetLoggedInUserInfo().UserID;

            if (UserId > 0)
            {
                var lst = CM.GetOrganisationListByUserId(UserId);

                if (lst.Count > 0)
                {
                    return Json(new { Result = true, Data = lst }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false, Data = lst }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Message = "User not found." }, JsonRequestBehavior.AllowGet);
            }


        }

        #region :: # Third party api

        [HttpGet]
        public ActionResult ThirdPartyApi(int ID)
        {
         
           ViewBag.CompanyID = ID;
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Third Party API Keys");
            ViewBag.lstbdcomb = lstBreadcrumb;
            return View();
        }


        [HttpPost]
        public JsonResult AddAPIKey(String inputAPIName, int CompanyID)
        {
            int UserID = UM.GetLoggedInUserInfo().UserID;
            var UserEmail = UM.GetLoggedInUserInfo().Email;
            var UserFirstName = UM.GetLoggedInUserInfo().FirstName;

            var result = TPM.AddAPIKey(inputAPIName, CompanyID, UserID, UserEmail, UserFirstName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteAPIKey(Int64 id)
        {
            var result = TPM.DeleteAPIKey(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LoadAPIKeys(Int64 CompanyID)
        {
            return PartialView("_APIKeysList", TPM.LoadAPIKeys(CompanyID));
        }

        #endregion

    }
}