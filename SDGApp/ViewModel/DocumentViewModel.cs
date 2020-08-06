using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SDGApp.ViewModel
{
    public class DocumentViewModel
    {
        public int FileID { get; set; }
        public int FKWorkID { get; set; }
        public string DisplayName { get; set; }

    }
}