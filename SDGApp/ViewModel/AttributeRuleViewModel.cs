using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.ViewModel
{
    public class AttributeRuleViewModel
    {
        public int AttributeRuleID { get; set; }

        public String AttributeLabel { get; set; }
        //    public List<SelectListItem> DDLAttributeRuleLabel { get; set; }
        public List<SelectListItem> DDLAttributeType { get; set; }
        public List<SelectListItem> DDLAttributeRuleType { get; set; }

      //  public int AttributeLabelID { get; set; }

        [Required(ErrorMessage = "Constraint rule type is required")]
        public int AttributeRuleTypeID { get; set; }

        [Required(ErrorMessage = "Constraint type is required")]
        public int? AttributeTypeID { get; set; }


   //     [Required(ErrorMessage = "Constraint label is required")]
       
  //      public String AllType { get; set; }

        public String ListAreaText { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        //  [Required(ErrorMessage = "Email is required")]
        //[DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        //[MaxLength(50)]
        //[RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter correct email")]
        public String Email { get; set; }

        /*[DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$")]*/
        public String Sms { get; set; }
        public string AttributeRuleValue { get; set; }


        //TO HOLD THE List  TEXT
        public Int32 TotalRecords { get; set; }

        public Int32 PageNumber { get; set; }

        public Int32 PageSize { get; set; }

       

    }
}