using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.ViewModel
{
    public class DashboardViewModel
    {
        public int ID { get; set; }
        public int FkUserID { get; set; }
       
        public List<WidgetDetails> ListWidgetDetails { get; set; }

    }

    public class WidgetDetails
    {
        public String WidgetName { get; set; }
        public String TabName { get; set; }
        public Decimal Height { get; set; }
        public Decimal Width { get; set; }
        public int Position { get; set; }
    }

   
}