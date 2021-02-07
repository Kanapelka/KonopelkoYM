namespace Athena.Core.Models
{
    public class ProjectThumbnail
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public int TicketsCount { get; set; }
    }
}