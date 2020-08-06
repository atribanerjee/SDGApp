using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SDGApp.ViewModel
{
    public class TagLabelViewModel
    {
        public int ID { get; set; }

        [DisplayName("Tag Label")]
        [Required(ErrorMessage = "Tag Label is required")]
        public string LabelName { get; set; }

        [DisplayName("Tag Min Range")]
        [Required(ErrorMessage = "Tag Min Range is required ")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Tag Min must be a number")]
        public int MinRange { get; set; }

        [DisplayName("Tag Max Range")]
        [Required(ErrorMessage = "Tag Max Range is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Tag Max must be a number")]
        public int MaxRange { get; set; }

        //[RegularExpression("([1-9][0-9]*)", ErrorMessage = "DefaultValue must be a number")]
        public int DefaultValue { get; set; }

        //[RegularExpression("([1-9][0-9]*)", ErrorMessage = "PrecisionDigit must be a number")]
        public int PrecisionDigit { get; set; }


                
        public Int32 TotalRecords { get; set; }

        public Int32 PageNumber { get; set; }

        public Int32 PageSize { get; set; }

        public string UnitName { get; set; }

        public string ImageName { get; set; }

        public int UserID { get; set; }

        [DisplayName("Select Label Type")]
        [Required(ErrorMessage = "Label Type is required")]
        public int FKTagLabelTypeID { get; set; }
        
        public DateTime CreatedDateTime { get; set; }
    }
}