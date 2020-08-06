using System;
using System.Configuration;
using System.Web;

namespace SDGApp
{
    public class GlobalConstants
    {
        public static Int32 DefaultSessionTimeout = 20;
        public static String BaseUrl = Convert.ToString(ConfigurationManager.AppSettings["BaseUrl"]);
        public static Int32 PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        public static string MailSettings = ConfigurationManager.AppSettings["MailSettings"];
        public static String MailTemplatePath = HttpContext.Current.Server.MapPath("~/Content/email-templates/");
        public static String BaseUrlBloodPresurwe = Convert.ToString(ConfigurationManager.AppSettings["BaseUrl"]);
        public static string EncryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
        public static String DBConn()
        {
               return ConfigurationManager.ConnectionStrings["SDGAppDBContext"].ToString();
            //return ConfigurationManager.ConnectionStrings["SDGAppDBContext"].ToString();
        }
    }
}