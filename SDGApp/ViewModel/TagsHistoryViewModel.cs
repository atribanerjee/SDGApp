using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SDGApp.ViewModel
{
    public class TagsHistoryViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Type")]
        [Required(ErrorMessage = "Tag Type is required")]
        public int FKTagLabelID { get; set; }

        public List<SelectListItem> DDLTagLabelType { get; set; }


        [DisplayName("Tag Value")]
        [Required(ErrorMessage = "Tag Value is required")]
        public int TagValue { get; set; }

        [DisplayName("Tag Note")]
        [Required(ErrorMessage = "Note is required")]
        public string Note { get; set; }

        public int FKUserID { get; set; }

        public String TypeName { get; set; }


        public Int32 TotalRecords { get; set; }

        public Int32 PageNumber { get; set; }

        public Int32 PageSize { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public int hdnTagValue { get; set; }

        public String CreatedDateTimeTimestamp { get; set; }

        public String HasColorCode { get; set; }
    }
}