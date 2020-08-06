using SDGApp.ViewModel;
using SDGAppDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.Models
{
    public class AttributeRuleModel : BaseModel
    {
        public List<SelectListItem> GetAttributeRuleType()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    list = (from att in db.AttributeRuleType
                            select new SelectListItem { Text = att.AttributeRuleTypeText, Value = att.AttributeRuleTypeID.ToString() }).ToList();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.AttributeRuleModel - GetAttributeRuleType", Ex.Message);
            }
            return list;
        }

        //FOR ATTRIBUTE TYPE
        public List<SelectListItem> GetAttributeType()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    list = (from att in db.AttributeType
                            select new SelectListItem { Text = att.AttributeTypeName, Value = att.AttribteTypeID.ToString() }).ToList();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.AttributeRuleModel - GetAttributeType", Ex.Message);
            }
            return list;
        }

        // Save Attribte Rule

        public bool SaveAttributeRule(AttributeRuleViewModel model)
        {
            bool Result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entitylabel = new SDGAppDB.POCO.AttributeRuleLabel();
                    entitylabel.AttributeRuleLabelText = model.AttributeLabel;

                    db.AttributeRuleLabel.Add(entitylabel);
                    db.SaveChanges();

                    var entityrule = new SDGAppDB.POCO.AttributeRule();
                    entityrule.FKRuleLabelID = entitylabel.AttributeRuleLabelID;
                    entityrule.FKRuleTypeID = model.AttributeRuleTypeID;
                    entityrule.FKAttributeTypeID =GetIntegerValue(model.AttributeTypeID);
                    if(model.AttributeRuleTypeID==1)
                    {
                        entityrule.AttributeRuleValue = model.ListAreaText;
                    }
                   
                    if (model.AttributeRuleTypeID == 2)
                    {
                        entityrule.AttributeRuleValue =GetStringValue(model.MinValue).Trim()+","+ GetStringValue(model.MaxValue).Trim();
                    }
                    if (model.AttributeRuleTypeID == 3)
                    {
                        entityrule.AttributeRuleValue = model.Email;
                    }
                    if (model.AttributeRuleTypeID == 4)
                    {
                        entityrule.AttributeRuleValue = model.Sms;
                    }

                    db.AttributeRule.Add(entityrule);
                    db.SaveChanges();
                    Result = true;

                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.SaveAttributeRule - GetAttributeType", Ex.Message);
            }


            return Result;
        }

        // List of Attribute Rule

        public List<AttributeRuleViewModel> GetAttributeRuleList(int pageSize, int pageNumber)
        {
            List<AttributeRuleViewModel> lst = new List<AttributeRuleViewModel>();
            List<AttributeRuleViewModel> lstchunk = new List<AttributeRuleViewModel>();
            //int cid = UM.GetLoggedInUserInfo().CompanyID;
            //int UserID = UM.GetLoggedInUserInfo().UserID;

            try
            {

                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    lst = (from at in db.AttributeRule
                           join al in db.AttributeRuleLabel on at.FKRuleLabelID equals al.AttributeRuleLabelID
                           join aty in db.AttributeRuleType on at.FKRuleTypeID equals aty.AttributeRuleTypeID
                           select new AttributeRuleViewModel
                           {
                               AttributeRuleID = at.AttribteRuleID,
                               AttributeLabel = al.AttributeRuleLabelText,
                               AttributeTypeID = at.FKAttributeTypeID,
                               AttributeRuleTypeID=aty.AttributeRuleTypeID,
                              
                               PageSize = pageSize,
                               PageNumber = pageNumber
                           }).ToList();

                    lstchunk = lst.OrderByDescending(q => q.AttributeRuleID).Skip(SkipRecords(pageSize, pageNumber)).Take(pageSize).ToList();
                    lstchunk.ForEach(l => l.TotalRecords = lst.Count());
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.AttributeRuleModel - GetAttributeRuleList", Ex.Message);
            }
            return lstchunk;
        }

        public bool DeleteAttributeRulebyID(int AttributeRuleID)
        {
            bool result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.AttributeRule.Find(AttributeRuleID);
                    var LabelEntity = db.AttributeRuleLabel.Find(entity.FKRuleLabelID);

                    if (entity != null && entity.AttribteRuleID > 0)
                    {
                        db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                        db.SaveChanges();
                        result = true;

                        db.Entry(LabelEntity).State = System.Data.Entity.EntityState.Deleted;
                        db.SaveChanges();

                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeleteAttributeRulebyID - DeleteAttributeRulebyID", Ex.Message);
            }
            return result;
        }

        // EDit Attribute Rule

        public AttributeRuleViewModel GetforEditAttributeRulebyID(int AttributeRuleID)
        {
            AttributeRuleViewModel arvm = new AttributeRuleViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    arvm = (from at in db.AttributeRule
                            join al in db.AttributeRuleLabel on at.FKRuleLabelID equals al.AttributeRuleLabelID
                            join aty in db.AttributeRuleType on at.FKRuleTypeID equals aty.AttributeRuleTypeID
                            where at.AttribteRuleID == AttributeRuleID
                            select new AttributeRuleViewModel
                            {
                                AttributeRuleID = at.AttribteRuleID,
                                AttributeLabel = al.AttributeRuleLabelText,
                                AttributeRuleTypeID = at.FKRuleTypeID,
                                AttributeRuleValue = at.AttributeRuleValue,
                                AttributeTypeID = at.FKAttributeTypeID
                            }).FirstOrDefault();

                    arvm.DDLAttributeRuleType = GetAttributeRuleType();
                    arvm.DDLAttributeType = GetAttributeType();
                    if (arvm.AttributeRuleTypeID == 1)
                    {
                        arvm.ListAreaText = arvm.AttributeRuleValue;
                    }
                    if (arvm.AttributeRuleTypeID == 2)
                    {
                        arvm.MinValue = GetIntegerValue(arvm.AttributeRuleValue.Split(',')[0].Trim());
                        arvm.MaxValue = GetIntegerValue(arvm.AttributeRuleValue.Split(',')[1].Trim());
                    }
                    if (arvm.AttributeRuleTypeID == 3)
                    {
                        arvm.Email = arvm.AttributeRuleValue;
                    }
                    if (arvm.AttributeRuleTypeID == 4)
                    {
                        arvm.Sms = arvm.AttributeRuleValue;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.AttributeRuleModel - GetforEditAttributeRulebyID", Ex.Message);
            }
            return arvm;
        }


        public bool UpdateAttributeRule(AttributeRuleViewModel model)
            {
            bool result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.AttributeRule.Find(model.AttributeRuleID);
                    if (model.AttributeRuleTypeID == 1)
                    {
                        entity.AttributeRuleValue = model.ListAreaText;
                    }

                   if (model.AttributeRuleTypeID == 2)
                    {
                        entity.AttributeRuleValue = GetStringValue(model.MinValue).Trim() + "," + GetStringValue(model.MaxValue).Trim();
                    }
                    if (model.AttributeRuleTypeID == 3)
                    {
                        entity.AttributeRuleValue = model.Email;
                    }
                    if (model.AttributeRuleTypeID == 4)
                    {
                        entity.AttributeRuleValue = model.Sms;
                    }

                    var entityAttrLabel = db.AttributeRuleLabel.Find(entity.FKRuleLabelID);
                    entityAttrLabel.AttributeRuleLabelText = model.AttributeLabel;
                    db.Entry(entityAttrLabel).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    entity.FKRuleTypeID = model.AttributeRuleTypeID;
                    entity.FKAttributeTypeID =GetIntegerValue(model.AttributeTypeID);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                result = true;

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UpdateAttributeRule - UpdateAttributeRule", Ex.Message);
            }
            return result;
        }

        //End

        public List<SelectListItem> GetAttributeLabelByAttrType(int EntityType)
        {
            List<SelectListItem> list = new List<SelectListItem>();
          //  int CompanyID = UM.GetLoggedInUserInfo().CompanyID;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    list = (from ar in db.AttributeRule
                            join arl in db.AttributeRuleLabel on ar.FKRuleLabelID equals arl.AttributeRuleLabelID
                            where ar.FKAttributeTypeID == EntityType 
                            select new SelectListItem { Text = arl.AttributeRuleLabelText, Value = arl.AttributeRuleLabelID.ToString() }).ToList();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.AttributeRuleModel - GetAttributeLabelByAttrType", Ex.Message);
            }
            return list;
        }

        public AttributeRuleViewModel GetAttrLabelValue(int AttLabelId)
        {
            AttributeRuleViewModel list = new AttributeRuleViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    list = (from att in db.AttributeRule
                            where att.FKRuleLabelID == AttLabelId
                            select new AttributeRuleViewModel
                            {
                                AttributeTypeID = att.FKAttributeTypeID,
                                AttributeRuleValue = att.AttributeRuleValue,
                                AttributeRuleTypeID=att.FKRuleTypeID,
                            }).FirstOrDefault();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.AttributeRuleModel - GetAttributeRuleType", Ex.Message);
            }
            return list;
        }

    }
}