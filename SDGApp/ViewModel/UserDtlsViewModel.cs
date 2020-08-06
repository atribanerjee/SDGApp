using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.ViewModel
{
    public class UserDtlsViewModel
    {
        public Int64 id { get; set; }
        public int UserID { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String FullName { get; set; }
        public String UserName { get; set; }
        public String UserImage { get; set; }
        public Boolean IsActive { get; set; }

    }

   


}