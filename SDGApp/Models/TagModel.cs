using System;
using SDGApp.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using SDGAppDB;

namespace SDGApp.Models
{
    public class TagModel : BaseModel
    {
        public bool AddNewTags(Int32 TagCategoryID, bool IsActivity, String TagName, String Desc, Int32 UserID, String[] Fields)
        {
            bool Result = false;
            try
            {
                Int32 TagID = GetIntegerValue(SqlHelper.ExecuteScalar(GlobalConstants.DBConn(), "USP_INSERT_TAGS", TagName, Desc, UserID));
                if (TagID > 0)
                {
                    for (int i = 0; i < Fields.Length; i++)
                    {
                        String Prompt = Fields[i];
                        Int32 TagType = GetIntegerValue(Fields[i + 1]);
                        String Min = "";
                        String Max = "";
                        String Choice = "";

                        if (TagType == 1)
                        {
                            Min = Fields[i + 2].Split('|')[0];
                            Max = Fields[i + 2].Split('|')[1];
                        }
                        else if (TagType == 2)
                        {
                            Choice = Fields[i + 2];
                        }


                        if (SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_INSERT_TAG_DETAILS", TagID, TagType, Prompt, Min, Max, Choice) > 0)
                        {
                            Result = true;
                        }

                        i = i + 2;
                    }
                }
            }
            catch (Exception Ex)
            {
                Result = false;
                WriteLog("SDGApp.Models.TagModel - AddNewTags", Ex.Message);
            }
            return Result;

        }

        public bool AddNewTagCategories(int tagID, int userID, string[] Prompts, string[] fields, String[] TagDetails)
        {
            bool Result = false;
            try
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    if (SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_TagCategories_Insert", tagID, userID, Prompts[i], fields[i], TagDetails[i]) > 0)
                    {
                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                Result = false;
                WriteLog("SDGApp.Models.TagModel - AddNewTagCategories", Ex.Message);
            }
            return Result;
        }

        //public bool EditMeasurement(int ID, string MeasurementValue)
        //{
        //    bool Result = false;
        //    try
        //    {
        //        Update Tags Table 1st

        //        if (GetIntegerValue(SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_UPDATE_MEASUREMENT", ID, MeasurementValue)) > 0)
        //        {

        //            Result = true;
        //        }

        //    }
        //    catch (Exception Ex)
        //    {
        //        Result = false;
        //        WriteLog("SDGApp.Models.TagModel - EditTagCategory", Ex.Message);
        //    }
        //    return Result;
        //}



        //public TagDetailsViewModel GetTagDetailsByUserActivityDetailsID(int? ID)
        //{
        //    TagDetailsViewModel model = new TagDetailsViewModel();
        //    DataSet DS = null;
        //    try
        //    {
        //        using (DS = new DataSet())
        //        {

        //            DS = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), "[USP_GET_TagDetails_BY_UserActivityDetailsID]", ID);
        //            if (DS != null && DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0)
        //            {
        //                model.TagID = GetIntegerValue(DS.Tables[0].Rows[0]["TagID"]);
        //                model.TagName = GetStringValue(DS.Tables[0].Rows[0]["TagName"]);
        //                model.Description = GetStringValue(DS.Tables[0].Rows[0]["Description"]);
        //                model.Prompt = GetStringValue(DS.Tables[0].Rows[0]["Prompt"]);
        //                model.TagDetailsID = GetIntegerValue(DS.Tables[0].Rows[0]["TagDetailsID"]);

        //                // fetch tag detail to bind Min, Max & Choice

        //                TagDetailsViewModel objModel = GetTagDetailByTagID(model.TagID);
        //                List<Field> _list = new List<Field>();
        //                foreach (var item in objModel.Fields)
        //                {
        //                        Field obj = new Field();
        //                        // Numeric Slider
        //                        if (item.FKTagTypeID == 1)
        //                        {
        //                            obj.Min = GetIntegerValue(DS.Tables[0].Rows[0]["Min"]);
        //                            obj.Max = GetIntegerValue(DS.Tables[0].Rows[0]["Max"]);
        //                        }
        //                        else if (item.FKTagTypeID == 2) // Choice
        //                        {
        //                            obj.Choice = GetStringValue(DS.Tables[0].Rows[0]["Choice"]);
        //                        }
        //                    //model.TagTypeID = item.FKTagTypeID;

        //                }
        //                model.Fields = objModel.Fields;
        //            }
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        WriteLog("SDGApp.Models.TagModel - GetTagCategoryDetailsByTagCategoryID", Ex.Message);
        //    }

        //    return model;
        //}



        //public bool AddMeasurement(int? ID,String MeasurementValue )
        //{
        //    bool Result = false;
        //    try
        //    {
        //            if (GetIntegerValue(SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_AddMeasurement", ID, MeasurementValue)) > 0)
        //            {
        //                Result = true;
        //            }
        //    }
        //    catch (Exception Ex)
        //    {
        //        Result = false;
        //        WriteLog("SDGApp.Models.TagModel - AddMeasurement", Ex.Message);
        //    }
        //    return Result;
        //}

        public TagDetailsViewModel GetTagDetailByTagID(int tagID)
        {
            TagDetailsViewModel model = new TagDetailsViewModel();
            DataSet dsTag = null;
            try
            {
                using (dsTag = new DataSet())
                {
                    dsTag = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), "USP_GET_TAG_DETAILS", tagID);
                    if (dsTag != null && dsTag.Tables[0] != null && dsTag.Tables[0].Rows.Count > 0)
                    {
                        model.TagID = GetIntegerValue(dsTag.Tables[0].Rows[0]["TagID"]);
                        model.TagName = GetStringValue(dsTag.Tables[0].Rows[0]["TagName"]);
                        model.Description = GetStringValue(dsTag.Tables[0].Rows[0]["Description"]);
                        model.TagDetailsID = GetIntegerValue(dsTag.Tables[0].Rows[0]["TagDetailsID"]);
                        model.TagDetailTypeList = GetTypeList();
                        model.TagCategoryTypeList = GetTagCatagoriesListDDL();
                        List<Field> _list = new List<Field>();

                        foreach (DataRow dr in dsTag.Tables[0].Rows)
                        {
                            _list.Add(new Field { TagDetailsID = GetIntegerValue(dr["TagDetailsID"]), FKTagTypeID = GetIntegerValue(dr["FKTagTypeID"]), Prompt = GetStringValue(dr["Prompt"]), Min = GetIntegerValue(dr["Min"]), Max = GetIntegerValue(dr["Max"]), Choice = GetStringValue(dr["Choice"]) });
                        }
                        model.Fields = _list;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagModel - GetTagDetailByTagID", Ex.Message);
            }
            return model;
        }

        public List<SelectListItem> GetTagsListDDL()
        {
            DataSet dsTag = null;
            List<SelectListItem> _list = new List<SelectListItem>();
            try
            {
                using (dsTag = new DataSet())
                {
                    dsTag = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), CommandType.StoredProcedure, "USP_GET_ALL_TAGS_LIST");
                    if (dsTag != null && dsTag.Tables[0] != null && dsTag.Tables[0].Rows.Count > 0)
                    {
                        _list.Add(new SelectListItem { Text = GetStringValue("---Select---"), Value = GetStringValue("0") });

                        foreach (DataRow dr in dsTag.Tables[0].Rows)
                        {
                            _list.Add(new SelectListItem { Text = GetStringValue(dr["TagName"]), Value = GetStringValue(dr["TagID"]) });
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagModel - GetTagsListDDL", Ex.Message);
            }
            return _list;
        }
        public List<SelectListItem> GetTagCatagoriesListDDL()
        {
            DataSet dsTagCategory = null;
            List<SelectListItem> _list = new List<SelectListItem>();
            try
            {
                using (dsTagCategory = new DataSet())
                {
                    dsTagCategory = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), CommandType.StoredProcedure, "USP_GetAllCategoryList");
                    if (dsTagCategory != null && dsTagCategory.Tables[0] != null && dsTagCategory.Tables[0].Rows.Count > 0)
                    {
                    //    _list.Add(new SelectListItem { Text = GetStringValue("---Select---"), Value = GetStringValue("0") });

                        foreach (DataRow dr in dsTagCategory.Tables[0].Rows)
                        {
                            _list.Add(new SelectListItem { Text = GetStringValue(dr["TagCategoryValue"]), Value = GetStringValue(dr["TagCategoryID"]) });
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagModel - GetTagCatagoriesListDDL", Ex.Message);
            }
            return _list;
        }

        public List<SelectListItem> GetTypeList()
        {
            DataSet dsTag = null;
            List<SelectListItem> _list = new List<SelectListItem>();
            try
            {
                using (dsTag = new DataSet())
                {
                    dsTag = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), CommandType.StoredProcedure, "USP_GetTypeList");
                    if (dsTag != null && dsTag.Tables[0] != null && dsTag.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in dsTag.Tables[0].Rows)
                        {
                            _list.Add(new SelectListItem { Text = GetStringValue(dr["TypeName"]), Value = GetStringValue(dr["TypeID"]) });
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagModel - AddNewTags", Ex.Message);
            }
            return _list;
        }

        public List<TagsViewModel> GetTagList(TagsViewModel model)
        {
            List<TagsViewModel> _List = new List<TagsViewModel>();
            DataSet DS = null;
            try
            {
                using (DS = new DataSet())
                {
                    DS = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), "USP_GetTagsList", model.PageNumber, model.PageSize);
                    if (DS != null && DS.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow DR in DS.Tables[0].Rows)
                        {
                            TagsViewModel TVM = new TagsViewModel();

                            TVM.TagsID = GetIntegerValue(DR["TagID"]);
                            TVM.TagName = GetStringValue(DR["TagName"]);
                            //TVM.TagcategoryName = GetStringValue(DR["CategoryName"]);
                            TVM.Description = GetStringValue(DR["Description"]);
                            TVM.TotalRecords = GetIntegerValue(DS.Tables[1].Rows[0]["TotalCount"]);
                            TVM.PageSize = model.PageSize;
                            TVM.PageNumber = model.PageNumber;

                            _List.Add(TVM);
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagModel - GetTagList", Ex.Message);
            }
            return _List;
        }

        public List<TagsCategoryViewModel> GetTagsCategoryList(TagsCategoryViewModel model)
        {
            List<TagsCategoryViewModel> lstchunk = new List<TagsCategoryViewModel>();
            List<TagsCategoryViewModel> lst = new List<TagsCategoryViewModel>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    lst = (from tc in db.TagCategories
                           where tc.IsDeleted == false
                           select new TagsCategoryViewModel
                           {
                               TagCategoryName = tc.TagCategoryValue,
                               TagID = tc.TagCategoryID,
                               PageSize = model.PageSize,
                               PageNumber = model.PageNumber,
                           }).ToList();


                    lstchunk = lst.OrderByDescending(q => q.TagID).Skip(SkipRecords(model.PageSize, model.PageNumber)).Take(model.PageSize).ToList();
                    lstchunk.ForEach(l => l.TotalRecords = lst.Count());
                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.CompanyModel - FetchCompanyList", Ex.Message);
            }
            return lstchunk;
        }


        public bool DeleteTagsbyTagID(int TagsID)
        {
            bool Result = false;
            try
            {
                int Val = SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_DeleteTagsbyTagID", TagsID);
                if (Val > 0)
                {
                    Result = true;
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagModel - DeleteTagsbyTagID", Ex.Message);
            }

            return Result;
        }

        public bool DeleteTagcatagoribyTagID(int TagCategoryID)
        {
            bool Result = false;
            try
            {
                int Val = SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_DeleteTagcatagoribyTagCatagoriesID", TagCategoryID);
                if (Val > 0)
                {
                    Result = true;
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagModel - DeleteTagcatagoribyTagID", Ex.Message);
            }

            return Result;
        }


        public bool EditTags( Int32 TagID, String TagName, String Desc, String[] Fields)
        {
            bool Result = false;
            try
            {
                // Update Tags Table 1st

                if (GetIntegerValue(SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_UPDATE_TAG", TagID, TagName, Desc)) > 0)
                {
                    // Update/Insert each Tag Details one by one

                    for (int i = 0; i < Fields.Length; i++)
                    {

                        int TagDetailsID = GetIntegerValue(Fields[i]);
                        String Prompt = Fields[i + 1];
                        Int32 TagType = GetIntegerValue(Fields[i + 2]);
                        String Min = "";
                        String Max = "";
                        String Choice = "";

                        if (TagType == 1)
                        {
                            Min = Fields[i + 3].Split('|')[0];
                            Max = Fields[i + 3].Split('|')[1];
                        }
                        else if (TagType == 2)
                        {
                            Choice = Fields[i + 3];
                        }

                        // Update existing TagDetails

                        if (TagDetailsID > 0 && SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_EditTagDetails", TagDetailsID, TagType, Prompt, Min, Max, Choice) > 0)
                        {
                            Result = true;
                        }
                        else if (SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_INSERT_TAG_DETAILS", TagID, TagType, Prompt, Min, Max, Choice) > 0) // Insert newly added field
                        {
                            Result = true;
                        }

                        i = i + 3;
                    }
                }
            }
            catch (Exception Ex)
            {
                Result = false;
                WriteLog("SDGApp.Models.TagModel - EditTags", Ex.Message);
            }
            return Result;

        }



        public bool InserNewTagCategories(TagsCategoryViewModel TCVM)
        {
            bool result = false;

            try
            {
                using(SDGAppDBContext db=new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = new SDGAppDB.POCO.TagCategories();
                    entity.FKTagID = TCVM.TagID;
                    entity.FKUserID = TCVM.FKUserId;
                    entity.FKTagDetailsID = TCVM.TagDetailsID;
                    entity.Prompt = TCVM.Prompt;
                    entity.TagCategoryValue = TCVM.TagCategoryName;

                    db.TagCategories.Add(entity);
                    db.SaveChanges();

                    if (entity.TagCategoryID > 0) {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                
            }

            //if (SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_INSERT_NEW_TAGCATEGORIES", TCVM.TagCategoryName) > 0)
            //{
            //    result = true;
            //}


            return result;

        }

        public bool DeleteTagcatagory(int TagCategoryID)
        {
            bool Result = false;
            try
            {
                int Val = SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_DeleteTagcatagoribyTagCatagoriesID", TagCategoryID);
                if (Val > 0)
                {
                    Result = true;
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagModel - DeleteTagcatagoribyTagID", Ex.Message);
            }

            return Result;
        }

        //public TagsCategoryViewModel GetTagCategoryDetails(int ID)
        //{
        //    TagsCategoryViewModel model = new TagsCategoryViewModel();
        //    DataSet DS = null;
        //    try
        //    {
        //        using (DS = new DataSet())
        //        {
        //            DS = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), "USP_GET_TAG_CATEGORY_DETAILS", ID);
        //            if (DS != null && DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0)
        //            {
        //                model.TagCategoryID = GetIntegerValue(DS.Tables[0].Rows[0]["CategoryID"]);
        //                model.TagCategoryName = GetStringValue(DS.Tables[0].Rows[0]["CategoryName"]);
        //            }
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        WriteLog("SDGApp.Models.TagModel - GetTagCategoryDetails", Ex.Message);
        //    }

        //    return model;
        //}

      

        //public int GetMeasurementByID(int ID)
        //{
        //    int MeasurementID = 0;
        //    DataSet DS = null;
        //    try
        //    {
        //        using (DS = new DataSet())
        //        {
        //            DS = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), "USP_GET_Measurement_ID_BY_UserActivityDetailsID", ID);
        //            if (DS != null && DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0)
        //            {
        //                MeasurementID = GetIntegerValue(DS.Tables[0].Rows[0]["MeasurementID"]);
        //            }
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        WriteLog("SDGApp.Models.TagModel - GetTagCategoryDetails", Ex.Message);
        //    }

        //    return MeasurementID;
        //}

        public List<TagsViewModel> GetTagListByTagCategoriesID(int ID)
        {
            List<TagsViewModel> _List = new List<TagsViewModel>();
            DataSet DS = null;
            try
            {
                using (DS = new DataSet())
                {
                    DS = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), "USP_GetTagsListByTagCateGoryID", ID);
                    if (DS != null && DS.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow DR in DS.Tables[0].Rows)
                        {
                            TagsViewModel TVM = new TagsViewModel();

                            TVM.TagsID = GetIntegerValue(DR["TagID"]);
                            TVM.TagName = GetStringValue(DR["TagName"]);
                           _List.Add(TVM);
                        }
                    }
                }
            }
            catch(Exception ex)
            { }
            return _List;
    }


    }

}