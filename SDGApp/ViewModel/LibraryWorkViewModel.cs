using System;
using System.Collections.Generic;

namespace SDGApp.ViewModel
{
    public class LibraryWorkViewModel
    {
        public Int32 WorkID { get; set; }
        public String WorkName { get; set; }
        public String WorkDescription { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean IsDelete { get; set; }
        public Int32 FKCompanyID { get; set; }
        public Int32 FKTopicID { get; set; }
        public String Note { get; set; }
        public Int32 OrderID { get; set; }

        public List<LibraryWorkDocumentViewModel> TaskDocuments { get; set; }

        public List<LibrarySubWorkViewModel> SubTasks { get; set; }

        public Boolean IsSelected { get; set; }
    }
}