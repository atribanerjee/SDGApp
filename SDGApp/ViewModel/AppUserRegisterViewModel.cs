using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class AppUserRegisterViewModel
    {
        public int Userid { get; set; }

        [DisplayName("First Name")]
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }


        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }


        [DisplayName("User Name")]
        [Required(ErrorMessage = "User Name is required")]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        [MaxLength(50)]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter correct email")]
        public string Email { get; set; }
        public string Phone { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(25, MinimumLength = 5)]
        public string Password { get; set; }

        //[DisplayName("Confirm Password")]
        //[StringLength(25, ErrorMessage = "Must be between 5 and 25 characters", MinimumLength = 5)]
        //[DataType(DataType.Password)]
        //[System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Confirmation password do not match.")]

        //public string ConfirmPassword { get; set; }



    }
}