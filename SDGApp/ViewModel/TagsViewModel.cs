using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SDGApp.ViewModel
{
    public class TagsViewModel
    {
        public Int32 TagsID { get; set; }
        public Int32 TagDetailsID { get; set; }
        public Int32 FkUserID { get; set; }

        [DisplayName("Tag Name")]
        [Required(ErrorMessage = "Tag Name is required")]
        public String TagName { get; set; }

        public String TagcategoryName { get; set; }


        [DisplayName("Description")]
        public string Description { get; set; }


        [DisplayName("Prompt")]
        public String Prompt { get; set; }

        public String Min { get; set; }
        public String Max { get; set; }


        public String TypeName { get; set; }
        public Int32 TypeID { get; set; }

        public String Choice { get; set; }

        public String RadioButtonName { get; set; }
        public Int32 RadioButtonID { get; set; }


        public List<SelectListItem> TypeList { get; set; }
        public List<SelectListItem> RadioButtonList { get; set; }

        public Int32 TotalRecords { get; set; }

        public Int32 PageNumber { get; set; }

        public Int32 PageSize { get; set; }


        public String TagDscription { get; set; }
        public Int32 Distancecover { get; set; }
        public Int32 Speed { get; set; }
        public Int32 CategoryID { get; set; }
        public List<SelectListItem> TagCategoryList { get; set; }
        public bool IsActivity { get; set; }


    }
}