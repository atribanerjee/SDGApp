using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("UserFeedBack")]
   public class UserFeedBack
    {
        [Key]
        public Int32 UserFeedbackID { get; set; }
        public String FeedbackContent { get; set; }
        public decimal FeedbackRating { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("HelpModule")]
        public Int32 FKHelpMOduleID { get; set; }
        public HelpModule HelpModule { get; set; }


        [ForeignKey("User")]
        public Int32 FKUserID { get; set; }
        public User User { get; set; }


        [ForeignKey("Company")]
        public Int32 FKCompanyID { get; set; }
        public Company Company { get; set; }
    }
}
