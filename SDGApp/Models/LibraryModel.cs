using SDGApp.ViewModel;
using SDGAppDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.Models
    {
    public class LibraryModel : UserModel
        {

        #region ALL LISTINGS
        //THIS FETCHES ONLY PARENT TOPICS
        public List<SelectListItem> GetTopicDDLbyCompanyID()
            {
            List<SelectListItem> _list = new List<SelectListItem>();
            int companyid = GetLoggedInUserInfo().CompanyID;
            try
                {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                    _list = (from impl in db.LibraryTopic
                             where impl.IsActive && !impl.IsDelete && impl.FKCompanyID == companyid
                             select new SelectListItem { Text = impl.TopicName, Value = impl.TopicID.ToString() }
                             ).ToList();
                    }
                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - GetStoreDDLbyCompanyID", Ex.Message);
                }

            return _list;
            }

        public List<SelectListItem> GetTaskDDLbyCompanyID()
            {
            List<SelectListItem> _list = new List<SelectListItem>();
            int companyid = GetLoggedInUserInfo().CompanyID;
            try
                {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                    _list = (from impl in db.LibraryWork
                             where impl.IsActive && !impl.IsDelete && impl.FKCompanyID == companyid && impl.WorkParentID == 0
                             select new SelectListItem { Text = impl.WorkName, Value = impl.WorkID.ToString() }
                             ).ToList();
                    }
                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - GetStoreDDLbyCompanyID", Ex.Message);
                }

            return _list;
            }

        //THIS FETCHES ALL TOPICS
        public List<SelectListItem> GetTaskDDLAll()
            {
            List<SelectListItem> _list = new List<SelectListItem>();

            try
                {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                    _list = (from impl in db.LibraryWork
                             where impl.IsActive && !impl.IsDelete
                             select new SelectListItem { Text = impl.WorkName, Value = impl.WorkID.ToString() }
                             ).ToList();
                    }
                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - GetTopicDDLbyCompanyIDAll", Ex.Message);
                }

            return _list;
            }

        //FETCH ALL SUBTASK BY STORED PROCEDURE
        public List<LibraryViewModel> GetSubTaskByIDsp(int TaskID)
            {
            List<LibraryViewModel> lst = new List<LibraryViewModel>();
            List<DocumentViewModel> lstdoctask = new List<DocumentViewModel>();
            List<DocumentViewModel> lstdocsubtask = new List<DocumentViewModel>();
            try
                {
                SqlParameter[] Params =
                    {
                            new SqlParameter("@VARTASKID",TaskID),
                        };

                DataSet ds = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), CommandType.StoredProcedure, "USP_GET_LIBRARY_SUBTASK", Params);

                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                    lst = (from DataRow row in ds.Tables[0].Rows

                           select new LibraryViewModel
                               {
                               TopicID = GetIntegerValue(row["FKTopicID"]),
                               TopicName = GetStringValue(row["TopicName"]),
                               WorkID = GetIntegerValue(row["WorkID"]),
                               WorkName = GetStringValue(row["WorkName"]),
                               WorkDescription = GetStringValue(row["WorkDescription"]),
                               ParentCount = GetIntegerValue(row["PARENTCOUNT"]),
                               WorkParentID = GetIntegerValue(row["WorkParentID"]),
                               FileID = GetIntegerValue(row["FileID"]),
                               DocumentName = GetStringValue(row["DocumentName"])
                               }).ToList();
                    }

                if (ds != null && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                    {
                    lstdoctask = (from DataRow row in ds.Tables[1].Rows
                                  select new DocumentViewModel
                                      {
                                      FileID = GetIntegerValue(row["ID"]),
                                      FKWorkID = GetIntegerValue(row["FKWorkID"]),
                                      DisplayName = GetStringValue(row["DisplayName"])
                                      }).ToList();
                    }
                if (ds != null && ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                    {
                    lstdocsubtask = (from DataRow row in ds.Tables[2].Rows
                                     select new DocumentViewModel
                                         {
                                         FileID = GetIntegerValue(row["ID"]),
                                         FKWorkID = GetIntegerValue(row["FKWorkID"]),
                                         DisplayName = GetStringValue(row["DisplayName"])
                                         }).ToList();
                    }

                lst.ForEach(l => l.LstFilesTask = lstdoctask);
                lst.ForEach(l => l.LstFilesSubTask = lstdocsubtask);



                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - GetSubTaskByIDsp", Ex.Message);
                }
            return lst;
            }

        // FETCH ALL SUBTASK BY TOPIC IN STORED PROCEDURE
        public List<LibraryViewModel> GetSubTaskTopicByIDsp(int TaskID)
            {
            List<LibraryViewModel> lst1 = new List<LibraryViewModel>();
            List<LibraryViewModel> lst2 = new List<LibraryViewModel>();
            List<LibraryViewModel> lst3 = new List<LibraryViewModel>();
            List<DocumentViewModel> lstdoctask = new List<DocumentViewModel>();
            List<DocumentViewModel> lstdocsubtask = new List<DocumentViewModel>();
            try
                {
                SqlParameter[] Params =
                    {
                            new SqlParameter("@VARTASKID",TaskID),
                        };

                DataSet ds = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), CommandType.StoredProcedure, "USP_GET_LIBRARY_SUBTASK_BY_TOPICID", Params);

                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                    lst1 = (from DataRow row in ds.Tables[0].Rows

                            select new LibraryViewModel
                                {
                                TopicID = GetIntegerValue(row["TopicID"]),
                                TopicName = GetStringValue(row["TopicName"])
                                //,WorkDescription = GetStringValue(row["WorkDescription"]),
                                //ParentCount = GetIntegerValue(row["PARENTCOUNT"]),
                                //WorkParentID = GetIntegerValue(row["WorkParentID"]),
                                //FileID = GetIntegerValue(row["FileID"]),
                                //DocumentName = GetStringValue(row["DocumentName"])
                                }).ToList();
                    }

                if (ds != null && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                    {
                    lst2 = (from DataRow row in ds.Tables[1].Rows

                            select new LibraryViewModel
                                {
                                WorkID = GetIntegerValue(row["WorkID"]),
                                WorkName = GetStringValue(row["WorkName"]),
                                WorkDescription = GetStringValue(row["WorkDescription"]),
                                ParentCount = GetIntegerValue(row["PARENTCOUNT"]),
                                WorkParentID = GetIntegerValue(row["WorkParentID"]),
                                FileID = GetIntegerValue(row["FileID"]),
                                DocumentName = GetStringValue(row["DocumentName"])
                                }).ToList();
                    }

                if (ds != null && ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                    {
                    lstdoctask = (from DataRow row in ds.Tables[2].Rows
                                  select new DocumentViewModel
                                      {
                                      FileID = GetIntegerValue(row["ID"]),
                                      FKWorkID = GetIntegerValue(row["FKWorkID"]),
                                      DisplayName = GetStringValue(row["DisplayName"])
                                      }).ToList();
                    }
                if (ds != null && ds.Tables[3] != null && ds.Tables[3].Rows.Count > 0)
                    {
                    lstdocsubtask = (from DataRow row in ds.Tables[3].Rows
                                     select new DocumentViewModel
                                         {
                                         FileID = GetIntegerValue(row["ID"]),
                                         FKWorkID = GetIntegerValue(row["FKWorkID"]),
                                         DisplayName = GetStringValue(row["DisplayName"])
                                         }).ToList();
                    }

                //lst1.AddRange(lst2);
                if (lst2.Count > 0)
                    {
                    for (var i = 0; i < lst2.Count; i++)
                        {
                        lst2[i].TopicID = lst1.FirstOrDefault().TopicID;
                        lst2[i].TopicName = lst1.FirstOrDefault().TopicName;
                        if (lstdoctask.Count > 0)
                            {
                            if (lst2[i].WorkID == lstdoctask[0].FKWorkID)
                                {
                                lst2[i].LstFilesTask = lstdoctask;
                                }
                            }

                        if (lstdocsubtask.Count > 0)
                            {
                            if (lst2[i].WorkID == lstdoctask[0].FKWorkID)
                                {
                                lst2[i].LstFilesSubTask = lstdocsubtask;
                                }
                            }
                        }
                    /*lst2.ForEach(l => l.LstFilesTask = lstdoctask);
                    lst2.ForEach(l => l.LstFilesSubTask = lstdocsubtask);*/
                    }
                else
                    {
                    lst1.ForEach(l => l.LstFilesTask = lstdoctask);
                    lst1.ForEach(l => l.LstFilesSubTask = lstdocsubtask);


                    //if (lst1.Count > 0)
                    //{
                    //    lst2[0].TopicID = lst1.FirstOrDefault().TopicID;
                    //    lst2[0].TopicName = lst1.FirstOrDefault().TopicName;
                    //}
                    }


                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - GetSubTaskTopicByIDsp", Ex.Message);
                }
            return lst2.Count > 0 ? lst2 : lst1;
            }

        //FETCH ALL TASK AND SUBTASK BY STORED PROCEDURE
        public List<LibraryViewModel>[] GetTaskandSubTask()
            {
            List<LibraryViewModel> lst1 = new List<LibraryViewModel>();
            List<LibraryViewModel> lst2 = new List<LibraryViewModel>();
            List<LibraryViewModel>[] lst3 = new List<LibraryViewModel>[2];
            int companyid = GetLoggedInUserInfo().CompanyID;
            try
                {
                SqlParameter[] Params =
                    {
                            new SqlParameter("@VARCOMPANYID",companyid),
                        };

                DataSet ds = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), CommandType.StoredProcedure, "USP_GET_LIBRARY_TASKandSUBTASK", Params);

                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                    lst1 = (from DataRow row in ds.Tables[0].Rows

                            select new LibraryViewModel
                                {
                                WorkID = GetIntegerValue(row["WorkID"]),
                                WorkName = GetStringValue(row["WorkName"]),
                                WorkDescription = GetStringValue(row["WorkDescription"]),
                                ParentCount = GetIntegerValue(row["PARENTCOUNT"]),
                                WorkParentID = GetIntegerValue(row["WorkParentID"])
                                }).ToList();
                    }
                lst3[0] = lst1;

                if (ds != null && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                    {
                    lst2 = (from DataRow row in ds.Tables[1].Rows

                            select new LibraryViewModel
                                {
                                WorkID = GetIntegerValue(row["WorkID"]),
                                WorkName = GetStringValue(row["WorkName"]),
                                WorkDescription = GetStringValue(row["WorkDescription"]),
                                ParentCount = GetIntegerValue(row["PARENTCOUNT"]),
                                WorkParentID = GetIntegerValue(row["WorkParentID"])
                                }).ToList();
                    }
                lst3[1] = lst2;

                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - GetSubTaskByIDsp", Ex.Message);
                }
            return lst3;
            }

        //FETCH ALL TOPIC AND TASK
        public List<LibraryViewModel>[] GetTopicandTask()
            {
            List<LibraryViewModel> lst1 = new List<LibraryViewModel>();
            List<LibraryViewModel> lst2 = new List<LibraryViewModel>();
            List<LibraryViewModel>[] lst3 = new List<LibraryViewModel>[2];
            int companyid = GetLoggedInUserInfo().CompanyID;
            try
                {
                SqlParameter[] Params =
                    {
                            new SqlParameter("@VARCOMPANYID",companyid),
                        };

                DataSet ds = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), CommandType.StoredProcedure, "USP_GET_LIBRARY_TOPICandTASK", Params);

                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                    lst1 = (from DataRow row in ds.Tables[0].Rows

                            select new LibraryViewModel
                                {
                                WorkID = GetIntegerValue(row["TopicID"]),
                                WorkName = GetStringValue(row["TopicName"]),
                                ParentCount = GetIntegerValue(row["PARENTCOUNT"]),
                                TopicWidth = GetStringValue(row["TopicWidth"])
                                }).ToList();
                    }
                lst3[0] = lst1;

                if (ds != null && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                    {
                    lst2 = (from DataRow row in ds.Tables[1].Rows

                            select new LibraryViewModel
                                {
                                WorkID = GetIntegerValue(row["WorkID"]),
                                WorkName = GetStringValue(row["WorkName"]),
                                WorkParentID = GetIntegerValue(row["FKTopicID"])
                                }).ToList();
                    }
                lst3[1] = lst2;

                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - GetTopicandTask", Ex.Message);
                }
            return lst3;
            }


        #endregion

        #region :: 2. Add

        //ADD NEW TASK
        public bool AddNewTask(LibraryViewModel model, HttpPostedFileBase[] files, string[] txtFileName, string OriginalFilename)
            {
            bool result = false;
            int DefaultCompID = GetLoggedInUserInfo().CompanyID;

            try
                {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                    var entity = new SDGAppDB.POCO.LibraryWork();
                    entity.FKTopicID = GetIntegerValue(model.WorkParentID);
                    entity.WorkName = model.WorkName;
                    entity.WorkDescription = model.WorkDescription;
                    entity.IsActive = true;
                    entity.IsDelete = false;
                    entity.FKCompanyID = DefaultCompID;
                    entity.Note = model.Note;
                    entity.OrderID = model.OrderID;
                    //entity.DocumentName = GetStringValue(model.FileName);

                    db.LibraryWork.Add(entity);
                    db.SaveChanges();

                    //TO SAVE MULTIPLE FILES
                    //SaveUploadedFiles(files, entity.WorkID);
                    if (txtFileName != null && txtFileName[0] != null)
                        {
                        SaveUploadedFilesText(txtFileName, OriginalFilename, entity.WorkID);
                        }
                    //TO SEND MAIL
                    //TaskMail(model.UserID, model.WorkName);

                    result = true;
                    }
                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - AddNewTask", Ex.Message);
                }
            return result;
            }

        //ADD NEW SUBTASK
        public bool AddNewSubTask(LibraryViewModel model, HttpPostedFileBase[] files, string[] txtFileName, string OriginalFilename)
            {
            bool result = false;
            int DefaultCompID = GetLoggedInUserInfo().CompanyID;

            try
                {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                    var taskentity = db.LibraryWork.Find(model.WorkParentID);
                    if (taskentity != null && taskentity.WorkID > 0)
                        {
                        var entity = new SDGAppDB.POCO.LibraryWork();
                        entity.FKTopicID = GetIntegerValue(taskentity.FKTopicID);
                        entity.WorkParentID = GetIntegerValue(model.WorkParentID);
                        entity.WorkName = model.WorkName;
                        entity.WorkDescription = model.WorkDescription;
                        entity.IsActive = true;
                        entity.IsDelete = false;
                        entity.FKCompanyID = DefaultCompID;
                        entity.Note = model.Note;
                        entity.OrderID = model.OrderID;
                        db.LibraryWork.Add(entity);
                        db.SaveChanges();

                        //TO SAVE MULTIPLE FILES
                        //SaveUploadedFiles(files, entity.WorkID);
                        if (txtFileName != null && txtFileName[0] != null)
                            {
                            SaveUploadedFilesText(txtFileName, OriginalFilename, entity.WorkID);
                            }
                        //TO SEND MAIL
                        //TaskMail(model.UserID, model.WorkName);

                        result = true;
                        }

                    }
                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - AddNewSubTask", Ex.Message);
                }
            return result;
            }

        //ADD NEW TOPIC
        public bool AddNewTopic(LibraryViewModel model)
            {
            bool result = false;
            int DefaultCompID = GetLoggedInUserInfo().CompanyID;

            try
                {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                    var entity = new SDGAppDB.POCO.LibraryTopic();
                    entity.TopicName = model.WorkName;
                    entity.TopicDescription = model.WorkDescription;
                    entity.IsActive = true;
                    entity.IsDelete = false;
                    entity.FKCompanyID = DefaultCompID;
                    entity.OrderID = model.OrderID;
                    entity.TopicWidth = model.TopicWidth;

                    db.LibraryTopic.Add(entity);
                    db.SaveChanges();

                    result = true;
                    }
                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - AddNewTopic", Ex.Message);
                }
            return result;
            }

        private void SaveUploadedFiles(HttpPostedFileBase[] files, int WorkID)
            {
            foreach (HttpPostedFileBase file in files)
                {
                string renamedpath = string.Empty;
                string renamedfile = string.Empty;
                string extension = string.Empty;
                //Checking file is available to save.  
                if (file != null)
                    {
                    try
                        {
                        renamedfile = System.Guid.NewGuid().ToString("N");
                        extension = Path.GetExtension(file.FileName);
                        renamedpath = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/LibraryDocument"), renamedfile + extension);
                        //Save file to server folder  
                        file.SaveAs(renamedpath);

                        using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                            {
                            var entity = new SDGAppDB.POCO.LibraryWorkDocument();
                            entity.FKWorkID = WorkID;
                            entity.DocumentName = renamedfile + extension;
                            db.LibraryWorkDocument.Add(entity);
                            db.SaveChanges();
                            }
                        }
                    catch (Exception Ex)
                        {
                        }
                    }
                }
            }

        #endregion

        #region :: 3. Edit

        public LibraryViewModel GetTopicByID(int topicid)
            {
            LibraryViewModel model = new LibraryViewModel();
            try
                {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                    var TopicEntity = db.LibraryTopic.Find(topicid);
                    if (TopicEntity != null)
                        {
                        model.WorkID = TopicEntity.TopicID;
                        model.WorkName = TopicEntity.TopicName;
                        model.WorkDescription = TopicEntity.TopicDescription;
                        model.OrderID = TopicEntity.OrderID;
                        model.TopicWidth = TopicEntity.TopicWidth;
                        }
                    }

                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - GetTopicByID", Ex.Message);
                }
            return model;
            }

        public bool UpdateTopic(LibraryViewModel model)
            {
            bool result = false;
            try
                {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                    var TopicEntity = db.LibraryTopic.Find(model.WorkID);
                    if (TopicEntity != null)
                        {
                        TopicEntity.TopicName = model.WorkName;
                        TopicEntity.TopicDescription = model.WorkDescription;
                        TopicEntity.OrderID = model.OrderID;
                        TopicEntity.TopicWidth = model.TopicWidth;
                        db.Entry(TopicEntity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        result = true;
                        }

                    //THIS IS FOR UPDATING TOPIC WIDTH FOR ALL RECORDS
                    try
                        {
                        var entty = (from it in db.LibraryTopic
                                     select new { it }).ToList();
                        foreach (var item in entty)
                            {
                            item.it.TopicWidth = model.TopicWidth;
                            db.Entry(item.it).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            }
                        }
                    catch (Exception Ex)
                        {
                        }

                    }
                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - UpdateTopic", Ex.Message);
                }
            return result;
            }

        public LibraryViewModel GetTaskByID(int taskid)
            {
            LibraryViewModel model = new LibraryViewModel();
            try
                {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                    var TaskEntity = db.LibraryWork.Find(taskid);
                    if (TaskEntity != null)
                        {
                        model.WorkID = TaskEntity.WorkID;
                        model.WorkName = TaskEntity.WorkName;
                        model.WorkDescription = TaskEntity.WorkDescription;
                        model.WorkParentID = TaskEntity.FKTopicID;
                        model.ParentText = GetTopicByID(TaskEntity.FKTopicID).WorkName;
                        model.Note = TaskEntity.Note;
                        model.DDLTopic = GetTopicDDLbyCompanyID();
                        model.FileName = GetStringValue(GetFilesName(TaskEntity.WorkID));
                        model.OrderID = TaskEntity.OrderID;

                        }
                    }

                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - GetTaskByID", Ex.Message);
                }
            return model;
            }

        public LibraryViewModel GetSubTaskByID(int taskid)
            {
            LibraryViewModel model = new LibraryViewModel();
            try
                {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                    var TaskEntity = db.LibraryWork.Find(taskid);
                    if (TaskEntity != null)
                        {
                        model.WorkID = TaskEntity.WorkID;
                        model.WorkName = TaskEntity.WorkName;
                        model.WorkDescription = TaskEntity.WorkDescription;
                        model.WorkParentID = TaskEntity.WorkParentID;
                        model.ParentText = GetTaskByID(TaskEntity.WorkParentID).WorkName;
                        model.Note = TaskEntity.Note;
                        model.DDLTopic = GetTopicDDLbyCompanyID();
                        model.FileName = GetStringValue(GetFilesName(taskid));
                        model.OrderID = TaskEntity.OrderID;

                        }
                    }

                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - GetTaskByID", Ex.Message);
                }
            return model;
            }

        public bool UpdateTask(LibraryViewModel model, HttpPostedFileBase[] files, string[] txtFileName, string OriginalFilename)
            {
            bool result = false;
            try
                {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                    var TaskEntity = db.LibraryWork.Find(model.WorkID);
                    if (TaskEntity != null && TaskEntity.WorkID > 0)
                        {
                        TaskEntity.WorkName = model.WorkName;
                        TaskEntity.WorkDescription = model.WorkDescription;
                        TaskEntity.Note = model.Note;
                        TaskEntity.OrderID = model.OrderID;

                        if (txtFileName != null && txtFileName[0] != null)
                            {
                            //TO FETCH DOCUMENT STORED
                            //var DocEntity = (from wd in db.LibraryWorkDocument
                            //                 where wd.FKWorkID == model.WorkID
                            //                 select new { wd }).ToList();
                            //if (DocEntity != null && DocEntity.Count > 0)
                            //{
                            //    foreach (var item in DocEntity)
                            //    {
                            //        var wentity = db.LibraryWorkDocument.Find(item.wd.ID);
                            //        db.Entry(wentity).State = System.Data.Entity.EntityState.Deleted;
                            //        db.SaveChanges();
                            //    }
                            //}

                            //SAVE UPLOADED FILES
                            //SaveUploadedFiles(files, model.WorkID);
                            SaveUploadedFilesText(txtFileName, OriginalFilename, model.WorkID);
                            }


                        db.Entry(TaskEntity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        result = true;
                        }


                    }
                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - UpdateTopic", Ex.Message);
                }
            return result;
            }

        public bool UpdateSubTask(LibraryViewModel model, HttpPostedFileBase[] files, string[] txtFileName, string OriginalFilename)
            {
            bool result = false;
            try
                {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                    var TaskEntity = db.LibraryWork.Find(model.WorkID);

                    if (TaskEntity != null)
                        {
                        TaskEntity.WorkName = model.WorkName;
                        TaskEntity.WorkDescription = model.WorkDescription;
                        TaskEntity.Note = model.Note;
                        TaskEntity.OrderID = model.OrderID;

                        if (txtFileName != null && txtFileName[0] != null)
                            {
                            //TO FETCH DOCUMENT STORED
                            //var DocEntity = (from wd in db.LibraryWorkDocument
                            //                 where wd.FKWorkID == model.WorkID
                            //                 select new { wd }).ToList();
                            //if (DocEntity != null && DocEntity.Count > 0)
                            //{
                            //    foreach (var item in DocEntity)
                            //    {
                            //        var wentity = db.LibraryWorkDocument.Find(item.wd.ID);
                            //        db.Entry(wentity).State = System.Data.Entity.EntityState.Deleted;
                            //        db.SaveChanges();
                            //    }
                            //}

                            //SAVE UPLOADED FILES
                            //SaveUploadedFiles(files, model.WorkID);
                            SaveUploadedFilesText(txtFileName, OriginalFilename, model.WorkID);
                            }


                        db.Entry(TaskEntity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        result = true;
                        }
                    }
                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - UpdateSubTask", Ex.Message);
                }
            return result;
            }

        public void TaskMail(int UserID, string TaskName)
            {
            UserModel UM = new UserModel();
            var UVM = UM.GetUserDetailByUserID(UserID);

            Dictionary<string, string> objDict = new Dictionary<string, string>();

            objDict.Add("TaskName", TaskName);
            objDict.Add("AssigneeName", UVM.FirstName + " " + UVM.LastName);
            objDict.Add("Year", DateTime.Now.Year.ToString());
            //objDict.Add("aidant_logo", GlobalConstants.BaseUrl + "/Content/images/aidant-logo.jpg");
            //objDict.Add("customer_logo", GlobalConstants.BaseUrl + "/Content/CompanyLogo/" + UVM.Picture + ".jpg");

            SDGApp.Helpers.MailHelper MH = new Helpers.MailHelper();
            if (MH.SendEmail("Task Assigned", UVM.Email, "TaskAssign.html", objDict))
                {
                string SuccessMessage = "Mail sent. Please check your email.";
                }
            }



        public string GetFilesName(int taskid)
            {
            string Filenames = string.Empty;
            try
                {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                    var entity = (from wd in db.LibraryWorkDocument
                                  where wd.FKWorkID == taskid
                                  select new { wd }).ToList();
                    if (entity != null && entity.Count > 0)
                        {
                        foreach (var item in entity)
                            {
                            Filenames += item.wd.DocumentName + ",";
                            }
                        }
                    }
                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - GetFilesName", Ex.Message);
                }
            return Filenames.TrimEnd(',');
            }


        #endregion

        #region :: 4 Delete

        public bool DeleteTopicTaskByID(int id, string flag)
            {
            bool result = false;
            try
                {
                int success = SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_DELETE_LIBRARY_TOPIC_TASK_SUBTASK_BYID", id, flag);

                if (success > 0)
                    {
                    result = true;
                    }
                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - DeleteTopicTaskByID", Ex.Message);
                }
            return result;
            }
        public bool DeleteUploadedFileID(string filename)
            {
            bool result = false;
            try
                {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                    var documentid = (from doc in db.LibraryWorkDocument where doc.DocumentName == filename select doc.ID).FirstOrDefault();
                    if (documentid > 0)
                        {
                        var user = db.LibraryWorkDocument.Find(documentid);
                        if (user != null)
                            {
                            db.LibraryWorkDocument.Remove(user);
                            db.SaveChanges();
                            result = true;
                            }
                        }
                    }
                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - DeleteUploadedFileID", Ex.Message);
                }
            return result;
            }

        #endregion

        // DownLoad File
        public LibraryViewModel GetDocumentDetailsByID(int fileID)
            {
            LibraryViewModel model = new LibraryViewModel();
            try
                {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                    var entity_doc = db.LibraryWorkDocument.Find(fileID);

                    if (entity_doc != null && entity_doc.ID > 0)
                        {
                        model.FileID = entity_doc.ID;
                        model.DisplayName = entity_doc.DisplayName;
                        model.DocumentName = entity_doc.DocumentName;
                        }
                    }
                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - GetDocumentDetailsByID", Ex.Message);
                }
            return model;
            }


        private void SaveUploadedFilesText(string[] txtfilenames, string originalfilename, int WorkID)
            {
            try
                {
                if (!string.IsNullOrEmpty(originalfilename))
                    {

                    string[] arrOrgFilename = originalfilename.Split(',');


                    using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                        {
                        var entity = new SDGAppDB.POCO.LibraryWorkDocument();
                        for (int i = 0; i < arrOrgFilename.Length; i++)
                            {

                            entity.FKWorkID = WorkID;
                            entity.DocumentName = arrOrgFilename[i];
                            entity.DisplayName = txtfilenames[i];
                            db.LibraryWorkDocument.Add(entity);
                            db.SaveChanges();
                            }

                        }
                    }
                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - SaveUploadedFilesText", Ex.Message);
                }

            }

        #region [ Return MaxID from task or subtask ]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Flag"></param>
        /// <returns></returns>
        public int GetMaxID(string Flag, int tid)
            {
            int maxid = 0;

            try
                {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                    //var topicid = (from work in db.ImplementationWork where work.WorkID == tid select work.FKTopicID).FirstOrDefault();

                    //if (topicid > 0)
                    //{
                    //    if (!string.IsNullOrEmpty(Flag) && Flag.ToLower() == "add task")
                    //    {
                    //        maxid = (from task in db.ImplementationWork
                    //                 where !task.IsDelete && task.FKTopicID == topicid && (task.WorkParentID == 0)
                    //                 select task.OrderId).Max() + 1;
                    //    }
                    //    else if (!string.IsNullOrEmpty(Flag) && Flag.ToLower() == "add subtask")
                    //    {
                    //        maxid = (from task in db.ImplementationWork
                    //                 where !task.IsDelete && task.FKTopicID == topicid && (task.WorkParentID > 0)
                    //                 select task.OrderId).Max() + 1;
                    //    }
                    //}
                    }
                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - GetMaxID", Ex.Message);
                }
            return maxid;
            }


        #endregion


        #region :: New Methods Added By Atri on 20181212

        public List<LibraryHierarchyViewModel> GetLibraryHierarchy()
            {
            List<LibraryHierarchyViewModel> model = new List<LibraryHierarchyViewModel>();
            List<LibraryWorkViewModel> TaskList = new List<LibraryWorkViewModel>();

            try
                {
                Int32 ActiveCompanyID = GetLoggedInUserInfo().CompanyID;

                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                    // Fetch list of active Topics for current company

                    var TopicEntity = (from lt in db.LibraryTopic where lt.FKCompanyID == ActiveCompanyID && lt.IsActive && !lt.IsDelete orderby lt.OrderID select new { lt }).ToList();

                    // Fetch list of Tasks for above Topics respectively

                    if (TopicEntity != null && TopicEntity.Count > 0)
                        {
                        foreach (var itemTopic in TopicEntity)
                            {
                            var Tasks = (from lw in db.LibraryWork
                                         where lw.FKTopicID == itemTopic.lt.TopicID && lw.FKCompanyID == itemTopic.lt.FKCompanyID && lw.WorkParentID == 0 && lw.IsActive && !lw.IsDelete
                                         orderby lw.OrderID
                                         select new LibraryWorkViewModel
                                             {
                                             WorkID = lw.WorkID,
                                             WorkName = lw.WorkName,
                                             WorkDescription = lw.WorkDescription,
                                             IsActive = lw.IsActive,
                                             IsDelete = lw.IsDelete,
                                             FKCompanyID = lw.FKCompanyID,
                                             FKTopicID = lw.FKTopicID,
                                             Note = lw.Note,
                                             OrderID = lw.OrderID
                                             }
                                       ).ToList();

                            // Fetch list of Sub Tasks & Documents for above Tasks respectively
                            if (Tasks != null && Tasks.Count > 0)
                                {
                                foreach (var itemTask in Tasks)
                                    {
                                    var TaskDocs = (from lwd in db.LibraryWorkDocument
                                                    where lwd.FKWorkID == itemTask.WorkID
                                                    select new LibraryWorkDocumentViewModel
                                                        {
                                                        ID = lwd.ID,
                                                        FKWorkID = lwd.FKWorkID,
                                                        DocumentName = lwd.DocumentName,
                                                        DisplayName = lwd.DisplayName
                                                        }
                                       ).ToList();

                                    // Update task doc List
                                    itemTask.TaskDocuments = TaskDocs;

                                    // Sub Tasks

                                    var SubTasks = (from lw in db.LibraryWork
                                                    where lw.FKTopicID == itemTopic.lt.TopicID && lw.FKCompanyID == itemTopic.lt.FKCompanyID && lw.WorkParentID == itemTask.WorkID && lw.IsActive && !lw.IsDelete
                                                    orderby lw.OrderID
                                                    select new LibrarySubWorkViewModel
                                                        {
                                                        WorkID = lw.WorkID,
                                                        WorkName = lw.WorkName,
                                                        WorkDescription = lw.WorkDescription,
                                                        WorkParentID = lw.WorkParentID,
                                                        IsActive = lw.IsActive,
                                                        IsDelete = lw.IsDelete,
                                                        FKCompanyID = lw.FKCompanyID,
                                                        FKTopicID = lw.FKTopicID,
                                                        Note = lw.Note,
                                                        OrderID = lw.OrderID
                                                        }
                                           ).ToList();

                                    // Update sub task List
                                    itemTask.SubTasks = SubTasks;

                                    // Fetch list of sub tasks documents for above SubTasks respectively


                                    if (SubTasks != null && SubTasks.Count > 0)
                                        {
                                        foreach (var itemSubTask in SubTasks)
                                            {
                                            var SubTaskDocs = (from lwd in db.LibraryWorkDocument
                                                               where lwd.FKWorkID == itemSubTask.WorkID
                                                               select new LibrarySubWorkDocumentViewModel
                                                                   {
                                                                   ID = lwd.ID,
                                                                   FKWorkID = lwd.FKWorkID,
                                                                   DocumentName = lwd.DocumentName,
                                                                   DisplayName = lwd.DisplayName
                                                                   }
                                               ).ToList();

                                            // Update sub task doc List
                                            itemSubTask.SubTaskDocuments = SubTaskDocs;
                                            }
                                        }
                                    }
                                TaskList = Tasks;
                                }


                            // Final binding
                            model.Add(new LibraryHierarchyViewModel
                                {

                                TopicID = itemTopic.lt.TopicID,
                                TopicName = itemTopic.lt.TopicName,
                                TopicDescription = itemTopic.lt.TopicDescription,
                                IsActive = itemTopic.lt.IsActive,
                                IsDelete = itemTopic.lt.IsDelete,
                                FKCompanyID = itemTopic.lt.FKCompanyID,
                                OrderID = itemTopic.lt.OrderID,
                                TopicWidth = itemTopic.lt.TopicWidth,
                                TaskList = TaskList
                                });

                            }
                        }
                    }
                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - GetLibraryHierarchy", Ex.Message);
                }
            return model;
            }

        public List<LibraryHierarchyViewModel> GetLibraryHierarchyUptoTaskLevel()
            {
            List<LibraryHierarchyViewModel> model = new List<LibraryHierarchyViewModel>();
            List<LibraryWorkViewModel> TaskList = new List<LibraryWorkViewModel>();

            try
                {
                Int32 ActiveCompanyID = GetLoggedInUserInfo().CompanyID;

                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                    // Fetch list of active Topics for current company

                    var TopicEntity = (from lt in db.LibraryTopic where lt.FKCompanyID == ActiveCompanyID && lt.IsActive && !lt.IsDelete orderby lt.OrderID select new { lt }).ToList();

                    // Fetch list of Tasks for above Topics respectively

                    if (TopicEntity != null && TopicEntity.Count > 0)
                        {
                        foreach (var itemTopic in TopicEntity)
                            {
                            //FOR GETTING SUBSTRING LENGTH
                            int substrlength = 4;
                            if (GetIntegerValue(itemTopic.lt.TopicWidth) == 6)
                                {
                                substrlength = 60;// (GetIntegerValue(itemTopic.lt.TopicWidth)) * 5;
                                }
                            else if (GetIntegerValue(itemTopic.lt.TopicWidth) == 5)
                                {
                                substrlength = 50;
                                }
                            else if (GetIntegerValue(itemTopic.lt.TopicWidth) == 4)
                                {
                                substrlength = 40;
                                }
                            else if (GetIntegerValue(itemTopic.lt.TopicWidth) == 3)
                                {
                                substrlength = 30;
                                }
                            else if (GetIntegerValue(itemTopic.lt.TopicWidth) == 2)
                                {
                                substrlength = 20;
                                }
                            else if (GetIntegerValue(itemTopic.lt.TopicWidth) == 1)
                                {
                                substrlength = 10;
                                }
                            else
                                {
                                substrlength = 45;
                                }

                            var Tasks = (from lw in db.LibraryWork
                                         where lw.FKTopicID == itemTopic.lt.TopicID && lw.FKCompanyID == itemTopic.lt.FKCompanyID && lw.WorkParentID == 0 && lw.IsActive && !lw.IsDelete
                                         orderby lw.OrderID
                                         select new LibraryWorkViewModel
                                             {
                                             WorkID = lw.WorkID,
                                             WorkName = lw.WorkName.Substring(0, lw.WorkName.Length < substrlength ? lw.WorkName.Length : substrlength),
                                             WorkDescription = lw.WorkDescription,
                                             IsActive = lw.IsActive,
                                             IsDelete = lw.IsDelete,
                                             FKCompanyID = lw.FKCompanyID,
                                             FKTopicID = lw.FKTopicID,
                                             Note = lw.Note,
                                             OrderID = lw.OrderID
                                             }
                                       ).ToList();

                            if (Tasks != null && Tasks.Count > 0)
                                {
                                TaskList = Tasks;
                                }

                            if (itemTopic != null || Tasks != null)
                                {
                                string strtopicname = itemTopic.lt.TopicName;
                                if (strtopicname != null)
                                    {
                                    if (substrlength < strtopicname.Length)
                                        {
                                        strtopicname = strtopicname.Substring(0, substrlength) + "..";
                                        }
                                    }

                                TaskList = new List<LibraryWorkViewModel>();

                                // Final binding
                                model.Add(new LibraryHierarchyViewModel
                                    {

                                    TopicID = itemTopic.lt.TopicID,
                                    TopicName = strtopicname,// itemTopic.lt.TopicName,
                                    TopicDescription = itemTopic.lt.TopicDescription,
                                    IsActive = itemTopic.lt.IsActive,
                                    IsDelete = itemTopic.lt.IsDelete,
                                    FKCompanyID = itemTopic.lt.FKCompanyID,
                                    OrderID = itemTopic.lt.OrderID,
                                    TopicWidth = itemTopic.lt.TopicWidth,
                                    TaskList = Tasks
                                    });
                                }


                            }
                        }
                    }
                }
            catch (Exception Ex)
                {
                WriteLog("SDGApp.Models.LibraryModel - GetLibraryHierarchy", Ex.Message);
                }
            return model;
            }

        #endregion

        }




    }