using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SDGApp.Models
{
    public class TagCatagoriesModel : BaseModel
    {
        public List<TagsViewModel> GetAllTagCatagories(Int32 LoggedInUserID)
        {
            DataSet DS = null;
           List< TagsViewModel> _list = new List<TagsViewModel>();
            try
            {
                using (DS = new DataSet())
                {
                    DS = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), "USP_GetAllTagCatagories", LoggedInUserID);
                    if (DS != null && DS.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow DR in DS.Tables[0].Rows)
                        {
                            TagsViewModel model = new TagsViewModel();

                            model.Choice = GetStringValue(DR["Choice"]);
                            model.Max = GetStringValue(DR["Max"]);
                            model.Min = GetStringValue(DR["Min"]);
                            model.Prompt = GetStringValue(DR["Prompt"]);
                            model.TagName = GetStringValue(DR["TagName"]);
                            model.TagsID = GetIntegerValue(DR["TagID"]);
                            model.TypeID = GetIntegerValue(DR["TypeID"]);
                            model.TypeName = GetStringValue(DR["TypeName"]);
                            model.Description=GetStringValue(DR["Description"]);
                            model.TagDetailsID = GetIntegerValue(DR["TagDetailsID"]);
                            
                            _list.Add(model);
                        }
                       
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagCatagoriesModel - GetAllTagCatagories", Ex.Message);
            }
            return _list;
        }

        public bool SaveTagCatagories(int UserID,int TagID, String[] Fields)
        {
            bool Result = false;
            try
            {
                if (UserID > 0)
                {
                    for (int i = 0; i < Fields.Length; i++)
                    {
                        String Prompt = Fields[i];
                        int Val = SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_SaveTagCatagories", TagID, Prompt);
                        if (Val > 0)
                        {
                            Result = true;
                        }

                       
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagCatagoriesModel - SaveTagCatagories", Ex.Message);
            }
            return Result;
        }
    }
}