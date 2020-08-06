using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("Company")]
    public class Company
    {
        [Key]
        public Int32 CompanyID { get; set; }
        public String CompanyName { get; set; }
        public String CompanyAddress { get; set; }
        public String CompanyCode { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
