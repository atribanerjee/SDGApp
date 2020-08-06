using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("User")]
    public class User
    {
        [Key]
        public Int32 UserID { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
       
       
        public String Zip { get; set; }
        public String Address { get; set; }

        public String Email { get; set; }
        public String SecurityNo { get; set; }
        public Boolean IsActive { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }

        public String GuID { get; set; }
        public bool GuIDIsActive { get; set; }

        public String Mobile { get; set; }
     

        public String Company { get; set; }
        public String Facebook { get; set; }

        public String Twitter { get; set; }
        public String GooglePlus { get; set; }
        public String Flickr { get; set; }
        public String Youtube { get; set; }

        public String Phone { get; set; }
        public String Skype { get; set; }
        public String Gender { get; set; }
        public decimal? Height { get; set; }

        public decimal? Weight { get; set; }
        public String Race { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DOB { get; set; }
        public String Geolocation { get; set; }
        public String Picture { get; set; }

       
        public String ResearchersName { get; set; }
        public String ResearchersPicture { get; set; }
        public String ResearchersInstitution { get; set; }
        public String ResearchersNotes { get; set; }
        public String SkinType { get; set; }


        [ForeignKey("StateDetail")]
        public Int32? FKStateID { get; set; }
        public StateDetail StateDetail { get; set; }

        [ForeignKey("CityDetail")]
        public int? FKCityID { get; set; }
        public CityDetail CityDetail { get; set; }

        [ForeignKey("Country")]
        public int? FKCountryID { get; set; }
        public Country Country { get; set; }


        [ForeignKey("UserType")]
        public int fkUserType { get; set; }
        public UserType UserType { get; set; }

        [ForeignKey("Unit")]
        public int? fkUnit { get; set; }
        public Unit Unit { get; set; }

        public String CityName { get; set; }

        public DateTime? CreateDateTime { get; set; }
    }
}
