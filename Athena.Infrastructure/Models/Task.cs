using System.Text.Json.Serialization;

namespace Athena.Infrastructure.Models
{
    public class Task
    {
        public int TaskId { get; set; }
        public int TicketId { get; set; }

        public string Title { get; set; }
        public bool Done { get; set; }

        [JsonIgnore]
        public Ticket CorrespondingTicket { get; set; }
    }
}