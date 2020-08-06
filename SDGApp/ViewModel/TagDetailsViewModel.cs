using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.ViewModel
{
    public class TagDetailsViewModel
    {
        public Int32 MeasurementID { get; set; }
        public String TagCategoryName { get; set; }
       
        public string MeasurementValue { get; set; }
        public int TagID { get; set; }
        public String TagName { get; set; }
        public int TagCategoryID { get; set; }
        public String TAgCategoryName { get; set; }
        public int TagTypeID { get; set; }
        public bool IsActivity { get; set; }
        public string Description { get; set; }
        public List<SelectListItem> TagDetailTypeList { get; set; }
        public List<SelectListItem> TagCategoryTypeList { get; set; }
        public List<Field> Fields { get; set; }
        public int TagDetailsID { get; set; }
        public int FKTagTypeID { get; set; }
        public string Prompt { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public string Choice { get; set; }
       
    }

    public class Field
    {
        public int TagDetailsID { get; set; }
        public int FKTagTypeID { get; set; }
        public string Prompt { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public string Choice { get; set; }
    }
}