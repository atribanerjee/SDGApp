using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("LibraryWorkDocument")]
    public class LibraryWorkDocument
    {
        [Key]
        public int ID { get; set; }
        public int FKWorkID { get; set; }
        public string DocumentName { get; set; }
        public string DisplayName { get; set; }
    }
}
