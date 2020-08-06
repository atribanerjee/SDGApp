using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("Unit")]
  public  class Unit
    {
        [Key]
        public int Id { get; set; }
        public String UnitName { get; set; }
    }
}
