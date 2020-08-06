using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class LibraryHierarchyViewModel
    {
        public Int32 TopicID { get; set; }
        public String TopicName { get; set; }
        public String TopicDescription { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean IsDelete { get; set; }
        public Int32 FKCompanyID { get; set; }
        public Int32 OrderID { get; set; }
        public String TopicWidth { get; set; }
        public List<LibraryWorkViewModel> TaskList { get; set; }

        public Boolean IsSelected { get; set; }
    }
}