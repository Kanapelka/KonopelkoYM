namespace Athena.Core.Models
{
    public class UserProfile
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public bool IsActive { get; set; }

        public string Location { get; set; }
        public string JobTitle { get; set; }
    }
}