namespace Athena.Infrastructure.Models
{
    public class Member
    {
        public int MemberId { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public int Role { get; set; }

        public User CorrespondingUser { get; set; }
        public Project CorrespondingProject { get; set; }
    }
}