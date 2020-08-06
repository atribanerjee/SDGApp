using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class CareTeamViewModel
    {
        public int CarePeopleID { get; set; }
        public int CarePersonUserID { get; set; }
        public int RequestUserID { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public String Phone { get; set; }
        public String Address { get; set; }
        public String Status { get; set; }
        public Boolean IsViewed { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean IsDeleted { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public String CreatedDateTimeStamp { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}