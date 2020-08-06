using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class UserOrganisationViewModel
    {
        public int ID { get; set; }

        public String OrganisationName { get; set; }

        public String UserCode { get; set; }

        public DateTime? JoiningDate { get; set; }
    }
}