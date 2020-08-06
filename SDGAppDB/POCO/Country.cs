using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("Country")]
  public  class Country
    {
        [Key]
        public int CountryID { get; set; }
        public String CountryName { get; set; }
    }
}
