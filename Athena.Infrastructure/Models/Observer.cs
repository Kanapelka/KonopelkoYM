using System.Text.Json.Serialization;

namespace Athena.Infrastructure.Models
{
    public class Observer
    {
        public int ObserverId { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }

        [JsonIgnore]
        public Ticket TicketObserved { get; set; }

        [JsonIgnore]
        public User CorrespondingUser { get; set; }
    }
}