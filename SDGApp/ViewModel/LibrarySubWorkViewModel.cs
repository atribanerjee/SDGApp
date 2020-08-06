using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class LibrarySubWorkViewModel
    {
        public Int32 WorkID { get; set; }
        public String WorkName { get; set; }
        public String WorkDescription { get; set; }
        public Int32 WorkParentID { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean IsDelete { get; set; }
        public Int32 FKCompanyID { get; set; }
        public Int32 FKTopicID { get; set; }
        public String Note { get; set; }
        public Int32 OrderID { get; set; }

        public List<LibrarySubWorkDocumentViewModel> SubTaskDocuments { get; set; }
    }
}