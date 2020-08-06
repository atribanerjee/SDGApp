using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace SDGApp.Models
{
    public class ActivityModel : BaseModel
    {
        public String RegisterNewPlannedActivity(string plannedActivitiesName, DateTime DefaultDateTime)
        {
            String retresult = "";
            try
            {
                if (SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_INSERT_PlannedActivities", plannedActivitiesName, DefaultDateTime) > 0)
                {
                    retresult = "Success";
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.ActivityModel - RegisterNewPlannedActivity", Ex.Message);
                retresult = "Failed ( Exception - " + Ex.Message + " )";
            }
            return retresult;
        }

        public String RegisterNewRecognisedActivity(string recognisedActivitiesName)
        {
            String retresult = "";
            try
            {
                if (SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_INSERT_RecognisedActivities", recognisedActivitiesName) > 0)
                {
                    retresult = "Success";
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.ActivityModel - RegisterNewRecognisedActivity", Ex.Message);
                retresult = "Failed ( Exception - " + Ex.Message + " )";
            }
            return retresult;
        }

        //public Boolean SaveEvent(DateTime startDateTime, DateTime endDateTime, string title)
        //{
        //    Boolean Result = false;
        //    try
        //    {
        //        UserModel UM = new UserModel();

        //        Int32 userID = UM.GetLoggedInUserInfo().UserID;

        //        if (SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_SaveEvent", userID, startDateTime, endDateTime, title) > 0)
        //        {
        //            Result = true;
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        WriteLog("SDGApp.Models.ActivityModel - SaveEvent", Ex.Message);
        //    }
        //    return Result;
        //}

        public Int32 GetNextEventID()
        {
            Int32 Result = 0;
            DataSet DS = null;
            try
            {
                using (DS = new DataSet())
                {
                    DS = (SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), CommandType.StoredProcedure, "USP_GetNextEventID"));
                    if (DS != null && DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0)
                    {
                        Result = GetIntegerValue(DS.Tables[0].Rows[0]["UserActivityDetailsID"]);
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.ActivityModel - GetNextEventID", Ex.Message);
            }
            return Result;
        }

        //public Boolean UpdateEvent(DateTime startDateTime, DateTime endDateTime, int id)
        //{
        //    Boolean Result = false;
        //    try
        //    {
        //        if (SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_UpdateEvent", id, startDateTime, endDateTime) > 0)
        //        {
        //            Result = true;
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        WriteLog("SDGApp.Models.ActivityModel - UpdateEvent", Ex.Message);
        //    }
        //    return Result;
        //}

        public Boolean SaveUserActivity(int userID, int deviceID, DateTime startDateTime, DateTime endDatetime, int TagID)
        {
            Boolean retresult = false;
            try
            {
                if (SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_INSERT_UserActivityDetails", userID, deviceID, startDateTime, endDatetime, TagID) > 0)
                {
                    retresult = true;
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.ActivityModel - SaveUserActivity", Ex.Message);
            }
            return retresult;
        }

        public ActivityViewModel GetActivityList()
        {
            ActivityViewModel model = new ActivityViewModel();
            DataSet DS = null;
            try
            {
                using (DS = new DataSet())
                {
                    DS = (SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), CommandType.StoredProcedure, "USP_GET_ALL_PlannedActivities"));
                    if (DS != null && DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0)
                    {
                        List<PlannedActivities> _list = new List<PlannedActivities>();

                        foreach (DataRow dr in DS.Tables[0].Rows)
                        {
                            _list.Add(new PlannedActivities { PlannedActivitiesID = GetIntegerValue(dr["PlannedActivitiesID"]), PlannedActivitiesName = GetStringValue(dr["PlannedActivitiesName"]) });
                        }

                        if (_list.Count > 0)
                        {
                            model.PlannedActivityList = _list;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.ActivityModel - GetActivityList", Ex.Message);
            }
            return model;
        }

        //public List<calendarEvents> GetAllSavedEventsByUserID(Int32 userID)
        //{
        //    List<calendarEvents> model = new List<calendarEvents>();
        //    DataSet DS = null;
        //    try
        //    {
        //        using (DS = new DataSet())
        //        {
        //            DS = (SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), "USP_GET_ALL_SavedActivitiesByUserID", userID));
        //            if (DS != null && DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0)
        //            {
        //                List<calendarEvents> _list = new List<calendarEvents>();

        //                foreach (DataRow dr in DS.Tables[0].Rows)
        //                {
        //                    _list.Add(new calendarEvents { id = GetIntegerValue(dr["UserActivityDetailsID"]), title = GetStringValue(dr["PlannedActivitiesName"]), start = GetNotNullDateTimeValue(dr["StartDateTime"]).ToString("yyyy-MM-dd HH:mm:ss"), end = GetNotNullDateTimeValue(dr["EndDatetime"]).ToString("yyyy-MM-dd HH:mm:ss") });
        //                }

        //                if (_list.Count > 0)
        //                {
        //                    model = _list;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        WriteLog("SDGApp.Models.ActivityModel - GetAllSavedEventsByUserID", Ex.Message);
        //    }
        //    return model;
        //}

        public bool AddNewActivity(PlannedActivities PA)
        {
            bool Result = false;
            try
            {
                var value = SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_INSERT_PlannedActivities", PA.PlannedActivitiesName, PA.DefaultDateTime);
                if (value > 0)
                {
                    Result = true;
                }
            }
            catch (Exception Ex)
            {

                WriteLog("SDGApp.Models.ActivityModel - AddNewActivity", Ex.Message);
            }
            return Result;

        }

        public List<PlannedActivities> GetAllActivityList(PlannedActivities PA)
        {
            List<PlannedActivities> _List = new List<PlannedActivities>();
            DataSet DS = null;
            try
            {
                using (DS = new DataSet())
                {
                    DS = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), "USP_GetAllActivityList", PA.PageNumber, PA.PageSize);
                    if (DS != null && DS.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow DR in DS.Tables[0].Rows)
                        {
                            PlannedActivities model = new PlannedActivities();

                            model.PlannedActivitiesID = GetIntegerValue(DR["PlannedActivitiesID"]);
                            model.PlannedActivitiesName = GetStringValue(DR["PlannedActivitiesName"]);
                            model.TotalRecords = GetIntegerValue(DS.Tables[1].Rows[0]["TotalCount"]);
                            model.PageSize = PA.PageSize;
                            model.PageNumber = PA.PageNumber;
                            model.DefaultDateTime = GetDateTimeValue(DR["DefaultDateTime"]) ?? DateTime.Now;
                            _List.Add(model);
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - GetAllActivityList", Ex.Message);
            }
            return _List;
        }


        public PlannedActivities GetActivityDetailByUserID(int PlanedActivityID)
        {

            DataSet DS = null;
            PlannedActivities model = new PlannedActivities();
            try
            {
                using (DS = new DataSet())
                {
                    DS = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), "USP_GetActivityDetailByActivityID", PlanedActivityID);
                    if (DS != null && DS.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow DR in DS.Tables[0].Rows)
                        {
                            model.PlannedActivitiesID = GetIntegerValue(DR["PlannedActivitiesID"]);
                            model.PlannedActivitiesName = GetStringValue(DR["PlannedActivitiesName"]);
                            model.DefaultDateTime = GetDateTimeValue(DR["DefaultDateTime"]) ?? DateTime.Now;

                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - GetActivityDetailByUserID", Ex.Message);
            }
            return model;
        }

        public bool ActivityDetailUpdate(PlannedActivities pam)
        {
            bool result = false;
            try
            {
                int res = SqlHelper.ExecuteNonQuery(
                    GlobalConstants.DBConn(),
                    "USP_ActivityDetailUpdate",
                   GetIntegerValue(pam.PlannedActivitiesID),
                   GetStringValue(pam.PlannedActivitiesName),
                   GetNotNullDateTimeValue(pam.DefaultDateTime).ToShortTimeString()
                   );
                if (res > 0)
                {
                    result = true;
                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - ActivityDetailUpdate", Ex.Message);
            }

            return result;
        }

        public bool DeleteActivitybyActivityID(int PlanedActivityID)
        {
            bool Result = false;
            try
            {
                int Val = SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_DeleteActivitybyActivityID", PlanedActivityID);
                if (Val > 0)
                {
                    Result = true;
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - DeleteActivitybyActivityID", Ex.Message);
            }

            return Result;
        }

        //public bool DeleteEventbyActivityID(int ID)
        //{
        //    bool Result = false;
        //    try
        //    {
        //        int Val = SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_DeleteEventbyActivityID", ID);
        //        if (Val > 0)
        //        {
        //            Result = true;
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        WriteLog("SDGApp.Models.UserModel - DeleteActivitybyActivityID", Ex.Message);
        //    }

        //    return Result;
        //}



    }

}