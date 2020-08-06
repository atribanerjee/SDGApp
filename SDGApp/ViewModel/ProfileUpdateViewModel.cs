using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.ViewModel
{
    public class ProfileUpdateViewModel
    {

        
        public int userId { get; set; }
        

        [DisplayName("First Name")]
        //[Required(ErrorMessage = "First Name is required")]
        [StringLength(50, MinimumLength = 3)]
        public String firstName { get; set; }

        [DisplayName("Last Name")]
        //[Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, MinimumLength = 3)]
        public String lastName { get; set; }

        [DisplayName("User Name")]
        //[Required(ErrorMessage = "User Name is required")]
        [StringLength(50, MinimumLength = 3)]
        public String userName { get; set; }

        //[Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        [MaxLength(50)]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter correct email")]
        public String Email { get; set; }

        public String Country { get; set; }
        public int CountryID { get; set; }

        //[DisplayName("State")]
        //[Required(ErrorMessage = "State Name is required")]
        public int State { get; set; }

        public String CityName { get; set; }
        
        //[DisplayName("Zip")]
        //[Required(ErrorMessage = "Zip Name is required")]
        public String Zip { get; set; }

        public String Institution { get; set; }

        public int UserRoleID { get; set; }

        public int UserTypeID { get; set; }
        public String UserType { get; set; }

        public String gender { get; set; }

        [DisplayName("Address")]
        [Required(ErrorMessage = "Address is required minimum 5 and maximum 50")]
        [StringLength(50, MinimumLength = 5)]
        public String Address { get; set; }

        public String Phone { get; set; }

        public String MobileNo { get; set; }

        public String Picture { get; set; }

        public String dateOfBirth { get; set; }

        public String height { get; set; }

        public String weight { get; set; }

        public String GeoLocation { get; set; }

        public String skin { get; set; }

        public String UserRole { get; set; }
                
        public int unit { get; set; }

    }


}


