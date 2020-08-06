using Newtonsoft.Json;
using SDGApp.ViewModel;
using SDGAppDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static SDGApp.Helpers.SDGUtilities;

namespace SDGApp.Models
{
    public class DashboardModel : UserModel
    {

        public Boolean SaveWidgetStyle(String WidgetDtls, int UserId)
        {
            Boolean result = false;
            DashboardViewModel model = new DashboardViewModel();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    model.ListWidgetDetails = JsonConvert.DeserializeObject<List<WidgetDetails>>(WidgetDtls);

                    if (model.ListWidgetDetails.Count > 0 && model.ListWidgetDetails != null)
                    {
                        foreach (var item in model.ListWidgetDetails)
                        {
                            var existuser = (from ws in db.WidgetStyle
                                             where ws.FkUserID == UserId && ws.WidgetName.ToLower() == item.WidgetName.ToLower()
                                             select ws).FirstOrDefault();

                            var existwidgetid = (from wm in db.WidgetMaster
                                                 where wm.WidgetName.ToLower() == item.WidgetName.ToLower()
                                                 select wm.ID).FirstOrDefault();

                            var existtabid = (from tm in db.TabMaster
                                              where tm.FKUserId == UserId && tm.TabName.ToLower() == item.TabName.ToLower()
                                              select tm.ID).FirstOrDefault();

                            if (existuser != null)
                            {
                                //User Exist
                                if (existtabid > 0 && existwidgetid > 0)
                                {

                                    existuser.FKTabId = existtabid;
                                    existuser.Height = item.Height;
                                    existuser.Width = item.Width;
                                    existuser.Position = item.Position;

                                    db.Entry(existuser).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();

                                    result = true;
                                }
                            }
                            else
                            {
                                // User Not Exist

                                if (existtabid > 0 && existwidgetid > 0)
                                {
                                    var entity = new SDGAppDB.POCO.WidgetStyle();

                                    entity.FkUserID = UserId;
                                    entity.FKWidgetId = existwidgetid;
                                    entity.WidgetName = item.WidgetName;
                                    entity.FKTabId = existtabid;
                                    entity.Height = item.Height;
                                    entity.Width = item.Width;
                                    entity.Position = item.Position;
                                    entity.CreatedDateTime = DateTime.Now;
                                    db.WidgetStyle.Add(entity);
                                    db.SaveChanges();

                                    result = true;
                                }
                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("SDGApp.Models.DashboardModel - SaveWidgetStyle", ex.Message);

            }

            return result;
        }

        public DashboardViewModel GetUserWidgetStyle(int UserId)
        {
            DashboardViewModel model = new DashboardViewModel();
            List<WidgetDetails> lstwidgetDetails = new List<WidgetDetails>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entitywidgetdtls = (from ws in db.WidgetStyle
                                            where ws.FkUserID == UserId
                                            orderby ws.Position descending,ws.FKTabId
                                            select ws).ToList();

                    if (entitywidgetdtls != null && entitywidgetdtls.Count > 0)
                    {
                        foreach (var item in entitywidgetdtls)
                        {
                            var tabname = db.TabMaster.Where(x => x.ID == item.FKTabId).FirstOrDefault();

                            if (tabname != null)
                            {
                                WidgetDetails widgetDetails = new WidgetDetails();

                                widgetDetails.WidgetName = item.WidgetName;
                                widgetDetails.TabName = tabname.TabName;
                                widgetDetails.Height = item.Height;
                                widgetDetails.Width = item.Width;
                                widgetDetails.Position = item.Position;

                                lstwidgetDetails.Add(widgetDetails);
                            }
                        }
                    }

                    if (lstwidgetDetails != null && lstwidgetDetails.Count > 0)
                    {
                        model.ListWidgetDetails = lstwidgetDetails;
                    }




                }

            }
            catch (Exception ex)
            {
                WriteLog("SDGApp.Models.DashboardModel - GetUserWidgetStyle", ex.Message);

            }


            return model;
        }

        public Boolean AddNewTab(String TabNamevalue)
        {
            Boolean result = false;

            int UserId = GetLoggedInUserInfo().UserID;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    if (!String.IsNullOrEmpty(TabNamevalue))
                    {
                        var existTabName = (from tb in db.TabMaster
                                            where tb.TabName.ToLower() == TabNamevalue.ToLower() && tb.FKUserId == UserId
                                            select tb).FirstOrDefault();

                        if (existTabName == null)
                        {
                            var entity = new SDGAppDB.POCO.TabMaster();

                            entity.TabName = TabNamevalue;
                            entity.FKUserId = UserId;
                            entity.CreatedDateTime = DateTime.Now;
                            db.TabMaster.Add(entity);
                            db.SaveChanges();

                            result = true;
                        }


                    }
                }

            }
            catch (Exception ex)
            {
                WriteLog("SDGApp.Models.DashboardModel - AddNewTab", ex.Message);
            }

            return result;
        }

        public List<string> TabListByUserId(int UserID)
        {
            List<string> lst = new List<string>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    lst = (from tb in db.TabMaster
                           where tb.FKUserId == UserID
                           select tb.TabName).ToList();

                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DashboardModel - TabListByUserId", Ex.Message);
            }
            return lst;
        }

        public List<SelectListItem> WidgetNameList(int UserID)
        {
            List<SelectListItem> Listdata = new List<SelectListItem>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                   
                  var result = db.WidgetMaster.Where(wm => !db.WidgetStyle.Any(ws => ws.FKWidgetId == wm.ID && ws.FkUserID == UserID)).ToList();

                    if(result!=null && result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            Listdata.Add(new SelectListItem { Text = item.WidgetName, Value = item.ID.ToString() });
                             
                        }
                    }

                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DashboardModel - WidgetNameList", Ex.Message);
            }
            return Listdata;
        }

        public Boolean RemoveTab(String TabName, int UserId)
        {
            Boolean result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    if (!String.IsNullOrEmpty(TabName) && UserId > 0)
                    {

                        var tabdetails = db.TabMaster.Where(x => x.TabName.ToLower() == TabName.ToLower() && x.FKUserId == UserId).FirstOrDefault();

                        if (tabdetails != null)
                        {
                            // Select all the records to be deleted
                            var existTabinwidget = db.WidgetStyle.Where(x => x.FKTabId == tabdetails.ID).ToList();

                            if (existTabinwidget.Count > 0)
                            {
                                // Use Remove Range function to delete all records at once
                                db.WidgetStyle.RemoveRange(existTabinwidget);

                                // Save changes
                                db.SaveChanges();
                            }

                        }

                        var existTabName = (from tb in db.TabMaster
                                            where tb.TabName.Trim().ToLower() == TabName.Trim().ToLower() && tb.FKUserId == UserId
                                            select tb).FirstOrDefault();

                        if (existTabName != null)
                        {
                            db.TabMaster.Remove(existTabName);
                            db.SaveChanges();

                            result = true;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                WriteLog("SDGApp.Models.DashboardModel - RemoveTab", ex.Message);
            }

            return result;
        }

        public Boolean RemoveWidget(String widgetName, int UserId)
        {
            Boolean result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    if (!String.IsNullOrEmpty(widgetName) && UserId > 0)
                    {


                        var existWidgetName = (from ws in db.WidgetStyle
                                               where ws.WidgetName.Trim().ToLower() == widgetName.Trim().ToLower()
                                               && ws.FkUserID == UserId
                                               select ws).FirstOrDefault();

                        if (existWidgetName != null)
                        {
                            db.WidgetStyle.Remove(existWidgetName);
                            db.SaveChanges();

                            result = true;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                WriteLog("SDGApp.Models.DashboardModel - RemoveWidget", ex.Message);
            }

            return result;
        }
    }
}