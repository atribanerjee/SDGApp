using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SDGApp.ViewModel
{
    public class TagsHistServiceViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Type")]
        [Required(ErrorMessage = "Tag Type is required")]
        public int FKTagLabelID { get; set; }

        public List<SelectListItem> DDLTagLabelType { get; set; }

        public string TagLabelName { get; set; }

        [DisplayName("Tag Value")]
        [Required(ErrorMessage = "Tag Value is required")]
        public int value { get; set; }

        [DisplayName("Tag Note")]
        public string note { get; set; }

        public int userId { get; set; }

        public string type { get; set; }

        
        public string date { get; set; }

        public string time { get; set; }

        public List<Tag> tags { get; set; }

    }

    public class Tag
    {
        public string userId { get; set; }
        public string type { get; set; }
        public string value { get; set; }
        public string time { get; set; }
        public string date { get; set; }
        public string notes { get; set; }
        public string image { get; set; }
    }
}