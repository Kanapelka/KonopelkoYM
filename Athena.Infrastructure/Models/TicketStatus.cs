using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Athena.Infrastructure.Models
{
    public class TicketStatus
    {
        public int TicketStatusId { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; }

        [JsonIgnore]
        public Project BelongsToProject { get; set; }

        [JsonIgnore]
        public List<Ticket> TicketsWithThisStatus { get; set; }
    }
}