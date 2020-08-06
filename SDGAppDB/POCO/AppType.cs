using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("AppType")]
    public class AppType
    {
        [Key]
        public int ID { get; set; } 
        public String AppTypeName { get; set; }
    }
}
