using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Globalization;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Linq;
using SDGApp.Helpers;
using System.Configuration;

namespace SDGApp.Models
{
    public class BaseModel
    {
        public DateTime? CurrentDate
        {
            get
            {
                return DateTime.Now;

            }
        }

        #region Common Methods

        public List<SelectListItem> GetDDL(DataTable dt, String textField, String valueField)
        {
            var selectList = new List<SelectListItem>();
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    selectList.Add(new SelectListItem
                    {
                        Text = GetStringValue(dr[textField]),
                        Value = GetStringValue(dr[valueField])
                    });
                }
            }
            catch { }
            return selectList;
        }

        public Boolean CheckFolder(String FolderPath)
        {
            Boolean NewFolder = false;
            try
            {
                if (!System.IO.Directory.Exists(FolderPath))
                {
                    NewFolder = true;
                    System.IO.Directory.CreateDirectory(FolderPath);
                }
            }
            catch (Exception Ex)
            {
                WriteLog("UserModel - CheckFolder", Ex.Message);
            }
            return NewFolder;
        }

        public String GetFolderNameID(Int64 ID)
        {
            String FolderName = "";
            try
            {
                Int64 FolderSize = 5000;
                Int64 CurrentFolder = 0;
                Int64 CF = ((ID / FolderSize) + 1) * FolderSize;
                if (ID == (CF - FolderSize))
                {
                    CurrentFolder = (CF - FolderSize);
                }
                else
                {
                    CurrentFolder = CF;
                }
                FolderName = Convert.ToString(CurrentFolder);

            }
            catch { }
            return FolderName;
        }

        public Boolean HasWriteAccessToFolder(String FolderPath)
        {
            try
            {
                String FullPath = FolderPath + DateTime.Now.Ticks.ToString() + ".txt";
                using (FileStream fstream = new FileStream(FullPath, FileMode.Create))
                {
                    using (TextWriter writer = new StreamWriter(fstream))
                    {
                        // try catch block for write permissions 
                        writer.WriteLine("access");
                    }
                }
                if (System.IO.File.Exists(FullPath))
                {
                    System.IO.File.Delete(FullPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        public String ReadHtmlFile(String htmlFilePath)
        {
            StringBuilder store = new StringBuilder();

            try
            {
                using (StreamReader htmlReader = new StreamReader(htmlFilePath))
                {
                    String line;
                    while ((line = htmlReader.ReadLine()) != null)
                    {
                        store.Append(line);
                    }
                }
            }
            catch { }

            return store.ToString();
        }

        public void ResizeImageRation(String OriginalFile, String NewFile, Int32 NewWidth, Int32 MaxHeight, Boolean OnlyResizeIfWider)
        {
            try
            {
                System.Drawing.Image FullsizeImage = System.Drawing.Image.FromFile(OriginalFile);

                // Prevent using images internal thumbnail
                FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
                FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

                if (OnlyResizeIfWider)
                {
                    if (FullsizeImage.Width <= NewWidth)
                    {
                        NewWidth = FullsizeImage.Width;
                    }
                }

                int NewHeight = FullsizeImage.Height * NewWidth / FullsizeImage.Width;
                if (NewHeight > MaxHeight)
                {
                    // Resize with height instead
                    NewWidth = FullsizeImage.Width * MaxHeight / FullsizeImage.Height;
                    NewHeight = MaxHeight;
                }

                System.Drawing.Image NewImage = FullsizeImage.GetThumbnailImage(NewWidth, NewHeight, null, IntPtr.Zero);

                // Clear handle to original file so that we can overwrite it if necessary
                FullsizeImage.Dispose();

                // Save resized picture
                NewImage.Save(NewFile);
            }
            catch { }
        }

        public Int32 SkipRecords(Int32 pageSize, Int32 pageNumber)
        {
            Int32 returnVal = 0;
            try
            {
                if (pageNumber > 0)
                {
                    returnVal = pageSize * (pageNumber - 1);
                }
            }
            catch (Exception ex)
            {
            }
            return returnVal;
        }

        #endregion

        #region Get Null Value from Object

        public object GetNullValue(object obj)
        {
            object defaultValue = null;
            try
            {
                if (!(obj == null || obj == DBNull.Value))
                    defaultValue = Convert.ToString(obj);
            }
            catch
            {
            }
            return defaultValue;
        }

        #endregion

        #region Get Integer Value from Object

        public int GetIntegerValue(object obj)
        {
            return GetIntegerValue(obj, 0);
        }
        public int GetIntegerValue(object obj, int defaultReturnValue)
        {
            try
            {
                defaultReturnValue = Convert.ToInt32(obj);
            }
            catch
            {

            }
            return defaultReturnValue;
        }
        public long GetLontValue(object obj)
        {
            return GetLontValue(obj, 0);
        }
        public long GetLontValue(object obj, long defaultReturnValue)
        {
            try
            {
                defaultReturnValue = Convert.ToInt64(obj);
            }
            catch
            {

            }
            return defaultReturnValue;
        }

        #endregion

        #region Get Decimel Value from Object

        public decimal GetDecimelValue(object obj)
        {
            return GetDecimelValue(obj, 0);
        }
        public decimal GetDecimelValue(object obj, decimal defaultReturnValue)
        {
            try
            {
                defaultReturnValue = Convert.ToDecimal(obj);
            }
            catch
            {

            }
            return defaultReturnValue;
        }
        public decimal GetDecimalPlaceValue(object Value)
        {
            return GetDecimalPlaceValue(GetDecimelValue(Value), 2);

        }
        public decimal GetDecimalPlaceValue(decimal Value, int DecimalPlace)
        {
            try
            {
                return decimal.Round(Value, DecimalPlace);
            }
            catch
            {
            }
            return 0.00M;

        }        

        #endregion

        #region Get Boolean Value from Object

        public bool GetBooleanValue(object obj)
        {
            return GetBooleanValue(obj, false);
        }
        public bool GetBooleanValue(object obj, bool defaultValue)
        {
            try
            {
                if (!(obj == null || obj == DBNull.Value))
                    defaultValue = Convert.ToBoolean(obj);
            }
            catch
            {
            }
            return defaultValue;
        }

        public Boolean? GetNullableBooleanValue(object obj)
        {
            Boolean? defaultValue = null;
            try
            {
                if (!(obj == null || obj == DBNull.Value))
                    defaultValue = Convert.ToBoolean(obj);
            }
            catch
            {
            }
            return defaultValue;
        }

        #endregion

        #region Get DateTime Value from Object

        public DateTime? GetDateTimeValue(object obj)
        {
            DateTime? defaultValue = null;
            try
            {
                if (!(obj == null || obj == DBNull.Value))
                    defaultValue = Convert.ToDateTime(obj,CultureInfo.InvariantCulture);
            }
            catch
            {
            }
            return defaultValue;
        }


        public DateTime GetNotNullDateTimeValue(object obj)
        {
            DateTime defaultValue = DateTime.Now;
            try
            {
                if (!(obj == null || obj == DBNull.Value))
                    defaultValue = Convert.ToDateTime(obj,CultureInfo.InvariantCulture);
            }
            catch
            {
            }
            return defaultValue;
        }

        #endregion

        #region Get String Value from Object

        public string GetStringValue(object obj)
        {
            return GetStringValue(obj, "");
        }
        public string GetStringValue(object obj, string defaultValue)
        {
            if (!(obj == null || obj == DBNull.Value))
                defaultValue = obj.ToString();
            return defaultValue;
        }

        #endregion

        #region Handle Session Data

        public void SetSessionValue(string sKey, object sValue)
        {
            HttpContext.Current.Session[sKey.ToLower()] = sValue;
        }
        public object GetSessionValue(string sKey)
        {
            return GetSessionValue(sKey, null);
        }
        public object GetSessionValue(string sKey, object oReturnValue)
        {
            try
            {
                if (HttpContext.Current.Session[sKey.ToLower()] == null) return oReturnValue;
                else return HttpContext.Current.Session[sKey.ToLower()];
            }
            catch
            {
                return oReturnValue;
            }
        }

        #endregion

        #region Handle Cookie Data

        public void SetCookieValue(string cookieKey, string value)
        {
            SetCookieValue(cookieKey, value, 1);
        }
        public void SetCookieValue(string cookieKey, string value, int days)
        {
            try
            {
                cookieKey = cookieKey.ToLower();
                HttpCookie Cookie = new HttpCookie(cookieKey);
                Cookie.Values.Add(cookieKey, value);
                Cookie.Expires = DateTime.Now.AddDays(days);
                HttpContext.Current.Response.Cookies.Add(Cookie);
            }
            catch { }
        }
        public string GetCookieValue(string cookieKey)
        {
            string value = "";
            try
            {
                cookieKey = cookieKey.ToLower();
                HttpCookie Cookie = HttpContext.Current.Request.Cookies[cookieKey];
                if (Cookie != null)
                {
                    value = Cookie.Value;
                }
            }
            catch
            {
            }
            return value;
        }

        #endregion

        #region :: Error Log

        public void WriteLog(string subject, string message,Boolean isError=true)
        {
            try
            {
                string logPath = HttpContext.Current.Server.MapPath("~/Content/ErrorLogs/"+DateTime.Now.ToString("yyyy-MM-dd")+"_Log.txt");
                if(!isError)
                {
                    logPath = HttpContext.Current.Server.MapPath("~/Content/SimpleLogs/" + DateTime.Now.ToString("yyyy-MM-dd") + "_Log.txt");
                }
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(logPath, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss") + " - " + subject + " ::: " + message + "\n");
                }
            }
            catch (Exception ex)
            {
            }
        }

        //THIS IS LOGGING THE DEVICE DELETION
        public void DeviceDeleteLog(string subject, string message, string guid)
        {
            string DDlogPath = HttpContext.Current.Server.MapPath("~/Content/DeviceDeleteLogs/" + DateTime.Now.ToString("yyyy-MM-dd") + "_DeviceDeleteLog.txt");
            try
            {
                using (StreamWriter ddsw = new StreamWriter(DDlogPath, true))
                {
                    ddsw.WriteLine("Subject:: " + subject + Environment.NewLine+ "GUID:: " + guid+Environment.NewLine+ "Message:: " + message);
                    ddsw.WriteLine("=================End End=======================");
                }

            }
            catch(Exception ex)
            { }
        }


        #endregion

        #region :: PASSWORD ECRYPTION AND DECRYPTION

        public string Encrypt(string clearText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(GlobalConstants.EncryptionKey, new byte[] { 0x65, 0x3d, 0x54, 0x9d, 0x76, 0x49, 0x76, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x61 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(GlobalConstants.EncryptionKey, new byte[] { 0x65, 0x3d, 0x54, 0x9d, 0x76, 0x49, 0x76, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x61 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        #endregion

        private Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public Boolean WriteUserMeasurement(int UserId, string JsonFileName,string FolderDate, object json)        {            Boolean Result = false;            try            {
                if (UserId > 0 && !string.IsNullOrEmpty(JsonFileName) && json != null)
                {
                    String directiorypath = "~/Content/Measurement/" + UserId + "/" + FolderDate;
                    String filepath = "~/Content/Measurement/" + UserId + "/" + FolderDate + "/" + JsonFileName;

                    string rootdir = System.Web.HttpContext.Current.Server.MapPath(filepath);

                    string directorymappath = System.Web.HttpContext.Current.Server.MapPath(directiorypath);


                    // If directory does not exist, create it. 

                    if (!Directory.Exists(directorymappath))
                    {
                        Directory.CreateDirectory(directorymappath);
                    }

                    // serialize JSON directly to a file
                    using (StreamWriter file = File.CreateText(rootdir))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(file, json);
                    }

                    Result = true;
                }


            }            catch (Exception Ex)            {
                String subject = "Mesurment Data Storing Failed";
                SendMailFailReport(subject,Ex.Message);

                //WriteLog("SDGApp.Models.BaseModel - WriteUserMeasurement", Ex.Message);
            }            return Result;        }


        public Boolean SendMailFailReport(String Subject, String ErrorMessage)
        {
            Boolean isSend = false;

            String MessageSubject = Subject;

            try
            {
                MailHelper mailHelper = new MailHelper();
                MailService mailService = new MailService();

                Dictionary<string, string> objDict = new Dictionary<string, string>();
                objDict.Add("ToYear", DateTime.Now.Year.ToString());
                objDict.Add("ErrorMessage", ErrorMessage);

                var Body = mailHelper.ReadHtmlFile("ErrorNotification.html", objDict);

                string ErrorSendEmils = ConfigurationManager.AppSettings["ErrorReportSend"];

                if (!String.IsNullOrEmpty(ErrorSendEmils))
                {
                    var splitString = ErrorSendEmils.Split(',').ToList();

                    isSend= mailService.SendBulkMessage(splitString, Body, MessageSubject);

                }
            }
            catch (Exception Ex)
            {

            }

            return isSend;
        }
    }
}