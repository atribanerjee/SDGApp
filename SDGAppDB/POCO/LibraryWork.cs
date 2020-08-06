using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("LibraryWork")]
    public class LibraryWork
    {
        [Key]
        public int WorkID { get; set; }
        public string WorkName { get; set; }
        public string WorkDescription { get; set; }
        public int WorkParentID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int FKCompanyID { get; set; }
        public int FKTopicID { get; set; }
        public string Note { get; set; }
        public string DocumentName { get; set; }
        public int OrderID { get; set; }
    }
}
