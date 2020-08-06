using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace SDGApp
{
    public class MailService
    {
        public MailService() { }

        public Boolean Send(String Email, String Subject, String Body, String MailSettings, String FileName = "", bool IsAttachment = false, String FileType = "")
        {
            Boolean Result = false;
            try
            {
                String[] settings = MailSettings.Split('|');

                String From = settings[0];
                String Host = settings[1];
                Int32 Port = Convert.ToInt32(settings[2]);
                String AccessKeyID = settings[3];
                String SecretAccessKey = settings[4];
                Int32 SSL = Convert.ToInt32(settings[5]);


                MailMessage email = new MailMessage();
                MailAddress mFrom = new MailAddress(From);

                email.Subject = Subject;
                email.IsBodyHtml = true;
                email.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                email.Body = Body;
                email.From = mFrom;
                email.To.Add(Email.ToLower().Replace(" ", ""));

                if (IsAttachment)
                {
                    email.Attachments.Add(new Attachment(FileName, FileType));
                }

                SmtpClient smtp = new SmtpClient();
                smtp.EnableSsl = SSL == 1;
                smtp.UseDefaultCredentials = false;

                NetworkCredential credential = new NetworkCredential(AccessKeyID, SecretAccessKey);

                smtp.Credentials = credential;
                smtp.Host = Host;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Port = Port;

                smtp.Send(email);

                Result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        #region :: Common Method for sending E-Mail 

        public Boolean SendMail(String ToEmail, String Subject, String HtmlBody)
        {
            BaseModel BM = new BaseModel();
            String Result = string.Empty;
            bool emailsent = false;
            try
            {
                String Email = String.Empty;


                if (!String.IsNullOrEmpty(ToEmail))
                {

                    var myMessage = new SendGrid.SendGridMessage();
                    if (ToEmail.Contains("@"))
                    {
                        Email = ToEmail;
                    }

                    string AdminEmail = ConfigurationManager.AppSettings["AdminEmail"];
                    string FromEmailDisplayName = ConfigurationManager.AppSettings["FromEmailDisplayName"];
                    string ApiKey = ConfigurationManager.AppSettings["ApiKey"];

                    if (!String.IsNullOrEmpty(Email))
                    {
                        myMessage.AddTo(Email);
                        myMessage.From = new MailAddress(AdminEmail, FromEmailDisplayName);
                        myMessage.Subject = Subject;
                        myMessage.Html = HtmlBody;
                        var transportWeb = new SendGrid.Web(ApiKey);
                        transportWeb.DeliverAsync(myMessage);

                        Result = "E-Mail sent successfully to - " + ToEmail;
                        emailsent = true;

                        BM.WriteLog("SGDApp.Models.MailService - SendEmail", Result);

                    }
                }
            }
            catch (Exception Ex)
            {
                Result = "E-Mail sent failed";
                emailsent = false;
                BM.WriteLog("SGDApp.Models.MailService - SendEmail", "ToEmail - " + ToEmail + ", Subject - " + Subject + ", HtmlBody - " + HtmlBody + ", Result - " + Result + "=====" + Ex.StackTrace);
            }
            return emailsent;
        }


        public Boolean SendMailMEssage(String ToEmail, string ToCC, String Subject, String HtmlBody)
        {
            BaseModel BM = new BaseModel();
            String Result = string.Empty;
            bool emailsent = false;
            try
            {
                String Email = String.Empty;


                if (!String.IsNullOrEmpty(ToEmail))
                {

                    var myMessage = new SendGrid.SendGridMessage();
                    if (ToEmail.Contains("@"))
                    {
                        Email = ToEmail;
                    }

                    string AdminEmail = ConfigurationManager.AppSettings["AdminEmail"];
                    string FromEmailDisplayName = ConfigurationManager.AppSettings["FromEmailDisplayName"];
                    string ApiKey = ConfigurationManager.AppSettings["ApiKey"];

                    if (!String.IsNullOrEmpty(Email))
                    {
                        myMessage.AddTo(Email);
                        if (!string.IsNullOrEmpty(ToCC))
                        {
                            myMessage.AddCc(ToCC);
                        }
                        myMessage.From = new MailAddress(AdminEmail, FromEmailDisplayName);
                        myMessage.Subject = Subject;
                        myMessage.Html = HtmlBody;
                        var transportWeb = new SendGrid.Web(ApiKey);
                        transportWeb.DeliverAsync(myMessage);

                        Result = "E-Mail sent successfully to - " + ToEmail;
                        emailsent = true;

                        BM.WriteLog("SGDApp.Models.MailService - SendMailMEssage", Result);

                    }
                }
            }
            catch (Exception Ex)
            {
                Result = "E-Mail sent failed";
                emailsent = false;
                BM.WriteLog("SGDApp.Models.MailService - SendMailMEssage", "ToEmail - " + ToEmail + ", Subject - " + Subject + ", HtmlBody - " + HtmlBody + ", Result - " + Result + "=====" + Ex.StackTrace);
            }
            return emailsent;
        }




        // SEND MESSAGE IN BULK EMAIL

        public Boolean SendMailMEssageBulk(UserMessageViewModel model, String HtmlBody)
        {
            BaseModel BM = new BaseModel();
            String Result = string.Empty;
            bool emailsent = false;
            string dirPath = string.Empty;
            string filePath = string.Empty;
            string extension = string.Empty;

            try
            {
                String Email = String.Empty;

                var myMessage = new SendGrid.SendGridMessage();

                if (model.lstemilto != null && model.lstemilto.Count > 0)
                {
                    myMessage.AddTo(model.lstemilto); //adding multiple TO Email Id                   
                }


                if (model.lstemilCc != null && model.lstemilCc.Count > 0)
                {
                    foreach (string CcEMailId in model.lstemilCc)
                    {
                        var ccmailid = new MailAddress(CcEMailId);
                        myMessage.AddCc(ccmailid); //adding multiple TO Email Id
                    }
                }

                #region [ MAIL ATTACHMENT IF REQUIRED ]

                if (model.FileAttachments != null)
                {
                    if (model.FileAttachments[0] != null)
                    {
                        dirPath = HttpContext.Current.Server.MapPath("~/Content/EmailAttachments");
                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }

                        foreach (HttpPostedFileBase file in model.FileAttachments)
                        {
                            if (file.ContentLength == 0)
                                continue;

                            extension = Path.GetExtension(file.FileName);
                            filePath = Path.Combine(dirPath, file.FileName);

                            file.SaveAs(filePath);

                            //myMessage.AddAttachment(file.InputStream, file.FileName);
                        }
                    }
                }

                

                #endregion

                string AdminEmail = ConfigurationManager.AppSettings["AdminEmail"];
                string FromEmailDisplayName = ConfigurationManager.AppSettings["FromEmailDisplayName"];
                string ApiKey = ConfigurationManager.AppSettings["ApiKey"];


                myMessage.From = new MailAddress(AdminEmail, FromEmailDisplayName);
                //myMessage.Subject = model.MessageSubject;
                myMessage.Subject = "New Mail Notification";
                myMessage.Html = HtmlBody;
                var transportWeb = new SendGrid.Web(ApiKey);
                transportWeb.DeliverAsync(myMessage);

                //Result = "E-Mail sent successfully to - " + ToEmail;
                emailsent = true;

                BM.WriteLog("SGDApp.Models.MailService - SendEmail", Result);

                //}
                //}
            }
            catch (Exception Ex)
            {
                Result = "E-Mail sent failed";
                emailsent = false;
                //BM.WriteLog("SGDApp.Models.MailService - SendMailMEssageBulk", "ToEmail - " + ToEmail + ", Subject - " + Subject + ", HtmlBody - " + HtmlBody + ", Result - " + Result + "=====" + Ex.StackTrace);
            }
            return emailsent;
        }


        public Boolean SendBulkMessage(List<String>MessageTo, String HtmlBody, String Subject, List<String> MessageCC=null)
        {
            BaseModel BM = new BaseModel();
            String Result = string.Empty;
            bool emailsent = false;

            try
            {
                String Email = String.Empty;

                var myMessage = new SendGrid.SendGridMessage();

                if (MessageTo != null && MessageTo.Count > 0)
                {
                    myMessage.AddTo(MessageTo); //adding multiple TO Email Id                   
                }

                if (MessageCC != null && MessageCC.Count > 0)
                {
                    foreach (string CcEMailId in MessageCC)
                    {
                        var ccmailid = new MailAddress(CcEMailId);
                        myMessage.AddCc(ccmailid); //adding multiple TO Email Id
                    }
                }

                string AdminEmail = ConfigurationManager.AppSettings["AdminEmail"];
                string FromEmailDisplayName = ConfigurationManager.AppSettings["FromEmailDisplayName"];
                string ApiKey = ConfigurationManager.AppSettings["ApiKey"];


                myMessage.From = new MailAddress(AdminEmail, FromEmailDisplayName);
                myMessage.Subject = Subject;
                myMessage.Html = HtmlBody;
                var transportWeb = new SendGrid.Web(ApiKey);
                transportWeb.DeliverAsync(myMessage);
                
                emailsent = true;

            }
            catch (Exception Ex)
            {
                Result = "E-Mail sent failed";
                emailsent = false;
            }
            return emailsent;
        }

        #endregion
    }
}
