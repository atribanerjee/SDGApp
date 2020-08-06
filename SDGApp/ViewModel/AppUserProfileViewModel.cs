namespace SDGApp.ViewModel
{
    public class AppUserProfileViewModel
    {

        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public int UnitID { get; set; }

        public string DateOfBirth { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string SkinType { get; set; }
        public string UserImage { get; set; }

        public bool IsUpdate { get; set; }
    }
}