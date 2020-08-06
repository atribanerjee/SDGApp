using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class RoleViewModel
    {
        public int RoleId { get; set; }
        public String RoleName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public Int32 TotalRecords { get; set; }

        public Int32 PageNumber { get; set; }

        public Int32 PageSize { get; set; }
    }
}