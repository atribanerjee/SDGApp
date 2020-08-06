using SDGApp.Helpers;
using SDGApp.ViewModel;
using SDGAppDB;
using SDGAppDB.POCO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;

namespace SDGApp.Models
{
    public class UserMessageModel : UserModel
    {


        public string GetUserNamesByEmaillist(string emaillist)
        {
            // = "amit.tict@gmail.com , sandip.bhattacharjee@gmail.com"

            string stremails = string.Empty;
            List<string> lstemails = new List<string>();

            if (!string.IsNullOrEmpty(emaillist) && emaillist.Length > 0)
            {
                var msgto = emaillist.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var itememail in msgto)
                {
                    try
                    {
                        using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                        {
                            stremails = (from usr in db.User
                                         where usr.Email.ToLower().Contains(itememail.Trim())
                                         select usr.FirstName + " " + usr.LastName).FirstOrDefault();

                            if (!string.IsNullOrEmpty(stremails))
                            {
                                lstemails.Add(stremails);
                            }
                        }
                    }
                    catch (Exception Ex)
                    {
                        WriteLog("SDGApp.Models.UserMessageModel - GetUserNamesByEmaillist", Ex.Message);
                    }
                }

                if (lstemails.Count > 0)
                {
                    stremails = string.Join(",", lstemails);
                }
            }

            return stremails;
        }
        public string GetEmailListByUserNamelist(string namelist)
        {
            // = "amit.tict@gmail.com , sandip.bhattacharjee@gmail.com"

            string strnames = string.Empty;
            List<string> lstnames = new List<string>();

            if (!string.IsNullOrEmpty(namelist) && namelist.Length > 0)
            {
                var msgto = namelist.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in msgto)
                {
                    try
                    {
                        using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                        {

                            strnames = (from usr in db.User
                                        where (usr.FirstName.ToLower() + " " + usr.LastName.ToLower()).Contains(item.ToLower().Trim())
                                        select usr.Email).FirstOrDefault();

                            if (!string.IsNullOrEmpty(strnames))
                            {
                                lstnames.Add(strnames);
                            }
                        }
                    }
                    catch (Exception Ex)
                    {
                        WriteLog("SDGApp.Models.UserMessageModel - GetUserNamesByEmaillist", Ex.Message);
                    }
                }

                if (lstnames.Count > 0)
                {
                    strnames = string.Join(",", lstnames);
                }
            }

            return strnames;
        }
        public bool UpdateMessage(UserMessageViewModel mvm)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.UserMessage.Find(mvm.MessageID);

                    if (entity != null && entity.MessageID > 0)
                    {
                        entity.MessageTo = mvm.MessageTo;
                        entity.MessageCC = mvm.MessageCc;
                        entity.MessageSubject = mvm.MessageSubject;
                        entity.MessageBody = mvm.MessageBody;

                        //entity.IsActive = cvm.IsActive;

                        db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - UpdateMessage", Ex.Message);
            }
            return Result;

        }


        #region [Send Messages]

        public bool SendEmail(string To, string cc, string Subject, string Body)
        {
            bool Result = false;
            try
            {
                MailService MS = new MailService();
                Result = MS.SendMailMEssage(To, cc, Subject, Body);
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - SendEmail", Ex.Message);
            }

            return Result;
        }


        #endregion

        public UserViewModel GetUserDetaislByEmailID(string EmailID)
        {
            UserViewModel model = new UserViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    model = (from u in db.User
                             where u.Email.ToLower() == EmailID.Trim().ToLower() && !u.IsDeleted && u.IsActive
                             select new UserViewModel
                             {
                                 UserID = u.UserID,
                                 FirstName = u.FirstName,
                                 LastName = u.LastName,
                                 Email = u.Email

                             }).FirstOrDefault();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - GetUserDetaislByEmailID", Ex.Message);
            }
            return model;
        }

        public UserViewModel ServGetUserDetailByUserID(int UserID)
        {
            UserViewModel model = new UserViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    model = (from u in db.User
                             where u.UserID == UserID && !u.IsDeleted && u.IsActive
                             select new UserViewModel
                             {
                                 UserID = u.UserID,
                                 FirstName = u.FirstName,
                                 LastName = u.LastName,
                                 Email = u.Email

                             }).FirstOrDefault();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - ServGetUserDetailByUserID", Ex.Message);
            }
            return model;
        }


        #region [Restore Messages]
        public bool RestoreFromTrashMessages(int[] MessagesID, int UserId)
        {
            bool Result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    if (MessagesID != null && MessagesID.Length > 0)
                    {
                        foreach (var item in MessagesID)
                        {
                            var entity = db.UserMessage.Find(item);
                            if (entity != null)
                            {

                                if (UserId == entity.ReceiverUserID)
                                {
                                    entity.FkMessageTypeID = GetIntegerValue(SDGUtilities.MessageTypeList.Inbox);
                                }
                                else if (UserId == entity.SenderUserID)
                                {
                                    entity.FkMessageTypeID = GetIntegerValue(SDGUtilities.MessageTypeList.Sent);
                                }

                                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();

                                Result = true;
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - RestoreFromTrashMessages", Ex.Message);
            }
            return Result;
        }

        public bool RestoreMessageByID(int UserId, int MessageID)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.UserMessage.Find(MessageID);
                    if (entity != null)
                    {
                        if (UserId == entity.ReceiverUserID)
                        {
                            entity.FkMessageTypeID = GetIntegerValue(SDGUtilities.MessageTypeList.Inbox);
                        }
                        else if (UserId == entity.SenderUserID)
                        {
                            entity.FkMessageTypeID = GetIntegerValue(SDGUtilities.MessageTypeList.Sent);
                        }

                        db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - RestoreMessageByID", Ex.Message);
            }
            return Result;
        }

        #endregion


        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        #region [Draft messages]

        public int SaveDraftDetails(int MessageID, int LoginUserID, string LoginUserEmail, string MessageTo = null, string MessageCc = null,
                                    string MessageSubject = null, string MessageBody = null)
        {
            int messageid = 0;

            List<string> lstemilto = new List<string>();
            List<string> lstemilCc = new List<string>();

            try
            {

                if (!string.IsNullOrEmpty(MessageTo))
                {
                    var msgto = MessageTo.ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var itemto in msgto)
                    {
                        string item_to = itemto.ToString().Split(',').Last();
                        lstemilto.Add(item_to);
                    }
                    lstemilto = lstemilto.Distinct().ToList();
                }

                if (!string.IsNullOrEmpty(MessageCc))
                {
                    var msgcc = MessageCc.ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var itemcc in msgcc)
                    {
                        string item_cc = itemcc.ToString().Split(',').Last();
                        lstemilCc.Add(item_cc);
                    }
                    lstemilCc = lstemilCc.Distinct().ToList();
                }

                //differences will be the data
                var lstFilteredCc = lstemilCc.RemoveAll(a => lstemilto.Contains(a));

                if (lstemilto != null && lstemilto.Count > 0)
                {
                    MessageTo = string.Join(",", lstemilto);
                }
                else
                {
                    MessageTo = "";
                }

                if (lstemilCc != null && lstemilCc.Count > 0)
                {
                    MessageCc = string.Join(",", lstemilCc);
                }
                else
                {
                    MessageCc = "";
                }

                //if (!string.IsNullOrEmpty(MessageTo) && MessageTo.Length > 0)
                //{
                //    var msgto = MessageTo.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                //    foreach (var itememailto in msgto)
                //    {
                //        lstemilto.Add(itememailto);
                //    }
                //}

                //if (!string.IsNullOrEmpty(MessageCc) && MessageCc.Length > 0)
                //{
                //    var msgcc = MessageCc.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                //    foreach (var itememailcc in msgcc)
                //    {
                //        lstemilCc.Add(itememailcc);
                //    }
                //}

                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (MessageID == 0)
                    {
                        // insert code


                        var entity = new SDGAppDB.POCO.UserMessage();

                        //Inbox menu showing

                        entity.SenderUserID = LoginUserID;
                        // entity.ReceiverUserID = userentity.UserID;
                        entity.FkMessageTypeID = (int)SDGUtilities.MessageTypeList.Draft;
                        entity.MessageFrom = LoginUserEmail;
                        entity.MessageTo = MessageTo;
                        entity.MessageCC = MessageCc;
                        entity.MessageSubject = MessageSubject;
                        entity.MessageBody = MessageBody;
                        entity.IsViewed = false;
                        entity.IsDeleted = false;

                        entity.CreatedDateTime = DateTime.Now;

                        db.UserMessage.Add(entity);
                        db.SaveChanges();

                        if (entity.MessageID > 0)
                        {
                            messageid = entity.MessageID;
                        }
                    }
                    else
                    {
                        var entity = db.UserMessage.Find(MessageID);

                        if (entity != null && entity.MessageID > 0)
                        {
                            entity.MessageTo = MessageTo;
                            entity.MessageCC = MessageCc;
                            entity.MessageSubject = MessageSubject;
                            entity.MessageBody = MessageBody;

                            //entity.IsActive = cvm.IsActive;

                            db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            if (entity.MessageID > 0)
                            {
                                messageid = entity.MessageID;
                            }
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - SaveDraftDetails", Ex.Message);
            }

            return messageid;
        }


        public bool EmailContactExist(string Emailid)
        {
            bool result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var userEntity = (from usr in db.User
                                      where usr.Email.Trim().ToLower() == Emailid.Trim().ToLower()
                                      select new { usr }).FirstOrDefault();


                    if (userEntity != null && userEntity.usr.UserID > 0)
                    {
                        var contactEntity = (from ct in db.UserContacts
                                             where (ct.FKSenderUserID == userEntity.usr.UserID || ct.FKReceiverUserID == userEntity.usr.UserID)
                                             && ct.IsAccepted == true
                                             select new { ct }).FirstOrDefault();
                        if (contactEntity != null)
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - EamilContactExist", Ex.Message);
            }
            return result;
        }

        public bool NameContactExist(string UsertName)
        {
            bool result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var userEntity = (from usr in db.User
                                      where (usr.FirstName.Trim().ToLower() + " " + usr.LastName.Trim().ToLower()).Contains(UsertName.Trim().ToLower())
                                      select new { usr }).FirstOrDefault();


                    if (userEntity != null && userEntity.usr.UserID > 0)
                    {
                        var contactEntity = (from ct in db.UserContacts
                                             where (ct.FKSenderUserID == userEntity.usr.UserID || ct.FKReceiverUserID == userEntity.usr.UserID)
                                             && ct.IsAccepted == true
                                             select new { ct }).FirstOrDefault();
                        if (contactEntity != null)
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - NameContactExist", Ex.Message);
            }
            return result;
        }

        #endregion

        #region [Delete messsages]

        public bool DeleteMessageByMessageID(int MessageID)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.UserMessage.Find(MessageID);
                    if (entity != null)
                    {
                        entity.FkMessageTypeID = GetIntegerValue(SDGUtilities.MessageTypeList.Bin);

                        db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - DeleteMessageByMessageID", Ex.Message);
            }
            return Result;
        }

        public bool MessagesMarkAsDelete(int[] MessagesID)
        {
            bool Result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    if (MessagesID != null && MessagesID.Length > 0)
                    {
                        foreach (var item in MessagesID)
                        {
                            var entity = db.UserMessage.Find(item);
                            if (entity != null)
                            {
                                entity.FkMessageTypeID = GetIntegerValue(SDGUtilities.MessageTypeList.Bin);

                                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();

                                Result = true;
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - MessagesMarkAsDelete", Ex.Message);
            }
            return Result;
        }

        public bool DeleteMessageTrash(int MessageID)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.UserMessage.Find(MessageID);
                    if (entity != null)
                    {

                        entity.IsDeleted = true;

                        db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - DeleteMessageTrash", Ex.Message);
            }
            return Result;
        }

        public bool MultipleMessageDeleteFromTrash(int[] MessagesID)
        {
            bool Result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    if (MessagesID != null && MessagesID.Length > 0)
                    {
                        foreach (var item in MessagesID)
                        {
                            var entity = db.UserMessage.Find(item);
                            if (entity != null)
                            {
                                entity.IsDeleted = true;

                                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();

                                Result = true;
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - MessagesMarkAsDelete", Ex.Message);
            }
            return Result;
        }

        public bool EmptyFromTrashByUserID(int UserId)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = (from um in db.UserMessage
                                  where (um.ReceiverUserID == UserId || um.SenderUserID == UserId)
                                  && um.FkMessageTypeID == (int)(SDGUtilities.MessageTypeList.Bin)
                                  select um).ToList();

                    if (entity != null && entity.Count > 0)
                    {
                        entity.ForEach(a => { a.IsDeleted = true; });
                        db.SaveChanges();

                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - EmptyFromTrashByUserID", Ex.Message);
            }
            return Result;
        }


        #endregion


        #region [Viewed Messages]

        public bool ViewedMessage(int MessageID)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.UserMessage.Find(MessageID);
                    if (entity != null)
                    {
                        if (!entity.IsViewed)
                        {
                            entity.IsViewed = true;

                            db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }


                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - ViewedMessage", Ex.Message);
            }
            return Result;
        }

        public bool MessagesMarkAsRead(int UserId)
        {
            bool Result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    var lst = db.UserMessage.Where(um => um.ReceiverUserID == UserId
                                && um.FkMessageTypeID == (int)SDGUtilities.MessageTypeList.Inbox
                                && !um.IsViewed).ToList();

                    if (lst != null && lst.Count > 0)
                    {
                        foreach (var item in lst)
                        {
                            item.IsViewed = true;

                            db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - MessagesMarkAsRead", Ex.Message);
            }
            return Result;
        }

        public bool MarkAsReadByMessageType(int UserId, int MessageTypeId)
        {
            bool Result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (MessageTypeId == (int)SDGUtilities.MessageTypeList.Inbox)
                    {
                        var lst = db.UserMessage.Where(um => um.ReceiverUserID == UserId
                                && um.FkMessageTypeID == (int)SDGUtilities.MessageTypeList.Inbox
                                && !um.IsViewed).ToList();

                        if (lst != null && lst.Count > 0)
                        {
                            foreach (var item in lst)
                            {
                                item.IsViewed = true;

                                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                            Result = true;
                        }

                    }
                    else if (MessageTypeId == (int)SDGUtilities.MessageTypeList.Bin)
                    {
                        var lst = db.UserMessage.Where(um => um.ReceiverUserID == UserId
                                && um.FkMessageTypeID == (int)SDGUtilities.MessageTypeList.Bin
                                && !um.IsViewed).ToList();

                        if (lst != null && lst.Count > 0)
                        {
                            foreach (var item in lst)
                            {
                                item.IsViewed = true;

                                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                            Result = true;
                        }

                    }


                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - MessagesMarkAsRead", Ex.Message);
            }
            return Result;
        }

        public bool MarkAsReadByMessageId(int MessageId)
        {
            bool Result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.UserMessage.Find(MessageId);

                    if (entity != null)
                    {
                        entity.IsViewed = true;

                        db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        Result = true;
                    }

                }



            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - MessagesMarkAsRead", Ex.Message);
            }
            return Result;
        }

        #endregion

        #region [Compose Messages]



        public Boolean ComposeNewMessage(UserMessageViewModel model)
        {
            Boolean result = false;

            model.lstemilto = new List<string>();
            model.lstemilCc = new List<string>();
            model.MessageReturnIDs = new List<int>();

            if (!string.IsNullOrEmpty(model.MessageTo))
            {
                model.lstemilto = model.MessageTo.Split(',').ToList();
            }

            if (!string.IsNullOrEmpty(model.MessageCc))
            {
                model.lstemilCc = model.MessageCc.Split(',').ToList();
            }

            int messagecount = 0;
            int refmessageid = 0;
            var LastMessageID = 0;
            var MainMessageID = 0;

            UserMessage userMessageDtls = new UserMessage();

            try
            {
                // sent mail

                if (result = SendUserMailMessage(model))
                {
                    using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {

                        if (model.MessageID > 0)
                        {
                            var entitymsg = db.UserMessage.Find(model.MessageID);

                            if (entitymsg != null && entitymsg.MessageID > 0)
                            {
                                MainMessageID = entitymsg.MessageID;
                                userMessageDtls = entitymsg;

                                if (model.MsgResponseTypeID == 0) // Apply Only New message 
                                {
                                    db.UserMessage.Remove(entitymsg);
                                    db.SaveChanges();
                                }

                                if (String.IsNullOrEmpty(model.ParentGUIID))
                                {
                                    model.ParentGUIID = GetStringValue(Guid.NewGuid());
                                }
                            }
                        }

                        if (model.lstemilto.Count > 0)
                        {
                            #region [  start of foreach (var itememailto in lstemilto) ]

                            

                            foreach (var itememailto in model.lstemilto)
                            {

                                var userentity = (from u in db.User
                                                  where u.Email.ToLower() == itememailto.Trim().ToLower() && !u.IsDeleted && u.IsActive
                                                  select new { u.UserID }
                                    ).FirstOrDefault();

                                if (userentity != null)
                                {
                                    var entity = new SDGAppDB.POCO.UserMessage();

                                    //Inbox menu showing

                                    entity.SenderUserID = model.LoginUserID;
                                    entity.ReceiverUserID = userentity.UserID;
                                    entity.FkMessageTypeID = (int)SDGUtilities.MessageTypeList.Inbox;
                                    entity.MessageFrom = model.LoginUserEmail;
                                    entity.MessageTo = String.Join(",", model.lstemilto);
                                    if (model.lstemilCc != null && model.lstemilCc.Count > 0)
                                    {
                                        entity.MessageCC = String.Join(",", model.lstemilCc);
                                    }
                                    entity.MessageSubject = model.MessageSubject;
                                    entity.MessageBody = model.MessageBody;
                                    entity.IsViewed = false;
                                    entity.IsDeleted = false;
                                    entity.ParentGUIID = model.ParentGUIID;

                                    entity.CreatedDateTime = DateTime.Now;
                                    if (messagecount > 0)
                                    {
                                        entity.FKMessageID = refmessageid;
                                    }

                                    if (model.MsgResponseTypeID > 0)
                                    {
                                        if (model.MsgResponseTypeID == (int)SDGUtilities.UserMessageReplyTypeList.Reply)
                                        {
                                            entity.FKReplyTypeID = (int)SDGUtilities.UserMessageReplyTypeList.Reply;
                                        }
                                        else if (model.MsgResponseTypeID == (int)SDGUtilities.UserMessageReplyTypeList.ReplyToAll)
                                        {
                                            entity.FKReplyTypeID = (int)SDGUtilities.UserMessageReplyTypeList.ReplyToAll;
                                        }
                                        else if (model.MsgResponseTypeID == (int)SDGUtilities.UserMessageReplyTypeList.Forward)
                                        {
                                            entity.FKReplyTypeID = (int)SDGUtilities.UserMessageReplyTypeList.Forward;
                                        }


                                        //userMessageDtls = db.UserMessage.Find(MainMessageID);

                                        if (userMessageDtls != null && userMessageDtls.MessageID > 0)
                                        {
                                            if (userMessageDtls.FkMessageTypeID == (int)SDGUtilities.MessageTypeList.Sent)
                                            {
                                                entity.ReplyMessageID = userMessageDtls.FKMessageID;
                                            }
                                            else
                                            {
                                                entity.ReplyMessageID = MainMessageID; //Apply for New Message Only
                                            }
                                        }
                                    }

                                    db.UserMessage.Add(entity);
                                    db.SaveChanges();

                                    if (entity.MessageID > 0)
                                    {
                                        LastMessageID = entity.MessageID;

                                        model.MessageReturnIDs.Add(LastMessageID);
                                    }


                                    // save file attachments for inbox
                                    SaveMessageAttachment(model, LastMessageID);

                                    if (messagecount == 0)
                                    {
                                        refmessageid = entity.MessageID;
                                    }
                                    if (messagecount > 0)
                                    {
                                        continue;
                                    }

                                    //Sent menu showing data 
                                    entity.SenderUserID = model.LoginUserID;
                                    entity.ReceiverUserID = userentity.UserID;
                                    entity.FkMessageTypeID = (int)SDGUtilities.MessageTypeList.Sent;
                                    entity.MessageFrom = model.LoginUserEmail;
                                    entity.MessageTo = String.Join(",", model.lstemilto);
                                    if (model.lstemilCc != null && model.lstemilCc.Count > 0)
                                    {
                                        entity.MessageCC = String.Join(",", model.lstemilCc);
                                    }
                                    entity.MessageSubject = model.MessageSubject;
                                    entity.MessageBody = model.MessageBody;
                                    entity.IsViewed = true;
                                    entity.IsDeleted = false;
                                    entity.ParentGUIID = model.ParentGUIID;
                                    entity.CreatedDateTime = DateTime.Now;

                                    entity.FKMessageID = refmessageid;

                                    if (model.MsgResponseTypeID > 0)
                                    {
                                        if (model.MsgResponseTypeID == (int)SDGUtilities.UserMessageReplyTypeList.Reply)
                                        {
                                            entity.FKReplyTypeID = (int)SDGUtilities.UserMessageReplyTypeList.Reply;
                                        }
                                        else if (model.MsgResponseTypeID == (int)SDGUtilities.UserMessageReplyTypeList.ReplyToAll)
                                        {
                                            entity.FKReplyTypeID = (int)SDGUtilities.UserMessageReplyTypeList.ReplyToAll;
                                        }
                                        else if (model.MsgResponseTypeID == (int)SDGUtilities.UserMessageReplyTypeList.Forward)
                                        {
                                            entity.FKReplyTypeID = (int)SDGUtilities.UserMessageReplyTypeList.Forward;
                                        }


                                        if (userMessageDtls != null && userMessageDtls.MessageID > 0)
                                        {
                                            if (userMessageDtls.FkMessageTypeID == (int)SDGUtilities.MessageTypeList.Sent)
                                            {
                                                entity.ReplyMessageID = MainMessageID;
                                            }
                                            else
                                            {
                                                //fatch send mail by mainmessage id
                                                var sendboxmailid = (from um in db.UserMessage
                                                                     where um.FKMessageID == MainMessageID
                                                                     select um.MessageID).FirstOrDefault();

                                                entity.ReplyMessageID = sendboxmailid;
                                            }
                                        }

                                    }


                                    db.UserMessage.Add(entity);
                                    db.SaveChanges();

                                    if (entity.MessageID > 0)
                                    {
                                        LastMessageID = entity.MessageID;
                                        model.MessageReturnIDs.Add(LastMessageID);
                                    }

                                    // save file attachments for sent
                                    SaveMessageAttachment(model, LastMessageID);


                                    result = true;
                                    messagecount++;


                                }




                            }  // end of foreach (var itememailto in lstemilto)

                            #endregion [ :: end of foreach (var itememailto in lstemilto) ]

                        }      // end of if (lstemilto.Count > 0)


                        // for mail cc

                        if (model.lstemilCc.Count > 0 && model.lstemilto.Count > 0)
                        {
                            #region [  start of foreach (var itememailcc in lstemilCc) ]

                            foreach (var itememailcc in model.lstemilCc)
                            {

                                var userentity = (from u in db.User
                                                  where u.Email.ToLower() == itememailcc.Trim().ToLower() && !u.IsDeleted && u.IsActive
                                                  select new { u.UserID }
                                    ).FirstOrDefault();

                                if (userentity != null)
                                {
                                    var entity = new SDGAppDB.POCO.UserMessage();

                                    //inbox  entry

                                    entity.SenderUserID = model.LoginUserID;
                                    entity.ReceiverUserID = userentity.UserID;
                                    entity.FkMessageTypeID = (int)SDGUtilities.MessageTypeList.Inbox;
                                    entity.MessageFrom = model.LoginUserEmail;
                                    entity.MessageTo = String.Join(",", model.lstemilto);
                                    if (model.lstemilCc != null && model.lstemilCc.Count > 0)
                                    {
                                        entity.MessageCC = String.Join(",", model.lstemilCc);
                                    }
                                    entity.MessageSubject = model.MessageSubject;
                                    entity.MessageBody = model.MessageBody;
                                    entity.IsViewed = false;
                                    entity.IsDeleted = false;
                                    entity.ParentGUIID = model.ParentGUIID;

                                    entity.CreatedDateTime = DateTime.Now;

                                    if (messagecount > 0)
                                    {
                                        entity.FKMessageID = refmessageid;

                                        if (model.MsgResponseTypeID > 0)
                                        {
                                            if (model.MsgResponseTypeID == (int)SDGUtilities.UserMessageReplyTypeList.Reply)
                                            {
                                                entity.FKReplyTypeID = (int)SDGUtilities.UserMessageReplyTypeList.Reply;
                                            }
                                            else if (model.MsgResponseTypeID == (int)SDGUtilities.UserMessageReplyTypeList.ReplyToAll)
                                            {
                                                entity.FKReplyTypeID = (int)SDGUtilities.UserMessageReplyTypeList.ReplyToAll;
                                            }
                                            else if (model.MsgResponseTypeID == (int)SDGUtilities.UserMessageReplyTypeList.Forward)
                                            {
                                                entity.FKReplyTypeID = (int)SDGUtilities.UserMessageReplyTypeList.Forward;
                                            }

                                            entity.ReplyMessageID = MainMessageID;
                                        }

                                    }

                                    db.UserMessage.Add(entity);
                                    db.SaveChanges();

                                    if (entity.MessageID > 0)
                                    {
                                        LastMessageID = entity.MessageID;
                                    }

                                    // save file attachments
                                    SaveMessageAttachment(model, LastMessageID);

                                    result = true;

                                }
                            }

                            #endregion [  end of foreach (var itememailcc in lstemilCc) ]

                        }  // end of if (lstemilCc.Count > 0)
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - ComposeNewMessage", Ex.Message);
            }

            return result;
        }



        public bool SaveMessageAttachment(UserMessageViewModel model, int MessageID)
        {
            bool result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    if (!string.IsNullOrEmpty(model.UserFile) && !string.IsNullOrEmpty(model.RequestType)
                        && model.RequestType.Trim().ToLower().Equals("api"))
                    {
                        ApiSaveFilenameInDb(MessageID, model.UserFile);
                    }
                    else
                    {
                        if (model.FileAttachments[0] != null)
                        {
                            foreach (HttpPostedFileBase file in model.FileAttachments)
                            {
                                if (file.ContentLength == 0)
                                    continue;

                                var extension = Path.GetExtension(file.FileName);
                                var entity = new SDGAppDB.POCO.MessageAttachment();
                                entity.FKMessageID = MessageID;
                                entity.FileName = file.FileName;

                                db.MessageAttachment.Add(entity);
                                db.SaveChanges();
                            }
                        }
                    }


                    result = true;
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - SaveMessageAttachment", Ex.Message);
            }

            return result;
        }

        public void ApiSaveFilenameInDb(int messageid, string filename)
        {
            using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
            {
                var entity = new SDGAppDB.POCO.MessageAttachment();
                entity.FKMessageID = messageid;
                entity.FileName = filename;

                db.MessageAttachment.Add(entity);
                db.SaveChanges();
            }

        }

        public string GetUserNameByUserID(int UserID)
        {
            string strname = string.Empty;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    strname = (from usr in db.User
                               where usr.UserID == UserID && !usr.IsDeleted
                               select usr.FirstName + " " + usr.LastName).FirstOrDefault();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - GetUserNameByUserID", Ex.Message);
            }

            return strname;
        }
        public UserMessageViewModel GetMessageDetailById(int MessageID)
        {
            UserMessageViewModel model = new UserMessageViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    UserMessage entity = new UserMessage();

                    var entitychklastmg = (from um in db.UserMessage
                                           where um.ReplyMessageID == MessageID
                                           select um).FirstOrDefault();

                    if (entitychklastmg != null)
                    {
                        entity = entitychklastmg;
                    }
                    else
                    {
                        entity = (from msg in db.UserMessage
                                  where msg.MessageID == MessageID
                                  select msg).FirstOrDefault();
                    }


                    if (entity != null)
                    {
                        model.MessageID = entity.MessageID;
                        model.FkMessageTypeID = entity.FkMessageTypeID;
                        model.MessageFrom = entity.MessageFrom;
                        model.MessageTo = entity.MessageTo;
                        model.MessageCc = entity.MessageCC;
                        model.MessageSubject = entity.MessageSubject;
                        model.MessageBody = entity.MessageBody;
                        model.CreatedDateTime = entity.CreatedDateTime;
                        model.MessageTypeName = Enum.GetName(typeof(SDGUtilities.MessageTypeList), entity.FkMessageTypeID);
                        model.IsViewed = entity.IsViewed;
                        model.FKMessageID = entity.FKMessageID;
                        model.AttachmentFiles = GetFiles(entity.MessageID);

                        model.ReplyMessageID = entity.ReplyMessageID;
                        model.ParentGUIID = entity.ParentGUIID;

                        model.SenderUserName = GetUserNamesByEmaillist(model.MessageFrom);
                        model.ReceiverUserName = GetUserNamesByEmaillist(model.MessageTo);
                        model.UserNameCc = GetUserNamesByEmaillist(model.MessageCc);



                        model.LoginUserID = entity.SenderUserID;

                        if (model.ReplyMessageID != null && model.FkMessageTypeID > 0)
                        {
                            model.MessageTree = FetchMessageReplyMessageID(model.ReplyMessageID, MessageID);
                        }
                        else if (model.ReplyMessageID == null && model.FKMessageID > 0)
                        {
                            model.MessageTree = FetchReplybyMessageID(model.FKMessageID);
                        }
                        else
                        {
                            model.MessageTree = FetchReplybyMessageID(model.MessageID);
                        }

                        // remove duplicate message by id
                        if (model.MessageTree != null && model.MessageTree.Count > 0)
                        {
                            var itemToRemove = model.MessageTree.Single(r => r.MessageID == entity.MessageID);
                            if (itemToRemove != null)
                            {
                                model.MessageTree.Remove(itemToRemove);
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - GetMessageDetailById", Ex.Message);
            }
            return model;
        }


        public UserMessageViewModel GetEditMessageDetailById(int MessageID)
        {
            UserMessageViewModel model = new UserMessageViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = (from msg in db.UserMessage
                                  where msg.MessageID == MessageID
                                  select msg).FirstOrDefault();
                    if (entity != null)
                    {
                        model.MessageID = entity.MessageID;
                        model.FkMessageTypeID = entity.FkMessageTypeID;
                        model.MessageFrom = entity.MessageFrom;
                        model.MessageTo = GetUserNamesByEmaillist(entity.MessageTo);
                        model.MessageCc = GetUserNamesByEmaillist(entity.MessageCC);
                        model.MessageSubject = entity.MessageSubject;
                        model.MessageBody = entity.MessageBody;
                        model.CreatedDateTime = entity.CreatedDateTime;
                        model.MessageTypeName = Enum.GetName(typeof(SDGUtilities.MessageTypeList), entity.FkMessageTypeID);
                        model.UserEmailTo = entity.MessageTo;
                        model.UserEmailCc = entity.MessageCC;

                        model.AttachmentFiles = GetFiles(entity.MessageID);
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - GetEditMessageDetailById", Ex.Message);
            }
            return model;
        }

        public bool SendUserMailMessage(UserMessageViewModel model)
        {
            bool result = false;
            string Body = string.Empty;

            MailHelper MH = new MailHelper();
            MailService MS = new MailService();

            try
            {
                // CHECK EMAIL ID EXISTS IN CONTACTS

                foreach (var itemto in model.lstemilto)
                {
                    if (itemto.Contains("@"))
                    {
                        result = EmailContactExist(itemto);
                    }
                }

                if (result)
                {
                    foreach (var itemcc in model.lstemilCc)
                    {
                        if (itemcc.Contains("@"))
                        {
                            result = EmailContactExist(itemcc);
                        }
                    }
                }

                if (result)
                {
                    UserViewModel loggedinUserModel = GetUserDetailByUserID(model.LoginUserID);

                    var SenderName = loggedinUserModel.FirstName + " " + loggedinUserModel.LastName;

                    Dictionary<string, string> objDict = new Dictionary<string, string>();
                    objDict.Add("SenderName", SenderName);
                    objDict.Add("ToYear", DateTime.Now.Year.ToString());


                    Body = MH.ReadHtmlFile("MessageNotification.html", objDict);
                    result = MS.SendMailMEssageBulk(model, Body);
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - SendUserMailMessage", Ex.Message);
            }

            return result;
        }

        public List<FileViewModel> GetFiles(int MessageID)
        {
            List<FileViewModel> lst = new List<FileViewModel>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    lst = (from f in db.MessageAttachment
                           where f.FKMessageID == MessageID
                           select new FileViewModel
                           {
                               FileID = f.ID,
                               FileName = f.FileName

                           }).ToList();

                    if (lst != null && lst.Count > 0)
                    {
                        for (int i = 0; i < lst.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(lst[i].FileName))
                            {
                                FileInfo fi2 = new FileInfo(Path.Combine(HttpContext.Current.Server.MapPath("~/Content/EmailAttachments"), lst[i].FileName));
                                if (!fi2.Exists)
                                {
                                    lst.RemoveAt(i);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - GetFiles", Ex.Message);
            }
            return lst;
        }

        // DOWNLOAD ALL ATTACHMENTS
        public List<FileViewModel> GetAllFilesByIDs(string FileIDs)
        {
            List<FileViewModel> lst = new List<FileViewModel>();
            FileViewModel model = new FileViewModel();
            List<string> lststrfiles = new List<string>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    List<int> File_IDs = FileIDs.Split(',').Select(int.Parse).ToList();

                    var entitylst = db.MessageAttachment.Where(x => File_IDs.Contains(x.ID)).ToList();

                    if (entitylst != null)
                    {
                        foreach (var fileitem in entitylst)
                        {
                            lststrfiles.Add(HttpContext.Current.Server.MapPath("~/Content/EmailAttachments/" + fileitem.FileName));
                            lst.Add(new FileViewModel
                            {
                                FileID = fileitem.ID,
                                FileName = fileitem.FileName,
                            });
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - GetAllFilesByIDs", Ex.Message);
            }
            return lst;
        }

        #endregion

        #region [Search List]

        public List<MessageSearchViewModel> GetMessageSearchList(int UserID, string prefix)
        {
            List<MessageSearchViewModel> lstmsgto = new List<MessageSearchViewModel>();


            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    lstmsgto = (from contact in db.UserContacts
                                join usr in db.User on contact.FKReceiverUserID equals usr.UserID
                                where contact.IsDeleted == false && contact.IsAccepted == true && !contact.IsRejected
                                && !usr.IsDeleted && usr.IsActive
                                && usr.UserID != UserID
                                && (contact.FKSenderUserID == UserID || contact.FKReceiverUserID == UserID)
                                && usr.Email.ToLower().Contains(prefix)

                                select new MessageSearchViewModel
                                {
                                    Name = usr.FirstName + " " + usr.LastName,
                                    Picture = (!string.IsNullOrEmpty(usr.Picture) ? "/Content/images/" + usr.Picture : "/Content/Latest/images/no-image-profile-male-sm.png"),
                                    Email = usr.Email

                                }).ToList();

                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - GetMessageToSearchList", Ex.Message);
            }
            return lstmsgto;


        }


        public List<UserMessageViewModel> SearchMailByValue(String SearchValue, int UserId, int messagetypeid)
        {
            List<UserMessageViewModel> list = new List<UserMessageViewModel>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (SearchValue.Length > 0 && UserId > 0)
                    {
                        if (messagetypeid == (int)SDGUtilities.MessageTypeList.Inbox)
                        {
                            list = (from msg in db.UserMessage
                                    where
                                    (msg.MessageTo.Trim().ToLower().Contains(SearchValue.Trim().ToLower())
                                    || msg.MessageFrom.Trim().ToLower().Contains(SearchValue.Trim().ToLower())
                                    || msg.MessageSubject.Trim().ToLower().Contains(SearchValue.Trim().ToLower())
                                    || msg.MessageBody.Trim().ToLower().Contains(SearchValue.Trim().ToLower()))
                                    && msg.FkMessageTypeID == (int)SDGUtilities.MessageTypeList.Inbox
                                    && msg.ReceiverUserID == UserId
                                    select new UserMessageViewModel
                                    {
                                        MessageID = msg.MessageID,
                                        MessageFrom = msg.MessageFrom,
                                        MessageTo = msg.MessageTo,
                                        MessageSubject = msg.MessageSubject,
                                        MessageBody = msg.MessageBody,
                                        IsViewed = msg.IsViewed,
                                        IsDeleted = msg.IsDeleted,
                                        PageTo = 10,
                                        PageFrom = 1,
                                        CreatedDateTime = msg.CreatedDateTime

                                    }).ToList();
                        }
                        else if (messagetypeid == (int)SDGUtilities.MessageTypeList.Sent)
                        {
                            list = (from msg in db.UserMessage
                                    where
                                    (msg.MessageTo.Trim().ToLower().Contains(SearchValue.Trim().ToLower())
                                    || msg.MessageFrom.Trim().ToLower().Contains(SearchValue.Trim().ToLower())
                                    || msg.MessageSubject.Trim().ToLower().Contains(SearchValue.Trim().ToLower())
                                    || msg.MessageBody.Trim().ToLower().Contains(SearchValue.Trim().ToLower()))
                                    && msg.FkMessageTypeID == (int)SDGUtilities.MessageTypeList.Sent
                                    && msg.SenderUserID == UserId
                                    select new UserMessageViewModel
                                    {
                                        MessageID = msg.MessageID,
                                        MessageFrom = msg.MessageFrom,
                                        MessageTo = msg.MessageTo,
                                        MessageSubject = msg.MessageSubject,
                                        MessageBody = msg.MessageBody,
                                        IsViewed = msg.IsViewed,
                                        IsDeleted = msg.IsDeleted,
                                        PageTo = 10,
                                        PageFrom = 1,
                                        CreatedDateTime = msg.CreatedDateTime

                                    }).ToList();
                        }
                        else if (messagetypeid == (int)SDGUtilities.MessageTypeList.Bin)
                        {
                            list = (from msg in db.UserMessage
                                    where
                                    (msg.MessageTo.Trim().ToLower().Contains(SearchValue.Trim().ToLower())
                                    || msg.MessageFrom.Trim().ToLower().Contains(SearchValue.Trim().ToLower())
                                    || msg.MessageSubject.Trim().ToLower().Contains(SearchValue.Trim().ToLower())
                                    || msg.MessageBody.Trim().ToLower().Contains(SearchValue.Trim().ToLower()))
                                    && msg.FkMessageTypeID == (int)SDGUtilities.MessageTypeList.Bin
                                    && (msg.ReceiverUserID == UserId || msg.SenderUserID == UserId)
                                    select new UserMessageViewModel
                                    {
                                        MessageID = msg.MessageID,
                                        MessageFrom = msg.MessageFrom,
                                        MessageTo = msg.MessageTo,
                                        MessageSubject = msg.MessageSubject,
                                        MessageBody = msg.MessageBody,
                                        IsViewed = msg.IsViewed,
                                        IsDeleted = msg.IsDeleted,
                                        PageTo = 10,
                                        PageFrom = 1,
                                        CreatedDateTime = msg.CreatedDateTime

                                    }).ToList();

                        }

                        if (list != null && list.Count > 0)
                        {
                            foreach (var item in list)
                            {
                                item.MessageBody = StripHTML(item.MessageBody);
                                item.TotalRecords = list.Count;
                            }
                        }


                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - SearchMailByValue", Ex.Message);
            }

            return list;

        }
        #endregion

        #region [Message List]

        public List<UserMessageViewModel> FetchMessageList_1(int UserId, int pagefrom, int pageto, int MessageTypeid, int[] IDs = null)
        {
            List<UserMessageViewModel> lstchunk = new List<UserMessageViewModel>();
            List<UserMessageViewModel> lst = new List<UserMessageViewModel>();

            List<UserMessageViewModel> plst = new List<UserMessageViewModel>();

            List<UserMessageViewModel> chkLst = new List<UserMessageViewModel>();

            List<UserMessageViewModel> finallst = new List<UserMessageViewModel>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (MessageTypeid == (int)SDGUtilities.MessageTypeList.Inbox)
                    {

                        var entityParentList = (from msg in db.UserMessage
                                                join usr in db.User on msg.SenderUserID equals usr.UserID
                                                where msg.ReceiverUserID == UserId && msg.FkMessageTypeID == MessageTypeid
                                                && !msg.IsDeleted
                                                select new UserMessageViewModel
                                                {
                                                    MessageID = msg.MessageID,
                                                    MessageFrom = msg.MessageFrom,
                                                    MessageTo = msg.MessageTo,
                                                    MessageSubject = msg.MessageSubject,
                                                    MessageBody = msg.MessageBody,
                                                    IsViewed = msg.IsViewed,
                                                    IsDeleted = msg.IsDeleted,
                                                    PageTo = pageto,
                                                    PageFrom = pagefrom,
                                                    CreatedDateTime = msg.CreatedDateTime,
                                                    SenderUserID = msg.SenderUserID,
                                                    SenderUserName = usr.FirstName + " " + usr.LastName,
                                                    ReplyMessageID = msg.ReplyMessageID
                                                }).ToList();


                        foreach (var item in entityParentList)
                        {
                            var FatchAnyReplyEntity = (from msg in db.UserMessage
                                                       join usr in db.User on msg.SenderUserID equals usr.UserID
                                                       where msg.ReplyMessageID == item.MessageID
                                                       && !msg.IsDeleted
                                                       select new UserMessageViewModel
                                                       {
                                                           MessageID = msg.MessageID,
                                                           MessageFrom = msg.MessageFrom,
                                                           MessageTo = msg.MessageTo,
                                                           MessageSubject = msg.MessageSubject,
                                                           MessageBody = msg.MessageBody,
                                                           IsViewed = msg.IsViewed,
                                                           IsDeleted = msg.IsDeleted,
                                                           PageTo = pageto,
                                                           PageFrom = pagefrom,
                                                           CreatedDateTime = msg.CreatedDateTime,
                                                           SenderUserID = msg.SenderUserID,
                                                           SenderUserName = usr.FirstName + " " + usr.LastName,
                                                           ReplyMessageID = msg.ReplyMessageID
                                                       }).FirstOrDefault();

                            if (FatchAnyReplyEntity != null)
                            {
                                plst.Add(FatchAnyReplyEntity);

                                if (FatchAnyReplyEntity.MessageID > 0)
                                {
                                    var FatchMainMessageEntity = (from msg in db.UserMessage
                                                                  join usr in db.User on msg.SenderUserID equals usr.UserID
                                                                  where msg.ReplyMessageID == FatchAnyReplyEntity.MessageID
                                                                  && !msg.IsDeleted
                                                                  select new UserMessageViewModel
                                                                  {
                                                                      MessageID = msg.MessageID,
                                                                      MessageFrom = msg.MessageFrom,
                                                                      MessageTo = msg.MessageTo,
                                                                      MessageSubject = msg.MessageSubject,
                                                                      MessageBody = msg.MessageBody,
                                                                      IsViewed = msg.IsViewed,
                                                                      IsDeleted = msg.IsDeleted,
                                                                      PageTo = pageto,
                                                                      PageFrom = pagefrom,
                                                                      CreatedDateTime = msg.CreatedDateTime,
                                                                      SenderUserID = msg.SenderUserID,
                                                                      SenderUserName = usr.FirstName + " " + usr.LastName,
                                                                      ReplyMessageID = msg.ReplyMessageID
                                                                  }).FirstOrDefault();

                                    if (FatchMainMessageEntity != null)
                                    {
                                        plst.Add(FatchMainMessageEntity);
                                    }
                                }
                            }

                        }

                        plst.AddRange(entityParentList);

                        foreach (var itemparent in plst)
                        {
                            if (itemparent.ReplyMessageID == null)
                            {
                                var entityFindAnyChield = plst.Where(p => p.ReplyMessageID == itemparent.MessageID).FirstOrDefault();
                                if (entityFindAnyChield == null)
                                {
                                    finallst.Add(itemparent);
                                }
                            }
                            else
                            {
                                var MessageID = itemparent.ReplyMessageID;
                                var LastMessageID = 0;

                                for (int i = 0; i < 1; i++)
                                {
                                    var entity = plst.Where(p => p.ReplyMessageID == MessageID).FirstOrDefault();

                                    if (entity != null)
                                    {
                                        LastMessageID = entity.MessageID;

                                        if (entity.ReplyMessageID != null)
                                        {
                                            var entityremove = plst.Where(p => p.MessageID == MessageID).FirstOrDefault();
                                            finallst.Remove(entityremove);
                                            MessageID = entity.MessageID;

                                            i--;
                                        }
                                    }
                                    else
                                    {
                                        var entityChild = plst.Where(p => p.MessageID == LastMessageID).FirstOrDefault();
                                        if (entityChild != null)
                                        {
                                            finallst.Add(entityChild);
                                        }
                                    }
                                }
                            }
                        }
                        if (finallst != null && finallst.Count > 0)
                        {
                            finallst = finallst.Distinct().ToList();
                            lst = finallst;
                        }
                    }

                    else if (MessageTypeid == (int)SDGUtilities.MessageTypeList.Sent)
                    {
                        var entityParentList = (from msg in db.UserMessage
                                                join usr in db.User on msg.ReceiverUserID equals usr.UserID
                                                where msg.SenderUserID == UserId && msg.FkMessageTypeID == MessageTypeid
                                                && !msg.IsDeleted

                                                select new UserMessageViewModel
                                                {
                                                    MessageID = msg.MessageID,
                                                    MessageFrom = msg.MessageFrom,
                                                    MessageTo = msg.MessageTo,
                                                    MessageSubject = msg.MessageSubject,
                                                    MessageBody = msg.MessageBody,
                                                    IsViewed = msg.IsViewed,
                                                    IsDeleted = msg.IsDeleted,
                                                    PageTo = pageto,
                                                    PageFrom = pagefrom,
                                                    CreatedDateTime = msg.CreatedDateTime,
                                                    ReceiverUserID = msg.ReceiverUserID,
                                                    ReceiverUserName = usr.FirstName + " " + usr.LastName,
                                                    ReplyMessageID = msg.ReplyMessageID

                                                }).ToList();



                        foreach (var item in entityParentList)
                        {

                            var FatchAnyReplyEntity = (from msg in db.UserMessage
                                                       join usr in db.User on msg.SenderUserID equals usr.UserID
                                                       where msg.ReplyMessageID == item.MessageID
                                                       && !msg.IsDeleted
                                                       select new UserMessageViewModel
                                                       {
                                                           MessageID = msg.MessageID,
                                                           MessageFrom = msg.MessageFrom,
                                                           MessageTo = msg.MessageTo,
                                                           MessageSubject = msg.MessageSubject,
                                                           MessageBody = msg.MessageBody,
                                                           IsViewed = msg.IsViewed,
                                                           IsDeleted = msg.IsDeleted,
                                                           PageTo = pageto,
                                                           PageFrom = pagefrom,
                                                           CreatedDateTime = msg.CreatedDateTime,
                                                           SenderUserID = msg.SenderUserID,
                                                           SenderUserName = usr.FirstName + " " + usr.LastName,
                                                           ReplyMessageID = msg.ReplyMessageID
                                                       }).FirstOrDefault();

                            if (FatchAnyReplyEntity != null)
                            {
                                plst.Add(FatchAnyReplyEntity);


                                if (FatchAnyReplyEntity.MessageID > 0)
                                {
                                    var FatchMainMessageEntity = (from msg in db.UserMessage
                                                                  join usr in db.User on msg.SenderUserID equals usr.UserID
                                                                  where msg.ReplyMessageID == FatchAnyReplyEntity.MessageID
                                                                  && !msg.IsDeleted
                                                                  select new UserMessageViewModel
                                                                  {
                                                                      MessageID = msg.MessageID,
                                                                      MessageFrom = msg.MessageFrom,
                                                                      MessageTo = msg.MessageTo,
                                                                      MessageSubject = msg.MessageSubject,
                                                                      MessageBody = msg.MessageBody,
                                                                      IsViewed = msg.IsViewed,
                                                                      IsDeleted = msg.IsDeleted,
                                                                      PageTo = pageto,
                                                                      PageFrom = pagefrom,
                                                                      CreatedDateTime = msg.CreatedDateTime,
                                                                      SenderUserID = msg.SenderUserID,
                                                                      SenderUserName = usr.FirstName + " " + usr.LastName,
                                                                      ReplyMessageID = msg.ReplyMessageID
                                                                  }).FirstOrDefault();

                                    if (FatchMainMessageEntity != null)
                                    {
                                        plst.Add(FatchMainMessageEntity);
                                    }
                                }
                            }

                        }

                        plst.AddRange(entityParentList);

                        foreach (var itemparent in plst)
                        {
                            if (itemparent.ReplyMessageID == null)
                            {
                                var entityFindAnyChield = plst.Where(p => p.ReplyMessageID == itemparent.MessageID).FirstOrDefault();
                                if (entityFindAnyChield == null)
                                {
                                    finallst.Add(itemparent);
                                }
                            }
                            else
                            {
                                var MessageID = itemparent.ReplyMessageID;
                                var LastMessageID = 0;

                                for (int i = 0; i < 1; i++)
                                {
                                    var entity = plst.Where(p => p.ReplyMessageID == MessageID).FirstOrDefault();

                                    if (entity != null)
                                    {
                                        LastMessageID = entity.MessageID;

                                        if (entity.ReplyMessageID != null)
                                        {
                                            var entityremove = plst.Where(p => p.MessageID == MessageID).FirstOrDefault();
                                            finallst.Remove(entityremove);
                                            MessageID = entity.MessageID;

                                            i--;
                                        }
                                    }
                                    else
                                    {
                                        var entityChild = plst.Where(p => p.MessageID == LastMessageID).FirstOrDefault();
                                        if (entityChild != null)
                                        {
                                            finallst.Add(entityChild);
                                        }
                                    }
                                }
                            }
                        }
                        if (finallst != null && finallst.Count > 0)
                        {
                            finallst = finallst.Distinct().ToList();
                            lst = finallst;
                        }

                    }//end If

                    else if (MessageTypeid == (int)SDGUtilities.MessageTypeList.Bin)
                    {
                        lst = (from msg in db.UserMessage
                               join usr in db.User on msg.ReceiverUserID equals usr.UserID
                               join usr2 in db.User on msg.SenderUserID equals usr2.UserID
                               where (msg.ReceiverUserID == UserId || msg.SenderUserID == UserId)
                               && msg.IsDeleted == false
                               && msg.FkMessageTypeID == MessageTypeid

                               select new UserMessageViewModel
                               {
                                   MessageID = msg.MessageID,
                                   MessageFrom = msg.MessageFrom,
                                   MessageTo = msg.MessageTo,
                                   MessageSubject = msg.MessageSubject,
                                   MessageBody = msg.MessageBody,
                                   IsViewed = msg.IsViewed,
                                   IsDeleted = msg.IsDeleted,
                                   PageTo = pageto,
                                   PageFrom = pagefrom,
                                   CreatedDateTime = msg.CreatedDateTime,
                                   ReceiverUserID = msg.ReceiverUserID,
                                   ReceiverUserName = usr.FirstName + " " + usr.LastName,
                                   SenderUserID = msg.SenderUserID,
                                   SenderUserName = usr2.FirstName + " " + usr2.LastName

                               }).ToList();

                    }

                    else if (MessageTypeid == (int)SDGUtilities.MessageTypeList.Draft)
                    {
                        lst = (from msg in db.UserMessage
                                   //join usr in db.User on msg.ReceiverUserID equals usr.UserID
                               where msg.SenderUserID == UserId && msg.FkMessageTypeID == MessageTypeid
                               && !msg.IsDeleted
                               select new UserMessageViewModel
                               {
                                   MessageID = msg.MessageID,
                                   MessageFrom = msg.MessageFrom,
                                   MessageTo = msg.MessageTo,
                                   MessageSubject = msg.MessageSubject,
                                   MessageBody = msg.MessageBody,
                                   IsViewed = msg.IsViewed,
                                   IsDeleted = msg.IsDeleted,
                                   PageTo = pageto,
                                   PageFrom = pagefrom,
                                   CreatedDateTime = msg.CreatedDateTime,
                                   ReceiverUserName = "Draft"

                               }).ToList();

                    }


                    if (lst != null && lst.Count > 0)
                    {
                        if (IDs != null && IDs.Length > 0)
                        {
                            foreach (var item in IDs)
                            {
                                lst.Remove(lst.Single(s => s.MessageID == item)); // Remove IDs from List
                            }

                        }

                        var skiprecordcount = (pagefrom - 1);
                        var takerecordcount = (pageto - pagefrom) + 1;

                        lstchunk = lst.OrderByDescending(q => q.MessageID).Skip(skiprecordcount).Take(takerecordcount).ToList();

                        foreach (var item in lstchunk)
                        {
                            if (!String.IsNullOrEmpty(item.MessageBody))
                            {
                                item.MessageBody = StripHTML(item.MessageBody);
                            }
                            item.TotalRecords = lst.Count;
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - FetchMessageList", Ex.Message);
            }
            return lstchunk;
        }

        public List<UserMessageViewModel> FetchMessageList_2(int UserId, int pagefrom, int pageto, int MessageTypeid, int[] IDs = null)
        {
            List<UserMessageViewModel> lstchunk = new List<UserMessageViewModel>();
            List<UserMessageViewModel> lst = new List<UserMessageViewModel>();

            List<UserMessageViewModel> finallst = new List<UserMessageViewModel>();

            List<UserMessage> userMessagesList = new List<UserMessage>();

            int ReplyMessageCount = 0;
            List<ReplyConversationCount> lstCount = new List<ReplyConversationCount>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (MessageTypeid == (int)SDGUtilities.MessageTypeList.Inbox)
                    {

                        var entityParentList = (from msg in db.UserMessage
                                                where msg.ReceiverUserID == UserId && msg.FkMessageTypeID == MessageTypeid
                                                && !msg.IsDeleted
                                                select msg).ToList();

                        List<int> lstCheckedMessageId = new List<int>();
                        foreach (var item in entityParentList)
                        {
                            var messageid = item.MessageID;
                            var LastEntity = item;


                            var isPresent = lstCheckedMessageId.Where(x => x == item.MessageID).FirstOrDefault();

                            if (messageid != isPresent)
                            {
                                ReplyMessageCount = 0;
                                for (int i = 0; i < 1; i++)
                                {

                                    var entityReplyMessage = (from msg in db.UserMessage
                                                              where msg.ReplyMessageID == messageid
                                                              select msg).FirstOrDefault();

                                    if (entityReplyMessage != null)
                                    {
                                        if (entityReplyMessage.ReplyMessageID > 0)
                                        {
                                            i--;

                                            ReplyMessageCount++;

                                            messageid = entityReplyMessage.MessageID;

                                            LastEntity = entityReplyMessage;
                                            lstCheckedMessageId.Add(entityReplyMessage.MessageID);
                                        }
                                        else
                                        {
                                            if (entityReplyMessage != null)
                                            {
                                                ReplyConversationCount replyConversationCount = new ReplyConversationCount();
                                                replyConversationCount.MessageID = entityReplyMessage.MessageID;
                                                replyConversationCount.Count = ReplyMessageCount;
                                                lstCount.Add(replyConversationCount);

                                                userMessagesList.Add(entityReplyMessage);
                                                lstCheckedMessageId.Add(entityReplyMessage.MessageID);
                                            }
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (LastEntity != null)
                                        {
                                            ReplyConversationCount replyConversationCount = new ReplyConversationCount();
                                            replyConversationCount.MessageID = LastEntity.MessageID;
                                            replyConversationCount.Count = ReplyMessageCount;
                                            lstCount.Add(replyConversationCount);

                                            userMessagesList.Add(LastEntity);
                                            lstCheckedMessageId.Add(LastEntity.MessageID);
                                        }
                                        break;
                                    }
                                }
                            }
                        }//end foreach

                        if (userMessagesList != null && userMessagesList.Count > 0)
                        {
                            lst = (from msg in userMessagesList
                                   select new UserMessageViewModel
                                   {
                                       MessageID = msg.MessageID,
                                       MessageFrom = msg.MessageFrom,
                                       MessageTo = msg.MessageTo,
                                       MessageSubject = msg.MessageSubject,
                                       MessageBody = msg.MessageBody,
                                       IsViewed = msg.IsViewed,
                                       IsDeleted = msg.IsDeleted,
                                       PageTo = pageto,
                                       PageFrom = pagefrom,
                                       CreatedDateTime = msg.CreatedDateTime,
                                       SenderUserID = msg.SenderUserID,
                                       SenderUserName = (from u in db.User
                                                         where u.UserID == msg.SenderUserID
                                                         select u.FirstName + " " + u.LastName).FirstOrDefault(),

                                       ReplyMessageID = msg.ReplyMessageID,

                                       ReplyMessageCount = (from c in lstCount
                                                            where c.MessageID == msg.MessageID
                                                            select c.Count).FirstOrDefault()
                                   }).ToList();
                        }



                    }//end Inbox

                    else if (MessageTypeid == (int)SDGUtilities.MessageTypeList.Sent)
                    {
                        var entityParentList = (from msg in db.UserMessage
                                                where msg.SenderUserID == UserId && msg.FkMessageTypeID == MessageTypeid
                                                && !msg.IsDeleted
                                                select msg).ToList();
                        List<int> lstCheckedMessageId = new List<int>();
                        foreach (var item in entityParentList)
                        {
                            var messageid = item.MessageID;
                            var LastEntity = item;


                            var isPresent = lstCheckedMessageId.Where(x => x == item.MessageID).FirstOrDefault();

                            if (messageid != isPresent)
                            {
                                ReplyMessageCount = 0;
                                for (int i = 0; i < 1; i++)
                                {

                                    var entityReplyMessage = (from msg in db.UserMessage
                                                              where msg.ReplyMessageID == messageid
                                                              select msg).FirstOrDefault();

                                    if (entityReplyMessage != null)
                                    {
                                        if (entityReplyMessage.ReplyMessageID > 0)
                                        {
                                            i--;

                                            ReplyMessageCount++;

                                            messageid = entityReplyMessage.MessageID;

                                            LastEntity = entityReplyMessage;
                                            lstCheckedMessageId.Add(entityReplyMessage.MessageID);
                                        }
                                        else
                                        {
                                            if (entityReplyMessage != null)
                                            {
                                                ReplyConversationCount replyConversationCount = new ReplyConversationCount();
                                                replyConversationCount.MessageID = entityReplyMessage.MessageID;
                                                replyConversationCount.Count = ReplyMessageCount;
                                                lstCount.Add(replyConversationCount);

                                                userMessagesList.Add(entityReplyMessage);
                                                lstCheckedMessageId.Add(entityReplyMessage.MessageID);
                                            }
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (LastEntity != null)
                                        {
                                            ReplyConversationCount replyConversationCount = new ReplyConversationCount();
                                            replyConversationCount.MessageID = LastEntity.MessageID;
                                            replyConversationCount.Count = ReplyMessageCount;
                                            lstCount.Add(replyConversationCount);

                                            userMessagesList.Add(LastEntity);
                                            lstCheckedMessageId.Add(LastEntity.MessageID);
                                        }
                                        break;
                                    }
                                }
                            }
                        }//end foreach

                        if (userMessagesList != null && userMessagesList.Count > 0)
                        {
                            lst = (from msg in userMessagesList
                                   select new UserMessageViewModel
                                   {
                                       MessageID = msg.MessageID,
                                       MessageFrom = msg.MessageFrom,
                                       MessageTo = msg.MessageTo,
                                       MessageSubject = msg.MessageSubject,
                                       MessageBody = msg.MessageBody,
                                       IsViewed = msg.IsViewed,
                                       IsDeleted = msg.IsDeleted,
                                       PageTo = pageto,
                                       PageFrom = pagefrom,
                                       CreatedDateTime = msg.CreatedDateTime,
                                       SenderUserID = msg.SenderUserID,
                                       SenderUserName = (from u in db.User
                                                         where u.UserID == msg.SenderUserID
                                                         select u.FirstName + " " + u.LastName).FirstOrDefault(),

                                       ReplyMessageID = msg.ReplyMessageID,

                                       ReplyMessageCount = (from c in lstCount
                                                            where c.MessageID == msg.MessageID
                                                            select c.Count).FirstOrDefault()
                                   }).ToList();
                        }




                    }//end sendbox

                    else if (MessageTypeid == (int)SDGUtilities.MessageTypeList.Bin)
                    {
                        lst = (from msg in db.UserMessage
                               join usr in db.User on msg.ReceiverUserID equals usr.UserID
                               join usr2 in db.User on msg.SenderUserID equals usr2.UserID
                               where (msg.ReceiverUserID == UserId || msg.SenderUserID == UserId)
                               && msg.IsDeleted == false
                               && msg.FkMessageTypeID == MessageTypeid

                               select new UserMessageViewModel
                               {
                                   MessageID = msg.MessageID,
                                   MessageFrom = msg.MessageFrom,
                                   MessageTo = msg.MessageTo,
                                   MessageSubject = msg.MessageSubject,
                                   MessageBody = msg.MessageBody,
                                   IsViewed = msg.IsViewed,
                                   IsDeleted = msg.IsDeleted,
                                   PageTo = pageto,
                                   PageFrom = pagefrom,
                                   CreatedDateTime = msg.CreatedDateTime,
                                   ReceiverUserID = msg.ReceiverUserID,
                                   ReceiverUserName = usr.FirstName + " " + usr.LastName,
                                   SenderUserID = msg.SenderUserID,
                                   SenderUserName = usr2.FirstName + " " + usr2.LastName

                               }).ToList();

                    }

                    else if (MessageTypeid == (int)SDGUtilities.MessageTypeList.Draft)
                    {
                        lst = (from msg in db.UserMessage
                                   //join usr in db.User on msg.ReceiverUserID equals usr.UserID
                               where msg.SenderUserID == UserId && msg.FkMessageTypeID == MessageTypeid
                               && !msg.IsDeleted
                               select new UserMessageViewModel
                               {
                                   MessageID = msg.MessageID,
                                   MessageFrom = msg.MessageFrom,
                                   MessageTo = msg.MessageTo,
                                   MessageSubject = msg.MessageSubject,
                                   MessageBody = msg.MessageBody,
                                   IsViewed = msg.IsViewed,
                                   IsDeleted = msg.IsDeleted,
                                   PageTo = pageto,
                                   PageFrom = pagefrom,
                                   CreatedDateTime = msg.CreatedDateTime,
                                   ReceiverUserName = "Draft"

                               }).ToList();

                    }


                    if (lst != null && lst.Count > 0)
                    {
                        if (IDs != null && IDs.Length > 0)
                        {
                            foreach (var item in IDs)
                            {
                                lst.Remove(lst.Single(s => s.MessageID == item)); // Remove IDs from List
                            }

                        }

                        var skiprecordcount = (pagefrom - 1);
                        var takerecordcount = (pageto - pagefrom) + 1;

                        lstchunk = lst.OrderByDescending(q => q.MessageID).Skip(skiprecordcount).Take(takerecordcount).ToList();

                        foreach (var item in lstchunk)
                        {
                            if (!String.IsNullOrEmpty(item.MessageBody))
                            {
                                item.MessageBody = StripHTML(item.MessageBody);
                            }
                            item.TotalRecords = lst.Count;
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - FetchMessageList", Ex.Message);
            }
            return lstchunk;
        }


        public List<UserMessageViewModel> FetchMessageList(int UserId, int pagefrom, int pageto, int MessageTypeid, int[] IDs = null)
        {
            List<UserMessageViewModel> lstchunk = new List<UserMessageViewModel>();
            List<UserMessageViewModel> lst = new List<UserMessageViewModel>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (MessageTypeid == (int)SDGUtilities.MessageTypeList.Inbox)
                    {

                        SqlParameter[] Params =
                        {
                            new SqlParameter("@ReceiverUserID",UserId),
                            new SqlParameter("@FkMessageTypeID",MessageTypeid),
                             new SqlParameter("@Sendbox_FkMessageTypeID",(int)SDGUtilities.MessageTypeList.Sent)
                        };

                        DataSet ds = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), CommandType.StoredProcedure, "USP_GetInboxMessageList", Params);

                        if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            lst = (from DataRow row in ds.Tables[0].Rows

                                   select new UserMessageViewModel
                                   {
                                       MessageID = GetIntegerValue(row["MessageID"]),
                                       MessageFrom = GetStringValue(row["MessageFrom"]),
                                       MessageTo = GetStringValue(row["MessageTo"]),
                                       MessageSubject = GetStringValue(row["MessageSubject"]),
                                       MessageBody = GetStringValue(row["MessageBody"]),
                                       IsViewed = GetBooleanValue(row["IsViewed"]),
                                       IsDeleted = GetBooleanValue(row["IsDeleted"]),
                                       PageTo = pageto,
                                       PageFrom = pagefrom,
                                       CreatedDateTime = GetNotNullDateTimeValue(row["CreatedDateTime"]),
                                       //SenderUserID = GetIntegerValue(row["SenderUserID"]),
                                       SenderUserName = GetStringValue(row["FirstName"] + " " + row["LastName"]),
                                       ParentGUIID = GetStringValue(row["ParentGUIID"]),
                                       ReplyMessageCount = GetIntegerValue(row["Count"]),
                                       LastInboxMessageID = GetIntegerValue(row["LastInboxMessageID"])

                                   }).ToList();
                        }


                    }
                    else if (MessageTypeid == (int)SDGUtilities.MessageTypeList.Sent)
                    {
                        SqlParameter[] Params =
                        {
                            new SqlParameter("@SenderUserID",UserId),
                            new SqlParameter("@FkMessageTypeID",MessageTypeid),
                             new SqlParameter("@Sendbox_FkMessageTypeID",(int)SDGUtilities.MessageTypeList.Sent)
                        };

                        DataSet ds = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), CommandType.StoredProcedure, "USP_GetOutboxMessageList", Params);

                        if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            lst = (from DataRow row in ds.Tables[0].Rows

                                   select new UserMessageViewModel
                                   {
                                       MessageID = GetIntegerValue(row["MessageID"]),
                                       MessageFrom = GetStringValue(row["MessageFrom"]),
                                       MessageTo = GetStringValue(row["MessageTo"]),
                                       MessageSubject = GetStringValue(row["MessageSubject"]),
                                       MessageBody = GetStringValue(row["MessageBody"]),
                                       IsViewed = GetBooleanValue(row["IsViewed"]),
                                       IsDeleted = GetBooleanValue(row["IsDeleted"]),
                                       PageTo = pageto,
                                       PageFrom = pagefrom,
                                       CreatedDateTime = GetNotNullDateTimeValue(row["CreatedDateTime"]),
                                       //SenderUserID = GetIntegerValue(row["SenderUserID"]),
                                       ReceiverUserName = GetStringValue(row["FirstName"] + " " + row["LastName"]),
                                       ParentGUIID = GetStringValue(row["ParentGUIID"]),
                                       ReplyMessageCount = GetIntegerValue(row["Count"])


                                   }).ToList();
                        }

                    }
                    else if (MessageTypeid == (int)SDGUtilities.MessageTypeList.Bin)
                    {
                        lst = (from msg in db.UserMessage
                               join usr in db.User on msg.ReceiverUserID equals usr.UserID
                               join usr2 in db.User on msg.SenderUserID equals usr2.UserID
                               where (msg.ReceiverUserID == UserId || msg.SenderUserID == UserId)
                               && msg.IsDeleted == false
                               && msg.FkMessageTypeID == MessageTypeid

                               select new UserMessageViewModel
                               {
                                   MessageID = msg.MessageID,
                                   MessageFrom = msg.MessageFrom,
                                   MessageTo = msg.MessageTo,
                                   MessageSubject = msg.MessageSubject,
                                   MessageBody = msg.MessageBody,
                                   IsViewed = msg.IsViewed,
                                   IsDeleted = msg.IsDeleted,
                                   PageTo = pageto,
                                   PageFrom = pagefrom,
                                   CreatedDateTime = msg.CreatedDateTime,
                                   ReceiverUserID = msg.ReceiverUserID,
                                   ReceiverUserName = usr.FirstName + " " + usr.LastName,
                                   SenderUserID = msg.SenderUserID,
                                   SenderUserName = usr2.FirstName + " " + usr2.LastName

                               }).ToList();

                    }
                    else if (MessageTypeid == (int)SDGUtilities.MessageTypeList.Draft)
                    {
                        lst = (from msg in db.UserMessage
                                   //join usr in db.User on msg.ReceiverUserID equals usr.UserID
                               where msg.SenderUserID == UserId && msg.FkMessageTypeID == MessageTypeid
                               && !msg.IsDeleted
                               select new UserMessageViewModel
                               {
                                   MessageID = msg.MessageID,
                                   MessageFrom = msg.MessageFrom,
                                   MessageTo = msg.MessageTo,
                                   MessageSubject = msg.MessageSubject,
                                   MessageBody = msg.MessageBody,
                                   IsViewed = msg.IsViewed,
                                   IsDeleted = msg.IsDeleted,
                                   PageTo = pageto,
                                   PageFrom = pagefrom,
                                   CreatedDateTime = msg.CreatedDateTime,
                                   ReceiverUserName = "Draft"

                               }).ToList();

                    }


                    if (lst != null && lst.Count > 0)
                    {
                        if (IDs != null && IDs.Length > 0)
                        {
                            foreach (var item in IDs)
                            {
                                lst.Remove(lst.Single(s => s.MessageID == item)); // Remove IDs from List
                            }

                        }

                        var skiprecordcount = (pagefrom - 1);
                        var takerecordcount = (pageto - pagefrom) + 1;

                        lstchunk = lst.OrderByDescending(q => q.MessageID).Skip(skiprecordcount).Take(takerecordcount).ToList();

                        foreach (var item in lstchunk)
                        {
                            if (!String.IsNullOrEmpty(item.MessageBody))
                            {
                                item.MessageBody = StripHTML(item.MessageBody);
                            }
                            item.TotalRecords = lst.Count;
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - FetchMessageList", Ex.Message);
            }
            return lstchunk;
        }



        #endregion

        public bool MessagesMarkAsDeleteTrash(int[] MessagesID)
        {
            bool Result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    if (MessagesID != null && MessagesID.Length > 0)
                    {
                        foreach (var item in MessagesID)
                        {
                            var entity = db.UserMessage.Find(item);
                            if (entity != null)
                            {
                                entity.IsDeleted = true;

                                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();

                                Result = true;
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - MessagesMarkAsDeleteTrash", Ex.Message);
            }
            return Result;
        }

        #region [ FILES FETCH , DELETE , DOWNLOAD ]

        // DownLoad File
        public FileViewModel GetDocumentDetailsByID(int fileID)
        {
            FileViewModel model = new FileViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity_doc = db.MessageAttachment.Find(fileID);

                    if (entity_doc != null && entity_doc.ID > 0)
                    {
                        model.FileID = entity_doc.ID;
                        model.FileName = entity_doc.FileName;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - GetDocumentDetailsByID", Ex.Message);
            }
            return model;
        }

        public bool DeleteUploadedFileID(int fileID)
        {
            bool result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var docs = db.MessageAttachment.Find(fileID);
                    if (docs != null)
                    {
                        db.MessageAttachment.Remove(docs);
                        db.SaveChanges();
                        result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - DeleteUploadedFileID", Ex.Message);
            }
            return result;
        }

        #endregion

        #region [ USER MESSAGE TREE HISTORY ]

        public List<UserMessageViewModel> FetchMessageReplyMessageID(int? MessageID, int MainMessageID)
        {
            List<UserMessageViewModel> lst = new List<UserMessageViewModel>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {


                    var entity = (from msg in db.UserMessage
                                  where msg.MessageID == MessageID
                                  && !msg.IsDeleted
                                  orderby msg.CreatedDateTime descending
                                  select new UserMessageViewModel
                                  {
                                      MessageID = msg.MessageID,
                                      MessageFrom = msg.MessageFrom,
                                      MessageTo = msg.MessageTo,
                                      MessageSubject = msg.MessageSubject,
                                      MessageBody = msg.MessageBody,
                                      IsViewed = msg.IsViewed,
                                      IsDeleted = msg.IsDeleted,
                                      CreatedDateTime = msg.CreatedDateTime,
                                      SenderUserID = msg.SenderUserID,
                                      FKReplyTypeID = msg.FKReplyTypeID,
                                      ReplyMessageID = msg.ReplyMessageID
                                  }).FirstOrDefault();

                    if (entity != null)
                    {
                        lst.Add(entity);

                        var ReplyMessageID = entity.ReplyMessageID;

                        if (entity.ReplyMessageID > 0)
                        {
                            for (int i = 0; i < 1; i++)
                            {
                                var entityreplymessage = IsPresentReplyMessage(GetIntegerValue(ReplyMessageID));

                                if (entityreplymessage != null)
                                {
                                    lst.Add(entityreplymessage);

                                    ReplyMessageID = entityreplymessage.ReplyMessageID;

                                    if (entityreplymessage.ReplyMessageID > 0)
                                    {
                                        i--;
                                    }

                                }
                            }
                        }

                        if (ReplyMessageID == null)
                        {

                            //check is any mail by massage id and other fkmessageid
                            var chkEntity = (from msg in db.UserMessage
                                             where msg.ReplyMessageID == MainMessageID
                                             && !msg.IsDeleted
                                             select new UserMessageViewModel
                                             {
                                                 MessageID = msg.MessageID,
                                                 MessageFrom = msg.MessageFrom,
                                                 MessageTo = msg.MessageTo,
                                                 MessageSubject = msg.MessageSubject,
                                                 MessageBody = msg.MessageBody,
                                                 IsViewed = msg.IsViewed,
                                                 IsDeleted = msg.IsDeleted,
                                                 CreatedDateTime = msg.CreatedDateTime,
                                                 SenderUserID = msg.SenderUserID,
                                                 FKReplyTypeID = msg.FKReplyTypeID
                                             }).FirstOrDefault();

                            if (chkEntity != null)
                            {
                                lst.Add(chkEntity);

                            }
                        }
                    }
                }
                if (lst.Count > 0)
                {
                    var lischunk = lst.OrderByDescending(l => l.MessageID).ToList();
                    lst = lischunk;
                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - FetchMessageReplyListByMessageID", Ex.Message);
            }
            return lst;
        }

        public UserMessageViewModel IsPresentReplyMessage(int MeassageID)
        {
            UserMessageViewModel retunResult = new UserMessageViewModel();

            try
            {
                if (MeassageID > 0)
                {
                    using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                        retunResult = (from msg in db.UserMessage
                                       where msg.MessageID == MeassageID
                                       && !msg.IsDeleted
                                       orderby msg.CreatedDateTime descending
                                       select new UserMessageViewModel
                                       {
                                           MessageID = msg.MessageID,
                                           MessageFrom = msg.MessageFrom,
                                           MessageTo = msg.MessageTo,
                                           MessageSubject = msg.MessageSubject,
                                           MessageBody = msg.MessageBody,
                                           IsViewed = msg.IsViewed,
                                           IsDeleted = msg.IsDeleted,
                                           CreatedDateTime = msg.CreatedDateTime,
                                           SenderUserID = msg.SenderUserID,
                                           FKReplyTypeID = msg.FKReplyTypeID,
                                           ReplyMessageID = msg.ReplyMessageID
                                       }).FirstOrDefault();
                    }
                }
            }
            catch (Exception Ex)
            {

                WriteLog("SDGApp.Models.UserMessageModel - IsPresentReplyMessage", Ex.Message);
            }

            return retunResult;
        }

        public List<UserMessageViewModel> FetchReplybyMessageID(int? ID)
        {
            List<UserMessageViewModel> lst = new List<UserMessageViewModel>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    lst = (from msg in db.UserMessage
                           where msg.ReplyMessageID == ID
                           && !msg.IsDeleted
                           orderby msg.CreatedDateTime descending
                           select new UserMessageViewModel
                           {
                               MessageID = msg.MessageID,
                               MessageFrom = msg.MessageFrom,
                               MessageTo = msg.MessageTo,
                               MessageSubject = msg.MessageSubject,
                               MessageBody = msg.MessageBody,
                               IsViewed = msg.IsViewed,
                               IsDeleted = msg.IsDeleted,
                               CreatedDateTime = msg.CreatedDateTime,
                               SenderUserID = msg.SenderUserID,
                               FKReplyTypeID = msg.FKReplyTypeID
                           }).ToList();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserMessageModel - FetchMessageReplyListByMessageID", Ex.Message);
            }
            return lst;
        }



        #endregion
    }
}