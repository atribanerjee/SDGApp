using SDGApp.Helpers;
using SDGApp.ViewModel;
using SDGAppDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SDGApp.Models
{
    public class UserContactsModel : BaseModel
    {
        public UserContactsViewModel GetContactReceiverDetailByGUID(string guid)
        {
            UserContactsViewModel model = new UserContactsViewModel();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    model = (from contact in db.UserContacts
                             join usr in db.User on contact.FKReceiverUserID equals usr.UserID
                             where contact.ContactGuid.Trim().ToLower() == guid.Trim().ToLower()

                             select new UserContactsViewModel
                             {
                                 ContactID = contact.ContactID,
                                 FKReceiverUserID = contact.FKReceiverUserID,
                                 FirstName = usr.FirstName,
                                 LastName = usr.LastName,
                                 Email = usr.Email,
                                 Phone = usr.Phone,
                                 Picture = usr.Picture,
                                 CreatedDatetime = contact.CreatedDatetime
                             }).FirstOrDefault();

                }


            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserContactsModel - GetContactDetailByGUID", Ex.Message);
            }
            return model;
        }

        public UserContactsViewModel GetContactSenderDetailByGUID(string guid)
        {
            UserContactsViewModel model = new UserContactsViewModel();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    model = (from contact in db.UserContacts
                             join usr in db.User on contact.FKSenderUserID equals usr.UserID
                             where contact.ContactGuid.Trim().ToLower() == guid.Trim().ToLower()

                             select new UserContactsViewModel
                             {
                                 ContactID = contact.ContactID,
                                 FKSenderUserID = contact.FKSenderUserID,
                                 FirstName = usr.FirstName,
                                 LastName = usr.LastName,
                                 Email = usr.Email,
                                 Phone = usr.Phone,
                                 Picture = usr.Picture,
                                 CreatedDatetime = contact.CreatedDatetime
                             }).FirstOrDefault();

                }


            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserContactsModel - GetContactDetailByGUID", Ex.Message);
            }
            return model;
        }

        public UserContactsViewModel GetContactDetailsByContactID(int ContactID)
        {
            UserContactsViewModel model = new UserContactsViewModel();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entityContact = db.UserContacts.Find(ContactID);

                    if (entityContact != null && entityContact.ContactID > 0)
                    {
                        model = (from contact in db.UserContacts
                                 join usr in db.User on contact.FKSenderUserID equals usr.UserID
                                 join usr2 in db.User on contact.FKReceiverUserID equals usr2.UserID
                                 where contact.ContactID == entityContact.ContactID

                                 select new UserContactsViewModel
                                 {
                                     ContactID = contact.ContactID,
                                     FKSenderUserID = contact.FKSenderUserID,
                                     FKReceiverUserID = contact.FKReceiverUserID,
                                     SenderFirstName = usr.FirstName,
                                     SenderLastName = usr.LastName,
                                     SenderEmail = usr.Email,
                                     SenderPhone = usr.Phone,
                                     SenderPicture = usr.Picture,

                                     ReceiverFirstName = usr2.FirstName,
                                     ReceiverLastName = usr2.LastName,
                                     ReceiverEmail = usr2.Email,
                                     ReceiverPhone = usr2.Phone,
                                     ReceiverPicture = usr2.Picture,


                                     CreatedDatetime = contact.CreatedDatetime
                                 }).FirstOrDefault();
                    }
                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserContactsModel - GetContactDetailsByContactID", Ex.Message);
            }
            return model;
        }

        public bool SaveContactGUID(int loggedinUserID, int inviteeUserID, string ID, ref bool isValidInvitee)
        {
            bool result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    // Check if this invitation is already sent before

                    var EntityUC = (from uc in db.UserContacts where uc.FKSenderUserID == loggedinUserID && uc.FKReceiverUserID == inviteeUserID select uc).FirstOrDefault();

                    if (EntityUC == null)
                    {

                        var entity = new SDGAppDB.POCO.UserContacts();
                        entity.FKSenderUserID = loggedinUserID;
                        entity.FKReceiverUserID = inviteeUserID;
                        entity.ContactGuid = ID;
                        entity.CreatedDatetime = DateTime.Now;


                        db.UserContacts.Add(entity);
                        db.SaveChanges();

                        result = true;
                    }
                    else
                    {
                        isValidInvitee = false;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserContactsModel - SaveContactGUID", Ex.Message);
            }

            return result;
        }

        public bool UpdateContactsReplyType(int ContactID, string invitationtype)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.UserContacts.Find(ContactID);

                    if (entity != null && entity.ContactID > 0)
                    {
                        if (invitationtype.ToLower().Contains("accepted"))
                        {
                            entity.IsAccepted = true;
                            entity.IsRejected = false;
                        }
                        else
                        {
                            entity.IsAccepted = false;
                            entity.IsRejected = true;
                        }


                        db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        string mode = "accepted";

                        if (!invitationtype.ToLower().Contains("accepted"))
                        {
                            mode = "rejected";
                        }

                        UserContactsViewModel model = GetContactDetailsByContactID(ContactID);

                        Dictionary<string, string> objDict = new Dictionary<string, string>();
                        objDict.Add("SenderName", model.SenderFirstName);
                        objDict.Add("ReceiverName", model.ReceiverFirstName + " " + model.ReceiverLastName);
                        objDict.Add("ReplyMode", mode);
                        objDict.Add("ToYear", DateTime.Now.Year.ToString());

                        MailHelper MH = new MailHelper();

                        MH.SendEmail("HealthGauge - Invitation " + mode, model.SenderEmail, "ContactInvitationReply.html", objDict);



                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserContactsModel - UpdateContactsReplyType", Ex.Message);
            }
            return Result;

        }

        public bool SaveContactAccept(int ContactID)
        {
            bool result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.UserContacts.Find(ContactID);

                    if (entity != null && entity.ContactID > 0)
                    {
                        // if already invitation sent previously, then just update

                        var entityAlreadyExists = (from uc in db.UserContacts where uc.FKSenderUserID == (int)entity.FKReceiverUserID && uc.FKReceiverUserID == (int)entity.FKSenderUserID select uc).FirstOrDefault();

                        if (entityAlreadyExists != null && entityAlreadyExists.ContactID > 0)
                        {
                            var entityAlreadyExistsNew = db.UserContacts.Find(entityAlreadyExists.ContactID);

                            entityAlreadyExistsNew.IsAccepted = true;
                            entityAlreadyExistsNew.AcceptedDate = DateTime.Now;
                            entityAlreadyExistsNew.IsRejected = false;
                            entityAlreadyExistsNew.IsDeleted = false;

                            db.Entry(entityAlreadyExistsNew).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            var entityNew = new SDGAppDB.POCO.UserContacts();
                            entityNew.FKSenderUserID = GetIntegerValue(entity.FKReceiverUserID);
                            entityNew.FKReceiverUserID = GetIntegerValue(entity.FKSenderUserID);
                            entityNew.IsAccepted = true;
                            entityNew.IsRejected = false;

                            entityNew.CreatedDatetime = DateTime.Now;
                            entityNew.AcceptedDate = DateTime.Now;  // added

                            db.UserContacts.Add(entityNew);
                            db.SaveChanges();
                        }

                        result = true;


                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserContactsModel - SaveContactAccept", Ex.Message);
            }

            return result;
        }

        public List<UserContactsViewModel> GetUserContactList(int UserID, string ServerPath, int pageNumber, int pageSize, string SearchKey = "", string sortbyvalue = "accepteddate", int[] IDs = null)
        {
            List<UserContactsViewModel> lstchunk = new List<UserContactsViewModel>();
            List<UserContactsViewModel> lst = new List<UserContactsViewModel>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (string.IsNullOrEmpty(SearchKey))
                    {

                        lst = (from contact in db.UserContacts
                               join usr in db.User on contact.FKReceiverUserID equals usr.UserID
                               where contact.IsDeleted == false && contact.IsAccepted == true && !contact.IsRejected
                               && contact.FKSenderUserID == UserID

                               select new UserContactsViewModel
                               {
                                   ContactID = contact.ContactID,
                                   FKSenderUserID = contact.FKSenderUserID,
                                   FKReceiverUserID = contact.FKReceiverUserID,
                                   FirstName = usr.FirstName,
                                   LastName = usr.LastName,
                                   Email = usr.Email,
                                   Phone = usr.Phone,
                                   Picture = usr.Picture,
                                   CreatedDatetime = contact.CreatedDatetime,
                                   AcceptedDate = contact.AcceptedDate,
                                   PageSize = pageSize,
                                   PageNumber = pageNumber
                               }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(SearchKey) && SearchKey.Length > 0)
                    {
                        lst = (from contact in db.UserContacts
                               join usr in db.User on contact.FKReceiverUserID equals usr.UserID
                               where contact.IsDeleted == false && contact.IsAccepted == true && !contact.IsRejected
                               && contact.FKSenderUserID == UserID &&
                               (usr.FirstName.Trim().ToLower().Contains(SearchKey.Trim().ToLower())
                                || usr.LastName.Trim().ToLower().Contains(SearchKey.Trim().ToLower())
                                || usr.Email.Trim().ToLower().Contains(SearchKey.Trim().ToLower())
                                || (usr.FirstName.Trim().ToLower() + " " + usr.LastName.Trim().ToLower()).Contains(SearchKey.Trim().ToLower()))

                               select new UserContactsViewModel
                               {
                                   ContactID = contact.ContactID,
                                   FKSenderUserID = contact.FKSenderUserID,
                                   FKReceiverUserID = contact.FKReceiverUserID,
                                   FirstName = usr.FirstName,
                                   LastName = usr.LastName,
                                   Email = usr.Email,
                                   Phone = usr.Phone,
                                   Picture = usr.Picture,
                                   CreatedDatetime = contact.CreatedDatetime,
                                   AcceptedDate = contact.AcceptedDate,

                                   PageSize = pageSize,
                                   PageNumber = pageNumber
                               }).ToList();
                    }



                    if (lst != null && lst.Count > 0)
                    {
                        if (IDs != null && IDs.Length > 0)
                        {
                            foreach (var item in IDs)
                            {
                                lst.Remove(lst.Single(s => s.ContactID == item)); // Remove IDs from List
                            }

                        }

                        if (sortbyvalue.Trim().ToLower().Contains("accepteddate"))
                        {
                            lstchunk = lst.OrderByDescending(q => q.AcceptedDate).Skip(SkipRecords(pageSize, pageNumber)).Take(pageSize).ToList();
                        }
                        else if (sortbyvalue.Trim().ToLower().Contains("fname"))
                        {
                            lstchunk = lst.OrderBy(q => q.FirstName).Skip(SkipRecords(pageSize, pageNumber)).Take(pageSize).ToList();
                        }
                        else if (sortbyvalue.Trim().ToLower().Contains("lname"))
                        {
                            lstchunk = lst.OrderBy(q => q.LastName).Skip(SkipRecords(pageSize, pageNumber)).Take(pageSize).ToList();
                        }


                        lstchunk.ForEach(l => l.TotalRecords = lst.Count());
                        lstchunk.ForEach(l => l.CreatedDatetimeString = l.CreatedDatetime.ToString("yyyy-MM-dd HH:mm:ss"));
                    }

                    foreach (var item in lstchunk)
                    {
                        if (!string.IsNullOrEmpty(item.Picture))
                        {
                            if (!File.Exists(Path.Combine(ServerPath, item.Picture)))
                            {
                                item.Picture = GlobalConstants.BaseUrl + "/" + "Content/Latest/images/no-image-profile-male-sm.png";
                            }
                            else
                            {
                                item.Picture = GlobalConstants.BaseUrl + "/Content/images/" + item.Picture;
                            }
                        }
                        else
                        {
                            item.Picture = GlobalConstants.BaseUrl + "/" + "Content/Latest/images/no-image-profile-male-sm.png";
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserContactsModel - GetUserContactList", Ex.Message);
            }
            return lstchunk;
        }

        public List<UserContactsViewModel> GetUserContactInvitationList(int UserID, string ServerPath, int pageNumber, int pageSize, string SearchKey = "")
        {
            List<UserContactsViewModel> lstchunk = new List<UserContactsViewModel>();
            List<UserContactsViewModel> lst = new List<UserContactsViewModel>();


            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (string.IsNullOrEmpty(SearchKey))
                    {

                        lst = (from usr in db.User
                               where !(from contact in db.UserContacts
                                       select contact.FKReceiverUserID)
                                      .Contains(usr.UserID) && usr.IsDeleted == false && usr.IsActive == true
                                      && usr.UserID != UserID     // not logged in user

                               select new UserContactsViewModel
                               {
                                   FKReceiverUserID = usr.UserID,
                                   FirstName = usr.FirstName,
                                   LastName = usr.LastName,
                                   Email = usr.Email,
                                   Phone = usr.Phone,
                                   Picture = usr.Picture,

                                   PageSize = pageSize,
                                   PageNumber = pageNumber,
                               }).ToList();

                    }
                    else if (!string.IsNullOrEmpty(SearchKey) && SearchKey.Length > 0)
                    {
                        lst = (from usr in db.User
                               where !(from contact in db.UserContacts
                                       select contact.FKReceiverUserID)
                                      .Contains(usr.UserID) && usr.IsDeleted == false && usr.IsActive == true
                                      && usr.UserID != UserID &&
                                      (usr.FirstName.Trim().ToLower().Contains(SearchKey.Trim().ToLower())
                                      || usr.LastName.Trim().ToLower().Contains(SearchKey.Trim().ToLower())
                                      || usr.Email.Trim().ToLower().Contains(SearchKey.Trim().ToLower())
                                      || (usr.FirstName.Trim().ToLower() + " " + usr.LastName.Trim().ToLower()).Contains(SearchKey.Trim().ToLower()))


                               select new UserContactsViewModel
                               {
                                   FKReceiverUserID = usr.UserID,
                                   FirstName = usr.FirstName,
                                   LastName = usr.LastName,
                                   Email = usr.Email,
                                   Phone = usr.Phone,
                                   Picture = usr.Picture,
                                   PageSize = pageSize,
                                   PageNumber = pageNumber,
                               }).ToList();
                    }
                    foreach (var item in lst)
                    {
                        if (!string.IsNullOrEmpty(item.Picture))
                        {
                            if (!File.Exists(Path.Combine(ServerPath, item.Picture)))
                            {
                                item.Picture = GlobalConstants.BaseUrl + "/" + "Content/Latest/images/no-image-profile-male-sm.png";
                            }
                            else
                            {
                                item.Picture = GlobalConstants.BaseUrl + "/Content/images/" + item.Picture;
                            }
                        }
                        else
                        {
                            item.Picture = GlobalConstants.BaseUrl + "/" + "Content/Latest/images/no-image-profile-male-sm.png";
                        }
                    }


                    if (lst != null && lst.Count > 0)
                    {
                        lstchunk = lst.OrderBy(q => q.FKReceiverUserID).Skip(SkipRecords(pageSize, pageNumber)).Take(pageSize).ToList();
                        lstchunk.ForEach(l => l.TotalRecords = lst.Count());
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserContactsModel - GetUserContactInvitationList", Ex.Message);
            }
            return lstchunk;
        }

        public List<UserContactsViewModel> GetPendingInvitationList(int UserID, string ServerPath, string SearchKey = "", int[] IDs = null)
        {
            List<UserContactsViewModel> lst = new List<UserContactsViewModel>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (string.IsNullOrEmpty(SearchKey))
                    {

                        lst = (from contact in db.UserContacts
                               join usr in db.User on contact.FKSenderUserID equals usr.UserID
                               where usr.IsDeleted == false && usr.IsActive == true
                               && contact.IsAccepted == false && contact.IsRejected == false && !contact.IsDeleted
                               && contact.FKReceiverUserID == UserID

                               select new UserContactsViewModel
                               {
                                   ContactID = contact.ContactID,
                                   FKSenderUserID = usr.UserID,
                                   FirstName = usr.FirstName,
                                   LastName = usr.LastName,
                                   Email = usr.Email,
                                   Phone = usr.Phone,
                                   Picture = usr.Picture
                               }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(SearchKey) && SearchKey.Length > 0)
                    {

                        lst = (from contact in db.UserContacts
                               join usr in db.User on contact.FKSenderUserID equals usr.UserID
                               where usr.IsDeleted == false && usr.IsActive == true
                               && contact.IsAccepted == false && contact.IsRejected == false && !contact.IsDeleted
                               && contact.FKReceiverUserID == UserID &&
                                (usr.FirstName.Trim().ToLower().Contains(SearchKey.Trim().ToLower())
                                      || usr.LastName.Trim().ToLower().Contains(SearchKey.Trim().ToLower())
                                      || usr.Email.Trim().ToLower().Contains(SearchKey.Trim().ToLower())
                                      || (usr.FirstName.Trim().ToLower() + " " + usr.LastName.Trim().ToLower()).Contains(SearchKey.Trim().ToLower()))

                               select new UserContactsViewModel
                               {
                                   ContactID = contact.ContactID,
                                   FKSenderUserID = usr.UserID,
                                   FirstName = usr.FirstName,
                                   LastName = usr.LastName,
                                   Email = usr.Email,
                                   Phone = usr.Phone,
                                   Picture = usr.Picture
                               }).ToList();
                    }

                    foreach (var item in lst)
                    {
                        if (!string.IsNullOrEmpty(item.Picture))
                        {
                            if (!File.Exists(Path.Combine(ServerPath, item.Picture)))
                            {
                                item.Picture = GlobalConstants.BaseUrl + "/" + "Content/Latest/images/no-image-profile-male-sm.png";
                            }
                            else
                            {
                                item.Picture = GlobalConstants.BaseUrl + "/Content/images/" + item.Picture;
                            }
                        }
                        else
                        {
                            item.Picture = GlobalConstants.BaseUrl + "/" + "Content/Latest/images/no-image-profile-male-sm.png";
                        }
                    }

                    if (IDs != null && IDs.Length > 0)
                    {
                        foreach (var item in IDs)
                        {
                            lst.Remove(lst.Single(s => s.ContactID == item)); // Remove IDs from List 
                        }

                    }

                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserContactsModel - GetPendingInvitationList", Ex.Message);
            }
            return lst;
        }

        public List<UserContactsViewModel> GetInfiniteScrollContacts(int PageNo = 1, int Pagesize = 10, string SearchKey = "")
        {
            List<UserContactsViewModel> lstchunk = new List<UserContactsViewModel>();
            List<UserContactsViewModel> lst = new List<UserContactsViewModel>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (string.IsNullOrEmpty(SearchKey))
                    {
                        lst = (from usr in db.User
                               where !(from contact in db.UserContacts
                                       select contact.FKReceiverUserID)
                                      .Contains(usr.UserID) && usr.IsDeleted == false && usr.IsActive == true
                               select new UserContactsViewModel
                               {
                                   FKReceiverUserID = usr.UserID,
                                   FirstName = usr.FirstName,
                                   LastName = usr.LastName,
                                   Email = usr.Email,
                                   Phone = usr.Phone,
                                   Picture = usr.Picture,

                                   PageSize = Pagesize,
                                   PageNumber = PageNo,
                               }).ToList();

                    }
                    else if (!string.IsNullOrEmpty(SearchKey) && SearchKey.Length > 0)
                    {
                        lst = (from usr in db.User
                               where !(from contact in db.UserContacts
                                       select contact.FKReceiverUserID)
                                      .Contains(usr.UserID) && usr.IsDeleted == false && usr.IsActive == true &&
                                      (usr.FirstName.Trim().ToLower().Contains(SearchKey.Trim().ToLower())
                                      || usr.LastName.Trim().ToLower().Contains(SearchKey.Trim().ToLower())
                                      || (usr.FirstName.Trim().ToLower() + " " + usr.LastName.Trim().ToLower()).Contains(SearchKey.Trim().ToLower()))

                               select new UserContactsViewModel
                               {
                                   FKReceiverUserID = usr.UserID,
                                   FirstName = usr.FirstName,
                                   LastName = usr.LastName,
                                   Email = usr.Email,
                                   Phone = usr.Phone,
                                   Picture = usr.Picture,

                                   PageSize = Pagesize,
                                   PageNumber = PageNo,
                               }).ToList();
                    }

                    if (lst != null && lst.Count > 0)
                    {
                        lstchunk = lst.OrderBy(q => q.FKReceiverUserID).Skip(SkipRecords(Pagesize, PageNo)).Take(Pagesize).ToList();
                        lstchunk.ForEach(l => l.TotalRecords = lst.Count());
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserContactsModel - GetInfiniteScrollContacts", Ex.Message);
            }

            return lstchunk;
        }

        public bool DeleteContactByUserId(int UserId, int ContactUserId)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = (from c in db.UserContacts
                                  where c.FKSenderUserID == UserId
                                  && c.FKReceiverUserID == ContactUserId
                                  select c).FirstOrDefault();

                    if (entity != null && entity.ContactID > 0)
                    {

                        db.UserContacts.Remove(entity);

                        //entity.IsDeleted = true;

                        //db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                    }

                    var entitycontact = (from c in db.UserContacts
                                         where c.FKSenderUserID == ContactUserId
                                         && c.FKReceiverUserID == UserId
                                         select c).FirstOrDefault();

                    if (entitycontact != null && entitycontact.ContactID > 0)
                    {

                        db.UserContacts.Remove(entitycontact);

                        //entitycontact.IsDeleted = true;

                        //db.Entry(entitycontact).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserContactsModel - DeleteContactByUserId", Ex.Message);
            }
            return Result;

        }

        public List<SearchLeadViewModel> SearchLeads(int loggedinUserID, string ServerPath, string SearchText)
        {
            List<SearchLeadViewModel> lst = new List<SearchLeadViewModel>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    // Get List of available contacts of the user

                    var Contacts = (from uc in db.UserContacts where uc.FKSenderUserID == loggedinUserID select uc.FKReceiverUserID).ToList();

                    if (Contacts != null && Contacts.Count > 0)
                    {
                        lst = (from u in db.User
                               where !Contacts.Contains(u.UserID) && u.IsActive && !u.IsDeleted && u.UserID != loggedinUserID
                               && (u.FirstName.ToLower().Trim().Contains(SearchText.ToLower().Trim())
                               || u.LastName.ToLower().Trim().Contains(SearchText.ToLower().Trim())
                               || u.Email.ToLower().Trim().Contains(SearchText.ToLower().Trim())
                               || u.UserName.ToLower().Trim().Contains(SearchText.ToLower().Trim()))
                               select new SearchLeadViewModel
                               {
                                   UserID = u.UserID,
                                   FirstName = u.FirstName,
                                   LastName = u.LastName,
                                   Picture = u.Picture
                               }).ToList();
                    }
                    else
                    {
                        lst = (from u in db.User
                               where u.IsActive && !u.IsDeleted && u.UserID != loggedinUserID && (u.FirstName.ToLower().Trim().Contains(SearchText.ToLower().Trim())
                               || u.LastName.ToLower().Trim().Contains(SearchText.ToLower().Trim())
                               || u.Email.ToLower().Trim().Contains(SearchText.ToLower().Trim())
                               || u.UserName.ToLower().Trim().Contains(SearchText.ToLower().Trim()))
                               select new SearchLeadViewModel
                               {
                                   UserID = u.UserID,
                                   FirstName = u.FirstName,
                                   LastName = u.LastName,
                                   Picture = u.Picture
                               }).ToList();
                    }

                    foreach (var item in lst)
                    {
                        if (!string.IsNullOrEmpty(item.Picture))
                        {
                            if (!File.Exists(Path.Combine(ServerPath, item.Picture)))
                            {
                                item.Picture = GlobalConstants.BaseUrl + "/" + "Content/Latest/images/no-image-profile-male-sm.png";
                            }
                            else
                            {
                                item.Picture = GlobalConstants.BaseUrl + "/Content/images/" + item.Picture;
                            }
                        }
                        else
                        {
                            item.Picture = GlobalConstants.BaseUrl + "/" + "Content/Latest/images/no-image-profile-male-sm.png";
                        }
                    }

                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserContactsModel - SearchLeads", Ex.Message);
            }
            return lst;
        }

        public bool SendInvitation(int loggedinUserID, int inviteeUserID, ref bool isValidInvitee, ref string ReturnMessage)
        {
            bool Result = false;
            try
            {
                UserModel UM = new UserModel();
                Guid guid = Guid.NewGuid();
                var strguid = guid.ToString();

                UserViewModel loggedinUserModel = UM.GetUserDetailByUserID(loggedinUserID);

                UserViewModel inviteeUserModel = UM.GetUserDetailByUserID(inviteeUserID);

                var SenderName = loggedinUserModel.FirstName + " " + loggedinUserModel.LastName;


                if (loggedinUserModel != null && loggedinUserModel.UserID > 0)
                {
                    if (SaveContactGUID(loggedinUserID, inviteeUserID, strguid, ref isValidInvitee))
                    {
                        string SenderPicturePath = "";

                        if (!string.IsNullOrEmpty(loggedinUserModel.Picture))
                        {
                            SenderPicturePath = GlobalConstants.BaseUrl + "/Content/images/" + loggedinUserModel.Picture;
                        }
                        else
                        {
                            SenderPicturePath = GlobalConstants.BaseUrl + "/Content/Latest/images/no-image-profile-male-sm.png";
                        }


                        Dictionary<string, string> objDict = new Dictionary<string, string>();
                        objDict.Add("ConnectName", inviteeUserModel.FirstName);
                        objDict.Add("SenderName", SenderName);
                        objDict.Add("SenderPicture", SenderPicturePath);
                        //objDict.Add("FromYear", (DateTime.Now.Year - 1).ToString());
                        objDict.Add("ToYear", DateTime.Now.Year.ToString());

                        //objDict.Add("JoinUrl", GlobalConstants.BaseUrl + "/Contacts/ContactAccept?GUID=" + strguid);

                        objDict.Add("JoinUrl", GlobalConstants.BaseUrl + "/Contacts/PendingInvitation");

                        MailHelper MH = new MailHelper();

                        MH.SendEmail(inviteeUserModel.FirstName + ", please add me to your HealthGauge network", inviteeUserModel.Email, "ContactInvitation.html", objDict);

                        ReturnMessage = "Invitation request has been sent to " + inviteeUserModel.FirstName + " " + inviteeUserModel.LastName + "'s registered email.";
                        Result = true;
                    }
                    else
                    {
                        if (isValidInvitee)
                        {
                            ReturnMessage = "Invitation sending is failed.";
                        }
                        else
                        {
                            ReturnMessage = "Invitation cannot be sent for this user, either this is already in contact or deleted contact or invitation pending or rejected.";
                        }
                    }
                }
                else
                {
                    ReturnMessage = "Invitation sending failed, invalied sender.";
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserContactsModel - SendInvitation", Ex.Message);
            }

            return Result;
        }

        public List<UserContactsViewModel> SendingInvitationList(int UserID, int[] IDs = null)
        {
            List<UserContactsViewModel> lstUserContact = new List<UserContactsViewModel>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    lstUserContact = (from uc in db.UserContacts
                                      join u in db.User on uc.FKReceiverUserID equals u.UserID
                                      where uc.FKSenderUserID == UserID
                                      && !uc.IsAccepted
                                      && !uc.IsRejected
                                      orderby uc.ContactID descending
                                      select new UserContactsViewModel
                                      {
                                          FKReceiverUserID = uc.FKReceiverUserID,
                                          ReceiverFirstName = u.FirstName,
                                          ReceiverLastName = u.LastName,
                                          ReceiverPicture = u.Picture,
                                          CreatedDatetime = uc.CreatedDatetime

                                      }).ToList();

                    if (lstUserContact != null && lstUserContact.Count > 0)
                    {
                        if (IDs != null && IDs.Length > 0)
                        {
                            foreach (var item in IDs)
                            {
                                lstUserContact.Remove(lstUserContact.Single(s => s.ContactID == item)); // Remove IDs from List 
                            }

                        }

                        lstUserContact.ForEach(l => l.CreatedDatetimeString = l.CreatedDatetime.ToString("yyyy-MM-dd HH:mm:ss"));

                    }
                }
            }
            catch (Exception Ex)
            {

                WriteLog("SDGApp.Models.UserContactsModel - SendingInvitationList", Ex.Message);
            }

            return lstUserContact;
        }
    }
}