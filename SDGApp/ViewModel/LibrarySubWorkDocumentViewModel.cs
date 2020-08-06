using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class LibrarySubWorkDocumentViewModel
    {
        public Int32 ID { get; set; }
        public Int32 FKWorkID { get; set; }
        public String DocumentName { get; set; }
        public String DisplayName { get; set; }
    }
}