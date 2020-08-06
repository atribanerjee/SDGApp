using SDGApp.Models;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

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

                        BM.WriteLog("SGDApp.Models.BaseModel - SendEmail", Result);

                    }
                }
            }
            catch (Exception Ex)
            {
                Result = "E-Mail sent failed";
                emailsent = false;
                BM.WriteLog("SGDApp.Models.BaseModel - SendEmail", "ToEmail - " + ToEmail + ", Subject - " + Subject + ", HtmlBody - " + HtmlBody + ", Result - " + Result + "=====" + Ex.StackTrace);
            }
            return emailsent;
        }


        public Boolean SendMailMEssage(String ToEmail, string ToCC, String Subject, String HtmlBody)        {            BaseModel BM = new BaseModel();            String Result = string.Empty;            bool emailsent = false;            try            {                String Email = String.Empty;                if (!String.IsNullOrEmpty(ToEmail))                {                    var myMessage = new SendGrid.SendGridMessage();                    if (ToEmail.Contains("@"))                    {                        Email = ToEmail;                    }                    string AdminEmail = ConfigurationManager.AppSettings["AdminEmail"];                    string FromEmailDisplayName = ConfigurationManager.AppSettings["FromEmailDisplayName"];                    string ApiKey = ConfigurationManager.AppSettings["ApiKey"];                    if (!String.IsNullOrEmpty(Email))                    {                        myMessage.AddTo(Email);                        if (!string.IsNullOrEmpty(ToCC))                        {                            myMessage.AddCc(ToCC);                        }                        myMessage.From = new MailAddress(AdminEmail, FromEmailDisplayName);                        myMessage.Subject = Subject;                        myMessage.Html = HtmlBody;                        var transportWeb = new SendGrid.Web(ApiKey);                        transportWeb.DeliverAsync(myMessage);                        Result = "E-Mail sent successfully to - " + ToEmail;                        emailsent = true;                        BM.WriteLog("SGDApp.Models.BaseModel - SendEmail", Result);                    }                }            }            catch (Exception Ex)            {                Result = "E-Mail sent failed";                emailsent = false;                BM.WriteLog("SGDApp.Models.BaseModel - SendEmail", "ToEmail - " + ToEmail + ", Subject - " + Subject + ", HtmlBody - " + HtmlBody + ", Result - " + Result + "=====" + Ex.StackTrace);            }            return emailsent;        }
        #endregion
    }
}
