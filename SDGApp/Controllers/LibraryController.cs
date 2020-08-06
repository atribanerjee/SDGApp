using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SDGApp.Controllers
{
    [UserAuthorization]
    public class LibraryController : Controller
    {
        BaseModel BM;
        UserModel UM;
        //SearchModel SEM;
        LibraryModel LBM;

        public LibraryController()
        {
            BM = new BaseModel();
            UM = new UserModel();
           // SEM = new SearchModel();
            LBM = new LibraryModel();
        }

        // GET: Library
        public ActionResult Index()
        {
            List<string> lstBreadcrumb = new List<string>();
            //lstBreadcrumb.Add("Library");
            //lstBreadcrumb.Add("Library");
            //lstBreadcrumb.Add("List");
            //ViewBag.lstbdcomb = lstBreadcrumb;
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Library");
            ViewBag.lstbdcomb = lstBreadcrumb;

            LBM.GetLibraryHierarchy();
            if (!String.IsNullOrEmpty(Convert.ToString(TempData["SuccessMessage"])))
            {
                ViewBag.SuccessMessage = Convert.ToString(TempData["SuccessMessage"]);
                TempData.Remove("SuccessMessage");
            }
            if (!String.IsNullOrEmpty(Convert.ToString(TempData["InfoMessage"])))
            {
                ViewBag.InfoMessage = Convert.ToString(TempData["InfoMessage"]);
                TempData.Remove("InfoMessage");
            }
            if (!String.IsNullOrEmpty(Convert.ToString(TempData["ErrorMessage"])))
            {
                ViewBag.ErrorMessage = Convert.ToString(TempData["ErrorMessage"]);
                TempData.Remove("ErrorMessage");
            }
            LibraryViewModel model = new LibraryViewModel();
            ViewBag.arrList = LBM.GetTopicandTask();

            return View(model);
        }

        [HttpGet]
        public ActionResult AddEditTopicTask(string Flag, int Tid)
        {
            
            LibraryViewModel model = new LibraryViewModel();
            ViewBag.TaskID = Tid;
            if (Flag.ToLower() == "add topic")
            {
                model.WorkFlag = Flag;
                model.OrderID = 1;
            }
            else if (Flag.ToLower() == "edit topic")
            {
                model = LBM.GetTopicByID(Tid);
                model.WorkFlag = Flag;
                model.OrderID = 1;
            }
            else if (Flag.ToLower() == "add task")
            {
                model.DDLTopic = LBM.GetTopicDDLbyCompanyID();
                model.ParentText = LBM.GetTopicByID(Tid).WorkName;
                model.WorkParentID = Tid;
                model.WorkFlag = Flag;
               // model.OrderID = LBM.GetMaxID(Flag, Tid);
            }
            else if (Flag.ToLower() == "edit task")
            {
                model = LBM.GetTaskByID(Tid);
                model.WorkFlag = Flag;
                model.OrderID = model.OrderID;
            }
            else if (Flag.ToLower() == "add subtask")
            {
                model.DDLTopic = LBM.GetTaskDDLbyCompanyID();
                model.ParentText = LBM.GetTaskByID(Tid).WorkName;
                model.WorkParentID = Tid;
                model.WorkFlag = Flag;
                //model.OrderID = LBM.GetMaxID(Flag, Tid);
            }
            else if (Flag.ToLower() == "edit subtask")
            {
                model = LBM.GetSubTaskByID(Tid);
                model.WorkFlag = Flag;
                model.OrderID = 1;
            }

            return PartialView("_AddEditTaskTopic", model);
        }

        [HttpPost]
        public JsonResult AddEditTopicTask(string Flag, LibraryViewModel model, HttpPostedFileBase[] files, string[] Uploadfilename, string UploadFile)
        {
          
            string FileName = string.Empty;

            if (Flag.ToLower() == "add topic")
            {
                if (LBM.AddNewTopic(model))
                {
                 
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //watch1.Stop();
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

            }
            else if (Flag.ToLower() == "edit topic")
            {
                if (LBM.UpdateTopic(model))
                {
                   
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //watch1.Stop();
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }

            else if (Flag.ToLower() == "add task")
            {
                if (LBM.AddNewTask(model, files, Uploadfilename, UploadFile))
                {
                  
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //watch1.Stop();
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

            }
            else if (Flag.ToLower() == "edit task")
            {
                if (LBM.UpdateTask(model, files, Uploadfilename, UploadFile))
                {
                  
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //watch1.Stop();
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

            }
            else if (Flag.ToLower() == "add subtask")
            {
                if (LBM.AddNewSubTask(model, files, Uploadfilename, UploadFile))
                {
                  
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //watch1.Stop();
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else if (Flag.ToLower() == "edit subtask")
            {

                if (LBM.UpdateSubTask(model, files, Uploadfilename, UploadFile))
                {
                   
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //watch1.Stop();
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                //watch1.Stop();
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        #region :: 1. List
       

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetSubTaskByTaskID(int taskid)
        {
            //var watch1 = System.Diagnostics.Stopwatch.StartNew();   // START TIMER
            //string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            //string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            List<LibraryViewModel> lst = new List<LibraryViewModel>();
            ViewBag.TaskID = taskid;
            lst = LBM.GetSubTaskByIDsp(taskid);
            if (lst.Count > 0)
            {
                //watch1.Stop();
                // SAVE LOADING TIME IN LOADING TABLE
                //BM.SaveEntityLoadingTime(controllerName, actionName, BM.GetIntegerValue(watch1.ElapsedMilliseconds));
                return Json(new { list = lst, Result = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //watch1.Stop();
                return Json(new { list = lst, Result = false }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        #region :: 2. Add

        [HttpGet]
        public ActionResult Add()
        {
            //var watch1 = System.Diagnostics.Stopwatch.StartNew();   // START TIMER
            //string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            //string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            LibraryViewModel model = new LibraryViewModel();
            model.DDLTopic = LBM.GetTopicDDLbyCompanyID();

            //watch1.Stop();
            // SAVE LOADING TIME IN LOADING TABLE
            //BM.SaveEntityLoadingTime(controllerName, actionName, BM.GetIntegerValue(watch1.ElapsedMilliseconds));

            return View(model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Add(LibraryViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (LBM.AddNewTask(model))
        //        {
        //            TempData["SuccessMessage"] = "Data Saved Successfully.";
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Data Not Saved.";
        //        }
        //    }
        //    model.DDLTopic = LBM.GetTopicDDLbyCompanyID();
        //    model.DDLWorkStatus = LBM.GetWorkStatus();
        //    return View(model);
        //}


        //TO SAVE THE TOPIC
        public JsonResult AddTopic(LibraryViewModel model)
        {
            //var watch1 = System.Diagnostics.Stopwatch.StartNew();   // START TIMER
            //string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            //string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            if (LBM.AddNewTopic(model))
            {
                //watch1.Stop();
                // SAVE LOADING TIME IN LOADING TABLE
                //BM.SaveEntityLoadingTime(controllerName, actionName, BM.GetIntegerValue(watch1.ElapsedMilliseconds));
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //watch1.Stop();
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

       


        #endregion

        #region Edit

        public ActionResult Edit(int TopicID)
        {
            //var watch1 = System.Diagnostics.Stopwatch.StartNew();   // START TIMER
            //string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            //string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            LibraryViewModel model = new LibraryViewModel();
            model = LBM.GetTopicByID(TopicID);

            //watch1.Stop();
            // SAVE LOADING TIME IN LOADING TABLE
            //BM.SaveEntityLoadingTime(controllerName, actionName, BM.GetIntegerValue(watch1.ElapsedMilliseconds));

            return View(model);
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

        #region :: 4. Delete Topic / Task / SubTask BY WORK ID

        [HttpPost]
        [AllowAnonymous]
        public JsonResult DeleteTopicTaskByID(int id, string flag)
        {
            //var watch1 = System.Diagnostics.Stopwatch.StartNew();   // START TIMER
            //string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            //string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            if (LBM.DeleteTopicTaskByID(id, flag))
            {
                //watch1.Stop();
                // SAVE LOADING TIME IN LOADING TABLE
                //BM.SaveEntityLoadingTime(controllerName, actionName, BM.GetIntegerValue(watch1.ElapsedMilliseconds));
                return Json(new { Result = true, Message = "Deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //watch1.Stop();
                return Json(new { Result = false, Message = "Delete failed." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult DeleteUploadedFile(string filename)
        {
            //var watch1 = System.Diagnostics.Stopwatch.StartNew();   // START TIMER
            //string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            //string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            try
            {
                if (!string.IsNullOrEmpty(filename))
                {
                    FileInfo fi2 = new FileInfo(Path.Combine(Server.MapPath("~/Content/LibraryDocument"), filename));

                    if (fi2.Exists)
                    {
                        try
                        {
                            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/LibraryDocument"), filename));
                        }
                        catch (Exception Ex)
                        {
                            //watch1.Stop();
                        }

                        LBM.DeleteUploadedFileID(filename);

                    }
                }
            }
            catch (Exception ex)
            {
                //watch1.Stop();
                return Json(new { result = false, message = "File delete failed" }, JsonRequestBehavior.AllowGet);
            }

            //watch1.Stop();
            // SAVE LOADING TIME IN LOADING TABLE
            //BM.SaveEntityLoadingTime(controllerName, actionName, BM.GetIntegerValue(watch1.ElapsedMilliseconds));

            return Json(new { result = true, message = "File deleted successfully" }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        [HttpGet]
        [AllowAnonymous]
        public FileResult DownloadFile(int id)
        {
            LibraryViewModel lvm = LBM.GetDocumentDetailsByID(id);

            if (lvm != null && lvm.FileID > 0)
            {
                try
                {
                    FileInfo fi2 = new FileInfo(Path.Combine(Server.MapPath("~/Content/LibraryDocument"), lvm.DocumentName));

                    if (fi2.Exists)
                    {
                        byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(Server.MapPath("~/Content/LibraryDocument"), lvm.DocumentName));
                        string fileName = lvm.DisplayName;
                        string extention = fi2.Extension;
                        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName + extention);
                    }
                    else
                    {
                        return File("", System.Net.Mime.MediaTypeNames.Application.Octet, "");
                    }
                }
                catch (Exception Ex)
                {
                    return null;
                }

            }
            else
            {
                return File("", System.Net.Mime.MediaTypeNames.Application.Octet, "");
            }
        }


        //FOR UPLOADING IMAGES USING UPLOAD IMAGES
        [AllowAnonymous]
        [HttpPost]
        public JsonResult UploadLibraryFile()
        {
            Object result = new { FileName = "", FileUrl = "" };
            try
            {
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase AttachmentFile = Request.Files[file] as HttpPostedFileBase;
                    if (AttachmentFile.ContentLength == 0)
                        continue;
                    string renamedpath = string.Empty;
                    string renamedfile = string.Empty;
                    string extension = string.Empty;
                    //Checking file is available to save.  
                    if (AttachmentFile != null && AttachmentFile.ContentLength != 0)
                    {
                        try
                        {
                            renamedfile = System.Guid.NewGuid().ToString("N");
                            extension = Path.GetExtension(AttachmentFile.FileName);
                            renamedpath = Path.Combine(Server.MapPath("~/Content/LibraryDocument"), renamedfile + extension);
                            //string savedFileName = Path.Combine(Server.MapPath("~/Content/TaskDocument/"), renamedfile + extension);
                            //Save file to server folder  
                            AttachmentFile.SaveAs(renamedpath);
                            String FileUrl = GlobalConstants.BaseUrl + "~/Content/LibraryDocument/" + AttachmentFile.FileName;
                            result = new { FileName = renamedfile + extension, FileUrl = FileUrl };

                        }
                        catch (Exception Ex)
                        {
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //return Json("Upload failed");
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetSubTasksByTaskID(int taskid)
        {
            //var watch1 = System.Diagnostics.Stopwatch.StartNew();   // START TIMER
            //string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            //string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            List<LibraryViewModel> lstuvm = new List<LibraryViewModel>();
            ViewBag.TaskID = taskid;
            //ViewBag.arrList = LBM.GetTopicandTask();
            lstuvm = LBM.GetSubTaskByIDsp(taskid);

            //watch1.Stop();
            // SAVE LOADING TIME IN LOADING TABLE
            //BM.SaveEntityLoadingTime(controllerName, actionName, BM.GetIntegerValue(watch1.ElapsedMilliseconds));

            return PartialView("_TaskSubTaskList", lstuvm);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetSubTasksByByTopicID(int taskid)
        {
            //var watch1 = System.Diagnostics.Stopwatch.StartNew();   // START TIMER
            //string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            //string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            List<LibraryViewModel> lstuvm = new List<LibraryViewModel>();
            ViewBag.TaskID = taskid;
            //ViewBag.arrList = LBM.GetTopicandTask();
            lstuvm = LBM.GetSubTaskTopicByIDsp(taskid);

            //watch1.Stop();
            // SAVE LOADING TIME IN LOADING TABLE
            //BM.SaveEntityLoadingTime(controllerName, actionName, BM.GetIntegerValue(watch1.ElapsedMilliseconds));

            return PartialView("_TaskSubTaskList", lstuvm);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult TopicTaskSubTasklist()
        {
            //var watch1 = System.Diagnostics.Stopwatch.StartNew();   // START TIMER
            //string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            //string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            LibraryViewModel model = new LibraryViewModel();
            ViewBag.arrList = LBM.GetTopicandTask();

            //watch1.Stop();
            // SAVE LOADING TIME IN LOADING TABLE
            //BM.SaveEntityLoadingTime(controllerName, actionName, BM.GetIntegerValue(watch1.ElapsedMilliseconds));

            return PartialView("_TopicTaskSubTasklist", ViewBag.arrList);
        }



        #region :: New Methods Added By Atri on 20181212

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetLibraryHierarchy()
        {
            //var watch1 = System.Diagnostics.Stopwatch.StartNew();   // START TIMER
            //string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            //string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            List<LibraryHierarchyViewModel> model = LBM.GetLibraryHierarchyUptoTaskLevel();

            //watch1.Stop();
            // SAVE LOADING TIME IN LOADING TABLE
            //BM.SaveEntityLoadingTime(controllerName, actionName, BM.GetIntegerValue(watch1.ElapsedMilliseconds));

            return PartialView("_GetLibraryHierarchy", model);
        }

        #endregion

    }
}