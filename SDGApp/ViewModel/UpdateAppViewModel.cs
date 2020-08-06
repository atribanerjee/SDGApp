using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class UpdateAppViewModel
    {
        public Int32 ID { get; set; }
                
        public int FKUserID { get; set; }
        public int FKAppTypeID { get; set; }         
        public String AppAndroidURL { get; set; }
        public String AppAndroidVersion { get; set; }
        public String AppiosURL { get; set; }
        public String AppiosVersion { get; set; }


        public bool? IsActive { get; set; }

        public bool? IsDelete { get; set; }


    }
}