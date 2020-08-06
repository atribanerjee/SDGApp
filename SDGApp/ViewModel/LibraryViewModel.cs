using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SDGApp.ViewModel
{
    public class LibraryViewModel
    {
        public int WorkID { get; set; }

        [Required(ErrorMessage = "Enter Name")]
        [Display(Name = "Name")]
        public string WorkName { get; set; }

        [Required(ErrorMessage = "Enter Description")]
        [Display(Name = "Description")]
        public string WorkDescription { get; set; }

        [Display(Name = "Parent")]
        public int? WorkParentID { get; set; }
        public string ParentText { get; set; }
        public List<SelectListItem> DDLTopic { get; set; }

        public int UserID { get; set; }
        public int ParentCount { get; set; }
        public string WorkFlag { get; set; }
        [Display(Name = "Note")]
        public string Note { get; set; }
        public string FileName { get; set; }

        [Display(Name = "Order")]
        public int OrderID { get; set; }
        public string DocumentName { get; set; }
        public int FileID { get; set; }
        public string TopicWidth { get; set; }
        public string DisplayName { get; set; }

        public List<DocumentViewModel> LstFilesTask { get; set; }
        public List<DocumentViewModel> LstFilesSubTask { get; set; }

        public int TopicID { get; set; }
        public string TopicName { get; set; }
    }
}