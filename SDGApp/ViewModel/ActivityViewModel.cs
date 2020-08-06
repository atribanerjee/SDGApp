using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class ActivityViewModel
    {
        public List<PlannedActivities> PlannedActivityList { get; set; }

        public List<calendarEvents> CalendarEvents { get; set; }
        public List<calendarEvents> TagCategoryList { get; set; }
        public Int32 CategoryID { get; set; }
    }

    public class PlannedActivities
    {
        public int PlannedActivitiesID { get; set; }

        [DisplayName("Activities Name")]
        [Required(ErrorMessage = "Activities Name is required")]
        public String PlannedActivitiesName { get; set; }

        [DisplayName("Date Time")]
        [Required(ErrorMessage = "Time is required")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public DateTime DefaultDateTime { get; set; }

        public Int32 TotalRecords { get; set; }

        public Int32 PageNumber { get; set; }

        public Int32 PageSize { get; set; }
       

    }

    public class calendarEvents
    {
        public int id { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string url { get; set; }
    }
}