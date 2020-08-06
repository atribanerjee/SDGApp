using System;

namespace SDGApp.ViewModel
{
    public class LibraryWorkDocumentViewModel
    {
        public Int32 ID { get; set; }
        public Int32 FKWorkID { get; set; }
        public String DocumentName { get; set; }
        public String DisplayName { get; set; }
    }
}