using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class MotionRecordsViewModel
    {
        public int ID { get; set; }

        [DisplayName("User Id")]
        [Required(ErrorMessage = "UserId is required")]
        public int userid { get; set; }
        public decimal motionCalorie { get; set; }
        public DateTime motionDate { get; set; }
        public decimal motionDistance { get; set; }
        public int motionSteps { get; set; }
        
    }
}