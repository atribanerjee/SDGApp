using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class SyncMessageViewModel
    {
        
        public int UserID { get; set; }

        public int MessageTypeID { get; set; }

        public int GetDataType { get; set; }

        public List<UserMessageViewModel> MessageList { get; set; }

        public String ErrorMessage { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

    }
}