using SDGApp.ViewModel;
using SDGAppDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.Models
{
    public class TagLabesModel : BaseModel
    {
        public bool AddNewLabel(TagLabelViewModel model)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    String Colorcode = RndColorCodeForTagLabel();

                    var entity = new SDGAppDB.POCO.TagLabel();
                    entity.LabelName = model.LabelName;
                    entity.MaxRange = model.MaxRange;
                    entity.MinRange = model.MinRange;
                    entity.DefaultValue = model.DefaultValue;
                    entity.PrecisionDigit = model.PrecisionDigit;
                    entity.ImageName = model.ImageName;
                    entity.UnitName = model.UnitName;
                    entity.FKTagLabelTypeID = model.FKTagLabelTypeID > 0 ? model.FKTagLabelTypeID : 1;//1- Normal
                    entity.CreatedDateTime = DateTime.Now;

                    if (model.UserID > 0)
                    {
                        entity.UserID = model.UserID;
                    }
                    else
                    {
                        entity.UserID = 0;
                    }


                    if (!String.IsNullOrEmpty(Colorcode))
                    {
                        entity.HasColorCode = Colorcode;
                    }

                    db.TagLabel.Add(entity);
                    db.SaveChanges();

                    if (entity.ID > 0)
                    {
                        model.ID = entity.ID;

                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagLabesModel - AddNewLabel", Ex.Message);
            }

            return Result;
        }

        public String RndColorCodeForTagLabel()
        {
            String Colorcode = String.Empty;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    for (int i = 0; i < 1; i++)
                    {
                        var random = new Random();
                        var hascolorcode = String.Format("#{0:X6}", random.Next(0x1000000)); // = "#0AA107"

                        var searchcolor = (from taglbl in db.TagLabel
                                           where taglbl.HasColorCode.ToLower() == hascolorcode.ToLower()
                                           select taglbl).FirstOrDefault();

                        if (searchcolor == null)
                        {
                            Colorcode = hascolorcode;
                            break;
                        }
                        else
                        {
                            i--;
                        }
                    }
                }

            }
            catch (Exception Ex)
            {

                WriteLog("SDGApp.Models.TagLabesModel - RndColorCodeForTagLabel", Ex.Message);
            }

            return Colorcode;
        }

        public bool DuplicateTagLabel(string labelname)
        {
            bool Result = false;
            TagLabelViewModel model = new TagLabelViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entityCount = (from taglabel in db.TagLabel
                                       where taglabel.LabelName.ToLower().Equals(labelname.ToLower())
                                       select new { taglabel }).Count();
                    if (entityCount != 0 && entityCount > 0)
                    {
                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagLabesModel - DuplicateTagLabel", Ex.Message);
            }
            return Result;
        }

        public TagLabelViewModel GetTagLabelDetailById(int labelid)
        {
            TagLabelViewModel model = new TagLabelViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = (from taglabel in db.TagLabel where taglabel.ID == labelid select new { taglabel }).FirstOrDefault();
                    if (entity != null)
                    {
                        model.ID = entity.taglabel.ID;
                        model.LabelName = entity.taglabel.LabelName;
                        model.MaxRange = entity.taglabel.MaxRange;
                        model.MinRange = entity.taglabel.MinRange;
                        model.DefaultValue = entity.taglabel.DefaultValue;
                        model.PrecisionDigit = entity.taglabel.PrecisionDigit;
                        model.UserID = entity.taglabel.UserID;
                        model.FKTagLabelTypeID = entity.taglabel.FKTagLabelTypeID;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagLabesModel - GetTagLabelDetailById", Ex.Message);
            }
            return model;
        }

        public bool UpdateTagLabelByID(TagLabelViewModel tvm)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.TagLabel.Find(tvm.ID);

                    if (entity != null && entity.ID > 0)
                    {
                        entity.LabelName = tvm.LabelName;
                        entity.MaxRange = tvm.MaxRange;
                        entity.MinRange = tvm.MinRange;
                        entity.DefaultValue = tvm.DefaultValue;
                        entity.PrecisionDigit = tvm.PrecisionDigit;
                        entity.ImageName = tvm.ImageName;
                        entity.UnitName = tvm.UnitName;
                        entity.FKTagLabelTypeID = tvm.FKTagLabelTypeID > 0 ? tvm.FKTagLabelTypeID : 1;//1- Normal


                        if (String.IsNullOrEmpty(entity.HasColorCode))
                        {
                            String Colorcode = RndColorCodeForTagLabel();
                            entity.HasColorCode = Colorcode;
                        }

                        db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagLabesModel - UpdateTagLabelByID", Ex.Message);
            }
            return Result;

        }

        public List<TagLabelViewModel> GetAllTagLabelList(int UserID, int PageNumber, int PageSize, DateTime? FromDate = null, DateTime? ToDate = null, int[] IDs = null, String UseingType = "web")
        {
            List<TagLabelViewModel> lstchunk = new List<TagLabelViewModel>();
            List<TagLabelViewModel> lst = new List<TagLabelViewModel>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (UseingType == "web")
                    {
                        lst = (from taglabel in db.TagLabel
                               where taglabel.UserID == UserID
                               || taglabel.UserID <= 0
                               select new TagLabelViewModel
                               {
                                   ID = taglabel.ID,
                                   LabelName = taglabel.LabelName,
                                   MaxRange = taglabel.MaxRange,
                                   MinRange = taglabel.MinRange,
                                   DefaultValue = taglabel.DefaultValue,
                                   PrecisionDigit = taglabel.PrecisionDigit,
                                   UnitName = taglabel.UnitName,
                                   ImageName = (string.IsNullOrEmpty(taglabel.ImageName) ? "" : taglabel.ImageName),
                                   UserID = taglabel.UserID,
                                   PageSize = PageSize,
                                   PageNumber = PageNumber,
                                   FKTagLabelTypeID = taglabel.FKTagLabelTypeID

                               }).ToList();
                    }
                    else if (UseingType == "api")
                    {
                        if (FromDate != null && ToDate != null)
                        {
                            DateTime newdate = ToDate.Value.AddDays(1);

                            lst = (from taglabel in db.TagLabel
                                   where taglabel.UserID == UserID
                                    && (taglabel.CreatedDateTime >= FromDate && taglabel.CreatedDateTime <= newdate)
                                   select new TagLabelViewModel
                                   {
                                       ID = taglabel.ID,
                                       LabelName = taglabel.LabelName,
                                       MaxRange = taglabel.MaxRange,
                                       MinRange = taglabel.MinRange,
                                       DefaultValue = taglabel.DefaultValue,
                                       PrecisionDigit = taglabel.PrecisionDigit,
                                       UnitName = taglabel.UnitName,
                                       ImageName = (string.IsNullOrEmpty(taglabel.ImageName) ? "" : GlobalConstants.BaseUrl + "/Content/images/TagLabel/" + taglabel.ImageName),
                                       UserID = taglabel.UserID,
                                       PageSize = PageSize,
                                       PageNumber = PageNumber,
                                       FKTagLabelTypeID = taglabel.FKTagLabelTypeID

                                   }).ToList();
                        }
                        else
                        {
                            lst = (from taglabel in db.TagLabel
                                   where taglabel.UserID == UserID
                                   select new TagLabelViewModel
                                   {
                                       ID = taglabel.ID,
                                       LabelName = taglabel.LabelName,
                                       MaxRange = taglabel.MaxRange,
                                       MinRange = taglabel.MinRange,
                                       DefaultValue = taglabel.DefaultValue,
                                       PrecisionDigit = taglabel.PrecisionDigit,
                                       UnitName = taglabel.UnitName,
                                       ImageName = (string.IsNullOrEmpty(taglabel.ImageName) ? "" : GlobalConstants.BaseUrl + "/Content/images/TagLabel/" + taglabel.ImageName),
                                       UserID = taglabel.UserID,
                                       PageSize = PageSize,
                                       PageNumber = PageNumber,
                                       FKTagLabelTypeID = taglabel.FKTagLabelTypeID

                                   }).ToList();
                        }
                            
                    }



                    if (lst.Count > 0)
                    {
                        if (IDs != null && IDs.Length > 0)
                        {
                            foreach (var item in IDs)
                            {
                                lst.Remove(lst.Single(s => s.ID == item)); // Remove IDs from List 
                            }

                        }
                        lstchunk = lst.OrderByDescending(q => q.ID).Skip(SkipRecords(PageSize, PageNumber)).Take(PageSize).ToList();
                        lstchunk.ForEach(l => l.TotalRecords = lst.Count());
                    }

                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagLabesModel - GetAllTagLabelList", Ex.Message);
            }
            return lstchunk;
        }

        public bool DeleteLabelByID(int TagLabelID)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.TagLabel.Find(TagLabelID);
                    if (entity != null)
                    {
                        //Remove Tags Master by Taglabel id 
                        var entityTagsmaster = db.TagsMaster.Where(x => x.FKTagLabelID == entity.ID).ToList();
                        if(entityTagsmaster!=null && entityTagsmaster.Count > 0)
                        {
                            db.TagsMaster.RemoveRange(entityTagsmaster);
                            db.SaveChanges();
                        }


                        //Remove Tag Label 
                        db.TagLabel.Remove(entity);
                        db.SaveChanges();

                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagLabesModel - DeleteLabelByID", Ex.Message);
            }
            return Result;
        }

        public List<SelectListItem> GetTagLabelTypeNameDDL()
        {
            List<SelectListItem> _list = new List<SelectListItem>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    _list = (from taglabel in db.TagLabelType
                             select new SelectListItem { Text = taglabel.TypeName, Value = taglabel.TagTypeID.ToString() }
                             ).ToList();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagLabesModel - GetTagLabelTypeNameDDL", Ex.Message);
            }

            return _list;
        }
    }
}