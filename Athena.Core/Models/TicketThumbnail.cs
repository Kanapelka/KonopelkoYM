using Athena.Infrastructure.Entities.Enums;

namespace Athena.Core.Models
{
    public class TicketThumbnail
    {
        public int TicketId { get; set; }
        public int ProjectId { get; set; }
        public int? AssigneeId { get; set; }

        public string AssigneeFirstName { get; set; }
        public string AssigneeLastName { get; set; }

        public string ProjectName { get; set; }
        public string TicketTitle { get; set; }

        public Priority Priority { get; set; }
        public int StatusId { get; set; }
        public string StatusDisplayed { get; set; }
    }
}