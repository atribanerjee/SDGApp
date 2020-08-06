using System;

namespace SDGApp.Models
{
    public class DeviceModel : BaseModel
    {
        public String RegisterNewDevice(string serialNumber, string deviceIMEI)
        {
            String retresult = "";
            try
            {
                if (SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_INSERT_DeviceDetail", serialNumber, deviceIMEI, 1) > 0)
                {
                    retresult = "Success";
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceModel - RegisterNewDevice", Ex.Message);
                retresult = "Failed ( Exception - " + Ex.Message + " )";
            }
            return retresult;
        }
    }
}