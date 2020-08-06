using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("CompanyUserRole")]
  public  class CompanyUserRole
    {
        [Key]
        public Int32 CompanyUserRoleID { get; set; }
        public bool IsDefault { get; set; }

        [ForeignKey("Company")]
        public Int32 FKCompanyID { get; set; }
        public Company Company { get; set; }

        [ForeignKey("User")]
        public Int32 FKUserID { get; set; }
        public User User { get; set; }

        [ForeignKey("Role")]
        public Int32 FKRoleID { get; set; }
        public Role Role { get; set; }

        public String UserCode { get; set; }
        public DateTime? JoiningDate { get; set; }
    }
}
