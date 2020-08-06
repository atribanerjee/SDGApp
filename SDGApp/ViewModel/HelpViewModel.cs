using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.ViewModel
{
    public class HelpViewModel
    {
        public Int32 HelpID { get; set; }

        [Required(ErrorMessage = "Select Help Topic")]
        [Display(Name = "Help Topic")]
        public int FKTopicID { get; set; }
        public List<SelectListItem> DDLTopic { get; set; }
        public String Topic { get; set; }


        [Required(ErrorMessage = "Enter Help Text")]
        [Display(Name = "Help Text")]
        public String HelpText { get; set; }
        public Int32 FromRow { get; set; }
        public Int32 ToRow { get; set; }
        public Int32 PageNumber { get; set; }
        public Int32 PageSize { get; set; }
        public Int32 TotalRecords { get; set; }
        public String SearchNamevalue { get; set; }

        public class JsonModel
        {
            public string HTMLString { get; set; }
            public string HTMLString2 { get; set; }
            public bool NoMoreData { get; set; }
            public Int32 BlockNo { get; set; }
        }
    }
}