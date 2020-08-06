using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDGApp.Helpers;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Text;

namespace SDGApp.Controllers
{
    [UserAuthorization]
    public class UserMessageController : Controller
    {

        BaseModel BM;
        UserModel UM;
        HelpModel HM;
        UserMessageModel UMM;
        MailHelper MH;

        public UserMessageController()
        {
            BM = new BaseModel();
            UM = new UserModel();
            HM = new HelpModel();
            UMM = new UserMessageModel();
            MH = new MailHelper();
        }


        public ActionResult Index()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Messages");
            ViewBag.lstbdcomb = lstBreadcrumb;

            return View();
        }


        public ActionResult ComposeMail()
        {
            
            UserMessageViewModel model = new UserMessageViewModel();

            return PartialView("_ComposeMail", model);
        }


        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ComposeMail(UserMessageViewModel model)
        {
            if (ModelState.IsValid)
            {
                

                if (!string.IsNullOrEmpty(model.UserEmailTo) && !string.IsNullOrEmpty(model.MessageTo))
                {
                    model.MessageTo = model.UserEmailTo.TrimEnd(' ');
                }

                if (!string.IsNullOrEmpty(model.UserEmailCc) && !string.IsNullOrEmpty(model.MessageCc))
                {
                    model.MessageCc = model.UserEmailCc.TrimEnd(' ');
                }

                model.lstemilto = new List<string>();
                model.lstemilCc = new List<string>();

                if (!string.IsNullOrEmpty(model.MessageTo))
                {
                    var msgto = model.MessageTo.ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var itemto in msgto)
                    {
                        string item_to = itemto.Split(',').Last();
                        model.lstemilto.Add(item_to);
                    }
                    model.lstemilto = model.lstemilto.Distinct().ToList();
                }

                if (!string.IsNullOrEmpty(model.MessageCc))
                {
                    var msgcc = model.MessageCc.ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var itemcc in msgcc)
                    {
                        string item_cc = itemcc.ToString().Split(',').Last();
                        model.lstemilCc.Add(item_cc);
                    }
                    model.lstemilCc = model.lstemilCc.Distinct().ToList();
                }

                //remove duplicate email

                var lstFilteredCc = model.lstemilCc.RemoveAll(a => model.lstemilto.Contains(a));


                model.LoginUserID = UM.GetLoggedInUserInfo().UserID;
                model.LoginUserEmail = UM.GetLoggedInUserInfo().Email;

                if (model.lstemilto != null && model.lstemilto.Count > 0)
                {
                    model.MessageTo = string.Join(",", model.lstemilto);
                }

                if (model.lstemilCc != null && model.lstemilCc.Count > 0)
                {
                    model.MessageCc = string.Join(",", model.lstemilCc);
                }


                if (UMM.ComposeNewMessage(model))
                {
                    TempData["SuccessMessage"] = "Message successfully send.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Message sending failed.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Message sending failed.";
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        public ActionResult MessageList(int pageto, int pagefrom, String messagetype)
        {
            int UserId = UM.GetLoggedInUserInfo().UserID;
            var MessageTypeid = 1;

            if (!String.IsNullOrEmpty(messagetype))
            {
                if (messagetype.ToLower() == SDGUtilities.MessageTypeList.Inbox.ToString().ToLower())
                {
                    MessageTypeid = (int)SDGUtilities.MessageTypeList.Inbox;
                }
                else if (messagetype.ToLower() == SDGUtilities.MessageTypeList.Sent.ToString().ToLower())
                {
                    MessageTypeid = (int)SDGUtilities.MessageTypeList.Sent;
                }
                else if (messagetype.ToLower() == SDGUtilities.MessageTypeList.Bin.ToString().ToLower())
                {
                    MessageTypeid = (int)SDGUtilities.MessageTypeList.Bin;
                }
                else if (messagetype.ToLower() == SDGUtilities.MessageTypeList.Draft.ToString().ToLower())
                {
                    MessageTypeid = (int)SDGUtilities.MessageTypeList.Draft;
                }

                ViewBag.MessageTypeName = Convert.ToString(messagetype);
            }

            List<UserMessageViewModel> lstinboxmg = UMM.FetchMessageList(UserId, pagefrom, pageto, MessageTypeid);
            return PartialView("_MessageList", lstinboxmg);
        }

        [HttpGet]
        public ActionResult ViewMessage(int MessageID, string SearchData)
        {
            ViewBag.SearchData = SearchData;
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("UserMessage/Index");
            lstBreadcrumb.Add("Message");
            lstBreadcrumb.Add("Edit");
            ViewBag.lstbdcomb = lstBreadcrumb;

            UserMessageViewModel MVM = new UserMessageViewModel();
            
            MVM = UMM.GetMessageDetailById(MessageID);
            
            return PartialView("_ViewMessage", MVM);
        }

        [HttpPost]
        public JsonResult DeleteMessage(int MessageID)
        {
            int UserId = UM.GetLoggedInUserInfo().UserID;
            if (UMM.DeleteMessageByMessageID(MessageID))
            {
                return Json(new { Result = true, Message = "Message deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Message = "Message deleted failed." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteMessageTrash(int MessageID)
        {
            int UserId = UM.GetLoggedInUserInfo().UserID;
            if (UMM.DeleteMessageTrash(MessageID))
            {
                return Json(new { Result = true, Message = "Message deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Message = "Message deleted failed." }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult UserViewedMessage(int MessageID)
        {
            if (UMM.ViewedMessage(MessageID))
            {
                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult MultipleMessageMarkAsRead()
        {
            int UserId = UM.GetLoggedInUserInfo().UserID;
            if (UMM.MessagesMarkAsRead(UserId))
            {
                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult MultipleMessageDelete(int[] arrMessageId)
        {
            if (arrMessageId != null && arrMessageId.Length > 0)
            {
                
                if (UMM.MessagesMarkAsDelete(arrMessageId))
                {
                    return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult RestoreFromTrash(int[] arrMessageId)
        {
            if (arrMessageId != null && arrMessageId.Length > 0)
            {
                int UserId = UM.GetLoggedInUserInfo().UserID;
                if (UMM.RestoreFromTrashMessages(arrMessageId, UserId))
                {
                    return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult RestoreMessageByID(int MessageID)
        {
            int UserId = UM.GetLoggedInUserInfo().UserID;
            if (UMM.RestoreMessageByID(UserId, MessageID))
            {
                return Json(new { Result = true, Message = "Message restored successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Message = "Message restored failed." }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult EmptyFromTrashByUserID()
        {
            int UserId = UM.GetLoggedInUserInfo().UserID;
            if (UMM.EmptyFromTrashByUserID(UserId))
            {
                return Json(new { Result = true, Message = "Trash empty successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Message = "Trash empty failed." }, JsonRequestBehavior.AllowGet);
            }
        }

        #region [ :: EDIT ]

        [HttpGet]
        public ActionResult Edit(int ID)
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("UserMessage/Index");
            lstBreadcrumb.Add("Message");
            lstBreadcrumb.Add("Edit");
            ViewBag.lstbdcomb = lstBreadcrumb;


            UserMessageViewModel MVM = new UserMessageViewModel();
            MVM = UMM.GetMessageDetailById(ID);


            return View(MVM);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Edit(UserMessageViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (!UMM.SendEmail(model.MessageTo, model.MessageCc, model.MessageSubject, model.MessageBody))
                {
                    ViewBag.ErrorMessage = "Email not send.";
                    return View(model);
                }


                if (UMM.UpdateMessage(model))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return PartialView("_MessageList", model);
                }

            }
            else
            {
                return View(model);
            }
        }

        #endregion


        #region [ :: DELETE ]



        #endregion

        #region [ :: AUTOCOMPLETE SEARCH ]

        [HttpPost]
        public JsonResult MessageSearchList(string prefix)
        {
            int UserID = UM.GetLoggedInUserInfo().UserID;
            var MessageToList = UMM.GetMessageSearchList(UserID, prefix);
            return Json(MessageToList);
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

        public ActionResult SearchMessageByValue(String searchvalue, String MessageTypeName)
        {
            ViewBag.SearchData = searchvalue;

            int messagetypeid = 1;

            if (!String.IsNullOrEmpty(MessageTypeName))
            {
                ViewBag.MessageTypeName = Convert.ToString(MessageTypeName.ToLower());

                if (MessageTypeName.ToLower() == SDGUtilities.MessageTypeList.Inbox.ToString().ToLower())
                {
                    messagetypeid = (int)SDGUtilities.MessageTypeList.Inbox;

                }
                else if (MessageTypeName.ToLower() == SDGUtilities.MessageTypeList.Sent.ToString().ToLower())
                {
                    messagetypeid = (int)SDGUtilities.MessageTypeList.Sent;
                }
                else if (MessageTypeName.ToLower() == SDGUtilities.MessageTypeList.Bin.ToString().ToLower())
                {
                    messagetypeid = (int)SDGUtilities.MessageTypeList.Bin;
                }
            }

            int UserId = UM.GetLoggedInUserInfo().UserID;
            List<UserMessageViewModel> searchmglst = UMM.SearchMailByValue(searchvalue, UserId, messagetypeid);
            return PartialView("_MessageList", searchmglst);

        }

        // AUTO SAVE CODE FOR DRAFT
        [ValidateInput(false)]
        [HttpPost]
        public JsonResult SaveDraft(int MessageID, string MessageTo = null, string MessageCc = null,
                                    string MessageSubject = null, string MessageBody = null)
        {
            int LoginUserID = UM.GetLoggedInUserInfo().UserID;
            string LoginUserEmail = UM.GetLoggedInUserInfo().Email;

            var Message_ID = UMM.SaveDraftDetails(MessageID, LoginUserID, LoginUserEmail, MessageTo, MessageCc,
                                                  MessageSubject, MessageBody);


            return Json(new { MessageID = Message_ID }, JsonRequestBehavior.AllowGet);

        }

        //public ActionResult EditMail(int MessageID)
        //{
        //    List<string> lstBreadcrumb = new List<string>();
        //    lstBreadcrumb.Add("UserMessage/Index");
        //    lstBreadcrumb.Add("Messages");
        //    lstBreadcrumb.Add("Compose");
        //    ViewBag.lstbdcomb = lstBreadcrumb;

        //    UserMessageViewModel model = new UserMessageViewModel();
        //    model = UMM.GetEditMessageDetailById(MessageID);

        //    return View(model);
        //}

        public ActionResult ComposeEditMail(int MessageID)
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("UserMessage/Index");
            lstBreadcrumb.Add("Messages");
            lstBreadcrumb.Add("Compose");
            ViewBag.lstbdcomb = lstBreadcrumb;

            UserMessageViewModel model = new UserMessageViewModel();
            model = UMM.GetEditMessageDetailById(MessageID);

            return PartialView("_ComposeEditMail", model);
        }



        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ComposeEditMail(UserMessageViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.UserEmailTo) && !string.IsNullOrEmpty(model.MessageTo))
                {
                    model.MessageTo = model.UserEmailTo.TrimEnd(' ');
                }

                if (!string.IsNullOrEmpty(model.UserEmailCc) && !string.IsNullOrEmpty(model.MessageCc))
                {
                    model.MessageCc = model.UserEmailCc.TrimEnd(' ');
                }


                model.LoginUserID = UM.GetLoggedInUserInfo().UserID;
                model.LoginUserEmail = UM.GetLoggedInUserInfo().Email;


                if (UMM.ComposeNewMessage(model))
                {
                    TempData["SuccessMessage"] = "Message successfully send.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Message sending failed.";
                    return RedirectToAction("Index");
                }


            }
            else
            {
                TempData["ErrorMessage"] = "Message sending failed.";
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public JsonResult MultipleMessageDeleteTrash(int[] arrMessageId)
        {
            if (arrMessageId != null && arrMessageId.Length > 0)
            {
                if (UMM.MessagesMarkAsDeleteTrash(arrMessageId))
                {
                    return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }

        }


        #region [ FILES FETCH , DELETE , DOWNLOAD ]

        [HttpGet]
        [AllowAnonymous]
        public FileResult DownloadFile(int FileID)
        {
            FileViewModel fvm = UMM.GetDocumentDetailsByID(FileID);
            string fileName = string.Empty;

            if (fvm != null && fvm.FileID > 0)
            {
                FileInfo fi2 = new FileInfo(Path.Combine(Server.MapPath("~/Content/EmailAttachments"), fvm.FileName));

                try
                {
                    if (fi2.Exists)
                    {
                        byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(Server.MapPath("~/Content/EmailAttachments"), fvm.FileName));
                        fileName = fvm.FileName;
                        string extention = fi2.Extension;
                        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                    }
                    else
                    {
                        return File(string.Empty, System.Net.Mime.MediaTypeNames.Application.Octet, string.Empty);
                    }
                }
                catch (Exception Ex)
                {
                    return null;
                }

            }
            else
            {
                return File(string.Empty, System.Net.Mime.MediaTypeNames.Application.Octet, string.Empty);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public void DownloadAllAttachment(string FileIDs)
        {
            using (var zipStream = new ZipOutputStream(Response.OutputStream))
            {
                // Give the file name of downloaded zip file
                Response.AddHeader("Content-Disposition", "attachment; filename=MailAttachments.zip");
                // Define content type 
                Response.ContentType = "application/zip";

                var list = UMM.GetAllFilesByIDs(FileIDs);

                // Get all file path one by one
                foreach (var path in list)
                {
                    // Get every file size
                    byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Content/EmailAttachments/" + path.FileName));
                    // Get every file path
                    var fileEntry = new ZipEntry(Path.GetFileName(Server.MapPath(@"~/Content/EmailAttachments/" + path.FileName)))
                    {
                        Size = fileBytes.Length
                    };
                    zipStream.PutNextEntry(fileEntry);
                    zipStream.Write(fileBytes, 0, fileBytes.Length);
                }
                // Clear and closed zipStream object
                zipStream.Flush();
                zipStream.Close();
            }
        }



        [HttpPost]
        [AllowAnonymous]
        public JsonResult DeleteUploadedFile(int fileID, string filename)
        {

            try
            {
                if (!string.IsNullOrEmpty(filename))
                {
                    FileInfo fi2 = new FileInfo(Path.Combine(Server.MapPath("~/Content/EmailAttachments"), filename));

                    if (fi2.Exists)
                    {
                        try
                        {
                            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/EmailAttachments"), filename));
                        }
                        catch (Exception Ex)
                        {

                        }

                        UMM.DeleteUploadedFileID(fileID);

                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = "File delete failed" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = true, message = "File deleted successfully" }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region [ Get Message BY ID For Reply ]
        #region [ Get Message BY ID For Reply ]
        public JsonResult GetMessageDetailForReplyById(int MessageID, int ReplyTypeID)
        {
            UserMessageViewModel model = new UserMessageViewModel();

            model = UMM.GetMessageDetailById(MessageID);


            var LogedInUserMailid = UM.GetLoggedInUserInfo().Email;
            var LogedInUserName = UM.GetLoggedInUserInfo().FirstName + " " + UM.GetLoggedInUserInfo().LastName;


            if (ReplyTypeID == (int)SDGUtilities.UserMessageReplyTypeList.Reply)
            {
                if (!String.IsNullOrEmpty(model.MessageFrom) && !String.IsNullOrEmpty(model.SenderUserName))
                {
                    model.MessageTo = model.SenderUserName;

                    model.UserEmailTo = model.SenderUserName + "," + model.MessageFrom;
                }

            }
            else if (ReplyTypeID == (int)SDGUtilities.UserMessageReplyTypeList.ReplyToAll)
            {
                if (!String.IsNullOrEmpty(model.MessageTo))
                {
                    List<String> ReplyMessageTo = new List<string>();
                    List<String> ReplyUserEmailTo = new List<string>();

                    List<String> ItemsMails = model.MessageTo.Split(',').Select(i => i.Trim()).Where(i => i != string.Empty).ToList(); //Split them all and remove spaces
                    ItemsMails.Remove(LogedInUserMailid);

                    if (LogedInUserMailid != model.MessageFrom)
                    {
                        ItemsMails.Add(model.MessageFrom);
                    }


                    List<String> ItemsNames = model.ReceiverUserName.Split(',').Select(i => i.Trim()).Where(i => i != string.Empty).ToList(); //Split them all and remove spaces
                    ItemsNames.Remove(LogedInUserName);

                    if (LogedInUserName != model.SenderUserName)
                    {
                        ItemsNames.Add(model.SenderUserName);
                    }

                    for (int i = 0; i < ItemsNames.Count; i++)
                    {
                        ReplyMessageTo.Add(ItemsNames[i]);
                    }

                    for (int i = 0; i < ItemsNames.Count; i++)
                    {
                        ReplyUserEmailTo.Add(ItemsNames[i] + " , " + ItemsMails[i]);
                    }

                    model.MessageTo = String.Join(" , ", ReplyMessageTo);

                    model.UserEmailTo = String.Join(" | ", ReplyUserEmailTo);


                    //Atri Banerjee,  atri.tih@gmail.com | Amitava Mukherjee,  amitava.mukherjee@baseclass.co.in |

                }

                if (!String.IsNullOrEmpty(model.MessageCc))
                {
                    List<String> ReplyMessagecc = new List<string>();
                    List<String> ReplyUserEmailcc = new List<string>();

                    List<String> ItemsMails = model.MessageCc.Split(',').Select(i => i.Trim()).Where(i => i != string.Empty).ToList(); //Split them all and remove spaces
                    ItemsMails.Remove(LogedInUserMailid);

                    List<String> ItemsNames = model.UserNameCc.Split(',').Select(i => i.Trim()).Where(i => i != string.Empty).ToList(); //Split them all and remove spaces
                    ItemsNames.Remove(LogedInUserName);


                    for (int i = 0; i < ItemsNames.Count; i++)
                    {
                        ReplyMessagecc.Add(ItemsNames[i]);
                    }

                    for (int i = 0; i < ItemsNames.Count; i++)
                    {
                        ReplyUserEmailcc.Add(ItemsNames[i] + " , " + ItemsMails[i]);
                    }

                    model.MessageCc = String.Join(" , ", ReplyMessagecc);

                    model.UserEmailCc = String.Join(" | ", ReplyUserEmailcc);
                }
            }
            else if (ReplyTypeID == (int)SDGUtilities.UserMessageReplyTypeList.Forward)
            {
                model.ReceiverUserName = model.SenderUserName;
                model.SenderUserName = LogedInUserName;

            }



            model.MessageCreatedDateTime = model.CreatedDateTime.ToString("f");

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

    }
}