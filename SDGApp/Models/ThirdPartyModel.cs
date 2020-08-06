using SDGApp.Helpers;
using SDGApp.ViewModel;
using SDGAppDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.Models
{
    public class ThirdPartyModel : UserModel
    {
        private Random random = new Random();
        MailHelper MH;

        public ThirdPartyModel()
        {
            MH = new MailHelper();
        }

        public Boolean AddAPIKey(string APIKeyName, int CompanyID, int UserID, string UserEmail, string UserFirstName)
        {
            Boolean result = false;

            //     int UserId = GetLoggedInUserInfo().UserID;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    if (!String.IsNullOrEmpty(APIKeyName))
                    {
                        var existAPIKeyName = (from tp in db.UserThirdPartyAPIKey
                                               where tp.APIKeyName.ToLower() == APIKeyName.ToLower() && tp.FKCompanyID == CompanyID && !tp.IsDeleted
                                               select tp).FirstOrDefault();

                        String APIKeyValue = GetStringValue(Guid.NewGuid());

                        if (existAPIKeyName == null && CompanyID > 0)
                        {
                            var entity = new SDGAppDB.POCO.UserThirdPartyAPIKey();

                            entity.FKCompanyID = GetLontValue(CompanyID);
                            entity.APIKeyName = GetStringValue(APIKeyName);
                            entity.APIKeyID = GetStringValue(APIKeyName + "-" + CompanyID + "-" + DateTime.Now.Ticks);
                            entity.APIKeyValue = APIKeyValue;
                            entity.ConfimationCode = RandomString(9);
                            entity.FKUserID = UserID;
                            db.UserThirdPartyAPIKey.Add(entity);
                            db.SaveChanges();


                            // Get User Details


                            var ConfirmationCode = entity.ConfimationCode;


                            // Send Email Notification
                            if (entity.UserThirdPartyAPIKeyID > 0)
                            {
                                Dictionary<string, string> objDict = new Dictionary<string, string>();
                                objDict.Add("ToName", UserFirstName);
                                objDict.Add("APIKeyName", APIKeyName);
                                objDict.Add("ConfirmationCode", ConfirmationCode);
                                objDict.Add("ToYear", DateTime.Now.Year.ToString());

                                MH.SendEmail("API Key Created - Confirmation", UserEmail, "APIConfirmationCodeNotification.html", objDict);
                            }


                            result = true;
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.ThirdPartyModel - AddAPIKey", Ex.Message);
            }

            return result;
        }

        public Boolean DeleteAPIKey(Int64 id)
        {
            Boolean result = false;            

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var Entity = db.UserThirdPartyAPIKey.Find(id);

                    if (Entity != null && Entity.UserThirdPartyAPIKeyID > 0)
                    {
                        Entity.IsDeleted = true;

                        db.Entry(Entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        // Get User Details

                        var UserEmail = GetLoggedInUserInfo().Email;
                        var UserFirstName = GetLoggedInUserInfo().FirstName;
                        var APIKeyName = Entity.APIKeyName;

                        // Send Email Notification

                        Dictionary<string, string> objDict = new Dictionary<string, string>();
                        objDict.Add("ToName", UserFirstName);
                        objDict.Add("APIKeyName", APIKeyName);                        
                        objDict.Add("ToYear", DateTime.Now.Year.ToString());

                        MH.SendEmail("API Key Deleted - Notification", UserEmail, "APIKeyDeleteNotification.html", objDict);

                        result = true;
                    }
                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.ThirdPartyModel - DeleteAPIKey", Ex.Message);
            }

            return result;
        }

        public List<ThirdPartyViewModel> LoadAPIKeys(Int64 CompanyID)
        {
            List<ThirdPartyViewModel> _list = new List<ThirdPartyViewModel>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var Entity = (from utp in db.UserThirdPartyAPIKey where utp.FKCompanyID == CompanyID && !utp.IsDeleted orderby utp.UserThirdPartyAPIKeyID descending select new { utp }).ToList();
                    if (Entity != null && Entity.Count > 0)
                    {
                        foreach (var item in Entity)
                        {
                            _list.Add(new ThirdPartyViewModel
                            {
                                UserThirdPartyAPIKeyID = item.utp.UserThirdPartyAPIKeyID,
                                FKCompanyID = item.utp.FKCompanyID,
                                APIKeyName = item.utp.APIKeyName,
                                APIKeyID = item.utp.APIKeyID,
                                APIKeyValue = item.utp.APIKeyValue,
                                ConfimationCode = item.utp.ConfimationCode,
                                IsActive = item.utp.IsActive,
                                IsDeleted = item.utp.IsDeleted,
                            });
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.ThirdPartyModel - LoadAPIKeys", Ex.Message);
            }
            return _list;
        }

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}