using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SDGApp.Helpers
{
    public class MailHelper
    {
        private String BaseUrl;
        private String MailSettings;
        private String MailTemplatePath;

        MailService MS;
        
        public MailHelper()
        {
            BaseUrl = GlobalConstants.BaseUrl;
            MailSettings = GlobalConstants.MailSettings;
            MailTemplatePath = GlobalConstants.MailTemplatePath;

            MS = new MailService();
        }

        public String ReadHtmlFile(String TemplatePath, Dictionary<String, String> obj)
        {
            String content = String.Empty;
            try
            {
                var fileStream = new FileStream(Path.Combine(MailTemplatePath, TemplatePath), FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    content = streamReader.ReadToEnd();
                }

                foreach (KeyValuePair<String, String> kv in obj)
                {
                    content = content.Replace("@@" + kv.Key + "@@", kv.Value);
                }
            }

            catch (Exception Ex)
            {
                throw Ex;
            }

            return content;
        }
        public bool SendEmail(string Subject, string To, string TemplatePath, Dictionary<String, String> obj)
        {
            bool Result = false;
            string Body = string.Empty;
            try
            {
                Body = ReadHtmlFile(TemplatePath, obj);

                Result = MS.SendMail(To, Subject, Body);

            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return Result;
        }
    }
}