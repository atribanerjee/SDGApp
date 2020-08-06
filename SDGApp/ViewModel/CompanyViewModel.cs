using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class CompanyViewModel
    {
        public Int32 CompanyID { get; set; }

        [Required(ErrorMessage = "Enter Company name")]
        [Display(Name = "Company Name")]
        [MaxLength(50)]
        public String CompanyName { get; set; }

        [Required(ErrorMessage = "Enter Company address")]
        [Display(Name = "Company Address")]
        [MaxLength(1000)]
        public String CompanyAddress { get; set; }

        public DateTime CreatedOn { get; set; }


        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public Int32 TotalRecords { get; set; }

        public Int32 PageNumber { get; set; }

        public Int32 PageSize { get; set; }

        public String CompanyCode { get; set; }

    }
}