using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Athena.Infrastructure.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string EmailAddress { get; set; }
        public string Password { get; set; }

        public bool IsActive { get; set; }

        public string Location { get; set; }
        public string JobTitle { get; set; }

        [JsonIgnore]
        public List<Member> TeamMemberOf { get; set; }

        [JsonIgnore]
        public List<Ticket> TicketsAssigned { get; set; }

        [JsonIgnore]
        public List<Observer> Observes { get; set; }

        [JsonIgnore]
        public List<Notification> Notifications { get; set; }
    }
}