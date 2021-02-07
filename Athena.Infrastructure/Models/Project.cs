using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Athena.Infrastructure.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public List<Member> Members { get; set; }

        [JsonIgnore]
        public List<Ticket> Tickets { get; set; }

        [JsonIgnore]
        public List<TicketStatus> CustomStatuses { get; set; }
    }
}