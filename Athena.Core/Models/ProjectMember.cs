namespace Athena.Core.Models
{
    public class ProjectMember
    {
        public int UserId { get; set; }
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string JobTitle { get; set; }
        public string Location { get; set; }

        public int ProjectId { get; set; }
        public int ProjectRole { get; set; }
    }
}