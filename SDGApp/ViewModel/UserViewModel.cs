using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.ViewModel
{
    public class UserViewModel
    {
        public Int64 id { get; set; }
        public int UserID { get; set; }
        public int DeviceID { get; set; }

        [DisplayName("First Name")]
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, MinimumLength = 2)]
        public String FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, MinimumLength = 2)]
        public String LastName { get; set; }

        [DisplayName("State")]
        [Required(ErrorMessage = "State Name is required")]
        public int State { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        [MaxLength(50)]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter correct email")]
        public String Email { get; set; }

        public String SecurityNo { get; set; }

        public bool IsActive { get; set; }

        [DisplayName("User Name")]
        [Required(ErrorMessage = "User Name is required")]
        [StringLength(50, MinimumLength = 3)]
        public String UserName { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(25, MinimumLength = 5)]
        public String Password { get; set; }

        [DisplayName("New Password")]
        [Required(ErrorMessage = "New Password is required")]
        [StringLength(25, ErrorMessage = "Must be between 5 and 25 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public String NewPassword { get; set; }

        [DisplayName("Confirm Password")]
        [StringLength(25, ErrorMessage = "Must be between 5 and 25 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Confirmation password do not match.")]
        public String ConfirmPassword { get; set; }

        public bool RoleisActive { get; set; }

        public String GuID { get; set; }
        public bool GuIDIsActive { get; set; }

        public String Country { get; set; }

        public int CountryID { get; set; }

        public String Picture { get; set; }

        public String StateName { get; set; }
        public String CityName { get; set; }
        public String MobileNo { get; set; }

        public String Company { get; set; }
        public String Phone { get; set; }
       
        public String Gender { get; set; }

        public String Height { get; set; }
               
        public String Weight { get; set; }
        public String Race { get; set; }
        public bool IsDelete { get; set; }

        public bool RememberMe { get; set; }


        public String Institution { get; set; }
        public String NotesHere { get; set; }

        public String UserRole { get; set; }
        public int UserRoleID { get; set; }

        public String DateOfBirth { get; set; }
        public String GeoLocation { get; set; }

        public int UserTypeID { get; set; }
        public String UserType { get; set; }

        public String ResearchName { get; set; }
        public String SkinType { get; set; }

        public List<SelectListItem> UserSkinTypeList { get; set;}
        public List<SelectListItem> UserTypeList { get; set; }


        public List<SelectListItem> StateNameList { get; set; }

        public List<SelectListItem> CountryNameList { get; set; }
        public List<SelectListItem> GenderNameList { get; set; }
        public List<SelectListItem> UserRoleList { get; set; }
        public List<SelectListItem> UserCompanyList { get; set; }

        //    public JsonResult BloodReport { get; set; }

        public Int32 TotalRecords { get; set; }

        public Int32 PageNumber { get; set; }

        public Int32 PageSize { get; set; }

        // Property gor blood report

        public String UUID { get; set; }
        public String BPSystolic { get; set; }
        public String BPDiastolic { get; set; }
        public String HeartRate { get; set; }
        public String PuseTransitTime { get; set; }
        public String SDNN { get; set; }

        public String RMSDD { get; set; }
        public String RRIntervals { get; set; }
        public String CreatedDateTime { get; set; }
       
        [Display(Name = "Company")]
        public Int32 CompanyID { get; set; }

        public List<SelectListItem> DDLCompany { get; set; }

        public String[] txtAttrLabel { get; set; }
        public String[] txtAttrValue { get; set; }
        public int[] AttributeRuleLabelID { get; set; }
        public int[] AttributeRuleTypeID { get; set; }
        public string[] AttributeRuleValue { get; set; }

        public List<SelectListItem> DDLAttributeLabel { get; set; }

        public List<SelectListItem> UnitNameList { get; set; }
        public int UnitNameID { get; set; }

        public DateTime CreateDateTime { get; set; }

        public String ReturnUrl { get; set; }
        public String SearchValue { get; set; }


    }

    public class State
    {
        public int ID { get; set; }
        public string StateName { get; set; }
    }

    public class City
    {
        public int ID { get; set; }
        public string CityName { get; set; }
    }


}