using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Athena.Infrastructure.Entities.Enums;

namespace Athena.Infrastructure.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public int ProjectId { get; set; }

        public int? AssigneeId { get; set; }
        public int? StatusId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }


        [JsonIgnore]
        public Project CorrespondingProject { get; set; }

        [JsonIgnore]
        public TicketStatus Status { get; set; }

        [JsonIgnore]
        public User Assignee { get; set; }

        [JsonIgnore]
        public List<Task> Tasks { get; set; }

        [JsonIgnore]
        public List<Comment> Comments { get; set; }

        [JsonIgnore]
        public List<Observer> Observers { get; set; }
    }
}