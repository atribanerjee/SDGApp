using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.ViewModel
{
    public class TagsCategoryViewModel
    {
        public Int32 TagID { get; set; }
        public String TagCategoryName { get; set; }

        public List<SelectListItem> TagDDL { get; set; }

        public List<SelectListItem> TagCategoryDDL { get; set; }

        public int Min { get; set; }

        public int Max { get; set; }

        public String Choice { get; set; }

        public int TagDetailsID { get; set; }

        public int TagTypeID { get; set; }

        public String TagName { get; set; }

        public string Description { get; set; }
        
        public Int32 TotalRecords { get; set; }

        public Int32 PageNumber { get; set; }

        public Int32 PageSize { get; set; }

        public int FKUserId { get; set; }

        public String Prompt { get; set; }

    }
   
}