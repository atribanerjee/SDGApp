using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.ViewModel
{
    public class FeedbackViewModel
    {
        public Int32 ID { get; set; }

        [Required(ErrorMessage = "Select Module")]
        [Display(Name = "Module")]
        public int FKHelpModuleID { get; set; }
        public List<SelectListItem> DDLTopic { get; set; }
        public string Topic { get; set; }
        public int FKUserID { get; set; }

        public String UserFullName { get; set; }

        //[Required(ErrorMessage = "Enter Rating")]
        //[Display(Name = "Rating")]
        //public decimal FeedbackRating { get; set; }

        [Required(ErrorMessage = "Enter Feedback")]
        [Display(Name = "Feedback")]
        public String FeedbackContent { get; set; }

        public Int32 PageNumber { get; set; }
        public Int32 PageSize { get; set; }
        public Int32 TotalRecords { get; set; }
        public string SearchNamevalue { get; set; }

        public int? FKCompanyID { get; set; }
    }
}